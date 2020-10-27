//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.Geometry;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Application = Autodesk.AutoCAD.ApplicationServices.Application;


//namespace ColunaPronta.Commands
//{
//    public static class ViewPlottColuna
//    {
//        public void CreateLayout()
//        {
//            var doc = Application.DocumentManager.MdiActiveDocument;

//            if (doc == null)
//                return;

//            var db = doc.Database;
//            var ed = doc.Editor;
//            var ext = new Extents2d();

//            //ext.MaxPoints

//            using (var tr = db.TransactionManager.StartTransaction())
//            {
//                // Create and select a new layout tab

//                var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("NewLayout");

//                // Open the created layout

//                var lay = (Layout)tr.GetObject(id, OpenMode.ForWrite);

//                // Make some settings on the layout and get its extents
               
//                lay.SetPlotSettings(
//                  //"ISO_full_bleed_2A0_(1189.00_x_1682.00_MM)", // Try this big boy!

//                  "ANSI_B_(11.00_x_17.00_Inches)",

//                  "monochrome.ctb",

//                  "DWF6 ePlot.pc3"

//                );

//                ext = lay.GetMaximumExtents();

//                lay.ApplyToViewport(
//                  tr, 2,
//                  vp =>
//                  {
//                    // Size the viewport according to the extents calculated when
//                    // we set the PlotSettings (device, page size, etc.)
//                    // Use the standard 10% margin around the viewport
//                    // (found by measuring pixels on screenshots of Layout1, etc.)

//                  vp.ResizeViewport(ext, 0.8);
//                  // Adjust the view so that the model contents fit
//                  if (ValidDbExtents(db.Extmin, db.Extmax))
//                  {
//                  vp.FitContentToViewport(new Extents3d(db.Extmin, db.Extmax), 0.9);
//                  }
//                  // Finally we lock the view to prevent meddling
//                  vp.Locked = true;
//                  }
//                  );
//                  // Commit the transaction
//                  tr.Commit();
//                  }
//                  // Zoom so that we can see our new layout, again with a little padding
//                  ed.Command("_.ZOOM", "_E");
//                  ed.Command("_.ZOOM", ".7X");
//                  ed.Regen();
//                  }

//        // Returns whether the provided DB extents - retrieved from
//        // Database.Extmin/max - are "valid" or whether they are the default
//        // invalid values (where the min's coordinates are positive and the
//        // max coordinates are negative)
//        private bool ValidDbExtents(Point3d min, Point3d max)
//        {
//            return !(min.X > 0 && min.Y > 0 && min.Z > 0 && max.X < 0 && max.Y < 0 && max.Z < 0);

//        }
        
//        public static ObjectId CreateAndMakeLayoutCurrent( this LayoutManager lm, string name, bool select = true )
//        {
//            // First try to get the layout
//            var id = lm.GetLayoutId(name);
            
//            // If it doesn't exist, we create it
//            if (!id.IsValid)
//            {
//                id = lm.CreateLayout(name);
//            }
            
//            // And finally we select it
//            if (select)
//            {
//                lm.CurrentLayout = name;
//            }
//            return id;
//        }

//        public static void SetPlotSettings( this Layout lay, string pageSize, string styleSheet, string device)
//        {
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
//                // Set the pen 
//                var ssl = psv.GetPlotStyleSheetList();
//               if (ssl.Contains(styleSheet))
//               {
//                    psv.SetCurrentStyleSheet(ps, styleSheet);
//               }
//               // Copy the PlotSettings data back to the Layout
//               var upgraded = false;
//               if (!lay.IsWriteEnabled)
//               {
//                    lay.UpgradeOpen();
//                    upgraded = true;
//               }

//               lay.CopyFrom(ps);
//               if (upgraded)
//               {
//                    lay.DowngradeOpen();
//               }
//            }
//        }
//    }

//}
