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

        [CommandMethod("NPolygon")]
        public void DrawPolygon()
        {
            int sides;
            Point3d basePt, endPt;
            Double apo;
            if (Selector.Point("Selecciona el centro del poligono", out basePt) &&
                Selector.Point("Selecciona el punto final del apotema", out endPt, basePt)
                && Selector.Integer("Dame el número de lados", out sides, 3))
            {
                double angle = 2 * Math.PI;
                double x, y, z = 0;
                angle = angle / sides;
                apo = basePt.DistanceTo(endPt);
                Point3dCollection pts = new Point3dCollection();
                for (int i = 0; i < sides; i++)
                {
                    x = basePt.X + apo * Math.Cos(angle * i);
                    y = basePt.Y + apo * Math.Sin(angle * i);
                    pts.Add(new Point3d(x, y, z));
                }
                TransactionWrapper t = new TransactionWrapper();
                t.Run(DrawPolygon, new Object[] { pts });
            }
        }


        [CommandMethod("NPyramid")]
        public void DrawPyramid()
        {
            Point3d o;
            Double n, h;
            if (Selector.Point("Selecciona el centro de la piramide", out o) &&
                Selector.Double("Dame el tamaño de la base", out n) &&
                Selector.Double("Dame el tamaño de la altura", out h))
            {
                Point3d[] pts = new Point3d[]
                {
                    o + new Vector3d(-n/2,-n/2,0),
                    o + new Vector3d(n/2,-n/2,0),
                    o + new Vector3d(n/2,n/2,0),
                    o + new Vector3d(-n/2,n/2,0),
                    o + new Vector3d(0,0,h)
                };
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DrawPyramidTask, new Object[] { pts });
            }
        }


        [CommandMethod("InsertarBloque")]
        public void InsertBlock()
        {
            System.Windows.Forms.OpenFileDialog dia =
                new System.Windows.Forms.OpenFileDialog();
            dia.Multiselect = false;
            dia.Filter = "*.dwg";
            if (dia.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && dia.FileName != null && dia.FileName.Length > 0)
            {
                string pth = dia.FileName;
                Point3d insPt;
                if (Selector.Point("Selecciona el punto de inserción del bloque", out insPt))
                {
                    TransactionWrapper tr = new TransactionWrapper();
                    tr.Run(InsertarBloqueTask, new Object[] { pth, insPt });
                }
            }
        }

        private object InsertarBloqueTask(Document doc, Transaction tr, object[] input)
        {
            String pth = input[0] as String;
            Point3d insPt = (Point3d)input[1];
            if (BlockManager.LoadBlock(pth, "OR", doc, tr))
            {
                //Se crea la referencia de bloque
                BlockReference insBlk = BlockManager.InsertBlock("OR", insPt, doc, tr);
                //Se dibuja el bloque
                Drawer dw = new Drawer(tr);
                dw.Entity(insBlk);
                //Se agregan los parámetros del bloque
                BlockManager.SetAttribute(insBlk, "INPUTA", "0", doc, tr);
                BlockManager.SetAttribute(insBlk, "INPUTB", "1", doc, tr);
                BlockManager.SetAttribute(insBlk, "OUTPUT", "1", doc, tr);
            }
            else
                doc.Editor.WriteMessage("Error al cargar el bloque");
            return null;
        }

        private object DrawPyramidTask(Document doc, Transaction tr, object[] input)
        {
            Drawer drawer = new Drawer(tr);
            Point3d[] pts = (Point3d[])input[0];
            Autodesk.AutoCAD.Colors.Color green = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0);
            drawer.Quad(pts[0], pts[1], pts[2], pts[3], green); //ABCD
            drawer.Triangle(pts[0], pts[1], pts[4], green); //ABE
            drawer.Triangle(pts[1], pts[2], pts[4], green); //BCE
            drawer.Triangle(pts[2], pts[3], pts[4], green); //CDE
            drawer.Triangle(pts[3], pts[0], pts[4], green); //DAE
            return null;
        }

        private object DrawPolygon(Document doc, Transaction tr, object[] input)
        {
            Drawer draw = new Drawer(tr);
            draw.Geometry(input[0] as Point3dCollection);
            return null;
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
