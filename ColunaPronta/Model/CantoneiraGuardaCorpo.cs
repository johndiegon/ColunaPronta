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
        public Point2dCollection PontosL { get; }
        public Point2dCollection PontosRetangulo { get; }
        public Point2dCollection Linha { get; }

        public CantoneiraGuardaCorpo(Point2d pontoInicial, Posicao posicao)
        {

            Point2d p1, p2, p3, p4, p5, p6;
            var settings = new Settings();
            var lado = settings.CantoneiraLargura;
            var espessura = settings.CantoneiraEspessura;

            var retangulo = 

            switch (posicao)
            {
                case Posicao.BaixoDireita:
                    p1 = new Point2d(pontoInicial.X, pontoInicial.Y);
                    p2 = new Point2d(pontoInicial.X, pontoInicial.Y - (lado ));
                    p3 = new Point2d(pontoInicial.X - ( lado ), pontoInicial.Y - (lado ));
                    p4 = new Point2d(pontoInicial.X - ( lado ), pontoInicial.Y - ((lado - espessura ) ));
                    p5 = new Point2d(pontoInicial.X - (espessura), pontoInicial.Y - ((lado - espessura ) ));
                    p6 = new Point2d(pontoInicial.X - ( espessura ), pontoInicial.Y);
                    break;
                case Posicao.BaixoEsquerda:
                    p1 = new Point2d(pontoInicial.X, pontoInicial.Y);
                    p2 = new Point2d(pontoInicial.X, pontoInicial.Y - (lado ));
                    p3 = new Point2d(pontoInicial.X + ( lado ), pontoInicial.Y - (lado ));
                    p4 = new Point2d(pontoInicial.X + ( lado ), pontoInicial.Y - ((lado - espessura ) ));
                    p5 = new Point2d(pontoInicial.X + ( espessura ), pontoInicial.Y - ((lado - espessura) ));
                    p6 = new Point2d(pontoInicial.X + (espessura), pontoInicial.Y);
                    break;
                case Posicao.CimaDireita:
                    p1 = new Point2d(pontoInicial.X, pontoInicial.Y);
                    p2 = new Point2d(pontoInicial.X + ( lado ), pontoInicial.Y);
                    p3 = new Point2d(pontoInicial.X + ( lado ), pontoInicial.Y - (lado ));
                    p4 = new Point2d(pontoInicial.X + (( lado - espessura ) ), pontoInicial.Y - ((lado) ));
                    p5 = new Point2d(pontoInicial.X + (( lado - espessura ) ), pontoInicial.Y - (espessura));
                    p6 = new Point2d(pontoInicial.X, pontoInicial.Y - (espessura));
                    break;
                default: // Posicao.CimaEsquerda
                    p1 = new Point2d(pontoInicial.X, pontoInicial.Y);
                    p2 = new Point2d(pontoInicial.X, pontoInicial.Y - (lado ));
                    p3 = new Point2d(pontoInicial.X + ( espessura ), pontoInicial.Y - (lado ));
                    p4 = new Point2d(pontoInicial.X + (espessura), pontoInicial.Y - (espessura));
                    p5 = new Point2d(pontoInicial.X + (lado ), pontoInicial.Y - (espessura));
                    p6 = new Point2d(pontoInicial.X + (lado ), pontoInicial.Y);
                    break;
            }

            PontosL.Add(p1);
            PontosL.Add(p2);
            PontosL.Add(p3);
            PontosL.Add(p4);
            PontosL.Add(p5);
            PontosL.Add(p6);

        }
    }


}
