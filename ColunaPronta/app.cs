using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.ServiceModel.Channels;
using System.Windows;

namespace ColunaPronta
{
    public class InitializeComponent : IExtensionApplication
    {

        #region >> Initialization

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }

        #endregion

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
            var layout = IntegraLayout.GetUltimaColuna();
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
            var coluna = IntegraColuna.SelecionaColuna();
            var layout = IntegraLayout.GetLayout(id);

            if (layout != null)
            {
                IntegraColuna.AddColunaLayout(coluna, layout);
            }
            else
            {
                MessageBox.Show("Não foi possivel identificar o tipo de coluna informado!", "Atenção!");
            }

        }

        [CommandMethod("GeraRelatorio")]
        public void GeraRelatorio()
        {
            IntegraColuna.GeraRelatorio();
        }

        [CommandMethod("Teste")]
        public void Teste()
        {
            var com = new LayoutCreation.Commands();
            com.CreateLayout();
        }
    }
}
