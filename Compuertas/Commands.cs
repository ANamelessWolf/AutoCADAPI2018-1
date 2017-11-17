using AutoCADAPI.Lab2;
using AutoCADAPI.Lab3.Model;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AutoCADAPI.Lab3
{
    public class Commands
    {
        [CommandMethod("DibujarPulso")]
        public void DPulso()
        {
            Point3d insPt;
            Random r = new Random((int)DateTime.Now.Ticks);
            Boolean[] data = new Boolean[]
            {
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true,
                false, true, false, true, false, true, false, true
            };
            int pulsoSize;
            if (Selector.Point("Selecciona el punto de inserción del pulso", out insPt) &&
                Selector.Integer("El tamaño del pulso", out pulsoSize, 4))
            {
                Boolean[] input = new Boolean[pulsoSize];
                for (int i = 0; i < input.Length; i++)
                    input[i] = data[r.Next(data.Length - 1)];
                Pulso p = new Pulso(insPt, input);
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DPulsoTask, new Object[] { p });
            }

        }

        private object DPulsoTask(Document doc, Transaction tr, object[] input)
        {
            Drawer d = new Drawer(tr);
            return (input[0] as Pulso).Draw(d);
        }
    }
}
