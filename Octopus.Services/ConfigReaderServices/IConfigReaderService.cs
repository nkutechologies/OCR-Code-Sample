using Newtonsoft.Json.Linq;
using Octopus.Dtos.OctopusConfigDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Services.ConfigReaderServices
{
    public interface IConfigReaderService
    {
        SectionData GetSectionData(string sectionName, JObject config, bool isCoordinatesNeeded, bool isDoubleClickDelayNeeded, bool isContextMenuActionsNeeded);
    }
}
