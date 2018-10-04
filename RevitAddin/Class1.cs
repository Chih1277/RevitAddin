using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            const double angleConvert = 180 / Math.PI;

            SiteLocation site = doc.SiteLocation;
            double projectLongitude = site.Longitude * angleConvert;
            double projectLatitude = site.Latitude * angleConvert;
            XYZ location = new XYZ(projectLongitude, projectLatitude, 0);

            Selection sel = uidoc.Selection;
            Reference reference = sel.PickObject(ObjectType.Element);
            Element element = doc.GetElement(reference);
            Parameter comment = element.LookupParameter("Comments");
            Parameter mark = element.LookupParameter("Mark");
            LocationPoint lp = element.Location as LocationPoint;
            XYZ startPoint = new XYZ(lp.Point.X, lp.Point.Y, lp.Point.Z);

            using (Transaction t = new Transaction(doc, "parameter"))
            {
                t.Start("parameter");
                comment.Set("Selected");

                if (GetParameterValue(comment) != "Selected")
                {
                    TextNote.Create(doc, uidoc.ActiveView.Id, startPoint, GetParameterValue(mark), doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType));
                }

                t.Commit();
            }

            reference = sel.PickObject(ObjectType.Element);
            element = doc.GetElement(reference);
            lp = element.Location as LocationPoint;
            XYZ endPoint = new XYZ(lp.Point.X, lp.Point.Y, lp.Point.Z);
            comment = element.LookupParameter("Comments");

            using (Transaction t = new Transaction(doc, "line"))
            {
                t.Start("line");
                comment.Set("Selected");
                Line line = Line.CreateBound(startPoint, endPoint);
                doc.Create.NewDetailCurve(uidoc.ActiveView, line);
                t.Commit();
            }

            return Result.Succeeded;
        }

        public string GetParameterValue(Parameter parameter)
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
