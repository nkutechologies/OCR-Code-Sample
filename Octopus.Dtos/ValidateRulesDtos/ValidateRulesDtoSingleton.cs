using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Dtos.ValidateRulesDtos
{
    public class ValidateRulesDtoSingleton
    {
        private static ValidateRulesDtoSingleton instance;
        public ValidateRulesDto ValidateRulesDtoInstance { get; private set; }

        private ValidateRulesDtoSingleton()
        {
            // Initialize your ValidateRulesDto here if needed
            ValidateRulesDtoInstance = new ValidateRulesDto();
        }

        public static ValidateRulesDtoSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ValidateRulesDtoSingleton();
                }
                return instance;
            }
        }
    }

}
