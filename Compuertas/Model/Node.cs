using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCADAPI.Lab3.Model
{
    public class Node
    {
        public Object Data;
        public Node Parent;
        public List<Node> Children;
        public Boolean IsRoot { get { return this.Parent == null; } }
        public Boolean IsLeaf { get { return this.Children.Count == 0; } }

        public void Add(Object data)
        {
            this.Children.Add(new Node() { Parent = this, Children = new List<Node>() });
        }

        public static Node CrearArbolPrueba(Object entrada)
        {
            Node root = new Node() { Children = new List<Node>() };
            root.Add(new OR());
            //Primer hijo
            Node firstChild = root.Children.FirstOrDefault();
            //Al primer hijo de Root agregue dos compuertas
            firstChild.Add(new OR());
            firstChild.Add(new OR());
            //Agregamos pulsos a los nietos de root
            firstChild.Children[0].Add(new Pulso(new Autodesk.AutoCAD.Geometry.Point3d()));
            firstChild.Children[0].Add(new Pulso(new Autodesk.AutoCAD.Geometry.Point3d()));
            firstChild.Children[1].Add(new Pulso(new Autodesk.AutoCAD.Geometry.Point3d()));
            firstChild.Children[1].Add(new Pulso(new Autodesk.AutoCAD.Geometry.Point3d()));

            return root;
        }

        //Matriz de transición
        public int[][] matriz;
        


    }




}
