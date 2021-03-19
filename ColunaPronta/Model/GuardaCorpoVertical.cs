using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class GuardaCorpoVertical
    {
        private double altura;
        private double comprimento;
        private Point2d pontoInicial;

        public List<Retangulo> EstruturasVerticais { get; set; }
        public List<Retangulo> EstruturasHorizontais { get; set; }
        public Retangulo PosteReforco { get; set; }
        public Retangulo Retangulo { get; set; }
        public Retangulo Cantoneira { get; set; }
        
        public GuardaCorpoVertical( double altura, double comprimento, Point2d pontoInicial )
        {
            this.altura = altura;
            this.comprimento = comprimento;
            this.pontoInicial = pontoInicial;

            SetEstruturasVerticais();
            SetEstruturasHorizontais();
            SetPosteReforco();
        }

        private void SetEstruturasVerticais()
        {
            var settings = new Settings();
            var estruturasVerticais = new List<Retangulo>();
            
            var estruturaVerticalInicio = new Retangulo(settings.PosteLargura, settings.Altura, this.pontoInicial, Posicao.Vertical);

            double PontoXFinal, PontoYFinal;
            PontoXFinal = pontoInicial.X + comprimento - settings.PosteLargura;
            PontoYFinal = pontoInicial.Y;

            var estruturaVerticalFinal = new Retangulo(settings.PosteLargura, settings.Altura, new Point2d(PontoXFinal, PontoYFinal), Posicao.Vertical);
            estruturasVerticais.Add(estruturaVerticalInicio);
            estruturasVerticais.Add(estruturaVerticalFinal);

            EstruturasVerticais  = estruturasVerticais;
        }

        private void SetEstruturasHorizontais()
        {
            var settings = new Settings();
            var estruturasHorizontais = new List<Retangulo>();
            var alturaRestante = this.altura;
            var espacamentoPadrao = 170 / 1000f;
            double X = pontoInicial.X, Y = pontoInicial.Y;

            while(alturaRestante > 0)
            {
                var posteHorizontal = new Retangulo(settings.PosteLargura, this.comprimento, new Point2d(X, Y), Posicao.Horizontal);
                estruturasHorizontais.Add(posteHorizontal);
              
                Y = Y - settings.PosteLargura - espacamentoPadrao;
                alturaRestante = alturaRestante - settings.PosteLargura - espacamentoPadrao;
            }

            EstruturasHorizontais = estruturasHorizontais;
        }

        private void SetPosteReforco()
        {
            var settings = new Settings();
            
            if ( this.comprimento >= settings.ComprimentoMinimoReforco )
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
