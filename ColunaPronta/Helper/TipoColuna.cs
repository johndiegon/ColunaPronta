namespace ColunaPronta.Model
{
    using Newtonsoft.Json;
    using System.IO;
    using System.Text;
    using Autodesk.AutoCAD.Geometry;
    using NLog.Config;
    using System.Collections.Generic;
    using System.Linq;

    namespace FormaPronta.Helper
    {
        public class Options
        {
            const string arquivoJson = "C:\\Autodesk\\ColunaPronta\\TipoColuna\\layoutColunaPronta.json";

            public int iLayout(Coluna coluna)
            {
                var layouts = GetLayouts();

                foreach( Coluna layout in layouts)
                {
                    if(   coluna.Comprimento        == layout.Comprimento
                       && coluna.Largura            == layout.Largura
                       && coluna.Altura             == layout.Altura
                       && coluna.DiametroParafuso   == layout.DiametroParafuso
                       && coluna.DiametroSapata     == layout.DiametroSapata
                       && coluna.QuantidadeParafuso == layout.QuantidadeParafuso
                       && coluna.ParafusoA          == layout.ParafusoA
                       && coluna.ParafusoB          == layout.ParafusoB
                       && coluna.ParafusoC          == layout.ParafusoC
                       && coluna.ParafusoD          == layout.ParafusoD
                       && coluna.ParafusoE          == layout.ParafusoE
                       && coluna.ParafusoF          == layout.ParafusoF
                       && coluna.ParafusoG          == layout.ParafusoG
                       && coluna.ParafusoH          == layout.ParafusoH
                       && coluna.SapataA            == layout.SapataA
                       && coluna.SapataB            == layout.SapataB
                       && coluna.SapataC            == layout.SapataC
                       && coluna.SapataD            == layout.SapataD
                       && coluna.PassanteA          == layout.PassanteA
                       && coluna.PassanteB          == layout.PassanteB
                       && coluna.PassanteC          == layout.PassanteC
                       && coluna.PassanteD          == layout.PassanteD
                       && coluna.eleAmarelo         == layout.eleAmarelo
                       && coluna.eleVermelho        == layout.eleVermelho
                       && coluna.eleAzul            == layout.eleAzul
                       && coluna.eleCinza           == layout.eleCinza
                      )
                    {
                      return layout.iColuna;
                    }
                }

                var layoutNovo = layouts.Max().iColuna + 1;
                coluna.iColuna = layoutNovo;
                layouts.Add(coluna);
                AddLayout(layouts);

                return layoutNovo;
            }

            public List<Coluna> GetLayouts()
            {
                try
                {
                    string jsonSettings;
                    if (File.Exists(arquivoJson))
                    {
                        jsonSettings = File.ReadAllText(arquivoJson);
                        return JsonConvert.DeserializeObject<List<Coluna>>(jsonSettings);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (System.Exception e)
                {
                    NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                    NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                    Logger.Error(e.ToString());
                    return null;
                }
            }

            public void AddLayout(List<Coluna> layouts)
            {
                try
                {

                    if (File.Exists(arquivoJson))
                    {
                        File.Delete(arquivoJson);

                    }

                    var arquivoLayout = JsonConvert.SerializeObject(layouts);

                    using (FileStream fs = File.Create(arquivoJson))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(layouts);
                        fs.Write(info, 0, info.Length);
                    }

                }
                catch (System.Exception e)
                {
                    NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                    NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                    Logger.Error(e.ToString());
                }
            }
        }

    }

}
