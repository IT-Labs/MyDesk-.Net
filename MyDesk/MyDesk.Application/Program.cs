using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using MyDesk.Application.Mapper;
using MyDesk.BusinessLogicLayer;
using MyDesk.Data.Entities.Extensions;
using MyDesk.Data;
using MyDesk.Application.Middleware;
using MyDesk.Data.Interfaces.BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using MyDesk.Core.Database;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.Utils;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetValue<string>("ConnectionString"), b => b.MigrationsAssembly("MyDesk.Data")), contextLifetime: ServiceLifetime.Transient);

builder.Services.AddCors();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<MigrationService>();
builder.Services.AddTransient<Func<DbContextOptions<ApplicationDbContext>, ApplicationDbContext>>(provider => (options) => ActivatorUtilities.CreateInstance<ApplicationDbContext>(provider, options));
builder.Services.AddScoped<IContext, ApplicationDbContext>();

builder.Services.AddTransient<IOfficeService, OfficeService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IReviewService, ReviewService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IConferenceRoomService, ConferenceRoomService>();
builder.Services.AddTransient<IDeskService, DeskService>();
builder.Services.AddTransient<IAuthService, AuthService>();
//builder.Services.AddTransient<Func<IEmployeeService>>(provider => () => provider.GetService<IEmployeeService>());

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// Configure mapper
IMapper mapper = MapperConfigurations.CreateMapper();
builder.Services.AddSingleton(mapper);

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

builder.Services
    .AddAuthentication()
    .AddJwtBearer("Local", x =>
    {
        x.RequireHttpsMetadata = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("Authentication:Local:CustomBearerTokenSigningKey"))),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    })
    .AddJwtBearer("AzureAd", x =>
    {
        x.RequireHttpsMetadata = true;
        x.MetadataAddress = builder.Configuration.GetValue<string>("Authentication:AzureAd:MetadataAddress");
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetValue<string>("Authentication:AzureAd:Audience"),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:AzureAd:Issuer"),
        };
    })
    .AddJwtBearer("Google", options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("Authentication:Google:Issuer");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Google:Issuer"),
            ValidateAudience = true,
            ValidAudience =  builder.Configuration.GetValue<string>("Authentication:Google:ClientId"),
            ValidateLifetime = false
        };
    });

builder.Services.AddAuthorization(options =>
     {
         options.DefaultPolicy = new AuthorizationPolicyBuilder()
             .RequireAuthenticatedUser()
             .AddAuthenticationSchemes("Local","AzureAd", "Google")
             .Build();
     });

WebApplication app = builder.Build();

using (IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var migrationService = serviceScope.ServiceProvider.GetService<MigrationService>();
    if (migrationService != null)
        migrationService.ExecuteMigrations(DbType.SQL);

    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context, builder.Configuration.GetValue<string>("AdminEmail"), builder.Configuration.GetValue<string>("AdminPassword"));
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());

app.MapControllers();

app.Run();