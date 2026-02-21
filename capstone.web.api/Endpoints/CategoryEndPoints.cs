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

    public static class CategoryEndpoints
    {
        public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/categories");

            // GET: /api/categories
            group.MapGet("/", async (MongoDbContext mongoDbContext) =>
            {
                var categoriesCollection = mongoDbContext.GetCollection<Category>("Categories");
                var categories = await categoriesCollection.Find(c => !c.IsDeleted).ToListAsync();
                return Results.Ok(categories);
            });

            // GET: /api/categories/{id}
            group.MapGet("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {
                var categoriesCollection = mongoDbContext.GetCollection<Category>("Categories");
                var category = await categoriesCollection
                    .Find(c => c.CategoryId == id && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                return category is not null ? Results.Ok(category) : Results.NotFound();
            });

            // POST: /api/categories
            group.MapPost("/", async (Category category, MongoDbContext mongoDbContext) =>
            {
                
                var categoriesCollection = mongoDbContext.GetCollection<Category>("Categories");

                var existingCategory = await categoriesCollection
                    .Find(c => c.Name == category.Name)
                    .FirstOrDefaultAsync();

                if (existingCategory != null)
                {
                    return Results.BadRequest("Category with this name already exists.");
                }

                // Generate sequential CategoryId
                var lastCategory = await categoriesCollection
                    .Find(_ => true)
                    .SortByDescending(c => c.CategoryId)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                if (lastCategory != null && lastCategory.CategoryId.StartsWith("cat"))
                {
                    var lastNumber = int.Parse(lastCategory.CategoryId[3..]);
                    category.CategoryId = $"cat{lastNumber + 1}";
                }
                else
                {
                    category.CategoryId = "cat1"; // first category
                }

                category.IsDeleted = false;
                category.DateCreated = DateTime.Now;

                await categoriesCollection.InsertOneAsync(category);

                return Results.Created($"/api/categories/{category.CategoryId}", category);
            });

            // PUT: /api/categories/{id}
            group.MapPut("/{id}", async (string id, Category updatedCategory, MongoDbContext mongoDbContext) =>
            {

                var categoriesCollection = mongoDbContext.GetCollection<Category>("Categories");

                var category = await categoriesCollection.Find(c => c.CategoryId == id).FirstOrDefaultAsync();

                if (category is null) return Results.NotFound();

                // Propeties be updated.
                category.Name = updatedCategory.Name;  
                category.DateCreated = updatedCategory.DateCreated;
                category.IsDeleted = updatedCategory.IsDeleted;

                 // Replace the existing document with the updated one
                await categoriesCollection.ReplaceOneAsync(c => c.CategoryId == id, category);

                return Results.NoContent();
            });

            // DELETE: /api/categories/{id}
            group.MapDelete("/{id}", async (string id, MongoDbContext mongoDbContext) =>
            {

                var categoriesCollection = mongoDbContext.GetCollection<Category>("Categories");


                // Find the category by Id
                var category = await categoriesCollection.Find(c => c.CategoryId == id).FirstOrDefaultAsync();
                if (category is null) return Results.NotFound();

                category.IsDeleted = true;
                
                // Replace the document with the updated one
                await categoriesCollection.ReplaceOneAsync(c => c.CategoryId == id, category);

                return Results.NoContent();
            });
        }
    }

}
