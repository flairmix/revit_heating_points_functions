using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Fill_ADSK_Parameters
{
    public static class PositionElementReader
    {
        public static IEnumerable<PositionElementData> GetPositionElementData(Document doc)
        {
            HashSet<ElementId> handledPositionTargets =
            new HashSet<ElementId>();

            foreach (Element el in GetAdskElements(doc))
            {
                Element type =
                doc.GetElement(el.GetTypeId());

                Parameter positionParam =
                el.LookupParameter(HelperFunctions.AdskPosition);

                Element positionTarget =
                el;

                if (positionParam == null && type != null)
                {
                    positionParam =
                    type.LookupParameter(HelperFunctions.AdskPosition);

                    if (positionParam != null)
                        positionTarget = type;
                }

                if (positionParam == null || positionParam.IsReadOnly)
                    continue;

                if (!handledPositionTargets.Add(positionTarget.Id))
                    continue;

                yield return new PositionElementData
                {
                    PositionParameter = positionParam,
                    SearchText = GetPositionSearchText(el, type),
                    GroupName = GetPositionGroupName(el, type),
                    Mark = GetParameterString(el, type, HelperFunctions.AdskMark),
                    SortId = positionTarget.Id.Value
                };
            }
        }

        private static IEnumerable<Element> GetAdskElements(Document doc)
        {
            foreach (BuiltInCategory bic in AdskCategoryCatalog.Categories)
            {
                foreach (Element el in new FilteredElementCollector(doc)
                    .OfCategory(bic)
                    .WhereElementIsNotElementType())
                {
                    yield return el;
                }
            }
        }

        private static string GetPositionSearchText(Element el, Element type)
        {
            List<string> parts =
            new List<string>();

            AddText(parts, GetParameterString(el, type, HelperFunctions.AdskName));
            AddText(parts, GetParameterString(el, type, HelperFunctions.AdskMark));
            AddText(parts, el.Name);

            if (type != null)
            {
                AddText(parts, type.Name);

                Parameter typeCommentsParam =
                type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                if (typeCommentsParam != null)
                    AddText(parts, typeCommentsParam.AsString());
            }

            return string.Join(" ", parts);
        }

        private static string GetPositionGroupName(Element el, Element type)
        {
            string name =
            GetParameterString(el, type, HelperFunctions.AdskName);

            if (!string.IsNullOrEmpty(name))
                return name;

            if (type != null && !string.IsNullOrEmpty(type.Name))
                return type.Name;

            return el.Name ?? "";
        }

        private static string GetParameterString(Element el, Element type, string parameterName)
        {
            Parameter param =
            el.LookupParameter(parameterName);

            if (param != null)
                return param.AsString() ?? "";

            if (type != null)
            {
                param =
                type.LookupParameter(parameterName);

                if (param != null)
                    return param.AsString() ?? "";
            }

            return "";
        }

        private static void AddText(List<string> parts, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                parts.Add(value);
        }
    }
}
