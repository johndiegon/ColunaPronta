using Autodesk.AutoCAD.Runtime;
using ColunaPronta.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            ComandoAutocad comando = new ComandoAutocad();
            var coluna = comando.SelecionaColuna();
            WPFColunaPronta window = new WPFColunaPronta(coluna);
            window.Show();
        }

        //[CommandMethod("Teste")]
        //public void teste()
        //{
        //    ComandoAutocad comando = new ComandoAutocad();
        //    comando.TesteInserBlck();
        //}

    }
}
