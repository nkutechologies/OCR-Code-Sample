using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.ConditionsDtos
{
    public class GroupedConditionsDto
    {
        public string GroupId { get; set; }
        public int GroupCount { get; set; }
        public List<ConditionDto> Conditions { get; set; }
    }

    public class ConditionDto
    {
        public string Condition { get; set; }
    }
}
