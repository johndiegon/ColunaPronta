using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace ColunaPronta.Model
{
    public class ItemRelatorio
    {
        public double Comprimento { get; set; }
        public double ComprimentoTuboInterno { get; set; }
        public double ComprimentoTuboExterno { get; set; }
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
        public Point2d PontoA { get; set; }
        public Point2d PontoB { get; set; }
        public Point2d PontoC { get; set; }
        public Point2d PontoD { get; set; }
        public Abertura Abertura { get; set; }
        public ItemRelatorio (Polyline poly)
        {
            var points = new Point3dCollection();

            for (int i = 0; i < poly.NumberOfVertices; i++)
            {
                points.Add(poly.GetPoint3dAt(i));
            }

            SetItem(points);
        }
        public ItemRelatorio(Point3dCollection points)
        {
            SetItem(points);
        }
        public ItemRelatorio() { }
        private void SetItem(Point3dCollection points)
        {
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
            this.PontoA = new Point2d(x, y);

            // Ponto B
            x = ListaX.Max();
            y = ListaY.Max();
            this.PontoB = new Point2d(x, y);

            // Ponto C
            x = ListaX.Min();
            y = ListaY.Min();
            this.PontoC = new Point2d(x, y);

            // Ponto D
            x = ListaX.Max();
            y = ListaY.Min();
            this.PontoD = new Point2d(x, y);

            var lado1 = this.PontoA.GetDistanceTo(this.PontoB) * 1000;
            var lado2 = this.PontoA.GetDistanceTo(this.PontoC) * 1000;

            Comprimento = lado1 > lado2 ? lado1 : lado2;
            Largura = lado1 > lado2 ? lado2 : lado1;
        }

    }
}
