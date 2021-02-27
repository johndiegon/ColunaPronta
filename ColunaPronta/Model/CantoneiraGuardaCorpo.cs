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

        }
    }


}
