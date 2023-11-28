﻿using Microsoft.AspNetCore.DataProtection;
using System.Text.Json;
using System.Web;

namespace OAuthService.Endpoints.OAuth;

public static class AuthorizationEndpoints
{
	public static IResult Handle(
		HttpRequest request,
		IDataProtectionProvider dataProtectionProvider)
	{
		var iss = HttpUtility.UrlEncode("http://localhost:5001");
        request.Query.TryGetValue("state", out var state);

        if (!request.Query.TryGetValue("response_type", out var responseType))
		{
			return Results.BadRequest(new
			{
				error = "invalid_request",
				state,
				iss
			});
		}
		
		request.Query.TryGetValue("client_id", out var clientId);
		request.Query.TryGetValue("code_challenge", out var codeChallenge);
		request.Query.TryGetValue("code_challenge_method", out var codeChallengeMethod);
		request.Query.TryGetValue("redirect_uri", out var redirecUri);
		request.Query.TryGetValue("scope", out var scope);

		var protector = dataProtectionProvider.CreateProtector("oauth");
		var code = new AuthCode
		{
			ClientId = clientId,
			CodeChallenge = codeChallenge,
			CodeChallengeMethod = codeChallengeMethod,
			RedirectUri = redirecUri,
			Expiry = DateTime.Now.AddMinutes(5)
		};

		var codeString = protector.Protect(JsonSerializer.Serialize(code));

		return Results.Redirect($"{redirecUri}?code={codeString}&state={state}&iss={HttpUtility.UrlEncode("http://localhost:5001")}");
	}
}
