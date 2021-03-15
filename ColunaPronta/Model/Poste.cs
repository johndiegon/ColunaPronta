using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class Poste
    {
        public Retangulo PosteRetangulo { get; set; }

        public List<CantoneiraGuardaCorpo> Cantoneiras { get; set; }

        public List<Circulo> Parafuros { get; set; }

        public Poste(Point2d pontoInicial, Posicao posicao, TipoPoste tipoPoste)
        {
           
            if (tipoPoste == TipoPoste.Normal)
                SetPosteNormal(pontoInicial, posicao);
            else
                SetPosteReforco(pontoInicial, posicao);

        }
        private void SetPosteNormal(Point2d pointInicial, Posicao posicao)
        {
            double X, Y;
            var settings = new Settings();
            var cantoneiras = new List<CantoneiraGuardaCorpo>();
            var posicaoCantoneira = posicao;
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicao = Posicao.Vertical;
                    X = pointInicial.X;
                    Y = pointInicial.Y - settings.PosteComprimento;
                    break;
                case Posicao.VoltadoCima:
                    posicao = Posicao.Vertical;
                    X = pointInicial.X;
                    Y = pointInicial.Y + settings.CantoneiraPosteComprimento;
                    break;
                case Posicao.VoltadoDireita:
                    posicao = Posicao.Horizontal;
                    X = pointInicial.X + settings.PosteComprimento;
                    Y = pointInicial.Y;
                    break;
                default:
                    posicao = Posicao.Horizontal;
                    X = pointInicial.X - settings.CantoneiraPosteComprimento;
                    Y = pointInicial.Y;
                    break;
            }
       
        
            PosteRetangulo = new Retangulo(settings.PosteLargura, settings.PosteComprimento, pointInicial, posicao);

            var cantoneira = new CantoneiraGuardaCorpo(new Point2d(X, Y), posicaoCantoneira, TipoCantoneira.Cantoneira38MM);
            cantoneiras.Add(cantoneira);
            this.Cantoneiras = cantoneiras;
        }

        private void SetPosteReforco(Point2d pointInicial, Posicao posicao)
        {
            var settings = new Settings();

            if (posicao == Posicao.VoltadoBaixo || posicao == Posicao.VoltadoCima)
            {
                posicao = Posicao.Vertical;
            }
            else
            {
                posicao = Posicao.Horizontal;
            }

            PosteRetangulo = new Retangulo(settings.PosteReforcaoLargura, settings.PosteReforcoComprimento, pointInicial, posicao);

        }

    }

    public enum TipoPoste
    {
        Reforco,
        Normal
    }
}

