using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LakoDoStana.Pages
{
    public class PocetnaZaKorisnikaModel : PageModel
    {
        public Korisnik LogovaniKorisnik;

        public List<Oglas> ListaOglasa;

        [BindProperty(Name = "username", SupportsGet = true)]
        public string Username { get; set; }
        private readonly LDSContext context;
        public PocetnaZaKorisnikaModel(LDSContext con)
        {
            context = con;
        }

        public List<Poruka> poruke;

        public string[] slike;

        public void OnGet()
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.Username == Username).FirstOrDefault();
            ListaOglasa = context.Oglasi.ToList();
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();

            slike = new string[100];
            for (int t = 0; t < 100; t++)
            {
                string[] putanje = Directory.GetFiles(@"wwwroot\Pictures");
                string pom = Path.GetFileName(putanje.First());
                slike[t] = ("Pictures/" + pom);
            }
            foreach (Oglas o in ListaOglasa)
            {
                string[] putanje;
                if (Directory.Exists(@"wwwroot\Pictures\" + o.OglasId))
                {
                    putanje = Directory.GetFiles(@"wwwroot\Pictures\" + o.OglasId);
                    {
                        if (putanje.Count() != 0)
                        {
                            string pom = Path.GetFileName(putanje.First());
                            slike[o.OglasId] = ("Pictures/" + o.OglasId + "/" + pom);
                        }
                        else
                        {
                            putanje = Directory.GetFiles(@"wwwroot\Pictures");
                            string pom = Path.GetFileName(putanje.First());
                            slike[o.OglasId] = ("Pictures/" + pom);
                        }
                    }
                }
                else
                {
                    putanje = Directory.GetFiles(@"wwwroot\Pictures");
                    string pom = Path.GetFileName(putanje.First());
                    slike[o.OglasId] = ("Pictures/" + pom);
                }
            }
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

        public string VratiTipObjekta(Oglas o)
        {
            if (o.TipObjekta == 0)
                return "Kuća";
            else
                return "Stan";
        }

        public string VratiDatum(Oglas o)
        {
            return o.DatumObjavljivanja.ToString("dd/MM/yy");
        }

        /*public string VratiUsername(Oglas o)
        {
            KorisnikOglas ko = context.KorisniciOglasi.Where(x => x.OglasId == o.OglasId && x.TipVeze == "Postavio").FirstOrDefault();
            Korisnik k = context.Korisnici.Where(x => x.ID == ko.KorisnikId).FirstOrDefault();
            return k.Username;
        }*/

        public string VratiTipOglasa(Oglas o)
        {
            if (o.TipOglasa == 0)
                return "Traži se cimer!";
            else
                return "Traže se stanari!";
        }
    }
}