using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class CantoneiraGuardaCorpo
    {
        private Settings Settings { get; }
        private Posicao Posicao { get; }
        private bool bInicio { get; }
        private Point2d pontoInicial { get; }
        public Point2dCollection PontosL { get; }
        public Retangulo Retangulo { get; }
        public Point2dCollection Linha { get; }
        public CantoneiraGuardaCorpo(Point2d pontoInicial, Posicao posicao, bool bInicio)
        {
            PontosL = new Point2dCollection();
            Linha = new Point2dCollection();
            this.pontoInicial = pontoInicial;

            this.Posicao = posicao;
            this.Settings = new Settings();
            this.bInicio = bInicio;
            var lado = Settings.CantoneiraLargura;
            var espessura = Settings.CantoneiraEspessura;

            Posicao posicaoCantoneira = GetPosicaoEleCantoneira();
            Point2d pontoInicialdoELe = GetPontoInicialL();
            Point2d pontoInicialCantoneira = GetPontoInicial();
            SetPontosCantoneiraL(posicaoCantoneira, pontoInicialdoELe, lado, espessura);

            this.Retangulo = new Retangulo(Settings.CantoneiraLargura, Settings.CantoneiraComprimento, pontoInicialCantoneira, GetPosicaoRetangulo());

            SetLinha();
        }
        private Posicao GetPosicaoEleCantoneira()
        {
            Posicao posicaoCantoneira;
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
            

            switch (Posicao)
            {
                //case Posicao.VoltadoBaixo:
                //    //Y = Y - Settings.DistanciaCantoneiraGC + Settings.CantoneiraEspessura;
                //    //X = X + Settings.CantoneiraEspessura + Settings.CantoneiraFolga;
                //    break;
                //case Posicao.VoltadoDireita:
                //    X = X;// - ( Settings.DistanciaCantoneiraGC + Settings.CantoneiraEspessura);
                //    //Y = this.bInicio == true ? Y : Y ;
                //    break;
                //case Posicao.VoltadoCima:
                //    Y = Y + Settings.DistanciaCantoneiraL;
                //    //X = X + Settings.CantoneiraEspessura + Settings.CantoneiraFolga;
                //    break;
                case Posicao.VoltadoEsqueda:
                    X = X - Settings.CantoneiraComprimento;
                    //Y = Y + Settings.CantoneiraEspessura + Settings.CantoneiraFolga;
                    break;
                default:
                    break;
            }

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
                    Y1 = bInicio ?  pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura - Settings.CantoneiraLargura;
                    X2 = pontoInicial.X + Settings.CantoneiraComprimento; ;
                    Y2 = Y1;
                    break;
                case Posicao.VoltadoCima:
                    X1 = bInicio ? pontoInicial.X - Settings.CantoneiraEspessura : pontoInicial.X + Settings.CantoneiraEspessura;
                    Y1 = pontoInicial.Y;
                    X2 = X1;
                    Y2 = pontoInicial.Y - Settings.CantoneiraComprimento;
                    break;
                default: // Posicao.Esquerda
                    X1 = pontoInicial.X;
                    Y1 = bInicio ? pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura - Settings.CantoneiraLargura;
                    X2 = pontoInicial.X - Settings.CantoneiraComprimento; 
                    Y2 = Y1;
                    break;
            }

            Linha.Add(new Point2d(X1, Y1));
            Linha.Add(new Point2d(X2, Y2));
        }
    }
}
