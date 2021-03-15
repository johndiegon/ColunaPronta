using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ColunaPronta.Model
{
    public enum Layer
    {
        Cantoneira, 
        CantoneiraL,
        Tubo, 
        Poste, 
        PosteReforco
    }
    public class EspecificacaoLayer
    {
        public EspecificacaoLayer()
        {
            Detalhes = GetDetalhes();
        }
        public class Detalhe
        {
            public string Objeto { get; set; }
            public string Nome { get; set; }
        }
        public List<Detalhe> Detalhes { get; set;}
        public static EspecificacaoLayer.Detalhe FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Detalhe detalhe = new Detalhe();

            detalhe.Objeto = (values[0]).ToUpper();
            detalhe.Nome = values[1];

            return detalhe;
        }
        public string GetNomeLayer(Layer layer)
        {
            var nomeLayers = (from detalhe in Detalhes
                              where detalhe.Objeto == layer.ToString().ToUpper()
                              select new Detalhe
                              {
                                  Nome = detalhe.Nome,
                                  Objeto = detalhe.Objeto
                              }
                            ).FirstOrDefault();

            return nomeLayers == null ? "" : nomeLayers.Nome;
        }
        private static List<EspecificacaoLayer.Detalhe> GetDetalhes()
        {
            try
            {
                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\Settings\\especificaolayers.csv";

                if (File.Exists(nomeArquivo))
                {

                    List<EspecificacaoLayer.Detalhe> values = File.ReadAllLines(nomeArquivo)
                                                       .Skip(1)
                                                       .Select(v => EspecificacaoLayer.FromCsv(v))
                                                       .ToList();
                    return values;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\FundoViga\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
    }
    

   

}
