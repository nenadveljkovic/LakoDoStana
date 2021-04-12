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
    public class NoviOglasModel : PageModel
    {
        public Korisnik LogovaniKorisnik { get; set; }
        public Oglasivac Oglasivac { get; set; }
        [BindProperty]
        public Oglas Oglas { get; set; }
        private readonly LDSContext context;
        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }
        [BindProperty]
        public List<IFormFile> Slike { get; set; }
        public List<Poruka> poruke;
        [BindProperty]
        public string TipObjekta { get; set; }
        [BindProperty]
        public string TipOglasa { get; set; }

        [BindProperty]
        public List<IFormFile> files { get; set; }
        public NoviOglasModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet(string  username)
        {
            poruke = new List<Poruka>();
            while(LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == username).FirstOrDefault();
            Oglasivac = context.Oglasivaci.Find(LogovaniKorisnik.ID);
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
        }
        
        public async Task<IActionResult> OnPostPostaviAsync(string username)
        {
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == username).FirstOrDefault();
            Oglasivac = context.Oglasivaci.Find(LogovaniKorisnik.ID);

            //kreiranje oglasa
            Oglas.BrojPregleda = 0;
            Oglas.DatumObjavljivanja = Convert.ToDateTime(DateTime.Now.ToString("F"));
            if (TipObjekta == "Kuća")
                Oglas.TipObjekta = 0;
            else
                Oglas.TipObjekta = 1;
            if (TipOglasa == "Traži se cimer")
                Oglas.TipOglasa = 0;
            else
                Oglas.TipOglasa = 1;

            int idzaoglas;
            if (context.Oglasi.Any())
            {
                idzaoglas = context.Oglasi.OrderByDescending(x => x.OglasId).Select(x => x.OglasId).First();
                idzaoglas++;
            }
            else
                idzaoglas = 1;
            Oglas.OglasId = idzaoglas;
            context.KorisniciOglasi.Add(new KorisnikOglas { KorisnikId = Oglasivac.ID, OglasId = Oglas.OglasId, Datum = DateTime.Now, TipVeze = "Postavio" });
            Oglasivac.BrojPostavljenihOglasa++;
            context.Oglasivaci.Update(Oglasivac);
            //Ucitavanje slika
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string filePath = "wwwroot/Pictures/" + idzaoglas + "/" + formFile.FileName;
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
                    string str = "wwwroot/Pictures/" + idzaoglas + "/" + "NotFound.png";
                    Directory.CreateDirectory(Path.GetDirectoryName(str));
                    if(!System.IO.File.Exists(str))
                        System.IO.File.Copy(file, str);
                }
            }
            context.Oglasi.Add(Oglas);           
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
            return RedirectToPage("/PregledOglasa", new { iD = LogovaniKorisnik.ID, oglasiD = Oglas.OglasId });
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