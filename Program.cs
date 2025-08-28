using Farmacia.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Configuration
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policyBuilder =>
	{
		policyBuilder.WithOrigins("https://wonderful-tree-054da1710.6.azurestaticapps.net")
				   .AllowAnyHeader()
				   .AllowAnyMethod()
				   .AllowCredentials();
	});
});

// ----------------------------
// DATABASE CONFIGURATION
// ----------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString, npgsqlOptions =>
	{
		// npgsqlOptions.EnableRetryOnFailure(maxRetryCount:5, maxRetryDelay: TimeSpan.FromSeconds(30));
	}));

// ----------------------------
// JWT Authentication
// ----------------------------
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
	Console.WriteLine("⚠️ WARNING: Jwt:Key not found in configuration. Using default dev key.");
	jwtKey = "default_dev_key_change_me"; // clave fallback
}
else
{
	Console.WriteLine("✅ Jwt:Key loaded correctly from configuration.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

var app = builder.Build();

// Apply migrations automatically
try
{
	using (var scope = app.Services.CreateScope())
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		if (dbContext.Database.IsRelational())
		{
			dbContext.Database.Migrate();
		}
	}
}
catch (Exception ex)
{
	Console.WriteLine($"Error applying migrations: {ex.Message}");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
