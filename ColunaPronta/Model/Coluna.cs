using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ColunaPronta.Model
{
    public class Coluna
    {
        public Coluna()
        {
            ParafusoA = false;
            ParafusoB = false;
            ParafusoC = false;
            ParafusoD = false;
            ParafusoE = false;
            ParafusoF = false;
            ParafusoG = false;
            ParafusoH = false;
            SapataA   = false;
            SapataB   = false;
            SapataC   = false;
            SapataD   = false;
        }

        const double _escala = 1000;
        protected double comprimento = 0, largura =0 , altura=0;
        public Point2d PointA { get; set; }
        public Point2d PointB { get; set; }
        public Point2d PointC { get; set; }
        public Point2d PointD { get; set; }
        public double Comprimento 
        {
            get
            { 
                if ( comprimento == 0)
                {
                    if (this.PointA != null && this.PointB != null && this.PointC != null)
                    {
                        var lado1 = this.PointA.GetDistanceTo(this.PointB) * _escala;
                        var lado2 = this.PointA.GetDistanceTo(this.PointC) * _escala;

                        return lado1 > lado2 ? lado1 : lado2;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return this.comprimento;
                }
            }
            set
            {
                this.comprimento = value;
            }
        }
        public double Largura 
        {
            get
            {
                if( largura == 0)
                {
                    if (this.PointA != null && this.PointB != null && this.PointC != null)
                    {
                        var lado1 = this.PointA.GetDistanceTo(this.PointB) * _escala;
                        var lado2 = this.PointA.GetDistanceTo(this.PointC) * _escala;

                        return lado1 > lado2 ? lado2 : lado1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return this.largura;
                }
            }
            set
            {
                this.largura = value;
            }
        }
        public bool ParafusoA { get; set; }
        public bool ParafusoB { get; set; }
        public bool ParafusoC { get; set; }
        public bool ParafusoD { get; set; }
        public bool ParafusoE { get; set; }
        public bool ParafusoF { get; set; }
        public bool ParafusoG { get; set; }
        public bool ParafusoH { get; set; }
        public bool SapataA { get; set; }
        public bool SapataB { get; set; }
        public bool SapataC { get; set; }
        public bool SapataD { get; set; }
        public double DiametroParafuso { get; set; }
        public double DiametroSapata { get; set; }
        public double Altura { get; set; }
        public double QuantidadeParafuso { get; set; }
        public string NomeArquivo { get; set; }
        public List<long> ObjectIds { get; set; }
        public TipoColuna tipoColuna { get; set; }
        public TipoColuna GetTipoColuna()
        {
            return tipoColuna;
        }
        public void SetTipoColuna(TipoColuna tipoColuna)
        {
            this.tipoColuna = tipoColuna;
        }
        public DateTime dInclusao { get; set; }
        public static Coluna FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Coluna coluna = new Coluna();
            coluna.tipoColuna = (TipoColuna)Convert.ToInt32((values[0]));
            coluna.PointA= new Point2d(Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
            coluna.DiametroSapata = Convert.ToDouble(values[3]);
            coluna.DiametroParafuso = Convert.ToDouble(values[4]);
            coluna.QuantidadeParafuso = Convert.ToDouble(values[5]);
            coluna.Comprimento = Convert.ToDouble(values[6]);
            coluna.Largura = Convert.ToDouble(values[7]);
            coluna.Altura = Convert.ToDouble(values[8]);
            coluna.dInclusao = Convert.ToDateTime(values[9]);
            return coluna;
        }
        public void SetPontos(Point3dCollection points)
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

            if (ListaX.Count > 0 && ListaY.Count > 0)
            {
                // Ponto A
                x = ListaX.Min();
                y = ListaY.Max();
                this.PointA = new Point2d(x, y);

                // Ponto B
                x = ListaX.Max();
                y = ListaY.Max();
                this.PointB = new Point2d(x, y);

                // Ponto C
                x = ListaX.Min();
                y = ListaY.Min();
                this.PointC = new Point2d(x, y);

                // Ponto D
                x = ListaX.Max();
                y = ListaY.Min();
                this.PointD = new Point2d(x, y);
            }
        }
    }
}
