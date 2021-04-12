using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LakoDoStana.Pages
{
    public class PregledSvojihOglasaModel : PageModel
    {
        public Korisnik LogovaniKorisnik { get; set; }
        public List<KorisnikOglas> lista;
        public List<Oglas> ListaOglasa;
        private readonly LDSContext context;
        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }

        public List<Poruka> poruke;
        public PregledSvojihOglasaModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet(string username)
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == username).FirstOrDefault();
            lista = context.KorisniciOglasi.Where(x => x.KorisnikId == LogovaniKorisnik.ID && x.TipVeze == "Postavio").Include(x=>x.Oglas).ToList();
            if(lista != null)
            {
                ListaOglasa = new List<Oglas>();
                foreach (KorisnikOglas ko in lista)
                    ListaOglasa.Add(ko.Oglas);
            }
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
        }
        public async Task<IActionResult> OnGetObrisiAsync(int id, string Username)
        {
            var oglas = await context.Oglasi.FindAsync(id);
            if (oglas != null)
            {
                DirectoryInfo di = new DirectoryInfo(@"wwwroot\Pictures\" + oglas.OglasId);
                foreach(FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(@"wwwroot\Pictures\" + oglas.OglasId);
                List<KorisnikOglas> vezanozaoglas = await context.KorisniciOglasi.Where(x => x.OglasId == oglas.OglasId).ToListAsync();
                if (vezanozaoglas != null)
                    foreach (KorisnikOglas ko in vezanozaoglas)
                        context.KorisniciOglasi.Remove(ko);
                context.Oglasi.Remove(oglas);
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
            }
            return base.RedirectToPage(new { username = Username });
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