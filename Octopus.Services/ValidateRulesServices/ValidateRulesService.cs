using Newtonsoft.Json;
using Octopus.Dtos.Common;
using Octopus.Dtos.ValidateRulesDtos;
using Octopus.Services.ApiConsumerManagerServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Services.ValidateRulesServices
{
    public class ValidateRulesService
    {
        public static ResponseModel<List<RulesValidationResult>> Validate(string requestPayLoad, string token, string apiBaseUrl)
        {
            using (var apiClient = new ApiClientService())
            {
                var response = apiClient.HttpPostClientAsync<ResponseModel<DiscoveryRulesResponseDtos>>(apiBaseUrl + "api/Octopus/Codify", requestPayLoad, token).GetAwaiter().GetResult();

                if (response != null && response.Data.Data != null && response.Data.Data.result!=null && response.Data.Data.result.Any())
                {
                    return new ResponseModel<List<RulesValidationResult>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = response.Data.Data.result.Where(x => !string.IsNullOrEmpty(x.action)).ToList(),
                        Request = requestPayLoad
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    // Handle other error cases
                    return new ResponseModel<List<RulesValidationResult>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<RulesValidationResult>(),
                        Request = requestPayLoad
                    };
                }
                else
                {
                    // Handle other error cases
                    return new ResponseModel<List<RulesValidationResult>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<RulesValidationResult>(),
                        Request = requestPayLoad
                    };
                }
            }
        }
    }
}
