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
        public GuardaCorpo(Point2d point1 , Point2d point2, Posicao posicao, bool bPInicial, bool bPFinal)
        {
            this.Posicao = Posicao;
            this.PontoA = point1;
            this.Settings = new Settings();

            //var posteInical = MessageBox.Show( "Há poste no inicio?", "Poste" , MessageBoxButton.YesNo);
            bPosteInicial = bPInicial;// posteInical == MessageBoxResult.Yes ? true : false;

            //var posteFinal  = /*MessageBox*/.Show("Há poste no fim?",  "Poste" , MessageBoxButton.YesNo);
            bPosteFinal = bPFinal; // posteFinal == MessageBoxResult.Yes ? true : false;

            
            switch(Posicao)
            {
                case Posicao.VoltadoBaixo :
                    PontoB = new Point2d(point2.X, point1.Y);
                    bVertical = false;
                    break;
                case Posicao.VoltadoCima:
                    PontoB = new Point2d(point2.X, point1.Y);
                    bVertical = false;
                    break;
                case Posicao.VoltadoDireita:
                    PontoB = new Point2d(point1.X, point2.Y);
                    bVertical = true;
                    break;
                default:
                    PontoB = new Point2d(point1.X, point2.Y);
                    bVertical = true;
                    break;
            }

            PontoInicio = PontoA;            
            Comprimento = PontoA.GetDistanceTo(PontoB);
            Largura = Settings.Largura;
            Folga = Settings.CantoneiraFolga ;

            SetGuardaCorpo();
        }
        #endregion

        #region >> Métodos
        private void SetGuardaCorpo()
        {
            var comprimentoRestante = this.Comprimento ;
            var X = this.PontoInicio.X;
            var Y = this.PontoInicio.Y;
            var posicaoPoste = Posicao.Horizontal;

            this.Postes= new List<Retangulo>();
            this.GuardaCorpos = new List<GuardaCorpoFilho>();

            if (this.Posicao == Posicao.VoltadoBaixo || this.Posicao == Posicao.VoltadoCima)
            {
                posicaoPoste = Posicao.Vertical;
            }

            if (this.bPosteInicial)
            {
                var poste = new Retangulo(Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);

                this.Postes.Add(poste);

                comprimentoRestante = comprimentoRestante - this.Settings.PosteLargura;

                if(bVertical)
                {
                    Y = Y - ( this.Settings.PosteLargura) ;
                }
                {
                    X = X + ( this.Settings.PosteLargura ) ;
                }
            }

            while (comprimentoRestante > 0)
            {
                if (comprimentoRestante > this.Settings.ComprimentoMaxima)
                {
                    #region >> Gera Guarda Corpo
                    var comprimento = Settings.ComprimentoPadrao ;

                    AddGuardaCorpo(comprimento, Largura, new Point2d( X , Y ));

                    if (bVertical)
                    {
                        Y = Y - (comprimento); /// Settings.Escala
                    }
                    {
                        X = X + (comprimento); //// Settings.Escala
                    }

                    comprimentoRestante = comprimentoRestante - comprimento;
                    #endregion

                    #region >> Add Cantoneira 

                    var poste = new Retangulo(Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);

                    this.Postes.Add(poste);

                    comprimentoRestante = comprimentoRestante - this.Settings.PosteLargura;

                    if (bVertical)
                    {
                        Y = Y - (this.Settings.PosteLargura );
                    }
                    {
                        X = X + (this.Settings.PosteLargura);
                    }

                    #endregion

                }
                else
                {
                    #region >> Gera Guarda Corpo
                    var comprimento = bPosteFinal ? comprimentoRestante - this.Settings.PosteComprimento : comprimentoRestante;

                    AddGuardaCorpo(comprimento, Largura, new Point2d(X, Y));

                    if (bVertical)
                    {
                        Y = Y - (comprimento); /// Settings.Escala
                    }
                    {
                        X = X + (comprimento); //  / Settings.Escala
                    }

                    if (bPosteFinal)
                    {
                        var poste = new Retangulo(this.Settings.PosteLargura, this.Settings.PosteComprimento, new Point2d(X, Y), posicaoPoste);
                        this.Postes.Add(poste);
                    }

                    comprimentoRestante = 0;
                    #endregion
                }

            }
        }
        private void AddGuardaCorpo(double comprimento, double largura, Point2d pontoInicial)
        {
            Double X = pontoInicial.X, Y = pontoInicial.Y;
            bool bCantoneiraInicio = true;
       
            #region >> Cantoneira Inicial

            var cantoneiraInicial = new CantoneiraGuardaCorpo( new Point2d(X,Y), this.Posicao, bCantoneiraInicio );

            bCantoneiraInicio = false;

            if (bVertical)
            {
                Y = Y - ( ( this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga ));
            }
            {
                X = X +( ( this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga ) );
            }

            #endregion

            #region >> Guarda Corpo 
          
            var guardaCorpo = new GuardaCorpoFilho(largura, comprimento, new Point2d(X, Y), this.Posicao, Settings.DistanciaCantoneiraGC);

            #endregion

            #region >> Cantoneira Final 

            if (bVertical)
            {
                Y = Y - ( comprimento - this.Settings.CantoneiraLargura ) ;
            }
            {
                X = X + ( comprimento - ( this.Settings.CantoneiraLargura -  (this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga ))  ) ;
            }

            var cantoneiraFinal = new CantoneiraGuardaCorpo(new Point2d(X, Y), this.Posicao, bCantoneiraInicio);

            #endregion
            var cantoneiras = new List<CantoneiraGuardaCorpo>();
            cantoneiras.Add(cantoneiraInicial);
            cantoneiras.Add(cantoneiraFinal);
            guardaCorpo.Cantoneiras= cantoneiras;
            this.GuardaCorpos.Add(guardaCorpo);
        }

      
        #endregion
    } 
}
