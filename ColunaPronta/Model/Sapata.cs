using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class Sapata
    {
        public double Comprimento { get; set; }
        public double Largura { get; set; }
        public double Chumbador { get; set; }
        public double Quantidade { get; set; }
        public Point2d PointA { get; set; }
        public Point2d PointB { get; set; }
        public Point2d PointC { get; set; }
        public Point2d PointD { get; set; }
    }
}
