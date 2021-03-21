using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Contracts
{
    public  class Settings
    {

        public int Escala { get; set; }
        public double Largura { get; set; }
        public double Altura { get; set; }
        public double ComprimentoPadrao { get; set; }
        public double ComprimentoMaxima { get; set; }
        public double ComprimentoMinimoReforco { get; set; }
        public double PosteComprimento { get; set; }
        public double PosteLargura { get; set; }
        public double CantoneiraLargura { get; set; }
        public double CantoneiraComprimento { get; set; }
        public double CantoneiraFolga { get; set; }
        public double CantoneiraEspessura { get; set; }
        public double DistanciaCantoneiraGC { get; set; }
        public double DistanciaCantoneiraL { get; set; }
        public double PosteReforcoLargura { get; set; }
        public double PosteReforcoComprimento { get; set; }
        public double PosteReforcoAltura { get; set; }
        public double PosteReforcoDistancia { get; set; }
        public double CantoneiraPosteLargura { get; set; }
        public double CantoneiraPosteComprimento { get; set; }
        public double PosteReforcoCantoneira { get; set; }
        public double CantoneiraPosteFolga { get; set; }
        public double ParafusoRaio { get; set; }
        public double PosteEspessura { get; set; }
    }
}
