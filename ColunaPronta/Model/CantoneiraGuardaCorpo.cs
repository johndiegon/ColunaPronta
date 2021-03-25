using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using ColunaPronta.Model;

namespace ColunaPronta.Model
{
    public class CantoneiraGuardaCorpo
    {
        private TipoCantoneira tipoCantoneira { get; set; }
        private Settings Settings { get; }
        private Posicao Posicao { get; }
        private Point2d pontoInicial { get; }
        public Point2dCollection PontosL { get; set; }
        public Retangulo Retangulo { get; set; }
        public Point2dCollection Linha { get; set; }
        public List<Circulo> Parafusos { get; set; } 
        public CantoneiraGuardaCorpo(Point2d pontoInicial, Posicao posicao, TipoCantoneira tipo)
        {
            this.tipoCantoneira = tipo;
            this.Posicao = posicao;
            this.pontoInicial = pontoInicial;
            this.Settings = new Settings(true);

            switch(tipo)
            {
                case TipoCantoneira.Cantoneira38MM:
                    SetCantoneira38MM();
                    break;
                default:
                    SetCantoneiraNormal();
                    break;
            }
        }
        public string Layer { get; set; }
        private void SetCantoneiraNormal()
        {
            PontosL = new Point2dCollection();
            Linha = new Point2dCollection();

            var lado = Settings.CantoneiraLargura;
            var espessura = Settings.CantoneiraEspessura;

            Posicao posicaoCantoneira = GetPosicaoEleCantoneira();
            Point2d pontoInicialdoELe = GetPontoInicialL();
            Point2d pontoInicialCantoneira = GetPontoInicial();

            SetPontosCantoneiraL(posicaoCantoneira, pontoInicialdoELe, lado, espessura);

            this.Retangulo = new Retangulo(Settings.CantoneiraLargura, Settings.CantoneiraComprimento, pontoInicialCantoneira, GetPosicaoRetangulo(), Model.Layer.Cantoneira);

            AddParafuso();
            SetLinha();
        }
        private void AddParafuso()
        {
            Parafusos = new List<Circulo>();
            
            if (TipoCantoneira.Cantoneira38MM == tipoCantoneira)
            {
                double X, Y;
                switch (this.Retangulo.Posicao)
                {
                    case Posicao.Vertical:
                        X = this.Retangulo.PontoInicial.X + (Settings.CantoneiraPosteLargura / 2f);
                        Y = this.Retangulo.PontoInicial.Y - (Settings.CantoneiraPosteComprimento / 2f);

                        break;
                    default://case Posicao.Horizontal:
                        X = this.Retangulo.PontoInicial.X + (Settings.CantoneiraPosteComprimento / 2f);
                        Y = this.Retangulo.PontoInicial.Y - (Settings.CantoneiraPosteLargura / 2f);
                        break;
                }
                var circulo = new Circulo
                {
                    Raio = Settings.ParafusoRaio,
                    Point = new Point3d(X, Y, 0)
                };

                this.Parafusos.Add(circulo);
            }
            else
            {
                double X1, X2, Y1, Y2;
                float distancia1 = (26f / 1000f);
                float distancia2 = (26f + 20f) / 1000f;

                switch (this.Retangulo.Posicao)
                {
                    case Posicao.Vertical:
                   
                        X1 = this.Retangulo.PontoInicial.X + (this.Retangulo.Largura/ 2);
                        Y1 = this.Retangulo.PontoInicial.Y - distancia1;
                        X2 = X1;
                        Y2 = this.Retangulo.PontoInicial.Y - distancia2;
                        break;
                    default://case Posicao.Horizontal:
                        X1 = this.Retangulo.PontoInicial.X + distancia1;
                        Y1 = this.Retangulo.PontoInicial.Y - (Settings.CantoneiraPosteLargura / 2f);
                        X2 = this.Retangulo.PontoInicial.X + distancia2;
                        Y2 = Y1;
                        break;
                }
                var circulo1 = new Circulo
                {
                    Raio = Settings.ParafusoRaio,
                    Point = new Point3d(X1, Y1, 0)
                };

                this.Parafusos.Add(circulo1);
              
                var circulo2 = new Circulo
                {
                    Raio = Settings.ParafusoRaio,
                    Point = new Point3d(X2, Y2, 0)
                };

                this.Parafusos.Add(circulo2);
            }
        }
        private void SetCantoneira38MM()
        {
            Linha = new Point2dCollection();
            this.Retangulo = new Retangulo(Settings.CantoneiraPosteLargura, Settings.CantoneiraPosteComprimento, this.pontoInicial, GetPosicaoRetangulo(), Model.Layer.Cantoneira) ;

            AddParafuso();
            SetLinha();
        }
        private Posicao GetPosicaoEleCantoneira()
        {
            Posicao posicaoCantoneira;

            var bInicio = this.tipoCantoneira == TipoCantoneira.NormalInicio ? true : false;
            switch (Posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoCantoneira = bInicio ? Posicao.CimaEsquerda : Posicao.CimaDireita;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoCantoneira = bInicio ? Posicao.CimaEsquerda : Posicao.BaixoEsquerda;
                    break;
                case Posicao.VoltadoCima:
                    posicaoCantoneira = bInicio ? Posicao.BaixoEsquerda : Posicao.BaixoDireita;
                    break;
                default: // Posicao.Esquerda
                    posicaoCantoneira = bInicio ? Posicao.CimaDireita : Posicao.BaixoDireita;
                    break;
            }
        
            return posicaoCantoneira;
        }
        private Point2d GetPontoInicialL()
        {
            double X = pontoInicial.X, Y = pontoInicial.Y;
            switch (Posicao)
            {
                case Posicao.VoltadoBaixo:
                    Y = Y - Settings.DistanciaCantoneiraGC + Settings.CantoneiraEspessura;
                    break;
                case Posicao.VoltadoDireita:
                    X = X + (Settings.DistanciaCantoneiraGC - Settings.CantoneiraEspessura); 
                    break;
                case Posicao.VoltadoCima:
                    Y = Y - Settings.CantoneiraComprimento + Settings.CantoneiraLargura + Settings.DistanciaCantoneiraGC - +Settings.CantoneiraEspessura;
                    break;
                case Posicao.VoltadoEsqueda:
                    X =  X - (Settings.CantoneiraLargura)- Settings.DistanciaCantoneiraGC + Settings.CantoneiraEspessura;
                    break;
                default:
                    break;
            }

            return new Point2d(X, Y);
        }
        private Point2d GetPontoInicial()
        {
            double X = pontoInicial.X, Y = pontoInicial.Y;
            
            if (Posicao == Posicao.VoltadoEsqueda)
                X = X - Settings.CantoneiraComprimento;
                
            return new Point2d(X, Y);
        }
        private Posicao GetPosicaoRetangulo()
        {
            Posicao posicaoRetangulo;
            switch (Posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoRetangulo = Posicao.Vertical;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoRetangulo = Posicao.Horizontal;
                    break;
                case Posicao.VoltadoCima:
                    posicaoRetangulo = Posicao.Vertical;
                    break;
                default: // Posicao.Esquerda
                    posicaoRetangulo = Posicao.Horizontal;
                    break;
            }

            return posicaoRetangulo;
        }
        private void SetPontosCantoneiraL(Posicao posicao, Point2d pontoInicial, double lado, double espessura)
        {
            Point2d p1, p2, p3, p4, p5, p6;
            double X = pontoInicial.X, Y = pontoInicial.Y;
       
            switch (posicao)
            {
                case Posicao.BaixoDireita:

                    p1 = new Point2d(X, Y - (lado));
                    p2 = new Point2d(X + (lado), Y - (lado));
                    p3 = new Point2d(X + (lado), Y );
                    p4 = new Point2d(X + (lado-espessura), Y);
                    p5 = new Point2d(X + (lado - espessura), Y - ((lado - espessura)));
                    p6 = new Point2d(X, Y - ((lado - espessura)));
                    break;
                case Posicao.BaixoEsquerda:
                    p1 = new Point2d(X, Y);
                    p2 = new Point2d(X, Y - (lado));
                    p3 = new Point2d(X + (lado), Y - (lado));
                    p4 = new Point2d(X + (lado), Y - ((lado - espessura)));
                    p5 = new Point2d(X + (espessura), Y - ((lado - espessura)));
                    p6 = new Point2d(X + (espessura), Y);
                    break;
                case Posicao.CimaDireita:
                    p1 = new Point2d(X, Y);
                    p2 = new Point2d(X + (lado), Y);
                    p3 = new Point2d(X + (lado), Y - (lado));
                    p4 = new Point2d(X + ((lado - espessura)), Y - ((lado)));
                    p5 = new Point2d(X + ((lado - espessura)), Y - (espessura));
                    p6 = new Point2d(X, Y - (espessura));
                    break;
                default: // Posicao.CimaEsquerda
                    p1 = new Point2d(X, Y);
                    p2 = new Point2d(X, Y - (lado));
                    p3 = new Point2d(X + (espessura), Y - (lado));
                    p4 = new Point2d(X + (espessura), Y - (espessura));
                    p5 = new Point2d(X + (lado), Y - (espessura));
                    p6 = new Point2d(X + (lado), Y);
                    break;
            }

            PontosL.Add(p1);
            PontosL.Add(p2);
            PontosL.Add(p3);
            PontosL.Add(p4);
            PontosL.Add(p5);
            PontosL.Add(p6);
            PontosL.Add(p1);
        }
        private void SetLinha()
        {
            double X1, Y1 , X2, Y2;
          
            if(this.tipoCantoneira == TipoCantoneira.Cantoneira38MM)
            {
                switch (Posicao)
                {
                    case Posicao.VoltadoBaixo:
                        X1 = pontoInicial.X;
                        Y1 = pontoInicial.Y - Settings.CantoneiraEspessura ;
                        X2 = pontoInicial.X + Settings.CantoneiraPosteLargura; 
                        Y2 = Y1;
                        break;
                    case Posicao.VoltadoDireita:
                        X1 = pontoInicial.X + Settings.CantoneiraEspessura;
                        Y1 = pontoInicial.Y;
                        X2 = X1;
                        Y2 = pontoInicial.Y - Settings.CantoneiraPosteLargura;
                        break;
                    case Posicao.VoltadoCima:
                        X1 = pontoInicial.X;
                        Y1 = pontoInicial.Y - (Settings.CantoneiraPosteComprimento - Settings.CantoneiraEspessura );
                        X2 = pontoInicial.X + Settings.CantoneiraPosteLargura;
                        Y2 = Y1;
                        break;
                    default:
                        X1 = pontoInicial.X + (Settings.CantoneiraPosteComprimento - Settings.CantoneiraEspessura);
                        Y1 = pontoInicial.Y;
                        X2 = X1;
                        Y2 = pontoInicial.Y - Settings.CantoneiraPosteLargura;
                        break;
                }
            }
            else
            {
                var bInicio = this.tipoCantoneira == TipoCantoneira.NormalInicio ? true : false;

                switch (Posicao)
                {
                    case Posicao.VoltadoBaixo:
                        X1 = bInicio ? pontoInicial.X + Settings.CantoneiraEspessura : pontoInicial.X + Settings.CantoneiraLargura - Settings.CantoneiraEspessura;
                        Y1 = pontoInicial.Y;
                        X2 = X1;
                        Y2 = pontoInicial.Y - Settings.CantoneiraComprimento;
                        break;
                    case Posicao.VoltadoDireita:
                        X1 = pontoInicial.X;
                        Y1 = bInicio ? pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura - Settings.CantoneiraLargura;
                        X2 = pontoInicial.X + Settings.CantoneiraComprimento; ;
                        Y2 = Y1;
                        break;
                    case Posicao.VoltadoCima:
                        X1 = bInicio ? pontoInicial.X - Settings.CantoneiraEspessura : pontoInicial.X + Settings.CantoneiraEspessura;
                        Y1 = pontoInicial.Y;
                        X2 = X1;
                        Y2 = pontoInicial.Y - Settings.CantoneiraComprimento;
                        break;
                    default:
                        X1 = pontoInicial.X;
                        Y1 = bInicio ? pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura - Settings.CantoneiraLargura;
                        X2 = pontoInicial.X - Settings.CantoneiraComprimento;
                        Y2 = Y1;
                        break;
                }
            }
            
            Linha.Add(new Point2d(X1, Y1));
            Linha.Add(new Point2d(X2, Y2));
        }
    }
    public enum TipoCantoneira
    {
        NormalInicio,
        NormalFim,
        Cantoneira38MM
    }
}
