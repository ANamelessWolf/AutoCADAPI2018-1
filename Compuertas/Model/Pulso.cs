using AutoCADAPI.Lab2;
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
            this.Start = new Point3d();
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
                Point2d lastPt = this.Geometry.GetPoint2dAt(i);
                if (lasVal == this.Values[i])
                    this.Geometry.AddVertexAt(i + 1, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                else if (lasVal == false && this.Values[i] == true)
                {
                    lastPt = lastPt + new Vector2d(0, Size);
                    this.Geometry.AddVertexAt(i + 1, lastPt, 0, 0, 0);
                    this.Geometry.AddVertexAt(i + 1, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                }
                else
                {
                    lastPt = lastPt + new Vector2d(0, -Size);
                    this.Geometry.AddVertexAt(i + 1, lastPt, 0, 0, 0);
                    this.Geometry.AddVertexAt(i + 1, lastPt + new Vector2d(Size, 0), 0, 0, 0);
                }
            }
        }

        public ObjectId Draw(Drawer d)
        {
            d.Entity(this.Geometry);
            return d.Ids.OfType<ObjectId>().FirstOrDefault();
        }

    }
}
