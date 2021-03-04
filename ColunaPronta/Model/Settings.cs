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
        const string caminhoSettings = @"C:\Autodesk\ColunaPronta\SettingsGC.json";
        const int _escala = 1000;
        private double largura;
        private double altura;
        private double comprimentoPadrao;
        private double comprimentoMaxima;
        private double comprimentoMinimoReforco;
        private double posteComprimento;
        private double posteLargura;
        private double cantoneiraLargura;
        private double cantoneiraComprimento;
        private double cantoneiraFolga;
        private double cantoneiraEspessura;
        private double distanciaCantoneiraGC;
        private double distanciaCantoneiraL;
        public int Escala {  get { return _escala; } }
        public double Largura 
        { 
            get { return largura / _escala; }
            set { largura = value; }
        }
        public double Altura 
        {
            get { return altura / _escala; }
            set { altura = value; }
        }
        public double ComprimentoPadrao
        {
            get { return comprimentoPadrao / _escala;   }
            set { comprimentoPadrao = value; }
        }
        public double ComprimentoMaxima
        {
            get { return comprimentoMaxima / _escala;   }
            set { comprimentoMaxima = value; }
        }
        public double ComprimentoMinimoReforco 
        {
            get { return comprimentoMinimoReforco / _escala;  }
            set { comprimentoMinimoReforco = value; }
        }
        public double PosteComprimento
        {
            get { return posteComprimento / _escala; } 
            set { posteComprimento = value; }
        }
        public double PosteLargura 
        {
            get { return posteLargura / _escala; }
            set { posteLargura = value; }
        }
        public double CantoneiraLargura 
        {
            get { return cantoneiraLargura / _escala; }
            set { cantoneiraLargura = value; }
        }
        public double CantoneiraComprimento 
        {
            get { return cantoneiraComprimento / _escala; }
            set { cantoneiraComprimento = value; }
        }
        public double CantoneiraFolga 
        { 
            get { return cantoneiraFolga / _escala;   } 
            set { cantoneiraFolga = value; }
        }
        public double CantoneiraEspessura 
        {
            get { return cantoneiraEspessura / _escala; } 
            set { cantoneiraEspessura = value; }
        }
        public double DistanciaCantoneiraGC 
        {
            get { return distanciaCantoneiraGC / _escala; } 
            set { distanciaCantoneiraGC = value; } 
        }
        public double DistanciaCantoneiraL 
        { 
            get { return distanciaCantoneiraL / _escala; } 
            set { distanciaCantoneiraL = value; }
        }
        public Settings()
        {
            try
            {
                //if (File.Exists(caminhoSettings))
                //{
                //    var jsonSettings = File.ReadAllText(caminhoSettings);
                //    var settings = JsonConvert.DeserializeObject<Settings>(jsonSettings);
                 
                //    this.Largura                  = settings.Largura;
                //    this.Altura                   = settings.Altura;
                //    this.ComprimentoPadrao        = settings.ComprimentoPadrao;
                //    this.ComprimentoMaxima        = settings.ComprimentoMaxima;
                //    this.ComprimentoMinimoReforco = settings.ComprimentoMinimoReforco;
                //    this.PosteComprimento         = settings.PosteComprimento;
                //    this.PosteLargura             = settings.PosteLargura;
                //    this.CantoneiraLargura        = settings.CantoneiraLargura;
                //    this.CantoneiraComprimento    = settings.CantoneiraComprimento;
                //    this.CantoneiraFolga          = settings.CantoneiraFolga;
                //    this.CantoneiraEspessura      = settings.CantoneiraEspessura;
                //    this.DistanciaCantoneiraGC    = settings.DistanciaCantoneiraGC;
                //    this.DistanciaCantoneiraL     = settings.DistanciaCantoneiraL;
                //}
                //else
                //{
                    this.Altura                   = 1.75;
                    this.Largura                  = 20;
                    this.ComprimentoPadrao        = 3000;
                    this.ComprimentoMaxima        = 4000;
                    this.ComprimentoMinimoReforco = 1000;
                    this.PosteComprimento         = 70;
                    this.PosteLargura             = 30;
                    this.CantoneiraEspessura      = 3;
                    this.CantoneiraFolga          = 2;
                    this.CantoneiraLargura        = 38;
                    this.DistanciaCantoneiraGC    = 18;
                    this.DistanciaCantoneiraL     = 10;
                    this.CantoneiraComprimento    = 70;
                //    SetSettings();
                //}
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
