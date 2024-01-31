using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.PatientDtos
{
    public class CptIcdAndDOb
    {
        public List<CPT> CPT { get; set; }
        public List<Dignosis> Dignoses { get; set; }
        public PatientPersonalInfo PatientPersonalInfo { get; set; }
        public Encounter Encounter { get; set; }
    }
    public class CPT
    {
        public string CPTCode { get; set; }
        public int Quantity { get; set; }
        public string Type  { get; set; }
    }
    public class Dignosis
    {
        public string ICDType { get; set; }
        public string ICDCode { get; set; }
    }
    public class PatientPersonalInfo
    {
        public string Id { get; set; }
        public string DOB { get; set; }
    }
    public class Encounter
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
