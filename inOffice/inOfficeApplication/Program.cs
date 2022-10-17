using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data;
using Microsoft.EntityFrameworkCore;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using inOfficeApplication.Middleware;
using AutoMapper;
using inOfficeApplication.Mapper;
using inOffice.Repository;
using inOffice.BusinessLogicLayer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration["ConnectionString"], b => b.MigrationsAssembly("inOfficeApplication.Data")), contextLifetime: ServiceLifetime.Transient);


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
builder.Services.AddTransient<Func<IEmployeeRepository>>(provider => () => provider.GetService<IEmployeeRepository>());

builder.Services.AddScoped<IOfficeService, OfficeService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IConferenceRoomService, ConferenceRoomService>();
builder.Services.AddScoped<IDeskService, DeskService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IOpenIdConfigurationKeysFactory, OpenIdConfigurationKeysFactory>();
builder.Services.AddSingleton<IApplicationParmeters, ApplicationParmeters>();

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


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    using (IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        using (ApplicationDbContext scope = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
        {
            scope?.Database.Migrate();
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseMiddleware(typeof(AuthorizationMiddleware));

app.UseHttpsRedirection();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());

app.MapControllers();

app.Run();