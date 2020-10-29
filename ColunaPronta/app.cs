using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
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

        [CommandMethod("GeraColuna")]
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
