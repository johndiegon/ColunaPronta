using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using ColunaPronta.Viewer;
using System.ServiceModel.Channels;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ColunaPronta
{
    public class InitializeComponent : IExtensionApplication
    {

        #region >> Initialization

        public void Initialize()
        {
            AbreAbaFerramenta();
        }

        public void Terminate()
        {
        }

        #endregion

        #region >> Integra Coluna
        [CommandMethod("geracp")]
        public void GeraColuna()
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

        [CommandMethod("geraucp")]
        public void GeraUltimaColunaPronta()
        {
            var coluna = IntegraColuna.SelecionaColuna();

            if(coluna == null)
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

        [CommandMethod("geraicp")]
        public void GeraColunaProntaIdentificada()
        {
            var id = IntegraColuna.GetLayoutIdentificado();
            if ( id > 0)
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

        [CommandMethod("GeraRelatorio")]
        public void GeraRelatorio()
        {
            IntegraColuna.GeraRelatorio();
        }

        [CommandMethod("GeraListaCorte")]
        public void GeraListaCorte()
        {
            IntegraColuna.GeraRelatorioExcel(TipoLista.ListaCorteColuna);
        }

        [CommandMethod("GeraListaEntrega")]
        public void GeraListaEntrega()
        {
            IntegraColuna.GeraRelatorioExcel(TipoLista.ListaEntregaColuna);
        }

    
        #endregion

        #region >> Integra Guarda Corpo 

        [CommandMethod("geragc")]
        public void GeraGuardaCorpo()
        {
            WPFGuardaCorpo window = new WPFGuardaCorpo();
            window.MaxHeight = 500;
            window.MaxWidth = 430;
            window.Show();

        }
        [CommandMethod("geralcgc")]
        public void GeraListaCorteGuardaCorpo()
        {
            IntegraGuardaCorpo.GeraListaCorte(TipoLista.ListaCorteGuardaCorpo);

        }

        [CommandMethod("GeraListaEntregaGC")]
        public void GeraListaEntregaGC()
        {
            IntegraGuardaCorpoVertical.GeraListaEntrega();
        }

        [CommandMethod("geralvgc")]
        public void GeraListaVerticalGuardaCorpo()
        {
            IntegraGuardaCorpoVertical.Integra();
        }

        //[CommandMethod("testevertical")]
        //public void TesteVertical()
        //{
        //    var settings = new Settings();

        //    Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

        //    PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto onde será gerado o relatório (EndPoint ): ")
        //    {
        //        AllowArbitraryInput = false,
        //        AllowNone = true
        //    };

        //    PromptPointResult prPtRes = editor.GetPoint(prPtOpt);
        //    var ponto = prPtRes.Value;

        //    var gc = new GuardaCorpoVertical(settings.Altura, settings.ComprimentoPadrao, new Point2d(ponto.X, ponto.Y));
        //    IntegraGuardaCorpoVertical.Integra(gc, new Point2d(ponto.X, ponto.Y));
        //}


        [CommandMethod("testehatch")]
        public void TesteTraceBoundaryAndHatch()
        {
            Helpers.TraceBoundaryAndHatch();
        }

        #endregion
        public void AbreAbaFerramenta()
        {
            AbaFerramenta aba = new AbaFerramenta();
            aba.Add();
        }

    }
}
