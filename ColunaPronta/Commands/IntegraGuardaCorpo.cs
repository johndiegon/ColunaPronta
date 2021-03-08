using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpo 
    {
        
        public static void Add(Posicao posicao, bool bPosteInicial, bool bPosteFinal)
        {
       
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto inicial: ")
            {
                AllowArbitraryInput = false,
                AllowNone = true
            };

            PromptPointResult prPtRes1 = editor.GetPoint(prPtOpt);

            if (prPtRes1.Status != PromptStatus.OK) 
                return;
            
            prPtOpt.Message = "\nIndique o ponto final: ";

            PromptPointResult prPtRes2 = editor.GetPoint(prPtOpt);
            
            if (prPtRes2.Status != PromptStatus.OK) 
                return;

            var guardaCorpo = new GuardaCorpo(  new Point2d(prPtRes1.Value.X, prPtRes1.Value.Y)
                                              , new Point2d(prPtRes2.Value.X, prPtRes2.Value.Y)
                                              , posicao
                                              , bPosteInicial
                                              , bPosteFinal
                                              );

            Integra(guardaCorpo);
        }

        private static void Integra(GuardaCorpo guardaCorpo)
        {
            var document = Application.DocumentManager.MdiActiveDocument;

            foreach (Retangulo poste in guardaCorpo.Postes)
            {
                Helpers.AddPolyline(document, poste.Pontos, ColorIndex.padrao);
            }

            foreach (Retangulo poste in guardaCorpo.Postes)
            {
                Helpers.AddPolyline(document, poste.Pontos, ColorIndex.padrao);
            }

            foreach (GuardaCorpoFilho gc in guardaCorpo.GuardaCorpos)
            {
                
                Helpers.AddPolyline(document, gc.retangulo.Pontos, ColorIndex.padrao);

                if (gc.PosteReforco != null)
                {
                    Helpers.AddPolyline(document, gc.PosteReforco.Pontos, ColorIndex.padrao);
                }

                foreach (CantoneiraGuardaCorpo cantoneira in gc.Cantoneiras)
                {
                    Helpers.AddPolyline(document, cantoneira.Retangulo.Pontos, ColorIndex.padrao);

                    Helpers.AddPolyline(document, cantoneira.PontosL, ColorIndex.padrao);

                    if (cantoneira.Linha.Count == 2)
                    {
                        Point3d pnt1 = new Point3d(cantoneira.Linha[0].X, cantoneira.Linha[0].Y, 0);
                        Point3d pnt2 = new Point3d(cantoneira.Linha[1].X, cantoneira.Linha[1].Y, 0);

                        Helpers.AddLinha(document, pnt1, pnt2, false, ColorIndex.padrao);
                    }
                }
            };
        }
    }
}
