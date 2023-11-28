using OAuthService;
using OAuthService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Cookie")
	.AddCookie("Cookie", o =>
	{
		o.LoginPath = "/login";
	});

builder.Services.AddAuthorization();

builder.Services.AddSingleton<DevKeys>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapOAuthEndpoints();

app.Run();
