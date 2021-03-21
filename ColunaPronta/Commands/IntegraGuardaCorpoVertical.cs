using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Linq;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpoVertical
    {
        public static void Integra(List<double> guardaCorpos)
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto onde será gerado o relatório (EndPoint ): ")
            {
                AllowArbitraryInput = false,
                AllowNone = true
            };

            PromptPointResult prPtRes = editor.GetPoint(prPtOpt);
            var ponto = prPtRes.Value;
            var settings = new Settings(true);

            double X = ponto.X, Y = ponto.Y, distanciaEntreGuardaCorpos = 300 / 1000f;
            var listGC = guardaCorpos.GroupBy(s => s).Select(group => new { Comprimento = group.Key, Quantidade = group.Count() });


            foreach (var gc in listGC)
            {
                var guardaCorpo = new GuardaCorpoVertical(settings.Altura, gc.Comprimento, new Point2d(X, Y));
                GeraGuardaCorpo(guardaCorpo);
                X = X + gc.Comprimento + distanciaEntreGuardaCorpos;
            }
        }

        private static void GeraGuardaCorpo(GuardaCorpoVertical guardaCorpo)
        {
            var document = Application.DocumentManager.MdiActiveDocument;

            foreach (var ent in guardaCorpo.EstruturasVerticais)
            {
                Helpers.AddPolylineHatch(document, ent.Pontos, Layer.Poste, ColorIndex.AzulClaro);
            }

            foreach (var ent in guardaCorpo.EstruturasHorizontais)
            {
                Helpers.AddPolyline(document, ent.Pontos, Layer.Poste, ColorIndex.AzulEscuroPersonalizado);
            }

            foreach (var ent in guardaCorpo.Dimensions)
            {
                Helpers.AddDimension(document, ent.PontoLinha1, ent.PontoLinha2, ent.PontoDimension);
            }

            if (guardaCorpo.PosteReforco != null)
            {
                Helpers.AddPolylineHatch(document, guardaCorpo.PosteReforco.Pontos, Layer.Poste, ColorIndex.AzulPersonalizado);

                Helpers.AddPolyline(document, guardaCorpo.Cantoneira.Pontos, Layer.Poste, ColorIndex.Branco);

                Helpers.AddPolylineHatch(document, guardaCorpo.CoberturaReforco.Pontos, Layer.Poste, ColorIndex.Branco);
            }

            foreach (var pontos in guardaCorpo.Linhas)
            {
                Helpers.AddLinha(document, pontos, Layer.Cantoneira);
            }
        }

    }
}
