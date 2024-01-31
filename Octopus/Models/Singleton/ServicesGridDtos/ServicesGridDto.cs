using System.Collections.Generic;

namespace Octopus.Models.Singleton.ServicesGridDtos
{
    public class ServicesGridDto
    {
        public string Code { get; set; }
        public string Type { get; set; }
    }
    // Custom comparer for ServicesGridDto
    class ServicesGridDtoComparer : IEqualityComparer<ServicesGridDto>
    {
        public bool Equals(ServicesGridDto x, ServicesGridDto y)
        {
            return x.Code == y.Code;
        }

        public int GetHashCode(ServicesGridDto obj)
        {
            return obj.Code.GetHashCode();
        }
    }
}
