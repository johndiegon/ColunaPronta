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
            double posteX, posteY, cantoneiraX, cantoneiraY;
            var settings = new Settings();
            var cantoneiras = new List<CantoneiraGuardaCorpo>();
            var posicaoCantoneira = posicao;
            
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicao = Posicao.Vertical;
                    posteX = pointInicial.X;
                    posteY = pointInicial.Y; 
                    cantoneiraX = posteX;
                    cantoneiraY = posteY - settings.PosteComprimento;
                    break;
                case Posicao.VoltadoCima:
                    posicao = Posicao.Vertical;
                    posteX = pointInicial.X;
                    posteY = pointInicial.Y; 
                    cantoneiraX = posteX;
                    cantoneiraY = posteY + settings.CantoneiraPosteComprimento;

                    break;
                case Posicao.VoltadoDireita:
                    posicao = Posicao.Horizontal;
                    posteX = pointInicial.X;
                    posteY = pointInicial.Y;
                    cantoneiraX = posteX + settings.PosteComprimento;
                    cantoneiraY = posteY;
                    break;
                default:
                    posicao = Posicao.Horizontal;
                    posteX = pointInicial.X -settings.PosteComprimento;
                    posteY = pointInicial.Y;
                    cantoneiraX = posteX - settings.CantoneiraPosteComprimento;
                    cantoneiraY = posteY;
                    break;
            }
        
            PosteRetangulo = new Retangulo(settings.PosteLargura, settings.PosteComprimento, new Point2d(posteX, posteY), posicao);

            var cantoneira = new CantoneiraGuardaCorpo(new Point2d(cantoneiraX, cantoneiraY), posicaoCantoneira, TipoCantoneira.Cantoneira38MM);
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

            PosteRetangulo = new Retangulo(settings.PosteReforcoLargura, settings.PosteReforcoComprimento, pointInicial, posicao);
        }

    }

    public enum TipoPoste
    {
        Reforco,
        Normal
    }
}

