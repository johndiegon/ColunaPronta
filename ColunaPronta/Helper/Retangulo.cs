using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Helper
{
    public class Retangulo
    {
        public double Largura { get;  }
        public double Comprimento { get;  }
        public double Area { get { return Largura * Comprimento; } }
        public Point2dCollection Pontos { get;  }

        public Retangulo(double largura, double comprimento, Point2d pontoInicial, Posicao posicao)
        {
            this.Largura = largura;
            this.Comprimento = comprimento;

            switch (posicao)
            {
                case Posicao.Vertical:
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    this.Pontos.Add(new Point2d(pontoInicial.X + largura, pontoInicial.Y));
                    this.Pontos.Add(new Point2d(pontoInicial.X + largura, pontoInicial.Y - comprimento));
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y - comprimento));
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    break;
                case Posicao.Horizontal:
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    this.Pontos.Add(new Point2d(pontoInicial.X + comprimento, pontoInicial.Y));
                    this.Pontos.Add(new Point2d(pontoInicial.X + comprimento, pontoInicial.Y - largura));
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y - largura));
                    this.Pontos.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    break;
            }

        }

    }
}
