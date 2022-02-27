using inOfficeApplication.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("inOfficeDb"),b=> b.MigrationsAssembly("inOfficeApplication.Data")));


//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//                options.UseNpgsql(builder.Configuration.GetConnectionString("inOfficeDb"), o=>o.MigrationsAssembly("inOfficeApplication")));

/*await using var conn = new NpgsqlConnection("inOfficeDb");

await using (var cmd = new NpgsqlCommand("INSERT INTO Emplyoees (Email,FirstName,LastName,Password,JobTitle) VALUES ('vedrannuub@inoffice.com','Vedran','Nuub','Nuub123!','Dev')", conn))
{
    cmd.Parameters.AddWithValue("p", "Hello world");
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        Console.WriteLine(reader.GetString(0));
}*/

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (var scope = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
        {
            scope?.Database.Migrate();
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
 
app.UseAuthorization();

app.MapControllers();

app.Run();
