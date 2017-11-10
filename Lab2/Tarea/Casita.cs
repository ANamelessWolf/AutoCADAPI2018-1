using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab2.Tarea
{
    public class Casita
    {
        /// <summary> 
        /// Aquí se guardan las caras que forman la geometría 
        /// </summary> 
        public List<Face> Faces;
        /// <summary> 
        /// Aquí se guarda el punto de inserción  
        ///  </summary> 
        public Point3d InsPt;
        /// <summary>
        /// La geometría de la casita se define con 10 vertices
        /// </summary>
        public Point3d[] Geometry;
        /// <summary> 
        /// El tamaño de la casita 
        ///</summary> 
        public double Size;

        public Color Rojo = Color.FromRgb(180, 0, 0);
        public Color Cafe = Color.FromRgb(117, 75, 13);

        /// <summary> 
        /// El constructor para crear una casita, especificando un tamaño 
        /// y el punto de inserción 
        /// </summary> 
        /// <param name="size">El tamaño de la casita</param> 
        /// <param name="insPt">El punto de inserción de la casita</param>
        public Casita(double size, Point3d insPt)
        {
            this.Size = size;

            this.Geometry = new Point3d[]
            {
                //4 Puntos inferiores
                insPt,                                                                                              //0
                new Point3d(insPt.X + this.Size, insPt.Y, 0),                                                       //1
                new Point3d(insPt.X + this.Size, insPt.Y + this.Size, 0),                                           //2
                new Point3d(insPt.X, insPt.Y + this.Size, 0),                                                       //3
                //4 Puntos superiores
                new Point3d(insPt.X, insPt.Y, this.Size),                                                           //4
                new Point3d(insPt.X + this.Size, insPt.Y, this.Size),                                               //5
                new Point3d(insPt.X + this.Size, insPt.Y + this.Size, this.Size),                                   //6
                new Point3d(insPt.X, insPt.Y + this.Size, this.Size),                                               //7
                //2 Puntos del techo
                new Point3d(insPt.X + this.Size / 2, insPt.Y + this.Size / 4, this.Size + this.Size / 3),           //8
                new Point3d(insPt.X + this.Size / 2, insPt.Y + 3 * this.Size / 4, this.Size + this.Size / 3),       //9
            };
            this.Faces = new List<Face>();
            //Frontal
            QUAD(0, 1, 5, 4, Cafe);
            TRI(5, 8, 4, Rojo);
            //Trasera
            QUAD(3, 2, 6, 7, Cafe);
            TRI(6, 9, 7, Rojo);
            //Lateral Izq
            QUAD(0, 3, 7, 4, Cafe);
            QUAD(7, 4, 8, 9, Rojo);
            //Lateral Derecha
            QUAD(1, 5, 6, 2, Cafe);
            QUAD(6, 5, 8, 9, Rojo);
        }

        /// <summary>
        /// Agrega una nueva cara especificando los indices
        /// </summary>
        /// <param name="i">El primer indice</param>
        /// <param name="j">El segundo indice</param>
        /// <param name="k">El tercer indice</param>
        /// <param name="l">El cuarto indice</param>
        /// <param name="col">El color de la cara</param>
        public void QUAD(int i, int j, int k, int l, Color col)
        {
            Face f = new Face(this.Geometry[i], this.Geometry[j], this.Geometry[k], this.Geometry[l], true, true, true, true);
            f.Color = col;
            this.Faces.Add(f);
        }
        /// <summary>
        /// Agrega una nueva cara especificando los indices
        /// </summary>
        /// <param name="i">El primer indice</param>
        /// <param name="j">El segundo indice</param>
        /// <param name="k">El tercer indice</param>
        /// <param name="col">El color de la cara</param>
        public void TRI(int i, int j, int k, Color col)
        {
            Face f = new Face(this.Geometry[i], this.Geometry[j], this.Geometry[k], true, true, true, true);
            f.Color = col;
            this.Faces.Add(f);
        }
    }
}
