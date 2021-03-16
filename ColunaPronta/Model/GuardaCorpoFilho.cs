using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoFilho
    {
        public Retangulo retangulo { get; }
        public double Largura { get; }
        public double Comprimento { get; }
        public PosteReforco PosteReforco { get; }
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
                        PosteX = X + ((comprimento / 2) - (settings.PosteReforcoLargura / 2));
                        PosteY = Y + (settings.PosteReforcoDistancia);
                        break;
                    case Posicao.VoltadoDireita:
                        PosteX = X - settings.PosteReforcoDistancia;
                        PosteY = Y - ((comprimento / 2) - (settings.PosteReforcoLargura / 2));
                        break;
                    case Posicao.VoltadoCima:
                        PosteX = X + ((comprimento / 2) - (settings.PosteReforcoLargura / 2));
                        PosteY = Y + ( settings.PosteComprimento - settings.PosteReforcoDistancia);
                        break;
                    default:
                        PosteX = X - (settings.PosteReforcoComprimento  -   settings.PosteReforcoDistancia - settings.Largura);
                        PosteY = Y - +((comprimento / 2) - (settings.PosteReforcoLargura / 2));
                        break;
                }

                this.PosteReforco = new PosteReforco(new Point2d(PosteX, PosteY), posicao);
            }

            this.retangulo = new Retangulo(largura, comprimento, new Point2d(X, Y), posicaoRetangulo);
        }
      
    }
    public class PosteReforco
    {
        public List<CantoneiraGuardaCorpo> Cantoneiras { get; set; }
        public List<Retangulo> Poste { get; set; }

        public PosteReforco(Point2d PontoInicial, Posicao posicao )
        {
            double X = PontoInicial.X;
            double Y = PontoInicial.Y;

            var settings = new Settings();
            var cantoneiras = new List<CantoneiraGuardaCorpo>();
            var poste = new List<Retangulo>();
            var posicaoRetangulo = Posicao.Vertical;
            var posicaoInversa = posicao;
          
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoInversa = Posicao.VoltadoCima;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoInversa = Posicao.VoltadoEsqueda;
                    break;
                case Posicao.VoltadoCima:
                    posicaoInversa = Posicao.VoltadoBaixo;
                    break;
                default:
                    X = X - (0.008);
                    Y = Y;
                    posicaoInversa = Posicao.VoltadoDireita;
                    break;
            }

           
            var cantoneira1 = new CantoneiraGuardaCorpo(new Point2d(X, Y), posicaoInversa, TipoCantoneira.Cantoneira38MM);
            cantoneiras.Add(cantoneira1);
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    X = PontoInicial.X;
                    Y = PontoInicial.Y - settings.CantoneiraPosteComprimento ;
                    posicaoRetangulo = Posicao.Vertical;
                    break;
                case Posicao.VoltadoDireita:
                    X = PontoInicial.X + settings.CantoneiraPosteComprimento;
                    Y = PontoInicial.Y;
                    posicaoRetangulo = Posicao.Horizontal;
                    break;
                case Posicao.VoltadoCima:
                    X = PontoInicial.X;
                    Y = PontoInicial.Y - settings.CantoneiraPosteComprimento;
                    posicaoRetangulo = Posicao.Vertical;
                    break;
                default:
                    X = PontoInicial.X - settings.CantoneiraPosteComprimento;
                    Y = PontoInicial.Y;
                    posicaoRetangulo = Posicao.Horizontal;
                    break;
            }

            var posteExterno = new Retangulo(settings.PosteReforcoLargura, settings.PosteReforcoComprimento, new Point2d(X, Y), posicaoRetangulo);
            var posteInterno = new Retangulo(settings.PosteReforcoLargura - ( settings.PosteEspessura * 2), settings.PosteReforcoComprimento - (settings.PosteEspessura * 2), new Point2d(X + settings.PosteEspessura, Y -settings.PosteEspessura), posicaoRetangulo);

            poste.Add(posteExterno);
            poste.Add(posteInterno);

            Poste = poste;

            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    X = PontoInicial.X;
                    Y = PontoInicial.Y - (settings.PosteReforcoComprimento + settings.CantoneiraPosteComprimento);
                    break;
                case Posicao.VoltadoDireita:
                    X = PontoInicial.X + (settings.PosteReforcoComprimento + settings.CantoneiraPosteComprimento);
                    Y = PontoInicial.Y;
                    break;
                case Posicao.VoltadoCima:
                    posicao = Posicao.VoltadoBaixo;
                    X = PontoInicial.X;
                    Y = PontoInicial.Y - (settings.PosteReforcoComprimento + settings.CantoneiraPosteComprimento);
                    break;
                default:
                    X = PontoInicial.X - (settings.PosteReforcoComprimento + settings.CantoneiraPosteComprimento) - (0.008); 
                    Y = PontoInicial.Y;
                    break;
            }

            var cantoneira2 = new CantoneiraGuardaCorpo(new Point2d(X, Y), posicao, TipoCantoneira.Cantoneira38MM);
            cantoneiras.Add(cantoneira2);

            Cantoneiras = cantoneiras;
        }
    }
}
