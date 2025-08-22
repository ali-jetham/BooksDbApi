using System.Net.Mime;
using System.Text;
using LifeDbApi.Data;
using LifeDbApi.Models.Domain;
using LifeDbApi.Options;
using LifeDbApi.Repositories;
using LifeDbApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure port (Render will provide PORT env var)
if (builder.Environment.IsProduction())
{
	var port = Environment.GetEnvironmentVariable("PORT") ?? "5001";
	builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy(
		"AllowFrontend",
		policy =>
		{
			policy.WithOrigins(
				"http://localhost:5173",
				"https://yeti-measured-correctly.ngrok-free.app",
				"https://lifedb.netlify.app",
				"https://lifedbapi.onrender.com",
				"https://booksdb.alijetham.com"
			);
			policy.AllowAnyHeader();
			policy.AllowAnyMethod();
			policy.AllowCredentials();
		}
	);
});

// AUTH
builder
	.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			ValidAudience = builder.Configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
			),
			ClockSkew = TimeSpan.Zero,
		};
		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				if (context.Request.Cookies.TryGetValue("access_token", out var token))
				{
					context.Token = token;
				}
				return Task.CompletedTask;
			},
		};
	});
builder.Services.AddAuthorization();

builder.Services.AddDbContextPool<LifeDbContext>(dbContextOptions =>
	dbContextOptions
		.UseNpgsql(
			builder.Configuration["Database:ConnectionString"],
			postgresOptions =>
				postgresOptions
					.SetPostgresVersion(17, 0)
					.MapEnum<OAuthProvider>("oauth_provider")
					.MapEnum<BookSource>("book_source")
					.MapEnum<BookStatus>("book_status")
					.CommandTimeout(120)
		)
		.UseSnakeCaseNamingConvention()
);

// OPTIONS
builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection("Frontend"));
builder.Services.Configure<BackendOptions>(builder.Configuration.GetSection("Backend"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<CollectionService>();
builder.Services.AddScoped<IAuthRepository, SqlAuthRepository>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<ITokenRepository, SqlTokenRepository>();
builder.Services.AddScoped<ICollectionRepository, SqlCollectionRepository>();
builder.Services.AddScoped<IBookRepository, SqlBookRepository>();
builder.Services.AddScoped<IUserBookRepository, SqlUserBookRepository>();

// HTTP Client
builder.Services.AddHttpClient<BookService>(client =>
{
	client.BaseAddress = new Uri("https://openlibrary.org/");
	client.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
	client.DefaultRequestHeaders.UserAgent.ParseAdd("LifeDb/0.0 (alijetham24@gmail.com)");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
