using Newtonsoft.Json;
using NLog.Config;
using System;
using System.IO;
using System.Text;

namespace ColunaPronta.Model
{
    public class Settings
    {
        private int _escala;
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
        private double posteReforcoLargura;
        private double posteReforcoComprimento;
        private double posteReforcoAltura;
        private double posteReforcoDistancia;
        private double cantoneiraPosteLargura;
        private double cantoneiraPosteComprimento;
        private double parafusoRaio;
        private double posteEspessura;
        private double posteReforcoCantoneira;
        private double cantoneiraPosteFolga;
        private double tuboInternoComprimento;
        private double tuboInternoLargura;
        private double tuboInternoComprimentoInicial;
        private double tuboExternoComprimento;
        private double tuboExternoLargura;
        private double tuboExternoComprimentoInicial;
        private double tuboExternoDistanciaInicial;
        const string arquivoJson = "C:\\Autodesk\\ColunaPronta\\Settings\\settingsGuardaCorpo.json";

        #region >> Parametros Get Set 
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
        public double PosteReforcoLargura
        {
            get { return posteReforcoLargura / _escala; }
            set { posteReforcoLargura = value; }
        }
        public double PosteReforcoComprimento
        {
            get { return posteReforcoComprimento / _escala; }
            set { posteReforcoComprimento = value; }
        }

        public double PosteReforcoAltura
        {
            get { return posteReforcoAltura / _escala; }
            set { posteReforcoAltura = value; }
        }

        public double PosteReforcoDistancia
        {
            get { return posteReforcoDistancia / _escala; }
            set { posteReforcoDistancia  = value; }
        }

        public double CantoneiraPosteLargura
        {
            get { return cantoneiraPosteLargura / _escala; }
            set { cantoneiraPosteLargura = value; }
        }
        public double CantoneiraPosteComprimento
        {
            get { return cantoneiraPosteComprimento / _escala; }
            set { cantoneiraPosteComprimento = value; }
        }

        public double PosteReforcoCantoneira
        {
            get { return posteReforcoCantoneira / _escala; }
            set { posteReforcoCantoneira = value; }
        }
        public double CantoneiraPosteFolga
        {
            get { return cantoneiraPosteFolga / _escala; }
            set { cantoneiraPosteFolga = value; }
        }
        
        public double ParafusoRaio
        {
            get { return parafusoRaio / _escala; }
            set { parafusoRaio = value; }
        }
        public double PosteEspessura
        {
            get { return posteEspessura / _escala; }
            set { posteEspessura = value; }
        }

        public double TuboInternoComprimento
        {
            get { return tuboInternoComprimento / _escala; }
            set { tuboInternoComprimento = value; }
        }
        public double TuboInternoLargura
        {
            get { return tuboInternoLargura / _escala; }
            set { tuboInternoLargura = value; }
        }

        public double TuboInternoComprimentoInicial
        {
            get { return tuboInternoComprimentoInicial / _escala; }
            set { tuboInternoComprimentoInicial = value; }
        }
        public double TuboExternoComprimento
        {
            get { return tuboExternoComprimento / _escala; }
            set { tuboExternoComprimento = value; }
        }
        public double TuboExternoLargura
        {
            get { return tuboExternoLargura / _escala; }
            set { tuboExternoLargura = value; }
        }

        public double TuboExternoComprimentoInicial
        {
            get { return tuboExternoComprimentoInicial / _escala; }
            set { tuboExternoComprimentoInicial = value; }
        }
        public double TuboExternoDistanciaInicial
        {
            get { return tuboExternoDistanciaInicial / _escala; }
            set { tuboExternoDistanciaInicial = value; }
        }

        #endregion

