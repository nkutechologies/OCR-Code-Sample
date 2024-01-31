using Newtonsoft.Json.Linq;
namespace Octopus.Dtos.OctopusConfigDtos
{
    public class SectionData
    {
        public int DoubleClickDelay { get; set; }
        public int InitialDelay { get; set; }
        public Coordinates Coordinates { get; set; }
        public JToken SectionConfig { get; set; }
        //public Drag Drag { get; set; }
        public ContextMenuActions contextMenuActions { get; set; }
    }
    public class Coordinates
    {
        public int InitialX { get; set; }
        public int InitialY { get; set; }
    }
    public class ContextMenuActions
    {
        public int InitialX { get; set; }
        public int InitialY { get; set; }
    }
    //public class Drag
    //{
    //    public int Distance { get; set; }
    //    public int SpeedOfDrag { get; set; }
    //    public int loopIncrementBy { get; set; }
    //}
}
