using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using ColunaPronta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ColunaPronta.Commands
{
    public static class IntegraGuardaCorpo 
    {
        
        public static void Add()
        {
            Document documentFundoViga = Application.DocumentManager.MdiActiveDocument;
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

            var guardaCorpo = new GuardaCorpo(new Point2d(prPtRes1.Value.X, prPtRes1.Value.Y), new Point2d(prPtRes2.Value.X, prPtRes2.Value.Y));

            Integra(guardaCorpo);
        }

        private static void Integra(GuardaCorpo guardaCorpo)
        {
            var queComecemOsJogos = "testetete";
        }

    }
}
