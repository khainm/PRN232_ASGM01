using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NguyenMinhKhai_PRN232_A01_BE.sln.Data;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using NguyenMinhKhai_PRN232_A01_BE.sln.Models;
using NguyenMinhKhai_PRN232_A01_BE.sln.Repositories;
using NguyenMinhKhai_PRN232_A01_BE.sln.Mappings;
using NguyenMinhKhai_PRN232_A01_BE.sln.Services;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using FUNewsManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use a different port if 7200 is not available
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7200, listenOptions =>
    {
        listenOptions.UseHttps();
    }); // HTTPS on port 7200
    serverOptions.ListenAnyIP(7201, listenOptions =>
    {
        listenOptions.UseHttps();
    }); // HTTPS on port 7201
    serverOptions.ListenAnyIP(7202, listenOptions =>
    {
        listenOptions.UseHttps();
    }); // HTTPS on port 7202
});

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .SetMaxTop(100)
        .Count()
        .Expand())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "News Management System API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();

// Add Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")))
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token validated successfully");
            
            if (context.Principal?.Claims != null)
            {
                var claims = context.Principal.Claims;
                logger.LogInformation("Claims in token: {Claims}", string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}")));
                
                var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                logger.LogInformation("Role claim found: {RoleClaim}", roleClaim?.Value ?? "null");
                
                if (roleClaim != null)
                {
                    var identity = context.Principal.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                        logger.LogInformation("Added role claim to identity: {Role}", roleClaim.Value);
                    }
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Authentication failed: {Error}", context.Exception);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Challenge issued: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(ClaimTypes.Role, "0") ||
            context.User.HasClaim(ClaimTypes.Role, "Admin")));
    options.AddPolicy("RequireStaffRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(ClaimTypes.Role, "1") ||
            context.User.HasClaim(ClaimTypes.Role, "Staff")));
    options.AddPolicy("RequireUserRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(ClaimTypes.Role, "2") ||
            context.User.HasClaim(ClaimTypes.Role, "User")));
    options.AddPolicy("RequireAdminOrStaffRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(ClaimTypes.Role, "0") ||
            context.User.HasClaim(ClaimTypes.Role, "1") ||
            context.User.HasClaim(ClaimTypes.Role, "Admin") ||
            context.User.HasClaim(ClaimTypes.Role, "Staff")));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
            "http://localhost:5173",
            "https://localhost:5173",
            "http://localhost:3000",
            "https://localhost:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure admin account exists
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var accountService = services.GetRequiredService<IAccountService>();
        await accountService.EnsureAdminAccountExists(builder.Configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the admin account.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

