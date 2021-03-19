using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.EditorInput;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpoVertical
    {
        public static void Integra()
        {
            //List<GuardaCorpoVertical> guardaCorpos
            var document = Application.DocumentManager.MdiActiveDocument;
            var settings = new Settings();
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto onde será gerado o relatório (EndPoint ): ")
            {
                AllowArbitraryInput = false,
                AllowNone = true
            };

            PromptPointResult prPtRes = editor.GetPoint(prPtOpt);
            var ponto = prPtRes.Value;


            var gc = new GuardaCorpoVertical(settings.Altura, settings.ComprimentoPadrao, new Point2d(ponto.X, ponto.Y));
            //foreach (var gc in guardaCorpos)
            //{
            Helpers.AddPolylineHatch(document, gc.Retangulo.Pontos, Layer.Poste, ColorIndex.Branco);

            foreach (var ent in gc.EstruturasVerticais)
            {
                Helpers.AddPolylineHatch(document, ent.Pontos, Layer.Poste,ColorIndex.AzulEscuroPersonalizado);
            }

            foreach (var ent in gc.EstruturasHorizontais)
            {
                Helpers.AddPolylineHatch(document, ent.Pontos, Layer.Poste, ColorIndex.AzulClaro);
            }

            Helpers.AddPolylineHatch(document, gc.PosteReforco.Pontos, Layer.Poste,  ColorIndex.AzulPersonalizado);

            Helpers.AddPolyline(document, gc.Cantoneira.Pontos, Layer.Poste);

        }
    }
}
