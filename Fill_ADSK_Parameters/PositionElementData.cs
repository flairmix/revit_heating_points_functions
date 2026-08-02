using Autodesk.Revit.DB;

namespace Fill_ADSK_Parameters
{
    public class PositionElementData
    {
        public Parameter PositionParameter { get; set; }
        public string SearchText { get; set; }
        public string GroupName { get; set; }
        public string Mark { get; set; }
        public long SortId { get; set; }
    }
}
