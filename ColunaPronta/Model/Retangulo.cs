using Autodesk.AutoCAD.Geometry;

namespace ColunaPronta.Model
{
    public class Retangulo
    {
        public double Largura { get; set; }
        public double Comprimento { get; set; }
        public double Area { get { return Largura * Comprimento; } }
        public Point2d PontoInicial { get; set; }
        private Posicao Posicao { get; set; }
        public Point2dCollection Pontos { get; set; }
        public Point2d Meio { get; set; }
        public Retangulo(double largura, double comprimento, Point2d pontoInicial, Posicao posicao)
        {
            this.Largura = largura;
            this.Comprimento = comprimento;
            this.PontoInicial = pontoInicial;

            var collection = new Point2dCollection();

            switch (posicao)
            {
                case Posicao.Vertical:
                    collection.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    collection.Add(new Point2d(pontoInicial.X + largura, pontoInicial.Y));
                    collection.Add(new Point2d(pontoInicial.X + largura, pontoInicial.Y - comprimento));
                    collection.Add(new Point2d(pontoInicial.X, pontoInicial.Y - comprimento));
                    collection.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    break;
                case Posicao.Horizontal:
                    collection.Add(new Point2d(pontoInicial.X              , pontoInicial.Y));
                    collection.Add(new Point2d(pontoInicial.X + comprimento, pontoInicial.Y));
                    collection.Add(new Point2d(pontoInicial.X + comprimento, pontoInicial.Y - largura));
                    collection.Add(new Point2d(pontoInicial.X              , pontoInicial.Y - largura));
                    collection.Add(new Point2d(pontoInicial.X, pontoInicial.Y));
                    break;
            }
            
            this.Pontos = collection;

            SetMeio();
        }
        private void SetMeio()
        {
            switch (Posicao)
            {
                case Posicao.Vertical:
                    Meio = new Point2d(PontoInicial.X + (this.Largura /2 ), PontoInicial.Y - (this.Comprimento / 2));
                    break;
                case Posicao.Horizontal:
                    Meio = new Point2d(PontoInicial.X + (this.Comprimento / 2 ), PontoInicial.Y - (this.Largura /2));
                    break;
            }
        }
    }
}
