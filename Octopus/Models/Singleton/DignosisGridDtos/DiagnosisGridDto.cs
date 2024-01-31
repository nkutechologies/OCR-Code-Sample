using System.Collections.Generic;

namespace Octopus.Models.Singleton.DignosisGridDtos
{


    public class DiagnosisGridDto
    {
        public string Code { get; set; }
        public string Type { get; set; }

    }
    class DiagnosisGridDtoComparer : IEqualityComparer<DiagnosisGridDto>
    {
        public bool Equals(DiagnosisGridDto x, DiagnosisGridDto y)
        {
            return x.Code == y.Code;
        }

        public int GetHashCode(DiagnosisGridDto obj)
        {
            return obj.Code.GetHashCode();
        }
    }
}
