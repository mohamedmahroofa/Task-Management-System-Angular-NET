namespace capstone.web.api.Endpoints
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using capstone.web.api.Data;
    using capstone.web.api.Models;
    using System.Net.Http;
    using MongoDB.Driver;

    public static class QuestEndpoints
    {
        public static void MapQuestEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/quests");

            // GET: /api/quests
            group.MapGet("/", async (MongoDbContext mongoDbContext, HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst("id");
                var userRoleClaim = httpContext.User.FindFirst(ClaimTypes.Role);

                if (userIdClaim == null || userRoleClaim == null)
                {
                    return Results.Unauthorized();
                }
                var userId = userIdClaim.Value;
                var userRole = userRoleClaim.Value;

                var questsCollection = mongoDbContext.GetCollection<Quest>("Quests");

                if (userRole == "Administrator")
                {
                    var allQuests = await questsCollection.Find(q => !q.IsDeleted).ToListAsync();
                    return Results.Ok(allQuests);
                }
                else
                {
                    // Otherwise, filter quests based on the user's ID
                    var userQuests = await questsCollection
                        .Find(q => q.UserId == userId && !q.IsDeleted)
                        .ToListAsync();

                    return Results.Ok(userQuests);
                }

                //  var quests = await db.Quests.Where(q => !q.IsDeleted && q.UserId == userId).ToListAsync();
                // return Results.Ok(quests); 




            });

            // GET: /api/quests/{id}
            group.MapGet("/{id}", async (string id, MongoDbContext mongoDbContext, HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }
                var userId = userIdClaim.Value;

                var questsCollection = mongoDbContext.GetCollection<Quest>("Quests");

                var quest = await questsCollection
                    .Find(q => q.QuestId == id && !q.IsDeleted)
                    .FirstOrDefaultAsync();
                
                if (quest == null)
                    return Results.NotFound();

                // Check ownership: allow admin or owner
                var userRoleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
                var userRole = userRoleClaim?.Value;

                if (userRole != "Administrator" && quest.UserId != userId)
                    return Results.Forbid(); // Not the owner and not admin

                return Results.Ok(quest);
            });

            // POST: /api/quests
            group.MapPost("/", async (Quest quest, MongoDbContext mongoDbContext, HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }
                var userId = userIdClaim.Value;

                quest.UserId = userId;

                quest.IsDeleted = false;
                quest.DateCreated = DateTime.Now;
                
                // Get the collection
                var questsCollection = mongoDbContext.GetCollection<Quest>("Quests");

                // Insert the new quest
                await questsCollection.InsertOneAsync(quest);

                return Results.Created($"/api/quests/{quest.QuestId}", quest);
            });

            // PUT: /api/quests/{id}
            group.MapPut("/{id}", async (string id, Quest updatedQuest, MongoDbContext mongoDbContext, HttpContext httpContext) =>
            {

                var userIdClaim = httpContext.User.FindFirst("id");
                if (userIdClaim == null)
                {
                    return Results.Unauthorized();
                }
                var userId = userIdClaim.Value;

                var questsCollection = mongoDbContext.GetCollection<Quest>("Quests");

                var quest = await questsCollection.Find(q => q.QuestId == id).FirstOrDefaultAsync();

                if (quest is null) return Results.NotFound();

                // Optional: Only allow the owner or admin to update
                var userRoleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
                var userRole = userRoleClaim?.Value;

                if (quest.UserId != userId && userRole != "Administrator")
                    return Results.Forbid();

                // Propeties be updated.
                quest.Name = updatedQuest.Name;  
                quest.DateCreated = updatedQuest.DateCreated;
                quest.DueDate = updatedQuest.DueDate;
                quest.CategoryId = updatedQuest.CategoryId;
                quest.PriorityId = updatedQuest.PriorityId;
                quest.StatusId = updatedQuest.StatusId;
                

                // Replace the document in MongoDB
                await questsCollection.ReplaceOneAsync(q => q.QuestId == id, quest);

                return Results.NoContent();
            });

            // DELETE: /api/quests/{id}
            group.MapDelete("/{id}", async (string id, MongoDbContext mongoDbContext, HttpContext httpContext) =>
            {
                
                var userIdClaim = httpContext.User.FindFirst("id");

                if (userIdClaim == null)
                    return Results.Unauthorized();

                var userId = userIdClaim.Value; // string now

                var questsCollection = mongoDbContext.GetCollection<Quest>("Quests");

                // Find the quest by ID
                var quest = await questsCollection.Find(q => q.QuestId == id).FirstOrDefaultAsync();

                if (quest is null) return Results.NotFound();

                var userRoleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
                var userRole = userRoleClaim?.Value;
                if (quest.UserId != userId && userRole != "Administrator")
                    return Results.Forbid();

                quest.IsDeleted = true;

                // Replace the document in MongoDB
                await questsCollection.ReplaceOneAsync(q => q.QuestId == id, quest);
                
                return Results.NoContent();
            });
        }
    }

}
