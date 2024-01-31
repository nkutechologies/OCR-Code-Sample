using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.CPTSuggestionDtos
{
   public class CPTSuggestionspayLoadDto
    {
        public string clinicianDepartment { get; set; } = string.Empty;
        public string dob { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public List<string> icd { get; set; }
    }
}
