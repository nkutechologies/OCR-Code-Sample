using Newtonsoft.Json;
using Octopus.Dtos.AuthenticationDtos;
using Octopus.Dtos.Common;
using Octopus.Helpers.CustomHelpers.Authentication.Login.Alerts;
using Octopus.Services.ApiConsumerManagerServices;
using Octopus.Services.AuthenticationServices;
using System;
public class AuthenticationService : IAuthenticationService,IDisposable
{
    public event Action<string> AuthTokenRetrieved;
    private readonly string _apiBaseURL;
    private readonly Lazy<string> _authToken;
    private readonly string _authUserId;
    private readonly string _authLoginPassword;

    // Constructor with parameters
    public AuthenticationService(string apiBaseURL, string authUserId, string authLoginPassword)
    {
        _authUserId = authUserId;
        _authLoginPassword = authLoginPassword;

        // Set the initial token from the constructor
        _authToken = new Lazy<string>(() => FetchAuthToken());
        _apiBaseURL = apiBaseURL;
    }

    public ResponseModel<string> Authenticate(string payLoad, string apiBaseUrl)
    {
        using (var apiClient = new ApiClientService())
        {
            var response = apiClient.HttpPostClientAsync<dynamic>(
                apiBaseUrl + "api/User/GetAuthorization",
               payLoad,
                 string.Empty
            ).GetAwaiter().GetResult();

            if (response != null && response.Message == "SUCCESS" && response.Data!= null)
            {
                return new ResponseModel<string>
                {
                    Response = response.Response,
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    Data = response.Data.data.accessToken,
                    Request = payLoad
                };
            }
            else
            {
                // Handle other error cases
                return new ResponseModel<string>
                {
                    Response = response.Response,
                    StatusCode = response.StatusCode,
                    Message = response.Message,
                    Data = Alert.LoginFailedMessage,
                    Request = payLoad
                };
            }
        }
    }

    public string GetAuthToken()
    {
        var authToken = _authToken.Value;

        // Notify UI layer about successful token retrieval
        AuthTokenRetrieved?.Invoke(authToken);

        return authToken;
    }

    private string FetchAuthToken()
    {
        if (!string.IsNullOrEmpty(_authUserId) && !string.IsNullOrEmpty(_authLoginPassword))
        {
            var authToken = Authenticate(JsonConvert.SerializeObject(new AuthenticationDto
            {
                UserId = _authUserId,
                Password = _authLoginPassword
            }), _apiBaseURL);
            return authToken.Data;
        }
        else
        {
            return Alert.UsernamePasswordIsRequired;
        }
    }

    public void Dispose()
    {
        // It's common to suppress finalization if resources have been explicitly released
        GC.SuppressFinalize(this);
    }
}
