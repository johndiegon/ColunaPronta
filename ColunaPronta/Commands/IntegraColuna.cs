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

namespace ColunaPronta.Commands
{
    public static class IntegraColuna 
    {
        #region >> Propriedades

        const double _escala = 1000;
        const double distancia = 38 / _escala;
        #endregion

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

            return coluna;
        }
        public static void AddColuna(Coluna coluna)
        {
            
            if (coluna.ParafusoA == true ) { AddParafuso("A", coluna);}
            if (coluna.ParafusoB == true ) { AddParafuso("B", coluna);}
            if (coluna.ParafusoC == true ) { AddParafuso("C", coluna);}
            if (coluna.ParafusoD == true ) { AddParafuso("D", coluna);}
            if (coluna.ParafusoE == true ) { AddParafuso("E", coluna);}
            if (coluna.ParafusoF == true ) { AddParafuso("F", coluna);}
            if (coluna.ParafusoG == true ) { AddParafuso("G", coluna);}
            if (coluna.ParafusoH == true ) { AddParafuso("H", coluna);}

            if (coluna.SapataA   == true ) 
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p1 = new Point2d(coluna.PointA.X, coluna.PointA.Y + distancia); 
                var p2 = new Point2d(coluna.PointB.X, coluna.PointB.Y + distancia);
                var p3 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p4 = new Point2d(coluna.PointA.X, coluna.PointA.Y);

                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);

            }
            if (coluna.SapataB   == true )
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p1 = new Point2d(coluna.PointB.X            , coluna.PointB.Y);
                var p2 = new Point2d(coluna.PointB.X + distancia, coluna.PointB.Y);
                var p3 = new Point2d(coluna.PointB.X + distancia, coluna.PointC.Y);
                var p4 = new Point2d(coluna.PointB.X, coluna.PointC.Y);
                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);

            }
            if (coluna.SapataC   == true)
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p4 = new Point2d(coluna.PointC.X, coluna.PointC.Y - distancia); 
                var p3 = new Point2d(coluna.PointD.X, coluna.PointD.Y - distancia);                 
                var p1 = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                var p2 = new Point2d(coluna.PointD.X, coluna.PointD.Y); 
                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataD == true)
            {

                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p1 = new Point2d(coluna.PointA.X - distancia, coluna.PointA.Y);
                var p2 = new Point2d(coluna.PointA.X            , coluna.PointA.Y);
                var p3 = new Point2d(coluna.PointA.X            , coluna.PointD.Y);
                var p4 = new Point2d(coluna.PointA.X - distancia, coluna.PointD.Y);
                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);
            }

        }
        public static Point3dCollection AddEstruturaColuna(Document document, Point2d startPoint, double largura, double comprimento)
        {
            var pontosColuna = new Point3dCollection();

            #region >> Estrutura Lado A ( Voltado para Direita  )
            var LadoA = new Point2dCollection();

            // Ponto A
            LadoA.Add(new Point2d(startPoint.X                                     , startPoint.Y                                      ));
            pontosColuna.Add((new Point3d(startPoint.X, startPoint.Y, 0)));
            // Ponto B                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X                                     , startPoint.Y - ( comprimento / _escala )          ));
            pontosColuna.Add(new Point3d(startPoint.X, startPoint.Y - (comprimento / _escala),0));
            // Ponto C                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X + ( (largura / 2) / _escala )       , startPoint.Y - ( comprimento / _escala )          ));
            // Ponto D                                                                 
            LadoA.Add(new Point2d(startPoint.X + ( (largura / 2) / _escala)        , startPoint.Y - ( (comprimento - 20 ) / _escala )  ));
            // Ponto E
            LadoA.Add(new Point2d(startPoint.X + ( ((largura / 2) - 2) / _escala)  , startPoint.Y - ( (comprimento - 20 ) / _escala )  ));
            // Ponto F
            LadoA.Add(new Point2d(startPoint.X + ( ((largura / 2) - 2) / _escala ) , startPoint.Y - ((comprimento - 2) / _escala)      ));
            // Ponto G
            LadoA.Add(new Point2d(startPoint.X + (2 / _escala)                     , startPoint.Y - ((comprimento - 2) / _escala)      ));
            // Ponto H                                                                 
            LadoA.Add(new Point2d(startPoint.X + (2 / _escala)                     , startPoint.Y - (2 / _escala)                      ));
            // Ponto I                                                                 
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala)   , startPoint.Y - (2 / _escala)                      ));
            // Ponto J                                                                 
            LadoA.Add(new Point2d(startPoint.X + (((largura / 2) - 2) / _escala)   , startPoint.Y - (20 / _escala)                     ));
            // Ponto L                                                                                                                     
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala)         , startPoint.Y - (20 / _escala)                     ));
            // Ponto M                                                                 
            LadoA.Add(new Point2d(startPoint.X + ((largura / 2) / _escala)         , startPoint.Y                                      ));
            // Ponto A                                                                 
            LadoA.Add(new Point2d(startPoint.X                                     , startPoint.Y                                      ));

            Helpers.AddPolyline(document, LadoA, ColorIndex.padrao);
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
            pontosColuna.Add(new Point3d(startPoint.X + ((largura) / _escala), startPoint.Y - (comprimento / _escala),0));
            // Ponto M                                                                 
            LadoB.Add(new Point2d(startPoint.X + ((largura) / _escala), startPoint.Y));
            pontosColuna.Add(new Point3d(startPoint.X + ((largura) / _escala), startPoint.Y,0));

            // Ponto A                                                                 
            LadoB.Add(new Point2d(startPoint.X, startPoint.Y));

            Helpers.AddPolyline(document, LadoB, ColorIndex.padrao);
            #endregion

            return pontosColuna;
        }
        public static void AddSapata(Point2d p1, Point2d p2, Point2d p3, Point2d p4, string tipocoluna, double diametro)
        {
            var raio = diametro / 2;
         
            Document document = Application.DocumentManager.MdiActiveDocument;

            Helpers.AddPolyline(document, p1, p2, p3, p4, tipocoluna);

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
        public static void AddParafuso(string TpParafuso, Coluna coluna)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;

            if (TpParafuso == "A")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X + (10 / _escala), pontoA.Y + (30 / _escala));
                var p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X , p1.Y - (10 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y );
                var p2v2 = new Point2d(p2v1.X , p2v1.Y - (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - +(50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y );

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "B")
            {
                var pontoA = new Point2d(coluna.PointB.X - (30 / _escala), coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X + (5 / _escala), pontoA.Y + (30 / _escala));
                var p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (10 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y);
                var p2v2 = new Point2d(p2v1.X, p2v1.Y - (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - +(50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "C")
            {
                var pontoA = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p1 = new Point2d(pontoA.X + (20 / _escala), pontoA.Y - (25 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y  -(20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p2.X, p2.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X , p2v1.Y - (10 / _escala));
                var p2v3 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y );

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "D")
            {
                var pontoA = new Point2d(coluna.PointB.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X + (20 / _escala), pontoA.Y + (40 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p2.X, p2.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));
                var p2v3 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y);

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "E")
            {
                var pontoA = new Point2d(coluna.PointD.X , coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X - (5 / _escala), pontoA.Y - (20 / _escala));
                var p2 = new Point2d(p1.X , p1.Y - (10 / _escala));
                var p3 = new Point2d(p1.X - (20 / _escala), p1.Y - (10 / _escala) );
                var p4 = new Point2d(p1.X - (20 / _escala), p1.Y );

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X - ( 5 / _escala), p2.Y );
                var p2v2 = new Point2d(p2v1.X , p2v1.Y + (50 / _escala));
                var p2v3 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y + (50 / _escala) );
                var p2v4 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y );

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "F")
            {
                var pontoA = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X + (5 / _escala), pontoA.Y - (20 / _escala));
                var p2 = new Point2d(p1.X, p1.Y - (10 / _escala));
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X + (20 / _escala), p1.Y);

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p2.Y);
                var p2v2 = new Point2d(p2v1.X, p2v1.Y + (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y + (50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "G")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X - (30 / _escala), pontoA.Y + ( 45 / _escala));
                var p2 = new Point2d(p1.X     + ( 10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X     + ( 10 / _escala) , p1.Y- ( 20 / _escala) );
                var p4 = new Point2d(p1.X    , p1.Y- ( 20 / _escala) );

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X, p1.Y-(5/ _escala));
                var p2v2 = new Point2d(p2v1.X + ( 50 /_escala ) , p2v1.Y);
                var p2v3 = new Point2d(p2v1.X + ( 50 / _escala) , p2v1.Y - ( 10/ _escala));
                var p2v4 = new Point2d(p2v1.X, p2v1.Y - ( 10 / _escala));

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3 );
            }
            if (TpParafuso == "H")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X - (30 / _escala), pontoA.Y - (15 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X, p1.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y);
                var p2v3 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
        }
        public static void AddTitulo(Point2d PontoA, TipoColuna tipoColuna)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            var textTipoColuna = tipoColuna.ToString();
            var point = new Point3d(PontoA.X - (20 / _escala), PontoA.Y - (5 / _escala), 0);
            Helpers.AddTexto(document, point, textTipoColuna, ColorIndex.padrao);
        }
        public static List<ItemRelatorio> GetDadosRelatorio(string nomeProjeto)
        {
            try
            {
                var colunas = ArquivoCSV.GetColunas(nomeProjeto);

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
                                 tipoColuna = coluna.tipoColuna,
                                 DiametroParafuso = coluna.DiametroParafuso,
                                 DiametroSapata = coluna.DiametroSapata,
                                 QuantidadeParafuso = coluna.QuantidadeParafuso,
                                 Comprimento = coluna.Comprimento,
                                 Largura = coluna.Largura,
                                 Altura = coluna.Altura,
                             }).FirstOrDefault();

                    colunasRelatorio.Add(c);
                }

                var relatorio = (from coluna in colunasRelatorio
                                 group coluna by new
                                 {
                                     coluna.Altura,
                                     coluna.Comprimento,
                                     coluna.Largura,
                                     coluna.tipoColuna,
                                     coluna.QuantidadeParafuso,
                                     coluna.DiametroSapata

                                 } into c
                                 select new ItemRelatorio
                                 {
                                     Altura = c.Key.Altura,
                                     Comprimento = c.Key.Comprimento,
                                     Largura = c.Key.Largura,
                                     tipoColuna = c.Key.tipoColuna,
                                     QtdeParafuso = c.Key.QuantidadeParafuso,
                                     DiametroSapata = c.Key.DiametroSapata,
                                     QtdeColuna = c.Count()
                                 }).ToList();
                return relatorio;
            }
            catch (Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static void GeraRelatorio()
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Editor editor = document.Editor;

                var nomeProjeto = document.Window.Text;

                var dadosRelatorio = GetDadosRelatorio(nomeProjeto);

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
                        var coluna = GetColunaModelo(item.tipoColuna);
                        coluna.Largura = item.Largura;
                        coluna.Comprimento = item.Comprimento;
                        coluna.Altura = item.Altura;
                        coluna.DiametroSapata = item.DiametroSapata;
                        coluna.tipoColuna = item.tipoColuna;
                        var pontosColuna = AddEstruturaColuna(document, new Point2d(startX, startY - (distancia / _escala)), item.Largura, item.Comprimento);

                        coluna.SetPontos(pontosColuna);

                        AddColuna(coluna);

                        var textoDescricao = string.Concat( coluna.tipoColuna.ToString(), " - "
                                                          , coluna.Comprimento.ToString("N2"),"x"
                                                          , coluna.Largura.ToString("N2"), "x"
                                                          , coluna.Altura.ToString("N2"), "mm - "
                                                          , item.QtdeColuna.ToString(), " "
                                                          , item.QtdeColuna == 1 ? "unidade." : "unidades."
                                                          );

                        Helpers.AddTexto( document
                                        , new Point3d(startX+ ( (item.Largura+60) / _escala) , startY - ( ( 50 + distancia) / _escala),0)
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
        public static Coluna GetColunaModelo(TipoColuna tipoColuna)
        {
            var coluna = new Coluna();

            switch(tipoColuna)
            {
                case TipoColuna.C1:
                    coluna.SapataB = true;
                    coluna.ParafusoB = true;
                    coluna.ParafusoH = true;
                    break;
                case TipoColuna.C2:
                    coluna.SapataB = true;
                    coluna.ParafusoE = true;
                    coluna.ParafusoG = true;
                    break;
                case TipoColuna.C3:
                    coluna.SapataB = true;
                    coluna.ParafusoB = true;
                    coluna.ParafusoE = true;
                    coluna.ParafusoG = true;
                    coluna.ParafusoH = true;
                    break;
                case TipoColuna.C4:
                    coluna.SapataB = true;
                    coluna.ParafusoA = true;
                    coluna.ParafusoC = true;
                    coluna.ParafusoD = true;
                    coluna.ParafusoF = true;
                    break;
                case TipoColuna.C5:
                    coluna.SapataB = true;
                    coluna.ParafusoB = true;
                    coluna.ParafusoG = true;
                    break;
                case TipoColuna.C6:
                    coluna.SapataB = true;
                    coluna.ParafusoE = true;
                    coluna.ParafusoH = true;
                    break;
                case TipoColuna.C8:
                    coluna.SapataB = true;
                    coluna.ParafusoA = true;
                    coluna.ParafusoB = true;
                    break;
                case TipoColuna.C9:
                    coluna.SapataB = true;
                    break;
                case TipoColuna.C12:
                    coluna.SapataC = true;
                    coluna.ParafusoE = true;
                    break;
                case TipoColuna.C13:
                    coluna.SapataC = true;
                    coluna.ParafusoF = true;
                    break;
                default: 
                    return coluna;
            }
            return coluna;
        }
    }
}
