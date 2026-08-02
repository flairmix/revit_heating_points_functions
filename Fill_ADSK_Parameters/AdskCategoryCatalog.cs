using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Fill_ADSK_Parameters
{
    public static class AdskCategoryCatalog
    {
        public static readonly BuiltInCategory[] Categories =
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

        public static readonly Dictionary<BuiltInCategory, string> GroupingValues =
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
    }
}
