using NLog.Config;
using System;
using System.IO;
using System.Linq;

namespace ColunaPronta.Model
{
    public class Settings
    {
        private int _escala ;
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
        private double posteReforcaoLargura;
        private double posteReforcoComprimento;
        private double posteReforcoDistancia;
        private double cantoneiraPosteLargura;
        private double cantoneiraPosteComprimento;
        private double parafusoRaio;

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
        public double PosteReforcaoLargura
        {
            get { return posteReforcaoLargura / _escala; }
            set { posteReforcaoLargura = value; }
        }
        public double PosteReforcoComprimento
        {
            get { return posteReforcoComprimento / _escala; }
            set { posteReforcoComprimento = value; }
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

        public double ParafusoRaio
        {
            get { return parafusoRaio / _escala; }
            set { parafusoRaio = value; }
        }
        public Settings()
        {
            _escala = 1000;
            SetSetting();
        }

        public Settings(int escala)
        {
            _escala = escala;
            SetSetting();
        }

        public Settings(bool csv) { }

        private void SetSetting()
        {
            try
            {
                string nomeArquivo = "C:\\Autodesk\\ColunaPronta\\Settings\\Settings.csv";

                if (File.Exists(nomeArquivo))
                {
                    var linha = File.ReadAllLines(nomeArquivo).Skip(1).FirstOrDefault();

                    string[] values = linha.Split(';');

                    this.Altura = Convert.ToDouble(values[0]);
                    this.Largura = Convert.ToDouble(values[1]);
                    this.ComprimentoPadrao = Convert.ToDouble(values[2]);
                    this.ComprimentoMaxima = Convert.ToDouble(values[3]);
                    this.ComprimentoMinimoReforco = Convert.ToDouble(values[4]);
                    this.PosteComprimento = Convert.ToDouble(values[5]);
                    this.PosteLargura = Convert.ToDouble(values[6]);
                    this.PosteReforcaoLargura = Convert.ToDouble(values[7]);
                    this.PosteReforcoComprimento = Convert.ToDouble(values[8]);
                    this.PosteReforcoDistancia = Convert.ToDouble(values[9]);
                    this.CantoneiraEspessura = Convert.ToDouble(values[10]);
                    this.CantoneiraFolga = Convert.ToDouble(values[11]);
                    this.CantoneiraLargura = Convert.ToDouble(values[12]);
                    this.DistanciaCantoneiraGC = Convert.ToDouble(values[13]);
                    this.DistanciaCantoneiraL = Convert.ToDouble(values[14]);
                    this.CantoneiraComprimento = Convert.ToDouble(values[15]);
                    this.CantoneiraPosteLargura = Convert.ToDouble(values[16]); 
                    this.CantoneiraPosteComprimento = Convert.ToDouble(values[17]); ;
                    this.ParafusoRaio = Convert.ToDouble(values[18]); 

                }
                else
                {

                    this.Altura = 1.75;
                    this.Largura = 30;
                    this.ComprimentoPadrao = 3000;
                    this.ComprimentoMaxima = 4000;
                    this.ComprimentoMinimoReforco = 1000;
                    this.PosteComprimento = 70;
                    this.PosteLargura = 30;
                    this.PosteReforcaoLargura = 30;
                    this.PosteReforcoComprimento = 106;
                    this.PosteReforcoDistancia = 8;
                    this.CantoneiraEspessura = 3;
                    this.CantoneiraFolga = 2;
                    this.CantoneiraLargura = 38;
                    this.DistanciaCantoneiraGC = 18;
                    this.DistanciaCantoneiraL = 10;
                    this.CantoneiraComprimento = 70;
                    this.cantoneiraPosteLargura = 30;
                    this.cantoneiraPosteComprimento = 38;
                    this.parafusoRaio = 3;
                }

            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

        public void ToCSV()
        {
            try
            {
                StreamWriter writer;

                string nomeArquivo = string.Concat("C:\\Autodesk\\ColunaPronta\\Settings\\settings.csv");

                if (File.Exists(nomeArquivo))
                {
                    File.Delete(nomeArquivo);
                }
                
                writer = File.CreateText(nomeArquivo);
                writer.WriteLine("Altura;Largura;ComprimentoPadrao;ComprimentoMaxima;ComprimentoMinimoReforco;PosteComprimento;PosteLargura;PosteReforcaoLargura;PosteReforcoComprimento;PosteReforcoDistancia;CantoneiraEspessura;CantoneiraFolga;CantoneiraLargura;DistanciaCantoneiraGC;DistanciaCantoneiraL;CantoneiraComprimento;CantoneiraPosteLargura;CantoneiraPosteComprimento;ParafusoRaio;");


                var settings = string.Concat(this.Altura.ToString(), ";"
                                           , this.Largura.ToString(), ";"
                                           , this.ComprimentoPadrao.ToString(), ";"
                                           , this.ComprimentoMaxima.ToString(), ";"
                                           , this.ComprimentoMinimoReforco.ToString(), ";"
                                           , this.PosteComprimento.ToString(), ";"
                                           , this.PosteLargura.ToString(), ";"
                                           , this.PosteReforcaoLargura.ToString(), ";"
                                           , this.PosteReforcoComprimento.ToString(), ";"
                                           , this.PosteReforcoDistancia.ToString(), ";"
                                           , this.CantoneiraEspessura.ToString(), ";"
                                           , this.CantoneiraFolga.ToString(), ";"
                                           , this.CantoneiraLargura.ToString(), ";"
                                           , this.DistanciaCantoneiraGC.ToString(), ";"
                                           , this.DistanciaCantoneiraL.ToString(), ";"
                                           , this.CantoneiraComprimento.ToString(), ";"
                                           , this.cantoneiraPosteLargura.ToString(), ";"
                                           , this.cantoneiraPosteComprimento.ToString(), ";"
                                           , this.parafusoRaio.ToString(), ";"
                                           ); 
                writer.WriteLine(settings);
                writer.Close();
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
