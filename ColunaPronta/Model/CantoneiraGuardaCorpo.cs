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
        private Settings Settings { get; set; }
        private Posicao Posicao { get; set; }
        private bool bInicio { get; set; }
        private Point2d pontoInicial { get; set; }
        public Point2dCollection PontosL { get; }
        public Retangulo Retangulo { get; }
        public Point2dCollection Linha { get; }
        public CantoneiraGuardaCorpo(Point2d pontoInicial, Posicao posicao, bool bInicio)
        {
            double X = pontoInicial.X, Y = pontoInicial.Y;
            this.Posicao = posicao;
            this.Settings = new Settings();
            this.bInicio = bInicio;
            var lado = Settings.CantoneiraLargura;
            var espessura = Settings.CantoneiraEspessura;

            Posicao posicaoCantoneira = GetPosicaoEleCantoneira();
            Point2d pontoInicialdoELe = GetPontoInicialL( pontoInicial);

            SetPontosCantoneiraL(posicaoCantoneira, pontoInicialdoELe, lado, espessura);

            this.Retangulo = new Retangulo(Settings.CantoneiraLargura, Settings.CantoneiraComprimento,pontoInicial, posicao );

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
                    posicaoCantoneira = bInicio ? Posicao.CimaDireita : Posicao.CimaEsquerda;
                    break;
                case Posicao.VoltadoCima:
                    posicaoCantoneira = bInicio ? Posicao.BaixoEsquerda : Posicao.BaixoDireita;
                    break;
                default: // Posicao.Esquerda
                    posicaoCantoneira = bInicio ? Posicao.CimaEsquerda : Posicao.CimaDireita;
                    break;
            }

            return posicaoCantoneira;
        }
        private Point2d GetPontoInicialL(Point2d pontoInicial)
        {
            double X = pontoInicial.X, Y = pontoInicial.Y;
            switch (Posicao)
            {
                case Posicao.VoltadoBaixo:
                    Y = Y - Settings.DistanciaCantoneiraL;
                    break;
                case Posicao.VoltadoDireita:
                    X = X + Settings.DistanciaCantoneiraL;
                    break;
                case Posicao.VoltadoCima:
                    Y = Y + Settings.DistanciaCantoneiraL;
                    break;
                case Posicao.VoltadoEsqueda:
                    X = X - Settings.DistanciaCantoneiraL;
                    break;
                default:
                    break;
            }

            return new Point2d(X, Y);
        }
        private void SetPontosCantoneiraL(Posicao posicao, Point2d pontoInicial, double lado, double espessura)
        {
            Point2d p1, p2, p3, p4, p5, p6;
            double X = pontoInicial.X, Y = pontoInicial.Y;

            switch (posicao)
            {
                case Posicao.BaixoDireita:

                    p1 = new Point2d(X, Y);
                    p2 = new Point2d(X, Y - (lado));
                    p3 = new Point2d(X - (lado), Y - (lado));
                    p4 = new Point2d(X - (lado), Y - ((lado - espessura)));
                    p5 = new Point2d(X - (espessura), Y - ((lado - espessura)));
                    p6 = new Point2d(X - (espessura), Y);
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
        }
        private void SetLinha()
        {
            double X1, Y1 , X2, Y2;

            switch (Posicao)
            {
                case Posicao.VoltadoBaixo:
                    X1 = bInicio ? pontoInicial.X + Settings.CantoneiraEspessura : pontoInicial.X - Settings.CantoneiraEspessura;
                    Y1 = pontoInicial.Y;
                    X2 = X1;
                    Y2 = pontoInicial.Y - Settings.CantoneiraComprimento;
                    break;
                case Posicao.VoltadoDireita:
                    X1 = pontoInicial.X;
                    Y1 = bInicio ?  pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura;
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
                    Y1 = bInicio ? pontoInicial.Y - Settings.CantoneiraEspessura : pontoInicial.Y + Settings.CantoneiraEspessura;
                    X2 = pontoInicial.X + Settings.CantoneiraComprimento; ;
                    Y2 = Y1;
                    break;
            }

            Linha.Add(new Point2d(X1, Y1));
            Linha.Add(new Point2d(X2, Y2));
        }
    }
}
