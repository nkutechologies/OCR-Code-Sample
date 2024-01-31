using Newtonsoft.Json.Linq;
using Octopus.Dtos.Common;
using Octopus.Dtos.ConditionsDtos;
using Octopus.Services.ApiConsumerManagerServices;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Services.PatientConditionsServices
{
    public class PatientConditionsService
    {
        public static ResponseModel<List<GroupedConditionsDto>> GetPatientConditions(string requestBody, string apiBaseUrl, string token)
        {
            string requestPayload = requestBody;
            using (var apiClient = new ApiClientService())
            {
                var response = apiClient.HttpPostClientAsync<dynamic>(apiBaseUrl + "api/Octopus/PatientConditions", requestPayload, token).GetAwaiter().GetResult();

                if (response != null && response.Response == "SUCCESS" && response.Data.data != null && response.Data.data.Count > 0)
                {
                    var groupedConditionsList = TransformJsonToGroupedConditions(response.Data);

                    return new ResponseModel<List<GroupedConditionsDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = groupedConditionsList,
                        Request = requestPayload
                    };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return new ResponseModel<List<GroupedConditionsDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<GroupedConditionsDto>(),
                        Request = requestPayload
                    };
                }
                else
                {
                    return new ResponseModel<List<GroupedConditionsDto>>
                    {
                        Response = response.Response,
                        StatusCode = response.StatusCode,
                        Message = response.Message,
                        Data = new List<GroupedConditionsDto>(),
                        Request = requestPayload
                    };
                }
            }
        }

        private static List<GroupedConditionsDto> TransformJsonToGroupedConditions(dynamic jsonData)
        {
            JArray dataArray = jsonData.data;
            var groupedConditionsList = new List<GroupedConditionsDto>();

            foreach (var group in dataArray.GroupBy(item => (string)item["groupId"]))
            {
                var conditions = group.SelectMany(item => item["conditions"].ToObject<List<ConditionDto>>()).ToList();

                var groupedConditions = new GroupedConditionsDto
                {
                    GroupId = group.Key,
                    GroupCount = conditions.Count,
                    Conditions = conditions
                };

                groupedConditionsList.Add(groupedConditions);
            }

            return groupedConditionsList;
        }
    }
}
