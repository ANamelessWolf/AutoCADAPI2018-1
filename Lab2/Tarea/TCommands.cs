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

namespace AutoCADAPI.Lab2.Tarea
{
    public class TCommands
    {
        [CommandMethod("NCasita")]
        public void DrawCasita()
        {
            Point3d insPt;
            Point3d endPt;
            if (Lab2.Selector.Point("Punto de inserción", out insPt) &&
                Lab2.Selector.Point("El tamaño de la casita", out endPt, insPt))
            {
                Casita c = new Casita(insPt.DistanceTo(endPt), insPt);
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DrawGeometry, c.Faces.ToArray());
            }
        }
        [CommandMethod("NToroide")]
        public void DrawToroide()
        {
            Point3d pt;
            Point3d endPt;
            int p;
            if (Lab2.Selector.Point("Selecciona el centro", out pt) &&
                Lab2.Selector.Point("El tamaño del toroide", out endPt, pt) &&
                Lab2.Selector.Integer("Presición del toroide", out p, 4))
            {
                double size = pt.DistanceTo(endPt);
                Toroide t = new Toroide(size, size / 2d, p, p, pt);
                t.Draw();
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DrawGeometry, t.Faces.ToArray());
            }
        }
        [CommandMethod("NSier")]
        public void DrawTriangle()
        {
            Point3d insPt;
            Point3d endPt;
            int order;
            if (Lab2.Selector.Point("Punto de inserción", out insPt) &&
                Lab2.Selector.Point("El tamaño del triangulo", out endPt, insPt) &&
                Lab2.Selector.Integer("El orden del triangulo", out order, 3))
            {
                Sierpinski triangle = new Sierpinski(insPt, insPt.DistanceTo(endPt));
                List<Entity> ents = new List<Entity>();
                triangle.Draw(triangle, ref ents, order);
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DrawGeometry, ents.ToArray());
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                ed.WriteMessage("Total de triangulos", ents.Count);
            }
        }
        /// <summary>
        /// Realiza el proceso de dibujo de la geometría
        /// </summary>
        /// <param name="doc">El documento activo.</param>
        /// <param name="tr">La transacción activa.</param>
        /// <param name="input">La entrada son las entidades a dibujar.</param>
        /// <returns>Los ids de los objetos dibujados</returns>
        private object DrawGeometry(Document doc, Transaction tr, object[] input)
        {
            Drawer drw = new Drawer(tr);
            drw.Entities(input as Entity[]);
            return drw.Ids;
        }
    }
}