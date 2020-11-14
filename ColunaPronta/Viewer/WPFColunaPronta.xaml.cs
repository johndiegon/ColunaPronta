using ColunaPronta.Commands;
using ColunaPronta.Helper;
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
        public bool bColunaNormal;
        public WPFColunaPronta(Coluna coluna)
        {
            InitializeComponent();
          
            this._coluna = coluna;
            
            SetTelaInicial();
            SetVisualizacao(coluna.Posicao);
        }

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

            this._coluna.ParafusoA   = (bool)this.checkBox_prfsA.IsChecked;
            _coluna.ParafusoB   = (bool)this.checkBox_prfsB.IsChecked;
            _coluna.ParafusoC   = (bool)this.checkBox_prfsC.IsChecked;
            _coluna.ParafusoD   = (bool)this.checkBox_prfsD.IsChecked;
            _coluna.ParafusoE   = (bool)this.checkBox_prfsE.IsChecked;
            _coluna.ParafusoF   = (bool)this.checkBox_prfsF.IsChecked;
            _coluna.ParafusoG   = (bool)this.checkBox_prfsG.IsChecked;
            _coluna.ParafusoH   = (bool)this.checkBox_prfsH.IsChecked;
            _coluna.SapataA     = bColunaNormal == true ? (bool)this.checkBox_sptA.IsChecked : (bool)this.checkBox_passante_sptA.IsChecked;
            _coluna.SapataB     = bColunaNormal == true ? (bool)this.checkBox_sptB.IsChecked : (bool)this.checkBox_passante_sptB.IsChecked;
            _coluna.SapataC     = bColunaNormal == true ? (bool)this.checkBox_sptC.IsChecked : (bool)this.checkBox_passante_sptC.IsChecked;
            _coluna.SapataD     = bColunaNormal == true ? (bool)this.checkBox_sptD.IsChecked : (bool)this.checkBox_passante_sptD.IsChecked;
            _coluna.PassanteA   = (bool)this.checkBox_passanteA.IsChecked;
            _coluna.PassanteB   = (bool)this.checkBox_passanteB.IsChecked;
            _coluna.PassanteC   = (bool)this.checkBox_passanteC.IsChecked;
            _coluna.PassanteD   = (bool)this.checkBox_passanteD.IsChecked;
            _coluna.eleAmarelo  = bColunaNormal == true ? false : (bool)this.checkBox_lAM.IsChecked;
            _coluna.eleAzul     = bColunaNormal == true ? false : (bool)this.checkBox_lA.IsChecked;
            _coluna.eleCinza    = bColunaNormal == true ? false : (bool)this.checkBox_lC.IsChecked;
            _coluna.eleVermelho = bColunaNormal == true ? false : (bool)this.checkBox_lV.IsChecked;
            
            _coluna.SetIdColuna();
            IntegraColuna.AddColuna(_coluna, true);
            this.Close();
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
            if ((bool)this.checkBox_passante_sptA.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_passante_sptB.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_passante_sptC.IsChecked) { diametroChumbadorObrigatorio = true; };
            if ((bool)this.checkBox_passante_sptD.IsChecked) { diametroChumbadorObrigatorio = true; };
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
                this.textBlock_qtParafuso.Visibility = Visibility.Visible;
                this.textBox_qtdeParafuso.Visibility = Visibility.Visible;

                this.textBlock_parafuso.Visibility = Visibility.Visible;
                this.textBox_parafuso.Visibility = Visibility.Visible;

                bColunaNormal = true;
            }
            else
            {
                StackPanel_SelecionaColunaNormal.Visibility = Visibility.Hidden;
                StackPanel_SelecionaColunapassante.Visibility = Visibility.Visible;
                this.textBlock_qtParafuso.Visibility = Visibility.Hidden;
                this.textBox_qtdeParafuso.Visibility = Visibility.Hidden;
                this.textBox_qtdeParafuso.Text = "0";

                this.textBlock_parafuso.Visibility = Visibility.Hidden;
                this.textBox_parafuso.Visibility = Visibility.Hidden;
                this.textBox_parafuso.Text = "0";

                bColunaNormal = false;
            }

        }
     
        private void SetTelaInicial()
        {
          
            StackPanel_TelaInicial.Visibility = Visibility.Visible;
            StackPanel_SelecionaColunaNormal.Visibility = Visibility.Hidden;
            StackPanel_SelecionaColunapassante.Visibility = Visibility.Hidden;
            Grid_Inputs.Visibility = Visibility.Hidden;

            var ultimaColuna = IntegraLayout.GetUltimaColuna(_coluna.NomeArquivo);
            if (ultimaColuna != null)
            {
                this.textBox_parafuso.Text = ultimaColuna.DiametroParafuso.ToString();
                this.textBox_diametro.Text = ultimaColuna.DiametroSapata.ToString();
                this.textBox_qtdeParafuso.Text = ultimaColuna.QuantidadeParafuso.ToString();
                this.textBox_altura.Text = ultimaColuna.Altura.ToString();
            }
        }

        private void SetVisualizacao(Posicao posicao)
        {
            this.label_metragem.Content = string.Concat("Coluna ", _coluna.Comprimento.ToString("N2"), " x ", _coluna.Largura.ToString("N2"), "MM");

            #region >> Posição das Imagens 

            this.image_colunaNormal_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_colunaNormal_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passante_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passante_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passanteb_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passanteb_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            this.image_passantec_horizontal.Visibility = posicao == Posicao.Horizontal ? Visibility.Visible : Visibility.Hidden;
            this.image_passantec_vertical.Visibility = posicao == Posicao.Horizontal ? Visibility.Hidden : Visibility.Visible;

            #endregion

            #region >> StackPanel Coluna Normal
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

            #endregion

            #region >> StackPanel Coluna Passante 

            #region >>Aba Sapata
            this.checkBox_passante_sptA.Margin = posicao == Posicao.Horizontal ? new Thickness(152, 82, 0, 0)  : new Thickness(306, 175, 0, 0); 
            this.checkBox_passante_sptB.Margin = posicao == Posicao.Horizontal ? new Thickness(393, 171, 0, 0) : new Thickness(115, 403, 0, 0);
            this.checkBox_passante_sptC.Margin = posicao == Posicao.Horizontal ? new Thickness(160, 368, 0, 0) : new Thickness(28, 183, 0, 0); 
            this.checkBox_passante_sptD.Margin = posicao == Posicao.Horizontal ? new Thickness(60, 175, 0, 0) : new Thickness(117, 78, 0, 0);
            #endregion

            #region >> Aba Passante 
            this.checkBox_passanteA.Margin = posicao == Posicao.Horizontal ? new Thickness(186,93, 0,0)  : new Thickness(338,206,0,0);
            this.checkBox_passanteB.Margin = posicao == Posicao.Horizontal ? new Thickness(402,209,0,0)  : new Thickness(218,418,0,0);
            this.checkBox_passanteC.Margin = posicao == Posicao.Horizontal ? new Thickness(184,380,0,0)  : new Thickness(32,206, 0,0);
            this.checkBox_passanteD.Margin = posicao == Posicao.Horizontal ? new Thickness(36, 210,0,0)  : new Thickness(216, 82, 0, 0);
            #endregion

            #region >> Aba L
            this.checkBox_lV.Margin = posicao == Posicao.Horizontal ?  new Thickness(117,110,0,0)    : new Thickness(23,194,0,0    );
            this.checkBox_lC.Margin  = posicao == Posicao.Horizontal ? new Thickness(52,264,0,0 )    : new Thickness(113,114,0,0   );
            this.checkBox_lA.Margin = posicao == Posicao.Horizontal ? new Thickness(289, 112, 0, 0) : new Thickness(26, 306, 0, 0); 
            this.checkBox_lAM.Margin = posicao == Posicao.Horizontal ? new Thickness(44, 180, 0, 0) : new Thickness(246, 110, 0, 0);
            #endregion

            #endregion
        }

        private void button_voltar_Click(object sender, RoutedEventArgs e)
        {
            SetTelaInicial();
        }
    }
}
