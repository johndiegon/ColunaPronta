using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoVertical
    {
        private double altura;
        private double comprimento;
        private Point2d pontoInicial;
        const double distanciaDimension = (90 / 1000f);
        public List<Retangulo> EstruturasVerticais { get; set; }
        public List<Retangulo> EstruturasHorizontais { get; set; }
        public Retangulo PosteReforco { get; set; }
        public Retangulo Retangulo { get; set; }
        public Retangulo Cantoneira { get; set; }
        public List<Dimension> Dimensions { get; set; }
        public List<Point2dCollection> Linhas { get; set; }
        public double Comprimento { get { return comprimento; } }
        public double Altura { get { return altura; } }
        public GuardaCorpoVertical( double altura, double comprimento, Point2d pontoInicial )
        {
            this.altura = altura;
            this.comprimento = comprimento;
            this.pontoInicial = pontoInicial;
            this.Dimensions = new List<Dimension>();
            this.Linhas = new List<Point2dCollection>();

            SetEstruturasVerticais();
            SetEstruturasHorizontais();
            SetPosteReforco();
        }
        public GuardaCorpoVertical ( double comprimento, double altura)
        {
            this.altura = altura;
            this.comprimento = comprimento;
        }
        public void SetGuardaCorpo(Point2d pontoInicial)
        {
            this.pontoInicial = pontoInicial;
            this.Dimensions = new List<Dimension>();
            this.Linhas = new List<Point2dCollection>();

            SetEstruturasVerticais();
            SetEstruturasHorizontais();
            SetPosteReforco();
        }

        private void SetEstruturasVerticais()
        {
            var settings = new Settings();
            var estruturasVerticais = new List<Retangulo>();

            #region Estrutura do Inicio
            var estruturaVerticalInicio = new Retangulo(settings.PosteLargura, settings.Altura, this.pontoInicial, Posicao.Vertical);

            var dimension = new Dimension
            {
                PontoLinha1 = new Point2d(pontoInicial.X, pontoInicial.Y),
                PontoLinha2 = new Point2d(pontoInicial.X, pontoInicial.Y - settings.Altura),
                PontoDimension = new Point2d(pontoInicial.X - distanciaDimension, pontoInicial.Y),
            };
            Dimensions.Add(dimension);

            #endregion
            #region Estrutura do Fim
            double PontoXFinal, PontoYFinal;
            PontoXFinal = pontoInicial.X + comprimento - settings.PosteLargura;
            PontoYFinal = pontoInicial.Y;

            var estruturaVerticalFinal = new Retangulo(settings.PosteLargura, settings.Altura, new Point2d(PontoXFinal, PontoYFinal), Posicao.Vertical);
            estruturasVerticais.Add(estruturaVerticalInicio);
            estruturasVerticais.Add(estruturaVerticalFinal);

            #endregion

            #region Linhas das Estruturas Verticais

            var linha1 = new Point2dCollection();
            linha1.Add(new Point2d(this.pontoInicial.X + settings.CantoneiraFolga, this.pontoInicial.Y));
            linha1.Add(new Point2d(this.pontoInicial.X + settings.CantoneiraFolga, this.pontoInicial.Y - settings.Altura));
            Linhas.Add(linha1);

            var linha2 = new Point2dCollection();
            linha2.Add(new Point2d(PontoXFinal - settings.CantoneiraFolga + settings.PosteLargura, this.pontoInicial.Y));
            linha2.Add(new Point2d(PontoXFinal - settings.CantoneiraFolga + settings.PosteLargura, this.pontoInicial.Y - settings.Altura));
            Linhas.Add(linha2);

            #endregion

            EstruturasVerticais = estruturasVerticais;
        }
        private void SetEstruturasHorizontais()
        {
            var settings = new Settings();
            var estruturasHorizontais = new List<Retangulo>();
            var alturaRestante = this.altura;
            var espacamentoPadrao = 170 / 1000f;
            double X = pontoInicial.X + settings.CantoneiraEspessura + settings.CantoneiraFolga, Y = pontoInicial.Y;
            double comprimentoEH = this.comprimento - ((settings.CantoneiraEspessura + settings.CantoneiraFolga) * 2);

            var dimensionHorizontal = new Dimension
            {
                PontoLinha1 = new Point2d(pontoInicial.X, pontoInicial.Y),
                PontoLinha2 = new Point2d(pontoInicial.X + this.comprimento, pontoInicial.Y),
                PontoDimension = new Point2d(pontoInicial.X, pontoInicial.Y + distanciaDimension),
            };

            Dimensions.Add(dimensionHorizontal);


            while (alturaRestante > 0)
            {
                var posteHorizontal = new Retangulo(settings.PosteLargura, comprimentoEH, new Point2d(X, Y), Posicao.Horizontal);
                estruturasHorizontais.Add(posteHorizontal);

                double distanciaY = alturaRestante > espacamentoPadrao ? settings.PosteLargura + espacamentoPadrao : alturaRestante;
                var dimensionVertical = new Dimension
                {
                    PontoLinha1 = new Point2d(X + distanciaDimension, Y - settings.PosteLargura),
                    PontoLinha2 = new Point2d(X + distanciaDimension, Y - distanciaY),
                    PontoDimension = new Point2d(X + distanciaDimension, Y - settings.PosteLargura)
                };
                Dimensions.Add(dimensionVertical);

                alturaRestante = alturaRestante - settings.PosteLargura - espacamentoPadrao;
                Y = Y - settings.PosteLargura - espacamentoPadrao;
            }

            EstruturasHorizontais = estruturasHorizontais;
        }
        private void SetPosteReforco()
        {
            var settings = new Settings();

            if (this.comprimento >= settings.ComprimentoMinimoReforco)
            {
                double folga = settings.CantoneiraPosteFolga;

                double X = pontoInicial.X, Y = pontoInicial.Y;

                X = X + (comprimento / 2) - (settings.PosteReforcoLargura / 2);

                Y = Y + (folga / 2);
                Retangulo = new Retangulo(settings.PosteLargura, settings.CantoneiraPosteFolga, new Point2d(X, Y), Posicao.Vertical);

                Y = Y - (folga) - (folga / 2);
                PosteReforco = new Retangulo(settings.PosteLargura, settings.PosteReforcoAltura, new Point2d(X, Y), Posicao.Vertical);

                Y = Y - settings.PosteReforcoAltura;
                Cantoneira = new Retangulo(settings.PosteLargura, settings.PosteReforcoCantoneira, new Point2d(X, Y), Posicao.Vertical);
            }
        }

    }

}
