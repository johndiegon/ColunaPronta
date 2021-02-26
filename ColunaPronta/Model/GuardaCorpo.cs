using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using Newtonsoft.Json;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ColunaPronta.Model
{
    public class GuardaCorpo
    {
        const string caminhoSettings = @"C:\Autodesk\ColunaPronta\";
        public double Largura { get; set; }
        public double Comprimento { get; set; }
        public double Folga { get; set; }
        public bool bVertical { get; set; }
        public Point2d PontoInicio { get; set; }
        public Point2d PontoA { get; set; }
        public Point2d PontoB { get; set; }
        public bool bPosteInicial { get; set; }
        public bool bPosteFinal { get; set; }
        public List<Retangulo> Postes { get; set; }
        public List<GuardaCorpoFilho> GuardaCorpos { get; set; }
        public Posicao Posicao { get; set; }
        public Settings Settings { get; set; }
        public GuardaCorpo(Point2d point1 , Point2d point2)
        {
            this.Settings = GetSettings();

            var posteInical = MessageBox.Show("Poste", "Há poste no inicio?", MessageBoxButton.YesNo);
            bPosteInicial = posteInical == MessageBoxResult.Yes ? true : false;

            var posteFinal  = MessageBox.Show("Poste", "Há poste no fim?"   , MessageBoxButton.YesNo);
            bPosteFinal = posteFinal == MessageBoxResult.Yes ? true : false;

            bVertical = point1.Y == point2.Y ? true : false;

            Largura = point1.GetDistanceTo(point2);

            SetGuardaCorpo();
        }
        private void SetGuardaCorpo()
        {
            var comprimentoRestante = this.Comprimento;
            var X = this.PontoInicio.X;
            var Y = this.PontoInicio.Y;
            var posicaoPoste = Posicao.Horizontal;

            if (this.Posicao == Posicao.VoltadoBaixo || this.Posicao == Posicao.VoltadoCima)
            {
                posicaoPoste = Posicao.Vertical;
            }

            if (this.bPosteInicial)
            {

                var poste = new Retangulo(this.Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);

                this.Postes.Add(poste);
                comprimentoRestante = comprimentoRestante - this.Settings.PosteComprimento;
            }

            while (comprimentoRestante > 0)
            {
                if ( comprimentoRestante > this.Settings.ComprimentoPadrao && comprimentoRestante <= this.Settings.ComprimentoMaxima)
                {
                    var comprimento = bPosteFinal ? comprimentoRestante - this.Settings.PosteComprimento : comprimentoRestante;

                    var guardaCorpo = new GuardaCorpoFilho(this.Settings.Largura, comprimento,  new Point2d( X, Y), this.Posicao);
                    
                    if(bPosteFinal)
                    {
                        var poste = new Retangulo(this.Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);

                        this.Postes.Add(poste);
                        comprimentoRestante = comprimentoRestante - this.Settings.PosteComprimento;
                    }

                    this.GuardaCorpos.Add(guardaCorpo);
                    comprimentoRestante = comprimentoRestante - comprimento;
                }
                else
                {
                    var comprimento = this.Settings.ComprimentoPadrao;

                    var guardaCorpo = new GuardaCorpoFilho(this.Settings.Largura, comprimento, new Point2d(X, Y), this.Posicao);
                    this.GuardaCorpos.Add(guardaCorpo);

                    var poste = new Retangulo(this.Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);
                    this.Postes.Add(poste);

                    comprimentoRestante = comprimentoRestante - comprimento - this.Settings.PosteComprimento;
                }
            }
        }
        private Settings GetSettings()
        {
            try
            {               
                if (File.Exists(caminhoSettings))
                {
                    var jsonSettings = File.ReadAllText(caminhoSettings);
                    var settings = JsonConvert.DeserializeObject<Settings>(jsonSettings);

                    return settings;
                }
                else
                {
                    var settings = new Settings
                    {
                        Altura = 1.75,
                        Largura = 0.3,
                        ComprimentoPadrao = 3,
                        ComprimentoMaxima = 4,
                        ComprimentoMinimoReforco= 1,
                        PosteComprimento = 0.30,
                        PosteLargura = 0.50,
                        EspessuraCantoneira = 0.03,
                    };

                    SetSettings(settings);
                    return settings;
                }
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        private static void SetSettings(Settings settings)
        {
            try
            {

                if (File.Exists(caminhoSettings))
                {
                    File.Delete(caminhoSettings);
                }

                var arquivo = JsonConvert.SerializeObject(settings);

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

    public class Settings
    {
        public double Largura { get; set; }
        public double Altura { get; set; }
        public double ComprimentoPadrao { get; set; }
        public double ComprimentoMaxima { get; set; }
        public double ComprimentoMinimoReforco { get; set; }
        public double PosteComprimento { get; set; }
        public double PosteLargura { get; set; }
        public double EspessuraCantoneira { get; set; }
    }

    public class GuardaCorpoFilho
    {
        public Retangulo retangulo { get; } 
        public double Largura { get; }
        public double Comprimento { get; }
        public Retangulo PosteReforco { get;  }
        public GuardaCorpoFilho( double largura, double comprimento, Point2d pontoA, Posicao posicao)
        {
            var posicaoRetangulo = Posicao.Vertical;
            this.Largura = largura;
            this.Comprimento = comprimento;

            if (posicao == Posicao.VoltadoBaixo || posicao == Posicao.VoltadoCima)
            {
                posicaoRetangulo = Posicao.Horizontal;
            }

            this.retangulo = new Retangulo(largura, comprimento, pontoA, posicaoRetangulo);
        }
    }
   
}
