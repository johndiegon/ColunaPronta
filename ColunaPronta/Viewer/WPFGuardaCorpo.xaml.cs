using ColunaPronta.Commands;
using ColunaPronta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColunaPronta.Viewer
{
    /// <summary>
    /// Interaction logic for WPFGuardaCorpo.xaml
    /// </summary>
    public partial class WPFGuardaCorpo : Window
    {
        Settings settings = new Settings();

        public WPFGuardaCorpo()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            settings.ToCSV();
            var bPosteInicial = this.RadioButtonPosteInicioSim.IsChecked == true ? true : false;
            var bPosteFinal = this.RadioButtonPosteFimSim.IsChecked == true ? true : false;
            var posicao = Posicao.VoltadoBaixo;

            if (RadioButtonLadoA.IsChecked == true) { posicao = Posicao.VoltadoBaixo; }
            if (RadioButtonLadoB.IsChecked == true) { posicao = Posicao.VoltadoEsqueda; }
            if (RadioButtonLadoC.IsChecked == true) { posicao = Posicao.VoltadoCima; }
            if (RadioButtonLadoD.IsChecked == true) { posicao = Posicao.VoltadoDireita; }

            IntegraGuardaCorpo.Add(posicao, bPosteInicial, bPosteFinal);
        }

        private void SetTelaInicial()
        {
            textBox_altura.Text = settings.Altura.ToString() ;
            textbox_largura.Text = settings.Largura.ToString();                 
            textbox_comprimentoPadrao.Text = settings.ComprimentoPadrao.ToString();       
            TextBox_comprimentoMaximo.Text = settings.ComprimentoMaxima.ToString();
            textbox_comprimentoMinimoReforco.Text = settings.ComprimentoMinimoReforco.ToString();
            textbox_comprimentoPoste.Text = settings.PosteComprimento.ToString();        
            textbox_larguraPoste.Text = settings.PosteLargura.ToString();            
            textbox_cantoneiraLargura.Text = settings.CantoneiraLargura.ToString();       
            textbox_cantoneiraComprimento.Text = settings.CantoneiraComprimento.ToString();   
            textBox_cantoneiraFolga.Text = settings.CantoneiraFolga.ToString();         
            textBox_cantoneiraEspessura.Text = settings.CantoneiraEspessura.ToString();     
            textbox_reforcoLargura.Text = settings.PosteReforcaoLargura.ToString();          
            textbox_reforcoComprimento.Text = settings.PosteReforcoComprimento.ToString();      
        }

    }
}
