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
        public class GuardaCorpo
        {
            public double Largura { get; set; }
            public double Altura { get; set; }
            public double ComprimentoPadrao { get; set; }
            public double ComprimentoMaxima { get; set; }
            public double ComprimentoMinimoReforco { get; set; }

        }
        public class Poste
        {
            public double Comprimento { get; set; }
            public double Largura { get; set; }
        }

        public class Cantoneira
        {
            public double Largura { get; set; }
            public double Comprimento { get; set; }
            public double Folga { get; set; }
            public double Espessura { get; set; }
            public double DistanciaGuardaCorpo { get; set; }
            public double DistanciaCantoneiraL { get; set; }

        }
        public class PosteReforco
        {
            public double Largura { get; set; }
            public double Comprimento { get; set; }
            public double Altura { get; set; }
            public double Distancia { get; set; }

            public Cantoneira Cantoneira { get; set; } 
        }
        public class Parafuso
        {
            public double Raio { get; set; }

        }
        public class TuboInterno
        {
            public double Comprimento { get; set; }
            public double Largura { get; set; }
            public double ComprimentoInicial { get; set; }

        }
        public class TuboExterno
        {
            public double Comprimento { get; set; }
            public double Largura { get; set; }

        }
        public double PosteEspessura { get; set; }
    }
}
