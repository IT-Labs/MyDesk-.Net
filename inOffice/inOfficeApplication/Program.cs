using inOffice.Repository.Implementation;
using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using Microsoft.EntityFrameworkCore;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Implementation;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("inOfficeDb"),b=> b.MigrationsAssembly("inOfficeApplication.Data")));


builder.Services.AddCors();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IOfficeRepository, OfficeRepository>();
builder.Services.AddScoped<IDeskRepository, DeskRepository>();
builder.Services.AddScoped<IConferenceRoomRepository, ConferenceRoomRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

builder.Services.AddTransient<IOfficeService, OfficeService>();
builder.Services.AddTransient<IEntitiesService, EntitiesService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IReviewService, ReviewService>();

var configManager = new ConfigurationManager<OpenIdConnectConfiguration>("https://login.microsoftonline.com/9a433611-0c81-4f7b-abae-891364ddda17/v2.0/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());

var openIdConfig = await configManager.GetConfigurationAsync();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = configuration["JwtInfo:Issuer"],
    ValidAudience = configuration["JwtInfo:Audience"],
    IssuerSigningKeys = openIdConfig.SigningKeys,
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
         }
    });
});


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
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);
 
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
