using Octopus.Dtos.ICDCodeDtos;
using Octopus.Services.ApiConsumerManagerServices;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Octopus.Dtos.Common;

namespace Octopus.Services.ICDSuggestionsServices
{
    public class ICDSuggestionsService
    {
        public static ResponseModel<List<ICDCodesDto>> Get(string payLoad, string apiBaseUrl,string token)
        {
            using (var apiClient = new ApiClientService())
            {
                // Pass the payload as the requestData
                var response = apiClient.HttpPostClientAsync<dynamic>(apiBaseUrl + "api/Octopus/GetICDSuggestion", payLoad, token).GetAwaiter().GetResult();

                if (response != null && response.Data.response == "SUCCESS" && response.Data.data != null)
                {
                    JArray dataArray = response.Data.data;

                    List<ICDCodesDto> icdList = dataArray
                        .Select(item => new ICDCodesDto
                        {
                            Code = (string)item["code"],
                            Description = (string)item["description"]
                        })
                        .ToList();

                    // Handle other error cases
                    return new ResponseModel<List<ICDCodesDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = icdList,
                        Request = payLoad
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    // Handle other error cases
                    return new ResponseModel<List<ICDCodesDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<ICDCodesDto>(),
                        Request = payLoad
                    };
                }
                else
                {
                    // Handle other error cases
                    return new ResponseModel<List<ICDCodesDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<ICDCodesDto>(),
                        Request = payLoad
                    };
                }
            }
        }
    }
}
