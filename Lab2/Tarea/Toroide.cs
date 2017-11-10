using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab2.Tarea
{
    /// <summary>
    /// En geometría, un toro es una superficie de revolución generada por una circunferencia que 
    /// gira alrededor de una recta exterior.
    /// Ecuaciones parámetricas
    /// x(θ,φ) = (R + r cos θ) cos φ
    /// y(θ,φ) = (R + r cos θ) sin φ
    /// z(θ,φ) = r sin φ
    /// </summary>
    public class Toroide
    {
        /// <summary>
        // La distancia del centro del tubo al centro del toroide
        /// </summary>
        public double R;
        /// <summary>
        /// El radio del tubo
        /// </summary>
        public double r;
        /// <summary>
        /// El número con el que se divide θ
        /// </summary>
        public int U;
        /// <summary>
        /// El número con el que se divide φ
        /// </summary>
        public int V;
        /// <summary>
        /// El centro para el toroide
        /// </summary>
        public Point3d Center;
        /// <summary>
        /// La geometría del toroide
        /// </summary>
        public Point3dCollection Geometry;
        /// <summary>
        /// La lista de caras a dibujar
        /// </summary>
        public List<Face> Faces;
        /// <summary>
        /// Crea la geometría del toroide
        /// </summary>
        /// <param name="_R">La distancia del centro del tubo al centro del toroide</param>
        /// <param name="_r">El radio del tubo</param>
        /// <param name="_u">El número con el que se divide θ</param>
        /// <param name="_v">El número con el que se divide φ</param>
        /// <param name="center">El centro para el toroide</param>
        public Toroide(Double _R, Double _r, int _u, int _v, Point3d center)
        {
            this.R = _R;
            this.r = _r;
            this.U = _u;
            this.V = _v;
            this.Center = center;
            this.Geometry = new Point3dCollection();
            Double angDelta1 = (Math.PI * 2) / this.U,
                   angDelta2 = (Math.PI * 2) / this.V,
                   x, y, z;
            for (int i = 0; i < U; i++)
                for (int j = 0; j < V; j++)
                {
                    x = center.X + (this.R + this.r * Math.Cos(angDelta2 * j)) * Math.Cos(angDelta1 * i);
                    y = center.Y + (this.R + this.r * Math.Cos(angDelta2 * j)) * Math.Sin(angDelta1 * i);
                    z = center.Z + this.r * Math.Sin(angDelta2 * j);
                    this.Geometry.Add(new Point3d(x, y, z));
                }
        }
        /// <summary>
        /// Genera la geometría del toroide
        /// </summary>
        public void Draw()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            int sides = this.Geometry.Count / this.U, a, b, c, d;
            Point3d v1, v2, v3, v4;
            this.Faces = new List<Face>();
            for (int i = 0; i < this.Geometry.Count; i++)
            {
                if ((i + 1) % U == 0)
                {
                    a = (i + 1) - U;
                    b = i + 1;
                    c = i + U;
                    d = i;
                }
                else
                {
                    a = i;
                    b = i + U;
                    c = i + 1 + U;
                    d = i + 1;
                }
                if (a >= this.Geometry.Count)
                    a -= this.Geometry.Count;
                if (b >= this.Geometry.Count)
                    b -= this.Geometry.Count;
                if (c >= this.Geometry.Count)
                    c -= this.Geometry.Count;
                if (d >= this.Geometry.Count)
                    d -= this.Geometry.Count;
                v1 = this.Geometry[a];
                v2 = this.Geometry[b];
                v3 = this.Geometry[c];
                v4 = this.Geometry[d];
                this.Faces.Add(CreateFace(v1, v2, v3, v4));
            }
        }
        /// <summary>
        /// Dibuja una cara del toroide.
        /// </summary>
        /// <param name="v1">El primer vertice.</param>
        /// <param name="v2">El segundo vertice.</param>
        /// <param name="v3">El tercer vertice.</param>
        /// <param name="v4">El cuarto vertice.</param>
        /// <returns>La cara de la malla</returns>
        private Face CreateFace(Point3d v1, Point3d v2, Point3d v3, Point3d v4)
        {
            Face f = new Face(v1, v2, v3, v4, true, true, true, true);
            Random rand = new Random((int)DateTime.Now.Ticks);
            byte r = (byte)rand.Next(0, 255),
                 g = (byte)rand.Next(0, 255),
                 b = (byte)rand.Next(0, 255);
            f.Color = Autodesk.AutoCAD.Colors.Color.FromRgb(r, g, b);
            return f;
        }
    }
}