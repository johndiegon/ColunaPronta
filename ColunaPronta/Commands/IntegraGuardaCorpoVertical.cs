using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpoVertical
    {
        public static void Integra(List<GuardaCorpoVertical> guardaCorpos)
        {
      
        }

        public static void Integra(GuardaCorpoVertical gc, Point2d ponto)
        {
            var document = Application.DocumentManager.MdiActiveDocument;

           
            double X = ponto.X, Y = ponto.Y, distanciaEntreGuardaCorpos = 300 / 1000f;

            //var gc = new GuardaCorpoVertical(settings.Altura, settings.ComprimentoPadrao, new Point2d(ponto.X, ponto.Y));

            //foreach (var gc in guardaCorpos)
            //{

                gc.SetGuardaCorpo(new Point2d(X, Y));

                Helpers.AddPolylineHatch(document, gc.Retangulo.Pontos, Layer.Poste, ColorIndex.Branco);

                foreach (var ent in gc.EstruturasVerticais)
                {
                    Helpers.AddPolylineHatch(document, ent.Pontos, Layer.Poste, ColorIndex.AzulClaro);
                }

                foreach (var ent in gc.EstruturasHorizontais)
                {
                    Helpers.AddPolyline(document, ent.Pontos, Layer.Poste, ColorIndex.AzulEscuroPersonalizado);
                }

                foreach (var ent in gc.Dimensions)
                {
                    Helpers.AddDimension(document, ent.PontoLinha1, ent.PontoLinha2, ent.PontoDimension);
                }

                Helpers.AddPolylineHatch(document, gc.PosteReforco.Pontos, Layer.Poste, ColorIndex.AzulPersonalizado);

                Helpers.AddPolyline(document, gc.Cantoneira.Pontos, Layer.Poste, ColorIndex.Branco);

                foreach (var pontos in gc.Linhas)
                {
                    Helpers.AddLinha(document, pontos, Layer.Cantoneira);
                }

                X = X + gc.Comprimento + distanciaEntreGuardaCorpos;
            //}
        }
    }
}
