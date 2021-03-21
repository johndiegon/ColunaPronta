﻿using Newtonsoft.Json;
using NLog.Config;
using System;
using System.IO;
using System.Linq;
using System.Text;

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
        const string arquivoJson = "C:\\Autodesk\\ColunaPronta\\Settings\\settingsGuardaCorpo.json";

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

                    this.Altura = arquivo.Altura;
                    this.Largura = arquivo.Largura;
                    this.ComprimentoPadrao = arquivo.ComprimentoPadrao;
                    this.ComprimentoMaxima = arquivo.ComprimentoMaxima;
                    this.ComprimentoMinimoReforco = arquivo.ComprimentoMinimoReforco;
                    this.PosteComprimento = arquivo.PosteComprimento;
                    this.PosteLargura = arquivo.PosteLargura;
                    this.PosteReforcoLargura = arquivo.PosteReforcoLargura;
                    this.PosteReforcoComprimento = arquivo.PosteReforcoComprimento;
                    this.PosteReforcoDistancia = arquivo.PosteReforcoDistancia;
                    this.CantoneiraEspessura = arquivo.CantoneiraEspessura;
                    this.CantoneiraFolga = arquivo.CantoneiraFolga;
                    this.CantoneiraLargura = arquivo.CantoneiraLargura;
                    this.DistanciaCantoneiraGC = arquivo.DistanciaCantoneiraGC;
                    this.DistanciaCantoneiraL = arquivo.DistanciaCantoneiraL;
                    this.CantoneiraComprimento = arquivo.CantoneiraComprimento;
                    this.CantoneiraPosteLargura = arquivo.CantoneiraPosteLargura;
                    this.CantoneiraPosteComprimento = arquivo.CantoneiraPosteComprimento;
                    this.ParafusoRaio = arquivo.ParafusoRaio;
                    this.PosteEspessura = arquivo.PosteEspessura;
                    this.PosteReforcoAltura = arquivo.PosteReforcoAltura;
                    this.PosteReforcoCantoneira = arquivo.PosteReforcoCantoneira;
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

                if (File.Exists(arquivoJson))
                {
                    File.Delete(arquivoJson);
                }

                var settings = JsonConvert.SerializeObject(this);

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
