using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace LakoDoStana.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Korisnik Korisnik { get; set; }
  
        public SelectList ListaUserName { get; set; }
        private readonly IMongoCollection<Korisnik> _korisnici;
       
        public RegisterModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }
        public void OnGet()
        {
            IQueryable<string> lista = _korisnici.AsQueryable<Korisnik>().Select(x => x.Username);
            ListaUserName = new SelectList(lista.ToList());
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                Korisnik.BrojPostavljenihOglasa = 0;
                Korisnik.BrojUkupnihPregleda = 0;
                Korisnik.DatumKreiranjaNaloga = Convert.ToDateTime(DateTime.Today.ToString("F"));
                try
                {
                    _korisnici.InsertOne(Korisnik);
                }
                catch (DbUpdateException ex)
                {
                    throw new Exception("Greška pri radu sa bazom!\n" + ex.Message);
                }
                catch (Exception exe)
                {
                    throw new Exception("Greška!" + exe.Message);
                }

                return RedirectToPage("/PocetnaZaKorisnika", new { username = Korisnik.Username });
            }
        }
    }
}