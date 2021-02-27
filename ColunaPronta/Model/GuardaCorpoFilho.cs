using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class GuardaCorpoFilho
    {
        public Retangulo retangulo { get; }
        public double Largura { get; }
        public double Comprimento { get; }
        public Retangulo PosteReforco { get; }
        public List<CantoneiraGuardaCorpo> Cantoneiras { get; set; }
        public GuardaCorpoFilho(double largura, double comprimento, Point2d pontoA, Posicao posicao)
        {
            var settings = new Settings();
            var posicaoRetangulo = Posicao.Vertical;
            this.Largura = largura;
            this.Comprimento = comprimento;
            double X = pontoA.X, Y = pontoA.Y;

            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoRetangulo = Posicao.Horizontal;
                    Y = Y - settings.DistanciaCantoneiraGC;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoRetangulo = Posicao.Vertical;
                    X = X + settings.DistanciaCantoneiraGC;
                    break;
                case Posicao.VoltadoCima:
                    posicaoRetangulo = Posicao.Horizontal;
                    Y = Y + settings.DistanciaCantoneiraGC;
                    break;
                default:
                    posicaoRetangulo = Posicao.Vertical;
                    X = X + settings.DistanciaCantoneiraGC;
                    break;
            }
           
            this.retangulo = new Retangulo(largura, comprimento, new Point2d(X, Y), posicaoRetangulo);
        }
    }
}
