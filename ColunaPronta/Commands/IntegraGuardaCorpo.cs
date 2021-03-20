using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using System.Linq;

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

            IntegraGuardaCorpoVertical.Integra(guardaCorpo.GuardaCorpoVertical);

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
            var layers = new EspecificacaoLayer();

            var listaTubo = new List<ItemRelatorio>();
            var listaCantoneiraL = new List<ItemRelatorio>();
            var listaPoste = new List<ItemRelatorio>();
            var listaCantoneira = new List<ItemRelatorio>();

            if (objetos.Polylines != null)
            {
                foreach( var poly in objetos.Polylines)
                {

                    var layer = layers.GetLayer(poly.Layer.ToString());
                    switch(layer)
                    {
                        
                        case Layer.Cantoneira:

                            var itemCantoneira = new ItemRelatorio(poly);
                            itemCantoneira.Descricao = layers.GetDescricaoLayer(layer);
                            listaCantoneira.Add(itemCantoneira);

                            break;
                         
                        case Layer.CantoneiraL:

                            var itemCantoneiraL = new ItemRelatorio(poly);
                            itemCantoneiraL.Descricao = layers.GetDescricaoLayer(layer);
                            listaCantoneiraL.Add(itemCantoneiraL);

                            break;
                        
                        case Layer.Tubo:

                            var itemRelatorio = new ItemRelatorio(poly);
                            itemRelatorio.Descricao = layers.GetDescricaoLayer(layer);
                            listaTubo.Add(itemRelatorio);

                            break;
                        
                        case Layer.Poste:
                            
                            var itemPoste = new ItemRelatorio(poly);
                            itemPoste.Descricao = layers.GetDescricaoLayer(layer);
                            listaPoste.Add(itemPoste);

                            break;
                        
                        case Layer.PosteReforco: 
                        
                            
                            break;
                    }

                }
                var relatorioTubo = (from item in listaTubo
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

                foreach (var item in relatorioTubo)
                {
                    var linha = new Planilha();
                    linha.Comprimento = item.Comprimento.ToString();
                    linha.Especificao = item.Descricao.ToString();
                    linha.Quantidade = item.QtdeColuna;
                    arquivoExcel.Add(linha);
                }

                var relatorioCantoneiraL = (from item in listaCantoneiraL
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

                foreach (var item in relatorioCantoneiraL)
                {
                    var linha = new Planilha();
                    linha.Comprimento = item.Comprimento.ToString();
                    linha.Especificao = item.Descricao.ToString();
                    linha.Quantidade = item.QtdeColuna;
                    arquivoExcel.Add(linha);
                }

                var relatorioPoste = (from item in listaPoste
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

                foreach (var item in relatorioPoste)
                {
                    var linha = new Planilha();
                    linha.Comprimento = item.Comprimento.ToString();
                    linha.Especificao = item.Descricao.ToString();
                    linha.Quantidade = item.QtdeColuna;
                    arquivoExcel.Add(linha);
                }

                var relatorioCantoneira = (from item in listaCantoneira
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

                foreach (var item in relatorioCantoneira)
                {
                    var linha = new Planilha();
                    linha.Comprimento = item.Comprimento.ToString();
                    linha.Especificao = item.Descricao.ToString();
                    linha.Quantidade = item.QtdeColuna;
                    arquivoExcel.Add(linha);
                }

                var nomeProjeto = Application.DocumentManager.MdiActiveDocument.Window.Text;
                Helpers.GeraArquivoExcel(arquivoExcel, nomeProjeto, TipoLista.ListaCorteGuardaCorpo);
            }
            //if (objetos.Circles != null)
            //{
            //    var countcircles = objetos.Circles.Count;
            //}

           

        }
        #endregion
    }
}
