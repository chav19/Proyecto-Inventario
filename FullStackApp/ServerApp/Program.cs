using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios esenciales
builder.Services.AddCors();
builder.Services.AddMemoryCache(); // Paso 3: Agregamos soporte para caché en memoria

var app = builder.Build();

// Configuración de CORS estandarizada
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

// Endpoint optimizado con almacenamiento en caché
app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    const string cacheKey = "products_list_cache";

    // Intentar obtener los datos desde la caché
    if (!cache.TryGetValue(cacheKey, out object? cachedProducts))
    {
        // Si no existen en caché, simulamos la extracción de datos (ej. Base de datos)
        cachedProducts = new[]
        {
            new
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
                Category = new { Id = 101, Name = "Electronics" }
            },
            new
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
                Category = new { Id = 102, Name = "Accessories" }
            }
        };

        // Guardamos los datos en caché por 5 minutos para optimizar el rendimiento del servidor
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        cache.Set(cacheKey, cachedProducts, cacheOptions);
    }

    return Results.Ok(cachedProducts);
});

app.Run();