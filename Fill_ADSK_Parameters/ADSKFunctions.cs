using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Fill_ADSK_Parameters
{

    public static class ADSKFunctions
    {
        private static readonly BuiltInCategory[] AdskCategories =
        {
            BuiltInCategory.OST_PipeAccessory,
            BuiltInCategory.OST_PipeFitting,
            BuiltInCategory.OST_PipeCurves,
            BuiltInCategory.OST_MechanicalEquipment,
            BuiltInCategory.OST_DuctCurves,
            BuiltInCategory.OST_DuctAccessory,
            BuiltInCategory.OST_DuctFitting,
            BuiltInCategory.OST_PipeInsulations
        };

        private static readonly Dictionary<BuiltInCategory, string> GroupingValues =
        new Dictionary<BuiltInCategory, string>
        {
            { BuiltInCategory.OST_PipeAccessory, "2" },
            { BuiltInCategory.OST_PipeCurves, "3" },
            { BuiltInCategory.OST_PipeFitting, "4" },
            { BuiltInCategory.OST_MechanicalEquipment, "1" },
            { BuiltInCategory.OST_DuctCurves, "3" },
            { BuiltInCategory.OST_DuctAccessory, "2" },
            { BuiltInCategory.OST_DuctFitting, "4" },
            { BuiltInCategory.OST_PipeInsulations, "5" }
        };

        private static readonly Dictionary<string, int> PositionRules =
        new Dictionary<string, int>
        {
            { "Насос циркуляционный", 2 },
            { "Насос", 2 },
            { "Клапан предохранительный", 3 },
            { "Предохранительный клапан", 3 },
            { "Двухходовой регулирующий седельный клапан", 4 },
            { "Двухходовой регулирующий", 4 },
            { "Двухходовой седельный", 4 },
            { "Регулирующий седельный клапан", 4 },
            { "Клапан регулятор перепада", 4 },
            { "Клапан электромагнитный", 4 },
            { "Электромагнитный клапан", 4 },
            { "Клапан балансировочный", 5 },
            { "Клапан перепада", 6 },
            { "Обратный клапан муфтовый", 7 },
            { "Клапан обратный", 7 },
            { "Обратный клапан", 7 },
            { "Клапан обратный муфтовый", 7 },
            { "Грязевик", 8 },
            { "Фильтр сетчатый", 9 },
            { "Фильтр", 9 },
            { "Гибкая антивибрационная вставка", 10 },
            { "Гибкая антивибрационная", 10 },
            { "Гибкая вставка", 10 },
            { "Вибровставка", 10 },
            { "Бак расширительный", 11 },
            { "Расширительный бак", 11 },
            { "Бак мембранный", 11 },
            { "Узел учета тепла", 13 },
            { "Узел учета", 13 },
            { "Расходомер", 14 },
            { "Счетчик", 14 },
            { "Термометр", 15 },
            { "Манометр", 16 },
            { "Кран для манометра", 17 },
            { "Кран муфтовый", 18 },
            { "Кран шаровой Standard Hidraulica", 18 },
            { "Шаровый кран", 18 },
            { "Шаровой кран", 18 },
            { "Кран шаровой", 18 },
            { "Кран шаровый", 18 },
            { "Затвор дисковый", 19 },
            { "Затворы", 19 },
            { "Труба прямошовная", 20 },
            { "Труба стальная", 20 },
            { "Труба бесшовная", 20 },
            { "Отвод 90°", 21 },
            { "Отвод 90", 21 },
            { "Отвод стальной", 21 },
            { "Отвод 45°", 22 },
            { "Отвод 45", 22 },
            { "Тройник стальной", 23 },
            { "Тройник равнопроходной", 23 },
            { "Тройник", 23 },
            { "Фланец", 24 },
            { "Переход концентрический", 25 },
            { "Переход стальной", 25 },
            { "Переход", 25 },
            { "Заглушка", 26 },
            { "Опора скользящая", 30 },
            { "Опора подвижная", 30 },
            { "Опора бескорпусная", 30 },
            { "Хомут трубный", 30 },
            { "Опора неподвижная", 31 },
            { "Теплоизоляция трубопровода", 37 },
            { "Цилиндры кашированные фольгой", 37 },
            { "Цилиндры кашированные", 37 },
            { "Цилиндры теплоизоляционные", 37 },
            { "Крепеж теплоизоляции трубопровода", 38 },
            { "Крепеж теплоизоляции", 33 },
            { "Лента для теплоизоляции", 34 },
            { "Антикоррозийная защита", 36 },
            { "Теплообменник разборный пластинчатый", 1 },
            { "Теплообменник", 1 },
            { "Клапан регулирующий", 4 },
            { "АУПД", 12 },
            { "Хомут", 30 },
            { "Теплоизоляция", 32 },
            { "Воздухоотводчик", 35 },
            { "Колонна", 40 },
            { "Балка", 40 },
            { "Уголок", 40 }
        };

        public static void FillADSKGrouping(Document doc)
        {
            int updated = 0;

            using (Transaction t =
            new Transaction(doc, "ADSK Группирование"))
            {

                t.Start();

                foreach (BuiltInCategory bic in AdskCategories)
                {
                    if (!GroupingValues.TryGetValue(bic, out string value))
                        continue;

                    FilteredElementCollector collector =
                    new FilteredElementCollector(doc)
                    .OfCategory(bic)
                    .WhereElementIsNotElementType();

                    foreach (Element el in collector)
                    {

                        if (HelperFunctions.TrySetParameter(el, HelperFunctions.AdskGrouping, value))
                            updated++;

                    }

                }

                t.Commit();

            }

            TaskDialog.Show("Готово", $"Заполнено ADSK_Группирование: {updated}");

        }

        public static void ADSK_Позиция_Fill(Document doc)
        {
            FillAndRenumberPositions(doc);

        }

        public static void RenumberGroupedPositions(Document doc)
        {
            FillAndRenumberPositions(doc);

        }

        public static void FillAndRenumberPositions(Document doc)
        {
            List<PositionElementData> items =
            GetPositionElementData(doc).ToList();

            Dictionary<PositionElementData, string> basePositions =
            new Dictionary<PositionElementData, string>();

            int cleared = 0;
            int matched = 0;
            int updated = 0;

            using (Transaction t =
            new Transaction(doc, "Заполнение и перенумерация ADSK Позиция"))
            {

                t.Start();

                foreach (PositionElementData item in items)
                {
                    Parameter posParam =
                    item.PositionParameter;

                    if (posParam == null || posParam.IsReadOnly)
                        continue;

                    posParam.Set("");
                    cleared++;

                    string name =
                    item.SearchText;

                    if (TryGetBasePosition(name, out string basePosition))
                    {
                        basePositions[item] = basePosition;
                        matched++;
                    }
                }

                foreach (var kvp in basePositions
                .GroupBy(x => x.Value)
                .OrderBy(x => int.Parse(x.Key)))
                {

                    string basePosition =
                    kvp.Key;

                    var subGroups =
                    kvp.GroupBy(x =>
                    new
                    {

                        Name = x.Key.GroupName,
                        Mark = x.Key.Mark

                    })
                    .OrderBy(g => g.Key.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(g => g.Key.Mark, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(g => g.Min(x => x.Key.SortId))
                    .ToList();

                    int index = 1;

                    foreach (var g in subGroups)
                    {

                        string newPos =
                        $"{basePosition}.{index}";

                        foreach (var item in g)
                        {

                            Parameter posParam =
                            item.Key.PositionParameter;

                            if (posParam != null &&
                            !posParam.IsReadOnly)
                            {
                                posParam.Set(newPos);
                                updated++;
                            }

                        }

                        index++;

                    }

                }

                t.Commit();

            }

            TaskDialog.Show("Готово",
            $"Очищено ADSK_Позиция: {cleared}\nПодобрано базовых позиций: {matched}\nЗаполнено ADSK_Позиция: {updated}");

        }

        private static bool TryGetBasePosition(string name, out string basePosition)
        {
            basePosition = "";

            if (string.IsNullOrEmpty(name))
                return false;

            string normalizedName =
            NormalizePositionText(name);

            foreach (var kvp in PositionRules.OrderByDescending(x => x.Key.Length))
            {
                string normalizedRule =
                NormalizePositionText(kvp.Key);

                if (normalizedName.IndexOf(
                normalizedRule,
                StringComparison.OrdinalIgnoreCase) >= 0)
                {

                    basePosition =
                    kvp.Value.ToString();

                    return true;

                }

            }

            return false;
        }

        private static IEnumerable<PositionElementData> GetPositionElementData(Document doc)
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
                    SearchText = GetPositionSearchText(doc, el, type),
                    GroupName = GetPositionGroupName(doc, el, type),
                    Mark = GetParameterString(el, type, HelperFunctions.AdskMark),
                    SortId = positionTarget.Id.Value
                };
            }
        }

        private static string GetPositionSearchText(Document doc, Element el)
        {
            return GetPositionSearchText(doc, el, doc.GetElement(el.GetTypeId()));
        }

        private static string GetPositionSearchText(Document doc, Element el, Element type)
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

        private static string GetPositionGroupName(Document doc, Element el)
        {
            return GetPositionGroupName(doc, el, doc.GetElement(el.GetTypeId()));
        }

        private static string GetPositionGroupName(Document doc, Element el, Element type)
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

        private static string NormalizePositionText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            string normalized =
            value.Replace('ё', 'е')
            .Replace('Ё', 'Е')
            .ToLowerInvariant();

            while (normalized.Contains("  "))
                normalized = normalized.Replace("  ", " ");

            return normalized.Trim();
        }

        private class PositionElementData
        {
            public Parameter PositionParameter { get; set; }
            public string SearchText { get; set; }
            public string GroupName { get; set; }
            public string Mark { get; set; }
            public long SortId { get; set; }
        }


        public static void Comments_inside(Document doc)
        {

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance));

            int updated = 0;
            StringBuilder errors = new StringBuilder();

            using (Transaction t =
            new Transaction(doc, "Копирование Comments в вложенные семейства"))
            {

                t.Start();

                foreach (FamilyInstance fi in collector)
                {

                    try
                    {

                        FamilyInstance parent =
                        fi.SuperComponent as FamilyInstance;

                        if (parent == null)
                            continue;



                        Parameter parentComm =
                        parent.LookupParameter(HelperFunctions.Comments);

                        if (parentComm == null)
                            continue;

                        string val =
                        parentComm.AsString();

                        if (string.IsNullOrEmpty(val))
                            continue;



                        Parameter childComm =
                        fi.LookupParameter(HelperFunctions.Comments);

                        if (childComm != null &&
                        !childComm.IsReadOnly)
                        {

                            childComm.Set(val);
                            updated++;

                        }

                    }
                    catch (Exception ex)
                    {

                        errors.AppendLine(
                        $"Эл.{fi.Id.Value}: {ex.Message}");

                    }

                }

                t.Commit();

            }

            string msg =
            $"Обновлено вложенных семейств: {updated}";

            if (errors.Length > 0)
                TaskDialog.Show("Готово с ошибками",
                msg + "\n\n" + errors.ToString());
            else
                TaskDialog.Show("Готово", msg);

        }

        private static IEnumerable<Element> GetAdskElements(Document doc)
        {
            foreach (BuiltInCategory bic in AdskCategories)
            {
                foreach (Element el in new FilteredElementCollector(doc)
                    .OfCategory(bic)
                    .WhereElementIsNotElementType())
                {
                    yield return el;
                }
            }
        }
    }
}
