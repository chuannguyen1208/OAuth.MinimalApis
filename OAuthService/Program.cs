using OAuthService;
using OAuthService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Cookie")
	.AddCookie("Cookie", o =>
	{
		o.LoginPath = "/login";
	});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<DevKeys>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "Authenticated").RequireAuthorization();
app.MapOAuthEndpoints();

app.Run();
