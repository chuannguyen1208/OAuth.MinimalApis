using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("custom", o =>
    {
        o.SignInScheme = "cookie";

        o.ClientId = "x";
        o.ClientSecret = "x";

        o.AuthorizationEndpoint = "http://localhost:5001/oauth/authorize";
        o.TokenEndpoint = "https://localhost:5001/oauth/token";
        o.CallbackPath = "/oauth/custom-cb";

        o.UsePkce = true;
        o.ClaimActions.MapJsonKey("sub", "sub");
        o.Events.OnCreatingTicket = async ctx =>
        {
            // todo: map claims
            await Task.CompletedTask;
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/login", () =>
{
    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = "http://localhost:5002",
        },
        authenticationSchemes: new List<string> { "custom" }
    );
});

app.Run();
