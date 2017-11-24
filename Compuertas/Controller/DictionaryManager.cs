using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab3.Controller
{
   public class DictionaryManager
    {
        /// <summary>
        /// Accede al diccionario de la aplicación
        /// </summary>
        /// <value>
        /// El diccionario de la aplicación
        /// </value>
        public ObjectId NOD
        {
            get
            {
                return
                    Application.DocumentManager.MdiActiveDocument.Database.NamedObjectsDictionaryId;
            }
        }
        public ObjectId GetExtensionD(Transaction tr, Document doc, Entity ent)
        {
            if (ent.ExtensionDictionary.IsValid)
            {
                return ent.ExtensionDictionary;
            }
            else
            {
                Entity e = ent.Id.GetObject(OpenMode.ForWrite) as Entity;
                e.CreateExtensionDictionary();
                return ent.ExtensionDictionary;
            }
        }


        public void SetData(ObjectId idD, Transaction tr, String xRecordName, params String[] data)
        {
            DBDictionary dict = idD.GetObject(OpenMode.ForWrite) as DBDictionary;
            //Se crea el registro para guardar la información
            Xrecord xRec = new Xrecord();
            //Se crea la información para guardar en el registro
            xRec.Data = new ResultBuffer(
                data.Select(x => new TypedValue((int)DxfCode.Text, x)).ToArray());
            //Se agregá el Xrecord al diccionario
            dict.SetAt(xRecordName, xRec);
            //Se notifica a la transacción
            tr.AddNewlyCreatedDBObject(xRec, true);
        }

        public String[] GetData(ObjectId idD, Transaction tr, String XRecordName)
        {
            DBDictionary dict = idD.GetObject(OpenMode.ForWrite) as DBDictionary;
            ObjectId xRecordId = dict.GetAt(XRecordName);
            if (xRecordId.IsValid)
            {
                Xrecord xRec = xRecordId.GetObject(OpenMode.ForRead) as Xrecord;
                if (xRec.Data != null)
                {
                    TypedValue[] data = xRec.Data.AsArray();
                    return data.Select(x => x.Value.ToString()).ToArray();
                }
                else
                    return new String[0];
            }
            else
                return new String[0];
        }


    }
}