using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.ServicesDtos
{
   public class ServicesDto
    {
        public string type { get; set; } = "3";
        public string code { get; set; } = string.Empty;
        public string qty { get; set; } = "1";
        public string priorApproval { get; set; } = "true";
        public string dateOfService { get; set; } = DateTime.Now.ToString("dd-MM-yyyy");
        public string clinician { get; set; } = string.Empty;
    }
}
