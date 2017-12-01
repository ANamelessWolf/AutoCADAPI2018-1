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
using Autodesk.AutoCAD.Windows;
using AutoCADAPI.Lab3.UI;
using Autodesk.AutoCAD.EditorInput;
using AutoCADAPI.Lab3.Controller;

namespace AutoCADAPI.Lab3
{
    public class Commands
    {
        PaletteSet compuertasSet;
        public static CompuertasUI myControlCompuertas;
        public static CompuertasUIWPF myControlCompuertasWPF;

        public object Editor { get; private set; }

        [CommandMethod("TestInitUI")]
        public void TestUI()
        {
            //Inicializan la interfaz
            compuertasSet = new PaletteSet("Compuertas");
            myControlCompuertas = new CompuertasUI();
            myControlCompuertasWPF = new CompuertasUIWPF();
            //Windows Forms
            compuertasSet.Add("Galería", myControlCompuertas);
            //WPF
            compuertasSet.AddVisual("GaleríaWPF", myControlCompuertasWPF);
            compuertasSet.Dock = DockSides.Left;
            compuertasSet.Visible = true;
        }

        [CommandMethod("TestDictionary")]
        public void Dictionary()
        {
            ObjectId obj;
            if (Selector.Entity("Selecciona una entidad", out obj))
            {
                TransactionWrapper tr = new TransactionWrapper();
                tr.Run(DictionaryTask, obj);
            }
        }

        private object DictionaryTask(Document doc, Transaction tr, object[] input)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Entity ent = ((ObjectId)input[0]).GetObject(OpenMode.ForRead) as Entity;
            DictionaryManager man = new DictionaryManager();
            //Abrimos el diccionario de extensión de la entidad
            var extD = man.GetExtensionD(tr, doc, ent);
            man.SetData(extD, tr, "Prueba", "Miguel", DateTime.Now.ToShortDateString());
            var data = man.GetData(extD, tr, "Prueba");
            ed.WriteMessage("Nombre: {0}, Fecha de Registro: {1}", data[0], data[1]);
            return null;
        }

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
