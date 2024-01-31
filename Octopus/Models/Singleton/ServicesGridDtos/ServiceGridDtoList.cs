using System.Collections.Generic;
using System.Linq;

namespace Octopus.Models.Singleton.ServicesGridDtos
{
    public class ServiceGridDtoList
    {

        private readonly List<ServicesGridDto> servicesList = new List<ServicesGridDto>();

        // Private constructor to prevent external instantiation
        private ServiceGridDtoList()
        {
        }

        // Private static instance of the class
        private static ServiceGridDtoList instance = new ServiceGridDtoList();

        // Public static method to access the single instance
        public static ServiceGridDtoList GetInstance()
        {
            return instance;
        }

        // Public method to add a DiagnosisGridDto object to the list
        public void AddDiagnosis(ServicesGridDto diagnosis)
        {
            servicesList.Add(diagnosis);
        }

        // Public method to add a list of DiagnosisGridDto objects to the list
        public void AddServiceList(IEnumerable<ServicesGridDto> diagnoses)
        {
            servicesList.AddRange(diagnoses);
        }

        // Public method to retrieve the list of DiagnosisGridDto objects
        public List<ServicesGridDto> GetServiceList()
        {
            return servicesList;
        }
        public void ClearServiceList()
        {
            servicesList.Clear();
        }
        public void RemoveByCodes(IEnumerable<string> codesToRemove)
        {
            servicesList.RemoveAll(diagnosis => codesToRemove.Contains(diagnosis.Code));
        }
    }
}
