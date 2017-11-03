using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace AutoCADAPI.Lab2
{
    public class Selector
    {
        public static Editor Ed
        {
            get
            {
                return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            }
        }

        public static Boolean Point(String msg, out Point3d pt, Boolean allowNone = false)
        {
            //Opciones de Selección
            PromptPointOptions opt = new PromptPointOptions(msg);
            opt.AllowNone = allowNone;
            //Obtenemos el resultado del punto
            PromptPointResult res = Ed.GetPoint(opt);
            if(res.Status == PromptStatus.OK)
            {
                pt = res.Value;
                return true;
            }
            else
            {
                pt = default(Point3d);
                return false;
            }
        }
        public static Boolean Entity(String msg, out ObjectId id)
        {
            //Opciones de Selección
            PromptEntityOptions opt = new PromptEntityOptions(msg);
            //Obtenemos el resultado del punto
            PromptEntityResult res = Ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                id = res.ObjectId;
                return true;
            }
            else
            {
                id = default(ObjectId);
                return false;
            }
        }
    }
}
