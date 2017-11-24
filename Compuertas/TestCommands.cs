using AutoCADAPI.Lab2;
using AutoCADAPI.Lab3.Model;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADAPI.Lab3
{
    /// <summary>
    /// Realizá la prueba de contacto de las compuertas
    /// </summary>
    public class TestCommands
    {
        /// <summary>
        /// Las compuertas insertadas en el plano
        /// </summary>
        Dictionary<Handle, Compuerta> Compuertas;
        /// <summary>
        /// Define un comando que prueba la inserción de una compuerta
        /// </summary>
        [CommandMethod("TestInsertCompuerta")]
        public void InsertCompuerta()
        {
            if (Compuertas == null)
                Compuertas = new Dictionary<Handle, Compuerta>();
            Point3d pt;
            if (Selector.Point("Selecciona el punto de inserción de la compuerta", out pt))
            {
                TransactionWrapper tr = new TransactionWrapper();
                var cmp = tr.Run(InsertCompuertaTask, new OR(), pt) as Compuerta;
                Compuertas.Add(cmp.Id, cmp);
            }
        }
        /// <summary>
        /// Define la transacción que inserta una compuerta
        /// </summary>
        /// <param name="doc">El documento activo.</param>
        /// <param name="tr">La transacción activa.</param>
        /// <param name="input">La entrada de la transacción.</param>
        /// <returns>La compuerta insertada</returns>
        private object InsertCompuertaTask(Document doc, Transaction tr, object[] input)
        {
            Compuerta cmp = (Compuerta)input[0];
            Point3d pt = (Point3d)input[1];
            cmp.Insert(pt, tr, doc);
            return cmp;
        }
        /// <summary>
        /// Realiza la prueba de contacto de la compuerta
        /// </summary>
        [CommandMethod("TestZone")]
        public void TestZone()
        {
            Point3d test_pt;
            ObjectId pickEnt;
            if (Selector.Entity("Selecciona una compuerta", out pickEnt, out test_pt))
            {
                //Buscamos la compuerta por ObjectId
                Compuerta cmp = this.Compuertas.Values.FirstOrDefault(x => x.Block.Id == pickEnt);
                Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                if (cmp != null)
                {
                    TransactionWrapper t = new TransactionWrapper();
                    //Se dibujan las zonas
                    ObjectIdCollection ids = t.Run(DrawZonesTask, cmp) as ObjectIdCollection;
                    ed.Regen();
                    //Se realizan las pruebas de contacto
                    t.Run(TestZoneTask, cmp);
                    //Se eliminan los rectangulos dibujados de la zona
                    t.Run(EraseZonesTask, ids);

                }
                else
                    ed.WriteMessage("No es una compuerta");
            }
        }
        /// <summary>
        /// Define la transacción que dibuja las áreas de contacto de la compuerta
        /// No es necesario que sean visibles para realizar la prueba.
        /// </summary>
        /// <param name="doc">El documento activo.</param>
        /// <param name="tr">La transacción activa.</param>
        /// <param name="input">La entrada de la transacción.</param>
        /// <returns>La compuerta insertada</returns>
        private object DrawZonesTask(Document doc, Transaction tr, object[] input)
        {
            Compuerta cmp = (Compuerta)input[0];
            Drawer d = new Drawer(tr);
            cmp.InitBox();
            cmp.DrawBox(d);
            return d.Ids;
        }
        /// <summary>
        /// Borra las zonas dibujadas
        /// </summary>
        /// <param name="doc">El documento activo.</param>
        /// <param name="tr">La transacción activa.</param>
        /// <param name="input">La entrada de la transacción.</param>
        private object EraseZonesTask(Document doc, Transaction tr, object[] input)
        {
            ObjectIdCollection ids = input[0] as ObjectIdCollection;
            Drawer d = new Drawer(tr);
            d.Erase(ids);
            return null;
        }
        /// <summary>
        /// Define la transacción que prueba la zona de contacto de la compuerta
        /// </summary>
        /// <param name="doc">El documento activo.</param>
        /// <param name="tr">La transacción activa.</param>
        /// <param name="input">La entrada de la transacción.</param>
        /// <returns>La compuerta insertada</returns>
        private object TestZoneTask(Document doc, Transaction tr, object[] input)
        {
            Compuerta cmp = (Compuerta)input[0];
            Point3d test_pt;
            Editor ed = doc.Editor;
            while (Selector.Point("Selecciona una zona de contacto", out test_pt, true))
            {
                string zoneName;
                Point3dCollection zone;
                cmp.GetZone(test_pt, out zoneName, out zone);
                if (zoneName == String.Empty)
                    ed.WriteMessage("\nPunto fuera de la zona");
                else
                {
                    String coords = string.Empty;
                    zone.OfType<Point3d>().ToList().ForEach(x => coords += String.Format("\n({0:N2},{1:N2})", x.X, x.Y));
                    ed.WriteMessage("\nCoordenadas:{1}\nPunto dentro de la zona {0}", zoneName, coords);
                    //ed.WriteMessage("\n{0}", zoneName);
                }
            }
            return null;
        }
        /// <summary>
        /// Realiza el calculo de una compuerta
        /// </summary>
        [CommandMethod("TestCompuerta")]
        public void TestCompuerta()
        {
            ObjectId p1Id, p2Id, cmpId;
            Point3d pt1, pt2;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (Selector.Entity("\nSelecciona un pulso", typeof(Polyline), out p1Id) &&
                Selector.Entity("\nSelecciona la entrada de conexión", out cmpId, out pt1) &&
                Selector.Entity("\nSelecciona un pulso", typeof(Polyline), out p2Id) &&
                Selector.Entity("\nSelecciona la entrada de conexión", out cmpId, out pt2))
            {
                TransactionWrapper tr = new TransactionWrapper();
                Compuerta cmp = this.Compuertas.Values.FirstOrDefault(x => x.Block.Id == cmpId);
                if (cmp != null)
                    tr.Run(TestCompuertaTask, cmp, p1Id, p2Id, pt1, pt2);
                else
                    ed.WriteMessage("No es Compuerta");
            }
        }

        private object TestCompuertaTask(Document doc, Transaction tr, object[] input)
        {
            Compuerta cmp = (Compuerta)input[0];
            cmp.InitBox();
            Polyline p1 = ((ObjectId)input[1]).GetObject(OpenMode.ForRead) as Polyline;
            Polyline p2 = ((ObjectId)input[2]).GetObject(OpenMode.ForRead) as Polyline;
            Point3d pt1 = (Point3d)input[3];
            Point3d pt2 = (Point3d)input[4];
            String zoneA, zoneB;
            //No nos interesa en este ejemplo las coordenadas
            Point3dCollection zone;
            cmp.GetZone(pt1, out zoneA, out zone);
            cmp.GetZone(pt2, out zoneB, out zone);
            InputValue inA = new InputValue() { Name = zoneA, Value = Pulso.GetValues(p1) },
                       inB = new InputValue() { Name = zoneB, Value = Pulso.GetValues(p2) };
            Boolean[] result = cmp.Solve(inA, inB);
            Drawer d = new Drawer(tr);
            Point3d pt;
            if (Selector.Point("Selecciona el punto de inserción de la salida", out pt))
            {
                Pulso p = new Pulso(pt, result);
                p.Draw(d);
                Line lA = new Line(p1.EndPoint, cmp.ConnectionPoints[inA.Name]),
                    lB = new Line(p2.EndPoint, cmp.ConnectionPoints[inB.Name]),
                    lO = new Line(pt, cmp.ConnectionPoints["OUTPUT"]);
                d.Entities(lA, lB, lO);
                cmp.SetData(tr, doc, inA.Value.LastOrDefault(), inB.Value.LastOrDefault());
            }
            return null;
        }
    }
}
