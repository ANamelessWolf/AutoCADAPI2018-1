using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace AutoCADAPI.Lab2
{
    public class TransactionWrapper
    {
        public Document Doc
        {
            get { return Application.DocumentManager.MdiActiveDocument; }
        }

        public Editor Ed
        {
            get { return Doc.Editor; }
        }
        //La definición de la función tipo Transaction Handler
        public delegate Object TransactionHandler(Document doc, Transaction tr, params Object[] input);
        //La variable tipo función y tipo Transaction Handler
        public TransactionHandler TransactionTask;

        public Object Run(TransactionHandler task, params Object[] input)
        {
            Database dwg = this.Doc.Database;
            Object result;
            using (Transaction tr = dwg.TransactionManager.StartTransaction())
            {
                try
                {
                    result = task(this.Doc, tr, input);
                    tr.Commit();
                }
                catch (System.Exception exc)
                {
                    this.Ed.WriteMessage(exc.Message);
                    tr.Abort();
                    throw exc;
                }
            }
            return result;
        }


    }
}







