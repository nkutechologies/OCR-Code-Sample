namespace Octopus.Models.Singleton.PatientMpiDtos
{
    namespace Octopus.Models.Singleton.PatientMpiDtos
    {
        public class PatientMpiDtoObject
        {
            private PatientMpiDto patientMpi;

            // Private constructor to prevent external instantiation
            private PatientMpiDtoObject()
            {
            }

            // Private static instance of the class
            private static PatientMpiDtoObject instance = new PatientMpiDtoObject();

            // Public static method to access the single instance
            public static PatientMpiDtoObject GetInstance()
            {
                return instance;
            }

            // Public method to set the single instance of PatientMpiDto
            public void SetPatientMpi(PatientMpiDto patientMpiDto)
            {
                patientMpi = patientMpiDto;
            }

            // Public method to retrieve the single instance of PatientMpiDto
            public PatientMpiDto GetPatientMpi()
            {
                return patientMpi;
            }

            // Public method to clear the single instance of PatientMpiDto
            public void ClearPatientMpi()
            {
                patientMpi = null;
            }
        }
    }

}
