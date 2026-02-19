using System.Text.Json.Serialization;

namespace capstone.web.api.Models
{
    public class Quest
    {
        public string QuestId { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsDeleted { get; set; }


        //Forgein Key
        public string CategoryId { get; set; }
        public string PriorityId { get; set; }
        public string UserId { get; set; }
        public string StatusId { get; set; }


        //Navigational properties
        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public Priority? Priority { get; set; }
        [JsonIgnore]
        public Status? Status { get; set; }

        [JsonIgnore]
        public User? User { get; set; }


    }
}
