using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAddin
{
    class LBeacon
    {
        public LBeacon(FamilyInstance fi)
        {
            Parameter mark = fi.LookupParameter("Mark");
            if (mark != null)
            {
                TaskDialog.Show("Mark", GetParameterValue(mark));
            }
        }

        public string Mark
        {
            get; private set;
        }

        public string GUID
        {
            get; private set;
        }

        public string BeaconType
        {
            get; private set;
        }

        public double XLocation
        {
            get; private set;
        }

        public double YLocation
        {
            get; private set;
        }

        public string Level
        {
            get; private set;
        }

        private string GetParameterValue(Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.Double:
                    return parameter.AsValueString();
                case StorageType.ElementId:
                    return parameter.AsElementId().IntegerValue.ToString();
                case StorageType.Integer:
                    return parameter.AsValueString();
                case StorageType.None:
                    return parameter.AsValueString();
                case StorageType.String:
                    return parameter.AsString();
                default:
                    return "";
            }
        }
    }
}
