using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAddin
{
    class LBeacon
    {
        public LBeacon(Element beacon, XYZ location)
        {
            Parameter mark = beacon.LookupParameter("Mark");
            Mark = GetParameterValue(mark);
            XLocation = location.X;
            YLocation = location.Y;
            Level = beacon.Document.GetElement(beacon.LevelId).Name;
            LocationPoint locationPoint = beacon.Location as LocationPoint;
            Region = locationPoint.ToString();
        }

        public string Region
        {
            get; private set;
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

        public string Neighbors
        {
            get; private set;
        }

        public static string GetParameterValue(Parameter parameter)
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

        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement node = document.CreateElement("node");
            node.SetAttribute("id", this.GUID);
            node.SetAttribute("region", this.Region);
            node.SetAttribute("lat", this.YLocation.ToString());
            node.SetAttribute("lon", this.XLocation.ToString());
            node.SetAttribute("elevation", this.Level);
            return node;
        }
    }
}
