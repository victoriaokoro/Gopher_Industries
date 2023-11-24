using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using foodremedy.api.Models.Requests;
using foodremedy.api.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace foodremedy.api.tests.Controllers;

internal sealed class AuthenticationControllerTests : ControllerTestFixture
{
    private const string _email = "test@test.com";
    private const string _password = "password";
    
    [Test]
    public async Task AttemptLogin_ValidDetails_ReturnsAccessToken()
    {
        await CreateTestUser();

        var response = await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(_email, _password)));
        var result = await response.Content.ReadFromJsonAsync<AccessTokenCreated>();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(result?.AccessToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.ExpiresIn.Should().BeGreaterThan(0);
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.TokenType.Should().Be("Bearer");
        jwtToken.Audiences.Should().ContainSingle().Which.Should().Be("FoodRemedy-API");
        jwtToken.Issuer.Should().Be("FoodRemedy-API");
        jwtToken.ValidFrom.Should().BeCloseTo(DateTime.Now.ToUniversalTime(), TimeSpan.FromMinutes(1));
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.Now.AddHours(1).ToUniversalTime(), TimeSpan.FromMinutes(1));
        jwtToken.Subject.Should().NotBeNullOrWhiteSpace();
        jwtToken.Claims.Should().Satisfy(
            claim => claim.Type == "sub" && claim.Value == jwtToken.Subject,
            claim => claim.Type == "jti" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "iat" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "subject" && claim.Value == jwtToken.Subject,
            claim => claim.Type == "nbf" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "exp" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "iss" && claim.Value == "FoodRemedy-API",
            claim => claim.Type == "aud" && claim.Value == "FoodRemedy-API"
        );
    }

    private async Task CreateTestUser()
    {
        await _webApiClient.PostAsync("/users/register", JsonContent.Create(new RegisterUser(_email, _password)));
    }

    [Test]
    public async Task AttemptLogin_InvalidLogin_ReturnsUnauthorised()
    {
        const string email = "test@test.com";
        const string password = "password";

        var response = await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(email, password)));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task RefreshAccessToken_Unauthenticated_ReturnsUnauthorised()
    {
        var response = await _webApiClient.PostAsync("auth/refresh", JsonContent.Create(
            new RefreshAccessToken("someToken")
        ));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Test]
    public async Task RefreshAccessToken_UserDoesNotExist_ReturnsUnauthorised()
    {
        await CreateTestUser();
        var loginResponse = await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(_email, _password)));
        var result = await loginResponse.Content.ReadFromJsonAsync<AccessTokenCreated>();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(result?.AccessToken);
        
        var user = await DbContext.User.SingleOrDefaultAsync(p => p.Id == Guid.Parse(jwtToken.Subject));
        ArgumentNullException.ThrowIfNull(user);
        DbContext.Remove(user);
        await DbContext.SaveChangesAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, "auth/refresh")
        {
            Content = JsonContent.Create(new RefreshAccessToken(result.RefreshToken)),
            Headers = { {"Authorization", $"Bearer {result.AccessToken}"} }
        };

        var response = await _webApiClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Test]
    public async Task RefreshAccessToken_RefreshTokenIsInvalid_ReturnsUnauthorised()
    {
        await CreateTestUser();
        var loginResponse = await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(_email, _password)));
        var result = await loginResponse.Content.ReadFromJsonAsync<AccessTokenCreated>();
        
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/refresh")
        {
            Content = JsonContent.Create(new RefreshAccessToken("invalid refresh token")),
            Headers = { {"Authorization", $"Bearer {result.AccessToken}"} }
        };
        
        var response = await _webApiClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Test]
    public async Task RefreshAccessToken_ValidRequest_ReturnsOkWithAccessTokenCreated()
    {
        await CreateTestUser();
        var loginResponse = await _webApiClient.PostAsync("auth/login", JsonContent.Create(new AttemptLogin(_email, _password)));
        var accessTokenCreated = await loginResponse.Content.ReadFromJsonAsync<AccessTokenCreated>();
        
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/refresh")
        {
            Content = JsonContent.Create(new RefreshAccessToken(accessTokenCreated.RefreshToken)),
            Headers = { {"Authorization", $"Bearer {accessTokenCreated.AccessToken}"} }
        };
        
        var response = await _webApiClient.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<AccessTokenCreated>();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(result?.AccessToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.ExpiresIn.Should().BeGreaterThan(0);
        result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.TokenType.Should().Be("Bearer");
        jwtToken.Audiences.Should().ContainSingle().Which.Should().Be("FoodRemedy-API");
        jwtToken.Issuer.Should().Be("FoodRemedy-API");
        jwtToken.ValidFrom.Should().BeCloseTo(DateTime.Now.ToUniversalTime(), TimeSpan.FromMinutes(1));
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.Now.AddHours(1).ToUniversalTime(), TimeSpan.FromMinutes(1));
        jwtToken.Subject.Should().NotBeNullOrWhiteSpace();
        jwtToken.Claims.Should().Satisfy(
            claim => claim.Type == "sub" && claim.Value == jwtToken.Subject,
            claim => claim.Type == "jti" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "iat" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "subject" && claim.Value == jwtToken.Subject,
            claim => claim.Type == "nbf" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "exp" && !string.IsNullOrWhiteSpace(claim.Value),
            claim => claim.Type == "iss" && claim.Value == "FoodRemedy-API",
            claim => claim.Type == "aud" && claim.Value == "FoodRemedy-API"
        );
    }
}
