using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using System;
using System.Text;

namespace Fill_ADSK_Parameters
{

    public static class PipeFunctions
    {

        public static void FillPipesLength(Document doc)
        {

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .OfClass(typeof(Pipe));

            StringBuilder errors = new StringBuilder();
            int quantityUpdated = 0;
            int nameUpdated = 0;

            using (Transaction t =
            new Transaction(doc, "Заполнение ADSK параметров для труб"))
            {

                t.Start();

                foreach (Pipe pipe in collector)
                {

                    try
                    {

                        Parameter qParam =
                        pipe.LookupParameter(HelperFunctions.AdskQuantity);

                        if (qParam != null && !qParam.IsReadOnly)
                        {

                            double lengthInternal =
                            pipe.get_Parameter(
                            BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                            double lengthMeters =
                            UnitUtils.ConvertFromInternalUnits(
                            lengthInternal,
                            UnitTypeId.Meters);

                            qParam.Set(Math.Round(lengthMeters, 2));
                            quantityUpdated++;

                        }


                        Parameter nameParam =
                        pipe.LookupParameter(HelperFunctions.AdskName);

                        if (nameParam != null && !nameParam.IsReadOnly)
                        {

                            PipeType pipeType =
                            doc.GetElement(pipe.GetTypeId()) as PipeType;

                            if (pipeType == null) continue;

                            string typeComment = "";

                            Parameter typeCommentsParam =
                            pipeType.get_Parameter(
                            BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

                            if (typeCommentsParam != null)
                                typeComment = typeCommentsParam.AsString() ?? "";

                            double outerDiameterMm =
                            UnitUtils.ConvertFromInternalUnits(
                            pipe.get_Parameter(
                            BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble(),
                            UnitTypeId.Millimeters);

                            Parameter innerDiameterParam =
                            pipe.get_Parameter(BuiltInParameter.RBS_PIPE_INNER_DIAM_PARAM);

                            if (innerDiameterParam == null)
                                continue;

                            double innerDiameterMm =
                            UnitUtils.ConvertFromInternalUnits(
                            innerDiameterParam.AsDouble(),
                            UnitTypeId.Millimeters);

                            double wallThicknessMm =
                            (outerDiameterMm - innerDiameterMm) / 2.0;

                            string newName =
                            $"{typeComment} dn{Math.Round(outerDiameterMm)}x{Math.Round(wallThicknessMm, 1)}";

                            nameParam.Set(newName);
                            nameUpdated++;

                        }

                    }
                    catch (Exception ex)
                    {

                        errors.AppendLine($"Элемент {pipe.Id}: {ex.Message}");

                    }

                }

                t.Commit();

            }

            string msg =
            $"ADSK_Количество обновлено: {quantityUpdated}\nADSK_Наименование обновлено: {nameUpdated}";

            if (errors.Length > 0)
                TaskDialog.Show("Готово с ошибками", msg + "\n\n" + errors.ToString());
            else
                TaskDialog.Show("Готово", msg);

        }

        public static void pipesLenght(Document doc)
        {
            FillPipesLength(doc);
        }

        public static void FillPipeInsulationADSK(Document doc)
        {

            FilteredElementCollector collector =
            new FilteredElementCollector(doc)
            .OfClass(typeof(PipeInsulation));

            int updated = 0;
            StringBuilder errors = new StringBuilder();

            using (Transaction t =
            new Transaction(doc, "Заполнение ADSK параметров для изоляции"))
            {

                t.Start();

                foreach (PipeInsulation ins in collector)
                {

                    try
                    {

                        double lengthInternal =
                        ins.get_Parameter(
                        BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();

                        double lengthMeters =
                        UnitUtils.ConvertFromInternalUnits(
                        lengthInternal,
                        UnitTypeId.Meters);

                        double withReserve =
                        Math.Round(lengthMeters * 1.1, 2);

                        Parameter qParam =
                        ins.LookupParameter(HelperFunctions.AdskQuantity);

                        if (qParam != null && !qParam.IsReadOnly)
                        {
                            qParam.Set(withReserve);
                            updated++;
                        }

                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine($"Элемент {ins.Id}: {ex.Message}");
                    }

                }

                t.Commit();

            }

            string msg = $"Обновлено изоляций: {updated}";

            if (errors.Length > 0)
                TaskDialog.Show("Готово с ошибками", msg + "\n\n" + errors.ToString());
            else
                TaskDialog.Show("Готово", msg);

        }

    }
}
