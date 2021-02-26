using Autodesk.AutoCAD.Geometry;
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
        public List<Poste> Postes { get; set; }
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
        }

        private void CriaGuardaCorpo()
        {
            var comprimentoRestante = this.Comprimento;
            var X = this.PontoInicio.X;
            var Y = this.PontoInicio.Y;

            if (this.bPosteInicial)
            {

                var poste = GetPoste(new Point2d(X, Y), this.Posicao);

                this.Postes.Add(poste);
                comprimentoRestante = comprimentoRestante - this.Settings.PosteComprimento;
            }

            while (comprimentoRestante > 0)
            {
                if ( comprimentoRestante > this.Settings.ComprimentoPadrao && comprimentoRestante <= this.Settings.ComprimentoMaxima)
                {
                    var comprimento = bPosteFinal ? comprimentoRestante - this.Settings.PosteComprimento : comprimentoRestante;

                    var guardaCorpo = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoB = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoC = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoD = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                    };

                    if(bPosteFinal)
                    {
                        var poste = GetPoste(new Point2d(X, Y), this.Posicao);

                        this.Postes.Add(poste);
                    }

                    this.GuardaCorpos.Add(guardaCorpo);
                    comprimentoRestante = comprimentoRestante - comprimento;
                }
                else
                {
                    var comprimento = this.Settings.ComprimentoPadrao;

                    var guardaCorpo = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoB = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoC = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                        PontoD = this.bVertical ? new Point2d(X, Y) : new Point2d(X, Y),
                    };

                    var poste = GetPoste(new Point2d(X, Y), this.Posicao);

                    this.Postes.Add(poste);

                    this.GuardaCorpos.Add(guardaCorpo);
                    comprimentoRestante = comprimentoRestante - comprimento - this.Settings.PosteComprimento;
                }
            }
        }

        private Poste GetPoste( Point2d pontoA , Posicao posicao )
        {
            switch(posicao)
            {
                case Posicao.VoltadoBaixo :
                    var posteVoltadoBaixo = new Poste
                    {
                        PontoA = new Point2d(pontoA.X, pontoA.Y),
                        PontoB = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y),
                        PontoC = new Point2d(pontoA.X, pontoA.Y + this.Settings.PosteComprimento),
                        PontoD = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y + this.Settings.PosteComprimento)                    
                    };
                    return posteVoltadoBaixo;
                    break;
                case Posicao.VoltadoCima :
                    var posteVoltadoCima = new Poste
                    {
                        PontoA = new Point2d(pontoA.X, pontoA.Y),
                        PontoB = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y),
                        PontoC = new Point2d(pontoA.X, pontoA.Y + this.Settings.PosteComprimento),
                        PontoD = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y + this.Settings.PosteComprimento)
                    };
                    return posteVoltadoCima;
                    break;
                case Posicao.VoltadoEsqueda :
                    var posteVoltadoEsqueda = new Poste
                    {
                        PontoA = new Point2d(pontoA.X, pontoA.Y),
                        PontoB = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y),
                        PontoC = new Point2d(pontoA.X, pontoA.Y + this.Settings.PosteComprimento),
                        PontoD = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y + this.Settings.PosteComprimento)
                    };
                    return posteVoltadoEsqueda;
                    break;
                case Posicao.VoltadoDireita :
                    var posteVoltadoDireita = new Poste
                    {
                        PontoA = new Point2d(pontoA.X, pontoA.Y),
                        PontoB = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y),
                        PontoC = new Point2d(pontoA.X, pontoA.Y + this.Settings.PosteComprimento),
                        PontoD = new Point2d(pontoA.X + this.Settings.PosteLargura, pontoA.Y + this.Settings.PosteComprimento)
                    };
                    return posteVoltadoDireita;
                    break;
                default:
                    return null;
                    break;
            }
        }

        private GuardaCorpoFilho GetGuardaCorpo(Point2d pontoA, Posicao posicao, double comprimento)
        {
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    var guardaCorpoVoltadoBaixo = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoB = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoC = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoD = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                    };
                    if(comprimento > this.Settings.ComprimentoMinimoReforco)
                    {
                        var posteVoltadoBaixo = GetPoste(new Point2d(X, Y), this.Posicao);

                        guardaCorpoVoltadoBaixo.PosteReforco = posteVoltadoBaixo;
                    }
                    return guardaCorpoVoltadoBaixo;
                    break;
                case Posicao.VoltadoCima:
                    var guardaCorpoVoltadoCima = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoB = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoC = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoD = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                    };
                    if (comprimento > this.Settings.ComprimentoMinimoReforco)
                    {
                        var posteVoltadoCima = GetPoste(new Point2d(X, Y), this.Posicao);
                        guardaCorpoVoltadoCima.PosteReforco = posteVoltadoCima;
                    }
                    return guardaCorpoVoltadoCima;
                    break;
                case Posicao.VoltadoEsqueda:
                    var guardaCorpoVoltadoEsqueda = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoB = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoC = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoD = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                    };
                    if (comprimento > this.Settings.ComprimentoMinimoReforco)
                    {
                        var posteVoltadoEsqueda = GetPoste(new Point2d(X, Y), this.Posicao);
                        guardaCorpoVoltadoEsqueda.PosteReforco = posteVoltadoEsqueda;
                    }
                    return guardaCorpoVoltadoEsqueda;
                    break;
                case Posicao.VoltadoDireita:
                    var guardaCorpoVoltadoDireita = new GuardaCorpoFilho
                    {
                        Altura = this.Settings.Altura,
                        PontoA = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoB = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoC = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                        PontoD = this.bVertical ? new Point2d(pontoA.X, pontoA.Y) : new Point2d(pontoA.X, pontoA.Y),
                    };
                    if (comprimento > this.Settings.ComprimentoMinimoReforco)
                    {
                        var posteVoltadoDireita = GetPoste(new Point2d(X, Y), this.Posicao);
                        guardaCorpoVoltadoDireita.PosteReforco = posteVoltadoDireita;
                    }
                    return guardaCorpoVoltadoDireita;
                    break;
                default:
                    return null;
                    break;
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
        public double Altura { get; set; }
        public double ComprimentoPadrao { get; set; }
        public double ComprimentoMaxima { get; set; }
        public double ComprimentoMinimoReforco { get; set; }
        public double PosteComprimento { get; set; }
        public double PosteLargura { get; set; }
        public double EspessuraCantoneira { get; set; }
    }

    public class Poste
    {
        public Point2d PontoA { get; set; }
        public Point2d PontoB { get; set; }
        public Point2d PontoC { get; set; }
        public Point2d PontoD { get; set; }
    }
    public class GuardaCorpoFilho
    {
        public Point2d PontoA { get; set; }
        public Point2d PontoB { get; set; }
        public Point2d PontoC { get; set; }
        public Point2d PontoD { get; set; }
        public double Altura { get; set; }
        public Poste PosteReforco { get; set; }
    }
   
}
