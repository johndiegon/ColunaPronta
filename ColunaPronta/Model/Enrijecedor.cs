using System;

namespace ColunaPronta.Model
{
    public class Enrijecedor
    {
        public double Perfil { get; set; }
        public double Valor { get; set; }

        public static Enrijecedor FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Enrijecedor enrijecedor = new Enrijecedor();
            enrijecedor.Perfil = Convert.ToDouble(values[0]);
            enrijecedor.Valor = Convert.ToDouble(values[1]);
         
            return enrijecedor;
        }
    }
}
