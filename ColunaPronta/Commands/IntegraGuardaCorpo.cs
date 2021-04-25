using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Helper;
using ColunaPronta.Model;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using System.Linq;
using System;
using NLog.Config;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpo 
    {
        #region >> Comandos 
        public static void Add(Posicao posicao, bool bPosteInicial, bool bPosteFinal, Abertura abertura)
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
                                              , abertura
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
            try
            {
                var document = Application.DocumentManager.MdiActiveDocument;

                foreach (Poste poste in guardaCorpo.Postes)
                {
                    Helpers.AddPolyline(document, poste.PosteRetangulo.Pontos, Layer.Poste);

                    foreach (var cantoneira in poste.Cantoneiras)
                    {
                        Helpers.AddPolyline(document, cantoneira.Retangulo.Pontos, cantoneira.Retangulo.Layer);
                        if (cantoneira.Linha.Count == 2)
                        {
                            Point3d pnt1 = new Point3d(cantoneira.Linha[0].X, cantoneira.Linha[0].Y, 0);
                            Point3d pnt2 = new Point3d(cantoneira.Linha[1].X, cantoneira.Linha[1].Y, 0);

                            Helpers.AddLinha(document, pnt1, pnt2, cantoneira.Retangulo.Layer);
                        }

                        if (cantoneira.Parafusos != null)
                        {
                            foreach (var parafuso in cantoneira.Parafusos)
                            {
                                Helpers.AddCircle(document, parafuso.Point, parafuso.Raio, cantoneira.Retangulo.Layer);
                            }
                        }
                    }
                }

                foreach (GuardaCorpoFilho gc in guardaCorpo.GuardaCorpos)
                {
                    foreach (var tubo in gc.Tubos)
                    {
                        Helpers.AddPolyline(document, tubo.Pontos, tubo.Layer);
                    }

                    if (gc.PosteReforco != null)
                    {
                        foreach (var poste in gc.PosteReforco.Poste)
                        {
                            Helpers.AddPolyline(document, poste.Pontos, poste.Layer);
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
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
         

        }
        private static void IntegraGuardaCorpoFilho( Document document , CantoneiraGuardaCorpo cantoneira)
        {
            try
            {

                Helpers.AddPolyline(document, cantoneira.Retangulo.Pontos, cantoneira.Retangulo.Layer);

                if (cantoneira.Linha.Count == 2)
                {
                    Point3d pnt1 = new Point3d(cantoneira.Linha[0].X, cantoneira.Linha[0].Y, 0);
                    Point3d pnt2 = new Point3d(cantoneira.Linha[1].X, cantoneira.Linha[1].Y, 0);

                    Helpers.AddLinha(document, pnt1, pnt2, cantoneira.Retangulo.Layer);
                }

                if (cantoneira.Parafusos != null)
                {
                    foreach (var parafuso in cantoneira.Parafusos)
                    {
                        Helpers.AddCircle(document, parafuso.Point, parafuso.Raio, cantoneira.Retangulo.Layer);
                    }
                }
                if (cantoneira.PontosL != null)
                {
                    Helpers.AddPolyline(document, cantoneira.PontosL, Layer.CantoneiraL);
                }
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        private static void GeraArquivoListaCorte(ObjetosSelecionados objetos )
        {
            try
            {

                var arquivoExcel = new List<Planilha>();
                var layers = new EspecificacaoLayer();

                var listaTubo = new List<ItemRelatorio>();
                var listaCantoneiraL = new List<ItemRelatorio>();
                var listaPoste = new List<ItemRelatorio>();
                var listaCantoneira = new List<ItemRelatorio>();
                var listaPosteReforco = new List<ItemRelatorio>();

                if (objetos.Polylines != null)
                {
                    foreach (var poly in objetos.Polylines)
                    {

                        var layer = layers.GetLayer(poly.Layer.ToString());
                        switch (layer)
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

                            case Layer.TuboExterno:

                                var itemRelatorio = new ItemRelatorio(poly);
                                itemRelatorio.Descricao = layers.GetDescricaoLayer(layer);
                                listaTubo.Add(itemRelatorio);

                                break;

                            case Layer.TuboInterno:

                                var itemTuboInterno = new ItemRelatorio(poly);
                                itemTuboInterno.Descricao = layers.GetDescricaoLayer(layer);
                                listaTubo.Add(itemTuboInterno);

                                break;

                            case Layer.Poste:

                                var itemPoste = new ItemRelatorio(poly);
                                itemPoste.Descricao = layers.GetDescricaoLayer(layer);
                                listaPoste.Add(itemPoste);

                                break;

                            case Layer.PosteReforco:

                                var itemPosteReforco = new ItemRelatorio(poly);
                                itemPosteReforco.Descricao = layers.GetDescricaoLayer(layer);
                                listaPosteReforco.Add(itemPosteReforco);

                                break;
                        }

                    }

                    var quantidadeTubos = 5;
                    var settings = new Settings(true);

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
                                             QtdeColuna = c.Count() * quantidadeTubos
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
                                              Comprimento = ( settings.Altura * 1000 ) ,
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

                    var relatorioPosteReforco = (from item in listaPosteReforco
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
                                              Comprimento = ( settings.PosteReforcoAltura * 1000 ),
                                              QtdeColuna = c.Count()
                                          }).ToList();

                    foreach (var item in relatorioPosteReforco)
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
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
   
            
        #endregion
    }
}
