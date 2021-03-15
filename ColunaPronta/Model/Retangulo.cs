using Autodesk.AutoCAD.Geometry;

namespace ColunaPronta.Model
{
    public class Retangulo
    {
        public double Largura { get; set; }
        public double Comprimento { get; set; }
        public double Area { get { return Largura * Comprimento; } }
        public Point2d PontoInicial { get; set; }
        public Posicao Posicao { get; set; }
        public Point2dCollection Pontos { get; set; }
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

        }
      
    }
}
