using System;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Connection : IExternalCommand
    {
        public class FamilyFilter : ISelectionFilter
        {
            bool ISelectionFilter.AllowElement(Element elem)
            {
                if (elem.Name == "30Degree")
                {
                    return true;
                }
                else if (elem.Name == "60Degree")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
            {
                return false;
            }
        }

        public struct LBeacon
        {
            public string[] Neighbors;
            public Parameter Mark;
            public Parameter Neighbor;
            public XYZ Point;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            FamilyFilter ff = new FamilyFilter();
            IList<Reference> sel = uidoc.Selection.PickObjects(ObjectType.Element, ff);
            List<LBeacon> LBeacons = new List<LBeacon>();

            try
            {
                foreach (Reference r in sel)
                {
                    Element beacon = doc.GetElement(r);
                    LBeacon lBeacon = new LBeacon();
                    lBeacon.Mark = beacon.LookupParameter("Mark");
                    lBeacon.Neighbor = beacon.LookupParameter("Neighbor");
                    lBeacon.Neighbors = lBeacon.Neighbor.AsString().Split('/');
                    LocationPoint locationPoint = beacon.Location as LocationPoint;
                    lBeacon.Point = locationPoint.Point;
                    LBeacons.Add(lBeacon);
                }

                using (Transaction t = new Transaction(doc, "connect"))
                {
                    t.Start("connect");
                    bool isExisted;
                    for (int i = 0; i < LBeacons.Count; i++)
                    {
                        for (int j = i + 1; j < LBeacons.Count; j++)
                        {
                            isExisted = false;
                            foreach (string neighbor in LBeacons[i].Neighbors)
                            {
                                if (neighbor == LBeacons[j].Mark.AsString())
                                {
                                    isExisted = true;
                                }
                            }

                            if (!isExisted)
                            {
                                LBeacons[i].Neighbor.Set(
                                    LBeacons[i].Neighbor.AsString() + LBeacons[j].Mark.AsString() + '/');
                                LBeacons[j].Neighbor.Set(
                                    LBeacons[j].Neighbor.AsString() + LBeacons[i].Mark.AsString() + '/');
                                Line line = Line.CreateBound(LBeacons[i].Point, LBeacons[j].Point);
                                doc.Create.NewDetailCurve(uidoc.ActiveView, line);
                            }
                        }
                    }
                    t.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Error", e.ToString());
            }

            return Result.Succeeded;
        }
    }
}
