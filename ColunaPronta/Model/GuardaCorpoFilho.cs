using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoFilho
    {
        public Retangulo retangulo { get; }
        public double Largura { get; }
        public double Comprimento { get; }
        public Poste PosteReforco { get; }
        public List<CantoneiraGuardaCorpo> Cantoneiras { get; set; }
        public GuardaCorpoFilho(double largura, double comprimento, Point2d pontoA, Posicao posicao, double distanciaCantoneiraGC)
        {
            var posicaoRetangulo = Posicao.Vertical;
            this.Largura = largura;
            this.Comprimento = comprimento;
            double X = pontoA.X, Y = pontoA.Y;
       
            var settings = new Settings();

            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoRetangulo = Posicao.Horizontal;
                    Y = Y - distanciaCantoneiraGC;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoRetangulo = Posicao.Vertical;
                    X = X + distanciaCantoneiraGC;
                    break;
                case Posicao.VoltadoCima:
                    posicaoRetangulo = Posicao.Horizontal;
                    Y = Y - settings.CantoneiraComprimento + settings.Largura + distanciaCantoneiraGC;
                    break;
                default:
                    posicaoRetangulo = Posicao.Vertical;
                    X = X - distanciaCantoneiraGC - settings.Largura;
                    break;
            }

            if (comprimento >= settings.ComprimentoMinimoReforco)
            {
                double PosteX, PosteY;

                switch (posicao)
                {
                    case Posicao.VoltadoBaixo:
                        PosteX = X + ((comprimento / 2) - (settings.PosteReforcaoLargura / 2));
                        PosteY = Y + (settings.PosteReforcoDistancia);
                        break;
                    case Posicao.VoltadoDireita:
                        PosteX = X - settings.PosteReforcoDistancia;
                        PosteY = Y - ((comprimento / 2) - (settings.PosteReforcaoLargura / 2));
                        break;
                    case Posicao.VoltadoCima:
                        PosteX = X + ((comprimento / 2) - (settings.PosteReforcaoLargura / 2));
                        PosteY = Y - settings.CantoneiraComprimento + (settings.PosteReforcoComprimento - settings.PosteReforcoDistancia );
                        break;
                    default:
                        PosteX = X - (settings.PosteReforcoComprimento  -   settings.PosteReforcoDistancia - settings.Largura);
                        PosteY = Y - +((comprimento / 2) - (settings.PosteReforcaoLargura / 2));
                        break;
                }

                this.PosteReforco = new Poste(new Point2d(PosteX, PosteY), posicao, TipoPoste.Reforco);
            }

            this.retangulo = new Retangulo(largura, comprimento, new Point2d(X, Y), posicaoRetangulo);
        }
    }
}
