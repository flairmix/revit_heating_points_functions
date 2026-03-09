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

            Document doc =
            commandData.Application.ActiveUIDocument.Document;

            ADSKFunctions.FillADSKGrouping(doc);

            return Result.Succeeded;

        }

    }
}