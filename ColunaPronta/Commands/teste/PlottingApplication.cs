using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Geometry;
using System.Runtime.InteropServices;
using System;
using NLog.Config;

namespace ColunaPronta.Commands
{
    public static class SimplePlottingCommands
    {
        //static extern int acedTrans(double[] point, IntPtr fromRb, IntPtr toRb, int disp, double[] result);

        public static void WindowPlot()
        {
            try
            {

                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor ed = doc.Editor;
                Database db = doc.Database;

                PromptPointOptions ppo =
                new PromptPointOptions(
                "/nSelect first corner of plot area: "
                );
                ppo.AllowNone = false;

                PromptPointResult ppr =
                ed.GetPoint(ppo);

                if (ppr.Status != PromptStatus.OK)
                    return;

                Point3d first = ppr.Value;

                PromptCornerOptions pco =
                new PromptCornerOptions(
                "/nSelect second corner of plot area: ",
                first
                );
                ppr = ed.GetCorner(pco);

                if (ppr.Status != PromptStatus.OK)
                    return;

                Point3d second = ppr.Value;



                Extents2d window =
                new Extents2d(first.X, first.Y, second.X, second.Y);

                Transaction tr =
                db.TransactionManager.StartTransaction();
                using (tr)
                {
                    // We'll be plotting the current layout

                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    Layout lo = (Layout)tr.GetObject( btr.LayoutId, OpenMode.ForRead );

                    //var id = LayoutManager.Current.CreateAndMakeLayoutCurrent("NewLayout");

                    //LayoutManager lm = LayoutManager.Current lo);
                    // Open the created layout

                    //var lo = (Layout)tr.GetObject(id, OpenMode.ForWrite);
                    // We need a PlotInfo object
                    // linked to the layout

                    PlotInfo pi = new PlotInfo();
                    pi.Layout = btr.LayoutId;


                    // We need a PlotSettings object
                    // based on the layout settings
                    // which we then customize

                    PlotSettings ps =
                    new PlotSettings(lo.ModelType);
                    ps.CopyFrom(lo);

                    // The PlotSettingsValidator helps
                    // create a valid PlotSettings object

                    PlotSettingsValidator psv =
                    PlotSettingsValidator.Current;

                    // We'll plot the extents, centered and
                    // scaled to fit

                    psv.SetPlotWindowArea(ps, window);
                    psv.SetPlotType(ps, Autodesk.AutoCAD.DatabaseServices.PlotType.Window);
                    psv.SetUseStandardScale(ps, true);
                    psv.SetStdScaleType(ps, StdScaleType.ScaleToFit);
                    psv.SetPlotCentered(ps, true);

                    // We'll use the standard DWF PC3, as
                    // for today we're just plotting to file

                    psv.SetPlotConfigurationName(
                    ps,
                    "DWF6 ePlot.pc3",
                    "ANSI_A_(8.50_x_11.00_Inches)"
                    );


                    // We need to link the PlotInfo to the
                    // PlotSettings and then validate it

                    pi.OverrideSettings = ps;
                    PlotInfoValidator piv = new PlotInfoValidator();
                    piv.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                    piv.Validate(pi);

                    // A PlotEngine does the actual plotting
                    // (can also create one for Preview)

                    if (PlotFactory.ProcessPlotState ==
                    ProcessPlotState.NotPlotting)
                    {
                        PlotEngine pe =
                        PlotFactory.CreatePublishEngine();
                        using (pe)
                        {
                            // Create a Progress Dialog to provide info
                            // and allow thej user to cancel

                            PlotProgressDialog ppd =
                            new PlotProgressDialog(false, 1, true);
                            using (ppd)
                            {
                                ppd.set_PlotMsgString(
                                PlotMessageIndex.DialogTitle,
                                "Custom Plot Progress"
                                );
                                ppd.set_PlotMsgString(
                                PlotMessageIndex.CancelJobButtonMessage,
                                "Cancel Job"
                                );
                                ppd.set_PlotMsgString(
                                PlotMessageIndex.CancelSheetButtonMessage,
                                "Cancel Sheet"
                                );
                                ppd.set_PlotMsgString(
                                PlotMessageIndex.SheetSetProgressCaption,
                                "Sheet Set Progress"
                                );
                                ppd.set_PlotMsgString(
                                PlotMessageIndex.SheetProgressCaption,
                                "Sheet Progress"
                                );
                                ppd.LowerPlotProgressRange = 0;
                                ppd.UpperPlotProgressRange = 100;
                                ppd.PlotProgressPos = 0;

                                // Let's start the plot, at last

                                ppd.OnBeginPlot();
                                ppd.IsVisible = true;
                                pe.BeginPlot(ppd, null);

                                // We'll be plotting a single document

                                pe.BeginDocument(
                                pi,
                                doc.Name,
                                null,
                                1,
                                true, // Let's plot to file
                                "c://test-output"
                                );

                                // Which contains a single sheet

                                ppd.OnBeginSheet();

                                ppd.LowerSheetProgressRange = 0;
                                ppd.UpperSheetProgressRange = 100;
                                ppd.SheetProgressPos = 0;

                                PlotPageInfo ppi = new PlotPageInfo();
                                pe.BeginPage(
                                ppi,
                                pi,
                                true,
                                null
                                );
                                pe.BeginGenerateGraphics(null);
                                pe.EndGenerateGraphics(null);

                                // Finish the sheet
                                pe.EndPage(null);
                                ppd.SheetProgressPos = 100;
                                ppd.OnEndSheet();

                                // Finish the document

                                pe.EndDocument(null);

                                // And finish the plot

                                ppd.PlotProgressPos = 100;
                                ppd.OnEndPlot();
                                pe.EndPlot(null);
                            }
                        }
                    }
                    else
                    {
                        ed.WriteMessage(
                        "/nAnother plot is in progress."
                        );
                    }
                }
                //tr.Commit();
            }
            catch (System.Exception e)
            {
                NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
                NLog.LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Autodesk\ColunaPronta\NLog.config");
                Logger.Error(e.ToString());
            }
        }

        public static ObjectId CreateAndMakeLayoutCurrent(this LayoutManager lm, string name, bool select = true)
        {
            // First try to get the layout
            var id = lm.GetLayoutId(name);

            // If it doesn't exist, we create it
            if (!id.IsValid)
            {
                id = lm.CreateLayout(name);
            }

            // And finally we select it
            if (select)
            {
                lm.CurrentLayout = name;
            }
            return id;
        }

    }
}
