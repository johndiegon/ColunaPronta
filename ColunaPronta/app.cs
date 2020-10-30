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

        [CommandMethod("ultimaCP")]
        public void GeraUltimaColunaPronta()
        {
            var coluna = IntegraColuna.SelecionaColuna();
            var layout = IntegraLayout.GetUltimaColuna();
            if(layout != null)
            {
                coluna.iColuna = layout.iColuna;
                coluna.Altura = layout.Altura;
                coluna.DiametroParafuso = layout.DiametroParafuso;
                coluna.DiametroSapata = layout.DiametroSapata;
                coluna.QuantidadeParafuso = layout.QuantidadeParafuso;
                coluna.ParafusoA = layout.ParafusoA;
                coluna.ParafusoB = layout.ParafusoB;
                coluna.ParafusoC = layout.ParafusoC;
                coluna.ParafusoD = layout.ParafusoD;
                coluna.ParafusoE = layout.ParafusoE;
                coluna.ParafusoF = layout.ParafusoF;
                coluna.ParafusoG = layout.ParafusoG;
                coluna.ParafusoH = layout.ParafusoH;
                coluna.SapataA = layout.SapataA;
                coluna.SapataB = layout.SapataB;
                coluna.SapataC = layout.SapataC;
                coluna.SapataD = layout.SapataD;
                coluna.PassanteA = layout.PassanteA;
                coluna.PassanteB = layout.PassanteB;
                coluna.PassanteC = layout.PassanteC;
                coluna.PassanteD = layout.PassanteD;
                coluna.eleAmarelo = layout.eleAmarelo;
                coluna.eleVermelho = layout.eleVermelho;
                coluna.eleAzul = layout.eleAzul;
                coluna.eleCinza = layout.eleCinza;

                IntegraColuna.AddColuna(coluna, true);
            }
            else
            {
                MessageBox.Show("Não foi possivel identificar ultima coluna gerada!", "Atenção!");
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
