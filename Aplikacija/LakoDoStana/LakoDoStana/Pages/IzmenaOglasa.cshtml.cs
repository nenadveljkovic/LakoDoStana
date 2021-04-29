using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Collections;

namespace LakoDoStana.Pages
{
    public class IzmenaOglasaModel : PageModel
    {
        
        public Korisnik LogovaniKorisnik { get; set; }

        [BindProperty]
        public Oglas Oglas { get; set; }

        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }
        [BindProperty(Name = "oglasid", SupportsGet = true)]
        public int oglasid { get; set; }


        public List<string> slike;

        [BindProperty]
        public List<IFormFile> files { get; set; }
        [BindProperty]
        public string TipObjekta { get; set; }
        [BindProperty]
        public string TipOglasa { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;

        public IzmenaOglasaModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }

        public void OnGet(string username, string oglasid)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();
            foreach(Oglas o in LogovaniKorisnik.Oglasi)
            {
                if (o.Id == oglasid)
                    Oglas = o;
            }

            slike = new List<string>();
            string[] putanje;
            if (Directory.Exists(@"wwwroot\Pictures\" + Oglas.Id))
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures\" + Oglas.Id);
                foreach (string s in putanje)
                {
                    string pom = Path.GetFileName(s);
                    slike.Add("Pictures/" + Oglas.Id + "/" + pom);
                }
            }
            else
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures");
                string pom = Path.GetFileName(putanje.First());
                slike.Add("Pictures/" + pom);
            }
        }

        public async Task<IActionResult> OnPostPostaviAsync(string username, string oglasid)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();

            //Ucitavanje slika
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string filePath = "wwwroot/Pictures/" + Oglas.Id + "/" + formFile.FileName;
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        try
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            throw new Exception("Greška pri radu sa direktorijumima!\n" + ex.Message);
                        }
                        catch (Exception exe)
                        {
                            throw new Exception("Greška1" + exe.Message);
                        }
                    }
                }
            }


            if (TipObjekta == "Kuća")
                Oglas.TipObjekta = 0;
            else
                Oglas.TipObjekta = 1;
            if (TipOglasa == "Traži se cimer")
                Oglas.TipOglasa = 0;
            else
                Oglas.TipOglasa = 1;

            foreach (Oglas o in LogovaniKorisnik.Oglasi.ToList())
            {
                if (o.Id == oglasid)
                    LogovaniKorisnik.Oglasi.Remove(o);
            }
            Oglas.Id = oglasid;
            LogovaniKorisnik.Oglasi.Add(Oglas);
            try
            {
                _korisnici.ReplaceOne(x => x.Id == LogovaniKorisnik.Id, LogovaniKorisnik);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Greška pri radu sa bazom!\n" + ex.Message);
            }
            catch (Exception exe)
            {
                throw new Exception("Greška!" + exe.Message);
            }
            return RedirectToPage("/PregledSvojihOglasa", new { username = LogovaniKorisnik.Username });
        }
    }
}