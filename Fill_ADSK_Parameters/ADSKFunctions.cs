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
        public static void FillADSKGrouping(Document doc)
        {
            int updated = 0;

            using (Transaction t =
            new Transaction(doc, "ADSK Группирование"))
            {

                t.Start();

                foreach (BuiltInCategory bic in AdskCategoryCatalog.Categories)
                {
                    if (!AdskCategoryCatalog.GroupingValues.TryGetValue(bic, out string value))
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

        public static void FillAdskPosition(Document doc)
        {
            FillBasePositions(doc);

        }

        public static void FillBasePositions(Document doc)
        {
            List<PositionElementData> items =
            PositionElementReader.GetPositionElementData(doc).ToList();

            int matched = 0;
            int updated = 0;
            int preserved = 0;

            using (Transaction t =
            new Transaction(doc, "Заполнение базовой ADSK Позиция"))
            {

                t.Start();

                foreach (PositionElementData item in items)
                {
                    Parameter posParam =
                    item.PositionParameter;

                    if (posParam == null || posParam.IsReadOnly)
                        continue;

                    if (!string.IsNullOrWhiteSpace(posParam.AsString()))
                    {
                        preserved++;
                        continue;
                    }

                    string name =
                    item.SearchText;

                    if (PositionRuleCatalog.TryGetBasePosition(name, out string basePosition))
                    {
                        posParam.Set(basePosition);
                        matched++;
                        updated++;
                    }
                }

                t.Commit();

            }

            TaskDialog.Show("Готово",
            $"Правил позиций загружено: {PositionRuleCatalog.RuleCount}\nCSV: {PositionRuleCatalog.SourcePath}\nСохранено существующих ADSK_Позиция: {preserved}\nПодобрано базовых позиций: {matched}\nЗаполнено ADSK_Позиция: {updated}");

        }

        public static void CopyCommentsToNestedFamilies(Document doc)
        {

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance));

            int updated = 0;
            int preserved = 0;
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
                            if (!string.IsNullOrWhiteSpace(childComm.AsString()))
                            {
                                preserved++;
                                continue;
                            }

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
            $"Заполнено вложенных семейств: {updated}\nСохранено существующих Comments: {preserved}";

            if (errors.Length > 0)
                TaskDialog.Show("Готово с ошибками",
                msg + "\n\n" + errors.ToString());
            else
                TaskDialog.Show("Готово", msg);

        }

    }
}
