using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        [BindProperty]
        public int ind { get; set; }
        private readonly LDSContext context;
        public LogInModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet()
        {
            ind = 0;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Korisnik = context.Korisnici.Where(x => x.Username == KorisnickoIme && x.Password == Sifra).FirstOrDefault();
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