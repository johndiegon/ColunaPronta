using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

namespace ColunaPronta.Model
{
    public class ObjetosSelecionados
    {
        public List<Polyline> Polylines { get; set; }
        public List<Circle> Circles { get; set; }
    }
}
