using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoFilho
    {
        public List<Retangulo> Tubos { get; }
        public double Largura { get; }
        public double Comprimento { get; }
        public PosteReforco PosteReforco { get; }
        public List<CantoneiraGuardaCorpo> Cantoneiras { get; set; }
        public GuardaCorpoFilho() {}
        public GuardaCorpoFilho(double largura, double comprimento, Point2d pontoA, Posicao posicao, double distanciaCantoneiraGC)
        {
            var posicaoRetangulo = Posicao.Vertical;
            this.Largura = largura;
            this.Comprimento = comprimento;
            double X = pontoA.X, Y = pontoA.Y;
            this.Tubos = new List<Retangulo>();
            var settings = new Settings(true);

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

            var retangulo = new Retangulo(largura, comprimento, new Point2d(X, Y), posicaoRetangulo, Model.Layer.TuboExterno);
            this.Tubos.Add(retangulo);
        }
        public GuardaCorpoFilho(double largura, double comprimento, Point2d pontoA, Posicao posicao, double distanciaCantoneiraGC, Abertura abertura)
        {
            var settings = new Settings(true);
            var posicaoRetangulo = Posicao.Vertical;
            this.Tubos = new List<Retangulo>();
            this.Largura = largura;
            this.Comprimento = comprimento ;
            var comprimentoTuboExterno = comprimento - settings.TuboExternoDistanciaInicial;
            
            double X = pontoA.X, Y = pontoA.Y;
            double tuboInternoX = pontoA.X, tuboInternoY = pontoA.Y;
            double tuboExternoX = pontoA.X, tuboExternoY = pontoA.Y;
            var distanciaTuboInterno = (largura - settings.TuboInternoLargura) / 2f;

            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoRetangulo = Posicao.Horizontal;
                    
                    tuboExternoX = abertura == Abertura.aEsqueda ? X + settings.TuboExternoDistanciaInicial : X ;
                    tuboExternoY = Y - distanciaCantoneiraGC;

                    tuboInternoX = abertura == Abertura.aEsqueda ? X : X + comprimento - settings.TuboInternoComprimento;
                    tuboInternoY = Y - distanciaCantoneiraGC - distanciaTuboInterno;
                    
                    break;
                case Posicao.VoltadoDireita:
                    posicaoRetangulo = Posicao.Vertical;

                    tuboExternoX = X + distanciaCantoneiraGC;
                    tuboExternoY = abertura == Abertura.aEsqueda ? Y : Y - settings.TuboExternoDistanciaInicial;

                    tuboInternoX = X + distanciaCantoneiraGC + distanciaTuboInterno;
                    tuboInternoY = abertura == Abertura.aEsqueda ? Y - comprimento + settings.TuboInternoComprimento : Y ; 

                    break;
                case Posicao.VoltadoCima:
                    posicaoRetangulo = Posicao.Horizontal;
                    
                    tuboExternoX = abertura == Abertura.aEsqueda ? X + settings.TuboExternoDistanciaInicial : X; 
                    tuboExternoY = Y - settings.CantoneiraComprimento + settings.Largura + distanciaCantoneiraGC;

                    tuboInternoX = abertura == Abertura.aEsqueda ? X : X + comprimento - settings.TuboInternoComprimento;
                    tuboInternoY = Y - settings.CantoneiraComprimento + settings.Largura + distanciaCantoneiraGC - distanciaTuboInterno;
                    
                    
                    break;
                default:

                    posicaoRetangulo = Posicao.Vertical;

                    tuboExternoX = X - distanciaCantoneiraGC - settings.Largura;
                    tuboExternoY = abertura == Abertura.aEsqueda ? Y - settings.TuboExternoDistanciaInicial : Y;

                    tuboInternoX = X - distanciaCantoneiraGC - settings.Largura + distanciaTuboInterno;
                    tuboInternoY = abertura == Abertura.aEsqueda ? Y : Y - comprimento + settings.TuboInternoComprimento;

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
                        PosteY = Y + (settings.PosteComprimento - settings.PosteReforcoDistancia);
                        break;
                    default:
                        PosteX = X - (settings.PosteReforcoComprimento - settings.PosteReforcoDistancia - settings.Largura);
                        PosteY = Y - +((comprimento / 2) - (settings.PosteReforcoLargura / 2));
                        break;
                }

                this.PosteReforco = new PosteReforco(new Point2d(PosteX, PosteY), posicao);
            }


            var tuboInterno = new Retangulo(settings.TuboInternoLargura, settings.TuboInternoComprimento, new Point2d(tuboInternoX, tuboInternoY), posicaoRetangulo, Model.Layer.TuboInterno);
            var tuboExterno = new Retangulo(largura, comprimentoTuboExterno, new Point2d(tuboExternoX, tuboExternoY), posicaoRetangulo, Model.Layer.TuboExterno);

            this.Tubos.Add(tuboInterno);
            this.Tubos.Add(tuboExterno);
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

            var settings = new Settings(true);
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

            var posteExterno = new Retangulo(settings.PosteReforcoLargura, settings.PosteReforcoComprimento, new Point2d(X, Y), posicaoRetangulo, Model.Layer.PosteReforco);
            poste.Add(posteExterno);
   
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
