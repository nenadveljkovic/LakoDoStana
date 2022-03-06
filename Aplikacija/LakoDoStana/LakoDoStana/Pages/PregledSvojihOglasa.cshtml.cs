using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LakoDoStana.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace LakoDoStana.Pages
{
    public class PregledSvojihOglasaModel : PageModel
    { 
        
        public Korisnik LogovaniKorisnik { get; set; }
        
        public List<Oglas> ListaOglasa;
        
        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;
        public PregledSvojihOglasaModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }
    
        public void OnGet(string username)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();
        }
        
        public async Task<IActionResult> OnGetObrisiAsync(string id, string username)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).Include(x => x.Oglasi).FirstOrDefault();
            int brojpregleda = 0;
            foreach (Oglas o in LogovaniKorisnik.Oglasi.ToList())
                if (o.Id == id)
                {
                    brojpregleda = o.BrojPregleda;
                    LogovaniKorisnik.Oglasi.Remove(o);
                }
            
            DirectoryInfo di = new DirectoryInfo(@"wwwroot\Pictures\" + id);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Directory.Delete(@"wwwroot\Pictures\" + id);

            try
            {
                LogovaniKorisnik.BrojPostavljenihOglasa--;
                LogovaniKorisnik.BrojUkupnihPregleda -= brojpregleda; 
                _korisnici.ReplaceOne(x => x.Id == LogovaniKorisnik.Id, LogovaniKorisnik);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Greška pri radu sa bazom!\n" + ex.Message);
            }
            catch (Exception exe)
            {
                throw new Exception("Greška!" + exe.Message);
            }
            return base.RedirectToPage(new { username = username });
        }
        
    }
}