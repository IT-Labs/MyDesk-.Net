using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using MyDesk.Application.Mapper;
using MyDesk.Repository;
using MyDesk.BusinessLogicLayer;
using MyDesk.Data.Utils;
using MyDesk.Data.Entities.Extensions;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data;
using MyDesk.Application.Middleware;
using MyDesk.Data.Interfaces.BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IApplicationParmeters applicationParmeters = new ApplicationParmeters(builder.Configuration, new MemoryCache(new MemoryCacheOptions()));

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(applicationParmeters.GetConnectionString(), b => b.MigrationsAssembly("MyDesk.Data")), contextLifetime: ServiceLifetime.Transient);

builder.Services.AddCors();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddTransient<IReservationRepository, ReservationRepository>();
builder.Services.AddTransient<IOfficeRepository, OfficeRepository>();
builder.Services.AddTransient<IDeskRepository, DeskRepository>();
builder.Services.AddTransient<IConferenceRoomRepository, ConferenceRoomRepository>();
builder.Services.AddTransient<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddTransient<IReviewRepository, ReviewRepository>();
builder.Services.AddTransient<IMigrationRepository, MigrationRepository>();
builder.Services.AddTransient<Func<DbContextOptions<ApplicationDbContext>, ApplicationDbContext>>(provider => (options) => ActivatorUtilities.CreateInstance<ApplicationDbContext>(provider, options));

builder.Services.AddTransient<IOfficeService, OfficeService>();
builder.Services.AddTransient<IReservationService, ReservationService>();
builder.Services.AddTransient<IReviewService, ReviewService>();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<IConferenceRoomService, ConferenceRoomService>();
builder.Services.AddTransient<IDeskService, DeskService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<Func<IEmployeeService>>(provider => () => provider.GetService<IEmployeeService>());

builder.Services.AddTransient<IApplicationParmeters, ApplicationParmeters>();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(applicationParmeters.GetCustomBearerTokenSigningKey(builder.Environment.IsDevelopment()))),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    })
    .AddJwtBearer("AzureAd", x =>
    {
        x.RequireHttpsMetadata = true;
        x.MetadataAddress = applicationParmeters.GetAzureAdMetadataAddress(builder.Environment.IsDevelopment());
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidAudience = applicationParmeters.GetAzureAdAudience(builder.Environment.IsDevelopment()),
            ValidateIssuer = true,
            ValidIssuer = applicationParmeters.GetAzureAdIssuer(builder.Environment.IsDevelopment()),
        };
    })
    .AddJwtBearer("Google", options =>
    {
        options.Authority = applicationParmeters.GetGoogleIssuer(builder.Environment.IsDevelopment());
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = applicationParmeters.GetGoogleIssuer(builder.Environment.IsDevelopment()),
            ValidateAudience = true,
            ValidAudience = applicationParmeters.GetGoogleClientId(builder.Environment.IsDevelopment()),
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

applicationParmeters = app.Services.GetRequiredService<IApplicationParmeters>();

using (IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    IMigrationRepository migrationRepository = serviceScope.ServiceProvider.GetService<IMigrationRepository>();
    migrationRepository.ExecuteMigrations(DbType.SQL);

    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context, applicationParmeters.GetAdminEmail(), applicationParmeters.GetAdminPassword());
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());

app.MapControllers();

app.Run();