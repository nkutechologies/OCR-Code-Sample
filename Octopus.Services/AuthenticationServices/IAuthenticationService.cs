using Octopus.Dtos.AuthenticationDtos;
using Octopus.Dtos.Common;
using System;

namespace Octopus.Services.AuthenticationServices
{
    public interface IAuthenticationService
    {
        event Action<string> AuthTokenRetrieved;
        string GetAuthToken();
        ResponseModel<string> Authenticate(string payLoad, string apiBaseUrl);
    }
}
