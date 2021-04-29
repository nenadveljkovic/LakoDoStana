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
using MongoDB.Bson;
using MongoDB.Driver;

namespace LakoDoStana.Pages
{
    public class NoviOglasModel : PageModel
    {
        
        public Korisnik LogovaniKorisnik { get; set; }
        
        [BindProperty]
        public Oglas Oglas { get; set; }
        
        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }
        
        [BindProperty]
        public string TipObjekta { get; set; }
        [BindProperty]
        public string TipOglasa { get; set; }

        [BindProperty]
        public List<IFormFile> files { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;
        public NoviOglasModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }
        
        public void OnGet(string  username)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();                      
        }
        
        public async Task<IActionResult> OnPostPostaviAsync(string username)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();
            if (LogovaniKorisnik.Oglasi == null)
                LogovaniKorisnik.Oglasi = new List<Oglas>();
            //kreiranje oglasa

            Oglas.Id = ObjectId.GenerateNewId().ToString();
            Oglas.BrojPregleda = 0;
            Oglas.DatumObjavljivanja = Convert.ToDateTime(DateTime.Now.Date.ToString("F"));
            if (TipObjekta == "Kuća")
                Oglas.TipObjekta = 0;
            else
                Oglas.TipObjekta = 1;
            if (TipOglasa == "Traži se cimer")
                Oglas.TipOglasa = 0;
            else
                Oglas.TipOglasa = 1;
           
            //Ucitavanje slika
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
            if(files.Count == 0)
            {
                string[] filePaths = Directory.GetFiles(@"wwwroot\Pictures");
                foreach (var filename in filePaths)
                {
                    string file = filename.ToString();
                    string str = "wwwroot/Pictures/" + Oglas.Id + "/" + "NotFound.png";
                    Directory.CreateDirectory(Path.GetDirectoryName(str));
                    if(!System.IO.File.Exists(str))
                        System.IO.File.Copy(file, str);
                }
            }            
            try
            {
                LogovaniKorisnik.Oglasi.Add(Oglas);
                LogovaniKorisnik.BrojPostavljenihOglasa++;
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
            return RedirectToPage("/PregledOglasa", new { username = LogovaniKorisnik.Username, oglasiD = Oglas.Id });
        }
       
    }
}