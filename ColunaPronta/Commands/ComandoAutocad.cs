using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using FormaPronta.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ColunaPronta.Commands
{
    public class ComandoAutocad : IDisposable
    {
        private bool disposedValue;
        const double _escala = 1000;
        const double distancia = 38 / _escala;

        public Coluna SelecionaColuna()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            var coluna = new Coluna();

            List<double> ListaY = new List<double>();
            List<double> ListaX = new List<double>();
            double x;
            double y;

            PromptSelectionResult psr = ed.GetSelection();

            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
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
        public void GeraColuna(Coluna coluna)
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
        public void AddSapata(Point2d p1, Point2d p2, Point2d p3, Point2d p4, string tipocoluna , double diametro)
        {
            var raio = diametro / 2;
            var helper = new Helpers();
            Document document = Application.DocumentManager.MdiActiveDocument;

            helper.AddPolyline(document, p1, p2, p3, p4, tipocoluna);


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
            helper.AddCircle(document, new Point3d(p1.X + centerX, p1.Y - centery, 0), raio);

        }
     
        public void AddParafuso(string TpParafuso, Coluna coluna)
        {
            var helper = new Helpers();
            
            Document document = Application.DocumentManager.MdiActiveDocument;

            if (TpParafuso == "A")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X + (20 / _escala), pontoA.Y + (30 / _escala));
                var p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X , p1.Y - (10 / _escala));

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y );
                var p2v2 = new Point2d(p2v1.X , p2v1.Y - (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - +(50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y );

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "B")
            {
                var pontoA = new Point2d(coluna.PointB.X - (30 / _escala), coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X + (5 / _escala), pontoA.Y + (30 / _escala));
                var p2 = new Point2d(p1.X + (20 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (10 / _escala));

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p1.Y);
                var p2v2 = new Point2d(p2v1.X, p2v1.Y - (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y - +(50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "C")
            {
                var pontoA = new Point2d(coluna.PointB.X, coluna.PointB.Y);
                var p1 = new Point2d(pontoA.X + (20 / _escala), pontoA.Y - (25 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y  -(20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p2.X, p2.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X , p2v1.Y - (10 / _escala));
                var p2v3 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y );

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "D")
            {
                var pontoA = new Point2d(coluna.PointB.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X + (20 / _escala), pontoA.Y + (40 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p2.X, p2.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));
                var p2v3 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X - (50 / _escala), p2v1.Y);

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "E")
            {
                var pontoA = new Point2d(coluna.PointD.X , coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X - (5 / _escala), pontoA.Y - (20 / _escala));
                var p2 = new Point2d(p1.X , p1.Y - (10 / _escala));
                var p3 = new Point2d(p1.X - (20 / _escala), p1.Y - (10 / _escala) );
                var p4 = new Point2d(p1.X - (20 / _escala), p1.Y );

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X - ( 5 / _escala), p2.Y );
                var p2v2 = new Point2d(p2v1.X , p2v1.Y + (50 / _escala));
                var p2v3 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y + (50 / _escala) );
                var p2v4 = new Point2d(p2v1.X - (10 / _escala), p2v1.Y );

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
            if (TpParafuso == "F")
            {
                var pontoA = new Point2d(coluna.PointC.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X + (5 / _escala), pontoA.Y - (20 / _escala));
                var p2 = new Point2d(p1.X, p1.Y - (10 / _escala));
                var p3 = new Point2d(p1.X + (20 / _escala), p1.Y - (10 / _escala));
                var p4 = new Point2d(p1.X + (20 / _escala), p1.Y);

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X + (5 / _escala), p2.Y);
                var p2v2 = new Point2d(p2v1.X, p2v1.Y + (50 / _escala));
                var p2v3 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y + (50 / _escala));
                var p2v4 = new Point2d(p2v1.X + (10 / _escala), p2v1.Y);

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }

            if (TpParafuso == "G")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointC.Y);
                var p1 = new Point2d(pontoA.X - (30 / _escala), pontoA.Y + ( 45 / _escala));
                var p2 = new Point2d(p1.X     + ( 10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X     + ( 10 / _escala) , p1.Y- ( 20 / _escala) );
                var p4 = new Point2d(p1.X    , p1.Y- ( 20 / _escala) );

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X, p1.Y-(5/ _escala));
                var p2v2 = new Point2d(p2v1.X + ( 50 /_escala ) , p2v1.Y);
                var p2v3 = new Point2d(p2v1.X + ( 50 / _escala) , p2v1.Y - ( 10/ _escala));
                var p2v4 = new Point2d(p2v1.X, p2v1.Y - ( 10 / _escala));

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3 );
            }

            if (TpParafuso == "H")
            {
                var pontoA = new Point2d(coluna.PointA.X, coluna.PointA.Y);
                var p1 = new Point2d(pontoA.X - (30 / _escala), pontoA.Y - (25 / _escala));
                var p2 = new Point2d(p1.X + (10 / _escala), p1.Y);
                var p3 = new Point2d(p1.X + (10 / _escala), p1.Y - (20 / _escala));
                var p4 = new Point2d(p1.X, p1.Y - (20 / _escala));

                helper.AddPolyline(document, p1, p2, p3, p4, "Parafuso", 3);

                var p2v1 = new Point2d(p1.X, p1.Y - (5 / _escala));
                var p2v2 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y);
                var p2v3 = new Point2d(p2v1.X + (50 / _escala), p2v1.Y - (10 / _escala));
                var p2v4 = new Point2d(p2v1.X, p2v1.Y - (10 / _escala));

                helper.AddPolyline(document, p2v1, p2v2, p2v3, p2v4, "Parafuso", 3);
            }
        }

       

        //public void TesteInserBlck()
        //{
        //    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        //    PromptPointOptions prPtOpt = new PromptPointOptions("\nIndique o ponto onde será gerado o parafuso: ")
        //    {
        //        AllowArbitraryInput = false,
        //        AllowNone = true
        //    };

        //    PromptPointResult prPtRes = ed.GetPoint(prPtOpt);

        //    if (prPtRes.Status != PromptStatus.OK) return;
        //    Point3d point = prPtRes.Value;

        //    InsertBlock(point, "PARAFUSOCOLUNAPRONTA");
        //}
        //public void InsertBlock(Point3d insPt, string blockName)
        //{
        //    var doc = Application.DocumentManager.MdiActiveDocument;
        //    var db = doc.Database;
        //    var ed = doc.Editor;
        //    Transaction tr = db.TransactionManager.StartTransaction();
        //    using (DocumentLock documentLock = doc.LockDocument())
        //    {
        //        // check if the block table already has the 'blockName'" block
        //        var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        if (!bt.Has(blockName))
        //        {
                    
        //        }

        //        // create a new block reference
        //        using (var br = new BlockReference(insPt, bt[blockName]))
        //        {
                    
        //            //br.ScaleFactors= new Scale3d(-1, 1, 1);
        //            var space = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
        //            space.AppendEntity(br);
        //            tr.AddNewlyCreatedDBObject(br, true);
        //        }
 
        //        //Database db = ThisBlockId.Database;
        //        //using (Transaction tr = db.TransactionManager.StartTransaction())
        //        //{
        //        //    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        //    using (BlockReference br = new BlockReference(Point3d.Origin, bt[Name])
        //        //    {
        //        //        ScaleFactors = new Scale3d(Xscale, Yscale, Zscale),
        //        //        Rotation = Rotation
        //        //    })
        //        //    {
        //        //        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(ThisBlockId, OpenMode.ForWrite);
        //        //        ObjectId brId = btr.AppendEntity(br);
        //        //        tr.AddNewlyCreatedDBObject(br, true);
        //        //        tr.Commit();
        //        //        return brId;
        //        //    }
        //        //}
        //    }
        //    tr.Commit();

        //}


        public void CopyEnt()

        {

            Document doc = Application.DocumentManager.MdiActiveDocument;

            Database db = doc.Database;

            Editor ed = doc.Editor;



            PromptEntityOptions options =

                    new PromptEntityOptions("\nSelect entity to copy");



            PromptEntityResult acSSPrompt = ed.GetEntity(options);



            if (acSSPrompt.Status != PromptStatus.OK)

                return;



            ObjectIdCollection collection = new ObjectIdCollection();

            collection.Add(acSSPrompt.ObjectId);



            //make model space as owner for new entity

            ObjectId ModelSpaceId =

                    SymbolUtilityServices.GetBlockModelSpaceId(db);



            IdMapping mapping = new IdMapping();

            db.DeepCloneObjects(collection, ModelSpaceId, mapping, false);



            //now open the new entity and change the color...

            using (Transaction Tx = db.TransactionManager.StartTransaction())

            {

                //get the map.

                IdPair pair1 = mapping[acSSPrompt.ObjectId];



                //new object

                Entity ent = Tx.GetObject(pair1.Value,

                                                OpenMode.ForWrite) as Entity;



                //change the color to red

                ent.ColorIndex = 1;



                Tx.Commit();

            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
