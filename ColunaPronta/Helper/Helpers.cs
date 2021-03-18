using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using NLog.Config;
using System;
using Autodesk.AutoCAD.Colors;
using ColunaPronta.Model;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace ColunaPronta.Helper
{
    public static class Helpers
    {

        public static void AddLinha(Document doc, Point3d point1, Point3d point2, bool trace, ColorIndex color)
        {
            try
            {
                Point3d pnt1 = new Point3d(point1.X, point1.Y, point1.Z);
                Point3d pnt2 = new Point3d(point2.X, point2.Y, point2.Z);

                Database database = doc.Database;
                Transaction transaction = database.TransactionManager.StartTransaction();
                using (DocumentLock documentLock = doc.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    Line line = new Line(pnt1, pnt2);
                    if (trace)
                    {
                        LinetypeTable lt = (LinetypeTable)transaction.GetObject(database.LinetypeTableId, OpenMode.ForWrite);
                        LinetypeTableRecord ltr = new LinetypeTableRecord()
                        {
                            AsciiDescription = "Sarrafo",
                            PatternLength = 0.75,
                            NumDashes = 2
                        };
                        ltr.SetDashLengthAt(0, 0.5);
                        ltr.SetDashLengthAt(1, -0.25);

                        ObjectId ltId = lt.Add(ltr);
                        line.LinetypeId = ltId;

                        transaction.AddNewlyCreatedDBObject(ltr, true);

                    }

                    if (color != ColorIndex.padrao)
                    {
                        line.ColorIndex = (int)color;
                    }

                    blockTableRecord.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

        public static void AddLinha(Document doc, Point3d point1, Point3d point2, Layer layer)
        {
            try
            {
                Point3d pnt1 = new Point3d(point1.X, point1.Y, point1.Z);
                Point3d pnt2 = new Point3d(point2.X, point2.Y, point2.Z);

                Database database = doc.Database;
                Transaction transaction = database.TransactionManager.StartTransaction();
                using (DocumentLock documentLock = doc.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
                    Line line = new Line(pnt1, pnt2);

                    var especificaolayer = new EspecificacaoLayer();
                    var nomeLayer = especificaolayer.GetNomeLayer(layer);

                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                    if (nomeLayer != "" && layerTable.Has(nomeLayer))
                    {
                        line.Layer = nomeLayer;
                    }

                    blockTableRecord.AppendEntity(line);
                    transaction.AddNewlyCreatedDBObject(line, true);
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

        public static ObjectIdCollection GetEntitiesOnLayer(string layerName)
        {
            try
            {
                Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

                Editor ed = doc.Editor;

                TypedValue[] tvs = new TypedValue[1] { new TypedValue((int)DxfCode.LayerName, layerName) };

                SelectionFilter sf = new SelectionFilter(tvs);

                PromptSelectionResult psr = ed.SelectAll(sf);

                if (psr.Status == PromptStatus.OK)
                {
                    return new ObjectIdCollection(psr.Value.GetObjectIds());
                }
                else
                {
                    return new ObjectIdCollection();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static IEnumerable<Line> GetAllLines(Database database)
        {
            using (var dockLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
            {
                using (var tr = database.TransactionManager.StartTransaction())
                {
                    var btr = tr.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTableRecord;
                    foreach (var obj in btr)
                    {
                        var line = tr.GetObject(obj, OpenMode.ForRead) as Line;
                        if (line != null)
                        {
                            yield return line;

                        }
                    }
                }
            }
        }
        public static void AddTexto(Document document, Point3d position, string text, ColorIndex color)
        {
            try
            {
                // Get the current document and database
                Database database = document.Database;

                Transaction transaction = database.TransactionManager.StartTransaction();
                using (DocumentLock documentLock = document.LockDocument())
                {
                    // Open the Block table for read
                    BlockTable acBlkTbl;
                    acBlkTbl = transaction.GetObject(database.BlockTableId,
                                                    OpenMode.ForRead) as BlockTable;

                    // Open the Block table record Model space for write
                    BlockTableRecord acBlkTblRec;
                    acBlkTblRec = transaction.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                    OpenMode.ForWrite) as BlockTableRecord;

                    // Create a single-line text object
                    using (DBText acText = new DBText())
                    {
                        acText.Position = position;
                        acText.TextString = text;
                        DBText dBText = new DBText
                        {
                            Height = 0.03
                        };

                        acText.Height = dBText.Height;

                        if (color != ColorIndex.padrao)
                        {
                            acText.ColorIndex = (int)color;
                        }

                        acBlkTblRec.AppendEntity(acText);
                        transaction.AddNewlyCreatedDBObject(acText, true);
                    }
                    // Save the changes and dispose of the transaction
                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddDimension(Document document, Point3d point1, Point3d point2, Point3d point3)
        {
            try
            {
                Database database = document.Database;
                Transaction transaction = database.TransactionManager.StartTransaction();

                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    DimStyleTable dst = (DimStyleTable)transaction.GetObject(database.DimStyleTableId, OpenMode.ForWrite);

                    AlignedDimension dimension = new AlignedDimension(new Point3d(point1.X, point1.Y, 0), new Point3d(point2.X, point2.Y, 0), point3, string.Empty, database.Dimstyle);
                    blockTableRecord.AppendEntity(dimension);
                    transaction.AddNewlyCreatedDBObject(dimension, true);
                }
                // Save the changes and dispose of the transaction
                transaction.Commit();

            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddCircle(Document document, Point3d center, double radiusCircle)
        {
            try
            {
                Database database = document.Database;
                Transaction transaction = database.TransactionManager.StartTransaction();
                
                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Circle acCirc = new Circle()
                    {
                        Center = center,
                        Radius = radiusCircle,
                        ColorIndex = (int)ColorIndex.vermelho //vermelho
                    };

                    blockTableRecord.AppendEntity(acCirc);
                    transaction.AddNewlyCreatedDBObject(acCirc, true);
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddCircle(Document document, Point3d center, double radiusCircle, Layer layer)
        {
            try
            {
                Database database = document.Database;
                Transaction transaction = database.TransactionManager.StartTransaction();

                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    Circle acCirc = new Circle()
                    {
                        Center = center,
                        Radius = radiusCircle,
                        ColorIndex = (int)ColorIndex.vermelho //vermelho
                    };

                    var especificaolayer = new EspecificacaoLayer();
                    var nomeLayer = especificaolayer.GetNomeLayer(layer);

                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                    if (nomeLayer != "" && layerTable.Has(nomeLayer))
                    {
                        acCirc.Layer = nomeLayer;
                    }

                    blockTableRecord.AppendEntity(acCirc);
                    transaction.AddNewlyCreatedDBObject(acCirc, true);
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddNumeracao(Document document, Point3d point, int i)
        {
            AddTexto(document, point, i.ToString(), ColorIndex.verde);

            var pointCircle = new Point3d(point.X, point.Y, 0);
            AddCircle(document, pointCircle, 10);

        }
        public static void AddPolyline(Document document, Point2d point1, Point2d point2, Point2d point3, Point2d point4, int iColor = 0)
        {
            try
            {
                Database database = document.Database;
                Transaction transaction = document.TransactionManager.StartTransaction();

                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var polyline = new Polyline(5);

                    Point2dCollection pts = new Point2dCollection();

                    pts.Add(new Point2d(point1.X, point1.Y));
                    pts.Add(new Point2d(point2.X, point2.Y));
                    pts.Add(new Point2d(point3.X, point3.Y));
                    pts.Add(new Point2d(point4.X, point4.Y));
                    pts.Add(new Point2d(point1.X, point1.Y));

                    var i = 0;
                    foreach (var pt in pts)
                    {
                        polyline.AddVertexAt(i, pt, 0, 0, 0);
                        i++;
                    }
                    if (iColor > 0)
                    {
                        polyline.ColorIndex = iColor;
                    }
                    blockTableRecord.AppendEntity(polyline);
                    transaction.AddNewlyCreatedDBObject(polyline, true);

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddPolyline(Document document, Point2dCollection points, ColorIndex color)
        {
            try
            {
                Database database = document.Database;

                Transaction transaction = document.TransactionManager.StartTransaction();

                // polyline do fundo de Viga
                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var polyline = new Polyline(points.Count);

                    var i = 0;
                    foreach (var pt in points)
                    {
                        polyline.AddVertexAt(i, pt, 0, 0, 0);
                        i++;
                    }

                    blockTableRecord.AppendEntity(polyline);
                    transaction.AddNewlyCreatedDBObject(polyline, true);

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static void AddPolyline(Document document, Point2dCollection points, Layer layer)
        {
            try
            {
                Database database = document.Database;

                Transaction transaction = document.TransactionManager.StartTransaction();

                // polyline do fundo de Viga
                using (DocumentLock documentLock = document.LockDocument())
                {
                    BlockTable blockTable = transaction.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord blockTableRecord = transaction.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                    var polyline = new Polyline(points.Count);

                    var i = 0;
                    foreach (var pt in points)
                    {
                        polyline.AddVertexAt(i, pt, 0, 0, 0);
                        i++;
                    }

                    var especificaolayer = new EspecificacaoLayer();
                    var nomeLayer = especificaolayer.GetNomeLayer(layer);

                    LayerTable layerTable = transaction.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;

                    if (nomeLayer != "" && layerTable.Has(nomeLayer))
                    {
                        polyline.Layer = nomeLayer;
                    } 

                    blockTableRecord.AppendEntity(polyline);
                    transaction.AddNewlyCreatedDBObject(polyline, true);

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }
        public static ObjectId GetTableStyle(TipoRelatorio tipoRelatorio)
        {
            try
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                Database database = document.Database;
                Transaction transaction = document.TransactionManager.StartTransaction();
                DBDictionary dictionary = (DBDictionary)transaction.GetObject(database.TableStyleDictionaryId, OpenMode.ForRead);
                ObjectId tableObject = ObjectId.Null;

                string styleName;

                switch (tipoRelatorio)
                {
                    case TipoRelatorio.FundoViga:
                        styleName = "Fundo Viga";
                        break;
                    default:
                        styleName = "Não encontrado";
                        break;
                }

                using (transaction)
                {

                    if (dictionary.Contains(styleName))
                    {
                        tableObject = dictionary.GetAt(styleName);
                    }
                    else
                    {
                        TableStyle tableStyle = new TableStyle();


                        tableStyle.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, (int)ColorIndex.verde), (int)(RowType.TitleRow | RowType.HeaderRow)); ;

                        tableStyle.SetBackgroundColor(Color.FromColorIndex(ColorMethod.ByAci, 2), (int)RowType.DataRow);

                        tableStyle.SetColor(Color.FromColorIndex(ColorMethod.ByAci, 6),
                          (int)(RowType.TitleRow |

                                RowType.HeaderRow |

                                RowType.DataRow)
                        );


                        tableStyle.SetGridColor(

                          Color.FromColorIndex(ColorMethod.ByAci, 4),

                          (int)GridLineType.OuterGridLines,

                          (int)(RowType.TitleRow |

                                RowType.HeaderRow |

                                RowType.DataRow)

                        );



                        tableStyle.SetGridColor(

                          Color.FromColorIndex(ColorMethod.ByAci, 3),

                          (int)GridLineType.InnerGridLines,

                          (int)(RowType.TitleRow |

                                RowType.HeaderRow |

                                RowType.DataRow)

                        );

                        tableStyle.SetGridLineWeight(

                          LineWeight.LineWeight211,

                          (int)GridLineType.AllGridLines,

                          (int)(RowType.TitleRow |

                                RowType.HeaderRow |

                                RowType.DataRow)

                        );


                        tableObject = tableStyle.PostTableStyleToDatabase(database, styleName);

                        transaction.AddNewlyCreatedDBObject(tableStyle, true);

                    }

                    transaction.Commit();
                }
                return tableObject;
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());

                return ObjectId.Null;
            }
        }
        public static void TraceBoundaryAndHatch()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Select a seed point for our boundary

            PromptPointResult ppr =
              ed.GetPoint("\nSelect internal point: ");

            if (ppr.Status != PromptStatus.OK)
                return;

            // Get the objects making up our boundary

            DBObjectCollection objs =
              ed.TraceBoundary(ppr.Value, false);

            if (objs.Count > 0)
            {
                Transaction tr =
                  doc.TransactionManager.StartTransaction();
                using (tr)
                {
                    // We'll add the objects to the model space

                    BlockTable bt =
                      (BlockTable)tr.GetObject(
                        doc.Database.BlockTableId,
                        OpenMode.ForRead
                      );

                    BlockTableRecord btr =
                      (BlockTableRecord)tr.GetObject(
                        bt[BlockTableRecord.ModelSpace],
                        OpenMode.ForWrite
                      );

                    // Add our boundary objects to the drawing and
                    // collect their ObjectIds for later use

                    ObjectIdCollection ids = new ObjectIdCollection();
                    foreach (DBObject obj in objs)
                    {
                        Entity ent = obj as Entity;
                        if (ent != null)
                        {
                            // Set our boundary objects to be of
                            // our auto-incremented colour index

                            ent.ColorIndex = 1;

                            // Set our transparency to 50% (=127)
                            // Alpha value is Truncate(255 * (100-n)/100)

                            ent.Transparency = new Transparency(127);

                            // Add each boundary object to the modelspace
                            // and add its ID to a collection

                            ids.Add(btr.AppendEntity(ent));
                            tr.AddNewlyCreatedDBObject(ent, true);
                        }
                    }

                    // Create our hatch

                    Hatch hat = new Hatch();

                    // Solid fill of our auto-incremented colour index

                    hat.SetHatchPattern(
                      HatchPatternType.PreDefined,
                      "SOLID"
                    );
                    hat.ColorIndex = 1;

                    // Set our transparency to 50% (=127)
                    // Alpha value is Truncate(255 * (100-n)/100)

                    hat.Transparency = new Transparency(127);

                    // Add the hatch to the modelspace & transaction

                    ObjectId hatId = btr.AppendEntity(hat);
                    tr.AddNewlyCreatedDBObject(hat, true);

                    // Add the hatch loops and complete the hatch

                    hat.Associative = true;
                    hat.AppendLoop(
                      HatchLoopTypes.Default,
                      ids
                    );

                    hat.EvaluateHatch(true);

                    // Commit the transaction

                    tr.Commit();
                }
            }
        }
        public static ObjetosSelecionados GetObjetos()
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            Document document = Application.DocumentManager.MdiActiveDocument;
            PromptSelectionResult acSSPrompt = editor.GetSelection();
            var objetos = new ObjetosSelecionados();

            Database acCurDb = document.Database;

            if (acSSPrompt.Status == PromptStatus.OK)
            {
                objetos.Polylines = Helpers.GetPolylines(document, acSSPrompt);
                objetos.Circles   = Helpers.GetCircles(document, acSSPrompt);
            }

            return objetos;
        }
        public static List<Polyline> GetPolylines(Document document , PromptSelectionResult acSSPrompt)
        {
            try
            {
                var ListPolylines = new List<Polyline>();
                var areaPolyline = new List<double>();
                Database acCurDb = document.Database;

                Transaction acTrans = acCurDb.TransactionManager.StartTransaction();
                using (DocumentLock documentLock = document.LockDocument())
                {
                    // Request for objects to be selected in the drawing area

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;

                        // Step through the objects in the selection set
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            // Check to make sure a valid SelectedObject object was returned
                            if (acSSObj != null)
                            {
                                // Open the selected object for write
                                var entity = acTrans.GetObject(acSSObj.ObjectId,
                                                                 OpenMode.ForWrite) as Polyline;

                                if (entity != null)
                                {
                                    ListPolylines.Add(entity);

                                }
                            }
                        }
                    }
                }
                return ListPolylines;
            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }
        public static List<Circle> GetCircles(Document document, PromptSelectionResult acSSPrompt)
        {
            try
            {
                var ListCircles = new List<Circle>();
                Database acCurDb = document.Database;

                Transaction acTrans = acCurDb.TransactionManager.StartTransaction();
                using (DocumentLock documentLock = document.LockDocument())
                {
                    // Request for objects to be selected in the drawing area

                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        SelectionSet acSSet = acSSPrompt.Value;

                        // Step through the objects in the selection set
                        foreach (SelectedObject acSSObj in acSSet)
                        {
                            // Check to make sure a valid SelectedObject object was returned
                            if (acSSObj != null)
                            {
                                // Open the selected object for write
                                var entity = acTrans.GetObject(acSSObj.ObjectId,
                                                                 OpenMode.ForWrite) as Circle;

                                if (entity != null)
                                {
                                    ListCircles.Add(entity);

                                }
                            }
                        }
                    }
                }
                return ListCircles;

            }
            catch (Exception e)
            {
                Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
                editor.WriteMessage(e.ToString());
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
                return null;
            }
        }

        public static void GeraArquivoExcel(List<Planilha> arquivoExcel, string nomeProjeto, TipoLista tipoLista)
        {
            var excelApp = new Excel.Application();
            // Make the object visible.
            excelApp.Visible = true;
            string path;

            switch(tipoLista)
            {
                case TipoLista.ListaEntregaColuna:

                    path = string.Concat(@"C:\Autodesk\ColunaPronta\ListaExcel\listaEntrega", nomeProjeto, System.Guid.NewGuid().ToString(), ".xlsx");
                    if(File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Copy(@"C:\Autodesk\ColunaPronta\Template\ListaDeEntrega.xlsx", path);
                    break;
                case TipoLista.ListaCorteColuna:
                    path = string.Concat(@"C:\Autodesk\ColunaPronta\ListaExcel\ListaCorte", nomeProjeto, System.Guid.NewGuid().ToString(), ".xlsx");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Copy(@"C:\Autodesk\ColunaPronta\Template\ListaDeCorte.xlsx", path);
                    break;
                default: //ListaCorteGuardaCOrpo
                    path = string.Concat(@"C:\Autodesk\ColunaPronta\ListaExcel\ListaCorte", nomeProjeto, System.Guid.NewGuid().ToString(), ".xlsx");
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    File.Copy(@"C:\Autodesk\ColunaPronta\Template\ListaDeCorteGuardaCorpo.xlsx", path);
                    break;
            }   

         

            // Create a new, empty workbook and add it to the collection returned
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template.
            // Because no argument is sent in this example, Add creates a new workbook.
            excelApp.Workbooks.Open(path);

            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            int row;

            switch (tipoLista)
            {
                case TipoLista.ListaEntregaColuna:

                    row = 18;
                    foreach (var linha in arquivoExcel)
                    {
                        if (
                            linha.Peca.ToUpper() != ("CANTONEIRA").ToUpper() &&
                            linha.Peca.ToUpper() != ("SAPATA").ToUpper() &&
                            linha.Peca.ToUpper() != ("cantoneirapassante").ToUpper()
                            )
                        {
                            row++;
                            workSheet.Cells[row, "B"] = linha.Item.ToUpper();
                            workSheet.Cells[row, "E"] = linha.Especificao.ToUpper();
                            workSheet.Cells[row, "G"] = linha.Comprimento;
                            workSheet.Cells[row, "H"] = linha.Quantidade;
                            workSheet.Cells[row, "I"] = linha.Observacao.ToUpper();
                        }

                    }

                    break;

                case TipoLista.ListaCorteColuna:
                    row = 9;
                    foreach (var linha in arquivoExcel)
                    {
                        if (linha.Peca.ToUpper() == ("Coluna").ToUpper() ||
                            linha.Peca.ToUpper() == ("colunaPassante").ToUpper() ||
                            linha.Peca.ToUpper() == ("CANTONEIRA").ToUpper() ||
                            linha.Peca.ToUpper() == ("SAPATA").ToUpper() ||
                            linha.Peca.ToUpper() == ("cantoneirapassante").ToUpper()


                            )
                        {
                            row++;
                            workSheet.Cells[row, "C"] = linha.Especificao.ToUpper();
                            workSheet.Cells[row, "D"] = linha.Comprimento;
                            workSheet.Cells[row, "E"] = linha.Quantidade;
                            workSheet.Cells[row, "F"] = linha.Item.ToUpper();
                        }
                    }

                    break;
                default: //ListaCorteGuardaCOrpo
                    row = 11;
                    foreach (var linha in arquivoExcel)
                    {
                       workSheet.Cells[row, "C"] = linha.Especificao.ToUpper();
                       workSheet.Cells[row, "D"] = linha.Comprimento;
                       workSheet.Cells[row, "E"] = linha.Quantidade;
                       //workSheet.Cells[row, "F"] = linha.Observacao.ToUpper();
                       row++;
                    }
                    break;
            }
  

        }

    }

}
