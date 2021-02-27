using Newtonsoft.Json;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColunaPronta.Model
{
    public class Settings
    {
        const string caminhoSettings = @"C:\Autodesk\ColunaPronta\";
        public double Largura { get; set; }
        public double Altura { get; set; }
        public double ComprimentoPadrao { get; set; }
        public double ComprimentoMaxima { get; set; }
        public double ComprimentoMinimoReforco { get; set; }
        public double PosteComprimento { get; set; }
        public double PosteLargura { get; set; }
        public double CantoneiraLargura { get; set; }
        public double CantoneiraFolga { get; set; }
        public double CantoneiraEspessura { get; set; }
        public double DistanciaCantoneiraGC { get; set; }
        public Settings()
        {
            try
            {
                if (File.Exists(caminhoSettings))
                {
                    var jsonSettings = File.ReadAllText(caminhoSettings);
                    var settings = JsonConvert.DeserializeObject<Settings>(jsonSettings);
                 
                    this.Largura = settings.Largura;
                    this.Altura = settings.Altura;
                    this.ComprimentoPadrao = settings.ComprimentoPadrao;
                    this.ComprimentoMaxima = settings.ComprimentoMaxima;
                    this.ComprimentoMinimoReforco = settings.ComprimentoMinimoReforco;
                    this.PosteComprimento = settings.PosteComprimento;
                    this.PosteLargura = settings.PosteLargura;
                    this.CantoneiraLargura = settings.CantoneiraLargura;
                    this.CantoneiraFolga = settings.CantoneiraFolga;
                    this.CantoneiraEspessura = settings.CantoneiraEspessura;
                    this.DistanciaCantoneiraGC = settings.DistanciaCantoneiraGC;
                }
                else
                {
                    this.Altura = 1.75;
                    this.Largura = 0.3;
                    this.ComprimentoPadrao = 3;
                    this.ComprimentoMaxima = 4;
                    this.ComprimentoMinimoReforco = 1;
                    this.PosteComprimento = 0.30;
                    this.PosteLargura = 0.50;
                    this.CantoneiraEspessura = 0.03;
                    this.CantoneiraFolga = 0.020;
                    this.CantoneiraLargura = 0.0380;
                    this.DistanciaCantoneiraGC = 0.018;
                    SetSettings();
                }
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        private void SetSettings()
        {
            try
            {

                if (File.Exists(caminhoSettings))
                {
                    File.Delete(caminhoSettings);
                }

                var arquivo = JsonConvert.SerializeObject(this);

                using (FileStream fs = File.Create(caminhoSettings))
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
    }

}
