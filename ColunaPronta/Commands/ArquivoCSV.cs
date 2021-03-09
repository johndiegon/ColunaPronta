using ColunaPronta.Model;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ColunaPronta.Commands
{
    public static class ArquivoCSV
    {
        #region >> Método Registrar
        public static void Registra(Coluna coluna)
        {
            try
            {
                StreamWriter writer;

                string nomeArquivo = string.Concat("C:\\Autodesk\\ColunaPronta\\Relatorio\\", coluna.NomeArquivo, ".csv");

                if (File.Exists(nomeArquivo))
                {
                    writer = File.AppendText(nomeArquivo);
                }
                else
                {
                    writer = File.CreateText(nomeArquivo);
                    writer.WriteLine("iColuna;PontoA_X;PontoA_Y;Comprimento;Largura;Altura;DiametroParafuso;DiametroSapata;QuantidadeParafuso;ParafusoA;ParafusoB;ParafusoC;ParafusoD;ParafusoE;ParafusoF;ParafusoG;ParafusoH;SapataA;SapataB;SapataC;SapataD;PassanteA;PassanteB;PassanteC;PassanteD;eleAmarelo;eleVermelho;eleAzul;eleCinza;dAlteracao;AlturaViga");
                }

                var linhaColuna = string.Concat( coluna.iColuna.ToString() , ";"
                                               , coluna.PointA.X.ToString(), ";"
                                               , coluna.PointA.Y.ToString(), ";"
                                               , coluna.Comprimento, ";"
                                               , coluna.Largura, ";"
                                               , coluna.Altura, ";"
                                               , coluna.DiametroParafuso, ";"
                                               , coluna.DiametroSapata, ";"
                                               , coluna.QuantidadeParafuso, ";"
                                               , coluna.ParafusoA == false ? 0 : 1, ";"
                                               , coluna.ParafusoB == false ? 0 : 1, ";"
                                               , coluna.ParafusoC == false ? 0 : 1, ";"
                                               , coluna.ParafusoD == false ? 0 : 1, ";"
                                               , coluna.ParafusoE == false ? 0 : 1, ";"
                                               , coluna.ParafusoF == false ? 0 : 1, ";"
                                               , coluna.ParafusoG == false ? 0 : 1, ";"
                                               , coluna.ParafusoH == false ? 0 : 1, ";"
                                               , coluna.SapataA == false ? 0 : 1, ";"
                                               , coluna.SapataB == false ? 0 : 1, ";"
                                               , coluna.SapataC == false ? 0 : 1, ";"
                                               , coluna.SapataD == false ? 0 : 1, ";"
                                               , coluna.PassanteA == false ? 0 : 1, ";"
                                               , coluna.PassanteB == false ? 0 : 1, ";"
                                               , coluna.PassanteC == false ? 0 : 1, ";"
                                               , coluna.PassanteD == false ? 0 : 1, ";"
                                               , coluna.eleAmarelo == false ? 0 : 1, ";"
                                               , coluna.eleVermelho == false ? 0 : 1, ";"
                                               , coluna.eleAzul == false ? 0 : 1, ";"
                                               , coluna.eleCinza == false ? 0 : 1, ";"
                                               , DateTime.Now.ToString() ,";"
                                               , coluna.AlturaViga
                                               ); ;
                writer.WriteLine(linhaColuna);
                writer.Close();
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void Registra(List<EspecificacaoLayer> layers)
        {
            try
            {
                StreamWriter writer;

                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\Settings\\especificaolayers.csv";

                if (File.Exists(nomeArquivo))
                {
                    File.Delete(nomeArquivo);
                    writer = File.AppendText(nomeArquivo);
                }
                else
                {
                    writer = File.CreateText(nomeArquivo);
                    writer.WriteLine("objeto;nome;");
                }

                foreach (var layer in layers)
                {

                    var linhaColuna = string.Concat(layer.Objeto.ToString(), ";"
                                                   , layer.Nome.ToString(), ";"
                                                   );
                    writer.WriteLine(linhaColuna);
                }

                writer.Close();
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        #endregion

        #region >> Método Leitura
        public static List<Coluna> GetColunas(string arquivo) 
        {
            try
            {
                string nomeArquivo = string.Concat("C:\\Autodesk\\ColunaPronta\\Relatorio\\", arquivo, ".csv");

                if (File.Exists(nomeArquivo))
                {

                    List<Coluna> values = File.ReadAllLines(nomeArquivo)
                                                       .Skip(1)
                                                       .Select(v => Coluna.FromCsv(v))
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
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static List<Enrijecedor> GetEnrijecedores()
        {
            try
            {
                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\enrijecedores.csv";

                if (File.Exists(nomeArquivo))
                {

                    List<Enrijecedor> values = File.ReadAllLines(nomeArquivo)
                                                       .Skip(1)
                                                       .Select(v => Enrijecedor.FromCsv(v))
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
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static List<Especificao> GetEspecificacao()
        {
            try
            {
                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\especificacao.csv";

                if (File.Exists(nomeArquivo))
                {

                    List<Especificao> values = File.ReadAllLines(nomeArquivo)
                                                       .Skip(1)
                                                       .Select(v => Especificao.FromCsv(v))
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
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static List<EspecificacaoLayer> GetEspecificacaoLayers()
        {
            try
            {
                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\Settings\\especificaolayers.csv";

                if (File.Exists(nomeArquivo))
                {

                    List<EspecificacaoLayer> values = File.ReadAllLines(nomeArquivo)
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
        #endregion
    }
}
