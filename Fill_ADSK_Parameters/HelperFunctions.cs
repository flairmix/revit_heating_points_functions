using Autodesk.Revit.DB;

namespace Fill_ADSK_Parameters
{

    public static class HelperFunctions
    {
        public const string AdskGrouping = "ADSK_Группирование";
        public const string AdskPosition = "ADSK_Позиция";
        public const string AdskName = "ADSK_Наименование";
        public const string AdskMark = "ADSK_Марка";
        public const string AdskQuantity = "ADSK_Количество";
        public const string Comments = "Comments";

        public static string GetName(Element el)
        {

            Parameter nameParam =
            el.LookupParameter(AdskName);

            if (nameParam != null)
                return nameParam.AsString() ?? "";

            return "";

        }

        public static string GetMark(Element el)
        {

            Parameter markParam =
            el.LookupParameter(AdskMark);

            if (markParam != null)
                return markParam.AsString() ?? "";

            return "";

        }

        public static bool TrySetParameter(Element el, string parameterName, string value)
        {
            Parameter param = el.LookupParameter(parameterName);

            if (param == null || param.IsReadOnly)
                return false;

            param.Set(value);
            return true;
        }

        public static bool TrySetParameter(Element el, string parameterName, double value)
        {
            Parameter param = el.LookupParameter(parameterName);

            if (param == null || param.IsReadOnly)
                return false;

            param.Set(value);
            return true;
        }

    }
}
