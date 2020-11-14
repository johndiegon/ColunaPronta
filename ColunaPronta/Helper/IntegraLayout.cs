using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using Newtonsoft.Json;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Helper
{
    public static class IntegraLayout
    {
        const string caminhoJson = "C:\\Autodesk\\ColunaPronta\\TipoColuna\\";
        const string ultimaColunaJson = "C:\\Autodesk\\ColunaPronta\\TipoColuna\\ultimaColuna.json";


        public static int GetIColuna(Coluna coluna)
        {
            try
            {
                var arquivoJson = string.Concat(caminhoJson, coluna.NomeArquivo, ".json");

                var layouts = new List<Coluna>();
                if (File.Exists(arquivoJson))
                {
                    var jsonLayout = File.ReadAllText(arquivoJson);
                    layouts = JsonConvert.DeserializeObject<List<Coluna>>(jsonLayout);

                    foreach (Coluna layout in layouts)
                    {
                        if (   coluna.Comprimento == layout.Comprimento
                            && coluna.Largura == layout.Largura
                            && coluna.Altura == layout.Altura
                            && coluna.DiametroParafuso == layout.DiametroParafuso
                            && coluna.DiametroSapata == layout.DiametroSapata
                            && coluna.QuantidadeParafuso == layout.QuantidadeParafuso
                            && coluna.ParafusoA == layout.ParafusoA
                            && coluna.ParafusoB == layout.ParafusoB
                            && coluna.ParafusoC == layout.ParafusoC
                            && coluna.ParafusoD == layout.ParafusoD
                            && coluna.ParafusoE == layout.ParafusoE
                            && coluna.ParafusoF == layout.ParafusoF
                            && coluna.ParafusoG == layout.ParafusoG
                            && coluna.ParafusoH == layout.ParafusoH
                            && coluna.SapataA == layout.SapataA
                            && coluna.SapataB == layout.SapataB
                            && coluna.SapataC == layout.SapataC
                            && coluna.SapataD == layout.SapataD
                            && coluna.PassanteA == layout.PassanteA
                            && coluna.PassanteB == layout.PassanteB
                            && coluna.PassanteC == layout.PassanteC
                            && coluna.PassanteD == layout.PassanteD
                            && coluna.eleAmarelo == layout.eleAmarelo
                            && coluna.eleVermelho == layout.eleVermelho
                            && coluna.eleAzul == layout.eleAzul
                            && coluna.eleCinza == layout.eleCinza
                            )
                        {
                            return layout.iColuna;
                        }
                    }
                }
                int novoLayout = 1;
                if (layouts.Count() > 0)
                {
                    novoLayout = layouts.Max(x => x.iColuna) + 1;
                }

                coluna.iColuna = novoLayout;

                layouts.Add(coluna);
                layouts.Add(GetLayoutInverso(coluna));

                AddLayout(layouts, arquivoJson);

                return novoLayout;
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return 0;
            }
        }

        private static Coluna GetLayoutInverso(Coluna coluna)
        {
            var layoutInverso = new Coluna()
            {
                iColuna = coluna.iColuna,
                Comprimento = coluna.Comprimento ,
                Largura = coluna.Largura ,
                Altura = coluna.Altura,
                DiametroParafuso = coluna.DiametroParafuso,
                DiametroSapata = coluna.DiametroSapata,
                QuantidadeParafuso = coluna.QuantidadeParafuso,
                ParafusoA = coluna.ParafusoE,
                ParafusoB = coluna.ParafusoF,
                ParafusoC = coluna.ParafusoG,
                ParafusoD = coluna.ParafusoH,
                ParafusoE = coluna.ParafusoA,
                ParafusoF = coluna.ParafusoB,
                ParafusoG = coluna.ParafusoC,
                ParafusoH = coluna.ParafusoD,
                SapataA = coluna.SapataC,
                SapataB = coluna.SapataD,
                SapataC = coluna.SapataA,
                SapataD = coluna.SapataB,
                PassanteA = coluna.PassanteC,
                PassanteB = coluna.PassanteD,
                PassanteC = coluna.PassanteA,
                PassanteD = coluna.PassanteB,
                eleAmarelo = coluna.eleCinza,
                eleVermelho = coluna.eleAzul,
                eleAzul = coluna.eleVermelho,
                eleCinza = coluna.eleAmarelo
            };

            return layoutInverso;
        }

        public static Coluna GetLayout(int iColuna, string nomeArquivo)
        {
            try
            {
                var arquivoJson = string.Concat(caminhoJson, nomeArquivo, ".json");

                var layouts = new List<Coluna>();
                if (File.Exists(arquivoJson))
                {
                    var jsonLayout = File.ReadAllText(arquivoJson);
                    layouts = JsonConvert.DeserializeObject<List<Coluna>>(jsonLayout);

                    foreach (Coluna layout in layouts)
                    {
                        if (layout.iColuna ==  iColuna)
                        {
                            return layout;
                        }
                    }
                }

                return null;
           }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        private static void AddLayout(List<Coluna> layouts, string arquivoJson)
        {
            try
            {
                if (File.Exists(arquivoJson))
                {
                    File.Delete(arquivoJson);
                }
                var ArquivoLayouts = JsonConvert.SerializeObject(layouts);

                using (FileStream fs = File.Create(arquivoJson))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(ArquivoLayouts);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static Coluna GetUltimaColuna(string nomeAqruivo)
        {
            try
            {
                if (File.Exists(ultimaColunaJson))
                {
                    var jsonLayout = File.ReadAllText(ultimaColunaJson);
                    var ultimaColuna = JsonConvert.DeserializeObject<UltimaColuna>(jsonLayout);

                    return GetLayout(ultimaColuna.iColuna, nomeAqruivo);
                }
                return null;
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static void SetUltimaColuna(int iColuna)
        {
            try
            {

                if (File.Exists(ultimaColunaJson))
                {
                    File.Delete(ultimaColunaJson);
                }

                var ultimaColuna = new UltimaColuna
                {
                    iColuna = iColuna
                };

                var arquivo = JsonConvert.SerializeObject(ultimaColuna);

                using (FileStream fs = File.Create(ultimaColunaJson))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(arquivo);
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        private class UltimaColuna
        {
            public int iColuna { get; set; }
        }

    }
}