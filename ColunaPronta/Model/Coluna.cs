using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
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
            PassanteA =   false;
            PassanteB   = false;
            PassanteC   = false;
            PassanteD   = false;
            eleAmarelo  = false;
            eleVermelho = false;
            eleAzul     = false;
            eleCinza = false;
        }

        const double _escala = 1000;
        protected double comprimento = 0, largura =0 , altura=0;
        public int iColuna { get; set; }
        public Point2d PointA { get; set; }
        public Point2d PointB { get; set; }
        public Point2d PointC { get; set; }
        public Point2d PointD { get; set; }
        public double Comprimento 
        {
            get
            { 
                //if ( comprimento == 0)
                //{
                //    if (this.PointA != null && this.PointB != null && this.PointC != null)
                //    {
                //        var lado1 = this.PointA.GetDistanceTo(this.PointB) * _escala;
                //        var lado2 = this.PointA.GetDistanceTo(this.PointC) * _escala;

                //        return lado1 > lado2 ? lado1 : lado2;
                //    }
                //    else
                //    {
                //        return 0;
                //    }

                //}
                //else
                //{
                    return this.comprimento;
                //}
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
               return this.largura;
            }
            set
            {
                this.largura = value;
            }
        }
        public double Altura { get; set; }
        public Posicao Posicao { get; set; }
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

            var lado1 = this.PointA.GetDistanceTo(this.PointB) * _escala;
            var lado2 = this.PointA.GetDistanceTo(this.PointC) * _escala;

            this.comprimento = lado1 > lado2 ? lado1 : lado2;
            this.largura = lado1 > lado2 ? lado2 : lado1;
            this.Posicao = lado1 > lado2 ? Posicao.Horizontal : Posicao.Vertical;

        }
        public string NomeArquivo { get; set; }
        public bool LayoutInverto { get; set; }
        #region >> Parafuso
        public double DiametroParafuso { get; set; }
        public double QuantidadeParafuso { get; set; }
        public bool ParafusoA { get; set; }
        public bool ParafusoB { get; set; }
        public bool ParafusoC { get; set; }
        public bool ParafusoD { get; set; }
        public bool ParafusoE { get; set; }
        public bool ParafusoF { get; set; }
        public bool ParafusoG { get; set; }
        public bool ParafusoH { get; set; }
        #endregion

        #region >> Sapata
        public bool SapataA { get; set; }
        public bool SapataB { get; set; }
        public bool SapataC { get; set; }
        public bool SapataD { get; set; }
        public double DiametroSapata { get; set; }
        #endregion
        
        #region >> Passante
        public bool PassanteA { get; set; }
        public bool PassanteB { get; set; }
        public bool PassanteC { get; set; }
        public bool PassanteD { get; set; }
        public bool eleAmarelo { get; set; }
        public bool eleVermelho { get; set; }
        public bool eleAzul { get; set; }
        public bool eleCinza { get; set; }
        #endregion

        public void SetIdColuna()
        {
            var idColuna = IntegraLayout.GetIColuna(this);
            this.iColuna = idColuna;
        }

        public DateTime dInclusao { get; set; }
        public static Coluna FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Coluna coluna = new Coluna();
            coluna.iColuna            = Convert.ToInt32(values[0]);
            coluna.PointA             = new Point2d(Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
            coluna.Comprimento        = Convert.ToDouble(values[3]);    
            coluna.Largura            = Convert.ToDouble(values[4]);    
            coluna.Altura             = Convert.ToDouble(values[5]);    
            coluna.DiametroParafuso   = Convert.ToDouble(values[6]);    
            coluna.DiametroSapata     = Convert.ToDouble(values[7]);    
            coluna.QuantidadeParafuso = Convert.ToDouble(values[8]);    
            coluna.ParafusoA           = Convert.ToBoolean(values[9] =="0" ? false : true);    
            coluna.ParafusoB           = Convert.ToBoolean(values[10]=="0" ? false : true);    
            coluna.ParafusoC           = Convert.ToBoolean(values[11]=="0" ? false : true);    
            coluna.ParafusoD           = Convert.ToBoolean(values[12]=="0" ? false : true);    
            coluna.ParafusoE           = Convert.ToBoolean(values[13]=="0" ? false : true);    
            coluna.ParafusoF           = Convert.ToBoolean(values[14]=="0" ? false : true);    
            coluna.ParafusoG           = Convert.ToBoolean(values[15]=="0" ? false : true);    
            coluna.ParafusoH           = Convert.ToBoolean(values[16]=="0" ? false : true);    
            coluna.SapataA             = Convert.ToBoolean(values[17]=="0" ? false : true);    
            coluna.SapataB             = Convert.ToBoolean(values[18]=="0" ? false : true);    
            coluna.SapataC             = Convert.ToBoolean(values[19]=="0" ? false : true);    
            coluna.SapataD             = Convert.ToBoolean(values[20]=="0" ? false : true);    
            coluna.PassanteA           = Convert.ToBoolean(values[21]=="0" ? false : true);    
            coluna.PassanteB           = Convert.ToBoolean(values[22]=="0" ? false : true);    
            coluna.PassanteC           = Convert.ToBoolean(values[23]=="0" ? false : true);    
            coluna.PassanteD           = Convert.ToBoolean(values[24]=="0" ? false : true);    
            coluna.eleAmarelo          = Convert.ToBoolean(values[25]=="0" ? false : true);    
            coluna.eleVermelho         = Convert.ToBoolean(values[26]=="0" ? false : true);    
            coluna.eleAzul             = Convert.ToBoolean(values[27]=="0" ? false : true);    
            coluna.eleCinza            = Convert.ToBoolean(values[28]=="0" ? false : true);    
            coluna.dInclusao          = Convert.ToDateTime(values[29]);
            return coluna;
        }
    }
}
