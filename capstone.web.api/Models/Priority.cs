using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace capstone.web.api.Models
{
    public class Priority
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string PriorityId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }
        public string Color { get; set; }


        [JsonIgnore]
        public ICollection<Quest>? Quests { get; set; }
        
    }
}
