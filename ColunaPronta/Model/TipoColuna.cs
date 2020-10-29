using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class TipoColuna
    {
        public string Tipo { get; set; }
        public Coluna Layout { get; set; }
        public Coluna LayoutInverto { get; set; }

    }

}
