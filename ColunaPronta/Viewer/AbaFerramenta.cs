using Autodesk.Windows;
using System;
using System.Collections.Generic;
using ApplicationServices = Autodesk.AutoCAD.ApplicationServices;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows.Media.Imaging;
using System.Windows;
using ColunaPronta.Commands;
using ColunaPronta.Helper;
using ColunaPronta.Model;

namespace ColunaPronta.Viewer
{

    public class Aba
    {
        public string Nome { get; set; }
        public string Id { get; set; }
        public string NomePainel { get; set; }
        public List<RibbonButton> Comandos { get; set; }
    }

    public class AbaFerramenta
    {
        private Aba GetAbaColuna
        {
            get
            {
                var buttons = new List<RibbonButton>();

                // Botão para gerar coluna

                Uri uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\coluna.png");
                Uri uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\coluna.png");
                BitmapImage image = new BitmapImage(uriImage);
                BitmapImage largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton geracp = new RibbonButton()
                {
                    Text = "Gera Coluna",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "geracp",
                    CommandHandler = new SimpleButtonCmdHandler()
                };
                buttons.Add(geracp);

                // Botão para calcular area do fundo de viga

                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\repetir.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\repetir.png");
                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton geraucp = new RibbonButton()
                {
                    Text = "Gera ultima coluna",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "geraucp",
                    CommandHandler = new SimpleButtonCmdHandler(),
                };

                buttons.Add(geraucp);

                // Botão para gerar coluna identificada
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\relatorio.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\relatorio.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraRelatorio = new RibbonButton()
                {
                    Text = "Relatorio",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "GeraRelatorio",
                    CommandHandler = new SimpleButtonCmdHandler()
                };

                buttons.Add(GeraRelatorio);

                // Botão para gerar coluna identificada
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listaentrega.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listaentrega.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraListaEntrega = new RibbonButton()
                {
                    Text = "Lista de Entrega",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "GeraListaEntrega",
                    CommandHandler = new SimpleButtonCmdHandler()
                };

                buttons.Add(GeraListaEntrega);


                // Botão para gerar coluna identificada
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraListaCorte = new RibbonButton()
                {
                    Text = "Lista de Corte",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "GeraListaCorte",
                    CommandHandler = new SimpleButtonCmdHandler()
                };
                
                buttons.Add(GeraListaCorte);

                return  new Aba
                {
                    Nome = "Coluna Pronta",
                    Id = "Coluna_Pronta_ID",
                    NomePainel = "Coluna Pronta",
                    Comandos = buttons
                };

                
                
            }
        }

        private Aba GetAbaGuardaCorpo
        {
            get
            {
                var buttons = new List<RibbonButton>();

                // Botão para gerar coluna

                Uri uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\coluna.png");
                Uri uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\coluna.png");
                BitmapImage image = new BitmapImage(uriImage);
                BitmapImage largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton geracp = new RibbonButton()
                {
                    Text = "Gera Guarda Corpo",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "geragc",
                    CommandHandler = new SimpleButtonCmdHandler()
                };
                buttons.Add(geracp);

                // Botão para Lista de corte
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraListaCorte = new RibbonButton()
                {
                    Text = "Lista de Corte",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "geralcgc",
                    CommandHandler = new SimpleButtonCmdHandler()
                };

                buttons.Add(GeraListaCorte);

                // Botão para gerar coluna identificada
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listaentrega.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listaentrega.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraListaEntrega = new RibbonButton()
                {
                    Text = "Lista de Entrega",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "GeraListaEntregaGC",
                    CommandHandler = new SimpleButtonCmdHandler()
                };

                buttons.Add(GeraListaEntrega);

                // Botão para Lista de corte
                uriImage = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");
                uriImage2 = new Uri(@"C:\Autodesk\ColunaPronta\Icones\listacorte.png");

                image = new BitmapImage(uriImage);
                largeImage = new BitmapImage(uriImage2);

                Autodesk.Windows.RibbonButton GeraListaVisual = new RibbonButton()
                {
                    Text = "Lista Visual",
                    Size = RibbonItemSize.Large,
                    Image = image,
                    LargeImage = largeImage,
                    ShowText = true,
                    CommandParameter = "geralvgc",
                    CommandHandler = new SimpleButtonCmdHandler()
                };


                buttons.Add(GeraListaVisual);

                return new Aba
                {
                    Nome = "Guarda Corpo Pronto",
                    Id = "Gurda_Corpo_ID",
                    NomePainel = "Guarda Corpo Pronto",
                    Comandos = buttons
                };
            }
        }

