﻿using Autodesk.AutoCAD.ApplicationServices;
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
        const int iLinhasParafuso = 15;
        const int ladoEle = 20;
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
            else
            {
                editor.WriteMessage("\nÉ necessário selecionar a coluna de referência.");
                return null;
            }

            return coluna;
        }
    
        public static void AddColuna(Coluna coluna)
        {
            
             AddParafuso(coluna);
             AddPassante(coluna);
             AddEle(coluna);

            if (coluna.SapataA == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataD == true && coluna.Posicao == Posicao.Vertical  ) 
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p1 = new Point2d(coluna.PointA.X, coluna.PointA.Y + distancia); 
                var p2 = new Point2d(coluna.PointB.X, coluna.PointB.Y + distancia);
                var p3 = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p4 = new Point2d(coluna.PointA.X, coluna.PointA.Y);

                AddSapata(p1, p2, p3, p4, "c1", coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataB == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataA == true && coluna.Posicao == Posicao.Vertical  )
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p1 = new Point2d(coluna.PointB.X            , coluna.PointB.Y);
                var p2 = new Point2d(coluna.PointB.X + distancia, coluna.PointB.Y);
                var p3 = new Point2d(coluna.PointB.X + distancia, coluna.PointC.Y);
                var p4 = new Point2d(coluna.PointB.X, coluna.PointC.Y);
                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);

            }
            if (coluna.SapataC == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataB == true && coluna.Posicao == Posicao.Vertical  )
            {
                string tipocoluna = coluna.GetTipoColuna().ToString();
                var p4 = new Point2d(coluna.PointC.X, coluna.PointC.Y - distancia); 
                var p3 = new Point2d(coluna.PointD.X, coluna.PointD.Y - distancia);                 
                var p1 = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                var p2 = new Point2d(coluna.PointD.X, coluna.PointD.Y); 
                AddSapata(p1, p2, p3, p4, tipocoluna, coluna.DiametroSapata / _escala);
            }
            if (coluna.SapataD == true && coluna.Posicao == Posicao.Horizontal || coluna.SapataC == true && coluna.Posicao == Posicao.Vertical  )
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
        
        public static void AddParafuso(Coluna coluna)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;

            if ((coluna.ParafusoA == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoG == true && coluna.Posicao == Posicao.Vertical))
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X + (10 / _escala), pontoA.Y + (30 / _escala));
                var p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (10 / _escala));

                Helpers.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y);
                var p2v2 = new Point2d(p2v1.X, p2v1.Y - (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - (50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                Helpers.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y - ((50 - i) / _escala), 0), new Point3d(p2v1.X + (10 / _escala), p2v1.Y - ((50 - i) / _escala), 0), false, ColorIndex.verde);
                }
                //var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                //AddParafusoAB(document, pontoA);

            }
            if ((coluna.ParafusoB == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoH == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y - ((50 - i) / _escala), 0), new Point3d(p2v1.X + (10 / _escala), p2v1.Y - ((50 - i) / _escala), 0), false, ColorIndex.verde);
                }

                //var pontoA = new Point2d(coluna.PointB.X - (30 / _escala), coluna.PointA.Y);
                //AddParafusoAB(document, pontoA);

            }
            if ((coluna.ParafusoC == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoA == true && coluna.Posicao == Posicao.Vertical))
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


                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X - (( 50 - i) / _escala), p2v1.Y - (10 / _escala), 0), new Point3d(p2v1.X - ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                }
            }
            if ((coluna.ParafusoD == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoB == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X - ((50 - i) / _escala), p2v1.Y - (10 / _escala), 0), new Point3d(p2v1.X - ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                }
            }
            if ((coluna.ParafusoE == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoC == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y + ((50 - i) / _escala), 0),
                                               new Point3d(p2v1.X - (10 / _escala), p2v1.Y + ((50 - i) / _escala), 0), false, ColorIndex.verde);
                }
            }
            if ((coluna.ParafusoF == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoD == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X, p2v1.Y + (( 50 -i) / _escala), 0),
                                               new Point3d(p2v1.X + (10 / _escala), p2v1.Y + ((50 - i) / _escala), 0), false, ColorIndex.verde);
                }
            }
            if ((coluna.ParafusoG == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoE == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y - (10 / _escala), 0),
                                             new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                }
            }
            if ((coluna.ParafusoH == true && coluna.Posicao == Posicao.Horizontal) || (coluna.ParafusoF == true && coluna.Posicao == Posicao.Vertical))
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

                for (int i = 1; i < iLinhasParafuso; i++)
                {
                    Helpers.AddLinha(document, new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y - (10 / _escala), 0),
                                               new Point3d(p2v1.X + ((50 - i) / _escala), p2v1.Y, 0), false, ColorIndex.verde);
                }
            }
      
        }
      
        public static void AddPassante(Coluna coluna)
        {
           if(     (coluna.PassanteA == true && coluna.Posicao == Posicao.Horizontal) 
                || (coluna.PassanteD == true && coluna.Posicao == Posicao.Vertical)
             ) 
            {

                var PontoA = new Point2d(coluna.PointA.X , coluna.PointA.Y );
                AddRetangulo(PontoA, Posicao.Horizontal, 40 , coluna.Largura);
            }

           if(     (coluna.PassanteB == true && coluna.Posicao == Posicao.Horizontal) || 
                   (coluna.PassanteA == true && coluna.Posicao == Posicao.Vertical)
             )
            {
                var PontoA = new Point2d(coluna.PointB.X - (40/_escala) , coluna.PointB.Y);
                AddRetangulo(PontoA,  Posicao.Vertical, 40 , coluna.Comprimento);
            }

           if(     (coluna.PassanteC == true && coluna.Posicao == Posicao.Horizontal) || 
                   (coluna.PassanteB == true && coluna.Posicao == Posicao.Vertical)
             ) 
            {
                var PontoA = new Point2d(coluna.PointC.X , coluna.PointC.Y + (40 / _escala));
                AddRetangulo(PontoA,  Posicao.Horizontal, 40 , coluna.Largura);
            }

           if(     (coluna.PassanteD == true && coluna.Posicao == Posicao.Horizontal) || 
                   (coluna.PassanteC == true && coluna.Posicao == Posicao.Vertical)
             ) 
            {
                
                var PontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                AddRetangulo(PontoA, Posicao.Vertical, 40 , coluna.Comprimento);
            }
        }

        public static void AddRetangulo(Point2d PontoA, Posicao posicao, double largura, double comprimento)
        {
            Point2d p1, p2, p3, p4 = new Point2d();
            var collection = new Point2dCollection();

            if ( posicao == Posicao.Vertical)
            {
                p1 = new Point2d(PontoA.X, PontoA.Y);
                p2 = new Point2d(PontoA.X + ( largura / _escala ), PontoA.Y);
                p3 = new Point2d(PontoA.X + (largura / _escala)  , PontoA.Y - (comprimento / _escala) );
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
           if( (coluna.eleVermelho == true && coluna.Posicao == Posicao.Horizontal) || 
               (coluna.eleCinza    == true && coluna.Posicao == Posicao.Vertical) )
            {
                AddElePolyline(new Point2d(coluna.PointA.X + (ladoEle / _escala), coluna.PointA.Y + (ladoEle / _escala)), Posicao.BaixoDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointC.X , coluna.PointC.Y), Posicao.CimaDireita, ladoEle);
            }
           if( (coluna.eleAmarelo   == true && coluna.Posicao == Posicao.Horizontal) || 
               (coluna.eleVermelho  == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointA.X, coluna.PointA.Y), Posicao.BaixoDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointB.X , coluna.PointB.Y), Posicao.BaixoEsquerda, ladoEle);
           
            }
            if ( (coluna.eleAzul     == true && coluna.Posicao == Posicao.Horizontal) || 
               (coluna.eleAmarelo  == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointB.X - (ladoEle / _escala), coluna.PointB.Y + (ladoEle / _escala)), Posicao.BaixoEsquerda, ladoEle);
                AddElePolyline(new Point2d(coluna.PointD.X - (ladoEle / _escala), coluna.PointD.Y), Posicao.CimaEsquerda, ladoEle);
            }
            if ( (coluna.eleCinza == true && coluna.Posicao == Posicao.Horizontal) || 
               (coluna.eleAzul  == true && coluna.Posicao == Posicao.Vertical))
            {
                AddElePolyline(new Point2d(coluna.PointC.X - (ladoEle / _escala), coluna.PointC.Y + (ladoEle / _escala)), Posicao.CimaDireita, ladoEle);
                AddElePolyline(new Point2d(coluna.PointD.X , coluna.PointD.Y + (ladoEle / _escala)), Posicao.CimaEsquerda, ladoEle);
            }
        }

        public static void AddElePolyline(Point2d PontoA, Posicao posicao, double lado)
        {
            Point2d p1, p2, p3, p4, p5, p6 = new Point2d();
            var collection = new Point2dCollection();
            bool bPosicaoInvalida = false;

            switch(posicao)
            {
                case Posicao.BaixoDireita:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X, PontoA.Y - (lado / _escala));
                    p3 = new Point2d(PontoA.X - (lado/ _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X - (lado / _escala), PontoA.Y - ( (lado -2) / _escala));
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
                case Posicao.CimaDireita:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y );
                    p3 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X + ( (lado - 2) / _escala ), PontoA.Y - ((lado) / _escala));
                    p5 = new Point2d(PontoA.X + ( (lado - 2) / _escala ), PontoA.Y - ( 2 / _escala));
                    p6 = new Point2d(PontoA.X , PontoA.Y - (2 / _escala));
                    break;
                case Posicao.CimaEsquerda:
                    p1 = new Point2d(PontoA.X, PontoA.Y);
                    p2 = new Point2d(PontoA.X, PontoA.Y - (lado / _escala));
                    p3 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y - (lado / _escala));
                    p4 = new Point2d(PontoA.X + (2 / _escala), PontoA.Y - ( 2 / _escala));
                    p5 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y - ( 2 / _escala));
                    p6 = new Point2d(PontoA.X + (lado / _escala), PontoA.Y);
                    break;
                default: 
                    bPosicaoInvalida = true;
                    p1 = new Point2d(0, 0);
                    p2 = new Point2d(0,0);
                    p3 = new Point2d(0,0);
                    p4 = new Point2d(0,0);
                    p5 = new Point2d(0,0);
                    p6 = new Point2d(0,0);
                    break;
            }

            if(!bPosicaoInvalida)
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
        public static List<ItemRelatorio> GetDadosRelatorio(string nomeProjeto)
        {
            try
            {
                var colunas = ArquivoCSV.GetColunas(nomeProjeto);

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
                                         coluna.iColuna,
                                         coluna.QuantidadeParafuso,
                                         coluna.DiametroSapata,
                                         coluna.DiametroParafuso

                                     } into c
                                     select new ItemRelatorio
                                     {
                                         Altura = c.Key.Altura,
                                         Comprimento = c.Key.Comprimento,
                                         Largura = c.Key.Largura,
                                         iColuna = c.Key.iColuna,
                                         QtdeParafuso = c.Key.QuantidadeParafuso,
                                         DiametroSapata = c.Key.DiametroSapata,
                                         DiametroParafuso = c.Key.DiametroParafuso,
                                         QtdeColuna = c.Count()
                                     }).ToList();
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

        public static void GeraRelatorio()
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Editor editor = document.Editor;

                var nomeProjeto = document.Window.Text;

                var dadosRelatorio = GetDadosRelatorio(nomeProjeto);

                if ( dadosRelatorio == null)
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
                        var coluna = IntegraLayout.GetLayout(item.iColuna) ;
                        coluna.Largura = item.Largura;
                        coluna.Comprimento = item.Comprimento;
                        coluna.Altura = item.Altura;
                        coluna.DiametroSapata = item.DiametroSapata;
                        coluna.DiametroParafuso = item.DiametroParafuso;
                        var pontosColuna = AddEstruturaColuna(document, new Point2d(startX, startY - (distancia / _escala)), item.Largura, item.Comprimento);

                        coluna.SetPontos(pontosColuna);

                        AddColuna(coluna);

                        var textoDescricao = string.Concat( "C", coluna.iColuna.ToString(), " - "
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
       
    }
}
