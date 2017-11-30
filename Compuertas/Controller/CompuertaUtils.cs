using AutoCADAPI.Lab3.Model;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab3.Controller
{
    public static class CompuertaUtils
    {
        /// <summary>
        /// Checa si un punto esta dentro de un poligono
        /// </summary>
        /// <param name="test_point">El punto a probar.</param>
        /// <param name="pol">La geometría del poligono.</param>
        /// <returns>Verdadero si el punto esta dentro del poligono</returns>
        public static Boolean TestPoint(this Point3d test_point, Point3dCollection pol)
        {
            //Si el área es mayor al área del polygono el punto esta afuera.
            //Si el área es igual a 0 de algun triangulo el punto esta en una línea del poligono
            //Si la suma de las áreas es igual el punto esta dentro del poligono
            double pol_area = pol.Area(), sum_area = 0, t_area;
            //Repetimos el punto inicial del poligono para poder generar todos los triangulos
            Point3dCollection polygon = new Point3dCollection(pol.OfType<Point3d>().ToArray());
            polygon.Add(pol.OfType<Point3d>().FirstOrDefault());
            //Calculamos el área de los triangulos
            for (int i = 1; i < polygon.Count; i++)
            {
                t_area = new Point3dCollection(new Point3d[]
                {
                    test_point,
                    polygon[i-1],
                    polygon[i]
                }).Area();
                sum_area += t_area;
                if (t_area == 0)
                    return true;
                else if ((sum_area - pol_area) > 0.001)
                    return false;
            }
            return Math.Abs(sum_area - pol_area) < 0.001;
        }
        /// <summary>
        /// Calcula el área de un polygono regular.
        /// </summary>
        /// <param name="pts">La geometría del polygono</param>
        /// <returns>El área del poligono</returns>
        public static double Area(this Point3dCollection pts)
        {
            var pl = new Autodesk.AutoCAD.DatabaseServices.Polyline();
            for (int i = 0; i < pts.Count; i++)
                pl.AddVertexAt(i, new Point2d(pts[i].X, pts[i].Y), 0, 0, 0);
            return pl.Area;
        }
        /// <summary>
        /// Devuelve el punto medio entre dos puntos
        /// </summary>
        /// <returns>El punto medio</returns>
        public static Point3d MidPoint(this Point3d start, Point3d end)
        {
            return new Point3d((start.X + end.X) / 2, (start.Y + end.Y) / 2, (start.Z + end.Z) / 2);
        }
        /// <summary>
        /// Define los puntos de conexión de la compuerta
        /// </summary>
        /// <param name="com">Los puntos de conexión.</param>
        /// <returns>Los puntos de conexión</returns>
        public static Dictionary<String, Point3d> GetConnectionPointsTwoInputs(this Compuerta com)
        {
            Dictionary<String, Point3d> connPoints = new Dictionary<string, Point3d>();
            String[] names = new String[] { "INPUTA", "INPUTB", "OUTPUT" };
            Point3d cInput = com.Box[0].MidPoint(com.Box[3]);
            Point3d cOutput = com.Box[1].MidPoint(com.Box[2]);
            Point3d[] pts = new Point3d[]
            {
                cInput.MidPoint(com.Box[3]),
                com.Box[0].MidPoint(cInput),
                cOutput
            };
            for (int i = 0; i < names.Length; i++)
                connPoints.Add(names[i], pts[i]);
            return connPoints;
        }
        /// <summary>
        /// Define la caja de colisión para una compuerta de dos entradas
        /// </summary>
        /// <param name="com">La compuerta con dos entradas.</param>
        /// <returns>Las zonas para la compuerta de dos entradas</returns>
        public static Dictionary<String, Point3dCollection> GetZonesTwoInputs(this Compuerta com)
        {
            Dictionary<String, Point3dCollection> zones = new Dictionary<string, Point3dCollection>();
            String[] names = new String[] { "INPUTA", "INPUTB", "OUTPUT" };
            Point3dCollection[] z = new Point3dCollection[3];
            Point3d center = com.Box[0].MidPoint(com.Box[1]).MidPoint(com.Box[3].MidPoint(com.Box[2]));
            z[0] = new Point3dCollection(new Point3d[]
            {
                com.Box[0].MidPoint(com.Box[3]),
                center,
                com.Box[3].MidPoint(com.Box[2]),
                com.Box[3]
            });
            z[1] = new Point3dCollection(new Point3d[]
            {
                com.Box[0],
                com.Box[0].MidPoint(com.Box[1]),
                center,
                com.Box[0].MidPoint(com.Box[3]),
            });
            z[2] = new Point3dCollection(new Point3d[]
            {
                com.Box[0].MidPoint(com.Box[1]),
                com.Box[1],
                com.Box[2],
                com.Box[2].MidPoint(com.Box[3])
            });
            for (int i = 0; i < names.Length; i++)
                zones.Add(names[i], z[i]);
            return zones;
        }


        public static ObjectIdCollection Select(this Point3d basePt)
        {
            //Dibujar el poligono
            double angle = 2 * Math.PI;
            double x, y, z = 0;
            angle = angle / 10;
            double apo = 5;
            Point3dCollection pts = new Point3dCollection();
            for (int i = 0; i < 10; i++)
            {
                x = basePt.X + apo * Math.Cos(angle * i);
                y = basePt.Y + apo * Math.Sin(angle * i);
                pts.Add(new Point3d(x, y, z));
            }
            //Ejecutar el metodo de selección
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var res = ed.SelectCrossingPolygon(pts);
            //Devuelve los elementos seleccionados
            return res.Status == PromptStatus.OK ? new ObjectIdCollection(res.Value.GetObjectIds()) : new ObjectIdCollection();
        }

    }
}
