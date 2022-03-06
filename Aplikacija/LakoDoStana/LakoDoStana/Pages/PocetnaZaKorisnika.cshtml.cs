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
using MongoDB.Driver;
using System.Collections;

namespace LakoDoStana.Pages
{
    public class PocetnaZaKorisnikaModel : PageModel
    {
        
        public Korisnik LogovaniKorisnik;

        public List<IList<Oglas>> SviOglasi;

        public List<Oglas> ListaOglasa { get; set; }

        [BindProperty(Name = "username", SupportsGet = true)]
        public string username { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;
        public PocetnaZaKorisnikaModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }

        public Dictionary<string, string> slike { get; set; }
        
        public void OnGet()
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).FirstOrDefault();
            SviOglasi = _korisnici.AsQueryable<Korisnik>().Select(x => x.Oglasi).ToList();
            ListaOglasa = new List<Oglas>();
            slike = new Dictionary<string, string>();
            foreach (IList<Oglas> l in SviOglasi)
                if(l != null)
                foreach (Oglas o in l)
                    ListaOglasa.Add(o);

            foreach (Oglas o in ListaOglasa)
            {
                string[] putanje;
                if (Directory.Exists(@"wwwroot\Pictures\" + o.Id))
                {
                    putanje = Directory.GetFiles(@"wwwroot\Pictures\" + o.Id);
                    {
                        if (putanje.Count() != 0)
                        {
                            string pom = Path.GetFileName(putanje.First());
                            slike.Add(o.Id, "Pictures/" + o.Id + "/" + pom);
                        }
                        else
                        {
                            putanje = Directory.GetFiles(@"wwwroot\Pictures");
                            string pom = Path.GetFileName(putanje.First());
                            slike.Add(o.Id, "Pictures/" + pom);
                        }
                    }
                }
                else
                {
                    putanje = Directory.GetFiles(@"wwwroot\Pictures");
                    string pom = Path.GetFileName(putanje.First());
                    slike.Add(o.Id, "Pictures/" + pom);
                }
            }
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
        
        public string VratiTipOglasa(Oglas o)
        {
            if (o.TipOglasa == 0)
                return "Traži se cimer!";
            else
                return "Traže se stanari!";
        }
        
    }
}