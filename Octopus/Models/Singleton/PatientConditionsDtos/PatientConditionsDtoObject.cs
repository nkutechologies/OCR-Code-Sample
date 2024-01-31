namespace Octopus.Models.Singleton.PatientConditionsDtos
{
    public class PatientConditionsDtoObject
    {
        private PatientConditionsDto patientConditions;

        // Private constructor to prevent external instantiation
        private PatientConditionsDtoObject()
        {
        }

        // Private static instance of the class
        private static PatientConditionsDtoObject instance = new PatientConditionsDtoObject();

        // Public static method to access the single instance
        public static PatientConditionsDtoObject GetInstance()
        {
            return instance;
        }

        // Public method to set the single instance of PatientConditionsDto
        public void SetPatientConditions(PatientConditionsDto patientConditionsDto)
        {
            patientConditions = patientConditionsDto;
        }

        // Public method to retrieve the single instance of PatientConditionsDto
        public PatientConditionsDto GetPatientConditions()
        {
            return patientConditions;
        }

        // Public method to clear the single instance of PatientConditionsDto
        public void ClearPatientConditions()
        {
            patientConditions = null;
        }
    }
}