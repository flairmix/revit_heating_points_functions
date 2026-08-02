using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Fill_ADSK_Parameters
{
    public static class RevitCommandRunner
    {
        public static Result Run(
            ExternalCommandData commandData,
            ref string message,
            Action<Document> command)
        {
            try
            {
                Document doc =
                commandData.Application.ActiveUIDocument.Document;

                command(doc);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Ошибка", ex.Message);

                return Result.Failed;
            }
        }
    }
}
