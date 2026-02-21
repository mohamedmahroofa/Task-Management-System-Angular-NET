using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using capstone.web.api.Data;
using capstone.web.api.Models;
using capstone.web.api.Endpoints;
using MongoDB.Driver;
using MongoDB.Bson;

//adding a comment just to test commit
namespace capstone.web.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

           // Configure MongoDB
            builder.Services.AddSingleton(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var mongoSettings = configuration.GetSection("MongoSettings");
                var connectionString = mongoSettings["ConnectionString"];
                var databaseName = mongoSettings["DatabaseName"];
                return new MongoDbContext(connectionString, databaseName);
            });

            // Retrieve the secret key from configuration
            var secretKey = builder.Configuration["JwtConfig:Secret"];
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            if (keyBytes.Length < 16)
            {
                throw new InvalidOperationException("The secret key must be at least 16 bytes long.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });

            // Add authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
                options.AddPolicy("GeneralAndAbove", policy => policy.RequireRole("Administrator", "General"));
                options.AddPolicy("ReadOnlyAndAbove", policy => policy.RequireRole("Administrator", "General", "ReadOnly"));
            });

                // This is for Windows
            // builder.Services.AddDbContext<AppDbContext>(options =>
            //     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // This is for Mac
            // builder.Services.AddDbContext<AppDbContext>(options =>
            //         options.UseInMemoryDatabase("TestDb"));

            // Enable authorization
            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OpenCorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            var app = builder.Build();

            // Seed MongoDB here
            using (var scope = app.Services.CreateScope())
            {
                var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
                SeedMongoDatabase(mongoDbContext).GetAwaiter().GetResult();
            }

            // Seed method
            static async Task SeedMongoDatabase(MongoDbContext mongoDbContext)
            {
                var usersCollection = mongoDbContext.GetCollection<User>("Users");

                // Check if any users already exist
                var existingUsers = await usersCollection.Find(_ => true).ToListAsync();
                if (existingUsers.Count > 0) return; // Already seeded

                // Admin user
                var adminUser = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@example.com",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin-password"),
                    Role = "Administrator"
                };

                // General user
                var generalUser = new User
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    FirstName = "Ahmed",
                    LastName = "Sheedh",
                    Email = "sheedh@example.com",
                    Username = "general",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("general-password"),
                    Role = "General"
                };

                await usersCollection.InsertManyAsync(new[] { adminUser, generalUser });
                Console.WriteLine("MongoDB seeded: Admin and General user added.");
            }

            // using (var scope = app.Services.CreateScope())
            // {
            //     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //     // This is for Windows
            //     // db.Database.Migrate(); 
            //     SeedDatabase(db);
            // }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Use CORS with the wide-open policy
            app.UseCors("OpenCorsPolicy");
            app.UseHttpsRedirection();
            // Ensure authentication and authorization middleware are added
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapUserEndpoints();
            app.MapCategoryEndpoints();
            app.MapPriorityEndpoints();
            app.MapQuestEndpoints();
            app.MapStatusEndpoints();

            app.Run();
        }

        // static void SeedDatabase(AppDbContext context)
        // {
        //     if (!context.Users.Any())
        //     {
        //         // Example seed users
        //         context.Users.Add(new User
        //         {
        //             FirstName = "Admin",
        //             LastName = "User",
        //             Email = "admin@example.com",
        //             Username = "admin",
        //             PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin-password"), // Securely hash passwords
        //             Role = "Administrator"
        //         });

        //         context.Users.Add(new User
        //         {
        //             FirstName = "Jesus",
        //             LastName = "Alapisco",
        //             Email = "alapiscodavilaj@mymacewan.ca",
        //             Username = "alapiscof",
        //             PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"), // Securely hash passwords
        //             Role = "Administrator"
        //         });

        //         context.Users.Add(new User
        //         {
        //             FirstName = "General",
        //             LastName = "User",
        //             Email = "general@example.com",
        //             Username = "general",
        //             PasswordHash = BCrypt.Net.BCrypt.HashPassword("general-password"),
        //             Role = "General"
        //         });

        //         context.SaveChanges();
        //     }
        //     if (!context.Categories.Any())
        //     {
        //         // Example seed categories
        //         context.Categories.Add(new Category
        //         {
        //             Name = "Schoolwork",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //         });

        //         context.Categories.Add(new Category
        //         {
        //             Name = "Personal",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //         });

        //         context.SaveChanges();
        //     }
        //     if (!context.Priorities.Any())
        //     {
        //         // Example seed priorities
        //         context.Priorities.Add(new Priority
        //         {
        //             Name = "Low",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             color = "Green",
        //         });

        //         context.Priorities.Add(new Priority
        //         {
        //             Name = "Medium",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             color = "Yellow",
        //         });

        //         context.Priorities.Add(new Priority
        //         {
        //             Name = "High",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             color = "Orange",
        //         });

        //         context.Priorities.Add(new Priority
        //         {
        //             Name = "Critical",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             color = "Red",
        //         });
        //         context.SaveChanges();  
        //     }
        //     if (!context.Statuses.Any())
        //     {
        //         context.Statuses.Add(new Status
        //         {
        //             Name = "New",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now
        //         });

        //         context.Statuses.Add(new Status
        //         {
        //             Name = "Active",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now
        //         });

        //         context.Statuses.Add(new Status
        //         {
        //             Name = "Resolved",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now
        //         });
        //         context.Statuses.Add(new Status
        //         {
        //             Name = "Closed",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now
        //         });

        //         context.SaveChanges();
        //     }
        //     if (!context.Quests.Any())
        //     {
        //         // Example seed quests
        //         context.Quests.Add(new Quest
        //         {
        //             Name = "Go to School",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             DueDate = DateTime.Now,
        //             CategoryId = 1,
        //             PriorityId = 3,
        //             UserId = 1,
        //             StatusId = 1,
        //         });

        //         context.Quests.Add(new Quest
        //         {
        //             Name = "Take 21:00 pill",
        //             IsDeleted = false,
        //             DateCreated = DateTime.Now,
        //             DueDate = DateTime.Now,
        //             CategoryId = 2,
        //             PriorityId = 1,
        //             UserId = 3,
        //             StatusId = 1,
        //         });

        //         context.SaveChanges();
        //     }
            
        // }
    }
}