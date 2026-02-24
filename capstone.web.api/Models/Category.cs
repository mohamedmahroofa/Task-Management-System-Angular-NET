using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace capstone.web.api.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public ICollection<Quest>? Quests { get; set; }

    }
}
