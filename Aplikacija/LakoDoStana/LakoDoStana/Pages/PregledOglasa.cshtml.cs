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
using MongoDB.Driver;
using System.Collections;

namespace LakoDoStana.Pages
{
    public class PregledOglasaModel : PageModel
    {
        
        public Korisnik LogovaniKorisnik { get; set; }

        public List<Korisnik> SviKorisnici;

        public List<Oglas> ListaOglasa { get; set; }

        [BindProperty]
        public Oglas Oglas { get; set; }

        [BindProperty]
        public Korisnik PostavioOglas { get; set; }

        [BindProperty(Name = "iD", SupportsGet = true)]
        public int iD { get; set; }

        [BindProperty(Name = "oglasiD", SupportsGet = true)]
        public int oglasiD { get; set; }

        public List<string> slike { get; set; }

        private readonly IMongoCollection<Korisnik> _korisnici;

        public PregledOglasaModel(ILDSDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _korisnici = database.GetCollection<Korisnik>(settings.LDSCollectionName);
        }

        public async Task OnGet(string username, string oglasiD)
        {
            LogovaniKorisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Username == username).FirstOrDefault();
            SviKorisnici = _korisnici.Find(korisnik => true).ToList();

            foreach(Korisnik k in SviKorisnici)
            {
                Korisnik temp_Korisnik = _korisnici.AsQueryable<Korisnik>().Where(x => x.Id == k.Id).Include(x => x.Oglasi).FirstOrDefault();
                foreach(Oglas o in temp_Korisnik.Oglasi)
                {
                    if(o.Id == oglasiD)
                    {
                        PostavioOglas = temp_Korisnik;
                        Oglas = o;
                    }
                }
            }

            slike = new List<string>();
            string[] putanje;
            if (Directory.Exists(@"wwwroot\Pictures\" + Oglas.Id))
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures\" + Oglas.Id);
                foreach (string s in putanje)
                {
                    string pom = Path.GetFileName(s);
                    slike.Add("Pictures/" + Oglas.Id + "/" + pom);
                }
            }
            else
            {
                putanje = Directory.GetFiles(@"wwwroot\Pictures");
                string pom = Path.GetFileName(putanje.First());
                slike.Add("Pictures/" + pom);
            }
            await Promeni();
        }

        public async Task Promeni()
        {
            PostavioOglas.Oglasi.Remove(Oglas);
            Oglas.BrojPregleda++;
            PostavioOglas.Oglasi.Add(Oglas);
            PostavioOglas.BrojUkupnihPregleda++;


            try
            {
                _korisnici.ReplaceOne(x => x.Id == PostavioOglas.Id, PostavioOglas);
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
    }
}