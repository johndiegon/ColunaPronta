using Autodesk.AutoCAD.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class Coluna
    {
        const double _escala = 1000;
        public Point2d PointA { get; set; }
        public Point2d PointB { get; set; }
        public Point2d PointC { get; set; }
        public Point2d PointD { get; set; }
        public double Comprimento 
        {
            get
            {
                
                if ( this.PointA != null && this.PointB != null && this.PointC != null)
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
        }
        public double Largura 
        {
            get
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

        private TipoColuna tipoColuna;

        public TipoColuna GetTipoColuna()
        {
            return tipoColuna;
        }

    }
}
