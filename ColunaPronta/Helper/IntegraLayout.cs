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
        const string caminhoJson = "C:\\Autodesk\\ColunaPronta\\TipoColuna\\layoutsColuna.json";

        public static int GetIColuna(Coluna coluna)
        {
            try
            {
                var layouts = new List<Coluna>();
                if (File.Exists(caminhoJson))
                {
                    var jsonLayout = File.ReadAllText(caminhoJson);
                    layouts = JsonConvert.DeserializeObject<List<Coluna>>(jsonLayout);

                    foreach (Coluna layout in layouts)
                    {
                        if (coluna.Comprimento == layout.Comprimento
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

                var novoLayout = layouts.Max().iColuna + 1;
                coluna.iColuna = novoLayout;
                layouts.Add(coluna);

                AddLayout(layouts);

                return novoLayout;
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\FundoViga\NLog.config");
                Logger.Error(e.ToString());
                return 0;
            }
        }

        public static Coluna GetLayout(int iColuna)
        {
            try
            {
                var layouts = new List<Coluna>();
                if (File.Exists(caminhoJson))
                {
                    var jsonLayout = File.ReadAllText(caminhoJson);
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
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\FundoViga\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }

        private static void AddLayout(List<Coluna> layouts)
        {
            if (File.Exists(caminhoJson))
            {
                File.Delete(caminhoJson);

                var ArquivoLayouts = JsonConvert.SerializeObject(layouts);

                using (FileStream fs = File.Create(caminhoJson))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(ArquivoLayouts);
                    fs.Write(info, 0, info.Length);
                }
            }
        }
    }
}
