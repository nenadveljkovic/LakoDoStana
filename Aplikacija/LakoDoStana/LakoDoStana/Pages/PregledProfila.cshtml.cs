using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace LakoDoStana.Pages
{
    public class PregledProfilaModel : PageModel
    {
        [BindProperty]
        public Korisnik LogovaniKorisnik { get; set; }

        public Korisnik Korisnik { get; set; }

        [BindProperty(Name = "iD", SupportsGet = true)]
        public string iD { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;

        public PregledProfilaModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }
        
        public void OnGet(string iD)
        {           
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Id == iD).Include(x => x.Oglasi).FirstOrDefault();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            Korisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Id == iD).Include(x => x.Oglasi).FirstOrDefault();
            if (!ModelState.IsValid)
            {
                return new EmptyResult();
            }
            else
            {
                Korisnik.Ime = LogovaniKorisnik.Ime;
                Korisnik.Prezime = LogovaniKorisnik.Prezime;
                Korisnik.Email = LogovaniKorisnik.Email;
                Korisnik.DatumRodjenja = LogovaniKorisnik.DatumRodjenja;
                Korisnik.Username = LogovaniKorisnik.Username;
                Korisnik.Pol = LogovaniKorisnik.Pol;
                try
                {
                    _korisnici.ReplaceOne(x => x.Id == Korisnik.Id, Korisnik);
                }
                catch (DbUpdateException ex)
                {
                    throw new Exception("Greška pri radu sa bazom!\n" + ex.Message);
                }
                catch (Exception exe)
                {
                    throw new Exception("Greška!" + exe.Message);
                }
                return RedirectToPage(new { iD = LogovaniKorisnik.Id });
            }
        }
        
    }
}