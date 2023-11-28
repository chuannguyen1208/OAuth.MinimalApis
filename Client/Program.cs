using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie")
    .AddOAuth("custom", o =>
    {
        o.SignInScheme = "cookie";

        o.ClientId = "x";
        o.ClientSecret = "x";

        o.AuthorizationEndpoint = "http://localhost:5001/oauth/authorize";
        o.TokenEndpoint = "http://localhost:5001/oauth/token";
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

app.UseHttpsRedirection();

app.MapGet("/", () => "Ok");

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
