namespace capstone.web.api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using capstone.web.api.Data;
    using capstone.web.api.Models;
    using MongoDB.Driver;

    public static class PriorityEndpoints
    {
        public static void MapPriorityEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/priorities");

            // GET: /api/priorities
            group.MapGet("/", async (MongoDbContext mongoDbContext) =>
            {
                var prioritiesCollection = mongoDbContext.GetCollection<Priority>("Priorities");

                var priorities = await prioritiesCollection
                    .Find(p => !p.IsDeleted)
                    .ToListAsync();
                
                return Results.Ok(priorities);
            });

            // GET: /api/priorities/{id}
            group.MapGet("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {
                var prioritiesCollection = mongoDbContext.GetCollection<Priority>("Priorities");
                var priority = await prioritiesCollection
                    .Find(p => p.PriorityId == id && !p.IsDeleted)
                    .FirstOrDefaultAsync();

                return priority is not null ? Results.Ok(priority) : Results.NotFound();
            });

            // POST: /api/priorities
            group.MapPost("/", async (Priority priority, MongoDbContext mongoDbContext) =>
            {

                var prioritiesCollection = mongoDbContext.GetCollection<Priority>("Priorities");


                // Count existing (non-deleted) priorities
                var priorityCount = await prioritiesCollection
                    .CountDocumentsAsync(p => !p.IsDeleted);

                if (priorityCount < 1) //at least one entry in the database to be able to post
                {
                    return Results.BadRequest("There must be at least 1 Priority level");
                }
                priority.IsDeleted = false;
                priority.DateCreated = DateTime.Now;
                
                await prioritiesCollection.InsertOneAsync(priority);

                return Results.Created($"/api/priorities/{priority.PriorityId}", priority);
            });

            // PUT: /api/priorities/{id}
            group.MapPut("/{id}", async (string id, Priority updatedPriority, MongoDbContext mongoDbContext) =>
            {

                var prioritiesCollection = mongoDbContext.GetCollection<Priority>("Priorities");

                var priority = await prioritiesCollection
                                .Find(p => p.PriorityId == id)
                                .FirstOrDefaultAsync();

                if (priority is null) return Results.NotFound();

                priority.Name = updatedPriority.Name;  // Update properties as needed
                priority.IsDeleted = updatedPriority.IsDeleted;
                priority.DateCreated = updatedPriority.DateCreated;
                
                await prioritiesCollection.ReplaceOneAsync(p => p.PriorityId == id, priority);

                return Results.NoContent();
            });

            // DELETE: /api/priorities/{id}
            group.MapDelete("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {

                var prioritiesCollection = mongoDbContext.GetCollection<Priority>("Priorities");

                var priority = await prioritiesCollection
                    .Find(p => p.PriorityId == id)
                    .FirstOrDefaultAsync();

                if (priority is null) return Results.NotFound();

                priority.IsDeleted = true;

                await prioritiesCollection.ReplaceOneAsync(p => p.PriorityId == id, priority);

                return Results.NoContent();
            });
        }
    }

}
