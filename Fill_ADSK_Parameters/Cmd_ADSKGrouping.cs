using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace Fill_ADSK_Parameters
{
    [Transaction(TransactionMode.Manual)]
    public class Cmd_ADSKGrouping : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            return RevitCommandRunner.Run(
                commandData,
                ref message,
                ADSKFunctions.FillADSKGrouping);
        }
    }
}