        public void Add()
        {
            var abaColuna = GetAbaColuna;
            var abaGuardaCorpo = GetAbaGuardaCorpo;

            Autodesk.Windows.RibbonControl ribbonControl = Autodesk.Windows.ComponentManager.Ribbon;
            RibbonTab Tab = new RibbonTab()
            {
                Title = abaColuna.Nome,
                Id = abaColuna.Id
            };

            ribbonControl.Tabs.Add(Tab);

            #region >> Painel Coluna

            Autodesk.Windows.RibbonPanelSource srcPanelColuna = new RibbonPanelSource()
            {
                Title = abaColuna.NomePainel
            };

            RibbonPanel Panel = new RibbonPanel()
            {
                Source = srcPanelColuna
            };


            foreach (RibbonButton button in abaColuna.Comandos)
            {
                srcPanelColuna.Items.Add(button);
            }
            #endregion


            Autodesk.Windows.RibbonPanelSource srcPanelGuardaCorpo = new RibbonPanelSource()
            {
                Title = abaGuardaCorpo.NomePainel
            };

            RibbonPanel PanelGuardaCorpo = new RibbonPanel()
            {
                Source = srcPanelGuardaCorpo
            };


            foreach (RibbonButton button in abaGuardaCorpo.Comandos)
            {
                srcPanelGuardaCorpo.Items.Add(button);
            }

            Tab.Panels.Add(PanelGuardaCorpo);
            Tab.Panels.Add(Panel);


            Tab.IsActive = true;

        }

        public class SimpleButtonCmdHandler : System.Windows.Input.ICommand
        {

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {

                if (parameter is RibbonButton)
                {
                    RibbonButton button = parameter as RibbonButton;

                    if (button.CommandParameter.ToString() == "geracp")
                    {
                        var coluna = IntegraColuna.SelecionaColuna();
                        if (coluna != null)
                        {
                            WPFColunaPronta window = new WPFColunaPronta(coluna);
                            window.Show();
                        }
                        else
                        {
                            MessageBox.Show("Favor selecionar a coluna!", "Atenção!");
                        }
                    }

                    if (button.CommandParameter.ToString() == "geragc")
                    {
                        WPFGuardaCorpo window = new WPFGuardaCorpo();
                        window.MaxHeight = 500;
                        window.MaxWidth = 430;
                        window.Show();
                    }

                    if (button.CommandParameter.ToString() == "geraucp")
                    {
                        var coluna = IntegraColuna.SelecionaColuna();

                        if (coluna == null)
                        {
                            MessageBox.Show("Favor selecionar a coluna novamente !", "Atenção!");
                            return;
                        }

                        var layout = IntegraLayout.GetUltimaColuna(coluna.NomeArquivo);
                        if (layout != null)
                        {
                            IntegraColuna.AddColunaLayout(coluna, layout);
                        }
                        else
                        {
                            MessageBox.Show("Não foi possivel identificar ultima coluna gerada!", "Atenção!");
                        }
                    }

                    if (button.CommandParameter.ToString() == "geraicp")
                    {
                        var id = IntegraColuna.GetLayoutIdentificado();
                        if (id > 0)
                        {
                            var coluna = IntegraColuna.SelecionaColuna();

                            if (coluna == null)
                            {
                                MessageBox.Show("Favor selecionar a coluna novamente !", "Atenção!");
                                return;
                            }

                            var layout = IntegraLayout.GetLayout(id, coluna.NomeArquivo);

                            if (layout != null)
                            {
                                IntegraColuna.AddColunaLayout(coluna, layout);
                            }
                            else
                            {
                                MessageBox.Show("Não foi possivel identificar o tipo de coluna informado!", "Atenção!");
                            }
                        }
                    }

                    if (button.CommandParameter.ToString() == "GeraRelatorio")
                    {
                        IntegraColuna.GeraRelatorio();
                    }

                    if (button.CommandParameter.ToString() == "GeraListaCorte")
                    {
                        IntegraColuna.GeraRelatorioExcel(TipoLista.ListaCorteColuna);
                    }

                    if (button.CommandParameter.ToString() == "GeraListaEntrega")
                    {
                       IntegraColuna.GeraRelatorioExcel(TipoLista.ListaEntregaColuna);
                    }

                  
                    if (button.CommandParameter.ToString() == "GeraListaEntregaGC")
                    {
                        IntegraGuardaCorpoVertical.GeraListaEntrega();
                    }

                    if (button.CommandParameter.ToString() == "geralvgc")
                    {
                        IntegraGuardaCorpoVertical.Integra();
                    }
                    
                }
            }

        }

    }

}
