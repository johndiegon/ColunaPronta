﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class ItemRelatorio
    {
        public double Comprimento { get; set; }
        public double Largura { get; set; }
        public double Altura { get; set; }
        public TipoColuna tipoColuna { get; set; }
        public double QtdeParafuso { get; set; }
        public double QtdeColuna { get; set; }
        public double DiametroSapata { get; set; }
    }
}