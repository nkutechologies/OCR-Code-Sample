using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.EncounterInfoDtos
{
   public class EncounterInfoDto
    {
        public string facilityId { get; set; } = "NA";
        public string type { get; set; }
        public string start { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");
        public string end { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");
        public string startType { get; set; } = "NA";
        public string endType { get; set; } = "NA";
    }
}
