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

            int quantityUpdated = 0;
            int nameUpdated = 0;
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

                        Parameter qParam =
                        ins.LookupParameter(HelperFunctions.AdskQuantity);

                        if (qParam != null && !qParam.IsReadOnly)
                        {
                            qParam.Set(Math.Round(lengthMeters, 2));
                            quantityUpdated++;
                        }

                        Parameter nameParam =
                        ins.LookupParameter(HelperFunctions.AdskName);

                        if (nameParam != null && !nameParam.IsReadOnly)
                        {
                            string typeComment =
                            GetTypeComment(doc, ins);

                            if (!string.IsNullOrEmpty(typeComment) &&
                            TryGetHostPipeDiameterText(doc, ins, out string diameterText))
                            {
                                nameParam.Set($"{typeComment} для труб Ø{diameterText}");
                                nameUpdated++;
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine($"Элемент {ins.Id}: {ex.Message}");
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

        private static string GetTypeComment(Document doc, Element el)
        {
            ElementType type =
            doc.GetElement(el.GetTypeId()) as ElementType;

            if (type == null)
                return "";

            Parameter typeCommentsParam =
            type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS);

            if (typeCommentsParam == null)
                return "";

            return typeCommentsParam.AsString() ?? "";
        }

        private static bool TryGetHostPipeDiameterText(
            Document doc,
            PipeInsulation ins,
            out string diameterText)
        {
            diameterText = "";

            Element host =
            doc.GetElement(ins.HostElementId);

            if (host == null)
                return false;

            Parameter diameterParam =
            host.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

            if (diameterParam == null || !diameterParam.HasValue)
            {
                diameterParam =
                host.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER);
            }

            if (diameterParam == null || !diameterParam.HasValue)
                return false;

            double diameterMm =
            UnitUtils.ConvertFromInternalUnits(
            diameterParam.AsDouble(),
            UnitTypeId.Millimeters);

            double roundedDiameter =
            Math.Round(diameterMm, 1);

            if (Math.Abs(roundedDiameter - Math.Round(roundedDiameter)) < 0.001)
                diameterText = Math.Round(roundedDiameter).ToString("0");
            else
                diameterText = roundedDiameter.ToString("0.#");

            return true;
        }

    }
}
