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
        public List<Retangulo> Cantoneira { get; set; }
        public List<GuardaCorpoFilho> GuardaCorpos { get; set; }
        public Posicao Posicao { get; set; }
        public Settings Settings { get; set; }

        #region >> Construtor
        public GuardaCorpo(Point2d point1 , Point2d point2)
        {
            var settings = new Settings();
            this.Settings = settings.GetSettings();

            var posteInical = MessageBox.Show("Poste", "Há poste no inicio?", MessageBoxButton.YesNo);
            bPosteInicial = posteInical == MessageBoxResult.Yes ? true : false;

            var posteFinal  = MessageBox.Show("Poste", "Há poste no fim?"   , MessageBoxButton.YesNo);
            bPosteFinal = posteFinal == MessageBoxResult.Yes ? true : false;

            bVertical = point1.Y == point2.Y ? true : false;

            Largura = point1.GetDistanceTo(point2);

            SetGuardaCorpo();
        }
        #endregion

        #region >> Métodos
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

                var poste = new Retangulo(Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);

                this.Postes.Add(poste);
                comprimentoRestante = comprimentoRestante - this.Settings.PosteComprimento;

                if(bVertical)
                {
                    Y = Y - this.Settings.PosteComprimento;
                }
                {
                    X = X + this.Settings.PosteComprimento;
                }
            }

            while (comprimentoRestante > 0)
            {
                if ( comprimentoRestante > this.Settings.ComprimentoPadrao && comprimentoRestante <= this.Settings.ComprimentoMaxima)
                {
                    #region >> Gera Guarda Corpo
                    var comprimento = bPosteFinal ? comprimentoRestante - this.Settings.PosteComprimento : comprimentoRestante;
                    
                    AddGuardaCorpo(comprimento, Largura, new Point2d(X, Y));
                  
                    if (bVertical)
                    {
                        Y = Y - comprimento;
                    }
                    {
                        X = X + comprimento;
                    }
                    
                    comprimentoRestante = comprimentoRestante - comprimento;
                    
                    #endregion

                    #region >> Gera Poste Final 
                    if (bPosteFinal)
                    {
                        var poste = new Retangulo(this.Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);
                        this.Postes.Add(poste);
                        
                        if (bVertical)
                        {
                            Y = Y - this.Settings.PosteComprimento;
                        }
                        {
                            X = X + this.Settings.PosteComprimento;
                        }

                        comprimentoRestante = comprimentoRestante - this.Settings.PosteComprimento;
                    }
                    #endregion
                }
                else
                {
                    #region >> Gera Guarda Corpo

                    var comprimento = this.Settings.ComprimentoPadrao;

                    AddGuardaCorpo(comprimento, Largura, new Point2d(X, Y));

                    if (bVertical)
                    {
                        Y = Y - comprimento;
                    }
                    {
                        X = X + comprimento;
                    }
                    #endregion
                }
            }
        }
        private void AddGuardaCorpo(double comprimento, double largura, Point2d pontoInicial)
        {
            Double X = pontoInicial.X, Y = pontoInicial.Y;
       
            #region >> Definir Posicao das Cantoneiras
            Posicao posicaoCantoneiraInicio, posicaoCantoneiraFim;

            switch (this.Posicao)
            {
                case Posicao.VoltadoBaixo:
                    posicaoCantoneiraInicio = Posicao.CimaEsquerda;
                    posicaoCantoneiraFim = Posicao.CimaDireita;
                    break;
                case Posicao.VoltadoDireita:
                    posicaoCantoneiraInicio = Posicao.CimaDireita;
                    posicaoCantoneiraFim = Posicao.CimaEsquerda;
                    break;
                case Posicao.VoltadoCima:
                    posicaoCantoneiraInicio = Posicao.BaixoEsquerda;
                    posicaoCantoneiraFim = Posicao.BaixoDireita;
                    break;
                default:
                    posicaoCantoneiraInicio = Posicao.CimaEsquerda;
                    posicaoCantoneiraFim = Posicao.CimaDireita;
                    break;
            }
            #endregion

            #region >> Cantoneira Inicial

            var cantoneiraInicial = new CantoneiraGuardaCorpo( new Point2d(X,Y), posicaoCantoneiraInicio );

            if (bVertical)
            {
                Y = Y - ( this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga);
            }
            {
                X = X + (this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga);
            }

            comprimento = comprimento - (this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga);

            #endregion

            #region >> Guarda Corpo 

            var guardaCorpo = new GuardaCorpoFilho(largura, comprimento, new Point2d(X, Y), this.Posicao);

            #endregion

            #region >> Cantoneira Final 
            if (bVertical)
            {
                Y = Y - (comprimento - this.Settings.CantoneiraLargura);
            }
            {
                X = X + (comprimento - this.Settings.CantoneiraLargura);
            }

            var cantoneiraFinal = new CantoneiraGuardaCorpo(new Point2d(X, Y), posicaoCantoneiraFim);

            #endregion

            guardaCorpo.Cantoneiras.Add(cantoneiraInicial);
            guardaCorpo.Cantoneiras.Add(cantoneiraFinal);
            this.GuardaCorpos.Add(guardaCorpo);
        }
        #endregion
    } 
}
