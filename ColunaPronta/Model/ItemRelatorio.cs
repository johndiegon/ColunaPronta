using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class ItemRelatorio
    {
        public double Comprimento { get; set; }
        public double Largura { get; set; }
        public double Enrijecedor { get; set; }
        public double Altura { get; set; }
        public double AlturaViga { get; set; }
        public int iColuna { get; set; }
        public double QtdeParafuso { get; set; }
        public double QtdeColuna { get; set; }
        public double DiametroSapata { get; set; }
        public double DiametroParafuso { get; set; }
        public bool bPassante { get; set; }
        public string Descricao { get; set; }
        public ItemRelatorio (Polyline poly)
        {
            var points = new Point3dCollection();

            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                points.Add(poly.GetPoint3dAt(i));
            }


            List<double> ListaY = new List<double>();
            List<double> ListaX = new List<double>();
            double x;
            double y;

            foreach (Point3d point in points)
            {
                ListaX.Add(point.X);
                ListaY.Add(point.Y);
            }

            // Ponto A
            x = ListaX.Min();
            y = ListaY.Max();
            var PointA = new Point2d(x, y);

            // Ponto B
            x = ListaX.Max();
            y = ListaY.Max();
            var PointB = new Point2d(x, y);

            // Ponto C
            x = ListaX.Min();
            y = ListaY.Min();
            var PointC = new Point2d(x, y);

            // Ponto D
            x = ListaX.Max();
            y = ListaY.Min();
            var PointD = new Point2d(x, y);

            var lado1 = PointA.GetDistanceTo(PointB) * 1000;
            var lado2 = PointA.GetDistanceTo(PointC) * 1000;

            Comprimento = lado1 > lado2 ? lado1 : lado2;
            Largura = lado1 > lado2 ? lado2 : lado1;
        }

        public ItemRelatorio() { }

    }
}
