using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace ColunaPronta.Commands
//{

//    public static class GeraRelatorio
//    {

//        public static void CreateLayout()
//        {

//            Document document = Application.DocumentManager.MdiActiveDocument;
//            Database database = document.Database;
//            Editor editor = document.Editor;

//            //var extents = new Extents2d();

//            Transaction tr = database.TransactionManager.StartTransaction();
//            using (DocumentLock documentLock = document.LockDocument())
//            {

//                var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("ColunaPronta");

//                var layout = (Layout)tr.GetObject(id, OpenMode.ForWrite);


//                layout.SetPlotSettings(

//                //  //"ISO_full_bleed_2A0_(1189.00_x_1682.00_MM)", // Try this big boy!

//                  "ANSI_B_(11.00_x_17.00_Inches)",

//                  "monochrome.ctb",

//                  "DWF6 ePlot.pc3"

//                );



//                //extents = lay.GetMaximumExtents();



//                //lay.ApplyToViewport(

//                //  tr, 2,

//                //  vp =>

//                //  {

//                //      // Size the viewport according to the extents calculated when

//                //      // we set the PlotSettings (device, page size, etc.)

//                //      // Use the standard 10% margin around the viewport

//                //      // (found by measuring pixels on screenshots of Layout1, etc.)



//                //      vp.ResizeViewport(ext, 0.8);



//                //      // Adjust the view so that the model contents fit

//                //      if (ValidDbExtents(db.Extmin, db.Extmax))

//                //      {

//                //          vp.FitContentToViewport(new Extents3d(db.Extmin, db.Extmax), 0.9);

//                //      }



//                //      // Finally we lock the view to prevent meddling



//                //      vp.Locked = true;

//                //  }

//                //);



//                // Commit the transaction



//                tr.Commit();

//            }



//            // Zoom so that we can see our new layout, again with a little padding



//            //editor.Command("_.ZOOM", "_E");

//            //editor.Command("_.ZOOM", ".7X");

//            editor.Regen();

//        }


//       public static ObjectId CreateAndMakeLayoutCurrent(this LayoutManager lm, string name , bool select = true)
//       {
//           var id = lm.GetLayoutId(name);

//           // Verificar se o layout existe
//           if (!id.IsValid)
//           {
//               id = lm.CreateLayout(name);
//           }

//           if (select)
//           {
//               lm.CurrentLayout = name;
//           }

//           return id;
//       }

//       public static void SetPlotSettings( this Layout lay, string pageSize, string styleSheet, string device )
//       {
//            using (var ps = new PlotSettings(lay.ModelType))
//            {
//                ps.CopyFrom(lay);

//                var psv = PlotSettingsValidator.Current;

//                // Set the device

//                var devs = psv.GetPlotDeviceList();

//                if (devs.Contains(device))
//                {
//                    psv.SetPlotConfigurationName(ps, device, null);

//                    psv.RefreshLists(ps);
//                }

//                // Set the media name/size

//                var mns = psv.GetCanonicalMediaNameList(ps);

//                if (mns.Contains(pageSize))
//                {
//                    psv.SetCanonicalMediaName(ps, pageSize);

//                }

//                // Set the pen settings

//                var ssl = psv.GetPlotStyleSheetList();

//                if (ssl.Contains(styleSheet))
//                {
//                    psv.SetCurrentStyleSheet(ps, styleSheet);
//                }

//                // Copy the PlotSettings data back to the Layout

//                var upgraded = false;

//                if (!lay.IsWriteEnabled)
//                {
//                    lay.UpgradeOpen();
//                    upgraded = true;
//                }

//                lay.CopyFrom(ps);

//                if (upgraded)
//                {
//                    lay.DowngradeOpen();
//                }
//            }
//        }


//        static public void PlotPageSetup()

//        {



//            Document doc = Application.DocumentManager.MdiActiveDocument;

//            Database db = doc.Database;

//            Editor ed = doc.Editor;



//            PromptStringOptions opts =

//                        new PromptStringOptions("Enter plot setting name");

//            opts.AllowSpaces = true;

//            PromptResult settingName = ed.GetString(opts);



//            if (settingName.Status != PromptStatus.OK)

//                return;



//            using (Transaction Tx =

//                        db.TransactionManager.StartTransaction())

//            {



//                LayoutManager layManager = LayoutManager.Current;

//                ObjectId layoutId =

//                           layManager.GetLayoutId(layManager.CurrentLayout);



//                Layout layoutObj =

//                           (Layout)Tx.GetObject(layoutId, OpenMode.ForRead);



//                DBDictionary plotSettingsDic =

//                   (DBDictionary)Tx.GetObject(db.PlotSettingsDictionaryId,

//                                                           OpenMode.ForRead);



//                if (!plotSettingsDic.Contains(settingName.StringResult))

//                    return;





//                ObjectId plotsetting =

//                            plotSettingsDic.GetAt(settingName.StringResult);

//                //layout type

//                bool bModel = layoutObj.ModelType;



//                PlotSettings plotSettings = Tx.GetObject(plotsetting,

//                                          OpenMode.ForRead) as PlotSettings;



//                if (plotSettings.ModelType != bModel)

//                {

//                    return;

//                }

//                object backgroundPlot =

//                   Application.GetSystemVariable("BACKGROUNDPLOT");



//                Application.SetSystemVariable("BACKGROUNDPLOT", 0);



//                try

//                {

//                    //now plot the setup...

//                    PlotInfo plotInfo = new PlotInfo();

//                    plotInfo.Layout = layoutObj.ObjectId;

