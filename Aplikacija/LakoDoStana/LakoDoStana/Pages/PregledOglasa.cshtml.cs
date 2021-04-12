using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LakoDoStana.Pages
{
    public class PregledOglasaModel : PageModel
    {
        public Korisnik LogovaniKorisnik { get; set; }

        [BindProperty]
        public Oglas Oglas { get; set; }

        [BindProperty]
        public Oglasivac PostavioOglas { get; set; }

        [BindProperty(Name = "iD", SupportsGet = true)]
        public int iD { get; set; }

        [BindProperty(Name = "oglasiD", SupportsGet = true)]
        public int oglasiD { get; set; }

        public readonly LDSContext context;

        [BindProperty]
        public string TextPoruke { get; set; }

        public List<Poruka> poruke;

        public List<string> slike;

        public PregledOglasaModel(LDSContext con)
        {
            context = con;
        }
        public async Task OnGet(int iD, int oglasiD)
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.ID == iD).FirstOrDefault();
            Oglas = context.Oglasi.Where(x => x.OglasId == oglasiD).FirstOrDefault();
            KorisnikOglas ko = context.KorisniciOglasi.Where(x => x.OglasId == oglasiD && x.TipVeze == "Postavio").Include(x=>x.Korisnik).FirstOrDefault();
            PostavioOglas = new Oglasivac(ko.Korisnik);
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();

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
                slike.Add("Pictures/" + pom);
            }
            await Promeni();
            TextPoruke = "";
        }

        public async Task Promeni()
        {
            Oglas.BrojPregleda++;
            if (LogovaniKorisnik is Posetilac)
            {
                var postoji = context.KorisniciOglasi.Where(x => x.KorisnikId == LogovaniKorisnik.ID && x.OglasId == Oglas.OglasId && x.TipVeze == "Pregledao").FirstOrDefault();
                if (postoji == null)
                {
                    context.KorisniciOglasi.Add(new KorisnikOglas { KorisnikId = LogovaniKorisnik.ID, OglasId = Oglas.OglasId, Datum = DateTime.Now, TipVeze = "Pregledao" });
                    Posetilac posetilac = context.Posetioci.Find(LogovaniKorisnik.ID);
                    posetilac.BrojPregledanihOglasa += 1;

                    //context.Posetioci.Update(Posetilac);
                    context.Attach(posetilac).State = EntityState.Modified;
                }
            }
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
        }

        public string VratiTipOb()
        {
            if (Oglas.TipObjekta == 1)
                return "Kuća";
            else
                return "Stan";
        }

        public string VratiTipOg()
        {
            if (Oglas.TipOglasa == 0)
                return "Traži se cimer.";
            else
                return "Traže se stanari.";
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.ID == iD).FirstOrDefault();
            Oglas = context.Oglasi.Where(x => x.OglasId == oglasiD).FirstOrDefault();
            KorisnikOglas ko = context.KorisniciOglasi.Where(x => x.OglasId == oglasiD && x.TipVeze == "Postavio").Include(x => x.Korisnik).FirstOrDefault();
            PostavioOglas = new Oglasivac(ko.Korisnik);
            poruke = new List<Poruka>();
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
            slike = new List<string>();
            if (Directory.Exists(@"wwwroot\Pictures\" + Oglas.OglasId))
            {
                string[] putanje = Directory.GetFiles(@"wwwroot\Pictures\" + Oglas.OglasId);
                foreach (string s in putanje)
                {
                    string pom = Path.GetFileName(s);
                    slike.Add("Pictures/" + Oglas.OglasId + "/" + pom);
                }
            }
            else
            {
                slike.Add("Pictures/NotFound.png");
            }
            Poruka P = new Poruka();
            if (PostavioOglas.ID != LogovaniKorisnik.ID)
            {
                P.PosiljalacId = LogovaniKorisnik.ID;
                P.Tekst = TextPoruke;
                P.DatumSlanja = Convert.ToDateTime(DateTime.Today.ToString("F"));
                P.Seen = false;
                P.PrimalacId = PostavioOglas.ID;
                context.Poruke.Add(P);
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
            return Page();
        }
    }
}