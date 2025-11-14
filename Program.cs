using Farmacia.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// CONFIG PORT PARA RENDER
// ----------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

// ----------------------------
// SERVICES
// ----------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------
// CORS
// ----------------------------
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policyBuilder =>
	{
		policyBuilder.WithOrigins("http://farmacia.local:3000")
				   .AllowAnyHeader()
				   .AllowAnyMethod()
				   .AllowCredentials();
	});
});

// ----------------------------
// DATABASE
// ----------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseNpgsql(connectionString));

// ----------------------------
// JWT
// ----------------------------
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
	Console.WriteLine("⚠ WARNING: Jwt:Key not found in configuration. Using default dev key.");
	jwtKey = "default_dev_key_change_me";
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

// ----------------------------
// APP
// ----------------------------
var app = builder.Build();

// Apply migrations on startup
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

// ----------------------------
// PIPELINE
// ----------------------------
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

// Endpoint raíz para probar en Render
app.MapGet("/", () => "API OK en Render 🚀");

app.Run();
