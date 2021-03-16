using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpo 
    {
        #region >> Comandos 
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

        public static void GeraListaCorte()
        {
            var objetos = Helpers.GetObjetos();
            GeraArquivoListaCorte(objetos);

        }

        #endregion

        #region >> Métodos
        private static void Integra(GuardaCorpo guardaCorpo)
        {
            var document = Application.DocumentManager.MdiActiveDocument;
            
            foreach (Poste poste in guardaCorpo.Postes)
            {
                Helpers.AddPolyline(document, poste.PosteRetangulo.Pontos, Layer.Poste);
                
                foreach(var cantoneira in poste.Cantoneiras)
                {
                    Helpers.AddPolyline(document, cantoneira.Retangulo.Pontos, Layer.Cantoneira);
                    if (cantoneira.Linha.Count == 2)
                    {
                        Point3d pnt1 = new Point3d(cantoneira.Linha[0].X, cantoneira.Linha[0].Y, 0);
                        Point3d pnt2 = new Point3d(cantoneira.Linha[1].X, cantoneira.Linha[1].Y, 0);

                        Helpers.AddLinha(document, pnt1, pnt2, Layer.Cantoneira);
                    }

                    if(cantoneira.Parafusos != null)
                    {
                        foreach (var parafuso in cantoneira.Parafusos)
                        {
                            Helpers.AddCircle(document, parafuso.Point, parafuso.Raio, Layer.Cantoneira);
                        }
                    }
                }
            }

            foreach (GuardaCorpoFilho gc in guardaCorpo.GuardaCorpos)
            {
                
                Helpers.AddPolyline(document, gc.retangulo.Pontos, Layer.Tubo);

                if (gc.PosteReforco != null)
                {
                    foreach (var poste in gc.PosteReforco.Poste)
                    {
                        Helpers.AddPolyline(document, poste.Pontos, Layer.PosteReforco);
                    }

                    foreach (CantoneiraGuardaCorpo cantoneira in gc.PosteReforco.Cantoneiras)
                    {
                        IntegraGuardaCorpoFilho(document, cantoneira);
                    }
                }

                foreach (CantoneiraGuardaCorpo cantoneira in gc.Cantoneiras)
                {
                    IntegraGuardaCorpoFilho(document, cantoneira);
                }
            };
        }

        private static void IntegraGuardaCorpoFilho( Document document , CantoneiraGuardaCorpo cantoneira)
        {
            Helpers.AddPolyline(document, cantoneira.Retangulo.Pontos, Layer.Cantoneira);

            if (cantoneira.Linha.Count == 2)
            {
                Point3d pnt1 = new Point3d(cantoneira.Linha[0].X, cantoneira.Linha[0].Y, 0);
                Point3d pnt2 = new Point3d(cantoneira.Linha[1].X, cantoneira.Linha[1].Y, 0);

                Helpers.AddLinha(document, pnt1, pnt2, Layer.Cantoneira);
            }

            if (cantoneira.Parafusos != null)
            {
                foreach (var parafuso in cantoneira.Parafusos)
                {
                    Helpers.AddCircle(document, parafuso.Point, parafuso.Raio, Layer.Cantoneira);
                }
            }
            if (cantoneira.PontosL != null)
            {
                Helpers.AddPolyline(document, cantoneira.PontosL, Layer.CantoneiraL);
            }
        }

        private static void GeraArquivoListaCorte(ObjetosSelecionados objetos )
        {
            var arquivoExcel = new List<Planilha>();
            var tubo = new List<ItemRelatorio>();
            var cantoneiraL = new List<ItemRelatorio>();
            var poste = new List<ItemRelatorio>();

            if (objetos.Polylines != null)
            {
                var countpolylines = objetos.Polylines.Count;
            }
            if (objetos.Circles != null)
            {
                var countcircles = objetos.Circles.Count;
            }
        }
        #endregion
    }
}
