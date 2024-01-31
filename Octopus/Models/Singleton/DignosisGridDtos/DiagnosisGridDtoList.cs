using System.Collections.Generic;
using System.Linq;

namespace Octopus.Models.Singleton.DignosisGridDtos
{
    public class DiagnosisGridDtoList
    {
        private readonly List<DiagnosisGridDto> diagnosisList = new List<DiagnosisGridDto>();

        // Private constructor to prevent external instantiation
        private DiagnosisGridDtoList()
        {
        }

        // Private static instance of the class
        private static DiagnosisGridDtoList instance = new DiagnosisGridDtoList();

        // Public static method to access the single instance
        public static DiagnosisGridDtoList GetInstance()
        {
            return instance;
        }

        // Public method to add a DiagnosisGridDto object to the list
        public void AddDiagnosis(DiagnosisGridDto diagnosis)
        {
            diagnosisList.Add(diagnosis);
        }

        // Public method to add a list of DiagnosisGridDto objects to the list
        public void AddDiagnosisList(IEnumerable<DiagnosisGridDto> diagnoses)
        {
            diagnosisList.AddRange(diagnoses);
        }

        // Public method to retrieve the list of DiagnosisGridDto objects
        public List<DiagnosisGridDto> GetDiagnosisList()
        {
            return diagnosisList;
        }
        // Public method to clear the list of DiagnosisGridDto objects
        public void ClearDiagnosisList()
        {
            diagnosisList.Clear();
        }

        public void RemoveByCodes(IEnumerable<string> codesToRemove)
        {
            diagnosisList.RemoveAll(diagnosis => codesToRemove.Contains(diagnosis.Code));
        }
    }
}
