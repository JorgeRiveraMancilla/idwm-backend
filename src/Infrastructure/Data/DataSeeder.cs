using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tienda_UCN_api.src.Domain.Models;

namespace Tienda_UCN_api.src.Infrastructure.Data
{
    public class DataSeeder
    {
        /// <summary>
        /// Método para inicializar la base de datos con datos de prueba.
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios para obtener el contexto de datos y otros servicios.</param>
        /// <returns>Tarea asíncrona que representa la operación de inicialización.</returns>
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            try
            {

                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await context.Database.EnsureCreatedAsync();
                await context.Database.MigrateAsync();

                var genders = configuration.GetSection("User:Genders").Get<string[]>() ?? throw new InvalidOperationException("La configuración de géneros no está presente.");

                // Creación de roles
                if (!context.Roles.Any())
                {
                    var roles = new List<Role>
                        {
                            new Role { Name = "Admin", NormalizedName = "ADMIN" },
                            new Role { Name = "Customer", NormalizedName = "CUSTOMER" }
                        };
                    await context.Roles.AddRangeAsync(roles);
                    await context.SaveChangesAsync();
                    Log.Information("Roles creados con éxito.");
                }

                // Creación de categorías
                if (!context.Categories.Any())
                {
                    var categories = new List<Category>
                            {
                                new Category { Name = "Electronics" },
                                new Category { Name = "Clothing" },
                                new Category { Name = "Home Appliances" },
                                new Category { Name = "Books" },
                                new Category { Name = "Sports" }
                            };
                    await context.Categories.AddRangeAsync(categories);
                    await context.SaveChangesAsync();
                    Log.Information("Categorías creadas con éxito.");
                }

                // Creación de marcas
                if (!await context.Brands.AnyAsync())
                {
                    var brands = new List<Brand>
                        {
                            new Brand { Name = "Sony" },
                            new Brand { Name = "Apple" },
                            new Brand { Name = "HP" }
                        };
                    await context.Brands.AddRangeAsync(brands);
                    await context.SaveChangesAsync();
                    Log.Information("Marcas creadas con éxito.");
                }

                // Creación de imágenes
                if (!await context.Images.AnyAsync())
                {
                    var imageFaker = new Faker<Image>()
                        .RuleFor(i => i.ImageUrl, f => f.Image.PicsumUrl())
                        .RuleFor(i => i.PublicId, f => f.Random.Guid().ToString());

                    var images = imageFaker.Generate(20);
                    await context.Images.AddRangeAsync(images);
                    await context.SaveChangesAsync();
                    Log.Information("Imágenes creadas con éxito.");
                }

                // Creación de usuarios
                if (!await context.Users.AnyAsync())
                {
                    int customerId = await context.Roles.Where(r => r.Name == "Customer").Select(r => r.Id).FirstOrDefaultAsync();
                    int adminId = await context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefaultAsync();
                    // Creación de usuario administrador
                    User adminUser = new User
                    {
                        UserName = configuration["User:AdminUser:Email"],
                        NormalizedUserName = configuration["User:AdminUser:Email"]?.ToUpper(),
                        FirstName = configuration["User:AdminUser:FirstName"] ?? throw new InvalidOperationException("El nombre del usuario administrador no está configurado."),
                        LastName = configuration["User:AdminUser:LastName"] ?? throw new InvalidOperationException("El apellido del usuario administrador no está configurado."),
                        Email = configuration["User:AdminUser:Email"] ?? throw new InvalidOperationException("El email del usuario administrador no está configurado."),
                        NormalizedEmail = configuration["User:AdminUser:Email"]?.ToUpper(),
                        EmailConfirmed = true,
                        RoleId = adminId,
                        Gender = configuration["User:AdminUser:Gender"] ?? throw new InvalidOperationException("El género del usuario administrador no está configurado."),
                        Rut = configuration["User:AdminUser:Rut"] ?? throw new InvalidOperationException("El RUT del usuario administrador no está configurado."),
                        BirthDate = DateTime.Parse(configuration["User:AdminUser:BirthDate"] ?? throw new InvalidOperationException("La fecha de nacimiento del usuario administrador no está configurada.")),
                        PhoneNumber = configuration["User:AdminUser:PhoneNumber"] ?? throw new InvalidOperationException("El número de teléfono del usuario administrador no está configurado.")
                    };
                    var adminPassword = configuration["User:AdminUser:Password"] ?? throw new InvalidOperationException("La contraseña del usuario administrador no está configurada.");
                    await userManager.CreateAsync(adminUser, adminPassword);
                    Log.Information("Usuario administrador creado con éxito.");

                    // Creación de usuarios aleatorios
                    var randomPassword = configuration["User:RandomUserPassword"] ?? throw new InvalidOperationException("La contraseña de los usuarios aleatorios no está configurada.");
                    var userFaker = new Faker<User>()
                        .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                        .RuleFor(u => u.LastName, f => f.Name.LastName())
                        .RuleFor(u => u.Email, f => f.Internet.Email())
                        .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpper())
                        .RuleFor(u => u.EmailConfirmed, f => true)
                        .RuleFor(u => u.Gender, f => f.PickRandom(genders))
                        .RuleFor(u => u.Rut, f => RandomRut())
                        .RuleFor(u => u.BirthDate, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
                        .RuleFor(u => u.PasswordHash, (f, u) => userManager.PasswordHasher.HashPassword(u, randomPassword))
                        .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                        .RuleFor(u => u.RoleId, f => customerId);
                    var users = userFaker.Generate(99);
                    await context.Users.AddRangeAsync(users);
                    await context.SaveChangesAsync();
                    Log.Information("Usuarios creados con éxito.");
                }

                // Creación de productos
                if (!await context.Products.AnyAsync())
                {
                    var categoryIds = await context.Categories.Select(c => c.Id).ToListAsync();
                    var brandIds = await context.Brands.Select(b => b.Id).ToListAsync();
                    var images = await context.Images.ToListAsync();

                    if (categoryIds.Any() && brandIds.Any() && images.Any())
                    {
                        var productFaker = new Faker<Product>()
                            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
                            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                            .RuleFor(p => p.Price, f => f.Random.Int(1000, 100000))
                            .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                            .RuleFor(p => p.CategoryId, f => f.PickRandom(categoryIds))
                            .RuleFor(p => p.BrandId, f => f.PickRandom(brandIds))
                            .RuleFor(p => p.Status, f => "Nuevo")
                            .RuleFor(p => p.Images, f => f.PickRandom(images, f.Random.Int(1, Math.Min(5, images.Count))).ToList());

                        var products = productFaker.Generate(50);
                        await context.Products.AddRangeAsync(products);
                        await context.SaveChangesAsync();
                        Log.Information("Productos creados con éxito.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al inicializar la base de datos: {Message}", ex.Message);
            }
        }

        /// <summary>
        /// Método para generar un RUT chileno aleatorio.
        /// </summary>
        /// <returns>Un RUT en formato "XXXXXXXX-X".</returns>
        private static string RandomRut()
        {
            var faker = new Faker();
            var rut = faker.Random.Int(1000000, 99999999).ToString();
            var dv = faker.Random.Int(0, 9).ToString();
            return $"{rut}-{dv}";
        }
    }
}

