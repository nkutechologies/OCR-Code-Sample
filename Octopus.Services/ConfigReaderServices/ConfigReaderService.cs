using Newtonsoft.Json.Linq;
using Octopus.Dtos.OctopusConfigDtos;
using System;
using System.Linq;

namespace Octopus.Services.ConfigReaderServices
{
    public class ConfigReaderService: IConfigReaderService
    {
        public SectionData GetSectionData(string sectionName, JObject config, bool isCoordinatesNeeded,bool isDoubleClickDelayNeeded,bool isContextMenuActionsNeeded)
        {
            try
            {
                var sectionConfig = config["ReadingSection"].FirstOrDefault(x => (string)x["Section"] == sectionName);

                if (sectionConfig != null)
                {
                    var sectionData = new SectionData
                    {
                        InitialDelay = (int)sectionConfig["InitialThreadSleeper"]["Halt"],
                        SectionConfig=sectionConfig,
                        contextMenuActions=new ContextMenuActions(),
                        Coordinates=new Coordinates()
                    };

                    if (isCoordinatesNeeded)
                    {
                        sectionData.Coordinates = new Coordinates
                        {
                            InitialX = (int)sectionConfig["Coordinates"]["InitialX"],
                            InitialY = (int)sectionConfig["Coordinates"]["InitialY"]
                        };
                    }

                    if (isDoubleClickDelayNeeded)
                    {
                        sectionData.DoubleClickDelay = (int)sectionConfig["DoubleClickDelay"]["Halt"];
                    }

                    if (isContextMenuActionsNeeded)
                    {
                        sectionData.contextMenuActions.InitialX = (int)sectionConfig["ContextMenuActions"]["InitialX"];
                        sectionData.contextMenuActions.InitialY = (int)sectionConfig["ContextMenuActions"]["InitialY"];
                    }

                    return sectionData;
                }
                else
                {
                    //_log.Info($"{sectionName} configuration not found in the JSON.");
                    return null;
                }
            }
            catch (Exception)
            {
               // _log.Info("Logged datetime:" + DateTime.Now.ToString("yyyy-MM-dd HHmmss"));
               // _log.Error(x);
                return null;
            }
        }
    }
}
