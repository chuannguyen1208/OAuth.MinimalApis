using Microsoft.AspNetCore.Authentication;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Cookie")
    .AddCookie("Cookie", o =>
    {
        o.LoginPath = "/login";
    })
    .AddOAuth("Custom", o =>
    {
        o.SignInScheme = "Cookie";

        o.ClientId = "x";
        o.ClientSecret = "x";

        o.AuthorizationEndpoint = "http://localhost:5000/oauth/authorize";
        o.TokenEndpoint = "http://localhost:5000/oauth/token";
        o.CallbackPath = "/oauth/custom-cb";

        o.UsePkce = true;
        o.ClaimActions.MapJsonKey("sub", "sub");
        o.ClaimActions.MapJsonKey("custom 33", "custom");
        o.Events.OnCreatingTicket = async ctx =>
        {
            var payloadBase64 = ctx.AccessToken!.Split('.')[1];
            var payloadJson = Base64UrlTextEncoder.Decode(payloadBase64);
            var payload = JsonDocument.Parse(payloadJson);
            ctx.RunClaimActions(payload.RootElement);

            await Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", () => "Authenticated").RequireAuthorization();

app.MapGet("/login", (HttpContext context) =>
{
    var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

    return Results.Challenge(
        new AuthenticationProperties()
        {
            RedirectUri = baseUrl,
        },
        authenticationSchemes: new List<string> { "Custom" }
    );
});

app.Run();
