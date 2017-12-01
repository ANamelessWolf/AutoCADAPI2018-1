using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace AutoCADAPI.Lab3.UI
{
    public partial class CompuertasUI : UserControl
    {
        public String CompuertaName;

        public CompuertasUI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompuertaName = (sender as Button).Name;
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute("TestInsertCompuerta ", true, false, false);
        }
    }
}
