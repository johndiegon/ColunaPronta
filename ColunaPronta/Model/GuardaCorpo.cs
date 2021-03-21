using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

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
        public List<Poste> Postes { get; set; }
        public List<Retangulo> Cantoneira { get; set; }
        public List<GuardaCorpoFilho> GuardaCorpos { get; set; }
        public List<Double> GuardaCorpoVertical { get; set; }

        public Posicao Posicao { get; set; }
        public Settings Settings { get; set; }

        #region >> Construtor
        public GuardaCorpo(Point2d point1 , Point2d point2, Posicao posicao, bool bPInicial, bool bPFinal)
        {
            this.Posicao = posicao;
           
            this.Settings = new Settings();

            bPosteInicial = bPInicial;// posteInical == MessageBoxResult.Yes ? true : false;

            bPosteFinal = bPFinal; // posteFinal == MessageBoxResult.Yes ? true : false;

            this.GuardaCorpoVertical = new List<double>();

            switch(Posicao)
            {
                case Posicao.VoltadoBaixo :
                    PontoA = new Point2d(point1.X, point1.Y);
                    PontoB = new Point2d(point2.X, point1.Y);
                    bVertical = false;
                    break;
                case Posicao.VoltadoCima:
                    PontoA = new Point2d(point1.X, point1.Y);
                    PontoB = new Point2d(point2.X, point1.Y);
                    bVertical = false;
                    break;
                case Posicao.VoltadoDireita:
                    PontoA = new Point2d(point1.X, point1.Y);
                    PontoB = new Point2d(point1.X, point2.Y);
                    bVertical = true;
                    break;
                default:
                    PontoA = new Point2d(point1.X, point1.Y);
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
            var Y = Posicao == Posicao.VoltadoCima ? this.PontoInicio.Y + Settings.CantoneiraComprimento : this.PontoInicio.Y;

            this.Postes= new List<Poste>();
            this.GuardaCorpos = new List<GuardaCorpoFilho>();


            if (this.bPosteInicial)
            {
                double posteX = X, postesY = Y;
              
                var poste = new Poste( new Point2d(posteX, postesY), this.Posicao, TipoPoste.Normal);

                this.Postes.Add(poste);

                comprimentoRestante = comprimentoRestante - this.Settings.PosteLargura;

                if(bVertical)
                {
                    Y = Y - ( this.Settings.PosteLargura) ;
                }else
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

                    this.GuardaCorpoVertical.Add(comprimento);


                    if (bVertical)
                    {
                        Y = Y - (comprimento); /// Settings.Escala
                    }else
                    {
                        X = X + (comprimento); //// Settings.Escala
                    }

                    comprimentoRestante = comprimentoRestante - comprimento;
                    #endregion

                    #region >> Add Cantoneira 

                    var poste = new Poste(new Point2d(X, Y), this.Posicao, TipoPoste.Normal);

                    this.Postes.Add(poste);

                    comprimentoRestante = comprimentoRestante - this.Settings.PosteLargura;

                    if (bVertical)
                    {
                        Y = Y - (this.Settings.PosteLargura);
                    } else
                    {
                        X = X + (this.Settings.PosteLargura);
                    }

                    #endregion

                }
                else
                {
                    #region >> Gera Guarda Corpo
                    double comprimento = bPosteFinal ? comprimentoRestante - this.Settings.PosteLargura - Settings.CantoneiraFolga - Settings.CantoneiraEspessura  : comprimentoRestante;

                    AddGuardaCorpo(comprimento, Largura, new Point2d(X, Y));
                    this.GuardaCorpoVertical.Add(comprimento);

                    if (bVertical)
                    {
                        Y = Y - (comprimento); /// Settings.Escala
                    } else
                    {
                        X = X + (comprimento); //  / Settings.Escala
                    }

                    if (bPosteFinal)
                    {
                        double posteX = X, posteY = Y;

                        //if (Posicao == Posicao.VoltadoEsqueda)
                        //{
                        //    posteX = posteX - Settings.PosteComprimento;
                        //}

                        
                        var poste = new Poste(new Point2d(posteX, posteY), this.Posicao, TipoPoste.Normal);
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
       
            #region >> Cantoneira Inicial

            var cantoneiraInicial = new CantoneiraGuardaCorpo( new Point2d(X,Y), this.Posicao, TipoCantoneira.NormalInicio );

            comprimento = comprimento - ((this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga)*2);

            if (bVertical)
            {
                Y = Y - ( ( this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga ));
            } else
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
                Y = Y - ( comprimento - this.Settings.CantoneiraLargura + (this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga)) ;
            } else
            {
                X = X + ( comprimento - this.Settings.CantoneiraLargura + this.Settings.CantoneiraEspessura + this.Settings.CantoneiraFolga ) ;
            }

            var cantoneiraFinal = new CantoneiraGuardaCorpo(new Point2d(X, Y), this.Posicao, TipoCantoneira.NormalFim);

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
