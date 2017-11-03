using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Espacios de nombre de AutoCAD
using Autodesk.AutoCAD.ApplicationServices;             //La administración de la aplicación.
using Autodesk.AutoCAD.DatabaseServices;                //Accede a la BD de AutoCAD
using Autodesk.AutoCAD.EditorInput;                     //La interacción del usuario con AutoCAD
using Autodesk.AutoCAD.Geometry;                        //Las clases auxiliares para manejar geometría.
using Autodesk.AutoCAD.Runtime;                         //Cachar excepciones de AutoCAD y definir comandos.

namespace AutoCADAPI.Lab2
{
    public class Commands
    {
        [CommandMethod("NCircle")]
        public void DrawCircle()
        {
            Point3d center, endPt;
            Double radio;
            if (Selector.Point("Selecciona el centro del punto", out center) &&
                Selector.Point("Selecciona el punto final del radio", out endPt, center))
            {
                radio = center.DistanceTo(endPt);
                TransactionWrapper t = new TransactionWrapper();
                t.Run(DrawCircleTask, new Object[] { center, radio });
            }

        }

        private object DrawCircleTask(Document doc, Transaction tr, object[] input)
        {
            Point3d center = (Point3d)input[0];
            Double radio = (Double)input[1];
            Circle c = new Circle(center, Vector3d.ZAxis, radio);
            BlockTable blockTable = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            DBObject obj = blockTable[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForWrite);
            if (obj is BlockTableRecord)
            {
                BlockTableRecord modelSpace = (BlockTableRecord)obj;
                modelSpace.AppendEntity(c);
                tr.AddNewlyCreatedDBObject(c, true);
            }
            return null;
        }
    }
}
