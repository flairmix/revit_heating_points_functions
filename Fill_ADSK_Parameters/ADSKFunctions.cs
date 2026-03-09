using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Fill_ADSK_Parameters
{

    public static class ADSKFunctions
    {

        public static void FillADSKGrouping(Document doc)
        {

            BuiltInCategory[] categories =
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

            using (Transaction t =
            new Transaction(doc, "ADSK Группирование"))
            {

                t.Start();

                foreach (BuiltInCategory bic in categories)
                {

                    FilteredElementCollector collector =
                    new FilteredElementCollector(doc)
                    .OfCategory(bic)
                    .WhereElementIsNotElementType();

                    foreach (Element el in collector)
                    {

                        Parameter groupParam =
                        el.LookupParameter("ADSK_Группирование");

                        if (groupParam == null || groupParam.IsReadOnly)
                            continue;

                        string value = "";

                        switch (bic)
                        {

                            case BuiltInCategory.OST_PipeAccessory:
                                value = "2"; break;

                            case BuiltInCategory.OST_PipeCurves:
                                value = "3"; break;

                            case BuiltInCategory.OST_PipeFitting:
                                value = "4"; break;

                        }

                        if (!string.IsNullOrEmpty(value))
                            groupParam.Set(value);

                    }

                }

                t.Commit();

            }

        }

        public static void ADSK_Позиция_Fill(Document doc)
        {

            Dictionary<string, int> dict =
            new Dictionary<string, int>()
            {

                {"Теплообменник", 1},
                {"Насос", 2},
                {"Насос циркуляционный", 2},

                {"Клапан предохранительный", 3},
                {"Предохранительный клапан", 3},

                {"Клапан регулирующий", 4},
                {"Двухходовой седельный", 4},
                {"Двухходовой регулирующий", 4},


                {"Клапан регулятор перепада", 4},

                {"Клапан электромагнитный", 4},

                {"Клапан балансировочный", 5},
                {"Клапан перепада", 6},
                {"Клапан обратный", 7},
                {"обратный клапан ", 7},

                {"Грязевик", 8},
                {"Фильтр", 9},
                {"Вибровставка", 10},
                {"Гибкая вставка", 10},
                {"Гибкая антивибрационная", 10},

                {"Бак расширительный", 11},
                {"АУПД", 12},
                {"Узел учета тепла", 13},
                {"Расходомер", 14},
                {"Счетчик", 14},

                {"Термометр", 15},
                {"Манометр", 16},
                {"Кран для манометра", 17},
                {"Шаровый кран", 18},
                {"шаровой кран", 18},
                {"Кран шаровой", 18},
                {"кран шаровый", 18},

                {"Затвор дисковый", 19},
                {"Труба стальная", 20},
                {"Отвод 90", 21},
                {"Отвод 45", 22},
                {"Тройник", 23},
                {"Фланец", 24},
                {"Переход", 25},
                {"Заглушка", 26},
                {"Опора скользящая", 30},
                {"Опора подвижная", 30},
                {"Хомут", 30},
                {"Хомут трубный", 30},

                {"Опора неподвижная", 31},
                {"Теплоизоляция", 32},
                {"Крепеж теплоизоляции", 33},
                {"Лента для теплоизоляции", 34},
                {"Воздухоотводчик", 35},
                {"Антикоррозийная защита", 36},
                {"Теплоизоляция трубопровода", 37},
                {"Цилиндры кашированные фольгой", 37},
                {"Крепеж теплоизоляции трубопровода", 38},
                {"Колонна", 40},
                {"Балка", 40},
                {"Уголок", 40},

            };

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .WhereElementIsNotElementType();

            using (Transaction t =
            new Transaction(doc, "Заполнение ADSK Позиция"))
            {

                t.Start();

                foreach (Element el in collector)
                {

                    Parameter posParam =
                    el.LookupParameter("ADSK_Позиция");

                    if (posParam == null || posParam.IsReadOnly)
                        continue;

                    string name =
                    HelperFunctions.GetName(el);

                    if (string.IsNullOrEmpty(name))
                        continue;

                    foreach (var kvp in dict)
                    {

                        if (name.IndexOf(
                        kvp.Key,
                        StringComparison.OrdinalIgnoreCase) >= 0)
                        {

                            posParam.Set(kvp.Value.ToString());
                            break;

                        }

                    }

                }

                t.Commit();

            }

        }

        public static void RenumberGroupedPositions(Document doc)
        {

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .WhereElementIsNotElementType();

            Dictionary<string, List<Element>> groups =
            new Dictionary<string, List<Element>>();

            foreach (Element el in collector)
            {

                Parameter posParam =
                el.LookupParameter("ADSK_Позиция");

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

                foreach (var kvp in groups)
                {

                    string basePos = kvp.Key;

                    var subGroups =
                    kvp.Value.GroupBy(e =>
                    new
                    {

                        Name = HelperFunctions.GetName(e),
                        Mark = HelperFunctions.GetMark(e)

                    }).ToList();

                    int index = 1;

                    foreach (var g in subGroups)
                    {

                        string newPos =
                        $"{basePos}.{index}";

                        foreach (Element el in g)
                        {

                            Parameter posParam =
                            el.LookupParameter("ADSK_Позиция");

                            if (posParam != null &&
                            !posParam.IsReadOnly)
                                posParam.Set(newPos);

                        }

                        index++;

                    }

                }

                t.Commit();

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
                        parent.LookupParameter("Comments");

                        if (parentComm == null)
                            continue;

                        string val =
                        parentComm.AsString();

                        if (string.IsNullOrEmpty(val))
                            continue;



                        Parameter childComm =
                        fi.LookupParameter("Comments");

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
                        $"Эл.{fi.Id.IntegerValue}: {ex.Message}");

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
    }
}