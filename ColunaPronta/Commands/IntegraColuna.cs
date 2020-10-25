using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using ColunaPronta.Helper;
using System.Collections.Generic;
using System.Linq;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Windows;
using System.Threading;

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

            List<double> ListaY = new List<double>();
            List<double> ListaX = new List<double>();
            double x;
            double y;

            PromptSelectionResult psr = editor.GetSelection();

            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction tr = database.TransactionManager.StartTransaction())
                {
                
                    SelectionSet selectionSet = psr.Value;
                    ObjectId[] objectIds = selectionSet.GetObjectIds();

                    foreach (ObjectId objectId in objectIds)
                    {
                        var dbo = tr.GetObject(objectId, OpenMode.ForRead);
                        var poly = (Polyline)dbo;
                        if (poly != null)
                        {
                            var points = new Point3dCollection();
                            for (int i = 0; i < poly.NumberOfVertices; i++)
                            {
                                points.Add(poly.GetPoint3dAt(i));
                            }

                            foreach (Point3d point in points)
                            {
                                ListaX.Add(point.X);
                                ListaY.Add(point.Y);
                            }
                        }
                    }
                    tr.Commit();
                }
            }

            if ( ListaX.Count > 0  && ListaY.Count > 0)
            {
                // Ponto A
                x = ListaX.Min();
                y = ListaY.Max();
                coluna.PointA = new Point2d(x, y);

                // Ponto B
                x = ListaX.Max();
                y = ListaY.Max();
                coluna.PointB = new Point2d(x, y);

                // Ponto C
                x = ListaX.Min();
                y = ListaY.Min();
                coluna.PointC = new Point2d(x, y);

                // Ponto D
                x = ListaX.Max();
                y = ListaY.Min();
                coluna.PointD = new Point2d(x, y);
            }

            return coluna;
        }
        public static void AddColuna(Coluna coluna)
        {
            AddTitulo(coluna.PointA, coluna.GetTipoColuna());
            
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

            ArquivoCSV.Registra(coluna);

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
            var point = new Point3d(PontoA.X - (5 / _escala), PontoA.Y - (5 / _escala), 0);
            Helpers.AddTexto(document, point, textTipoColuna, ColorIndex.padrao);
        }
     
        public static List<Coluna> GetColunas()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;

            return ArquivoCSV.GetColunas(document.Window.Text);
        }


        public static List<ItemRelatorio> GeraRelatorio()
        {
            var colunas = GetColunas();

            var colunasPonto = from coluna in colunas
                                  group coluna by coluna.PointA into  colunaPontoA
                                 select colunaPontoA.Key;
            var colunasRelatorio = new List<Coluna>();

            foreach( Point2d pontoColuna in colunasPonto)
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
                                coluna.QuantidadeParafuso

                            } into c
                            select new ItemRelatorio
                            {
                                Altura = c.Key.Altura,
                                Comprimento = c.Key.Comprimento,
                                Largura = c.Key.Largura,
                                tipoColuna = c.Key.tipoColuna,
                                QtdeParafuso = c.Key.QuantidadeParafuso,
                                QtdeColuna = c.Count()
                            }).ToList();
            return relatorio;
        }
    }
}
