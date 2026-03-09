using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace Fill_ADSK_Parameters
{

    [Transaction(TransactionMode.Manual)]
    public class Cmd_CopyComments : IExternalCommand
    {

        public Result Execute(
        ExternalCommandData commandData,
        ref string message,
        ElementSet elements)
        {

            Document doc =
            commandData.Application.ActiveUIDocument.Document;

            ADSKFunctions.Comments_inside(doc);

            return Result.Succeeded;

        }

    }
}