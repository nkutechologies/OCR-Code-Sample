using Newtonsoft.Json;
using Octopus.Dtos.Common;
using Octopus.Dtos.CPTSuggestionDtos;
using Octopus.Services.ApiConsumerManagerServices;
using System.Collections.Generic;

namespace Octopus.Services.CPTSuggestionServices
{
   public class CPTSuggestionService
    {
        public static ResponseModel<List<CPTSuggestionsResponseDto>> GetCPTSuggestion(string requestPayload, string token, string apiBaseUrl)
        {
            using (var apiClient = new ApiClientService())
            {
                // Pass the payload as the requestData
                var response = apiClient.HttpPostClientAsync<dynamic>(apiBaseUrl + "api/Octopus/CPTSuggestion", requestPayload, token).GetAwaiter().GetResult();

                if (response != null && response.Response == "SUCCESS" && response.Data.data != null && response.Data.data.Count>0)
                {
                    List<CPTSuggestionsResponseDto> cpTSuggestionsResponseList = JsonConvert.DeserializeObject<List<CPTSuggestionsResponseDto>>(response.Data.data.ToString());
                  
                    return new ResponseModel<List<CPTSuggestionsResponseDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = cpTSuggestionsResponseList
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    // Handle other error cases
                    return new ResponseModel<List<CPTSuggestionsResponseDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<CPTSuggestionsResponseDto>(),
                        Request = requestPayload
                    };
                }
                else
                {
                    // Handle other error cases
                    return new ResponseModel<List<CPTSuggestionsResponseDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<CPTSuggestionsResponseDto>(),
                        Request = requestPayload
                    };
                }
            }
        }

    }
}
