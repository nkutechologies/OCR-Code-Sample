using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.ICDSuggestionsDtos
{
    public class ICDSuggestionDto
    {
        public string Text { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; } = string.Empty;
        public string ChiefComplaint { get; set; } 
        public List<string> Conditions { get; set; } 
    }
}
