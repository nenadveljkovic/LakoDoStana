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

namespace LakoDoStana.Pages
{
    public class IzmenaOglasaModel : PageModel
    {
        public Korisnik LogovaniKorisnik { get; set; }
        public Oglasivac Oglasivac { get; set; }
        [BindProperty]
        public Oglas Oglas { get; set; }
        private readonly LDSContext context;
        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }
        [BindProperty(Name = "oglasid", SupportsGet = true)]
        public int oglasid { get; set; }

        public List<Poruka> poruke;

        public List<string> slike;

        [BindProperty]
        public List<IFormFile> files { get; set; }
        [BindProperty]
        public string TipObjekta { get; set; }
        [BindProperty]
        public string TipOglasa { get; set; }
        public IzmenaOglasaModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet(string username,int oglasid)
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == username).FirstOrDefault();
            Oglasivac = context.Oglasivaci.Find(LogovaniKorisnik.ID);
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
            Oglas = context.Oglasi.Find(oglasid);

            slike = new List<string>();
            string[] putanje;
            if (Directory.Exists(@"wwwroot\Pictures\" + Oglas.OglasId))
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures\" + Oglas.OglasId);
                foreach (string s in putanje)
                {
                    string pom = Path.GetFileName(s);
                    slike.Add("Pictures/" + Oglas.OglasId + "/" + pom);
                }
            }
            else
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures");
                string pom = Path.GetFileName(putanje.First());
                slike[Oglas.OglasId] = ("Pictures/" + pom);
            }
        }

        public async Task<IActionResult> OnPostPostaviAsync(string username)
        {
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == username).FirstOrDefault();
            Oglasivac = context.Oglasivaci.Find(LogovaniKorisnik.ID);

            //Ucitavanje slika
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string filePath = "wwwroot/Pictures/" + Oglas.OglasId + "/" + formFile.FileName;
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

            context.Oglasi.Update(Oglas);
            try
            {
                await context.SaveChangesAsync();
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
        public string IzracunajVreme(Poruka p)
        {
            int sekunde = Convert.ToInt32((Convert.ToDateTime(DateTime.Today.ToString("F")) - Convert.ToDateTime(p.DatumSlanja.ToString("F"))).TotalSeconds);
            int minuti = sekunde / 60;
            int sati = minuti / 60;
            int dani = sati / 24;
            if (dani != 0)
                return dani + "d ago";
            else
                return "Today";
        }
    }
}