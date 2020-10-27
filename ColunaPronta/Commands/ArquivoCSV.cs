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
                    writer.WriteLine("TipoColuna;PontoA_X;PontoA_Y;DiametroChumbador;DiametroParafauso;QtdeParafuso;Comprimento;Largura;Altura;dAlteracao");
                }

                var linhaColuna = string.Concat((int)coluna.GetTipoColuna(), ";"
                                               , coluna.PointA.X.ToString(), ";"
                                               , coluna.PointA.Y.ToString(), ";"
                                               , coluna.DiametroSapata, ";"
                                               , coluna.DiametroParafuso, ";"
                                               , coluna.QuantidadeParafuso, ";"
                                               , coluna.Comprimento.ToString(), ";"
                                               , coluna.Largura.ToString(), ";"
                                               , coluna.Altura.ToString(), ";"
                                               , DateTime.Now.ToString()
                                               );
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

        public static List<Coluna> GetColunas(string arquivo) 
        {
            try
            {
                string nomeArquivo = string.Concat("C:\\Autodesk\\ColunaPronta\\Relatorio\\", arquivo, ".csv");

                List<Coluna> values = File.ReadAllLines(nomeArquivo)
                                                   .Skip(1)
                                                   .Select(v => Coluna.FromCsv(v))
                                                   .ToList();
                return values;

            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }

    }
}
