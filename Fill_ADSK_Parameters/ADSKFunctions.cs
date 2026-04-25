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
            { "Клапан предохранительный", 3 },
            { "Предохранительный клапан", 3 },
            { "Двухходовой регулирующий", 4 },
            { "Двухходовой седельный", 4 },
            { "Клапан регулятор перепада", 4 },
            { "Клапан электромагнитный", 4 },
            { "Клапан балансировочный", 5 },
            { "Клапан перепада", 6 },
            { "Клапан обратный", 7 },
            { "Обратный клапан", 7 },
            { "Гибкая антивибрационная", 10 },
            { "Гибкая вставка", 10 },
            { "Бак расширительный", 11 },
            { "Узел учета тепла", 13 },
            { "Кран для манометра", 17 },
            { "Шаровый кран", 18 },
            { "Шаровой кран", 18 },
            { "Кран шаровой", 18 },
            { "Кран шаровый", 18 },
            { "Затвор дисковый", 19 },
            { "Труба стальная", 20 },
            { "Отвод 90", 21 },
            { "Отвод 45", 22 },
            { "Опора скользящая", 30 },
            { "Опора подвижная", 30 },
            { "Хомут трубный", 30 },
            { "Опора неподвижная", 31 },
            { "Теплоизоляция трубопровода", 37 },
            { "Цилиндры кашированные фольгой", 37 },
            { "Крепеж теплоизоляции трубопровода", 38 },
            { "Крепеж теплоизоляции", 33 },
            { "Лента для теплоизоляции", 34 },
            { "Антикоррозийная защита", 36 },
            { "Теплообменник", 1 },
            { "Насос", 2 },
            { "Клапан регулирующий", 4 },
            { "Грязевик", 8 },
            { "Фильтр", 9 },
            { "Вибровставка", 10 },
            { "АУПД", 12 },
            { "Расходомер", 14 },
            { "Счетчик", 14 },
            { "Термометр", 15 },
            { "Манометр", 16 },
            { "Тройник", 23 },
            { "Фланец", 24 },
            { "Переход", 25 },
            { "Заглушка", 26 },
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
            int updated = 0;

            using (Transaction t =
            new Transaction(doc, "Заполнение ADSK Позиция"))
            {

                t.Start();
                foreach (Element el in GetAdskElements(doc))
                {

                    Parameter posParam =
                    el.LookupParameter(HelperFunctions.AdskPosition);

                    if (posParam == null || posParam.IsReadOnly)
                        continue;

                    string name =
                    HelperFunctions.GetName(el);

                    if (string.IsNullOrEmpty(name))
                        continue;

                    foreach (var kvp in PositionRules.OrderByDescending(x => x.Key.Length))
                    {

                        if (name.IndexOf(
                        kvp.Key,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                        {

                            posParam.Set(kvp.Value.ToString());
                            updated++;
                            break;

                        }

                    }

                }

                t.Commit();

            }

            TaskDialog.Show("Готово", $"Заполнено ADSK_Позиция: {updated}");

        }

        public static void RenumberGroupedPositions(Document doc)
        {

            Dictionary<string, List<Element>> groups =
            new Dictionary<string, List<Element>>();

            foreach (Element el in GetAdskElements(doc))
            {

                Parameter posParam =
                el.LookupParameter(HelperFunctions.AdskPosition);

                if (posParam == null) continue;

                string posValue =
                posParam.AsString();

                if (string.IsNullOrEmpty(posValue))
                    continue;

                if (!groups.ContainsKey(posValue))
                    groups[posValue] =
                    new List<Element>();

                groups[posValue].Add(el);

            }

            using (Transaction t =
            new Transaction(doc, "Перенумерация"))
            {

                t.Start();

                int updated = 0;

                foreach (var kvp in groups.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
                {

                    string basePos = kvp.Key;

                    var subGroups =
                    kvp.Value.GroupBy(e =>
                    new
                    {

                        Name = HelperFunctions.GetName(e),
                        Mark = HelperFunctions.GetMark(e)

                    })
                    .OrderBy(g => g.Key.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(g => g.Key.Mark, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(g => g.Min(e => e.Id.Value))
                    .ToList();

                    int index = 1;

                    foreach (var g in subGroups)
                    {

                        string newPos =
                        $"{basePos}.{index}";

                        foreach (Element el in g)
                        {

                            Parameter posParam =
                            el.LookupParameter(HelperFunctions.AdskPosition);

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

                TaskDialog.Show("Готово", $"Перенумеровано элементов: {updated}");

            }

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
