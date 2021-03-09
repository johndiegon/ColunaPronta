using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using ColunaPronta.Viewer;
using System.ServiceModel.Channels;
using System.Windows;

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
            IntegraColuna.GeraRelatorioExcel(TipoLista.ListaCorte);
        }

        [CommandMethod("GeraListaEntrega")]
        public void GeraListaEntrega()
        {
            IntegraColuna.GeraRelatorioExcel(TipoLista.ListaEntrega);
        }
        #endregion

        #region >> Integra Guarda Corpo 

        [CommandMethod("geragc")]
        public void GeraGuardaCorpo()
        {
            WPFGuardaCorpo window = new WPFGuardaCorpo();
            window.MaxHeight = 430;
            window.MaxWidth = 430;
            window.Show();

        }

        #endregion
        public void AbreAbaFerramenta()
        {
            AbaFerramenta aba = new AbaFerramenta();
            aba.Add();
        }

    }
}
