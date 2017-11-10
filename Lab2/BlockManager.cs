using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;

namespace AutoCADAPI.Lab2
{

    public class BlockManager
    {
        public static Boolean LoadBlock(String path, String blockname, Document doc, Transaction tr)
        {
            if (File.Exists(path))
            {
                //Buscamos que no exista el registro
                BlockTable blkTab = (BlockTable)
                    doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                //Si no existe el registro debe cargarse del archivo
                if (!blkTab.Has(blockname))
                {
                    //1: Creación de espacio de bloque
                    blkTab.UpgradeOpen();//A modo escritura para escribir el bloque
                    BlockTableRecord newRecord = new BlockTableRecord();
                    newRecord.Name = blockname;
                    blkTab.Add(newRecord);
                    tr.AddNewlyCreatedDBObject(newRecord, true);
                    //2: Abrir base de datos externas
                    Database external = new Database();
                    external.ReadDwgFile(path, FileShare.Read, true, null);
                    ObjectIdCollection ids;
                    //3: Agregarles las entidades mediante un mapeo
                    using (Transaction extTr = external.TransactionManager.StartTransaction())
                    {
                        BlockTableRecord extModelSpace = (BlockTableRecord)
                            SymbolUtilityServices.GetBlockModelSpaceId(external).GetObject(OpenMode.ForRead);
                        ids = new ObjectIdCollection(extModelSpace.OfType<ObjectId>().ToArray());
                        using (IdMapping iMap = new IdMapping())
                        {
                            doc.Database.WblockCloneObjects(ids, newRecord.Id, iMap,
                                DuplicateRecordCloning.Replace, false);
                        }
                    }
                }
                return true;
            }
            else
            {
                BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
                //Si no existe el registro debe cargarse del archivo
                return blkTab.Has(blockname);
            }
        }

        public static BlockReference InsertBlock(String blockname, Point3d insPt, Document doc, Transaction tr, double scale = 1)
        {
            BlockTable blkTab =
                (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            if (blkTab.Has(blockname))
            {
                BlockReference blkRef = new BlockReference(insPt, blkTab[blockname]);
                blkRef.ScaleFactors = new Scale3d(scale);
                blkRef.Rotation = 0;
                return blkRef;
            }
            else
                throw new Exception("No existe el bloque " + blockname);
        }

        public static Boolean HasAttributes(BlockReference blkRef, String attName, out AttributeReference attRf,
                                            Document doc, Transaction tr)
        {
            IEnumerable<AttributeDefinition> attDefs; //Los atributos definidos en el registro de bloque
            IEnumerable<AttributeReference> attRefs;  //Los atributos referenciados en el bloque
            AttributeCollection attColl;
            //Si esta en modo lectura se cambia a modo escritura
            if (blkRef.IsReadEnabled)
                blkRef.UpgradeOpen();
            BlockTableRecord block = (BlockTableRecord)
                blkRef.BlockTableRecord.GetObject(OpenMode.ForRead);
            //Atributos existentes en el bloque
            attColl = blkRef.AttributeCollection;
            //Seleccionamos las definiciones de atributos del registro de bloque
            //LINQ para seleccionar los atributos
            attDefs = block.OfType<ObjectId>().Select(
                x =>
                {
                    DBObject obj = x.GetObject(OpenMode.ForRead);
                    if (obj is AttributeDefinition)
                        return (AttributeDefinition)obj;
                    else
                        return null;
                }).Where(y => y != null);
            if (attDefs.Count() > 0)
            {
                //Obtengo mis atributos referenciados
                attRefs = attColl == null ? new AttributeReference[0] :
                    attColl.OfType<ObjectId>().Select(x =>
                    {
                        DBObject obj = x.GetObject(OpenMode.ForRead);
                        if (obj is AttributeReference)
                            return (AttributeReference)obj;
                        else
                            return null;
                    }).Where(y => y != null);
                attRf = attRefs.FirstOrDefault(x => x.Tag == attName);
                if (attRf != null)
                    return true;
                else
                {
                    var attDef = attDefs.FirstOrDefault(x => x.Tag == attName);
                    if (attDef != null)
                    {
                        attRf = new AttributeReference();
                        attRf.SetAttributeFromBlock(attDef, blkRef.BlockTransform);
                        blkRef.AttributeCollection.AppendAttribute(attRf);
                        tr.AddNewlyCreatedDBObject(attRf, true);
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                attRf = null;
                return false;
            }
        }

        public static string GetAttribute(BlockReference blkRef, String attName,
            Document doc, Transaction tr)
        {
            AttributeReference attRef;
            if (HasAttributes(blkRef, attName, out attRef, doc, tr))
                return attRef.TextString;
            else
                return null;
        }

        public static void SetAttribute(BlockReference blkRef, String attName, String value,
     Document doc, Transaction tr)
        {
            AttributeReference attRef;
            if (HasAttributes(blkRef, attName, out attRef, doc, tr))
            {
                attRef.UpgradeOpen();
                attRef.TextString = value;
            }

        }
    }
}
