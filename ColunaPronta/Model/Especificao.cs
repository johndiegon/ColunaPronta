using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class Especificao
    {
        public string Peca { get; set; }
        public string Nomenclatura { get; set; }

        public static Especificao FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Especificao especificao = new Especificao();
            especificao.Peca = values[0];
            especificao.Nomenclatura = values[1];

            return especificao;
        }
    }
}
