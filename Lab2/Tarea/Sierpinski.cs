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
    /// El triángulo de Sierpiński es un fractal que se puede construir a partir de cualquier triángulo.
    /// Más información del triángulo en 
    /// https://es.wikipedia.org/wiki/Tri%C3%A1ngulo_de_Sierpinski
    /// http://mathworld.wolfram.com/SierpinskiSieve.html
    /// </summary>
    public class Sierpinski
    {
        /// <summary>
        /// Define el grado de profundidad del triángulo
        /// </summary>
        public int depth;
        /// <summary>
        /// El triángulo se dibujara con una polilínea con tres puntos
        /// </summary>
        public Polyline Triangle;
        /// <summary>
        /// El centro del triángulo es el punto de inserción
        /// </summary>
        public Point3d Center;
        /// <summary>
        /// Los vertices del triángulo
        /// </summary>
        public Point2d A, B, C;
        /// <summary>
        /// El tamaño de un lado del triángulo
        /// </summary>
        public Double Size;
        /// <summary>
        /// Inicializa la instancia de la clase <see cref="Sierpinski"/>.
        /// </summary>
        /// <param name="center">El centro del triángulo.</param>
        /// <param name="size">El tamaño de un lado del triángulo.</param>
        public Sierpinski(Point3d center, Double size = 100)
        {
            Triangle = new Polyline();
            this.Size = size;
            A = new Point2d(center.X - size / 2, center.Y);
            B = new Point2d(center.X, center.Y + size);
            C = new Point2d(center.X + size / 2, center.Y);
            Triangle.AddVertexAt(0, A, 0, 0, 0);
            Triangle.AddVertexAt(1, B, 0, 0, 0);
            Triangle.AddVertexAt(2, C, 0, 0, 0);
            Triangle.Closed = true;
            depth = 0;
        }
        /// <summary>
        /// Inicializa la instancia de la clase <see cref="Sierpinski"/>.
        /// </summary>
        /// <param name="a">El primer punto del triángulo.</param>
        /// <param name="b">El segundo punto del triángulo.</param>
        /// <param name="c">El tercer punto del triángulo.</param>
        public Sierpinski(Point2d a, Point2d b, Point2d c)
        {
            Triangle = new Polyline();
            A = a;
            B = b;
            C = c;
            this.Size = B.GetDistanceTo(A);
            Triangle.AddVertexAt(0, A, 0, 0, 0);
            Triangle.AddVertexAt(1, B, 0, 0, 0);
            Triangle.AddVertexAt(2, C, 0, 0, 0);
            Triangle.Closed = true;
            depth = 0;
        }
        /// <summary>
        /// Dibuja los triángulos
        /// </summary>
        /// <returns>La colección de triángulos</returns>
        public Sierpinski[] GetTriangles()
        {
            Point2d MidAB = MidPoint(A, B),
                    MidBC = MidPoint(B, C),
                    MidAC = MidPoint(A, C);
            Sierpinski t1 = new Sierpinski(A, MidAB, MidAC),
                       t2 = new Sierpinski(B, MidAB, MidBC),
                       t3 = new Sierpinski(C, MidAC, MidBC);
            t1.depth = depth + 1;
            t2.depth = depth + 1;
            t3.depth = depth + 1;
            return new Sierpinski[] { t1, t2, t3 };
        }
        /// <summary>
        /// Genera los triangulos hasta llegar al nivel de profundidad deseado.
        /// </summary>
        /// <param name="t">El triángulo.</param>
        /// <param name="ents">La colección de entidades dibujadas.</param>
        /// <param name="maxDepth">La profundidad máxima.</param>
        public void Draw(Sierpinski t, ref List<Entity> ents, int maxDepth = 5)
        {
            if (t.depth > maxDepth)
                return;
            else
            {
                ents.Add(t.Triangle);
                foreach (Sierpinski tri in t.GetTriangles())
                    Draw(tri, ref ents, maxDepth);
            }
        }
        /// <summary>
        /// Obtiene el punto medio entre dos puntos
        /// </summary>
        /// <param name="p0">El punto inicial.</param>
        /// <param name="p1">El punto final.</param>
        /// <returns></returns>
        private Point2d MidPoint(Point2d p0, Point2d p1)
        {
            return new Point2d((p0.X + p1.X) / 2, (p0.Y + p1.Y) / 2);
        }
    }
}