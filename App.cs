using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.DB.Visual;
using forms = System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace ClashDetection
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    class App : IExternalApplication
    {
        public static string ExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

        public Result OnStartup(UIControlledApplication application)
        {
            // Todo el código para crear los botones en la Ribbon
            // Crear Tab 1 Dynoscript
            string tabName = "QuickClash";
            application.CreateRibbonTab(tabName);

            // Crear Panel 1
            RibbonPanel panel1 = application.CreateRibbonPanel(tabName, "Clash Manage");


            // Agregar un botón a Panel 1
            PushButton boton1 = panel1.AddItem(new PushButtonData("boton1", "Start Clash", ExecutingAssemblyPath, "ClashDetection.BUTTON_1_CreateClashParameters")) as PushButton;
            PushButton boton2 = panel1.AddItem(new PushButtonData("boton2", "Clash Manage", ExecutingAssemblyPath, "ClashDetection.BUTTON_2_ClashManage")) as PushButton;
            PushButton boton3 = panel1.AddItem(new PushButtonData("boton3", "Section Box", ExecutingAssemblyPath, "ClashDetection.BUTTON_3_SectionBox")) as PushButton;

            // Agregar Imagen al botón
            boton1.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/architech-working-(1).png"));
            boton1.ToolTip = "Create all Clash Parameters or restart all Analysis";
            boton1.LongDescription = "We start the Quick Clash Tool by creating all the Clash Parameters or returning them to their default values if they already exist.";

            boton2.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/check-list-(1).png"));
            boton2.ToolTip = "Manage which Categories to Analyze";
            boton2.LongDescription = "We choose between which Categories of Elements we want to do the Analysis of Collisions or Interferences. It is chosen whether in Active View or if in the entire Document. \nnDepending on the size of the document or file, if many Categories are chosen for the Analysis, this process may take several minutes.";

            boton3.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/3d-(1).png"));
            boton3.ToolTip = "Choose which Section Box to Apply";
            boton3.LongDescription = "We choose between which Categories of Elements we want to do the Analysis of Collisions or Interferences. It is chosen whether in Active View or if in the entire Document. \nDepending on the size of the document or file, if many Categories are chosen for the Analysis, this process may take several minutes.";

            // Crear Panel 2
            RibbonPanel panel2 = application.CreateRibbonPanel(tabName, "Clash Review");

            // Agregar un botón a Panel 2
            PushButton boton4 = panel2.AddItem(new PushButtonData("boton4", "Quick Clash", ExecutingAssemblyPath, "ClashDetection.BUTTON_4_QuickClash")) as PushButton;
            PushButton boton5 = panel2.AddItem(new PushButtonData("boton5", "Clean Clash", ExecutingAssemblyPath, "ClashDetection.BUTTON_5_CleanClash")) as PushButton;
            PushButton boton6 = panel2.AddItem(new PushButtonData("boton6", "Clash Comments", ExecutingAssemblyPath, "ClashDetection.BUTTON_6_ClashComments")) as PushButton;

            // Agregar Imagen al botón
            boton4.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/pipes-angles-(1).png"));
            boton4.ToolTip = "Quick Analysis";
            boton4.LongDescription = "Quick Collision Analysis of all Categories against all Categories in Active View.";

            boton5.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/broom-(1).png"));
            boton5.ToolTip = "Clear Active View parameters";
            boton5.LongDescription = "Delete or return the Clash Parameters from the Active View to their default values. Minus the parameters: 'Clash Solved' and 'Clash Comments'.";

            boton6.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/edit-(1).png"));
            boton6.ToolTip = "Comment on the Items pending revision";
            boton6.LongDescription = "The comment will be written in the Elements with CLASH of the Active View.";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


    }
}
