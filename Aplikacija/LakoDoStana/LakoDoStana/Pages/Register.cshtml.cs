using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LakoDoStana.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Posetilac Posetilac { get; set; }
        public Oglasivac Oglasivac;

        public Korisnik LogovaniKorisnik;
  
        [BindProperty]
        public string TipNaloga { get; set; }
        public SelectList ListaUserName { get; set; }
        private readonly LDSContext context;

        [BindProperty(Name = "adminid", SupportsGet = true)]
        public int Adminid { get; set; }
        public RegisterModel(LDSContext con)
        {
            context = con;
        }
        public void OnGet()
        {
            IQueryable<string> lista = context.Korisnici.Select(x => x.Username);
            ListaUserName = new SelectList(lista.ToList());
        }
        public async Task<IActionResult> OnPostAsync()
        {
            string adminusername="";
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                if (Adminid != 0)
                {
                    Korisnik k = context.Korisnici.Where(x => x.ID == Adminid).FirstOrDefault();
                    adminusername = k.Username;
                }
                Posetilac.DatumKreiranjaNaloga = Convert.ToDateTime(DateTime.Today.ToString("F"));
                if (TipNaloga == "oglasivac")
                {
                    Oglasivac = new Oglasivac(Posetilac);
                    Oglasivac.BrojPostavljenihOglasa = 0;
                    Oglasivac.BrojUkupnihPregleda = 0;
                    Oglasivac.DatumKreiranjaNaloga = Convert.ToDateTime(DateTime.Today.ToString("F"));
                    context.Korisnici.Add(Oglasivac);
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
                else
                {
                    Posetilac.BrojPregledanihOglasa = 0;
                    Posetilac.DatumKreiranjaNaloga = Convert.ToDateTime(DateTime.Today.ToString("F"));
                    context.Korisnici.Add(Posetilac);
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
                if (adminusername == "")
                    return RedirectToPage("/PocetnaZaKorisnika", new { username = Posetilac.Username });
                else
                    return RedirectToPage("/PocetnaZaKorisnika", new { username = adminusername });
            }
        }
    }
}