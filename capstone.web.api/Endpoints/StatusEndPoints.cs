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
    using MongoDB.Driver;

    public static class StatusEndpoints
    {
        public static void MapStatusEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/statuses");

            // GET: /api/statuses
            group.MapGet("/", async (MongoDbContext mongoDbContext) =>
            {
                var statusesCollection = mongoDbContext.GetCollection<Status>("Statuses");

                var statuses = await statusesCollection
                                .Find(s => !s.IsDeleted)
                                .ToListAsync();

                return Results.Ok(statuses);
            });

            // GET: /api/statuses/{id}
            group.MapGet("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {
                var statusesCollection = mongoDbContext.GetCollection<Status>("Statuses");

                var status = await statusesCollection
                    .Find(s => s.StatusId == id && !s.IsDeleted)
                    .FirstOrDefaultAsync();

                return status is not null ? Results.Ok(status) : Results.NotFound();
            });

            // POST: /api/statuses
            group.MapPost("/", async (Status status, MongoDbContext mongoDbContext) =>
            {

                var statusesCollection = mongoDbContext.GetCollection<Status>("Statuses");

                var existingStatus = await statusesCollection
                                        .Find(s => s.Name == status.Name)
                                        .FirstOrDefaultAsync();

                if(existingStatus != null)
                {
                    return Results.BadRequest("Status with this name already exists.");
                }

                 // Generate sequential string ID
                var lastStatus = await statusesCollection
                                    .Find(_ => true)
                                    .SortByDescending(s => s.StatusId)
                                    .Limit(1)
                                    .FirstOrDefaultAsync();

                if(lastStatus != null && lastStatus.StatusId.StartsWith("sta"))
                {
                    var lastNumber = int.Parse(lastStatus.StatusId[3..]);
                    status.StatusId = $"sta{lastNumber + 1}";
                }
                else
                {
                    status.StatusId = "sta1";
                }
                
                status.IsDeleted = false;
                status.DateCreated = DateTime.Now;

                await statusesCollection.InsertOneAsync(status);

                return Results.Created($"/api/statuses/{status.StatusId}", status);
            });

            // PUT: /api/statuses/{id}
            group.MapPut("/{id}", async (string id, Status updatedStatus, MongoDbContext mongoDbContext) =>
            {

                var statusesCollection = mongoDbContext.GetCollection<Status>("Statuses");

                var status = await statusesCollection
                                .Find(s => s.StatusId == id)
                                .FirstOrDefaultAsync();
                    
                if (status is null) return Results.NotFound();

                // Propeties be updated.
                status.Name = updatedStatus.Name;
                status.IsDeleted = updatedStatus.IsDeleted;
                status.DateCreated = updatedStatus.DateCreated;

                // Replace the existing document with the updated one
                await statusesCollection.ReplaceOneAsync(s => s.StatusId == id, status);

                return Results.NoContent();
            });

            // DELETE: /api/statuses/{id}
            group.MapDelete("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {

                var statusesCollection = mongoDbContext.GetCollection<Status>("Statuses");

                var status = await statusesCollection
                                .Find(s => s.StatusId == id)
                                .FirstOrDefaultAsync();

                if (status is null) return Results.NotFound();

                status.IsDeleted = true;
                
                 // Replace the document with the updated one
                await statusesCollection.ReplaceOneAsync(s => s.StatusId == id, status);

                return Results.NoContent();
            });
        }
    }

}
