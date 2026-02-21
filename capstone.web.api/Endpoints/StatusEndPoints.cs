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

                // Count existing non-deleted statuses
                var statusCount = await statusesCollection.CountDocumentsAsync(s => !s.IsDeleted);

                if (statusCount < 1) //at least one entry in the database to be able to post
                {
                    return Results.BadRequest("There must be at least 1 Status");
                }

                status.IsDeleted = false;
                status.DateCreated = DateTime.Now;

                 // Generate sequential string ID
                var lastStatus = await statusesCollection
                    .Find(_ => true)
                    .SortByDescending(s => s.StatusId)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                status.StatusId = lastStatus != null && lastStatus.StatusId.StartsWith("s")
                    ? $"s{int.Parse(lastStatus.StatusId[1..]) + 1}"
                    : "s1";
                
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
