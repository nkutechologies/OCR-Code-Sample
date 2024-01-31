using Octopus.Dtos.DxInfoDtos;
using Octopus.Dtos.EncounterInfoDtos;
using Octopus.Dtos.InsuranceDtos;
using Octopus.Dtos.PatientConditionsDtos;
using Octopus.Dtos.ServicesDtos;
using System.Collections.Generic;

namespace Octopus.Dtos.ValidateRulesDtos
{
    public class ValidateRulesDto
    {
        public string requestId { get; set; } = string.Empty;
        public PatientInfoDto patientInfo { get; set; }
        public InsuranceDto insurance { get; set; }
        public EncounterInfoDto encounterInfo { get; set; }
        public List<dxInfoDto> dxInfos { get; set; }
        public List<ServicesDto> services { get; set; }
        public string clinicianDepartment { get; set; } = string.Empty;
    }
    public class DxInfo
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }

    public class Service
    {
        public string Code { get; set; }
        public int Qty { get; set; }
    }

    public class ReportContent
    {
        public string DisplayUrl { get; set; }
        public object Description { get; set; }
        public long Date { get; set; }
        public string Title { get; set; }
        public string VisitId { get; set; }
        public string DocLevel { get; set; }
        public string Doctor { get; set; }
        public string Provider { get; set; }
        public string Type { get; set; }
        public string JSessionId { get; set; }
    }

    public class ValidateRulesDtos
    {
        public string RequestId { get; set; }
        public string Gender { get; set; }
        public string DOB { get; set; }
        public string InsuranceName { get; set; }
        public string InsuranceLicence { get; set; }
        public string CardNo { get; set; }
        public string FacilityId { get; set; }
        public int Type { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string StartType { get; set; }
        public string EndType { get; set; }
        public string Invoice { get; set; }
        public string ClaimType { get; set; }
        public List<DxInfo> DxInfos { get; set; }
        public List<Service> Services { get; set; }
        public string ChiefComplaint { get; set; }
        public string VisitNo { get; set; }
        public string MRN { get; set; }
        public List<ReportContent> ReportContent { get; set; }
    }

}
