using Octopus.Helpers.CustomHelpers.EnumHelpers;
using System;

namespace Octopus.Services.HelperServices
{
    public class HelperService: IHelperService
    {
        public string GetEnumText(Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attr = fieldInfo.GetCustomAttributes(typeof(EnumTextAttribute), false) as EnumTextAttribute[];

            return attr.Length > 0 ? attr[0].Text : value.ToString();
        }
    }
}
