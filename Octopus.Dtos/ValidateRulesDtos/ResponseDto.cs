using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.ValidateRulesDtos
{
  
    public class ItemRemoval
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string ClientCode { get; set; }
        public string ClientServiceType { get; set; }
        public string ClientDescription { get; set; }
    }

    public class ItemAddition
    {
        // Define properties for ItemAddition if available in the response
        public string Code { get; set; }
        public string Description { get; set; }
        public string ClientCode { get; set; }
        public string ClientServiceType { get; set; }
        public string ClientDescription { get; set; }
    }

    public class ResponseDto
    {
        public string Solution { get; set; }
        public string ErrorMessage { get; set; }
        public string Action { get; set; }
        public string Section { get; set; }
        public string Item { get; set; }
        public string CodesToRemove { get; set; }
        public List<string> IcdCode { get; set; }
        public List<ItemRemoval> ItemRemoval { get; set; }
        public List<ItemAddition> ItemAddition { get; set; }
    }
    public class DiscoveryRulesResponseDtos
    {
        public string requestId { get; set; }
        public List<RulesValidationResult> result { get; set; }

        public List<LabReportAbnormality> ReportAbnormality { get; set; }

        public List<RuleValidationSummary> ResultSummary { get; set; }
    }
    public class AbnormalityItem
    {
        public string Alert { get; set; }
        public string TestItem { get; set; }
    }
    public class LabReportAbnormality
    {
        public string TestName { get; set; }
        public List<AbnormalityItem> TestItems { get; set; }
    }
    public class RuleValidationSummary
    {
        public string RuleCode { get; set; }

        public string RuleName { get; set; }

        public int Count { get; set; }

        public int Rank { get; set; }
    }
    public class RulesValidationResult
    {
        public int id { get; set; }
        public string ruleName { get; set; }
        public string ruleCode { get; set; }

        public string errorMessage { get; set; }

        public string longErrorMessage { get; set; }

        public string priority { get; set; }

        public string solution { get; set; }

        public string action { get; set; }

        public string item { get; set; }

        public string itemDescription { get; set; }

        public int ICDIsSigns { get; set; }

        public string HISCusId { get; set; }

        public string Section { get; set; }
        public string Qty { get; set; }

        public string ReplaceWith { get; set; }

        public List<string> replaceCodes { get; set; }

        public string ReplaceWithHISCusId { get; set; }

        public string CustHISServiceType { get; set; }

        public string ReplaceCusHISServiceType { get; set; }

        public string CustHISDescription { get; set; }

        public string ReplaceCusHISDescription { get; set; }

        public List<string> icdCode { get; set; }

        public List<string> codes { get; set; }

        public int ruleRank { get; set; }

        public List<ItemRemoval> ItemRemoval { get; set; }
        public List<ItemAddition> ItemAddition { get; set; }
    }
}
