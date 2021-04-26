using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoVertical
    {
        private double altura;
        private double comprimento;
        private Point2d pontoInicial;
        const double distanciaDimension = (150 / 1000f);
        public List<Retangulo> EstruturasVerticais { get; set; }
        public List<Retangulo> EstruturasHorizontais { get; set; }
        public List<Retangulo> EstruturasTubosInternos { get; set; }
        public Retangulo PosteReforco { get; set; }
        public Retangulo CoberturaReforco { get; set; }
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

        public GuardaCorpoVertical(double altura, double comprimento, double comprimentoInterno, double comprimentoExterno,  Point2d pontoInicial, Abertura abertura)
        {
            this.altura = altura;
            this.comprimento = comprimento;
            this.pontoInicial = pontoInicial;
            this.Dimensions = new List<Dimension>();
            this.Linhas = new List<Point2dCollection>();

            SetEstruturasVerticais();
            SetEstruturasHorizontais(abertura, comprimentoInterno, comprimentoExterno);
            SetPosteReforco();
        }

        private void SetEstruturasVerticais()
        {
            var settings = new Settings(true);
            var estruturasVerticais = new List<Retangulo>();

            #region Estrutura do Inicio
            var estruturaVerticalInicio = new Retangulo(settings.PosteLargura, settings.Altura, this.pontoInicial, Posicao.Vertical, Model.Layer.TuboExterno);

            var dimension = new Dimension
            {
                PontoLinha1 = new Point2d(pontoInicial.X, pontoInicial.Y),
                PontoLinha2 = new Point2d(pontoInicial.X, pontoInicial.Y - settings.Altura),
                PontoDimension = new Point2d(pontoInicial.X - distanciaDimension, pontoInicial.Y),
            };
            this.Dimensions.Add(dimension);

            #endregion
            #region Estrutura do Fim
            double PontoXFinal, PontoYFinal;
            PontoXFinal = pontoInicial.X + comprimento - settings.PosteLargura;
            PontoYFinal = pontoInicial.Y;

            var estruturaVerticalFinal = new Retangulo(settings.PosteLargura, settings.Altura, new Point2d(PontoXFinal, PontoYFinal), Posicao.Vertical, Model.Layer.TuboExterno);
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

            this.EstruturasVerticais = estruturasVerticais;
        }
        private void SetEstruturasHorizontais()
        {
            var settings = new Settings(true);
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

            this.Dimensions.Add(dimensionHorizontal);

            while (alturaRestante > 0)
            {
                var posteHorizontal = new Retangulo(settings.PosteLargura, comprimentoEH, new Point2d(X, Y), Posicao.Horizontal, Model.Layer.TuboExterno);
            
                estruturasHorizontais.Add(posteHorizontal);

                double distanciaY = alturaRestante > espacamentoPadrao ? settings.PosteLargura + espacamentoPadrao : alturaRestante;
                var dimensionVertical = new Dimension
                {
                    PontoLinha1 = new Point2d(X + distanciaDimension, Y - settings.PosteLargura),
                    PontoLinha2 = new Point2d(X + distanciaDimension, Y - distanciaY),
                    PontoDimension = new Point2d(X + distanciaDimension, Y - settings.PosteLargura)
                };
                this.Dimensions.Add(dimensionVertical);

                alturaRestante = alturaRestante - settings.PosteLargura - espacamentoPadrao;
                Y = Y - settings.PosteLargura - espacamentoPadrao;
            }

            this.EstruturasHorizontais = estruturasHorizontais;
        }

        private void SetEstruturasHorizontais(Abertura abertura,double comprimentoInterno, double comprimentoExterno)
        {
            comprimentoInterno = comprimentoInterno / 1000;
            comprimentoExterno = comprimentoExterno / 1000;
            var settings = new Settings(true);
            var estruturasHorizontais = new List<Retangulo>();
            var estruturasTUbosInternos = new List<Retangulo>();
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

            this.Dimensions.Add(dimensionHorizontal);

            while (alturaRestante > 0)
            {
                var distanciaPosteInterno = ( settings.TuboExternoLargura - settings.TuboInternoLargura) / 2f;
                var posteHorizontalExterno = new Retangulo(settings.TuboExternoLargura, comprimentoExterno, abertura == Abertura.Direita ? new Point2d(X , Y) : new Point2d(X + (comprimentoEH - comprimentoExterno), Y), Posicao.Horizontal, Model.Layer.TuboExterno);
                var posteHorizontalInterno = new Retangulo(settings.TuboInternoLargura, comprimentoInterno, abertura == Abertura.Direita ? new Point2d(X + (comprimentoEH - comprimentoInterno), Y - distanciaPosteInterno) : new Point2d(X, Y), Posicao.Horizontal, Model.Layer.TuboInterno);

                estruturasHorizontais.Add(posteHorizontalExterno);
                estruturasTUbosInternos.Add(posteHorizontalInterno);

                double distanciaY = alturaRestante > espacamentoPadrao ? settings.PosteLargura + espacamentoPadrao : alturaRestante;
                var dimensionVertical = new Dimension
                {
                    PontoLinha1 = new Point2d(X + distanciaDimension, Y - settings.PosteLargura),
                    PontoLinha2 = new Point2d(X + distanciaDimension, Y - distanciaY),
                    PontoDimension = new Point2d(X + distanciaDimension, Y - settings.PosteLargura)
                };
                this.Dimensions.Add(dimensionVertical);

                alturaRestante = alturaRestante - settings.PosteLargura - espacamentoPadrao;
                Y = Y - settings.PosteLargura - espacamentoPadrao;
            }

            this.EstruturasHorizontais = estruturasHorizontais;
            this.EstruturasTubosInternos = estruturasTUbosInternos;
        }
        private void SetPosteReforco()
        {
            var settings = new Settings(true);

            if (this.comprimento >= settings.ComprimentoMinimoReforco)
            {
                double folga = settings.CantoneiraPosteFolga;

                double X = pontoInicial.X, Y = pontoInicial.Y;

                X = X + (comprimento / 2) - (settings.PosteReforcoLargura / 2);

                Y = Y + (folga);
                this.CoberturaReforco = new Retangulo(settings.PosteLargura, settings.CantoneiraPosteFolga, new Point2d(X, Y), Posicao.Vertical, Model.Layer.PosteReforco);
                
                Y = Y - (folga) - (folga / 2);
                this.PosteReforco = new Retangulo(settings.PosteLargura, settings.PosteReforcoAltura, new Point2d(X, Y), Posicao.Vertical, Model.Layer.PosteReforco);

                Y = Y - settings.PosteReforcoAltura;
                this.Cantoneira = new Retangulo(settings.PosteLargura, settings.PosteReforcoCantoneira, new Point2d(X, Y), Posicao.Vertical, Model.Layer.PosteReforco);
            }
        }

    }

}
