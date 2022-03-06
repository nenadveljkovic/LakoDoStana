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
    public class LogInModel : PageModel
    {
        
        [BindProperty]
        public string KorisnickoIme { get; set; }
        [BindProperty]
        public string Sifra { get; set; }
        public Korisnik Korisnik;
        public bool Greska = false;

        private readonly IMongoCollection<Korisnik> _korisnici;

        public LogInModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }
        
        public void OnGet()
        {
            
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Korisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == KorisnickoIme && x.Password == Sifra).FirstOrDefault();
            if (Korisnik == null)
            {
                Greska = true;
                return Page();
            }
            else
                return RedirectToPage("/PocetnaZaKorisnika", new { username = Korisnik.Username});
        }
        
    }
}