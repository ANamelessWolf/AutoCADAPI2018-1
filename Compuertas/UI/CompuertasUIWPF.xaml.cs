using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace AutoCADAPI.Lab3.UI
{
    /// <summary>
    /// Interaction logic for CompuertasUIWPF.xaml
    /// </summary>
    public partial class CompuertasUIWPF : UserControl
    {
        public String CompuertaName;

        public CompuertasUIWPF()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CompuertaName = (sender as Button).Name;
            var doc = AcadApp.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute("TestInsertCompuerta ", true, false, false);
        }
    }
}
