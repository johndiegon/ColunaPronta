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
        public static void Integra()
        {
            //List<double> guardaCorpos
            var objetos = Helpers.GetObjetos();
            var itemRelatorio = GetGuardaCorpo(objetos);

            var gcNormal = (from item in itemRelatorio
                            where item.ComprimentoTuboExterno == 0
                            group item by new
                            {
                                item.Comprimento,
                                item.Largura,
                                item.Descricao
                            } into c
                            select new ItemRelatorio
                            {
                                Descricao = c.Key.Descricao,
                                Largura = c.Key.Largura,
                                Comprimento = c.Key.Comprimento,
                                QtdeColuna = c.Count()
                            }).ToList();

            var gcRegulavel = (from item in itemRelatorio
                               where item.ComprimentoTuboExterno != 0
                               group item by new
                               {
                                   item.Comprimento,
                                   item.Largura,
                                   item.Descricao,
                                   item.ComprimentoTuboExterno, 
                                   item.Abertura,
                                   item.ComprimentoTuboInterno
                               } into c
                               select new ItemRelatorio
                               {
                                   Descricao = c.Key.Descricao,
                                   Largura = c.Key.Largura,
                                   Comprimento = c.Key.Comprimento,
                                   Abertura = c.Key.Abertura,
                                   ComprimentoTuboExterno = c.Key.ComprimentoTuboExterno,
                                   ComprimentoTuboInterno = c.Key.ComprimentoTuboInterno,
                                   QtdeColuna = c.Count()
                               }).ToList();


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
            
            foreach (var gc in gcNormal)
            {
                double comprimento = ( gc.Comprimento / 1000f )+ ((settings.CantoneiraFolga + settings.CantoneiraEspessura) * 2);
                var guardaCorpo = new GuardaCorpoVertical(settings.Altura, comprimento, new Point2d(X, Y));
                GeraGuardaCorpo(guardaCorpo);

                var descricao = string.Concat("GUARDA CORPO - " + ( comprimento * 1000).ToString("N2") + " X +", (settings.Altura * 1000).ToString("N2"), " - ", gc.QtdeColuna, " UNIDADES");

                var pontoTexto = new Point3d(X, Y - settings.Altura - distanciaEntreGuardaCorpos, 0);

                Helpers.AddTexto(Application.DocumentManager.MdiActiveDocument, pontoTexto, descricao, ColorIndex.Branco);

                X = X + comprimento + distanciaEntreGuardaCorpos;
            }

            foreach (var gc in gcRegulavel)
            {
                double comprimento = (gc.Comprimento / 1000f) + ((settings.CantoneiraFolga + settings.CantoneiraEspessura) * 2);
                var guardaCorpo = new GuardaCorpoVertical(settings.Altura, comprimento, gc.ComprimentoTuboInterno, gc.ComprimentoTuboExterno, new Point2d(X, Y), gc.Abertura);
                GeraGuardaCorpo(guardaCorpo);

                var descricao = string.Concat("GUARDA CORPO - Regulável abertura "+ gc.Abertura.ToString() + (comprimento * 1000).ToString("N2") + " X +", (settings.Altura * 1000).ToString("N2"), " - ", gc.QtdeColuna, " UNIDADES");

                var pontoTexto = new Point3d(X, Y - settings.Altura - distanciaEntreGuardaCorpos, 0);

                Helpers.AddTexto(Application.DocumentManager.MdiActiveDocument, pontoTexto, descricao, ColorIndex.Branco);

                X = X + comprimento + distanciaEntreGuardaCorpos;
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

            if (guardaCorpo.EstruturasTubosInternos != null)
            {
                foreach (var ent in guardaCorpo.EstruturasTubosInternos)
                {
                    Helpers.AddPolylineHatch(document, ent.Pontos, Layer.Poste, ColorIndex.vermelho);
                }

            }

            foreach (var ent in guardaCorpo.Dimensions)
            {
                Helpers.AddDimension(document, ent.PontoLinha1, ent.PontoLinha2, ent.PontoDimension, Layer.Cotas);
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

        private static List<ItemRelatorio> GetGuardaCorpo(ObjetosSelecionados objetos)
        {
            var listaTuboExterno = new List<ItemRelatorio>();
            var listaTuboInterno = new List<ItemRelatorio>();
            var listaGurdaCorpo = new List<ItemRelatorio>();
            var layers = new EspecificacaoLayer();
            var nomeLayer = layers.GetNomeLayer(Layer.TuboExterno);
           
            if (objetos.Polylines != null)
            {
                foreach (var poly in objetos.Polylines)
                {
                    if (nomeLayer == poly.Layer.ToString())
                    {
                        var itemRelatorio = new ItemRelatorio(poly);
                        listaTuboExterno.Add(itemRelatorio);
                    }
                }
            }

            nomeLayer = layers.GetNomeLayer(Layer.TuboInterno);

            if (objetos.Polylines != null)
            {
                foreach (var poly in objetos.Polylines)
                {
                    if (nomeLayer == poly.Layer.ToString())
                    {
                        var itemRelatorio = new ItemRelatorio(poly);
                        listaTuboInterno.Add(itemRelatorio);
                    }
                }
            }

            foreach( var tuboExterno in  listaTuboExterno )
            {
                var abertura = Abertura.fechado;
                foreach ( var tuboInterno in listaTuboInterno)
                {

                    if(  tuboInterno.PontoD.X > tuboExterno.PontoA.X &&
                         tuboInterno.PontoD.X < tuboExterno.PontoB.X &&
                         tuboInterno.PontoA.Y < tuboExterno.PontoA.Y &&
                         tuboInterno.PontoA.Y > tuboExterno.PontoC.Y
                        )
                    {
                        abertura = Abertura.Esquerda;
                    }

                    if ( tuboInterno.PontoB.X > tuboExterno.PontoA.X &&
                         tuboInterno.PontoB.X < tuboExterno.PontoB.X &&
                         tuboInterno.PontoB.Y < tuboExterno.PontoA.Y &&
                         tuboInterno.PontoB.Y > tuboExterno.PontoC.Y
                        )
                    {
                        abertura = Abertura.Esquerda;
                    }

                    if (tuboInterno.PontoA.X > tuboExterno.PontoA.X &&
                        tuboInterno.PontoA.X < tuboExterno.PontoB.X &&
                        tuboInterno.PontoA.Y < tuboExterno.PontoA.Y &&
                        tuboInterno.PontoA.Y > tuboExterno.PontoC.Y
                       )
                    {
                        abertura = Abertura.Direita;
                    }


                    if (abertura != Abertura.fechado)
                    {
                        var points = new Point3dCollection();
                        points.Add(new Point3d(tuboInterno.PontoA.X, tuboInterno.PontoA.Y, 0));
                        points.Add(new Point3d(tuboInterno.PontoB.X, tuboInterno.PontoB.Y, 0));
                        points.Add(new Point3d(tuboInterno.PontoC.X, tuboInterno.PontoC.Y, 0));
                        points.Add(new Point3d(tuboInterno.PontoD.X, tuboInterno.PontoD.Y, 0));
                        points.Add(new Point3d(tuboExterno.PontoA.X, tuboExterno.PontoA.Y, 0));
                        points.Add(new Point3d(tuboExterno.PontoB.X, tuboExterno.PontoB.Y, 0));
                        points.Add(new Point3d(tuboExterno.PontoC.X, tuboExterno.PontoC.Y, 0));
                        points.Add(new Point3d(tuboExterno.PontoD.X, tuboExterno.PontoD.Y, 0));

                        var itemRelatorio = new ItemRelatorio(points);
                        itemRelatorio.ComprimentoTuboExterno = tuboExterno.Comprimento;
                        itemRelatorio.ComprimentoTuboInterno = tuboInterno.Comprimento;
                        itemRelatorio.Abertura = abertura;
                        listaGurdaCorpo.Add(itemRelatorio);
                    }
                }

                if (abertura == Abertura.fechado)
                {
                    tuboExterno.ComprimentoTuboExterno = 0;
                    tuboExterno.ComprimentoTuboInterno = 0;
                    listaGurdaCorpo.Add(tuboExterno);
                   
                }
            }

            return listaGurdaCorpo;
        }

    }
}
