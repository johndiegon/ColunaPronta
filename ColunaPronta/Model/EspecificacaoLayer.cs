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
        TuboExterno,
        TuboInterno,
        Poste, 
        PosteReforco,
        Cotas,
        Indefinido
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
            public string Perfil { get; set; }
        }
        public List<Detalhe> Detalhes { get; set;}
        public static EspecificacaoLayer.Detalhe FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            Detalhe detalhe = new Detalhe();

            detalhe.Objeto = (values[0]).ToUpper();
            detalhe.Nome = values[1];
            detalhe.Perfil = values[2];

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
        public Detalhe GetDetalheLayer(Layer layer)
        {
            return (from detalhe in Detalhes
                              where detalhe.Objeto == layer.ToString().ToUpper()
                              select new Detalhe
                              {
                                  Nome   = detalhe.Nome,
                                  Objeto = detalhe.Objeto,
                                  Perfil = detalhe.Perfil
                              }
                            ).FirstOrDefault();
        }
        public string GetDescricaoLayer(Layer layer)
        {
            var nomeLayers = (from detalhe in Detalhes
                              where detalhe.Objeto == layer.ToString().ToUpper()
                              select new Detalhe
                              {
                                  Nome = detalhe.Nome,
                                  Objeto = detalhe.Objeto,
                                  Perfil = detalhe.Perfil
                              }
                            ).FirstOrDefault();

            return nomeLayers == null ? "" : nomeLayers.Perfil;
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
  
        public Layer GetLayer(string nome)
        {
            
            var nomeLayer = (from detalhe in Detalhes
                             where detalhe.Nome.ToUpper() == nome.ToUpper()
                             select detalhe.Objeto
                             ).FirstOrDefault();

            Layer layer = (Layer)Enum.Parse(typeof(Layer), nomeLayer ==null ? "Indefinido" : nomeLayer, true);

            return layer;
        }
    }
    

   

}
