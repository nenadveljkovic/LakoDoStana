using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LakoDoStana.Pages
{
    public class PregledProfilaModel : PageModel
    {
        public Korisnik LogovaniKorisnik { get; set; }

        [BindProperty]
        public Posetilac Posetilac { get; set; }

        [BindProperty]
        public Oglasivac Oglasivac { get; set; }

        [BindProperty]
        public Administrator Admin { get; set; }

        [BindProperty(Name = "iD", SupportsGet = true)]
        public int iD { get; set; }
        private readonly LDSContext context;

        public List<Poruka> poruke;

        public PregledProfilaModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet(int iD)
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.ID == iD).FirstOrDefault();
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
            if (LogovaniKorisnik is Models.Posetilac)
            {
                Posetilac = context.Posetioci.Find(LogovaniKorisnik.ID);
            }
            if (LogovaniKorisnik is Models.Oglasivac)
            {
                Oglasivac = context.Oglasivaci.Find(LogovaniKorisnik.ID);
            }
            if (LogovaniKorisnik is Models.Administrator)
            {
                Admin = context.Administratori.Find(LogovaniKorisnik.ID);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            poruke = new List<Poruka>();
            while (LogovaniKorisnik == null)
                LogovaniKorisnik = context.Korisnici.Where(x => x.ID == iD).FirstOrDefault();
            poruke = context.Poruke.Where(x => x.PrimalacId == LogovaniKorisnik.ID).Include(x => x.Posiljalac).ToList();
            poruke.Reverse();
            if (!ModelState.IsValid)
            {
                return new EmptyResult();
            }
            else
            {
                if (LogovaniKorisnik is Models.Posetilac)
                {
                    LogovaniKorisnik.Ime = Posetilac.Ime;
                    LogovaniKorisnik.Prezime = Posetilac.Prezime;
                    LogovaniKorisnik.Username = Posetilac.Username;
                    LogovaniKorisnik.Pol = Posetilac.Pol;
                    LogovaniKorisnik.Email = Posetilac.Email;
                    LogovaniKorisnik.DatumRodjenja = Posetilac.DatumRodjenja;
                }
                if (LogovaniKorisnik is Models.Oglasivac)
                {
                    LogovaniKorisnik.Ime = Oglasivac.Ime;
                    LogovaniKorisnik.Prezime = Oglasivac.Prezime;
                    LogovaniKorisnik.Username = Oglasivac.Username;
                    LogovaniKorisnik.Pol = Oglasivac.Pol;
                    LogovaniKorisnik.Email = Oglasivac.Email;
                    LogovaniKorisnik.DatumRodjenja = Oglasivac.DatumRodjenja;
                }
                if (LogovaniKorisnik is Models.Administrator)
                {
                    LogovaniKorisnik.Ime = Admin.Ime;
                    LogovaniKorisnik.Prezime = Admin.Prezime;
                    LogovaniKorisnik.Username = Admin.Username;
                    LogovaniKorisnik.Pol = Admin.Pol;
                    LogovaniKorisnik.Email = Admin.Email;
                    LogovaniKorisnik.DatumRodjenja = Admin.DatumRodjenja;
                }
                try
                {
                    context.Korisnici.Update(LogovaniKorisnik);
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
                return RedirectToPage(new { iD = LogovaniKorisnik.ID });
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
    }
}