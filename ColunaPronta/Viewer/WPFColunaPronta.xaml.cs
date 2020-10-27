using ColunaPronta.Commands;
using ColunaPronta.Model;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ColunaPronta
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WPFColunaPronta : Window
    {
        public Coluna _coluna = new Coluna();
        public bool diametroChumbadorObrigatorio = false;
        public WPFColunaPronta(Coluna coluna)
        {
            InitializeComponent();
            this._coluna = coluna;
            this.label_metragem.Content = string.Concat("Coluna ", coluna.Comprimento.ToString("N2"), " x ", coluna.Largura.ToString("N2"), "MM");
            InicializaComboBoxTpColuna();
        }

        private void InicializaComboBoxTpColuna()
        {
            var tiposColuna = Enum.GetValues(typeof(TipoColuna)).Cast<TipoColuna>();
            combobox_TpColuna.ItemsSource = tiposColuna.ToList();
        }

        private void Btn_gerarcoluna_Click(object sender, RoutedEventArgs e)
        {
           
            //Diametro do Chumbador
            if (diametroChumbadorObrigatorio == true && textBox_diametro.Text.ToString() == "")
            {
                MessageBox.Show( "Preenchimento do Diâmetro do chumbador é obrigatório quando há sapata para gerar.", "Atenção!");
                return;
            }
            
            if (diametroChumbadorObrigatorio)
            {
                this._coluna.DiametroSapata = Convert.ToDouble(textBox_diametro.Text.ToString());
            }
            
            //Altura
            if (textBox_altura.Text.ToString() == "")
            {
                MessageBox.Show("Preenchimento da altura da coluna é obrigatório.", "Atenção!");
                return;
            }
            else
            {
                this._coluna.Altura = Convert.ToDouble(textBox_altura.Text.ToString());
            }

            //Quantidade de parafuso
            if (textBox_qtParafuso.Text.ToString() == "")
            {
                MessageBox.Show("Preenchimento da quantidade de parafuso é obrigatório.", "Atenção!");
                return;
            }
            else
            {
                this._coluna.QuantidadeParafuso = Convert.ToDouble(textBox_qtParafuso.Text.ToString());
            }

            //Tipo de coluna.
            if (combobox_TpColuna.SelectedValue.ToString()=="")
            {
                MessageBox.Show("Selecione o tipo de coluna.", "Atenção!");
                return;
            }

            //Quantidade de parafuso
            if (textBox_parafuso.Text.ToString() == "")
            {
                MessageBox.Show("Preenchimento do diametro de parafuso é obrigatório.", "Atenção!");
                return;
            }
            else
            {
                this._coluna.DiametroParafuso = Convert.ToDouble(textBox_parafuso.Text.ToString());
            }


            this._coluna.ParafusoA = (bool)this.checkBox_prfsA.IsChecked;
            this._coluna.ParafusoB = (bool)this.checkBox_prfsB.IsChecked;
            this._coluna.ParafusoC = (bool)this.checkBox_prfsC.IsChecked;
            this._coluna.ParafusoD = (bool)this.checkBox_prfsD.IsChecked;
            this._coluna.ParafusoE = (bool)this.checkBox_prfsE.IsChecked;
            this._coluna.ParafusoF = (bool)this.checkBox_prfsF.IsChecked;
            this._coluna.ParafusoG = (bool)this.checkBox_prfsG.IsChecked;
            this._coluna.ParafusoH = (bool)this.checkBox_prfsH.IsChecked;
            this._coluna.SapataA   = (bool)this.checkBox_sptA.IsChecked; 
            this._coluna.SapataB   = (bool)this.checkBox_sptB.IsChecked;
            this._coluna.SapataC   = (bool)this.checkBox_sptC.IsChecked;
            this._coluna.SapataD   = (bool)this.checkBox_sptD.IsChecked;
            this._coluna.SetTipoColuna( (TipoColuna)combobox_TpColuna.SelectedItem );

            this.Close();
            IntegraColuna.AddColuna(_coluna);
            ArquivoCSV.Registra(_coluna);
            IntegraColuna.AddTitulo(_coluna.PointA, _coluna.GetTipoColuna());

        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void checkBox_spt_Checked(object sender, RoutedEventArgs e)
        {
            diametroChumbadorObrigatorio = false;
            if ((bool)this.checkBox_sptA.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_sptB.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_sptC.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_sptD.IsChecked) { diametroChumbadorObrigatorio = true; };
        }


    }
}
