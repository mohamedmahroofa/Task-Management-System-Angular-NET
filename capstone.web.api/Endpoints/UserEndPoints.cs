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
    using System.Text.RegularExpressions;
    using MongoDB.Driver;

  public static class UserEndpoints
    {
        private static string GenerateJwtToken(User user, string secretKey)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role), // Important for role-based authorization
                new Claim("id", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static void MapUserEndpoints(this IEndpointRouteBuilder endpoints)
        {   
            var secretKey = "%^@#HD*@HD2387d223wyfi@67823gfSDHIFEQIWUC387f@3fhR$#@@jfwWEHI";

            endpoints.MapGet("/api/users", [Authorize (Policy = "ReadOnlyAndAbove")] async (MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");
                var users = usersCollection.Find(_ => true).ToList();
                return Results.Ok(users);
            });

            endpoints.MapGet("/api/users/{id}", [Authorize(Policy = "ReadOnlyAndAbove")] async (string id, MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");
                var user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                return user is not null ? Results.Ok(user) : Results.NotFound();
            });

            endpoints.MapPost("/api/users", async (User user, MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");

                // Check if a user with the same username or email already exists
                var existingUser = await usersCollection
                    .Find(u => u.Username == user.Username || u.Email == user.Email)
                    .FirstOrDefaultAsync();
                
                if (existingUser != null)
                {
                    return Results.BadRequest("Username or Email already exists.");
                }
                                // Hash the Password
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                    // Set the default role to "General"
                    if (string.IsNullOrWhiteSpace(user.Role))
                    {
                        user.Role = "General"; // Set default role
                    }

                  await usersCollection.InsertOneAsync(user);

                  return Results.Created($"/api/users/{user.Id}", user);
                /**user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash); // Secure hashing
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Results.Created($"/api/users/{user.Id}", user);**/
            });

            endpoints.MapPost("/api/register", async (User user, MongoDbContext mongoDbContext) =>
            {
               var usersCollection = mongoDbContext.GetCollection<User>("Users");

    // Check if a user with the same username or email already exists
                var existingUser = await usersCollection
                    .Find(u => u.Username == user.Username || u.Email == user.Email)
                    .FirstOrDefaultAsync();
                
                if (existingUser != null)
                {
                    return Results.BadRequest("Username or Email already exists.");
                }


                // Hash the Password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                // Set the default role to "General"
                if (string.IsNullOrWhiteSpace(user.Role))
                {
                    user.Role = "General"; // Set default role
                }

                // Insert the new user into MongoDB
                await usersCollection.InsertOneAsync(user);

                return Results.Created($"/api/users/{user.Id}", user);
            });

           /* endpoints.MapPost("/api/register", async (User user, AppDbContext db) =>
            {
                // Validate that the username and email are unique and match email pattern
                
               
            });*/

            endpoints.MapPut("/api/users/{id}", [Authorize(Policy = "AdministratorOnly")] async (string id, User updateUser, MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");
                var user = await usersCollection.Find(u => u.Id ==id).FirstOrDefaultAsync();
                if (user is null) return Results.NotFound();

                user.FirstName = updateUser.FirstName;
                user.LastName = updateUser.LastName;
                user.Email = updateUser.Email;
                user.Username = updateUser.Username;
                user.Role = updateUser.Role;

                if (!string.IsNullOrWhiteSpace(updateUser.PasswordHash))
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUser.PasswordHash);

                // Replace the existing user document with the updated one
                await usersCollection.ReplaceOneAsync(u => u.Id == id, user);

                return Results.NoContent();
            });

            endpoints.MapDelete("/api/users/{id}", [Authorize(Policy = "AdministratorOnly")] async (string id, MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");
                var user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user is not null)
                {
                    // Delete the user
                    await usersCollection.DeleteOneAsync(u => u.Id == id);
                }
                return Results.NoContent();
            });

            endpoints.MapPost("/api/login", async (LoginDto login, MongoDbContext mongoDbContext) =>
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");
                // Find the user by username
                var user = await usersCollection
                    .Find(u => u.Username == login.Username)
                    .FirstOrDefaultAsync();
                
                if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                    return Results.Unauthorized();

                var token = GenerateJwtToken(user, secretKey);
                return Results.Ok(new { Token = token });
            });

            endpoints.MapPost("/api/logout", [Authorize] () =>
            {
                // Clear token client-side by expiring or removing from storage
                return Results.Ok(new { Message = "Logout successful" });
            });
        }

        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }

}
