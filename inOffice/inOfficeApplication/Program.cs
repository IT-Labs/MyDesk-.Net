using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("inOfficeDb"),b=> b.MigrationsAssembly("inOfficeApplication.Data")));


builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<JwtService>();


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
            scope?.Database.Migrate();
        
    }
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseCors(options=>options
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
);
 
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
