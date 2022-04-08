using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public Usuario Usuario { get; set; }
        public Object EntidadeAntes { get; set; }
        public Object EntidadeDepois { get; set; }
        public string Operacao { get; set; }
        public DateTime Data { get; set; }
    }
}
