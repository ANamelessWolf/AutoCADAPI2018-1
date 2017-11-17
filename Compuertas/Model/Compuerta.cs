﻿using AutoCADAPI.Lab2;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab3.Model
{
    /// <summary>
    /// Esta clase define la estructura básica de una compuerta
    /// </summary>
    public abstract class Compuerta
    {
        /// <summary>
        /// Define la entrada de la compuerta
        /// </summary>
        public abstract Boolean[] Input { get; set; }
        /// <summary>
        /// The output
        /// </summary>
        public Boolean Output;
        /// <summary>
        /// Define las zonas de contacto
        /// </summary>
        public abstract KeyValuePair<String, Point3dCollection> Zones { get; }
        /// <summary>
        /// El bloque que hace referencia a la compuerta
        /// </summary>
        public BlockReference Block;
        /// <summary>
        /// Devolver la caja de colisión del bloque, en XY
        /// </summary>
        /// <returns>La caja de colisión</returns>
        public Point3dCollection GetBoundingBox()
        {
            Point3d min = this.Block.GeometricExtents.MinPoint,
                    max = this.Block.GeometricExtents.MaxPoint;
            return new Point3dCollection(new Point3d[]
            {
                min,
                new Point3d(max.X, min.Y, 0),
                max,
                new Point3d(min.X, max.Y, 0)
            });
        }
        /// <summary>
        /// Dibuja una caja de colisión
        /// </summary>
        public ObjectId DrawBox(Point3dCollection pts, Drawer drawer)
        {
            drawer.Geometry(pts, true);
            return drawer.Ids.OfType<ObjectId>().FirstOrDefault();
        }



    }
}