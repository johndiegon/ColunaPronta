namespace ColunaPronta.Model
{
    public class EspecificacaoLayer
    {
        public string Objeto { get; set; }
        public string Nome { get; set; }
        public static EspecificacaoLayer FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(';');
            EspecificacaoLayer layer = new EspecificacaoLayer();
            layer.Objeto = values[0];
            layer.Nome = (values[1]).ToUpper();

            return layer;
        }
    }
}
