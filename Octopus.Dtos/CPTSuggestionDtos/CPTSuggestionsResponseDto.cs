using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.CPTSuggestionDtos
{
    public class CPTSuggestionsResponseDto
    {
        public string serviceType { get; set; }
        public List<items> items { get; set; }
    }
    public class items
    {
        public string code { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string custHISCPTId { get; set; }
        public string custHISServiceType { get; set; }
    }
}
