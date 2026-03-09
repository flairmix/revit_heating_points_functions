using Autodesk.Revit.DB;

namespace Fill_ADSK_Parameters
{

    public static class HelperFunctions
    {

        public static string GetName(Element el)
        {

            Parameter nameParam =
            el.LookupParameter("ADSK_Наименование");

            if (nameParam != null)
                return nameParam.AsString();

            return "";

        }

        public static string GetMark(Element el)
        {

            Parameter markParam =
            el.LookupParameter("ADSK_Марка");

            if (markParam != null)
                return markParam.AsString();

            return "";

        }

    }
}