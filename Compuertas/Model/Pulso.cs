using AutoCADAPI.Lab2;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab3.Model
{
    public class Pulso
    {
        const Double Size = 20;
        public Boolean[] Values;
        public Polyline Geometry;
        public Point3d Start;
        /// <summary>
        /// Initializes a new instance of the <see cref="Pulso"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public Pulso(Point3d insPt, params Boolean[] values)
        {
            this.Values = values;
            this.Geometry = new Polyline();
            this.Start = insPt;
            this.Init();
        }
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            //Borramos la polilínea
            while (this.Geometry.NumberOfVertices > 1)
                this.Geometry.RemoveVertexAt(1);
            if (this.Geometry.NumberOfVertices == 0)
                this.Geometry.AddVertexAt(0, new Point2d(this.Start.X, this.Start.Y), 0, 0, 0);
            Boolean lasVal = false;
            for (int i = 0; i < this.Values.Length; i++)
            {
                Point2d lastPt = this.Geometry.GetPoint2dAt(this.Geometry.NumberOfVertices - 1);
                if (lasVal == this.Values[i])
                    this.Geometry.AddVertexAt(this.Geometry.NumberOfVertices, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                else if (lasVal == false && this.Values[i] == true)
                {
                    lastPt = lastPt + new Vector2d(0, Size);
                    this.Geometry.AddVertexAt(this.Geometry.NumberOfVertices, lastPt, 0, 0, 0);
                    this.Geometry.AddVertexAt(this.Geometry.NumberOfVertices, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                }
                else
                {
                    lastPt = lastPt + new Vector2d(0, -Size);
                    this.Geometry.AddVertexAt(this.Geometry.NumberOfVertices, lastPt, 0, 0, 0);
                    this.Geometry.AddVertexAt(this.Geometry.NumberOfVertices, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                }
                lasVal = this.Values[i];
            }
        }

        public static Boolean[] GetValues(Polyline pl)
        {
            int size = (int)(pl.GetPoint2dAt(0).GetDistanceTo(pl.GetPoint2dAt(pl.NumberOfVertices - 1)) / Size);
            Boolean[] values = new Boolean[size];
            Boolean val = false;
            Point2d pV,//Vertice anterior
                cV;//Vertice actual;
            int index = 0;
            for (int i = 1; i < pl.NumberOfVertices; i++)
            {
                pV = pl.GetPoint2dAt(i - 1);
                cV = pl.GetPoint2dAt(i);
                if (pV.X == cV.X)//Misma X cambia de valor
                    val = !val;
                else
                {
                    values[index] = val;
                    index++;
                }
            }
            return values;
        }


        public ObjectId Draw(Drawer d)
        {
            d.Entity(this.Geometry);
            return d.Ids.OfType<ObjectId>().FirstOrDefault();
        }

        public static void Connect(Transaction tr, Document doc, Dictionary<Handle, Compuerta> compuertas)
        {
            ObjectId pulsoId, cmpId;
            Point3d connPoint;
            if (Selector.Entity("Selecciona un pulso", typeof(Polyline), out pulsoId) &&
                Selector.Entity("Selecciona la compuerta a conectar el pulso", out cmpId, out connPoint))
            {
                Cable c;
                //Extraer información del pulso
                Polyline pl = pulsoId.GetObject(OpenMode.ForRead) as Polyline;
                Boolean[] data = Pulso.GetValues(pl);
                Point3d start = pl.EndPoint;
                //Extraer información de la compuerta
                Compuerta cmp = compuertas.Values.FirstOrDefault(x => x.Block.Id == cmpId);
                if (cmp != null)
                {
                    String name;
                    Point3dCollection zone;
                    cmp.GetZone(connPoint, out name, out zone);
                    //Obtención del punto de conexión
                    if (cmp.ConnectionPoints.ContainsKey(name))
                    {
                        Point3d end = cmp.ConnectionPoints[name];
                        Drawer d = new Drawer(tr);
                        c = Cable.InsertCable(start, end, d);
                        c.SetData(tr, doc, data);
                    }
                }
            }
        }


    }
}
