﻿using ColunaPronta.Commands;
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
            SetTelaInicial();

            this._coluna = coluna;
        }

        //private void InicializaComboBoxTpColuna()
        //{
        //    var tiposColuna = Enum.GetValues(typeof(TipoColuna)).Cast<TipoColuna>();
        //    combobox_TpColuna.ItemsSource = tiposColuna;
        //}
       
        private void Btn_gerarcoluna_Click(object sender, RoutedEventArgs e)
        {

            //Diametro do Chumbador
            if (diametroChumbadorObrigatorio == true && textBox_diametro.Text.ToString() == "")
            {
                MessageBox.Show("Preenchimento do Diâmetro do chumbador é obrigatório quando há sapata para gerar.", "Atenção!");
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
            if (textBox_qtdeParafuso.Text.ToString() == "")
            {
                MessageBox.Show("Preenchimento da quantidade de parafuso é obrigatório.", "Atenção!");
                return;
            }
            else
            {
                this._coluna.QuantidadeParafuso = Convert.ToDouble(textBox_qtdeParafuso.Text.ToString());
            }

            //Tipo de coluna.
            if (combobox_TpColuna.SelectedValue.ToString() == "")
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
            this._coluna.SapataA = (bool)this.checkBox_sptA.IsChecked;
            this._coluna.SapataB = (bool)this.checkBox_sptB.IsChecked;
            this._coluna.SapataC = (bool)this.checkBox_sptC.IsChecked;
            this._coluna.SapataD = (bool)this.checkBox_sptD.IsChecked;
            //this._coluna.SetTipoColuna((TipoColuna)combobox_TpColuna.SelectedItem);

            this.Close();
            IntegraColuna.AddColuna(_coluna);
            ArquivoCSV.Registra(_coluna);
            //IntegraColuna.AddTitulo(_coluna.PointA, _coluna.GetTipoColuna());

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

     
        private void menu_parafuso_MouseMove(object sender, MouseEventArgs e)
        {
            border_parafuso.Visibility = Visibility.Visible;
            border_passante.Visibility = Visibility.Hidden;
        }
        private void menu_passante_MouseMove(object sender, MouseEventArgs e)
        {
            border_parafuso.Visibility = Visibility.Hidden;
            border_passante.Visibility = Visibility.Visible;
        }

        private void border_parafuso_MouseEnter(object sender, MouseEventArgs e)
        {
            StackPanel_TelaInicial.Visibility = Visibility.Hidden;
            Grid_Inputs.Visibility = Visibility.Visible;

            if (border_parafuso.Visibility.Equals(Visibility.Visible))
            {
                StackPanel_SelecionaColunaNormal.Visibility = Visibility.Visible;
                StackPanel_SelecionaColunapassante.Visibility = Visibility.Hidden;
            }
            else
            {
                StackPanel_SelecionaColunaNormal.Visibility = Visibility.Hidden;
                StackPanel_SelecionaColunapassante.Visibility = Visibility.Visible;
            }

        }

        private void SetTelaInicial()
        {
            this.label_metragem.Content = string.Concat("Coluna ", _coluna.Comprimento.ToString("N2"), " x ", _coluna.Largura.ToString("N2"), "MM");

            StackPanel_TelaInicial.Visibility = Visibility.Visible;
            StackPanel_SelecionaColunaNormal.Visibility = Visibility.Hidden;
            StackPanel_SelecionaColunapassante.Visibility = Visibility.Hidden;
            Grid_Inputs.Visibility = Visibility.Hidden;
            
            SetVisualizacao(_coluna.Posicao);
        }

        private void SetVisualizacao(Posicao posicao)
        {

            this.checkBox_sptA.Margin = posicao == Posicao.Horizontal ? new Thickness(136, 79.8, 0, 0) : new Thickness(369, 157, 0, 0);
            this.checkBox_sptB.Margin = posicao == Posicao.Horizontal ? new Thickness(377, 170.2, 0, 0) : new Thickness(174, 409, 0, 0);
            this.checkBox_sptC.Margin = posicao == Posicao.Horizontal ? new Thickness(136, 369.8, 0, 0) : new Thickness(82, 167, 0, 0);
            this.checkBox_sptD.Margin = posicao == Posicao.Horizontal ? new Thickness(43, 170.8, 0, 0) : new Thickness(174, 81, 0, 0);
            this.checkBox_prfsA.Margin = posicao == Posicao.Horizontal ? new Thickness(164, 162.2, 0, 0) : new Thickness(279, 197, 0, 0);
            this.checkBox_prfsB.Margin = posicao == Posicao.Horizontal ? new Thickness(280, 159.2, 0, 0) : new Thickness(282, 299, 0, 0);
            this.checkBox_prfsC.Margin = posicao == Posicao.Horizontal ? new Thickness(293, 192.2, 0, 0) : new Thickness(272, 334, 0, 0);
            this.checkBox_prfsD.Margin = posicao == Posicao.Horizontal ? new Thickness(291, 270.2, 0, 0) : new Thickness(182, 337, 0, 0);
            this.checkBox_prfsE.Margin = posicao == Posicao.Horizontal ? new Thickness(280, 296.2, 0, 0) : new Thickness(167, 301, 0, 0);
            this.checkBox_prfsF.Margin = posicao == Posicao.Horizontal ? new Thickness(168, 295.2, 0, 0) : new Thickness(164, 188, 0, 0);
            this.checkBox_prfsG.Margin = posicao == Posicao.Horizontal ? new Thickness(133, 271.2, 0, 0) : new Thickness(186, 161, 0, 0);
            this.checkBox_prfsH.Margin = posicao == Posicao.Horizontal ? new Thickness(134, 190.2, 0, 0) : new Thickness(267, 159, 0, 0);
            
            this.image_colunaNormal_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_colunaNormal_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passante_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passante_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passanteb_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passanteb_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passantec_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passantec_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

        }

        private void button_voltar_Click(object sender, RoutedEventArgs e)
        {
            SetTelaInicial();
        }
    }
}
