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
            boton1.ToolTip = "Crea todos los Clash Parameters o reinicia todo el Análisis";
            boton1.LongDescription = "Iniciamos Quick Clash Tool creando todos los Clash Parameters o volviendolos a sus valores por defecto si ya existen. ";

            boton2.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/check-list-(1).png"));
            boton2.ToolTip = "Gestiona qué Categorias Analizar";
            boton2.LongDescription = "Escogemos entre qué Categorias de Elementos queremos hacer el Análisis de Colisiones o Interferencias. Se elige si en la Vista Activa o si en todo el Documento. \nDependiento del tamaño del documento o archivo si se eligen muchas Categorias para el Análisis este proceso puede demorar varios minutos. ";

            boton3.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/3d-(1).png"));
            boton3.ToolTip = "Escoge qué Section Box Aplicar";
            boton3.LongDescription = "Escogemos entre qué Categorias de Elementos queremos hacer el Análisis de Colisiones o Interferencias. Se elige si en la Vista Activa o si en todo el Documento. \nDependiento del tamaño del documento o archivo si se eligen muchas Categorias para el Análisis este proceso puede demorar varios minutos. ";

            // Crear Panel 2
            RibbonPanel panel2 = application.CreateRibbonPanel(tabName, "Clash Review");

            // Agregar un botón a Panel 2
            PushButton boton4 = panel2.AddItem(new PushButtonData("boton4", "Quick Clash", ExecutingAssemblyPath, "ClashDetection.BUTTON_4_QuickClash")) as PushButton;
            PushButton boton5 = panel2.AddItem(new PushButtonData("boton5", "Clean Clash", ExecutingAssemblyPath, "ClashDetection.BUTTON_5_CleanClash")) as PushButton;
            PushButton boton6 = panel2.AddItem(new PushButtonData("boton6", "Clash Comments", ExecutingAssemblyPath, "ClashDetection.BUTTON_6_ClashComments")) as PushButton;

            // Agregar Imagen al botón
            boton4.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/pipes-angles-(1).png"));
            boton4.ToolTip = "Análisis Rápido";
            boton4.LongDescription = "Análisis de Colisiones Rápido de todas las Categorías contra todas las Categorías de la Vista Activa. ";

            boton5.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/broom-(1).png"));
            boton5.ToolTip = "Limpiar parámetros de la Vista Activa";
            boton5.LongDescription = "Borra o devuelve a sus valores por defecto los Clash Parameters de la Vista Activa. Menos los parámetros: 'Clash Solved' y 'Clash Comments'. ";

            boton6.LargeImage = new BitmapImage(new Uri("pack://application:,,,/ClashDetection;component/Resources/edit-(1).png"));
            boton6.ToolTip = "Comenta los Elementos pendientes de revisión";
            boton6.LongDescription = "Se escribirá el comentario en los Elementos con CLASH de la Vista Activa. ";

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }


    }
}