        public Settings(bool configura)
        {
            if(configura)
            {
                _escala = 1000;
                SetSetting();
            }
        }
        public Settings(int escala)
        {
            _escala = escala;
            SetSetting();
        }
        public Settings() { }
        private void SetSetting()
        {
            try
            {
                if (File.Exists(arquivoJson))
                {
                    var jsonLayout = File.ReadAllText(arquivoJson);
                    var arquivo = JsonConvert.DeserializeObject<Contracts.Settings>(jsonLayout);

                    this.Altura = arquivo.guardaCorpo.Altura;
                    this.Largura = arquivo.guardaCorpo.Largura;
                    this.ComprimentoPadrao = arquivo.guardaCorpo.ComprimentoPadrao;
                    this.ComprimentoMaxima = arquivo.guardaCorpo.ComprimentoMaxima;
                    this.ComprimentoMinimoReforco = arquivo.guardaCorpo.ComprimentoMinimoReforco;
                    this.PosteComprimento = arquivo.poste.Comprimento;
                    this.PosteLargura = arquivo.poste.Largura;
                    this.PosteEspessura = arquivo.poste.Espessura;
                    this.PosteReforcoLargura = arquivo.posteReforco.Largura;
                    this.PosteReforcoComprimento = arquivo.posteReforco.Comprimento;
                    this.PosteReforcoDistancia = arquivo.posteReforco.Distancia;
                    this.PosteReforcoAltura = arquivo.posteReforco.Altura;
                    this.CantoneiraPosteLargura = arquivo.posteReforco.Cantoneira.Largura;
                    this.CantoneiraPosteComprimento = arquivo.posteReforco.Cantoneira.Comprimento;
                    this.CantoneiraEspessura = arquivo.cantoneira.Espessura;
                    this.CantoneiraFolga = arquivo.cantoneira.Folga;
                    this.CantoneiraLargura = arquivo.cantoneira.Largura;
                    this.DistanciaCantoneiraGC = arquivo.cantoneira.DistanciaGuardaCorpo;
                    this.DistanciaCantoneiraL = arquivo.cantoneira.DistanciaCantoneiraL;
                    this.CantoneiraComprimento = arquivo.cantoneira.Comprimento;
                    this.ParafusoRaio = arquivo.parafuso.Raio;
                    this.tuboInternoComprimento = arquivo.tuboInterno.Comprimento;
                    this.tuboInternoLargura = arquivo.tuboInterno.Largura;
                    this.tuboInternoComprimentoInicial = arquivo.tuboInterno.ComprimentoInicial;
                    this.tuboExternoComprimento = arquivo.tuboExterno.Comprimento;
                    this.tuboExternoLargura = arquivo.tuboExterno.Largura;
                    this.tuboExternoComprimentoInicial = arquivo.tuboExterno.ComprimentoInicial;
                    this.tuboExternoDistanciaInicial = arquivo.tuboExterno.DistanciaInicial;

                }
                else
                {

                    this.Altura = 961.9;
                    this.Largura = 30;
                    this.ComprimentoPadrao = 3000;
                    this.ComprimentoMaxima = 4000;
                    this.ComprimentoMinimoReforco = 1000;
                    this.PosteComprimento = 70;
                    this.PosteLargura = 30;
                    this.PosteReforcoLargura = 30;
                    this.PosteReforcoComprimento = 30;
                    this.PosteReforcoDistancia = 8;
                    this.CantoneiraEspessura = 3;
                    this.CantoneiraFolga = 2;
                    this.CantoneiraLargura = 38;
                    this.DistanciaCantoneiraGC = 18;
                    this.DistanciaCantoneiraL = 10;
                    this.CantoneiraComprimento = 70;
                    this.cantoneiraPosteLargura = 30;
                    this.cantoneiraPosteComprimento = 38;
                    this.cantoneiraPosteFolga = 6f;
                    this.parafusoRaio = 3;
                    this.posteEspessura = 1.5;
                    this.PosteReforcoAltura = 961.9;
                    this.PosteReforcoCantoneira = 35;
                    this.tuboInternoComprimento = 400;
                    this.tuboInternoLargura = 25;
                    this.tuboInternoComprimentoInicial =143.7;
                    this.tuboExternoComprimento = 143.7;
                    this.tuboExternoLargura = 30;
                    this.tuboExternoComprimentoInicial = 1293;
                    this.tuboExternoDistanciaInicial = 145;
                    Save();
                }

            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public void Save()
        {
            try
            {
                var arquivo = new Contracts.Settings();
                arquivo.guardaCorpo.Altura = this.Altura;
                arquivo.guardaCorpo.Largura = this.Largura;
                arquivo.guardaCorpo.ComprimentoPadrao = this.ComprimentoPadrao;
                arquivo.guardaCorpo.ComprimentoMaxima = this.ComprimentoMaxima;
                arquivo.guardaCorpo.ComprimentoMinimoReforco = this.ComprimentoMinimoReforco;
                arquivo.poste.Comprimento = this.PosteComprimento;
                arquivo.poste.Largura = this.PosteLargura;
                arquivo.poste.Espessura = this.PosteEspessura;
                arquivo.posteReforco.Largura = this.PosteReforcoLargura;
                arquivo.posteReforco.Comprimento = this.PosteReforcoComprimento;
                arquivo.posteReforco.Distancia = this.PosteReforcoDistancia;
                arquivo.posteReforco.Altura = this.PosteReforcoAltura;
                arquivo.posteReforco.Cantoneira.Largura = this.CantoneiraPosteLargura;
                arquivo.posteReforco.Cantoneira.Comprimento = this.CantoneiraPosteComprimento;
                arquivo.cantoneira.Espessura = this.CantoneiraEspessura;
                arquivo.cantoneira.Folga = this.CantoneiraFolga;
                arquivo.cantoneira.Largura = this.CantoneiraLargura;
                arquivo.cantoneira.DistanciaGuardaCorpo = this.DistanciaCantoneiraGC;
                arquivo.cantoneira.DistanciaCantoneiraL = this.DistanciaCantoneiraL;
                arquivo.cantoneira.Comprimento = this.CantoneiraComprimento;
                arquivo.parafuso.Raio = this.ParafusoRaio;
                arquivo.tuboInterno.Comprimento = this.tuboInternoComprimento;
                arquivo.tuboInterno.Largura = this.tuboInternoLargura;
                arquivo.tuboInterno.ComprimentoInicial = this.tuboInternoComprimentoInicial;
                arquivo.tuboExterno.Comprimento = this.tuboExternoComprimento;
                arquivo.tuboExterno.Largura = this.tuboExternoLargura;
                arquivo.tuboExterno.ComprimentoInicial = this.tuboExternoComprimentoInicial;
                arquivo.tuboExterno.DistanciaInicial = this.tuboExternoDistanciaInicial;

                if (File.Exists(arquivoJson))
                {
                    File.Delete(arquivoJson);
                }

                var settings = JsonConvert.SerializeObject(arquivo);

                using (FileStream fs = File.Create(arquivoJson))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(settings);
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
