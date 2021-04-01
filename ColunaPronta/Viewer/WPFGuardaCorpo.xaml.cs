using ColunaPronta.Commands;
using ColunaPronta.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColunaPronta.Viewer
{
    public partial class WPFGuardaCorpo : Window
    {
        Settings settings = new Settings(1);

        public WPFGuardaCorpo()
        {
            InitializeComponent();
            SetTelaInicial();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isValido())
                return;
            SetSettings();
            SaveLayers();

            this.Close();
      
            var bPosteInicial = this.RadioButtonPosteInicioSim.IsChecked == true ? true : false;
            var bPosteFinal = this.RadioButtonPosteFimSim.IsChecked == true ? true : false;
            var posicao = Posicao.VoltadoBaixo;

            var abertura = Abertura.fechado;

            if (this.CheckBoxPosteDireita.IsChecked == true  && this.CheckBoxPosteEsquerda.IsChecked == true ||
                this.CheckBoxPosteDireita.IsChecked == false && this.CheckBoxPosteEsquerda.IsChecked == false
                )
            {
                abertura = this.CheckBoxPosteEsquerda.IsChecked == true ?  Abertura.ambos : Abertura.fechado;
            }
            else
            {
                abertura = this.CheckBoxPosteEsquerda.IsChecked == true ? Abertura.Esquerda : Abertura.Direita;
            }


            if (RadioButtonLadoA.IsChecked == true) { posicao = Posicao.VoltadoBaixo; }
            if (RadioButtonLadoB.IsChecked == true) { posicao = Posicao.VoltadoEsqueda; }
            if (RadioButtonLadoC.IsChecked == true) { posicao = Posicao.VoltadoCima; }
            if (RadioButtonLadoD.IsChecked == true) { posicao = Posicao.VoltadoDireita; }

            IntegraGuardaCorpo.Add(posicao, bPosteInicial, bPosteFinal, abertura);
        }

        private bool isValido()
        {
            if ( comboBox_Cantoneira.SelectedItem == null ||
                 comboBox_CantoneiraL.SelectedItem == null ||
                 comboBox_PosteReforco.SelectedItem == null ||
                 comboBox_tuboExterno.SelectedItem == null ||
                 comboBox_tuboInterno.SelectedItem == null ||
                 comboBox_Cotas.SelectedItem == null 
                )
            {
                this.tabControlPrincipal.SelectedItem = this.tabLayerItem;
                MessageBox.Show("É necessário informar todos os layers!", "Guarda Corpo", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }
            return true;
        }

        private void SaveLayers()
        {
            var listLayers = new List<EspecificacaoLayer.Detalhe>();

            var detalheCantoneira = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_Cantoneira.SelectedValue.ToString(),
                Objeto = Layer.Cantoneira.ToString(),
                Perfil = textBox_Cantoneira.Text
            };
            listLayers.Add(detalheCantoneira);

            var detalheCantoneiraL = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_CantoneiraL.SelectedValue.ToString(),
                Objeto = Layer.CantoneiraL.ToString(),
                Perfil = textBox_CantoneiraL.Text
            };
            listLayers.Add(detalheCantoneiraL);
            var detalhePosteReforco = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_PosteReforco.SelectedValue.ToString(),
                Objeto = Layer.PosteReforco.ToString(),
                Perfil = textBox_PosteReforco.Text
            };
            listLayers.Add(detalhePosteReforco);
            var detalhetuboExterno = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_tuboExterno.SelectedValue.ToString(),
                Objeto = Layer.TuboExterno.ToString(),
                Perfil = textBox_PosteReforco.Text
            };
            listLayers.Add(detalhetuboExterno);

            var detalhetuboInterno = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_tuboInterno.SelectedValue.ToString(),
                Objeto = Layer.TuboInterno.ToString(),
                Perfil = textBox_tuboInterno.Text
            };
            listLayers.Add(detalhetuboInterno);

            var detalheCota = new EspecificacaoLayer.Detalhe()
            {
                Nome = comboBox_Cotas.SelectedValue.ToString(),
                Objeto = Layer.Cotas.ToString(),
                Perfil = ""
            };
            listLayers.Add(detalheCota);

            ArquivoCSV.Registra(listLayers);
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
            textbox_reforcoLargura.Text = settings.PosteReforcoLargura.ToString();          
            textbox_reforcoComprimento.Text = settings.PosteReforcoComprimento.ToString();

            textBox_ComprimentoGCInicio.Text     = settings.TuboExternoComprimentoInicial.ToString() ;
            textBox_ComprimentoTuboInterno.Text  = settings.TuboInternoComprimento.ToString();
            textBox_LarguraTuboInterno.Text      = settings.TuboInternoLargura.ToString();
            textBox_LarguraTuboExterno.Text      = settings.TuboExternoLargura.ToString();
            textBox_DistanciaTuboExterno.Text    = settings.TuboExternoComprimento.ToString();

            SetComboBox();
        
        }

        private void SetSettings()
        {
            settings.Altura = Convert.ToDouble(textBox_altura.Text);
            settings.Largura = Convert.ToDouble(textbox_largura.Text);
            settings.ComprimentoPadrao = Convert.ToDouble(textbox_comprimentoPadrao.Text);
            settings.ComprimentoMaxima = Convert.ToDouble(TextBox_comprimentoMaximo.Text);
            settings.ComprimentoMinimoReforco = Convert.ToDouble(textbox_comprimentoMinimoReforco.Text);
            settings.PosteComprimento = Convert.ToDouble(textbox_comprimentoPoste.Text);
            settings.PosteLargura = Convert.ToDouble(textbox_larguraPoste.Text);
            settings.CantoneiraLargura = Convert.ToDouble(textbox_cantoneiraLargura.Text);
            settings.CantoneiraComprimento = Convert.ToDouble(textbox_cantoneiraComprimento.Text);
            settings.CantoneiraFolga = Convert.ToDouble(textBox_cantoneiraFolga.Text);
            settings.CantoneiraEspessura = Convert.ToDouble(textBox_cantoneiraEspessura.Text);
            settings.PosteReforcoLargura = Convert.ToDouble(textbox_reforcoLargura.Text);
            settings.PosteReforcoComprimento = Convert.ToDouble(textbox_reforcoComprimento.Text);
            settings.TuboExternoComprimentoInicial = Convert.ToDouble(textBox_ComprimentoGCInicio.Text);
            settings.TuboInternoComprimento = Convert.ToDouble(textBox_ComprimentoTuboInterno.Text);
            settings.TuboInternoLargura = Convert.ToDouble(textBox_LarguraTuboInterno.Text);
            settings.TuboExternoLargura = Convert.ToDouble(textBox_LarguraTuboExterno.Text);
            settings.TuboExternoComprimento = Convert.ToDouble(textBox_DistanciaTuboExterno.Text);

            settings.Save();

        }

        private void SetComboBox()
        {
            var layers = Helper.Helpers.GetLayers();
            var especificaolayer = new EspecificacaoLayer();
            
            comboBox_Cantoneira.ItemsSource = layers;
            var detalhe = especificaolayer.GetDetalheLayer(Layer.Cantoneira);
            comboBox_Cantoneira.SelectedValue = detalhe == null ? "" : detalhe.Nome;
            textBox_Cantoneira.Text           = detalhe == null ? "" : detalhe.Perfil;

            comboBox_CantoneiraL.ItemsSource = layers;
            detalhe = especificaolayer.GetDetalheLayer(Layer.CantoneiraL);
            comboBox_CantoneiraL.SelectedValue = detalhe == null ? "" : detalhe.Nome;
            textBox_CantoneiraL.Text           = detalhe == null ? "" : detalhe.Perfil;

            comboBox_PosteReforco.ItemsSource = layers;
            detalhe = especificaolayer.GetDetalheLayer(Layer.PosteReforco);
            comboBox_PosteReforco.SelectedValue = detalhe == null ? "" : detalhe.Nome;
            textBox_PosteReforco.Text           = detalhe == null ? "" : detalhe.Perfil;

            comboBox_tuboExterno.ItemsSource = layers;
            detalhe = especificaolayer.GetDetalheLayer(Layer.TuboExterno);
            comboBox_tuboExterno.SelectedValue = detalhe == null ? "" : detalhe.Nome;
            textBox_tuboExterno.Text           = detalhe == null ? "" : detalhe.Perfil;

            comboBox_tuboInterno.ItemsSource = layers;
            detalhe = especificaolayer.GetDetalheLayer(Layer.TuboInterno);
            comboBox_tuboInterno.SelectedValue = detalhe == null ? "" : detalhe.Nome;
            textBox_tuboInterno.Text           = detalhe == null ? "" : detalhe.Perfil;

            comboBox_Cotas.ItemsSource = layers;
            detalhe = especificaolayer.GetDetalheLayer(Layer.Cotas);
            comboBox_Cotas.SelectedValue = detalhe == null ? "" : detalhe.Nome;

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "^[^0-9.?!]+$");//"[^0-9]+[.]");
        }

    }
}
