//namespace ColunaPronta.Model
//{
//    using FormaPronta.Objects;
//    using Newtonsoft.Json;
//    using System.IO;
//    using System.Text;
//    using Autodesk.AutoCAD.Geometry;
//    using Autodesk.AutoCAD.Runtime;
//    using System;
//    using NLog.Config;

//    namespace FormaPronta.Helper
//    {
//        public class Options
//        {
//            const string caminhoJsonSettings = "C:\\Autodesk\\FundoViga\\projectSettings.json";

//            public ProjetctSettings GetOptions()
//            {
//                try
//                {
//                    string jsonSettings;
//                    if (File.Exists(caminhoJsonSettings))
//                    {
//                        jsonSettings = File.ReadAllText(caminhoJsonSettings);
//                        return JsonConvert.DeserializeObject<ProjetctSettings>(jsonSettings);
//                    }
//                    else
//                    {
//                        ProjetctSettings projetctSettings = new ProjetctSettings
//                        {
//                            Nome = "fundoviga",
//                            FundoViga = new Objects.FundoViga { ComprimentoLimite = 4.8, ComprimentoPadrao = 3 },
//                            Sarrafo = new Sarrafo { Largura = 0.07, Comprimento = 0.30 },
//                            DistanciaPadrao = 1.5,
//                            UltimoPonto = new Ponto3d { X = 0, Y = 0, Z = 0 },
//                            TituloPonto = new Ponto3d { X = 5, Y = 5, Z = 0 },
//                            DimensionPonto = 10,
//                            RadiusCircle = 10
//                        };

//                        SetProjetctSettings(projetctSettings);
//                        return projetctSettings;
//                    }
//                }
//                catch (System.Exception e)
//                {
//                    NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
//                    NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\FundoViga\NLog.config");
//                    Logger.Error(e.ToString());
//                    return null;
//                }
//            }

//            public void SetUltimoPonto(Point3d point3D)
//            {
//                try
//                {
//                    string jsonSettings;

//                    if (File.Exists(caminhoJsonSettings))
//                    {
//                        jsonSettings = File.ReadAllText(caminhoJsonSettings);
//                        ProjetctSettings projetctSettings = JsonConvert.DeserializeObject<ProjetctSettings>(jsonSettings);

//                        File.Delete(caminhoJsonSettings);

//                        projetctSettings.UltimoPonto.X = point3D.X;
//                        projetctSettings.UltimoPonto.Y = point3D.Y;
//                        projetctSettings.UltimoPonto.Z = point3D.Z;


//                        var ArquivoProjetctSettings = JsonConvert.SerializeObject(projetctSettings);

//                        using (FileStream fs = File.Create(caminhoJsonSettings))
//                        {
//                            byte[] info = new UTF8Encoding(true).GetBytes(ArquivoProjetctSettings);
//                            fs.Write(info, 0, info.Length);
//                        }
//                    }
//                    else
//                    {
//                        ProjetctSettings projetctSettings = new ProjetctSettings
//                        {
//                            Nome = "fundoviga",
//                            FundoViga = new Objects.FundoViga { ComprimentoLimite = 4.8, ComprimentoPadrao = 3 },
//                            Sarrafo = new Sarrafo { Largura = 0.07, Comprimento = 0.30 },
//                            DistanciaPadrao = 1.5,
//                            UltimoPonto = { X = point3D.X, Y = point3D.Y, Z = point3D.Z },
//                            TituloPonto = new Ponto3d { X = 5, Y = 5, Z = 0 }
//                        };

//                        var ArquivoProjetctSettings = JsonConvert.SerializeObject(projetctSettings);

//                        using (FileStream fs = File.Create(caminhoJsonSettings))
//                        {
//                            byte[] info = new UTF8Encoding(true).GetBytes(ArquivoProjetctSettings);
//                            fs.Write(info, 0, info.Length);
//                        }

//                    }

//                }
//                catch (System.Exception e)
//                {
//                    NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
//                    NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\FundoViga\NLog.config");
//                    Logger.Error(e.ToString());
//                }
//            }

//            public void SetProjetctSettings(ProjetctSettings projetctSettings)
//            {
//                var ArquivoProjetctSettings = JsonConvert.SerializeObject(projetctSettings);

//                using (FileStream fs = File.Create(caminhoJsonSettings))
//                {
//                    byte[] info = new UTF8Encoding(true).GetBytes(ArquivoProjetctSettings);
//                    fs.Write(info, 0, info.Length);
//                }

//            }
//        }

//    }

//}