//                    plotInfo.OverrideSettings = plotSettings;



//                    PlotInfoValidator piv = new PlotInfoValidator();

//                    piv.Validate(plotInfo);





//                    string outName = "c:\\temp\\"

//                                        + plotSettings.PlotSettingsName;



//                    using (var pe = PlotFactory.CreatePublishEngine())

//                    {

//                        // Begin plotting a document.

//                        pe.BeginPlot(null, null);

//                        pe.BeginDocument(plotInfo,

//                                doc.Name, null, 1, true, outName);



//                        // Begin plotting the page.

//                        PlotPageInfo ppi = new PlotPageInfo();

//                        pe.BeginPage(ppi, plotInfo, true, null);

//                        pe.BeginGenerateGraphics(null);

//                        pe.EndGenerateGraphics(null);



//                        // Finish the sheet

//                        pe.EndPage(null);



//                        // Finish the document

//                        pe.EndDocument(null);



//                        //// And finish the plot

//                        pe.EndPlot(null);

//                    }



//                }

//                catch

//                {

//                }



//                Tx.Commit();



//                Application.SetSystemVariable("BACKGROUNDPLOT",

//                                 backgroundPlot);//

//            }

//        }
//        //public static void ApplyToViewport(this Layout lay, Transaction tr, int vpNum, Action<Viewport> f)
//        //{

//        //    var vpIds = lay.GetViewports();

//        //    Viewport vp = null;

//        //    foreach (ObjectId vpId in vpIds)
//        //    {
//        //        var vp2 = tr.GetObject(vpId, OpenMode.ForWrite) as Viewport;

//        //        if (vp2 != null && vp2.Number == vpNum)
//        //        {
//        //            vp = vp2;

//        //            break;
//        //        }
//        //    }


//        //    if (vp == null)
//        //    {

//        //        var btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForWrite);

//        //        vp = new Viewport();

//        //        btr.AppendEntity(vp);

//        //        tr.AddNewlyCreatedDBObject(vp, true);

//        //        vp.On = true;

//        //        vp.GridOn = true;
//        //    }

//        //    f(vp);

//        //}




//        ///// <summary>

//        ///// Determine the maximum possible size for this layout.

//        ///// </summary>

//        ///// <returns>The maximum extents of the viewport on this layout.</returns>



//        //public static Extents2d GetMaximumExtents(this Layout lay)

//        //{

//        //    // If the drawing template is imperial, we need to divide by

//        //    // 1" in mm (25.4)



//        //    var div = lay.PlotPaperUnits == PlotPaperUnit.Inches ? 25.4 : 1.0;



//        //    // We need to flip the axes if the plot is rotated by 90 or 270 deg



//        //    var doIt =

//        //      lay.PlotRotation == PlotRotation.Degrees090 ||

//        //      lay.PlotRotation == PlotRotation.Degrees270;



//        //    // Get the extents in the correct units and orientation



//        //    var min = lay.PlotPaperMargins.MinPoint.Swap(doIt) / div;

//        //    var max =

//        //      (lay.PlotPaperSize.Swap(doIt) -

//        //       lay.PlotPaperMargins.MaxPoint.Swap(doIt).GetAsVector()) / div;



//        //    return new Extents2d(min, max);

//        //}



//        ///// <summary>

//        ///// Sets the size of the viewport according to the provided extents.

//        ///// </summary>

//        ///// <param name="ext">The extents of the viewport on the page.</param>

//        ///// <param name="fac">Optional factor to provide padding.</param>



//        //public static void ResizeViewport(

//        //  this Viewport vp, Extents2d ext, double fac = 1.0

//        //)

//        //{

//        //    vp.Width = (ext.MaxPoint.X - ext.MinPoint.X) * fac;

//        //    vp.Height = (ext.MaxPoint.Y - ext.MinPoint.Y) * fac;

//        //    vp.CenterPoint =

//        //      (Point2d.Origin + (ext.MaxPoint - ext.MinPoint) * 0.5).Pad();

//        //}



//        ///// <summary>

//        ///// Sets the view in a viewport to contain the specified model extents.

//        ///// </summary>

//        ///// <param name="ext">The extents of the content to fit the viewport.</param>

//        ///// <param name="fac">Optional factor to provide padding.</param>



//        //public static void FitContentToViewport(

//        //  this Viewport vp, Extents3d ext, double fac = 1.0

//        //)

//        //{

//        //    // Let's zoom to just larger than the extents



//        //    vp.ViewCenter =

//        //      (ext.MinPoint + ((ext.MaxPoint - ext.MinPoint) * 0.5)).Strip();



//        //    // Get the dimensions of our view from the database extents



//        //    var hgt = ext.MaxPoint.Y - ext.MinPoint.Y;

//        //    var wid = ext.MaxPoint.X - ext.MinPoint.X;



//        //    // We'll compare with the aspect ratio of the viewport itself

//        //    // (which is derived from the page size)



//        //    var aspect = vp.Width / vp.Height;



//        //    // If our content is wider than the aspect ratio, make sure we

//        //    // set the proposed height to be larger to accommodate the

//        //    // content



//        //    if (wid / hgt > aspect)

//        //    {

//        //        hgt = wid / aspect;

//        //    }



//        //    // Set the height so we're exactly at the extents



//        //    vp.ViewHeight = hgt;



//        //    // Set a custom scale to zoom out slightly (could also

//        //    // vp.ViewHeight *= 1.1, for instance)



//        //    vp.CustomScale *= fac;

//        //}



//    }

//}
