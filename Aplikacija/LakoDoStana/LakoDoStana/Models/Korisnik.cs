using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace LakoDoStana.Models
{
    public class Korisnik
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public IList<Oglas> Oglasi { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Ime { get; set; }
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Prezime { get; set; }
        [Required]
        public string Email { get; set; }
        [BsonElement]
        [BsonDateTimeOptions(DateOnly = true)]
        [Required]
        public DateTime DatumRodjenja { get; set; }
        [BsonElement]
        [BsonDateTimeOptions(DateOnly = true)]
        [Required]
        public DateTime DatumKreiranjaNaloga { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Username { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        public char Pol { get; set; }

        public int BrojPostavljenihOglasa { get; set; }
        public int BrojUkupnihPregleda { get; set; }

        public Korisnik()
        {

        }

    }
}
