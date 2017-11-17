using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using AutoCADAPI.Lab2;

namespace AutoCADAPI.Lab3.Model
{
    public class OR : Compuerta
    {
        public override String[] Inputs => new String[] { "INPUTA", "INPUTB" };
        
        public override KeyValuePair<string, Point3dCollection> Zones => throw new NotImplementedException();
    }
}
