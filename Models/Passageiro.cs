using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Passageiro : Pessoa
    {
        public string CodPassaporte { get; set; }
    }
}
