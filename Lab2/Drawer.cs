using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Espacios de nombre de AutoCAD
using Autodesk.AutoCAD.ApplicationServices;             //La administración de la aplicación.
using Autodesk.AutoCAD.EditorInput;                     //La interacción del usuario con AutoCAD
using Autodesk.AutoCAD.Geometry;                        //Las clases auxiliares para manejar geometría.
using Autodesk.AutoCAD.Runtime;                         //Cachar excepciones de AutoCAD y definir comandos.
using Autodesk.AutoCAD.Colors;

namespace AutoCADAPI.Lab2
{
    public class Drawer
    {
        public BlockTableRecord Block;

        public Transaction ActiveTransaction;

        public ObjectIdCollection Ids;

        public Drawer(Transaction tr, String blockTableRecord = null)
        {
            this.Ids = new ObjectIdCollection();
            if (blockTableRecord == null)
            {
                //CurrentSpaceId Devuelve el espacio de modelo activo, ya
                //sea el modelspace o el paperspace
                this.Block = (BlockTableRecord)
                    Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId.GetObject(OpenMode.ForWrite);
            }
            else
            {
                Database dwg = Application.DocumentManager.MdiActiveDocument.Database;
                BlockTable blkTab = (BlockTable)dwg.BlockTableId.GetObject(OpenMode.ForRead);
                if (blkTab.Has(blockTableRecord))
                    this.Block = (BlockTableRecord)blkTab[blockTableRecord].GetObject(OpenMode.ForWrite);
                else
                {
                    //Se crea un nuevo registro si no existe
                    BlockTableRecord newRecord = new BlockTableRecord();
                    newRecord.Name = blockTableRecord;
                    //Cambia un objeto abierto de modo lectura a modo escritura
                    blkTab.UpgradeOpen();
                    blkTab.Add(newRecord);
                    tr.AddNewlyCreatedDBObject(newRecord, true);
                    this.Block = newRecord;
                    this.Block.UpgradeOpen();
                }
            }
            this.ActiveTransaction = tr;
        }

        public void Entities(params Entity[] ents)
        {
            foreach (Entity ent in ents)
            {
                this.Block.AppendEntity(ent);
                this.ActiveTransaction.AddNewlyCreatedDBObject(ent, true);
                this.Ids.Add(ent.Id);
            }
        }
        public void Entity(Entity ent)
        {
            this.Block.AppendEntity(ent);
            this.ActiveTransaction.AddNewlyCreatedDBObject(ent, true);
            this.Ids.Add(ent.Id);
        }

        public void Geometry(Point3dCollection pts, Boolean closed = true)
        {
            Polyline pl = new Polyline();
            for (int i = 0; i < pts.Count; i++)
                pl.AddVertexAt(i, new Point2d(pts[i].X, pts[i].Y), 0, 0, 0);
            pl.Closed = closed;
            this.Entity(pl);
        }

        public void Triangle(Point3d pt0, Point3d pt1, Point3d pt2, Color color)
        {
            Face f = new Face(pt0, pt1, pt2, true, true, true, true);
            f.Color = color;
            this.Entity(f);
        }

        public void Quad(Point3d pt0, Point3d pt1, Point3d pt2, Point3d pt3, Color color)
        {
            Face f = new Face(pt0, pt1, pt2, pt3, true, true, true, true);
            f.Color = color;
            this.Entity(f);
        }

        public void Erase(ObjectIdCollection ids)
        {
            DBObject obj;
           foreach(ObjectId id in ids)
            {
                obj = id.GetObject(OpenMode.ForWrite);
                if (!obj.IsErased)
                    obj.Erase();
            }
        }
    }
}
