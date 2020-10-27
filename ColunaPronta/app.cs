using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
using ColunaPronta.Model;

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
            WPFColunaPronta window = new WPFColunaPronta(coluna);
            window.Show();
        }

        [CommandMethod("GeraRelatorio")]
        public void GeraRelatorio()
        {
            IntegraColuna.GeraRelatorio();
        }

    }
}
