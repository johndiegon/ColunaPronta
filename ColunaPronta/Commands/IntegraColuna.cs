using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using ColunaPronta.Helper;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System;
using NLog.Config;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace ColunaPronta.Commands
{
    public static class IntegraColuna
    {
        #region >> Propriedades

        const double _escala = 1000;
        const double distancia = 38 / _escala;
        const int iLinhasParafuso = 15;
        const int ladoEle = 38;
        const int distanciaSapata = 2;
        #endregion

        #region >> Seleciona 

        public static Coluna SelecionaColuna()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Database database = document.Database;
            Editor editor = document.Editor;

            var coluna = new Coluna()
            {
                NomeArquivo = document.Window.Text
            };

            PromptSelectionResult psr = editor.GetSelection();

            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {

                    SelectionSet selectionSet = psr.Value;
                    ObjectId[] objectIds = selectionSet.GetObjectIds();

                    var points = new Point3dCollection();
                    foreach (ObjectId objectId in objectIds)
                    {
                        var dbo = tr.GetObject(objectId, OpenMode.ForRead);
                        var poly = (Polyline)dbo;
                        if (poly != null)
                        {

                            for (int i = 0; i < poly.NumberOfVertices; i++)
                            {
                                points.Add(poly.GetPoint3dAt(i));
                            }
                        }
                    }
                    coluna.SetPontos(points);
                    tr.Commit();
                }
            }
            else
            {
                editor.WriteMessage("\nÉ necessário selecionar a coluna de referência.");
                return null;
            }

            return coluna;
        }

        public static int GetLayoutIdentificado()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;

            int id = 0;
            PromptStringOptions opt = new PromptStringOptions("\nInforme o tipo de coluna:");
            PromptResult promptResult = editor.GetString(opt);

            if (promptResult.Status == PromptStatus.OK)
            {

                var teste = promptResult.StringResult.ToString().ToUpper();
                var txtId = teste.Replace("C", "");
                if (txtId == "")
                {
                    id = 0;
                }
                else
                {
                    id = Convert.ToInt32(txtId);
                }
            }

            if (id == 0)
            {
                editor.WriteMessage("\nÉ necessário informar o tipo de coluna para ser gerado.");
                return 0;
            }

            return id;
        }
        public static List<ItemRelatorio> GetDadosRelatorio(string nomeProjeto)
        {
            try
            {
                var colunas = ArquivoCSV.GetColunas(nomeProjeto);
                var enrijecedores = ArquivoCSV.GetEnrijecedores();

                if (colunas != null)
                {
                    var colunasPonto = from coluna in colunas
                                       group coluna by coluna.PointA into colunaPontoA
                                       select colunaPontoA.Key;
                    var colunasRelatorio = new List<Coluna>();

                    foreach (Point2d pontoColuna in colunasPonto)
                    {
                        var c = (from coluna in colunas
                                 where coluna.PointA == pontoColuna
                                 orderby coluna.dInclusao descending
                                 select new Coluna
                                 {
                                     iColuna = coluna.iColuna,
                                     DiametroParafuso = coluna.DiametroParafuso,
                                     DiametroSapata = coluna.DiametroSapata,
                                     QuantidadeParafuso = coluna.QuantidadeParafuso,
                                     Comprimento = coluna.Comprimento,
                                     Largura = coluna.Largura ,
                                     Altura = coluna.Altura,
                                     AlturaViga = coluna.AlturaViga
                                 }).FirstOrDefault();

                        colunasRelatorio.Add(c);
                    }

                    var relatorio = (from coluna in colunasRelatorio
                                     group coluna by new
                                     {
                                         coluna.Altura,
                                         coluna.AlturaViga,
                                         coluna.Comprimento,
                                         coluna.Largura,
                                         coluna.iColuna,
                                         coluna.QuantidadeParafuso,
                                         coluna.DiametroSapata,
                                         coluna.DiametroParafuso

                                     } into c
                                     select new ItemRelatorio
                                     {
                                         Altura = c.Key.Altura,
                                         AlturaViga = c.Key.AlturaViga,
                                         Comprimento = c.Key.Comprimento,
                                         Largura = c.Key.Largura,
                                         iColuna = c.Key.iColuna,
                                         QtdeParafuso = c.Key.QuantidadeParafuso,
                                         DiametroSapata = c.Key.DiametroSapata,
                                         DiametroParafuso = c.Key.DiametroParafuso,
                                         QtdeColuna = c.Count() 
                                     }).ToList();

                    foreach (ItemRelatorio item in relatorio)
                    {
                        var perfil = (from enrijecedor in enrijecedores
                                      where enrijecedor.Perfil == item.Comprimento
                                      select enrijecedor
                                       ).FirstOrDefault();

                        item.Enrijecedor = perfil == null ? 0 : perfil.Valor;
                    }

                    return relatorio;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }

        #endregion

        #region >> Gera Objetos 
        public static void AddColuna(Coluna coluna, bool registra)
        {
            AddParafuso(coluna);
            AddPassante(coluna);
            AddEle(coluna);

            if (coluna.SapataA == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataD == true && coluna.Posicao == Posicao.Vertical)
            {
                var p1 = new Point2d(coluna.PointA.X, coluna.PointA.Y + distancia);
                var p2 = new Point2d(coluna.PointB.X, coluna.PointB.Y + distancia);
                var p3 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p4 = new Point2d(coluna.PointA.X, coluna.PointA.Y);

                Document document = Application.DocumentManager.MdiActiveDocument;
                Helpers.AddLinha(document, new Point3d(p1.X, coluna.PointA.Y + (3 / _escala), 0), new Point3d(p2.X, coluna.PointA.Y + (3 / _escala), 0), true, ColorIndex.vermelho);

                AddSapata(p1, p2, p3, p4, coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataB == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataA == true && coluna.Posicao == Posicao.Vertical)
            {
                var p1 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p2 = new Point2d(coluna.PointB.X + distancia, coluna.PointB.Y);
                var p3 = new Point2d(coluna.PointB.X + distancia, coluna.PointC.Y);
                var p4 = new Point2d(coluna.PointB.X, coluna.PointC.Y);

                Document document = Application.DocumentManager.MdiActiveDocument;
                Helpers.AddLinha(document, new Point3d(p1.X + (3 / _escala), p1.Y, 0), new Point3d(p1.X + (3 / _escala), coluna.PointC.Y, 0), true, ColorIndex.vermelho);

                AddSapata(p1, p2, p3, p4, coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataC == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataB == true && coluna.Posicao == Posicao.Vertical)
            {
                var p4 = new Point2d(coluna.PointC.X, coluna.PointC.Y - distancia);
                var p3 = new Point2d(coluna.PointD.X, coluna.PointD.Y - distancia);
                var p1 = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                var p2 = new Point2d(coluna.PointD.X, coluna.PointD.Y);

                Document document = Application.DocumentManager.MdiActiveDocument;
                Helpers.AddLinha(document, new Point3d(p1.X, p1.Y - (3 / _escala), 0), new Point3d(p2.X, p1.Y - (3 / _escala), 0), true, ColorIndex.vermelho);

                AddSapata(p1, p2, p3, p4, coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataD == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataC == true && coluna.Posicao == Posicao.Vertical)
            {

                var p1 = new Point2d(coluna.PointA.X - distancia, coluna.PointA.Y);
                var p2 = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p3 = new Point2d(coluna.PointA.X, coluna.PointD.Y);
                var p4 = new Point2d(coluna.PointA.X - distancia, coluna.PointD.Y);

                Document document = Application.DocumentManager.MdiActiveDocument;
                Helpers.AddLinha(document, new Point3d(p2.X - (3 / _escala), p1.Y, 0), new Point3d(p2.X - (3 / _escala), p3.Y, 0), true, ColorIndex.vermelho);

                AddSapata(p1, p2, p3, p4, coluna.DiametroSapata / _escala);
            }

            if (registra)
            {
                ArquivoCSV.Registra(coluna);
                AddTitulo(coluna.PointA, coluna.iColuna);
            }
            IntegraLayout.SetUltimaColuna(coluna.iColuna);
        }

        public static void AddColunaLayout(Coluna coluna, Coluna layout)
        {
            coluna.iColuna = layout.iColuna;
            coluna.Altura = layout.Altura;
            coluna.DiametroParafuso = layout.DiametroParafuso;
            coluna.DiametroSapata = layout.DiametroSapata;
            coluna.QuantidadeParafuso = layout.QuantidadeParafuso;
            coluna.ParafusoA = layout.ParafusoA;
            coluna.ParafusoB = layout.ParafusoB;
            coluna.ParafusoC = layout.ParafusoC;
            coluna.ParafusoD = layout.ParafusoD;
            coluna.ParafusoE = layout.ParafusoE;
            coluna.ParafusoF = layout.ParafusoF;
            coluna.ParafusoG = layout.ParafusoG;
            coluna.ParafusoH = layout.ParafusoH;
            coluna.SapataA = layout.SapataA;
            coluna.SapataB = layout.SapataB;
            coluna.SapataC = layout.SapataC;
            coluna.SapataD = layout.SapataD;
            coluna.PassanteA = layout.PassanteA;
            coluna.PassanteB = layout.PassanteB;
            coluna.PassanteC = layout.PassanteC;
            coluna.PassanteD = layout.PassanteD;
            coluna.eleAmarelo = layout.eleAmarelo;
            coluna.eleVermelho = layout.eleVermelho;
            coluna.eleAzul = layout.eleAzul;
            coluna.eleCinza = layout.eleCinza;
            coluna.AlturaViga = layout.AlturaViga;

            AddColuna(coluna, true);
        }

        public static Point3dCollection AddEstruturaColuna(Document? document, Point2d startPoint, double largura, double comprimento)
        {
            var pontosColuna = new Point3dCollection();

            #region >> Estrutura Lado A ( Voltado para Direita  )
            var LadoA = new Point2dCollection();

            // Ponto A
            LadoA.Add(new Point2d(startPoint.X, startPoint.Y));
            pontosColuna.Add((new Point3d(startPoint.X, startPoint.Y, 0)));
            // Ponto B                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X, startPoint.Y - (comprimento / _escala)));
            pontosColuna.Add(new Point3d(startPoint.X, startPoint.Y - (comprimento / _escala), 0));
            // Ponto C                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - (comprimento / _escala)));
            // Ponto D                                                                 
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - ((comprimento - 20) / _escala)));
            // Ponto E
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala), startPoint.Y - ((comprimento - 20) / _escala)));
            // Ponto F
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala), startPoint.Y - ((comprimento - 2) / _escala)));
            // Ponto G
            LadoA.Add(new Point2d(startPoint.X + (2 / _escala), startPoint.Y - ((comprimento - 2) / _escala)));
            // Ponto H                                                                 
            LadoA.Add(new Point2d(startPoint.X + (2 / _escala), startPoint.Y - (2 / _escala)));
            // Ponto I                                                                 
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala), startPoint.Y - (2 / _escala)));
            // Ponto J                                                                 
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala), startPoint.Y - (20 / _escala)));
            // Ponto L                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - (20 / _escala)));
            // Ponto M                                                                 
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y));
            // Ponto A                                                                 
            LadoA.Add(new Point2d(startPoint.X, startPoint.Y));

            if(document !=null)
            {
                Helpers.AddPolyline(document, LadoA, ColorIndex.padrao);
            }
            #endregion

            #region >> Estrutura Lado B ( Voltado para Esquerda )
            var LadoB = new Point2dCollection();

            // Ponto A
            LadoB.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y));
            // Ponto B                                                                                                                     
            LadoB.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - (20 / _escala)));
            // Ponto C                                                                                                                     
            LadoB.Add(new Point2d(startPoint.X + (((largura / 2) + 2) / _escala), startPoint.Y - (20 / _escala)));
            // Ponto D                                                                 
            LadoB.Add(new Point2d(startPoint.X + (((largura / 2) + 2) / _escala), startPoint.Y - (2 / _escala)));
            // Ponto E
            LadoB.Add(new Point2d(startPoint.X + ((largura - 2) / _escala), startPoint.Y - (2 / _escala)));
            // Ponto F
            LadoB.Add(new Point2d(startPoint.X + ((largura - 2) / _escala), startPoint.Y - ((comprimento - 2) / _escala)));
            // Ponto G
            LadoB.Add(new Point2d(startPoint.X + (((largura / 2) + 2) / _escala), startPoint.Y - ((comprimento - 2) / _escala)));
            // Ponto H                                                                 
            LadoB.Add(new Point2d(startPoint.X + (((largura / 2) + 2) / _escala), startPoint.Y - ((comprimento - 20) / _escala)));
            // Ponto I                                                                 
            LadoB.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - ((comprimento - 20) / _escala)));
            // Ponto J                                                                 
            LadoB.Add(new Point2d(startPoint.X + ((largura / 2) / _escala), startPoint.Y - (comprimento / _escala)));
            // Ponto L                                                                                                                     
            LadoB.Add(new Point2d(startPoint.X + ((largura) / _escala), startPoint.Y - (comprimento / _escala)));
            pontosColuna.Add(new Point3d(startPoint.X + ((largura) / _escala), startPoint.Y - (comprimento / _escala), 0));
            // Ponto M                                                                 
            LadoB.Add(new Point2d(startPoint.X + ((largura) / _escala), startPoint.Y));
            pontosColuna.Add(new Point3d(startPoint.X + ((largura) / _escala), startPoint.Y, 0));

            // Ponto A                                                                 
            LadoB.Add(new Point2d(startPoint.X, startPoint.Y));

            if (document != null)
            {
                Helpers.AddPolyline(document, LadoB, ColorIndex.padrao);
            }

            #endregion

            return pontosColuna;
        }

        public static void AddSapata(Point2d p1, Point2d p2, Point2d p3, Point2d p4, double diametro)
        {
            var raio = diametro / 2;

            Document document = Application.DocumentManager.MdiActiveDocument;

            Helpers.AddPolyline(document, p1, p2, p3, p4);

            List<double> ListaY = new List<double>();
            List<double> ListaX = new List<double>();

            ListaX.Add(p1.X);
            ListaY.Add(p2.Y);
            ListaX.Add(p2.X);
            ListaY.Add(p2.Y);
            ListaX.Add(p3.X);
            ListaY.Add(p3.Y);
            ListaX.Add(p4.X);
            ListaY.Add(p4.Y);

            var ponto1 = new Point2d(ListaX.Min(), ListaY.Max());
            var ponto2 = new Point2d(ListaX.Max(), ListaY.Max());
            var ponto3 = new Point2d(ListaX.Max(), ListaY.Min());

            var centerX = ponto1.GetDistanceTo(ponto2) / 2;
            var centery = ponto2.GetDistanceTo(ponto3) / 2;

            Helpers.AddCircle(document, new Point3d(p1.X + centerX, p1.Y - centery, 0), raio);

        }

        public static void AddParafuso(Coluna coluna)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;

            if ((coluna.ParafusoA == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoG == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoBaixo, coluna.PointA);
            }
            if ((coluna.ParafusoB == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoH == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoBaixo, new Point2d(coluna.PointB.X - (40 / _escala), coluna.PointA.Y));
            }
            if ((coluna.ParafusoC == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoA == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoEsqueda, new Point2d(coluna.PointB.X, coluna.PointB.Y + (15 / _escala)));
            }
            if ((coluna.ParafusoD == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoB == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoEsqueda, new Point2d(coluna.PointD.X - (5 / _escala), coluna.PointD.Y + (55 / _escala)));
            }
            if ((coluna.ParafusoE == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoC == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoCima, new Point2d(coluna.PointD.X - (5 / _escala), coluna.PointD.Y));
            }
            if ((coluna.ParafusoF == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoD == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoCima, new Point2d(coluna.PointC.X + (35 / _escala), coluna.PointC.Y));
            }
            if ((coluna.ParafusoG == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoE == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoDireita, new Point2d(coluna.PointC.X, coluna.PointC.Y - (15 / _escala)));
            }
            if ((coluna.ParafusoH == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoF == true && coluna.Posicao == Posicao.Vertical))
            {
                AddParafuso(document, Posicao.VoltadoDireita, new Point2d(coluna.PointA.X, coluna.PointA.Y - (55 / _escala)));
            }
        }

        public static void AddParafuso(Document document, Posicao posicao, Point2d PontoA)
        {
            Point2d p1, p2, p3, p4, p2v1, p2v2, p2v3, p2v4;
            switch (posicao)
            {
                case Posicao.VoltadoBaixo:
                    p1 = new Point2d(PontoA.X + (10 / _escala), PontoA.Y + (30 / _escala));
                    p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                    p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                    p4 = new Point2d(p1.X, p1.Y - (10 / _escala));

                    Helpers.AddPolyline(document, p1, p2, p3, p4, 3);

                    p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y);
                    p2v2 = new Point2d(p2v1.X, p2v1.Y - (50 / _escala));
                    p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - (50 / _escala));
                    p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                    Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, 3);

                    for (int i = 1; i < iLinhasParafuso; i++)
                    {
                        Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y - ((50 - i) / _escala), 0), new Point3d(p2v1.X + (10 / _escala), p2v1.Y - ((50 - i) / _escala), 0), false, ColorIndex.verde);
                    }
                    break;
                case Posicao.VoltadoEsqueda:
                    p1 = new Point2d(PontoA.X + (20 / _escala), PontoA.Y - (25 / _escala));
                    p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                    p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                    p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                    Helpers.AddPolyline(document, p1, p2, p3, p4, 3);

                    p2v1 = new Point2d(p2.X, p2.Y - (5 / _escala));
                    p2v2 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));
                    p2v3 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y - (10 / _escala));
                    p2v4 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y);

                    Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, 3);

                    for (int i = 1; i < iLinhasParafuso; i++)
                    {
                        Helpers.AddLinha(document, new Point3d(p2v1.X - ((50 - i) / _escala), p2v1.Y - (10 / _escala), 0), new Point3d(p2v1.X - ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                    }
                    break;
                case Posicao.VoltadoCima:
                    p1 = new Point2d(PontoA.X - (5 / _escala), PontoA.Y - (20 / _escala));
                    p2 = new Point2d(p1.X, p1.Y - (10 / _escala));
                    p3 = new Point2d(p1.X - (20 / _escala), p1.Y - (10 / _escala));
                    p4 = new Point2d(p1.X - (20 / _escala), p1.Y);

                    Helpers.AddPolyline(document, p1, p2, p3, p4, 3);

                    p2v1 = new Point2d(p1.X - (5 / _escala), p2.Y);
                    p2v2 = new Point2d(p2v1.X, p2v1.Y + (50 / _escala));
                    p2v3 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y + (50 / _escala));
                    p2v4 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y);

                    Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, 3);

                    for (int i = 1; i < iLinhasParafuso; i++)
                    {
                        Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y + ((50 - i) / _escala), 0),
                                                   new Point3d(p2v1.X - (10 / _escala), p2v1.Y + ((50 - i) / _escala), 0), false, ColorIndex.verde);
                    }
                    break;
                case Posicao.VoltadoDireita:
                    p1 = new Point2d(PontoA.X - (30 / _escala), PontoA.Y + (45 / _escala));
                    p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                    p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                    p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                    Helpers.AddPolyline(document, p1, p2, p3, p4, 3);

                    p2v1 = new Point2d(p1.X, p1.Y - (5 / _escala));
                    p2v2 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y);
                    p2v3 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y - (10 / _escala));
                    p2v4 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));

                    Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, 3);

                    for (int i = 1; i < iLinhasParafuso; i++)
                    {
                        Helpers.AddLinha(document, new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y - (10 / _escala), 0),
                                                 new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                    }
                    break;
            }
        }

        public static void AddPassante(Coluna coluna)
        {

            if ((coluna.PassanteA == true && coluna.Posicao == Posicao.Horizontal)
                || (coluna.PassanteD == true && coluna.Posicao == Posicao.Vertical)
             )
            {
                var comprimento = Posicao.Horizontal == coluna.Posicao ? coluna.Comprimento : coluna.Largura;
                var PontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                AddRetangulo(PontoA, Posicao.Horizontal, 40, comprimento);
            }

            if ((coluna.PassanteB == true && coluna.Posicao == Posicao.Horizontal) ||
                    (coluna.PassanteA == true && coluna.Posicao == Posicao.Vertical)
              )
            {
                var comprimento = Posicao.Vertical == coluna.Posicao ? coluna.Comprimento : coluna.Largura;
                var PontoA = new Point2d(coluna.PointB.X - (40 / _escala), coluna.PointB.Y);
                AddRetangulo(PontoA, Posicao.Vertical, 40, comprimento);
            }

            if ((coluna.PassanteC == true && coluna.Posicao == Posicao.Horizontal) ||
                    (coluna.PassanteB == true && coluna.Posicao == Posicao.Vertical)
              )
            {
                var comprimento = Posicao.Horizontal == coluna.Posicao ? coluna.Comprimento : coluna.Largura;
                var PontoA = new Point2d(coluna.PointC.X, coluna.PointC.Y + (40 / _escala));
                AddRetangulo(PontoA, Posicao.Horizontal, 40, comprimento);
            }

            if ((coluna.PassanteD == true && coluna.Posicao == Posicao.Horizontal) ||
                    (coluna.PassanteC == true && coluna.Posicao == Posicao.Vertical)
              )
            {
                var comprimento = Posicao.Vertical == coluna.Posicao ? coluna.Comprimento : coluna.Largura;
                var PontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                AddRetangulo(PontoA, Posicao.Vertical, 40, comprimento);
            }
        }

        public static void AddRetangulo(Point2d PontoA, Posicao posicao, double largura, double comprimento)
        {
            Point2d p1, p2, p3, p4 = new Point2d();
            var collection = new Point2dCollection();

            if (posicao == Posicao.Vertical)
            {
                p1 = new Point2d(PontoA.X, PontoA.Y);
                p2 = new Point2d(PontoA.X + (largura / _escala), PontoA.Y);
                p3 = new Point2d(PontoA.X + (largura / _escala), PontoA.Y - (comprimento / _escala));
                p4 = new Point2d(PontoA.X, PontoA.Y - (comprimento / _escala));

            }
            else
            {
                p1 = new Point2d(PontoA.X, PontoA.Y);
                p2 = new Point2d(PontoA.X + (comprimento / _escala), PontoA.Y);
                p3 = new Point2d(PontoA.X + (comprimento / _escala), PontoA.Y - (largura / _escala));
                p4 = new Point2d(PontoA.X, PontoA.Y - (largura / _escala));
            }

            collection.Add(p1);
            collection.Add(p2);
            collection.Add(p3);
            collection.Add(p4);
            collection.Add(p1);

            Document document = Application.DocumentManager.MdiActiveDocument;
            Helpers.AddPolyline(document, collection, ColorIndex.padrao);

        }

        public static void AddEle(Coluna coluna)
        {
            if ((coluna.eleVermelho == true && coluna.Posicao == Posicao.Horizontal) ||
                (coluna.eleCinza == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointA.X + ((5 + ladoEle) / _escala), coluna.PointA.Y + (ladoEle / _escala)), Posicao.BaixoDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointC.X + (5 / _escala), coluna.PointC.Y), Posicao.CimDireita, ladoEle);
            }
            if ((coluna.eleAmarelo == true && coluna.Posicao == Posicao.Horizontal) ||
                (coluna.eleVermelho == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointA.X, coluna.PointA.Y - (5 / _escala)), Posicao.BaixoDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointB.X, coluna.PointB.Y - (5 / _escala)), Posicao.BaixoEsquerda, ladoEle);

            }
            if ((coluna.eleAzul == true && coluna.Posicao == Posicao.Horizontal) ||
               (coluna.eleAmarelo == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointB.X - ((5 + ladoEle) / _escala), coluna.PointB.Y + (ladoEle / _escala)), Posicao.BaixoEsquerda, ladoEle);
                AddElePolyline(new Point2d(coluna.PointD.X - ((5 + ladoEle) / _escala), coluna.PointD.Y), Posicao.CimEsquerda, ladoEle);
            }
            if ((coluna.eleCinza == true && coluna.Posicao == Posicao.Horizontal) ||
               (coluna.eleAzul == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointC.X - (ladoEle / _escala), coluna.PointC.Y + ((5 + ladoEle) / _escala)), Posicao.CimDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointD.X, coluna.PointD.Y + ((5 + ladoEle) / _escala)), Posicao.CimEsquerda, ladoEle);
            }
        }

        public static void AddElePolyline(Point2d PontoA, Posicao posicao, double lado)
        {
            Point2d p1, p2, p3, p4, p5, p6 = new Point2d();
            var collection = new Point2dCollection();
            bool bPosicaoInvalida = false;

            switch (posicao)
            {
                case Posicao.BaixoDireita:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X, PontoA.Y - (lado / _escala));
                    p3 = new Point2d(PontoA.X - (lado / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X - (lado / _escala), PontoA.Y - ((lado - 2) / _escala));
                    p5 = new Point2d(PontoA.X - (2 / _escala), PontoA.Y - ((lado - 2) / _escala));
                    p6 = new Point2d(PontoA.X - (2 / _escala), PontoA.Y);
                    break;
                case Posicao.BaixoEsquerda:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X, PontoA.Y - (lado / _escala));
                    p3 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - ((lado - 2) / _escala));
                    p5 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y - ((lado - 2) / _escala));
                    p6 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y);
                    break;
                case Posicao.CimDireita:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y);
                    p3 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X + ((lado - 2) / _escala), PontoA.Y - ((lado) / _escala));
                    p5 = new Point2d(PontoA.X + ((lado - 2) / _escala), PontoA.Y - (2 / _escala));
                    p6 = new Point2d(PontoA.X, PontoA.Y - (2 / _escala));
                    break;
                case Posicao.CimEsquerda:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X, PontoA.Y - (lado / _escala));
                    p3 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y - (2 / _escala));
                    p5 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - (2 / _escala));
                    p6 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y);
                    break;
                default:
                    bPosicaoInvalida = true;
                    p1 = new Point2d(0, 0);
                    p2 = new Point2d(0, 0);
                    p3 = new Point2d(0, 0);
                    p4 = new Point2d(0, 0);
                    p5 = new Point2d(0, 0);
                    p6 = new Point2d(0, 0);
                    break;
            }

            if (!bPosicaoInvalida)
            {
                collection.Add(p1);
                collection.Add(p2);
                collection.Add(p3);
                collection.Add(p4);
                collection.Add(p5);
                collection.Add(p6);
                collection.Add(p1);

                Document document = Application.DocumentManager.MdiActiveDocument;
                Helpers.AddPolyline(document, collection, ColorIndex.padrao);
            }
        }

        public static void AddTitulo(Point2d PontoA, int iColuna)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            var textTipoColuna = string.Concat("C", iColuna);
            var point = new Point3d(PontoA.X - (60 / _escala), PontoA.Y + (5 / _escala), 0);
            Helpers.AddTexto(document, point, textTipoColuna, ColorIndex.padrao);
        }
        #endregion

        #region >> Relatorio
        public static void GeraRelatorio()
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Editor editor = document.Editor;

                var nomeProjeto = document.Window.Text;

                var dadosRelatorio = GetDadosRelatorio(nomeProjeto);

                if (dadosRelatorio == null)
                {
                    editor.WriteMessage("\nNão possui dados para gerar o relatório ou o nome de um dos arquivos (.csv ou .dwg) foram renomeados ");
                    return;
                }

                PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto onde será gerado o relatório (EndPoint ): ")
                {
                    AllowArbitraryInput = false,
                    AllowNone = true
                };

                PromptPointResult prPtRes = editor.GetPoint(prPtOpt);

                var startPoint = new Point2d(prPtRes.Value.X, prPtRes.Value.Y);

                if (prPtRes.Status == PromptStatus.OK)
                {
                    double startX = startPoint.X;
                    double startY = startPoint.Y;
                    double distancia = 0;

                    foreach (ItemRelatorio item in dadosRelatorio)
                    {
                        var coluna = IntegraLayout.GetLayout(item.iColuna, nomeProjeto);
                        coluna.Largura = item.Largura;
                        coluna.Comprimento = item.Comprimento;
                        coluna.Altura = item.Altura;
                        coluna.DiametroSapata = item.DiametroSapata;
                        coluna.DiametroParafuso = item.DiametroParafuso;
                        var pontosColuna = AddEstruturaColuna(document, new Point2d(startX, startY - (distancia / _escala)), item.Largura, item.Comprimento);

                        coluna.SetPontos(pontosColuna);

                        AddColuna(coluna, false);

                        var textoDescricao = string.Concat("C", coluna.iColuna.ToString(), " - "
                                                       , coluna.Comprimento.ToString("N2"), " x "
                                                       , coluna.Largura.ToString("N2"), " x "
                                                       , coluna.Altura.ToString("N2"), " mm - "
                                                       , item.QtdeColuna.ToString(), ""
                                                       , item.QtdeColuna == 1 ? " unidade." : " unidades."
                                                       );
                        Helpers.AddTexto(document
                                        , new Point3d(startX + ((item.Largura + 60) / _escala), startY - ((50 + distancia) / _escala), 0)
                                        , textoDescricao
                                        , ColorIndex.verde
                                        );

                        distancia = distancia + item.Comprimento + 100;
                    }
                }
                document.TransactionManager.StartOpenCloseTransaction().Commit();
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

        public static void GeraRelatorioExcel(TipoLista tipoLista)
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Editor editor = document.Editor;

                var nomeProjeto = document.Window.Text;
                var especificacoes = ArquivoCSV.GetEspecificacao();
                var dadosRelatorio = GetDadosRelatorio(nomeProjeto);
                int fatorTpRelatorio = tipoLista == TipoLista.ListaCorteColuna ? 2 : 1;

                if (dadosRelatorio == null)
                {
                    editor.WriteMessage("\nNão possui dados para gerar o relatório ou o nome de um dos arquivos (.csv ou .dwg) foram renomeados ");
                    return;
                }

                var listaSapata = new List<Sapata>();
                double startX = 0;
                double startY = 0;
                double distancia = 0;
                var listaParafuso = new List<Parafuso>();
                var listaCantoneiraViga = new List<Cantoneira>();
                var listaCantoneira3Furos = new List<Cantoneira>();
                double qtdCantoneira3Furos = 0;
                int qtdCantoneira1Furo  = 0;
                double qtdColunaPassante   = 0;

                foreach (ItemRelatorio item in dadosRelatorio)
                {
                   
                    var coluna = IntegraLayout.GetLayout(item.iColuna, nomeProjeto);
                    coluna.Largura = item.Largura;
                    coluna.Comprimento = item.Comprimento;
                    coluna.Altura = item.Altura;
                    coluna.DiametroSapata = item.DiametroSapata;
                    coluna.DiametroParafuso = item.DiametroParafuso;
                    var pontosColuna = AddEstruturaColuna(null, new Point2d(startX, startY - (distancia / _escala)), item.Largura, item.Comprimento);
                    int iqtdParafuso = 0; 

                    coluna.SetPontos(pontosColuna);

                    if (coluna.SapataA)
                    {
                        var sapata = GetSapata(new Point2d(coluna.PointB.X, coluna.PointB.Y), new Point2d(coluna.PointD.X, coluna.PointD.Y));
                        sapata.Chumbador = coluna.DiametroSapata;
                        sapata.Quantidade = item.QtdeColuna;
                        listaSapata.Add(sapata);
                    }
                    if (coluna.SapataB)
                    {
                        var sapata = GetSapata(new Point2d(coluna.PointA.X, coluna.PointA.Y), new Point2d(coluna.PointB.X, coluna.PointB.Y));
                        sapata.Chumbador = coluna.DiametroSapata;
                        sapata.Quantidade = item.QtdeColuna;
                        listaSapata.Add(sapata);
                    }
                    if (coluna.SapataC)
                    {
                        var sapata = GetSapata(new Point2d(coluna.PointB.X, coluna.PointB.Y), new Point2d(coluna.PointD.X, coluna.PointD.Y));
                        sapata.Chumbador = coluna.DiametroSapata;
                        sapata.Quantidade = item.QtdeColuna;
                        listaSapata.Add(sapata);
                    }
                    if (coluna.SapataD)
                    { 
                        var sapata = GetSapata(new Point2d(coluna.PointC.X, coluna.PointC.Y), new Point2d(coluna.PointD.X, coluna.PointD.Y));
                        sapata.Chumbador = coluna.DiametroSapata;
                        sapata.Quantidade = item.QtdeColuna;
                        listaSapata.Add(sapata);
                    }
            
                    if (coluna.ParafusoA) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoB) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoC) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoD) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoE) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoF) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoG) { iqtdParafuso ++; item.bPassante = false; }
                    if (coluna.ParafusoH) { iqtdParafuso ++; item.bPassante = false; }

                    var parafuso = new Parafuso
                    {
                        Quantidade = iqtdParafuso * item.QtdeParafuso * item.QtdeColuna,
                        Diametro = coluna.DiametroParafuso
                    };

                    listaParafuso.Add(parafuso);

                    if (coluna.eleAmarelo)  { qtdCantoneira1Furo++;  item.bPassante = true; }
                    if (coluna.eleAzul)     { qtdCantoneira1Furo++;  item.bPassante = true; }
                    if (coluna.eleCinza)    { qtdCantoneira1Furo++;  item.bPassante = true; }
                    if (coluna.eleVermelho) { qtdCantoneira1Furo++;  item.bPassante = true; }
                    if (coluna.PassanteA)   
                    { 
                        qtdCantoneira3Furos += item.QtdeColuna; item.bPassante = true;
                        
                        var point1 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                        var point2 = new Point2d(coluna.PointD.X, coluna.PointD.Y);
                        var comprimento = point1.GetDistanceTo(point2);
                        var cantoneira = new Cantoneira
                        {
                            Comprimento = ( comprimento * _escala) - distanciaSapata,
                            Quantidade = item.QtdeColuna
                        };
                        listaCantoneira3Furos.Add(cantoneira);

                    }
                    if (coluna.PassanteB)   
                    { 
                        qtdCantoneira3Furos += item.QtdeColuna; item.bPassante = true;

                        var point1 = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                        var point2 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                        var comprimento = point1.GetDistanceTo(point2);
                        var cantoneira = new Cantoneira
                        {
                            Comprimento = (comprimento * _escala) - distanciaSapata,
                            Quantidade = item.QtdeColuna

                        };
                        listaCantoneira3Furos.Add(cantoneira);

                    }
                    if (coluna.PassanteC)  
                    { 
                        qtdCantoneira3Furos += item.QtdeColuna; item.bPassante = true;
                        var point1 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                        var point2 = new Point2d(coluna.PointD.X, coluna.PointD.Y);
                        var comprimento = point1.GetDistanceTo(point2);
                        var cantoneira = new Cantoneira
                        {
                            Comprimento = (comprimento * _escala) - distanciaSapata,
                            Quantidade = item.QtdeColuna

                        };
                        listaCantoneira3Furos.Add(cantoneira);

                    }
                    if (coluna.PassanteD)  
                    { 
                        qtdCantoneira3Furos += item.QtdeColuna; item.bPassante = true;

                        var point1 = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                        var point2 = new Point2d(coluna.PointD.X, coluna.PointD.Y);
                        var comprimento = point1.GetDistanceTo(point2);

                        var cantoneira = new Cantoneira
                        {
                            Comprimento = (comprimento * _escala) - distanciaSapata,
                            Quantidade = item.QtdeColuna

                        };

                        listaCantoneira3Furos.Add(cantoneira);

                    }

                }

                var arquivoExcel =new  List<Planilha>();

                #region >> Dados Coluna
                var especificacaoColuna = (  from item in especificacoes
                                                 where item.Peca == "COLUNA"
                                              select new Especificao
                                              {
                                                  Peca =item.Peca,
                                                  Descricao =item.Descricao,
                                                  Nomenclatura = item.Nomenclatura,
                                                  Observacao = item.Observacao   
                                              }
                                           ).FirstOrDefault();

                var especificacaoColunaPassante =  (  from item in especificacoes
                                                      where item.Peca == "COLUNAPASSANTE"
                                                   select new Especificao
                                                   {
                                                       Peca =item.Peca,
                                                       Descricao =item.Descricao,
                                                       Nomenclatura = item.Nomenclatura,
                                                       Observacao = item.Observacao   
                                                   }
                                                ).FirstOrDefault();

                var relatorioColuna = (from item in dadosRelatorio
                                 group item by new
                                 {
                                     item.Altura,
                                     item.Comprimento,
                                     item.Largura,
                                     item.Enrijecedor,
                                     item.bPassante
                                 } into c
                                 select new ItemRelatorio
                                 {
                                     Altura = c.Key.Altura,
                                     Comprimento = c.Key.Comprimento,
                                     Largura = c.Key.Largura / fatorTpRelatorio,
                                     Enrijecedor = c.Key.Enrijecedor,
                                     bPassante = c.Key.bPassante
                                 }).ToList();


                foreach (ItemRelatorio item in relatorioColuna)
                {
                    var qtde = (from coluna in dadosRelatorio
                                where coluna.Comprimento == item.Comprimento &&
                                      coluna.Largura == item.Largura * fatorTpRelatorio &&
                                      coluna.bPassante == item.bPassante
                                select coluna.QtdeColuna).Sum();

          
                    if (item.bPassante)
                    {
                        var iAlturaViga = (from coluna in dadosRelatorio
                                           where coluna.Comprimento == item.Comprimento &&
                                                 coluna.Largura == item.Largura * fatorTpRelatorio &&
                                                 coluna.bPassante == item.bPassante
                                           select coluna.AlturaViga).FirstOrDefault();

                        var alturaviga = new Cantoneira { 
                            AlturaViga = iAlturaViga == 0 ? 1 : iAlturaViga,
                            Quantidade = qtde * 2
                        };
                        listaCantoneiraViga.Add(alturaviga);
                        qtdColunaPassante = qtdColunaPassante + qtde;
                    }

                    var linha = new Planilha();
                    linha.Peca = item.bPassante ? especificacaoColunaPassante.Peca.ToString() : especificacaoColuna.Peca.ToString();
                    linha.Item = item.bPassante ? especificacaoColunaPassante.Descricao.ToString() : especificacaoColuna.Descricao.ToString();
                    linha.Especificao = item.bPassante ? string.Concat(especificacaoColunaPassante.Nomenclatura.ToString(), " ", item.Comprimento.ToString("N2"), " x ", item.Largura.ToString("N2"), "x 2,0") 
                                                       : string.Concat(especificacaoColuna.Nomenclatura.ToString(), " ", item.Comprimento.ToString("N2"), " x ", item.Largura.ToString("N2"), "x 2,0")
                                                       ;
                              
                    linha.Comprimento= item.Altura.ToString("N2");
                    linha.Quantidade = qtde * fatorTpRelatorio;
                    linha.Observacao = item.bPassante ? especificacaoColunaPassante.Observacao.ToString() : especificacaoColuna.Observacao.ToString();
                    arquivoExcel.Add(linha);
                }

                #endregion

                #region >> Dados Sapata

                var especificacaoSapata = (from item in especificacoes
                                           where item.Peca == "SAPATA"
                                           select new Especificao
                                           {
                                               Peca = item.Peca,
                                               Descricao = item.Descricao,
                                               Nomenclatura = item.Nomenclatura,
                                               Observacao = item.Observacao
                                           }).FirstOrDefault();

                var relatorioSapata = (from item in listaSapata
                                       group item by new
                                       {
                                           item.Comprimento
                                       } into c
                                       select new Sapata
                                       {
                                           Comprimento = Convert.ToDouble(c.Key.Comprimento),
                                           Quantidade = c.Count()
                                       }).ToList();


                double qtdeChumbador = 0;
                foreach (Sapata item in relatorioSapata)
                {
                    var qtde = (from sapata in listaSapata
                                where sapata.Comprimento == item.Comprimento
                                select sapata.Quantidade).Sum();

                    var linha = new Planilha();
                    linha.Peca        = especificacaoSapata.Peca.ToUpper();
                    linha.Item        = especificacaoSapata.Descricao.ToUpper();
                    linha.Comprimento = item.Comprimento.ToString("N2");
                    linha.Quantidade  = qtde;
                    linha.Especificao = especificacaoSapata.Nomenclatura.ToUpper();
                    linha.Observacao = especificacaoSapata.Observacao.ToUpper();
                    arquivoExcel.Add(linha);

                    qtdeChumbador = qtdeChumbador + qtde;
                }

                var especificacaoChumbador = (from item in especificacoes
                                              where item.Peca == "CHUMBADOR"
                                              select new Especificao
                                              {
                                                  Peca = item.Peca,
                                                  Descricao = item.Descricao,
                                                  Nomenclatura = item.Nomenclatura,
                                                  Observacao = item.Observacao
                                              }).FirstOrDefault();

                var linhaChumbador = new Planilha();
                linhaChumbador.Peca = especificacaoChumbador.Peca.ToUpper();
                linhaChumbador.Item = especificacaoChumbador.Descricao.ToUpper();
                linhaChumbador.Especificao = especificacaoChumbador.Nomenclatura.ToString();
                linhaChumbador.Comprimento = "-";
                linhaChumbador.Quantidade = qtdeChumbador;
                linhaChumbador.Observacao = especificacaoChumbador.Observacao.ToString();
                arquivoExcel.Add(linhaChumbador);

                #endregion

                #region >> Dados Parafuso

                var qtdParafuso = (from item in listaParafuso
                                         select item.Quantidade
                                          ).Sum();

                var especificacaoParafuso = (from item in especificacoes
                                             where item.Peca == "PARAFUSO"
                                             select new Especificao
                                             {
                                                 Peca = item.Peca,
                                                 Descricao = item.Descricao,
                                                 Nomenclatura = item.Nomenclatura,
                                                 Observacao = item.Observacao
                                             }).FirstOrDefault();

                var linhaParafuso = new Planilha();
                linhaParafuso.Peca = especificacaoParafuso.Peca.ToUpper();
                linhaParafuso.Item = especificacaoParafuso.Descricao.ToUpper();
                linhaParafuso.Especificao = especificacaoParafuso.Nomenclatura.ToString();
                linhaParafuso.Comprimento = "";
                linhaParafuso.Quantidade = qtdParafuso;
                linhaParafuso.Observacao = especificacaoParafuso.Observacao.ToUpper();
                arquivoExcel.Add(linhaParafuso);

                #endregion

                #region >> Dados Passante

                if (listaCantoneiraViga.Count > 0)
                {
                    var especificacaoCantoneira = (from item in especificacoes
                                                   where item.Peca == "CANTONEIRA"
                                                   select new Especificao
                                                   {
                                                       Peca = item.Peca,
                                                       Descricao = item.Descricao,
                                                       Nomenclatura = item.Nomenclatura,
                                                       Observacao = item.Observacao
                                                   }).FirstOrDefault();

                    var relatorioCantoneira3Furos = (from item in listaCantoneira3Furos
                                               group item by new
                                               {
                                                   item.Comprimento,
                                               } into c
                                               select new Cantoneira
                                               {
                                                   Comprimento = c.Key.Comprimento  ,
                                               }).ToList();

                    foreach (Cantoneira item in relatorioCantoneira3Furos)
                    {
                        var quantidade = (from lista in listaCantoneira3Furos
                                          where item.Comprimento == lista.Comprimento
                                          select lista.Quantidade).Sum();

                        var linha = new Planilha();
                        linha.Peca = especificacaoCantoneira.Peca.ToUpper();
                        linha.Item = especificacaoCantoneira.Descricao.ToUpper();
                        linha.Especificao = especificacaoCantoneira.Nomenclatura.ToUpper();
                        linha.Comprimento = ( item.Comprimento ).ToString("N2");
                        linha.Quantidade = quantidade;
                        linha.Observacao = especificacaoCantoneira.Observacao.ToUpper();
                        arquivoExcel.Add(linha);
                    }

                    var especificacaoAutobrocante = (from item in especificacoes
                                                     where item.Peca == "AUTOBROCANTE"
                                                     select new Especificao
                                                     {
                                                         Peca = item.Peca,
                                                         Descricao = item.Descricao,
                                                         Nomenclatura = item.Nomenclatura,
                                                         Observacao = item.Observacao
                                                     }).FirstOrDefault();

                    var linhaAutobrocante = new Planilha();
                    linhaAutobrocante.Peca = especificacaoAutobrocante.Peca.ToUpper();
                    linhaAutobrocante.Item = especificacaoAutobrocante.Descricao.ToUpper();
                    linhaAutobrocante.Especificao = especificacaoCantoneira.Nomenclatura.ToString();
                    linhaAutobrocante.Comprimento = "";
                    linhaAutobrocante.Quantidade = qtdColunaPassante * 7;
                    linhaAutobrocante.Observacao = especificacaoCantoneira.Observacao.ToString();
                    arquivoExcel.Add(linhaAutobrocante);

                    var relatorioCantoneira = (from item in listaCantoneiraViga
                                               group item by new
                                               {
                                                   item.AlturaViga,
                                               } into c
                                               select new Cantoneira
                                               {
                                                   AlturaViga = c.Key.AlturaViga,
                                                   Quantidade = c.Count()
                                               }).ToList();

                    var especificacaoCantoneiraPassante = (from item in especificacoes
                                                   where item.Peca == "CANTONEIRAPASSANTE"
                                                   select new Especificao
                                                   {
                                                       Peca = item.Peca,
                                                       Descricao = item.Descricao,
                                                       Nomenclatura = item.Nomenclatura,
                                                       Observacao = item.Observacao
                                                   }).FirstOrDefault();

                    foreach (Cantoneira item in relatorioCantoneira)
                    {
                        var quantidade = (from lista in listaCantoneiraViga
                                          where item.AlturaViga == lista.AlturaViga
                                          select lista.Quantidade).Sum();


                        var linha = new Planilha();
                        linha.Peca = especificacaoCantoneiraPassante.Peca.ToUpper();
                        linha.Item = especificacaoCantoneiraPassante.Descricao.ToUpper();
                        linha.Especificao = especificacaoCantoneiraPassante.Nomenclatura.ToUpper();
                        linha.Comprimento = (item.AlturaViga + 200).ToString("N2");
                        linha.Quantidade = item.Quantidade * quantidade;
                        linha.Observacao = especificacaoCantoneiraPassante.Observacao.ToUpper();
                        arquivoExcel.Add(linha);
                    }

                }

                #endregion

                Helpers.GeraArquivoExcel(arquivoExcel, nomeProjeto, tipoLista);

            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

  
        private static Sapata GetSapata(Point2d point1, Point2d point2)
        {
            //distanciaSapata
            var sapata = new Sapata();
            sapata.Comprimento = point1.GetDistanceTo(point2) * _escala;
            sapata.Comprimento = sapata.Comprimento - distanciaSapata;
            return sapata;
        }

        public static void CreateLayout()

        {

            var document = Application.DocumentManager.MdiActiveDocument;
            Editor ed = document.Editor;
            Database db = document.Database;
            Point3d first, second;
            Point2d min, max;
            PromptPointOptions ppo =

                new PromptPointOptions("/nSelect first corner of plot area: ");

            ppo.AllowNone = false;

            PromptPointResult ppr = ed.GetPoint(ppo);


            first = ppr.Value;
            max = new Point2d(first.X, first.Y);
            
            PromptCornerOptions pco =
             new PromptCornerOptions(
             "/nSelect second corner of plot area: ",
             first
             );
            
            ppr = ed.GetCorner(pco);
            
            if (ppr.Status == PromptStatus.OK)
            {
                second = ppr.Value;
                min = new Point2d(second.X, second.Y);
            }
            else
            {
                min = new Point2d(0, 0);
            }

            if (document == null)
                return;

            var database = document.Database;
            var editor = document.Editor;
            var extents2d = new Extents2d();

            using (var transaction = database.TransactionManager.StartTransaction())
            {

                var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("NewLayout");
                var lay = (Layout)transaction.GetObject(id, OpenMode.ForWrite);
                

                // Make some settings on the layout and get its extents

                string pageSize = "A4";
                string styleSheet = "monochrome.ctb";
                string device = "DWFG To PDF.pc3";

                using (var ps = new PlotSettings(lay.ModelType))
                {

                    ps.CopyFrom(lay);

                    var psv = PlotSettingsValidator.Current;

                    // Set the device

                    var devs = psv.GetPlotDeviceList();

                    if (devs.Contains(device))
                    {
                        psv.SetPlotConfigurationName(ps, device, null);
                        psv.RefreshLists(ps);

                    }

                    // Set the media name/size

                    var mns = psv.GetCanonicalMediaNameList(ps);

                    if (mns.Contains(pageSize))
                    {
                        psv.SetCanonicalMediaName(ps, pageSize);
                    }

                    // Set the pen settings

                    var ssl = psv.GetPlotStyleSheetList();
                    if (ssl.Contains(styleSheet))
                    {
                        psv.SetCurrentStyleSheet(ps, styleSheet);
                    }

                    // Copy the PlotSettings data back to the Layout

                    var upgraded = false;

                    if (!lay.IsWriteEnabled)
                    {
                        lay.UpgradeOpen();
                        upgraded = true;
                    }

                    lay.CopyFrom(ps);

                    if (upgraded)
                    {
                        lay.DowngradeOpen();
                    }
                }

                extents2d = new Extents2d(min, max); // lay.GetMaximumExtents();


                lay.ApplyToViewport(transaction, 2,
                  vp =>
                  {

                      // Size the viewport according to the extents calculated when
                      // we set the PlotSettings (device, page size, etc.)
                      // Use the standard 10% margin around the viewport
                      // (found by measuring pixels on screenshots of Layout1, etc.)

                      vp.ResizeViewport(extents2d, 0.8);


                      // Adjust the view so that the model contents fit

                      if (ValidDbExtents(database.Extmin, database.Extmax))

                      {

                          vp.FitContentToViewport(new Extents3d(database.Extmin, database.Extmax), 0.9);

                      }



                      // Finally we lock the view to prevent meddling



                      vp.Locked = true;

                  }

                );



                // Commit the transaction



                transaction.Commit();

            }



            // Zoom so that we can see our new layout, again with a little padding



            //editor.Command("_.ZOOM", "_E");

            //editor.Command("_.ZOOM", ".7X");

            editor.Regen();

        }


        public static void ResizeViewport(this Viewport vp, Extents2d ext, double fac = 1.0)
        {

            vp.Width = 142.705;                         //((ext.MaxPoint.X > ext.MinPoint.X ? ext.MaxPoint.X - ext.MinPoint.X : ext.MinPoint.X - ext.MaxPoint.X ) * fac  ) /1000;
            vp.Height = 341.73;                       // ((ext.MaxPoint.Y > ext.MinPoint.Y ? ext.MaxPoint.Y - ext.MinPoint.Y : ext.MinPoint.Y - ext.MaxPoint.Y ) * fac ) /1000;
            vp.CenterPoint = new Point3d(ext.MinPoint.X + ext.MinPoint.GetDistanceTo(ext.MaxPoint) /2, ext.MinPoint.Y + ext.MinPoint.GetDistanceTo(ext.MaxPoint) / 2, 0)  ;
            
        }
        public static void ApplyToViewport(this Layout lay, Transaction tr, int vpNum, Action<Viewport> f)
        {

            var vpIds = lay.GetViewports();

            Viewport vp = null;

            foreach (ObjectId vpId in vpIds)
            {

                var vp2 = tr.GetObject(vpId, OpenMode.ForWrite) as Viewport;
                if (vp2 != null && vp2.Number == vpNum)
                {

                    // We have found our viewport, so call the action
                    vp = vp2;
                    break;
                }
            }

            if (vp == null)
            {

                // We have not found our viewport, so create one

                var btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);

                vp = new Viewport();

                // Add it to the database

                btr.AppendEntity(vp);

                tr.AddNewlyCreatedDBObject(vp, true);

                // Turn it - and its grid - on

                vp.On = true;
                vp.GridOn = true;
            }

            // Finally we call our function on it
            f(vp);

        }

        public static void FitContentToViewport(this Viewport vp, Extents3d ext, double fac = 1.0)
        {

            // Let's zoom to just larger than the extents

            vp.ViewCenter = new Point2d(ext.MinPoint.X + ext.MinPoint.DistanceTo(ext.MaxPoint) / 2, ext.MinPoint.Y + ext.MinPoint.DistanceTo(ext.MaxPoint) / 2);  //(ext.MinPoint + ((ext.MaxPoint - ext.MinPoint) * 0.5)).Strip();


            var hgt = 297; // ext.MaxPoint.Y - ext.MinPoint.Y;

            vp.ViewHeight = hgt;
            

     
            vp.CustomScale *= 108;
        }


        public static Point2d Strip(this Point3d pt)

        {

            return new Point2d(pt.X, pt.Y);

        }

        private static bool ValidDbExtents(Point3d min, Point3d max)
        {
            return

              !(min.X > 0 && min.Y > 0 && min.Z > 0 &&
                max.X < 0 && max.Y < 0 && max.Z < 0);
        }
        #endregion

    }

}
