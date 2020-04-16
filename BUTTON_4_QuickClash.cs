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


namespace ClashDetection
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class BUTTON_4_QuickClash : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiApp = commandData.Application;
			UIDocument uidoc = uiApp.ActiveUIDocument;
			Document doc = uiApp.ActiveUIDocument.Document;
			Application app = uiApp.Application;

			//View activeView = doc.ActiveView;
            string ruta = App.ExecutingAssemblyPath;

            // Empezar a Escribir el código acá
            #region MACRO CODE


            #region BUTTONS 


            //void BUTTON_4_QuickClash() // clash rapido solo vista activa
            //{
            #region Buton_4
            try
            {
					List<Element> clash_yes = new List<Element>();

					DYNO_SetNoValueClashParameter(); // Vista Activa , "Clash" y "Clash Grid Location" = " " vacio.
					DYNO_IntersectMultipleElementsToMultipleCategory(); // Vista Activa eta reeeeeeemilllelllmeirefdaddaactmmttmm no funciona
					DYNO_IntersectMultipleElementsToMultipleFamilyInstances();// Vista Activa
					DYNO_SetClashGridLocation();// active view
					DYNO_SetIDValue_ActiveView(); // active view
												  //DYNO_CreateClashFilterMultipleElementsInView();
					List<Element> iclash_yes = DYNO_GetAllNOClashElements_OnlyActiveView();// Vista Activa
					foreach (Element e in iclash_yes)
					{
						clash_yes.Add(e);
					}
					DYNO_CheckClashSolved(clash_yes);// Vista Activa

			}
				catch (Exception ex)
				{
					TaskDialog.Show("Error ex", ex.Message.ToString());
					throw;
				}
            #endregion
            //} // clash rapido solo vista activa OK!

            #endregion

            #region DYNOSCRIPTS
            /// <summary>
            /// Set 3D view section box to next options.
            /// </summary>
            /// 
            List<Element> DYNO_lista_Elements_1(List<BuiltInCategory> UI_list1)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				List<BuiltInCategory> list1 = UI_list1;

				List<Element> E1 = new List<Element>();

				List<string> collec = new List<string>();

				foreach (BuiltInCategory blt in list1)
				{
					if (blt.ToString().Equals("BuiltInCategory.OST_DuctCurves"))
					{
						collec.Add("_DuctCurves");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_PipeCurves"))
					{
						collec.Add("_DuctCurves");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_Conduit"))
					{
						collec.Add("_DuctCurves");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_CableTray"))
					{
						collec.Add("_DuctCurves");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_FlexDuctCurves"))
					{
						collec.Add("_DuctCurves");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_FlexPipeCurves"))
					{
						collec.Add("_DuctCurves");
					}
				}

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements(); // PIPES

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements(); // CONDUIT

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements(); // CABLE TRAY

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements(); // FLEXDUCT

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements(); // FLEXPIPES

				foreach (string str in collec)
				{
					for (int i = 0; i < ducts.Count(); i++)
					{
						Element l1 = ducts[i] as Element;
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
					for (int i = 0; i < pipes.Count(); i++)
					{
						Element l1 = pipes[i];
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
					for (int i = 0; i < conduits.Count(); i++)
					{
						Element l1 = conduits[i];
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
					for (int i = 0; i < cabletrays.Count(); i++)
					{
						Element l1 = cabletrays[i];
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
					for (int i = 0; i < flexducts.Count(); i++)
					{
						Element l1 = flexducts[i];
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
					for (int i = 0; i < flexpipes.Count(); i++)
					{
						Element l1 = flexpipes[i];
						if (l1.Category.Name.ToString().Contains(str))
						{
							E1.Add(l1);
						}
					}
				}

				return E1;
			} // funciona

			List<Element> DYNO_lista_FamilyInstance_2(List<BuiltInCategory> UI_list2) // no funciona
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    //BuiltInCategory.OST_FlexDuctCurves,
				    //BuiltInCategory.OST_FlexPipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};

				List<BuiltInCategory> list2 = UI_list2;

				List<Element> E2 = new List<Element>();

				List<string> collec = new List<string>();

				foreach (BuiltInCategory blt in list2)
				{
					if (blt.ToString().Equals("BuiltInCategory.OST_CableTrayFitting"))
					{
						collec.Add("_CableTrayFitting");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_ConduitFitting"))
					{
						collec.Add("_ConduitFitting");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_DuctFitting"))
					{
						collec.Add("_DuctFitting");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_DuctTerminal"))
					{
						collec.Add("_DuctTerminal");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_ElectricalEquipment"))
					{
						collec.Add("_ElectricalEquipment");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_LightingDevices"))
					{
						collec.Add("_LightingDevices");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_LightingFixtures"))
					{
						collec.Add("_LightingFixtures");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_MechanicalEquipment"))
					{
						collec.Add("_MechanicalEquipment");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_PipeFitting"))
					{
						collec.Add("_PipeFitting");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_PlumbingFixtures"))
					{
						collec.Add("_PlumbingFixtures");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_SpecialityEquipment"))
					{
						collec.Add("_SpecialityEquipment");
					}
					if (blt.ToString().Equals("BuiltInCategory.OST_Sprinklers"))
					{
						collec.Add("_Sprinklers");
					}
				}

				foreach (BuiltInCategory fi in bics_familyIns)
				{


					// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
					ElementClassFilter elemFilter = new ElementClassFilter(typeof(FamilyInstance));

					// Create a category filter for Ducts
					ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(fi);

					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

					// Apply the filter to the elements in the active document
					FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

					IList<Element> elemfi = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // FAMILY INSTANCE

					foreach (string str in collec)
					{
						for (int i = 0; i < elemfi.Count(); i++)
						{
							Element l2 = elemfi[i] as Element;
							if (l2.Category.Name.ToString().Contains(str))
							{
								E2.Add(l2);
							}
						}
					}
				}

				return E2;
			} // no funciona

			void DYNO_CreateClashParameters_ModelElements()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;


				// Parameters strings
				string paramName = "Clash";
				string paramName2 = "Clash Category";
				string paramName3 = "Clash Comments";
				string paramName4 = "Clash Grid Location";
				string paramName5 = "Clash Solved";
				string paramName6 = "Done";
				string paramName7 = "ID Element";
				string paramName8 = "Percent Done";
				string paramName9 = "Zone";

				// open shared parameter file
				DefinitionFile myDefinitionFile = app.OpenSharedParameterFile();

				// get a group
				//Only works with a GroupParameterName which already exists in the SharedParameters file, e.g. "Dimensions". Not ideal!
				//Also this seems quite redundant because later I am inserting the new parameter on the "Data" ParameterGroup… How to go around this?
				DefinitionGroup myGroup = myDefinitionFile.Groups.Create("ClashParameters");

				// create an instance definition in definition group MyParameters
				ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text);
				ExternalDefinitionCreationOptions option2 = new ExternalDefinitionCreationOptions(paramName2, ParameterType.Text);
				ExternalDefinitionCreationOptions option3 = new ExternalDefinitionCreationOptions(paramName3, ParameterType.Text);
				ExternalDefinitionCreationOptions option4 = new ExternalDefinitionCreationOptions(paramName4, ParameterType.Text);
				ExternalDefinitionCreationOptions option5 = new ExternalDefinitionCreationOptions(paramName5, ParameterType.YesNo);
				ExternalDefinitionCreationOptions option6 = new ExternalDefinitionCreationOptions(paramName6, ParameterType.YesNo);
				ExternalDefinitionCreationOptions option7 = new ExternalDefinitionCreationOptions(paramName7, ParameterType.Text);
				ExternalDefinitionCreationOptions option8 = new ExternalDefinitionCreationOptions(paramName8, ParameterType.Text);
				ExternalDefinitionCreationOptions option9 = new ExternalDefinitionCreationOptions(paramName9, ParameterType.Text);


				// let the user modify the value, only the API
				option.UserModifiable = true;
				option2.UserModifiable = true;
				option3.UserModifiable = true;
				option4.UserModifiable = true;
				option5.UserModifiable = true;
				option6.UserModifiable = true;
				option7.UserModifiable = true;
				option8.UserModifiable = true;
				option9.UserModifiable = true;

				// Set tooltip
				option.Description = "Determina si el elemento tiene interferencia con otro. ";
				option2.Description = "La Categoría del Elemento contra el que existe la interferencia. ";
				option3.Description = "Comentario sobre la interferencia. ";
				option4.Description = "Zona más cerca de la interferencia. ";
				option5.Description = "Interferencia resuelta. Sí está activo en un Elemento, ese Elemento no será detectado con interferencias en el análisis. ";
				option6.Description = "Tarea resuelta. ";
				option7.Description = "Número de ID del Elemento";
				option8.Description = "Porcentaje de Tarea resuelta. ";
				option9.Description = "Zona General. ";

				Definition myDefinition_ProductDate = myGroup.Definitions.Create(option);
				Definition myDefinition_ProductDate2 = myGroup.Definitions.Create(option2);
				Definition myDefinition_ProductDate3 = myGroup.Definitions.Create(option3);
				Definition myDefinition_ProductDate4 = myGroup.Definitions.Create(option4);
				Definition myDefinition_ProductDate5 = myGroup.Definitions.Create(option5);
				Definition myDefinition_ProductDate6 = myGroup.Definitions.Create(option6);
				Definition myDefinition_ProductDate7 = myGroup.Definitions.Create(option7);
				Definition myDefinition_ProductDate8 = myGroup.Definitions.Create(option8);
				Definition myDefinition_ProductDate9 = myGroup.Definitions.Create(option9);

				CategorySet categories = app.Create.NewCategorySet();

				BuiltInCategory[] bics = new BuiltInCategory[]  // lista de BuiltInCategory
					{
					BuiltInCategory.OST_CableTray,
					BuiltInCategory.OST_CableTrayFitting,
					BuiltInCategory.OST_Conduit,
					BuiltInCategory.OST_ConduitFitting,
					BuiltInCategory.OST_DuctCurves,
					BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
					BuiltInCategory.OST_PipeCurves,
					BuiltInCategory.OST_FlexDuctCurves,
					BuiltInCategory.OST_FlexPipeCurves,
					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics)
				{
					Category MECat = doc.Settings.Categories.get_Item(bic);
					categories.Insert(MECat);
				}


				// get Mechanical Equipment category
				//		 Category MECat = doc.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment);
				//		 Category MECat2 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting);
				//		 Category MECat3 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves);
				//		 Category MECat4 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves);
				//		 Category MECat5 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeAccessory);
				//		 Category MECat6 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctTerminal);
				//		 Category MECat7 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTray);
				//		 Category MECat8 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Conduit);
				//		
				//		 
				//		 
				//		 categories.Insert(MECat);
				//		 categories.Insert(MECat2);
				//		 categories.Insert(MECat3);
				//		 categories.Insert(MECat4);
				//		 categories.Insert(MECat5);
				//		 categories.Insert(MECat6);
				//		 categories.Insert(MECat7);
				//		 categories.Insert(MECat8);




				// insert the new parameter
				InstanceBinding instanceBinding = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding2 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding3 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding4 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding5 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding6 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding7 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding8 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding9 = app.Create.NewInstanceBinding(categories);

				//		 // category and are family.
				//		 ElementClassFilter familyFilter = new ElementClassFilter(typeof(Family));
				//		 // Create a category filter for MechanicalEquipment
				//		 ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment);
				//		 // Create a logic And filter for all MechanicalEquipment Family
				//		 LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
				//		 // Apply the filter to the elements in the active document
				//		 FilteredElementCollector coll = new FilteredElementCollector(doc);
				//		 IList<Element> mechanicalEquipment = coll.WherePasses(MEInstancesFilter).ToElements();
				//		foreach (Family f in mechanicalEquipment) 
				//		 {
				//		 
				//		 if (!f.IsEditable)
				//		 continue;
				//		 
				//		 Document famdoc = doc.EditFamily(f);
				//		 
				//		 using (Transaction t = new Transaction(famdoc, "CreateClashParametersFamily"))
				//		 {
				//		 t.Start();
				//		 FamilyParameter FamParam = famdoc.FamilyManager.AddParameter(paramName, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam2 = famdoc.FamilyManager.AddParameter(paramName2, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam3 = famdoc.FamilyManager.AddParameter(paramName3, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam4 = famdoc.FamilyManager.AddParameter(paramName4, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam5 = famdoc.FamilyManager.AddParameter(paramName5, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.YesNo, true);
				//		 FamilyParameter FamParam6 = famdoc.FamilyManager.AddParameter(paramName6, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.YesNo, true);
				//		 FamilyParameter FamParam7 = famdoc.FamilyManager.AddParameter(paramName7, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam8 = famdoc.FamilyManager.AddParameter(paramName8, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 FamilyParameter FamParam9 = famdoc.FamilyManager.AddParameter(paramName9, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
				//		 
				//		// famdoc.FamilyManager.Set(FamParam5, 0);
				//		// famdoc.FamilyManager.Set(FamParam6, 0);
				//		 
				//		 t.Commit();
				//		 }
				//		 
				//		 }

				using (Transaction t = new Transaction(doc, "CreateClashParameters"))
				{
					t.Start();
					doc.ParameterBindings.Insert(myDefinition_ProductDate, instanceBinding, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate2, instanceBinding2, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate3, instanceBinding3, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate4, instanceBinding4, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate5, instanceBinding5, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate6, instanceBinding6, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate7, instanceBinding7, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate8, instanceBinding8, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate9, instanceBinding9, BuiltInParameterGroup.PG_CONSTRAINTS);

					t.Commit();
				}

				// get a ViewFamilyType for a 3D View
				ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
													 OfClass(typeof(ViewFamilyType)).
													 Cast<ViewFamilyType>()
												 where v.ViewFamily == ViewFamily.ThreeDimensional
												 select v).First();

				using (Transaction t = new Transaction(doc, "Create COORD view"))
				{
					t.Start();

					View3D COORD = View3D.CreateIsometric(doc, viewFamilyType.Id);
					//COORD.Name = "COORD";

					COORD.DisplayStyle = DisplayStyle.Shading;
					COORD.DetailLevel = ViewDetailLevel.Fine;
					//string view_name = "COORD";

					FilteredElementCollector DUFcoll = new FilteredElementCollector(doc).OfClass(typeof(View3D));
					List<View3D> views = new List<View3D>(); // lista vacia
					List<View3D> views_COORD = new List<View3D>(); // lista vacia
					int numero = 1;
					foreach (View3D ve in DUFcoll)
					{
						views.Add(ve); // lista con todos los ViewSchedule del proyecto
					}
					for (int i = 0; i < views.Count(); i++)
					{
						View3D ve = views[i];
						if (ve.Name.Contains("COORD"))
						{
							views_COORD.Add(ve); // todas la vistas con nombre que contiene "COORD"

							//numero = numero + 1;
						}

					}

					if (views_COORD.Count() == 0)
					{
						COORD.Name = "COORD";
					}
					else
					{
						for (int i = 0; i < views_COORD.Count(); i++)
						{
							View3D ve = views_COORD[i];
							if (ve.Name.Contains("COORD" + "  Copy "))
							{
								numero = numero + 1;
							}
							else
							{
								numero = 1; // solo COORD
							}
						}
						COORD.Name = "COORD" + "  Copy " + (numero).ToString();
					}

					List<Element> riv = new List<Element>();
					FilteredElementCollector links = new FilteredElementCollector(doc, COORD.Id);
					ElementCategoryFilter linkFilter = new ElementCategoryFilter(BuiltInCategory.OST_RvtLinks);
					links.WhereElementIsNotElementType();
					links.WherePasses(linkFilter);
					riv.AddRange(links.ToElements());
					//                		foreach (Element link in riv) 
					//                		{
					//                			
					//                		}

					t.Commit();
					uidoc.ActiveView = COORD;
				}

			} // Crea CLASH parametros a todas la categorias del modelo

			void DYNO_CreateClashParameters_ModelElements_SharedParameterExisting() // Crea CLASH parametros a todas la categorias del modelo usando Shared Parameter Existente
			{

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;

				// Parameters strings
				//		 	string paramName = "Clash";
				//		 	string paramName2 = "Clash Category";
				//		 	string paramName3 = "Clash Comments";
				//		 	string paramName4 = "Clash Grid Location";
				//		 	string paramName5 = "Clash Solved";
				//		 	string paramName6 = "Done";
				//		 	string paramName7 = "ID Element";
				//		 	string paramName8 = "Percent Done";
				//		 	string paramName9 = "Zone";
				//CreateClashParameters_Elements();
				//CreateClashParameters_Family();

				// open shared parameter file
				DefinitionFile myDefinitionFile = app.OpenSharedParameterFile();

				// get a group
				DefinitionGroup myGroup = myDefinitionFile.Groups.get_Item("ClashParameters");

				//DefinitionGroup myGroup = myDefinitionFile.Groups;

				//		 	// create an instance definition in definition group MyParameters
				//		 	ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option2 = new ExternalDefinitionCreationOptions(paramName2, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option3 = new ExternalDefinitionCreationOptions(paramName3, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option4 = new ExternalDefinitionCreationOptions(paramName4, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option5 = new ExternalDefinitionCreationOptions(paramName5, ParameterType.YesNo);
				//		 	ExternalDefinitionCreationOptions option6 = new ExternalDefinitionCreationOptions(paramName6, ParameterType.YesNo);
				//		 	ExternalDefinitionCreationOptions option7 = new ExternalDefinitionCreationOptions(paramName7, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option8 = new ExternalDefinitionCreationOptions(paramName8, ParameterType.Text);
				//		 	ExternalDefinitionCreationOptions option9 = new ExternalDefinitionCreationOptions(paramName9, ParameterType.Text);
				//		 
				//		 
				//		 	// let the user modify the value, only the API
				//		 	option.UserModifiable = true;
				//		 	option2.UserModifiable = true;
				//		 	option3.UserModifiable = true;
				//		 	option4.UserModifiable = true;
				//		 	option5.UserModifiable = true;
				//		 	option6.UserModifiable = true;
				//		 	option7.UserModifiable = true;
				//		 	option8.UserModifiable = true;
				//		 	option9.UserModifiable = true;
				//		 
				//		 	// Set tooltip
				//		 	option.Description = "Determina si el elemento tiene interferencia con otro. ";
				//		 	option2.Description = "La Categoría del Elemento contra el que existe la interferencia. ";
				//		 	option3.Description = "Comentario sobre la interferencia. ";
				//		 	option4.Description = "Zona más cerca de la interferencia. ";
				//		 	option5.Description = "Interferencia resuelta. Sí está activo en un Elemento, ese Elemento no será detectado con interferencias en el análisis. ";
				//		 	option6.Description = "Tarea resuelta. ";
				//		 	option7.Description = "Número de ID del Elemento";
				//		 	option8.Description = "Porcentaje de Tarea resuelta. ";
				//		 	option9.Description = "Zona General. ";

				Definition myDefinition_ProductDate = myGroup.Definitions.get_Item("Clash");
				Definition myDefinition_ProductDate2 = myGroup.Definitions.get_Item("Clash Category");
				Definition myDefinition_ProductDate3 = myGroup.Definitions.get_Item("Clash Comments");
				Definition myDefinition_ProductDate4 = myGroup.Definitions.get_Item("Clash Grid Location");
				Definition myDefinition_ProductDate5 = myGroup.Definitions.get_Item("Clash Solved");
				Definition myDefinition_ProductDate6 = myGroup.Definitions.get_Item("Done");
				Definition myDefinition_ProductDate7 = myGroup.Definitions.get_Item("ID Element");
				Definition myDefinition_ProductDate8 = myGroup.Definitions.get_Item("Percent Done");
				Definition myDefinition_ProductDate9 = myGroup.Definitions.get_Item("Zone");

				CategorySet categories = app.Create.NewCategorySet();

				BuiltInCategory[] bics = new BuiltInCategory[]  // lista de BuiltInCategory
					{
					BuiltInCategory.OST_CableTray,
					BuiltInCategory.OST_CableTrayFitting,
					BuiltInCategory.OST_Conduit,
					BuiltInCategory.OST_ConduitFitting,
					BuiltInCategory.OST_DuctCurves,
					BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
					BuiltInCategory.OST_PipeCurves,
					BuiltInCategory.OST_FlexDuctCurves,
					BuiltInCategory.OST_FlexPipeCurves,
					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics)
				{
					Category MECat = doc.Settings.Categories.get_Item(bic);
					categories.Insert(MECat);
				}

				// insert the new parameter
				InstanceBinding instanceBinding = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding2 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding3 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding4 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding5 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding6 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding7 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding8 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding9 = app.Create.NewInstanceBinding(categories);

				using (Transaction t = new Transaction(doc, "CreateClashParameters"))
				{
					t.Start();
					doc.ParameterBindings.Insert(myDefinition_ProductDate, instanceBinding, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate2, instanceBinding2, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate3, instanceBinding3, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate4, instanceBinding4, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate5, instanceBinding5, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate6, instanceBinding6, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate7, instanceBinding7, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate8, instanceBinding8, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate9, instanceBinding9, BuiltInParameterGroup.PG_CONSTRAINTS);

					t.Commit();
				}

			}// Crea CLASH parametros a todas la categorias del modelo usando Shared Parameter Existente


			void DYNO_SetEmptyYesNoParameters()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// category Mechanical Equipment

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    //BuiltInCategory.OST_FlexDuctCurves,
				    //BuiltInCategory.OST_FlexPipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();
					foreach (Element e in mechanicalequipment)
					{
						Parameter param = e.LookupParameter("Clash Solved");
						Parameter param2 = e.LookupParameter("Done");

						using (Transaction t = new Transaction(doc, "parametersME"))
						{
							t.Start();
							param.Set(1);
							param.Set(0);

							param2.Set(1);
							param2.Set(0);
							t.Commit();
						}
					}
				}



				//		 // category Duct Fittings
				//		 
				//		 ElementClassFilter DUFFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 // Create a category filter for MechanicalEquipment
				//		 ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				//		 // Create a logic And filter for all MechanicalEquipment Family
				//		 LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFFilter, DUFCategoryfilter);
				//		 // Apply the filter to the elements in the active document
				//		 FilteredElementCollector DUFcoll = new FilteredElementCollector(doc);
				//		 IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();
				//		 
				//		 foreach (Element e in ductfittings) 
				//		 {
				//		 Parameter param = e.LookupParameter("Clash Solved");
				//		 Parameter param2 = e.LookupParameter("Done");
				//		 
				//		 using (Transaction t = new Transaction(doc, "parametersDU"))
				//		 {
				//		 t.Start();
				//		param.Set(1);
				//		 param.Set(0);
				//		 
				//		 param2.Set(1);
				//		 param2.Set(0);
				//		t.Commit();
				//		 }
				//		 } 

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();



				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();



				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();



				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();


				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();



				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				List<Element> allProducts = new List<Element>();
				allProducts.Concat(ducts);
				allProducts.Concat(pipes);
				allProducts.Concat(conduits);
				allProducts.Concat(cabletrays);
				allProducts.Concat(flexducts);
				allProducts.Concat(flexpipes);
				allProducts.ToList();


				foreach (Element e in ducts)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t = new Transaction(doc, "parameters duct"))
					{
						t.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t.Commit();
					}

				}

				foreach (Element e in pipes)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t2 = new Transaction(doc, "parameters pipes"))
					{
						t2.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t2.Commit();
					}

				}

				foreach (Element e in conduits)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t3 = new Transaction(doc, "parameters conduits"))
					{
						t3.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t3.Commit();
					}

				}

				foreach (Element e in cabletrays)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t4 = new Transaction(doc, "parameters cable tray"))
					{
						t4.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t4.Commit();
					}

				}

				foreach (Element e in flexducts)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t = new Transaction(doc, "parameters duct"))
					{
						t.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t.Commit();
					}

				}

				foreach (Element e in flexpipes)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Done");

					using (Transaction t = new Transaction(doc, "parameters duct"))
					{
						t.Start();
						param.Set(1);
						param.Set(0);

						param2.Set(1);
						param2.Set(0);
						t.Commit();
					}

				}
			} // Valor vacio a TOdo el documento

			void DYNO_SetIDValue()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				// category Duct.

				foreach (Element e in ducts)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				foreach (Element e in pipes)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in conduits)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in cabletrays)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in flexducts)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				foreach (Element e in flexpipes)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				// category Mechanical Equipment

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    //BuiltInCategory.OST_FlexDuctCurves,
				    //BuiltInCategory.OST_FlexPipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter MEelemFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Ducts
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(MEelemFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();
					foreach (Element e in mechanicalequipment)
					{
						Parameter param = e.LookupParameter("ID Element");

						Autodesk.Revit.DB.ElementId selectedId = e.Id;
						string idString = selectedId.IntegerValue.ToString();

						using (Transaction t = new Transaction(doc, "ID Element"))
						{
							t.Start();
							param.Set(idString);
							t.Commit();
						}
					}
				}



				// category Mechanical Equipment


				//		 		// category ducts fittings
				//		 		ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 		// Create a category filter for Ducts
				//		 		ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				//		 		// Create a logic And filter for all MechanicalEquipment Family
				//		 		LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
				//		 		// Apply the filter to the elements in the active document
				//		 		FilteredElementCollector DUFcoll = new FilteredElementCollector(doc);
				//		 		IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();
				//				foreach (Element e in ductfittings) 
				//		 		{
				//		 			Parameter param = e.LookupParameter("ID Element");
				//		 
				//		 			Autodesk.Revit.DB.ElementId selectedId = e.Id;
				//		 			string idString = selectedId.IntegerValue.ToString();
				//		 
				//		 			using (Transaction t = new Transaction(doc, "ID Element"))
				//		 			{
				//		 				t.Start();
				//						param.Set(idString);
				//						t.Commit();
				//		 			}
				//		 		}

			} // Coloca calor ID Element al parametro ELEMENT ID TOdo el documento

			void DYNO_SetIDValue_ActiveView()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				// category Duct.

				foreach (Element e in ducts)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				foreach (Element e in pipes)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in conduits)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in cabletrays)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}
				foreach (Element e in flexducts)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				foreach (Element e in flexpipes)
				{
					Parameter param = e.LookupParameter("ID Element");

					Autodesk.Revit.DB.ElementId selectedId = e.Id;
					string idString = selectedId.IntegerValue.ToString();

					using (Transaction t = new Transaction(doc, "ID Element"))
					{
						t.Start();
						param.Set(idString);
						t.Commit();
					}
				}

				// category Mechanical Equipment

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    //BuiltInCategory.OST_FlexDuctCurves,
				    //BuiltInCategory.OST_FlexPipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter MEelemFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Ducts
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(MEelemFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();
					foreach (Element e in mechanicalequipment)
					{
						Parameter param = e.LookupParameter("ID Element");

						Autodesk.Revit.DB.ElementId selectedId = e.Id;
						string idString = selectedId.IntegerValue.ToString();

						using (Transaction t = new Transaction(doc, "ID Element"))
						{
							t.Start();
							param.Set(idString);
							t.Commit();
						}
					}
				}



				// category Mechanical Equipment


				//		 		// category ducts fittings
				//		 		ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 		// Create a category filter for Ducts
				//		 		ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				//		 		// Create a logic And filter for all MechanicalEquipment Family
				//		 		LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
				//		 		// Apply the filter to the elements in the active document
				//		 		FilteredElementCollector DUFcoll = new FilteredElementCollector(doc);
				//		 		IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();
				//				foreach (Element e in ductfittings) 
				//		 		{
				//		 			Parameter param = e.LookupParameter("ID Element");
				//		 
				//		 			Autodesk.Revit.DB.ElementId selectedId = e.Id;
				//		 			string idString = selectedId.IntegerValue.ToString();
				//		 
				//		 			using (Transaction t = new Transaction(doc, "ID Element"))
				//		 			{
				//		 				t.Start();
				//						param.Set(idString);
				//						t.Commit();
				//		 			}
				//		 		}

			} // Coloca calor ID Element al parametro ELEMENT ID a solo los elementos de la vista activa.

			void DYNO_CheckClashSolved(List<Element> clash_no_)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<Element> clashsolved_yes = new List<Element>();
				List<Element> clashsolved_no = new List<Element>();

				List<Element> clash_yes = new List<Element>();
				List<Element> clash_no = clash_no_;

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();
					foreach (Element e in mechanicalequipment)
					{
						Parameter param = e.LookupParameter("Clash Solved");
						Parameter param2 = e.LookupParameter("Clash");

						using (Transaction t = new Transaction(doc, "parametersME"))
						{
							t.Start();
							if (param.AsInteger() == 1 && param2.AsString() == "YES")
							{
								param2.Set("");
								clashsolved_yes.Add(e);
							}
							else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
							{
								clashsolved_yes.Add(e);
							}
							else
							{
								clashsolved_no.Add(e);
							}

							if (param2.AsString() == "YES")
							{
								clash_yes.Add(e);
							}
							//						 	if (!(param2.AsString() == "YES"))
							//						 	{
							//						 		clash_no.Add(e);
							//						 	}
							t.Commit();
						}
					}
				}



				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();



				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();



				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();



				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();


				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();



				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				foreach (Element e in ducts)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t = new Transaction(doc, "parameters duct"))
					{
						t.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}
						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						 	if (!(param2.AsString() == "YES"))
						//						 	{
						//						 		clash_no.Add(e);
						//						 	}
						t.Commit();
					}

				}

				foreach (Element e in pipes)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t2 = new Transaction(doc, "parameters pipes"))
					{
						t2.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}

						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						if (!(param2.AsString() == "YES"))
						//						 {
						//						 	clash_no.Add(e);
						//						}
						t2.Commit();
					}

				}

				foreach (Element e in conduits)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t3 = new Transaction(doc, "parameters conduits"))
					{
						t3.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}
						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						 	if (!(param2.AsString() == "YES"))
						//						 	{
						//						 		clash_no.Add(e);
						//						 	}
						t3.Commit();
					}

				}

				foreach (Element e in cabletrays)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t4 = new Transaction(doc, "parameters cable tray"))
					{
						t4.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}

						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						 	if (!(param2.AsString() == "YES"))
						//						 	{
						//						 		clash_no.Add(e);
						//						 	}
						t4.Commit();
					}

				}

				foreach (Element e in flexducts)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t = new Transaction(doc, "parameters flexduct"))
					{
						t.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}

						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						 	if (!(param2.AsString() == "YES"))
						//						 	{
						//						 		clash_no.Add(e);
						//						 	}
						t.Commit();
					}

				}

				foreach (Element e in flexpipes)
				{
					Parameter param = e.LookupParameter("Clash Solved");
					Parameter param2 = e.LookupParameter("Clash");

					using (Transaction t = new Transaction(doc, "parameters flexpipes"))
					{
						t.Start();
						if (param.AsInteger() == 1 && param2.AsString() == "YES")
						{
							param2.Set("");
							clashsolved_yes.Add(e);
						}
						else if (param.AsInteger() == 1 && !(param2.AsString() == "YES"))
						{
							clashsolved_yes.Add(e);
						}
						else
						{
							clashsolved_no.Add(e);
						}

						if (param2.AsString() == "YES")
						{
							clash_yes.Add(e);
						}
						//						 	if (!(param2.AsString() == "YES"))
						//						 	{
						//						 		clash_no.Add(e);
						//						 	}
						t.Commit();
					}

				}

				foreach (Element elem in clash_no)
				{

					//Parameter param = elem.LookupParameter("Clash Solved");
					Parameter param2 = elem.LookupParameter("Clash");
					string vacio = "";
					using (Transaction t = new Transaction(doc, "Set CLASH = vacio "))
					{

						t.Start();

						param2.Set(vacio);

						t.Commit();
					}
				}

				if (clash_yes.Count() < 1)
				{
					TaskDialog.Show("Dynoscript", "NO HAY INTERFERENCIAS!! en esta vista activa \n\n Muy bien! :)");
					using (Transaction t = new Transaction(doc, "Cambiar nombre Comentado"))
					{
						t.Start();
						activeView.Name = activeView.Name.ToString() + " - RESUELTO";
						t.Commit();
					}

				}
			}

			// todo con todo documento
			void DYNO_IntersectMultipleElementsToMultipleCategory_doc()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				string mensaje = "";

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					// category Duct.
					ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
					ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
					ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
					ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
					ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
					ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
					// Create a category filter for Ducts
					ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
					ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
					ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
					ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
					ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
					ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);

					LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
					LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);
					LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);
					LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);
					LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);
					LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					//Collector = Clashes
					ExclusionFilter filter = new ExclusionFilter(collectoreID);
					ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter6 = new ExclusionFilter(collectoreID);

					FilteredElementCollector collector = new FilteredElementCollector(doc);
					FilteredElementCollector collector2 = new FilteredElementCollector(doc);
					FilteredElementCollector collector3 = new FilteredElementCollector(doc);
					FilteredElementCollector collector4 = new FilteredElementCollector(doc);
					FilteredElementCollector collector5 = new FilteredElementCollector(doc);
					FilteredElementCollector collector6 = new FilteredElementCollector(doc);

					collector.OfClass(typeof(Duct));
					collector2.OfClass(typeof(Pipe));
					collector3.OfClass(typeof(Conduit));
					collector4.OfClass(typeof(CableTray));
					collector5.OfClass(typeof(FlexDuct));
					collector6.OfClass(typeof(FlexPipe));

					collector.WherePasses(DU2InstancesFilter);
					collector2.WherePasses(DU2InstancesFilter2);
					collector3.WherePasses(DU2InstancesFilter3);
					collector4.WherePasses(DU2InstancesFilter4);
					collector5.WherePasses(DU2InstancesFilter5);
					collector6.WherePasses(DU2InstancesFilter6);

					collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector.WherePasses(filter);
					collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector2.WherePasses(filter2);
					collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector3.WherePasses(filter3);
					collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector4.WherePasses(filter4);
					collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector5.WherePasses(filter5);
					collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector6.WherePasses(filter6);

					if (collector.Count() > 0 || collector2.Count() > 0 || collector3.Count() > 0 || collector4.Count() > 0 || collector5.Count() > 0 || collector6.Count() > 0)
					{
						clash_yesA.Add(e);
					}
					else
					{
						clash_noA.Add(e);
					}
					//		 		else if (collector2.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector3.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector4.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector5.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector6.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//bool alreadyExist = false;

					foreach (Element elem in collector)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector2)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector3)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector4)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector5)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector6)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}


					mensaje = mensaje + Environment.NewLine + (collector.Count() + collector2.Count() + collector3.Count()
															+ collector4.Count() + collector5.Count() + collector6.Count()).ToString()
															+ " elements intersect with the selected element ("
															+ e.Name.ToString()
															+ e.Category.Name.ToString() + " id:"
															+ eID.ToString() + ")" + Environment.NewLine;

				}

				string mensaje2 = "";
				string msg = "";
				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
				#endregion
			} // todo con todo documento
			  // todo con todo VIsta ACtiva
			void DYNO_IntersectMultipleElementsToMultipleCategory()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				string mensaje = "";

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					// category Duct.
					ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
					ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
					ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
					ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
					ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
					ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
					// Create a category filter for Ducts
					ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
					ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
					ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
					ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
					ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
					ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);

					LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
					LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);
					LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);
					LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);
					LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);
					LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					//Collector = Clashes
					ExclusionFilter filter = new ExclusionFilter(collectoreID);
					ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter6 = new ExclusionFilter(collectoreID);

					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector2 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector3 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector4 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector5 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector6 = new FilteredElementCollector(doc, activeView.Id);

					collector.OfClass(typeof(Duct));
					collector2.OfClass(typeof(Pipe));
					collector3.OfClass(typeof(Conduit));
					collector4.OfClass(typeof(CableTray));
					collector5.OfClass(typeof(FlexDuct));
					collector6.OfClass(typeof(FlexPipe));

					collector.WherePasses(DU2InstancesFilter);
					collector2.WherePasses(DU2InstancesFilter2);
					collector3.WherePasses(DU2InstancesFilter3);
					collector4.WherePasses(DU2InstancesFilter4);
					collector5.WherePasses(DU2InstancesFilter5);
					collector6.WherePasses(DU2InstancesFilter6);

					collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector.WherePasses(filter);
					collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector2.WherePasses(filter2);
					collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector3.WherePasses(filter3);
					collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector4.WherePasses(filter4);
					collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector5.WherePasses(filter5);
					collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector6.WherePasses(filter6);

					if (collector.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}

					}

					else if (collector2.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector2.First().Category.Name.ToString() + " / ID: " + collector2.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}
					}
					else if (collector3.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector3.First().Category.Name.ToString() + " / ID: " + collector3.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}
					}
					else if (collector4.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector4.First().Category.Name.ToString() + " / ID: " + collector4.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}
					}
					else if (collector5.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector5.First().Category.Name.ToString() + " / ID: " + collector5.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}
					}
					else if (collector6.Count() > 0)
					{
						if (clash_yesA.Contains(e) == false)
						{
							clash_yesA.Add(e);
							Parameter param = e.LookupParameter("Clash Category");

							string elemcategory = collector6.First().Category.Name.ToString() + " / ID: " + collector6.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);
								t.Commit();
							}
						}
					}
					else
					{
						clash_noA.Add(e);
					}



					foreach (Element elem in collector)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}

						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector2)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}


						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector3)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}

						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector4)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}

						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector5)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}

						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector6)
					{
						Parameter param = elem.LookupParameter("Clash Category");

						string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

						using (Transaction t = new Transaction(doc, "Clash Category"))
						{
							t.Start();
							param.Set(elemcategory);
							t.Commit();
						}

						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}


					mensaje = mensaje + Environment.NewLine + (collector.Count() + collector2.Count() + collector3.Count()
															+ collector4.Count() + collector5.Count() + collector6.Count()).ToString()
															+ " elements intersect with the selected element ("
															+ e.Name.ToString()
															+ e.Category.Name.ToString() + " id:"
															+ eID.ToString() + ")" + Environment.NewLine;

				}

				string mensaje2 = "";
				string msg = "";
				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//TaskDialog.Show("Dynoscript", mensaje2);
				#endregion
			} // todo con todo VIsta ACtiva
			  // todo con todo VIsta ACtiva
			List<Element> DYNO_IntersectMultipleElementsToMultipleCategory_ReturnListaClashNO() // retorna: Lista de Elementos con clash NO
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				string mensaje = "";

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					// category Duct.
					ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
					ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
					ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
					ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
					ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
					ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
					// Create a category filter for Ducts
					ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
					ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
					ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
					ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
					ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
					ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);

					LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
					LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);
					LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);
					LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);
					LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);
					LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					//Collector = Clashes
					ExclusionFilter filter = new ExclusionFilter(collectoreID);
					ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
					ExclusionFilter filter6 = new ExclusionFilter(collectoreID);

					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector2 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector3 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector4 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector5 = new FilteredElementCollector(doc, activeView.Id);
					FilteredElementCollector collector6 = new FilteredElementCollector(doc, activeView.Id);

					collector.OfClass(typeof(Duct));
					collector2.OfClass(typeof(Pipe));
					collector3.OfClass(typeof(Conduit));
					collector4.OfClass(typeof(CableTray));
					collector5.OfClass(typeof(FlexDuct));
					collector6.OfClass(typeof(FlexPipe));

					collector.WherePasses(DU2InstancesFilter);
					collector2.WherePasses(DU2InstancesFilter2);
					collector3.WherePasses(DU2InstancesFilter3);
					collector4.WherePasses(DU2InstancesFilter4);
					collector5.WherePasses(DU2InstancesFilter5);
					collector6.WherePasses(DU2InstancesFilter6);

					collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector.WherePasses(filter);
					collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector2.WherePasses(filter2);
					collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector3.WherePasses(filter3);
					collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector4.WherePasses(filter4);
					collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector5.WherePasses(filter5);
					collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector6.WherePasses(filter6);

					if (collector.Count() > 0 || collector2.Count() > 0 || collector3.Count() > 0 || collector4.Count() > 0 || collector5.Count() > 0 || collector6.Count() > 0)
					{
						clash_yesA.Add(e);
					}
					else
					{
						clash_noA.Add(e);
					}
					//		 		else if (collector2.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector3.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector4.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector5.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//		 		else if (collector6.Count() > 0) 
					//		 		{
					//					clash_yesA.Add(e);
					//				}
					//bool alreadyExist = false;

					foreach (Element elem in collector)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector2)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector3)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector4)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector5)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}
					foreach (Element elem in collector6)
					{
						if (clash_yesA.Contains(elem) == false)
						{
							clash_yesA.Add(elem);
						}
						else
						{
							clash_noA.Add(elem);
						}
					}


					mensaje = mensaje + Environment.NewLine + (collector.Count() + collector2.Count() + collector3.Count()
															+ collector4.Count() + collector5.Count() + collector6.Count()).ToString()
															+ " elements intersect with the selected element ("
															+ e.Name.ToString()
															+ e.Category.Name.ToString() + " id:"
															+ eID.ToString() + ")" + Environment.NewLine;

				}

				string mensaje2 = "";
				string msg = "";
				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
				#endregion
				return clash_noA;
			} // retorna: Lista de Elementos con clash NO
			  // todo con todo documento
			void DYNO_IntersectMultipleElementsToMultipleFamilyInstances_doc()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				string numero_ductos = allElements.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					BuiltInCategory[] bics_fi = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				   // BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers
						//BuiltInCategory.OST_Wire,

						//builtInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming,
					};



					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							if (clash_yesA.Contains(e) == false)
							{
								clash_yesA.Add(e);
							}

						}
						else
						{
							clash_noA.Add(e);
						}


						foreach (Element elem in collector)
						{
							if (clash_yesA.Contains(elem) == false)
							{
								clash_yesA.Add(elem);
							}

							else
							{
								clash_noA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}
				string mensaje2 = "";
				string msg = "";

				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			} // todo con todo documento
			  // todo con todo VIsta ACtiva
			void DYNO_IntersectMultipleElementsToMultipleFamilyInstances()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				string numero_ductos = allElements.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					BuiltInCategory[] bics_fi = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				   // BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers
						//BuiltInCategory.OST_Wire,

						//builtInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming,
					};



					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							Parameter param = e.LookupParameter("Clash Category");
							//						Parameter paramID = e.LookupParameter("ID Element");

							string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}

							if (clash_yesA.Contains(e) == false)
							{
								clash_yesA.Add(e);
							}

						}
						else
						{
							clash_noA.Add(e);
						}


						foreach (Element elem in collector)
						{
							Parameter param = elem.LookupParameter("Clash Category");
							//						Parameter paramID = elem.LookupParameter("ID Element");

							string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}

							if (clash_yesA.Contains(elem) == false)
							{
								clash_yesA.Add(elem);
							}

							else
							{
								clash_noA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}
				string mensaje2 = "";
				string msg = "";

				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//TaskDialog.Show("Revit", mensaje2);
			} // todo con todo VIsta ACtiva
			  // todo con todo VIsta ACtiva
			List<Element> DYNO_IntersectMultipleElementsToMultipleFamilyInstances_ReturnListaClashNO() // retorna: Lista de Elementos con clash NO
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct PIpes COnduit Calbe tray Flexduct flex pipes
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				List<Element> allElements = new List<Element>();

				foreach (Element i in ducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in pipes)
				{
					allElements.Add(i);
				}
				foreach (Element i in conduits)
				{
					allElements.Add(i);
				}
				foreach (Element i in cabletrays)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexducts)
				{
					allElements.Add(i);
				}
				foreach (Element i in flexpipes)
				{
					allElements.Add(i);
				}

				string numero_ductos = allElements.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();
				List<Element> clash_noA = new List<Element>();

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					BuiltInCategory[] bics_fi = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				   // BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers
						//BuiltInCategory.OST_Wire,

						//builtInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming,
					};



					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							if (clash_yesA.Contains(e) == false)
							{
								clash_yesA.Add(e);
							}

						}
						else
						{
							clash_noA.Add(e);
						}


						foreach (Element elem in collector)
						{
							if (clash_yesA.Contains(elem) == false)
							{
								clash_yesA.Add(elem);
							}

							else
							{
								clash_noA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}
				string mensaje2 = "";
				string msg = "";

				//			foreach (Element elem in clash_noA)
				//			{
				//				Parameter param = elem.LookupParameter("Clash");
				//			 	string clashNOvacio = "";
				//			 	using (Transaction t = new Transaction(doc, "Clash NO"))
				//			 	{
				//			 		t.Start();
				//					param.Set(clashNOvacio);
				//					t.Commit();
				//			 	}
				//			}

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
				return clash_noA;
			} // retorna: Lista de Elementos con clash NO

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers
					    //BuiltInCategory.OST_Wire,
					};

				List<Element> clashID_familyinstance = new List<Element>();


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						clashID_familyinstance.Add(elem);
					}

				}



				string numero_ductos = clashID_familyinstance.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();

				foreach (Element e in clashID_familyinstance)
				{

					XYZ efi_point = (e.Location as LocationPoint).Point;

					ElementId eID = e.Id;

					List<Solid> listsolid = new List<Solid>();

					//BoundingBoxXYZ bbox = e.get_BoundingBox(null);

					GeometryElement geoEle = e.get_Geometry(new Options());
					//Solid so = null;
					foreach (GeometryObject geomObje1 in geoEle)
					{
						GeometryElement geoInstance = (geomObje1 as GeometryInstance).GetInstanceGeometry();
						if (geoInstance != null)
						{
							foreach (GeometryObject geomObje2 in geoInstance)
							{
								Solid geoSolid = geomObje2 as Solid;
								if (geoSolid != null)
								{
									//foreach (Face face in geoSolid.Faces)
									//{

									listsolid.Add(geoSolid);
									break;

									//}

								}
							}
						}
					}

					foreach (Solid so in listsolid)
					{

						TaskDialog.Show("PUTA MIERDA", so.ToString());
						//				GeometryElement geomElement = e.get_Geometry(new Options() );
						//				
						//				GeometryInstance geomFinstace = geomElement.FirstOrDefault() as GeometryInstance;
						//
						//				GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();
						//				
						//				//Solid solid = gSymbol.First() as Solid;
						//	 
						//	 			
						//	 			foreach(GeometryObject geomObj in gSymbol)
						//	 			{
						//
						//	 			}

						BuiltInCategory[] bics_fi2 = new BuiltInCategory[]
						{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					   // BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers

						};
						foreach (BuiltInCategory bic in bics_fi2)
						{
							// Find intersections between family instances and a selected element
							// category Mechanical Euqipment
							ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
							// Create a category filter for Mechanical Euqipment
							ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

							LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

							ICollection<ElementId> collectoreID = new List<ElementId>();
							if (collectoreID.Contains(e.Id) == false)
							{
								collectoreID.Add(eID);
							}
							ExclusionFilter filter = new ExclusionFilter(collectoreID);

							FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
							collector.OfClass(typeof(FamilyInstance));
							collector.WherePasses(DU2InstancesFilter);
							//collector.WherePasses(new BoundingBoxIntersectsFilter(new Outline(bbox.Min, bbox.Max), 0.01));
							collector.WherePasses(new ElementIntersectsSolidFilter(so)).ToElements(); // Apply intersection filter to find matches
							collector.WherePasses(filter);

							if (collector.Count() > 0)
							{
								if (clash_yesA.Contains(e) == false)
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
							mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																						+ e.Name.ToString()
																						+ e.Category.Name.ToString() + " id:"
																						+ eID.ToString() + ")"
																						+ Environment.NewLine;
						}
					}

				}
				string mensaje2 = "";
				string msg = "";
				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			}

			void DYNO_ClashManage()
			{
				// access the default UIDocument and Document
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				List<string> lista1 = new List<string>(); // seleccion "s" Element
				List<string> lista2 = new List<string>(); // seleccion "s" Family Instance

				List<string> lista3 = new List<string>(); // seleccion "s" Element
				List<string> lista4 = new List<string>(); // seleccion "s" Family Instance

				//			bool checkBox_1 = false;
				//			bool checkBox_2 = false;
				//			bool checkBox_3 = false;

				List<bool> lista_checkBox_1 = new List<bool>();
				List<bool> lista_checkBox_2 = new List<bool>();
				List<bool> lista_checkBox_3 = new List<bool>();

				//add the window form. Create an instance on the form and display it. We need to lead the system.windows.form namaspace
				using (var form = new Form2())
				{
					form.ShowDialog();

					if (form.DialogResult == forms.DialogResult.Cancel) // Boton Cancel
					{
						return;
					}

					if (form.DialogResult == forms.DialogResult.OK) // OK
					{
						bool checkBox_1 = form.checkBox_1; // solo vista activa
						lista_checkBox_1.Add(checkBox_1);

						//					bool checkBox_2 = form.checkBox_2; // todo vs todo
						//					lista_checkBox_2.Add(checkBox_2);

						bool checkBox_3 = form.checkBox_3; // todo documento
						lista_checkBox_3.Add(checkBox_3);

						foreach (string s in form.checkedItems1)
						{
							//TaskDialog.Show("Selected", s);
							if (s == "CableTray" || s == "Conduit" || s == "Duct" || s == "Pipe" || s == "FlexDuct" || s == "FlexPipe")
							{
								lista1.Add(s); // Elements del grupo 1
							}

						}

						foreach (string s in form.checkedItems1)
						{
							//TaskDialog.Show("Selected", s);
							if (s == "CableTrayFitting" || s == "ConduitFitting" || s == "DuctFitting" || s == "DuctTerminal" || s == "ElectricalEquipment" ||
								s == "ElectricalFixtures" || s == "LightingDevices" || s == "LightingFixtures" || s == "MechanicalEquipment" || s == "PipeFitting" ||
								s == "PlumbingFixtures" || s == "SpecialityEquipment" || s == "Sprinklers")
							{
								lista2.Add(s); // Family Instance dele grupo 1
							}
						}

						foreach (string s in form.checkedItems2)
						{
							//TaskDialog.Show("Selected", s);
							if (s == "CableTray" || s == "Conduit" || s == "Duct" || s == "Pipe" || s == "FlexDuct" || s == "FlexPipe")
							{
								lista3.Add(s); // Elements del grupo 2
							}

						}
						foreach (string s in form.checkedItems2)
						{
							//TaskDialog.Show("Selected", s);
							if (s == "CableTrayFitting" || s == "ConduitFitting" || s == "DuctFitting" || s == "DuctTerminal" || s == "ElectricalEquipment" ||
								s == "ElectricalFixtures" || s == "LightingDevices" || s == "LightingFixtures" || s == "MechanicalEquipment" || s == "PipeFitting" ||
								s == "PlumbingFixtures" || s == "SpecialityEquipment" || s == "Sprinklers")
							{
								lista4.Add(s); // Family Instance dele grupo 2
							}
						}

					}

				}

				bool checkBox_1s = lista_checkBox_1.First();
				//			bool checkBox_2s = lista_checkBox_2.First();
				bool checkBox_3s = lista_checkBox_3.First();

				List<BuiltInCategory> UI_list1 = new List<BuiltInCategory>(); // Element	
				List<BuiltInCategory> UI_list2 = new List<BuiltInCategory>(); // Family Instance

				List<BuiltInCategory> UI_list3 = new List<BuiltInCategory>(); // Element
				List<BuiltInCategory> UI_list4 = new List<BuiltInCategory>(); // Family Instance

				// Elements
				BuiltInCategory[] bics = new BuiltInCategory[]
					{
					BuiltInCategory.OST_CableTray,

					BuiltInCategory.OST_Conduit,

					BuiltInCategory.OST_DuctCurves,

					BuiltInCategory.OST_PipeCurves,
					BuiltInCategory.OST_FlexDuctCurves,
					BuiltInCategory.OST_FlexPipeCurves

					};

				foreach (BuiltInCategory bic in bics) // bic  = BuiltInCategory.OST_CableTray
				{
					foreach (string s in lista1) // CableTray
					{
						string sT = "OST_" + s; // OST_CableTray
						if (bic.ToString().Contains(sT)) // ¿"OST_CableTray" dentro de : "BuiltInCategory.OST_CableTray"? : TRUE
						{
							if (!UI_list1.Contains(bic)) // ¿BuiltInCategory.OST_CableTray dentro de : Lista UI_list1? : !FALSE == TRUE
							{
								UI_list1.Add(bic); // agregar BuiltInCategory.OST_CableTray a : Lista UI_list1
							}
						}
					}
					foreach (string s in lista3) // CableTray
					{
						string sT = "OST_" + s; // sT = "OST_CableTray
						if (bic.ToString().Contains(sT)) // ¿"OST_CableTray" dentro de : "BuiltInCategory.OST_CableTray"? : TRUE
						{
							if (!UI_list3.Contains(bic))
							{
								UI_list3.Add(bic); // agregar BuiltInCategory.OST_CableTray a : Lista UI_list2
							}
						}
					}
				}


				// Family Instance
				BuiltInCategory[] bics_finst = new BuiltInCategory[]
					{

					BuiltInCategory.OST_CableTrayFitting,

					BuiltInCategory.OST_ConduitFitting,

					BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,

					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,

					};

				foreach (BuiltInCategory bic in bics_finst) // BuiltInCategory.OST_CableTrayFitting = bic
				{
					foreach (string s in lista2) // CableTrayFitting
					{
						string sT = "OST_" + s; // "OST_CableTrayFitting"
						if (bic.ToString().Contains(sT)) //  ¿"OST_CableTrayFitting" dentro de : "BuiltInCategory.OST_CableTrayFitting"? == TRUE
						{
							if (!UI_list2.Contains(bic))
							{
								UI_list2.Add(bic);
							}
						}
					}
					foreach (string s in lista4)
					{
						string sT = "OST_" + s;
						if (bic.ToString().Contains(sT))
						{
							if (!UI_list4.Contains(bic))
							{
								UI_list4.Add(bic);
							}
						}
					}
				}
				//			List<Element> aa = DYNO_lista_Elements_1(UI_list1); // return : lista de Element
				//			List<Element> bb = DYNO_lista_FamilyInstance_2(UI_list2); // return : lista de FamilyInstance
				string a1 = "Numero total UI_list1 = " + UI_list1.Count().ToString() + Environment.NewLine;
				string a2 = "Numero total UI_list2 = " + UI_list2.Count().ToString() + Environment.NewLine;

				string a3 = "Numero total UI_list3 = " + UI_list3.Count().ToString() + Environment.NewLine;
				string a4 = "Numero total UI_list4 = " + UI_list4.Count().ToString() + Environment.NewLine;

				foreach (BuiltInCategory e in UI_list1)
				{
					a1 = a1 + e.ToString() + " : " + Environment.NewLine;
				}
				foreach (BuiltInCategory e in UI_list2)
				{
					a2 = a2 + e.ToString() + " : " + Environment.NewLine;
				}
				foreach (BuiltInCategory e in UI_list3)
				{
					a3 = a3 + e.ToString() + " : " + Environment.NewLine;
				}
				foreach (BuiltInCategory e in UI_list4)
				{
					a4 = a4 + e.ToString() + " : " + Environment.NewLine;
				}

				//TaskDialog.Show("ULTIMO1", a1 + Environment.NewLine + Environment.NewLine 
				//				                		+ a2 + Environment.NewLine + "------------------------------------------" + Environment.NewLine
				//										+ a3 + Environment.NewLine + Environment.NewLine	
				//										+ a4 + Environment.NewLine + "------------------------------------------" + Environment.NewLine );

				//TaskDialog.Show("ULTIMO2", (UI_list1.Count()+ UI_list2.Count()).ToString()+ Environment.NewLine
				//				                		+ (UI_list3.Count()+ UI_list4.Count()).ToString()+ Environment.NewLine);

				//TaskDialog.Show("ULTIMO3", checkBox_1s.ToString() + Environment.NewLine + checkBox_3s.ToString() + Environment.NewLine);

				if (checkBox_1s && !checkBox_3s) // TRUE Solo Vista Activa
				{
					DYNO_IntersectMultipleElementsToMultipleCategory_UI(UI_list1, UI_list3);
					DYNO_IntersectMultipleElementsToMultipleFamilyInstances_UI(UI_list1, UI_list4);
					//DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_UI( UI_list2, UI_list4);
					//DYNO_IntersectMultipleFamilyInstanceToMultipleCategory_UI(UI_list2, UI_list3);
				}
				else if (checkBox_3s && !checkBox_1s) // TRUE Todo documento
				{
					DYNO_IntersectMultipleElementsToMultipleCategory_UI_doc(UI_list1, UI_list3);
					DYNO_IntersectMultipleElementsToMultipleFamilyInstances_UI_doc(UI_list1, UI_list4);
				}
				//			else if (checkBox_2s && checkBox_1s && !checkBox_3s) // TRUE Todo vs TOdoo Vista activa
				//			{
				//				DYNO_IntersectMultipleElementsToMultipleCategory();
				//				DYNO_IntersectMultipleElementsToMultipleFamilyInstances();
				//			}
				//			else if (checkBox_2s && checkBox_3s && !checkBox_1s) // TRUE Todo vs TOdoo Todo documento
				//			{
				//				DYNO_IntersectMultipleElementsToMultipleCategory_doc();
				//				DYNO_IntersectMultipleElementsToMultipleFamilyInstances_doc();
				//			}
				else
				{

				}



			} // // Only Active View

			void DYNO_IntersectMultipleElementsToMultipleCategory_UI(List<BuiltInCategory> UI_list1_, List<BuiltInCategory> UI_list3_)
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list1 = UI_list1_; // Element grupo 1
				List<BuiltInCategory> UI_list3 = UI_list3_; // Element grupo 2

				List<Element> allElements = new List<Element>();

				foreach (BuiltInCategory bic in UI_list1)
				{
					if (bic == BuiltInCategory.OST_CableTray)
					{
						ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
						ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
						LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);

						FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

						foreach (Element i in cabletrays)
						{
							allElements.Add(i);
						}

					}

					if (bic == BuiltInCategory.OST_Conduit)
					{
						ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
						ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
						LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);

						FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

						foreach (Element i in conduits)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_DuctCurves)
					{
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

						FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

						foreach (Element i in ducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_PipeCurves)
					{
						ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
						ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
						LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);

						FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

						foreach (Element i in pipes)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexDuctCurves)
					{
						ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
						ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
						LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);

						FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

						foreach (Element i in flexducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexPipeCurves)
					{
						ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));
						ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
						LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

						FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

						foreach (Element i in flexpipes)
						{
							allElements.Add(i);
						}
					}
				}


				List<Element> clash_yesA = new List<Element>();

				string mensaje = "";

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					foreach (BuiltInCategory bic in UI_list3)
					{
						if (bic == BuiltInCategory.OST_CableTray)
						{
							ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
							ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
							LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);

							ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector4 = new FilteredElementCollector(doc, activeView.Id);

							collector4.OfClass(typeof(CableTray));
							collector4.WherePasses(DU2InstancesFilter4);
							collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector4.WherePasses(filter4);

							if (collector4.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector4.First().Category.Name.ToString() + " / ID: " + collector4.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector4)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_Conduit)
						{
							ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
							ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
							LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);

							ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector3 = new FilteredElementCollector(doc, activeView.Id);

							collector3.OfClass(typeof(Conduit));
							collector3.WherePasses(DU2InstancesFilter3);
							collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector3.WherePasses(filter3);

							if (collector3.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector3.First().Category.Name.ToString() + " / ID: " + collector3.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector3)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_DuctCurves)
						{
							ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
							ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
							LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

							ExclusionFilter filter = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);

							collector.OfClass(typeof(Duct));
							collector.WherePasses(DU2InstancesFilter);
							collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector.WherePasses(filter);

							if (collector.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_PipeCurves)
						{
							ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
							ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
							LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);

							ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector2 = new FilteredElementCollector(doc, activeView.Id);

							collector2.OfClass(typeof(Pipe));
							collector2.WherePasses(DU2InstancesFilter2);
							collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector2.WherePasses(filter2);
							if (collector2.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector2.First().Category.Name.ToString() + " / ID: " + collector2.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector2)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexDuctCurves)
						{
							ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
							ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
							LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);

							ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector5 = new FilteredElementCollector(doc, activeView.Id);

							collector5.OfClass(typeof(FlexDuct));
							collector5.WherePasses(DU2InstancesFilter5);
							collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector5.WherePasses(filter5);

							if (collector5.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector5.First().Category.Name.ToString() + " / ID: " + collector5.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector5)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexPipeCurves)
						{
							ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
							ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
							LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

							ExclusionFilter filter6 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector6 = new FilteredElementCollector(doc, activeView.Id);

							collector6.OfClass(typeof(FlexPipe));
							collector6.WherePasses(DU2InstancesFilter6);
							collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector6.WherePasses(filter6);

							if (collector6.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector6.First().Category.Name.ToString() + " / ID: " + collector6.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}

								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector6)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}
					}

				}

				string mensaje2 = "";
				string msg = "";

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");

					string clash = "YES";

					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);

						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//TaskDialog.Show("Dynoscript", mensaje2);

				#endregion
				DYNO_SetClashGridLocation();
			} // Elem vs Elem // Only Active View

			void DYNO_IntersectMultipleElementsToMultipleFamilyInstances_UI(List<BuiltInCategory> UI_list1_, List<BuiltInCategory> UI_list4_)
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list1 = UI_list1_; // Element grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				List<Element> allElements = new List<Element>();

				foreach (BuiltInCategory bic in UI_list1)
				{
					if (bic == BuiltInCategory.OST_CableTray)
					{
						ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
						ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
						LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);

						FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

						foreach (Element i in cabletrays)
						{
							allElements.Add(i);
						}

					}

					if (bic == BuiltInCategory.OST_Conduit)
					{
						ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
						ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
						LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);

						FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

						foreach (Element i in conduits)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_DuctCurves)
					{
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

						FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

						foreach (Element i in ducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_PipeCurves)
					{
						ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
						ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
						LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);

						FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

						foreach (Element i in pipes)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexDuctCurves)
					{
						ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
						ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
						LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);

						FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

						foreach (Element i in flexducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexPipeCurves)
					{
						ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));
						ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
						LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

						FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

						foreach (Element i in flexpipes)
						{
							allElements.Add(i);
						}
					}
				}


				List<Element> clash_yesA = new List<Element>();

				List<Element> clash_yesA_element = new List<Element>();
				List<Element> clash_yesA_familyinstance = new List<Element>();

				string mensaje = "";

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					IList<BuiltInCategory> bics_fi = UI_list4;

					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0) // agrega el elemento
						{
							Parameter param = e.LookupParameter("Clash Category");
							Parameter paramID = e.LookupParameter("ID Element");

							string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}



							if (clash_yesA_element.Contains(e) == false)
							{
								clash_yesA_element.Add(e);
								clash_yesA.Add(e);
							}
						}

						foreach (Element elem in collector) // agrega el family instances
						{
							Parameter param = elem.LookupParameter("Clash Category");
							Parameter paramID = elem.LookupParameter("ID Element");

							string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}
							if (clash_yesA_familyinstance.Contains(elem) == false)
							{
								clash_yesA_familyinstance.Add(elem);
								clash_yesA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}

				string mensaje2 = "";
				string msg = "";

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}

				DYNO_SetClashGridLocation_UI(clash_yesA_element, clash_yesA_familyinstance);


				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//TaskDialog.Show("Revit", mensaje2);
				#endregion


			} // Elem vs FamilyInstance // Only Active View


			void DYNO_IntersectMultipleElementsToMultipleCategory_UI_doc(List<BuiltInCategory> UI_list1_, List<BuiltInCategory> UI_list3_)
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list1 = UI_list1_; // Element grupo 1
				List<BuiltInCategory> UI_list3 = UI_list3_; // Element grupo 2

				List<Element> allElements = new List<Element>();

				foreach (BuiltInCategory bic in UI_list1)
				{
					if (bic == BuiltInCategory.OST_CableTray)
					{
						ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
						ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
						LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);

						FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);
						IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

						foreach (Element i in cabletrays)
						{
							allElements.Add(i);
						}

					}

					if (bic == BuiltInCategory.OST_Conduit)
					{
						ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
						ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
						LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);

						FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);
						IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

						foreach (Element i in conduits)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_DuctCurves)
					{
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

						FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

						foreach (Element i in ducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_PipeCurves)
					{
						ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
						ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
						LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);

						FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);
						IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

						foreach (Element i in pipes)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexDuctCurves)
					{
						ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
						ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
						LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);

						FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);
						IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

						foreach (Element i in flexducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexPipeCurves)
					{
						ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));
						ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
						LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

						FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);
						IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

						foreach (Element i in flexpipes)
						{
							allElements.Add(i);
						}
					}
				}


				List<Element> clash_yesA = new List<Element>();

				string mensaje = "";

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					foreach (BuiltInCategory bic in UI_list3)
					{
						if (bic == BuiltInCategory.OST_CableTray)
						{
							ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
							ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
							LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);

							ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector4 = new FilteredElementCollector(doc);

							collector4.OfClass(typeof(CableTray));
							collector4.WherePasses(DU2InstancesFilter4);
							collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector4.WherePasses(filter4);

							if (collector4.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector4.First().Category.Name.ToString() + " / ID: " + collector4.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector4)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_Conduit)
						{
							ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
							ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
							LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);

							ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector3 = new FilteredElementCollector(doc);

							collector3.OfClass(typeof(Conduit));
							collector3.WherePasses(DU2InstancesFilter3);
							collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector3.WherePasses(filter3);

							if (collector3.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector3.First().Category.Name.ToString() + " / ID: " + collector3.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector3)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_DuctCurves)
						{
							ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
							ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
							LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

							ExclusionFilter filter = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector = new FilteredElementCollector(doc);

							collector.OfClass(typeof(Duct));
							collector.WherePasses(DU2InstancesFilter);
							collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector.WherePasses(filter);

							if (collector.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_PipeCurves)
						{
							ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
							ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
							LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);

							ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector2 = new FilteredElementCollector(doc);

							collector2.OfClass(typeof(Pipe));
							collector2.WherePasses(DU2InstancesFilter2);
							collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector2.WherePasses(filter2);
							if (collector2.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector2.First().Category.Name.ToString() + " / ID: " + collector2.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector2)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexDuctCurves)
						{
							ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
							ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
							LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);

							ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector5 = new FilteredElementCollector(doc);

							collector5.OfClass(typeof(FlexDuct));
							collector5.WherePasses(DU2InstancesFilter5);
							collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector5.WherePasses(filter5);

							if (collector5.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector5.First().Category.Name.ToString() + " / ID: " + collector5.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector5)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexPipeCurves)
						{
							ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
							ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
							LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

							ExclusionFilter filter6 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector6 = new FilteredElementCollector(doc);

							collector6.OfClass(typeof(FlexPipe));
							collector6.WherePasses(DU2InstancesFilter6);
							collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector6.WherePasses(filter6);

							if (collector6.Count() > 0)
							{
								Parameter param = e.LookupParameter("Clash Category");
								Parameter paramID = e.LookupParameter("ID Element");

								string elemcategory = collector6.First().Category.Name.ToString() + " / ID: " + collector6.First().Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}

								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector6)
							{
								Parameter param = elem.LookupParameter("Clash Category");
								Parameter paramID = elem.LookupParameter("ID Element");

								string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

								using (Transaction t = new Transaction(doc, "Clash Category"))
								{
									t.Start();
									param.Set(elemcategory);

									t.Commit();
								}
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}
					}

				}

				string mensaje2 = "";
				string msg = "";

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");

					string clash = "YES";

					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);

						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
				#endregion
				DYNO_SetClashGridLocation();
			} // Elem vs Elem // Todo documento

			void DYNO_IntersectMultipleElementsToMultipleFamilyInstances_UI_doc(List<BuiltInCategory> UI_list1_, List<BuiltInCategory> UI_list4_)
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list1 = UI_list1_; // Element grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				List<Element> allElements = new List<Element>();

				foreach (BuiltInCategory bic in UI_list1)
				{
					if (bic == BuiltInCategory.OST_CableTray)
					{
						ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
						ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
						LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);

						FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);
						IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

						foreach (Element i in cabletrays)
						{
							allElements.Add(i);
						}

					}

					if (bic == BuiltInCategory.OST_Conduit)
					{
						ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
						ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
						LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);

						FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);
						IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

						foreach (Element i in conduits)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_DuctCurves)
					{
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

						FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

						foreach (Element i in ducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_PipeCurves)
					{
						ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
						ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
						LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);

						FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);
						IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

						foreach (Element i in pipes)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexDuctCurves)
					{
						ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
						ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
						LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);

						FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);
						IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

						foreach (Element i in flexducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexPipeCurves)
					{
						ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));
						ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
						LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

						FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);
						IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

						foreach (Element i in flexpipes)
						{
							allElements.Add(i);
						}
					}
				}


				List<Element> clash_yesA = new List<Element>();

				List<Element> clash_yesA_element = new List<Element>();
				List<Element> clash_yesA_familyinstance = new List<Element>();

				string mensaje = "";

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					IList<BuiltInCategory> bics_fi = UI_list4;

					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0) // agrega el elemento
						{
							Parameter param = e.LookupParameter("Clash Category");
							Parameter paramID = e.LookupParameter("ID Element");

							string elemcategory = collector.First().Category.Name.ToString() + " / ID: " + collector.First().Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}



							if (clash_yesA_element.Contains(e) == false)
							{
								clash_yesA_element.Add(e);
								clash_yesA.Add(e);
							}
						}

						foreach (Element elem in collector) // agrega el family instances
						{
							Parameter param = elem.LookupParameter("Clash Category");
							Parameter paramID = elem.LookupParameter("ID Element");

							string elemcategory = e.Category.Name.ToString() + " / ID: " + e.Id.ToString();

							using (Transaction t = new Transaction(doc, "Clash Category"))
							{
								t.Start();
								param.Set(elemcategory);

								t.Commit();
							}
							if (clash_yesA_familyinstance.Contains(elem) == false)
							{
								clash_yesA_familyinstance.Add(elem);
								clash_yesA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}

				string mensaje2 = "";
				string msg = "";

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}

				DYNO_SetClashGridLocation_UI(clash_yesA_element, clash_yesA_familyinstance);


				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//TaskDialog.Show("Revit", mensaje2);
				#endregion


			} // Elem vs FamilyInstance // Todo documento



			void DYNO_IntersectMultipleFamilyInstanceToMultipleCategory_UI(List<BuiltInCategory> UI_list1_, List<BuiltInCategory> UI_list4_)
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list1 = UI_list1_; // Family Instance grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Element grupo 2

				List<Element> allElements = new List<Element>();

				foreach (BuiltInCategory bic in UI_list4)
				{
					if (bic == BuiltInCategory.OST_CableTray)
					{
						ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
						ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
						LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);

						FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

						foreach (Element i in cabletrays)
						{
							allElements.Add(i);
						}

					}

					if (bic == BuiltInCategory.OST_Conduit)
					{
						ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
						ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
						LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);

						FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

						foreach (Element i in conduits)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_DuctCurves)
					{
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

						FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // DUCTS

						foreach (Element i in ducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_PipeCurves)
					{
						ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
						ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
						LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);

						FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

						foreach (Element i in pipes)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexDuctCurves)
					{
						ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
						ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
						LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);

						FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

						foreach (Element i in flexducts)
						{
							allElements.Add(i);
						}

					}
					if (bic == BuiltInCategory.OST_FlexPipeCurves)
					{
						ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));
						ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
						LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

						FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

						foreach (Element i in flexpipes)
						{
							allElements.Add(i);
						}
					}
				}


				List<Element> clash_yesA = new List<Element>();

				string mensaje = "";

				// foreach ( Element e in ducts) // OUT: mensaje2  y  clash_yesA<List>
				#region 

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					IList<BuiltInCategory> bics_fi = UI_list1;

					foreach (BuiltInCategory bic in bics_fi)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}


						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc);
						collector.OfClass(typeof(FamilyInstance));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																									 //collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							if (clash_yesA.Contains(e) == false)
							{
								clash_yesA.Add(e);
							}
						}

						foreach (Element elem in collector)
						{
							if (clash_yesA.Contains(elem) == false)
							{
								clash_yesA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}

				string mensaje2 = "";
				string msg = "";

				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
				#endregion
			} // Family Instance vs Element

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_UI_nofunciona(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list4_) // Family Instance vs Family Instance
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				// FAMILY INSTANCES

				IList<BuiltInCategory> bics_familyIns = UI_list2;

				List<ElementId> clashID_familyinstance = new List<ElementId>();


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						clashID_familyinstance.Add(elem.Id);
					}

				}



				string numero_ductos = clashID_familyinstance.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();

				foreach (ElementId eID in clashID_familyinstance)
				{

					//ElementId eID = e.Id;
					Element e = doc.GetElement(eID);
					//LocationPoint p = e.Location as LocationPoint;

					//XYZ xyz = new XYZ(p.Point.X, p.Point.Y,p.Point.Z);

					//XYZ p = so.ComputeCentroid();
					//FamilyInstance efi = e as FamilyInstance;

					GeometryElement geomElement = e.get_Geometry(new Options());

					GeometryInstance geomFinstace = geomElement.First() as GeometryInstance;

					GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();

					//Solid solid = gSymbol.First() as Solid;

					Solid solid = null;
					foreach (GeometryObject geomObj in gSymbol)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					IList<BuiltInCategory> bics_fi2 = UI_list4;

					foreach (BuiltInCategory bic in bics_fi2)
					{
						// Find intersections between family instances and a selected element
						// category Mechanical Euqipment
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
						// Create a category filter for Mechanical Euqipment
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						if (collectoreID.Contains(e.Id) == false)
						{
							collectoreID.Add(eID);
						}
						//		 			
						//		 
						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);

						if (bic == BuiltInCategory.OST_CableTray)
						{
							collector.OfClass(typeof(CableTray));
						}
						if (bic == BuiltInCategory.OST_DuctCurves)
						{
							collector.OfClass(typeof(Duct));
						}
						if (bic == BuiltInCategory.OST_PipeCurves)
						{
							collector.OfClass(typeof(Pipe));
						}
						if (bic == BuiltInCategory.OST_Conduit)
						{
							collector.OfClass(typeof(Conduit));
						}
						if (bic == BuiltInCategory.OST_FlexPipeCurves)
						{
							collector.OfClass(typeof(FlexPipe));
						}
						if (bic == BuiltInCategory.OST_FlexPipeCurves)
						{
							collector.OfClass(typeof(FlexPipe));
						}

						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
						collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							if (clash_yesA.Contains(e) == false)
							{
								clash_yesA.Add(e);
							}
						}

						foreach (Element elem in collector)
						{
							if (clash_yesA.Contains(elem) == false)
							{
								clash_yesA.Add(elem);
							}
						}
						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")"
																					+ Environment.NewLine;
					}
				}


				string mensaje2 = "";
				string msg = "";
				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						if (!(param.AsString() == "YES"))
						{
							param.Set(clash);
						}

						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			} // Family Instance vs Family Instance

			void DYNO_IntersectMultipleFamilyInstanceToMultipleCategory_UI_nofunciona(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list3_) // Family Instance vs Element
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				List<BuiltInCategory> UI_list3 = UI_list3_; // Element grupo 2

				List<Element> allElements = new List<Element>();

				// FAMILY INSTANCES

				IList<BuiltInCategory> bics_familyIns = UI_list2;

				List<ElementId> clashID_familyinstance = new List<ElementId>();


				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						clashID_familyinstance.Add(elem.Id);
					}

				}



				string numero_ductos = clashID_familyinstance.Count().ToString();
				string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();

				foreach (ElementId eID in clashID_familyinstance)
				{
					Element elem = doc.GetElement(eID);
					allElements.Add(elem);
				}

				foreach (Element e in allElements)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					foreach (BuiltInCategory bic in UI_list3)
					{
						if (bic == BuiltInCategory.OST_CableTray)
						{
							ElementClassFilter DUFilter4 = new ElementClassFilter(typeof(CableTray));
							ElementCategoryFilter DU2Categoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
							LogicalAndFilter DU2InstancesFilter4 = new LogicalAndFilter(DUFilter4, DU2Categoryfilter4);

							ExclusionFilter filter4 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector4 = new FilteredElementCollector(doc, activeView.Id);

							collector4.OfClass(typeof(CableTray));
							collector4.WherePasses(DU2InstancesFilter4);
							collector4.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector4.WherePasses(filter4);

							if (collector4.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector4)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_Conduit)
						{
							ElementClassFilter DUFilter3 = new ElementClassFilter(typeof(Conduit));
							ElementCategoryFilter DU2Categoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
							LogicalAndFilter DU2InstancesFilter3 = new LogicalAndFilter(DUFilter3, DU2Categoryfilter3);

							ExclusionFilter filter3 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector3 = new FilteredElementCollector(doc, activeView.Id);

							collector3.OfClass(typeof(Conduit));
							collector3.WherePasses(DU2InstancesFilter3);
							collector3.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector3.WherePasses(filter3);

							if (collector3.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector3)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_DuctCurves)
						{
							ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
							ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
							LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

							ExclusionFilter filter = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);

							collector.OfClass(typeof(Duct));
							collector.WherePasses(DU2InstancesFilter);
							collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector.WherePasses(filter);

							if (collector.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}

						if (bic == BuiltInCategory.OST_PipeCurves)
						{
							ElementClassFilter DUFilter2 = new ElementClassFilter(typeof(Pipe));
							ElementCategoryFilter DU2Categoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
							LogicalAndFilter DU2InstancesFilter2 = new LogicalAndFilter(DUFilter2, DU2Categoryfilter2);

							ExclusionFilter filter2 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector2 = new FilteredElementCollector(doc, activeView.Id);

							collector2.OfClass(typeof(Pipe));
							collector2.WherePasses(DU2InstancesFilter2);
							collector2.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector2.WherePasses(filter2);
							if (collector2.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector2)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexDuctCurves)
						{
							ElementClassFilter DUFilter5 = new ElementClassFilter(typeof(FlexDuct));
							ElementCategoryFilter DU2Categoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
							LogicalAndFilter DU2InstancesFilter5 = new LogicalAndFilter(DUFilter5, DU2Categoryfilter5);

							ExclusionFilter filter5 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector5 = new FilteredElementCollector(doc, activeView.Id);

							collector5.OfClass(typeof(FlexDuct));
							collector5.WherePasses(DU2InstancesFilter5);
							collector5.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector5.WherePasses(filter5);

							if (collector5.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector5)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}

						}

						if (bic == BuiltInCategory.OST_FlexPipeCurves)
						{
							ElementClassFilter DUFilter6 = new ElementClassFilter(typeof(FlexPipe));
							ElementCategoryFilter DU2Categoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);
							LogicalAndFilter DU2InstancesFilter6 = new LogicalAndFilter(DUFilter6, DU2Categoryfilter6);

							ExclusionFilter filter6 = new ExclusionFilter(collectoreID);
							FilteredElementCollector collector6 = new FilteredElementCollector(doc, activeView.Id);

							collector6.OfClass(typeof(FlexPipe));
							collector6.WherePasses(DU2InstancesFilter6);
							collector6.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
							collector6.WherePasses(filter6);

							if (collector6.Count() > 0)
							{
								if (!clash_yesA.Contains(e))
								{
									clash_yesA.Add(e);
								}
							}

							foreach (Element elem in collector6)
							{
								if (clash_yesA.Contains(elem) == false)
								{
									clash_yesA.Add(elem);
								}
							}
						}
					}

				}


				string mensaje2 = "";
				string msg = "";
				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						if (!(param.AsString() == "YES"))
						{
							param.Set(clash);
						}

						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			} // Family Instance vs Element





			void DYNO_CreateClashFilterMultipleElementsInMultipleViews_UI(List<View3D> lista_3dviews) // Crea filtros en la vista activa
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				foreach (View3D view3d in lista_3dviews)
				{

					//View activeView = this.ActiveUIDocument.ActiveView;
					View activeView = view3d;

					List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					using (Transaction ta = new Transaction(doc, "create clash filter view"))
					{
						ta.Start();

						//activeView.Name = "COORD";

						FilteredElementCollector collector = new FilteredElementCollector(doc);

						Parameter param = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash");
						Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved");

						FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

						List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
						foreach (ParameterFilterElement e in collector_filterview)
						{
							lista_filtros.Add(e);
						}


						List<FilterRule> filterRules = new List<FilterRule>();
						List<FilterRule> filterRules_no = new List<FilterRule>();


						filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param.Id, "YES", true));
						filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)0)); // Clash Solved , EQUAL,  False(int=0),
																											   //filterRules_no.Add(ParameterFilterRuleFactory.CreateNotEqualsRule(param.Id,"YES", true));
						filterRules_no.Add(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true));

						for (int i = 0; i < lista_filtros.Count(); i++)
						{
							if (lista_filtros[i].Name == "CLASH YES FILTER")
							{
								lista_ParameterFilterElement.Add(lista_filtros[i]);
								i = lista_filtros.Count();
								break;
							}

						}

						if (lista_ParameterFilterElement.Count() == 0)
						{
							ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
							//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

							lista_ParameterFilterElement.Add(parameterFilterElement);
						}



						for (int i = 0; i < lista_filtros.Count(); i++)
						{
							if (lista_filtros[i].Name == "CLASH NO FILTER")
							{
								lista_ParameterFilterElement_no.Add(lista_filtros[i]);
								i = lista_filtros.Count();
								break;
							}

						}
						if (lista_ParameterFilterElement_no.Count() == 0)
						{
							//ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
							ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

							lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
						}

						ParameterFilterElement aa = lista_ParameterFilterElement.First();
						ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();

						//filterRules.First();
						// si no existe parameterfilterelement llamado "CLASH YES FILTER", aplicar el mismo filtro



						//lista_ParameterFilterElement.Add(parameterFilterElement);
						//lista_ParameterFilterElement_no.Add(parameterFilterElement_no);


						OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
						ogs3.SetProjectionLineColor(new Color(250, 0, 0));
						ogs3.SetSurfaceForegroundPatternColor(new Color(250, 0, 0));
						ogs3.SetSurfaceForegroundPatternVisible(true);
						ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

						OverrideGraphicSettings ogs4 = new OverrideGraphicSettings();
						ogs4.SetProjectionLineColor(new Color(192, 192, 192));
						ogs4.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
						ogs4.SetSurfaceForegroundPatternVisible(true);
						ogs4.SetSurfaceForegroundPatternId(solidFillPattern.Id);
						ogs4.SetHalftone(true);

						activeView.AddFilter(aa.Id);
						activeView.AddFilter(aa_no.Id);


						activeView.SetFilterOverrides(aa.Id, ogs3);
						activeView.SetFilterOverrides(aa_no.Id, ogs4);


						ta.Commit();
					}
				}


			}// Crea filtros en la lista de vistas ingresadas

			void DYNO_CreateClashSOLVEDFilterMultipleElementsInMultipleViews_UI(List<View3D> lista_3dviews) // Crea filtros en la vista activa
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				foreach (View3D view3d in lista_3dviews)
				{

					//View activeView = this.ActiveUIDocument.ActiveView;
					View activeView = view3d;

					//			 	List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					//				List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					using (Transaction ta = new Transaction(doc, "create clash solved filter view"))
					{
						ta.Start();

						//activeView.Name = "COORD";
						List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
						List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

						FilteredElementCollector collector = new FilteredElementCollector(doc);

						Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved");

						FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

						List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
						foreach (ParameterFilterElement e in collector_filterview)
						{
							lista_filtros.Add(e);
						}


						List<FilterRule> filterRules = new List<FilterRule>();

						filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1)); // Clash Solved , EQUAL,  False(int=0),



						for (int i = 0; i < lista_filtros.Count(); i++)
						{
							if (lista_filtros[i].Name == "CLASH SOLVED FILTER")
							{
								lista_ParameterFilterElement.Add(lista_filtros[i]);
								i = lista_filtros.Count();
								break;
							}

						}

						if (lista_ParameterFilterElement.Count() == 0)
						{
							ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, filterRules);
							//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

							lista_ParameterFilterElement.Add(parameterFilterElement);
						}



						ParameterFilterElement aa = lista_ParameterFilterElement.First();


						OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
						ogs3.SetProjectionLineColor(new Color(192, 192, 192));
						ogs3.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
						ogs3.SetSurfaceForegroundPatternVisible(true);
						ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

						activeView.AddFilter(aa.Id);

						activeView.SetFilterOverrides(aa.Id, ogs3);



						ta.Commit();
					}
				}


			}// Crea filtros en la lista de vistas ingresadas


			void DYNO_CreateClashFilterMultipleElementsInView_UI(View3D view_3d) // Crea filtros en la vista activa
			{

				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				View activeView = view_3d;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				using (Transaction ta = new Transaction(doc, "create clash filter view"))
				{
					ta.Start();

					//activeView.Name = "COORD";
					List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					FilteredElementCollector collector = new FilteredElementCollector(doc);

					Parameter param = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash"); // Clash
					Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved"); // Clash Solved

					FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

					List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
					foreach (ParameterFilterElement e in collector_filterview)
					{
						lista_filtros.Add(e);
					}


					List<FilterRule> filterRules = new List<FilterRule>();
					List<FilterRule> filterRules_no = new List<FilterRule>();


					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param.Id, "YES", true)); // Clash , EQUAL,  "YES", 
					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)0)); // Clash Solved , EQUAL,  False(int=0),
																										   //filterRules_no.Add(ParameterFilterRuleFactory.CreateNotEqualsRule(param.Id,"YES", true));
					filterRules_no.Add(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true));

					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH YES FILTER")
						{
							lista_ParameterFilterElement.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement.Count() == 0)
					{
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
						//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement.Add(parameterFilterElement);
					}



					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH NO FILTER")
						{
							lista_ParameterFilterElement_no.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}
					if (lista_ParameterFilterElement_no.Count() == 0)
					{
						//ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
						ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
					}

					ParameterFilterElement aa = lista_ParameterFilterElement.First();
					ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();

					//filterRules.First();
					// si no existe parameterfilterelement llamado "CLASH YES FILTER", aplicar el mismo filtro



					//lista_ParameterFilterElement.Add(parameterFilterElement);
					//lista_ParameterFilterElement_no.Add(parameterFilterElement_no);


					OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
					ogs3.SetProjectionLineColor(new Color(250, 0, 0));
					ogs3.SetSurfaceForegroundPatternColor(new Color(250, 0, 0));
					ogs3.SetSurfaceForegroundPatternVisible(true);
					ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

					OverrideGraphicSettings ogs4 = new OverrideGraphicSettings();
					ogs4.SetProjectionLineColor(new Color(192, 192, 192));
					ogs4.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
					ogs4.SetSurfaceForegroundPatternVisible(true);
					ogs4.SetSurfaceForegroundPatternId(solidFillPattern.Id);
					ogs4.SetHalftone(true);

					activeView.AddFilter(aa.Id);
					activeView.AddFilter(aa_no.Id);


					activeView.SetFilterOverrides(aa.Id, ogs3);
					activeView.SetFilterOverrides(aa_no.Id, ogs4);


					ta.Commit();
				}

			}// Crea filtros en la vista ingresada

			void DYNO_CreateClashSOLVEDFilterMultipleElementsInView_UI(View3D view_3d) // Crea filtros en la vista activa
			{

				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				View activeView = view_3d;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				using (Transaction ta = new Transaction(doc, "create clash solved filter view"))
				{
					ta.Start();

					//activeView.Name = "COORD";
					List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					FilteredElementCollector collector = new FilteredElementCollector(doc);

					Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved");

					FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

					List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
					foreach (ParameterFilterElement e in collector_filterview)
					{
						lista_filtros.Add(e);
					}


					List<FilterRule> filterRules = new List<FilterRule>();

					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1)); // Clash Solved , EQUAL,  False(int=0),



					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH SOLVED FILTER")
						{
							lista_ParameterFilterElement.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement.Count() == 0)
					{
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, filterRules);
						//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement.Add(parameterFilterElement);
					}



					ParameterFilterElement aa = lista_ParameterFilterElement.First();


					OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
					ogs3.SetProjectionLineColor(new Color(192, 192, 192));
					ogs3.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
					ogs3.SetSurfaceForegroundPatternVisible(true);
					ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

					activeView.AddFilter(aa.Id);

					activeView.SetFilterOverrides(aa.Id, ogs3);



					ta.Commit();
				}

			}// Crea filtros en la vista ingresada


			void DYNO_CreateClashFilterMultipleElementsInView() // Crea filtros en la vista activa
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;
				//View activeView = view_3d;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				using (Transaction ta = new Transaction(doc, "create clash filter view"))
				{
					ta.Start();

					//activeView.Name = "COORD";
					List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					FilteredElementCollector collector = new FilteredElementCollector(doc);

					Parameter param = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash");
					Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved");

					FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

					List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
					foreach (ParameterFilterElement e in collector_filterview)
					{
						lista_filtros.Add(e);
					}


					List<FilterRule> filterRules = new List<FilterRule>();
					List<FilterRule> filterRules_no = new List<FilterRule>();


					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param.Id, "YES", true));
					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)0)); // Clash Solved , EQUAL,  False(int=0),
																										   //filterRules_no.Add(ParameterFilterRuleFactory.CreateNotEqualsRule(param.Id,"YES", true));
					filterRules_no.Add(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true));


					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH YES FILTER")
						{
							lista_ParameterFilterElement.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement.Count() == 0)
					{
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
						//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement.Add(parameterFilterElement);
					}



					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH NO FILTER")
						{
							lista_ParameterFilterElement_no.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}
					if (lista_ParameterFilterElement_no.Count() == 0)
					{
						//ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, filterRules);
						ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
					}

					ParameterFilterElement aa = lista_ParameterFilterElement.First();
					ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();

					//filterRules.First();
					// si no existe parameterfilterelement llamado "CLASH YES FILTER", aplicar el mismo filtro



					//lista_ParameterFilterElement.Add(parameterFilterElement);
					//lista_ParameterFilterElement_no.Add(parameterFilterElement_no);


					OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
					ogs3.SetProjectionLineColor(new Color(250, 0, 0));
					ogs3.SetSurfaceForegroundPatternColor(new Color(250, 0, 0));
					ogs3.SetSurfaceForegroundPatternVisible(true);
					ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

					OverrideGraphicSettings ogs4 = new OverrideGraphicSettings();
					ogs4.SetProjectionLineColor(new Color(192, 192, 192));
					ogs4.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
					ogs4.SetSurfaceForegroundPatternVisible(true);
					ogs4.SetSurfaceForegroundPatternId(solidFillPattern.Id);
					ogs4.SetHalftone(true);

					activeView.AddFilter(aa.Id);
					activeView.AddFilter(aa_no.Id);


					activeView.SetFilterOverrides(aa.Id, ogs3);
					activeView.SetFilterOverrides(aa_no.Id, ogs4);


					ta.Commit();
				}
			}// Crea filtros en la vista activa

			void DYNO_CreateClashSOLVEDFilterMultipleElementsInView() // Crea filtros en la vista activa
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;
				//View activeView = view_3d;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				List<ElementId> cats = new List<ElementId>();

				cats.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_Conduit));
				cats.Add(new ElementId(BuiltInCategory.OST_CableTray));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexDuctCurves));
				cats.Add(new ElementId(BuiltInCategory.OST_FlexPipeCurves));

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    //BuiltInCategory.OST_FlexDuctCurves,
					    //BuiltInCategory.OST_FlexPipeCurves,
					    BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					cats.Add(new ElementId(bic));
				}



				using (Transaction ta = new Transaction(doc, "create clash solved filter view"))
				{
					ta.Start();

					//activeView.Name = "COORD";
					List<ParameterFilterElement> lista_ParameterFilterElement = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement_no = new List<ParameterFilterElement>();

					FilteredElementCollector collector = new FilteredElementCollector(doc);

					Parameter param_solved = collector.OfClass(typeof(Duct)).FirstElement().LookupParameter("Clash Solved");

					FilteredElementCollector collector_filterview = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement));

					List<ParameterFilterElement> lista_filtros = new List<ParameterFilterElement>();
					foreach (ParameterFilterElement e in collector_filterview)
					{
						lista_filtros.Add(e);
					}


					List<FilterRule> filterRules = new List<FilterRule>();

					filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1)); // Clash Solved , EQUAL,  False(int=0),

					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH SOLVED FILTER")
						{
							lista_ParameterFilterElement.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement.Count() == 0)
					{
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, filterRules);
						//ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, filterRules_no);

						lista_ParameterFilterElement.Add(parameterFilterElement);
					}



					ParameterFilterElement aa = lista_ParameterFilterElement.First();


					OverrideGraphicSettings ogs3 = new OverrideGraphicSettings();
					ogs3.SetProjectionLineColor(new Color(192, 192, 192));
					ogs3.SetSurfaceForegroundPatternColor(new Color(192, 192, 192));
					ogs3.SetSurfaceForegroundPatternVisible(true);
					ogs3.SetSurfaceForegroundPatternId(solidFillPattern.Id);

					activeView.AddFilter(aa.Id);

					activeView.SetFilterOverrides(aa.Id, ogs3);



					ta.Commit();
				}
			}// Crea filtro cLASH solVED en la vista activa


			void DYNO_ClashComments()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
						BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				IList<Element> clash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> clash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							clash.Add(elem);
						}
						else
						{
							clash_no.Add(elem);
						}
					}

				}

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}

				//activeView.Name = activeView.Name.ToString() + " - Pendiente";

				using (var form = new Form1())
				{
					form.ShowDialog();

					if (form.DialogResult == forms.DialogResult.Cancel) return;

					string param_value = form.textString.ToString();

					foreach (Element e in clash)
					{
						Parameter param = e.LookupParameter("Clash Comments");
						//Parameter param2 = e.LookupParameter("Clash Solved");

						using (Transaction t = new Transaction(doc, "Set Comment value to Clash comments paramtere in Active View"))
						{
							t.Start();
							param.Set(param_value);
							//param2.Set(0);
							t.Commit();
						}
					}
				}
				using (Transaction t = new Transaction(doc, "Cambiar nombre Comentado"))
				{
					t.Start();
					activeView.Name = activeView.Name.ToString() + " - Comentado";
					t.Commit();
				}

			}

			void DYNO_SetClashGridLocation_doc()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;
				// Get Active View
				View activeView = uidoc.ActiveView;

				// get elements with "clash" parameter value == "YES"
				IList<Element> clash = new List<Element>();
				IList<Element> clash_familyinstance = new List<Element>();
				IList<ElementId> clashID_elements = new List<ElementId>();
				IList<ElementId> clashID_familyinstance = new List<ElementId>();

				// get elements with "clash" parameter value == "NO"
				IList<Element> clash_no = new List<Element>();

				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}

				foreach (Element elem in clash)
				{
					clashID_elements.Add(elem.Id);
				}

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							clash_familyinstance.Add(elem);
						}
						else
						{
							clash_no.Add(elem);
						}
					}

				}

				foreach (Element elem in clash_familyinstance)
				{
					clashID_familyinstance.Add(elem.Id);
				}



				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				XYZ p1 = new XYZ();
				XYZ p2 = new XYZ();

				foreach (ElementId eid in clashID_elements)
				{
					Element e = doc.GetElement(eid);

					Options op = new Options();
					//            		op.View = doc.ActiveView;
					//            		op.ComputeReferences = true;
					GeometryElement gm = e.get_Geometry(op);
					Solid so = gm.First() as Solid;
					XYZ p = so.ComputeCentroid();

					XYZ xyz = new XYZ(p.X, p.Y, 0);

					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Centroid Point : (" + p.X.ToString() + ", " + p.Y.ToString() + ", 0)  :  " + "Clash Grid Location : " + param_value + "  Category:" + e.Category.Name.ToString() + "  ID : " + e.Id.ToString() + Environment.NewLine;
				}

				foreach (ElementId eid in clashID_familyinstance)
				{
					Element e = doc.GetElement(eid);
					LocationPoint p = e.Location as LocationPoint;

					XYZ xyz = new XYZ(p.Point.X, p.Point.Y, 0);
					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Clash Grid Location : " + param_value + "      Category:" + e.Category.Name.ToString() + "      ID : " + e.Id.ToString() + Environment.NewLine;
				}

				//TaskDialog.Show("Dynosript", msg);

			} // Todos los elemnentos con CLASH = YES del documento

			void DYNO_SetClashGridLocation()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;
				// Get Active View
				View activeView = uidoc.ActiveView;

				// get elements with "clash" parameter value == "YES"
				IList<Element> clash = new List<Element>();
				IList<Element> clash_familyinstance = new List<Element>();
				IList<ElementId> clashID_elements = new List<ElementId>();
				IList<ElementId> clashID_familyinstance = new List<ElementId>();

				// get elements with "clash" parameter value == "NO"
				IList<Element> clash_no = new List<Element>();

				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}

				foreach (Element elem in clash)
				{
					clashID_elements.Add(elem.Id);
				}

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Element elem in mechanicalequipment)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							clash_familyinstance.Add(elem);
						}
						else
						{
							clash_no.Add(elem);
						}
					}

				}

				foreach (Element elem in clash_familyinstance)
				{
					clashID_familyinstance.Add(elem.Id);
				}



				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				XYZ p1 = new XYZ();
				XYZ p2 = new XYZ();

				foreach (ElementId eid in clashID_elements)
				{
					Element e = doc.GetElement(eid);

					Options op = new Options();
					//            		op.View = doc.ActiveView;
					//            		op.ComputeReferences = true;
					GeometryElement gm = e.get_Geometry(op);
					Solid so = gm.First() as Solid;
					XYZ p = so.ComputeCentroid();

					XYZ xyz = new XYZ(p.X, p.Y, 0);

					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Centroid Point : (" + p.X.ToString() + ", " + p.Y.ToString() + ", 0)  :  " + "Clash Grid Location : " + param_value + "  Category:" + e.Category.Name.ToString() + "  ID : " + e.Id.ToString() + Environment.NewLine;
				}

				foreach (ElementId eid in clashID_familyinstance)
				{
					Element e = doc.GetElement(eid);
					LocationPoint p = e.Location as LocationPoint;

					XYZ xyz = new XYZ(p.Point.X, p.Point.Y, 0);
					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Clash Grid Location : " + param_value + "      Category:" + e.Category.Name.ToString() + "      ID : " + e.Id.ToString() + Environment.NewLine;
				}

				//TaskDialog.Show("Dynoscript", msg);

			} // Todos los elemnentos con CLASH = YES de la vista activa.

			void DYNO_SetClashGridLocation_UI(IList<Element> clash_, IList<Element> clash_familyinstance_)
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;
				// Get Active View
				View activeView = uidoc.ActiveView;

				List<ElementId> clashID_elements = new List<ElementId>();
				List<ElementId> clashID_familyinstance = new List<ElementId>();

				// get elements with "clash" parameter value == "YES"
				IList<Element> clash = clash_;
				IList<Element> clash_familyinstance = clash_familyinstance_;
				// get elements with "clash" parameter value == "NO"
				IList<Element> clash_no = new List<Element>();

				foreach (Element elem in clash)
				{
					clashID_elements.Add(elem.Id);
				}

				foreach (Element elem in clash_familyinstance)
				{
					clashID_familyinstance.Add(elem.Id);
				}

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				XYZ p1 = new XYZ();
				XYZ p2 = new XYZ();

				foreach (ElementId eid in clashID_elements)
				{
					Element e = doc.GetElement(eid);

					Options op = new Options();
					//            		op.View = doc.ActiveView;
					//            		op.ComputeReferences = true;
					GeometryElement gm = e.get_Geometry(op);
					Solid so = gm.First() as Solid;
					XYZ p = so.ComputeCentroid();

					XYZ xyz = new XYZ(p.X, p.Y, 0);

					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Centroid Point : (" + p.X.ToString() + ", " + p.Y.ToString() + ", 0)  :  " + "Clash Grid Location : " + param_value + "  Category:" + e.Category.Name.ToString() + "  ID : " + e.Id.ToString() + Environment.NewLine;
				}

				foreach (ElementId eid in clashID_familyinstance)
				{
					Element e = doc.GetElement(eid);
					LocationPoint p = e.Location as LocationPoint;

					XYZ xyz = new XYZ(p.Point.X, p.Point.Y, 0);
					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Clash Grid Location : " + param_value + "      Category:" + e.Category.Name.ToString() + "      ID : " + e.Id.ToString() + Environment.NewLine;
				}

				//TaskDialog.Show("Dynoscript", msg);

			}

			void DYNO_SetNoValueClashParameter() // Vista Activa
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}

				foreach (Element e in ductsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					//Parameter param2 = e.LookupParameter("Clash Solved");
					//Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					Parameter param5 = e.LookupParameter("Clash Category");

					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						//param2.Set(1);
						//param3.Set(param_value);
						param4.Set(param_value);
						param5.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					//Parameter param2 = e.LookupParameter("Clash Comments");
					Parameter param3 = e.LookupParameter("Clash Grid Location");
					Parameter param5 = e.LookupParameter("Clash Category");


					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						//param2.Set(param_value);
						param3.Set(param_value);
						param5.Set(param_value);
						t.Commit();
					}
				}

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductfittingsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductfittingsclash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					// category ducts fittings
					ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Ducts
					ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector DUFcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();

					foreach (Element elem in ductfittings)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							ductfittingsclash.Add(elem);
						}
						else
						{
							ductfittingsclash_no.Add(elem);
						}
					}

				}

				foreach (Element e in ductfittingsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					//Parameter param2 = e.LookupParameter("Clash Solved");
					//Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					Parameter param5 = e.LookupParameter("Clash Category");

					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						//param2.Set(1);
						//param3.Set(param_value);
						param4.Set(param_value);
						param5.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductfittingsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					//Parameter param2 = e.LookupParameter("Clash Comments");
					Parameter param3 = e.LookupParameter("Clash Grid Location");
					Parameter param5 = e.LookupParameter("Clash Category");

					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						//param2.Set(param_value);
						param3.Set(param_value);
						param5.Set(param_value);
						t.Commit();
					}
				}
			} // Vista Activa , "Clash" y "Clash Grid Location" = " " vacio.

			void DYNO_SetNoValueAllParameters()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();




				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}

				foreach (Element e in ductsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductfittingsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductfittingsclash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					// category ducts fittings
					ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Ducts
					ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector DUFcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();

					foreach (Element elem in ductfittings)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							ductfittingsclash.Add(elem);
						}
						else
						{
							ductfittingsclash_no.Add(elem);
						}
					}

				}

				foreach (Element e in ductfittingsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductfittingsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}
			} // Vista Activa , "Clash", "Clash Grid Location", "Clash Comments" y "Clash Solved" = " " vacio.

			void DYNO_SetNoValueAllParameters_doc()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();




				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}

				foreach (Element e in ductsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
					    //BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductfittingsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductfittingsclash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					// category ducts fittings
					ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Ducts
					ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector DUFcoll = new FilteredElementCollector(doc);
					IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();

					foreach (Element elem in ductfittings)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							ductfittingsclash.Add(elem);
						}
						else
						{
							ductfittingsclash_no.Add(elem);
						}
					}

				}

				foreach (Element e in ductfittingsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductfittingsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(0);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}
			} // Todo documento , "Clash", "Clash Grid Location", "Clash Comments" y "Clash Solved" = " " vacio.

			void DYNO_CreateClashSchedules_sololasquefaltan_nofunciona()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				//List<Category> categories = ObtenerCategorias(doc);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUFcoll = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));
				List<ViewSchedule> viewSchedules = new List<ViewSchedule>(); // lista vacia
				foreach (ViewSchedule ve in DUFcoll)
				{
					viewSchedules.Add(ve); // lista con todos los ViewSchedule del proyecto
				}

				List<ViewSchedule> viewSchedules_clash = new List<ViewSchedule>(); // lista vacia



				List<ViewSchedule> viewSchedules_yaexiste = new List<ViewSchedule>(); // lista vacia
				List<ViewSchedule> viewSchedules_noexisten = new List<ViewSchedule>(); // lista vacia

				List<ViewSchedule> viewSchedules_listasParaCrear = new List<ViewSchedule>(); // lista vacia

				List<BuiltInCategory> builtInCategory_yaexisten = new List<BuiltInCategory>(); // lista vacia
				List<BuiltInCategory> builtInCategory_noexisten = new List<BuiltInCategory>(); // lista vacia



				BuiltInCategory[] bics = new BuiltInCategory[]
				{
				BuiltInCategory.OST_CableTray,
				BuiltInCategory.OST_CableTrayFitting,
				BuiltInCategory.OST_Conduit,
				BuiltInCategory.OST_ConduitFitting,
				BuiltInCategory.OST_DuctCurves,
				BuiltInCategory.OST_DuctFitting,
				BuiltInCategory.OST_DuctTerminal,
				//BuiltInCategory.OST_ElectricalEquipment,
				BuiltInCategory.OST_ElectricalFixtures,
				BuiltInCategory.OST_LightingDevices,
				BuiltInCategory.OST_LightingFixtures,
				BuiltInCategory.OST_MechanicalEquipment,
				BuiltInCategory.OST_PipeCurves,
				BuiltInCategory.OST_PipeFitting,
				BuiltInCategory.OST_PlumbingFixtures,
				//BuiltInCategory.OST_SpecialityEquipment,
				BuiltInCategory.OST_Sprinklers
					//BuiltInCategory.OST_Wire,
				};

				string[] listasstrings = new string[]   // lista de categorias strings
				{
				"CLASH OST_CableTray SCHEDULE",
				"CLASH OST_CableTrayFitting SCHEDULE",
				"CLASH OST_Conduit SCHEDULE",
				"CLASH OST_ConduitFitting SCHEDULE",
				"CLASH OST_DuctCurves SCHEDULE",
				"CLASH OST_DuctFitting SCHEDULE",
				"CLASH OST_DuctTerminal SCHEDULE",
				"CLASH OST_ElectricalFixtures SCHEDULE",
				"CLASH OST_LightingDevices SCHEDULE",
				"CLASH OST_LightingFixtures SCHEDULE",
				"CLASH OST_MechanicalEquipment SCHEDULE",
				"CLASH OST_PipeCurves SCHEDULE",
				"CLASH OST_PipeFitting SCHEDULE",
				"CLASH OST_PlumbingFixtures SCHEDULE",
				"CLASH OST_Sprinklers SCHEDULE"
				};


				foreach (ViewSchedule ve in viewSchedules) // para todas las clash Schedules del proyecto 
				{
					if (ve.Name.ToString() == "CLASH OST_CableTray SCHEDULE" ||            // si alguna Schedule "ve" es TRUE igual a : YA EXISTE
						ve.Name.ToString() == "CLASH OST_CableTrayFitting SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_Conduit SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_ConduitFitting SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_DuctCurves SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_DuctFitting SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_DuctTerminal SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_ElectricalFixtures SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_LightingDevices SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_LightingFixtures SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_MechanicalEquipment SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_PipeCurves SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_PipeFitting SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_PlumbingFixtures SCHEDULE" ||
						ve.Name.ToString() == "CLASH OST_Sprinklers SCHEDULE")
					{
						viewSchedules_yaexiste.Add(ve);
						//TaskDialog.Show("Mensaje Final", " Las CLASH Schedules ya existen! :)");
					}
				}
				foreach (BuiltInCategory bic in bics)
				{
					foreach (string st in listasstrings)
					{
						foreach (ViewSchedule ve in viewSchedules_yaexiste) // todas las vistasSchedules que ya existen
						{
							if (ve.Name.ToString() == st)           // si alguna Schedule "ve" es TRUE igual a : YA EXISTE
							{
								if (!builtInCategory_yaexisten.Contains(bic))
								{
									builtInCategory_yaexisten.Add(bic);

								}

								//TaskDialog.Show("Mensaje Final", " Las CLASH Schedules ya existen! :)");
							}
						}

					}
				}
				int numero = builtInCategory_yaexisten.Count();

				for (int i = 0; i < bics.Count(); i++)
				{
					foreach (BuiltInCategory bt in builtInCategory_yaexisten) //CableTray , DuctFitting
					{
						if (bics[i] == bt)
						{
							continue; // no hacer nada
						}
						else // si no esta agregarlo
						{
							if (!builtInCategory_noexisten.Contains(bics[i]))
							{
								builtInCategory_noexisten.Add(bics[i]);

							}

						}

					}
				}
				TaskDialog.Show("MIERDA", "builtInCategory_noexisten : " + builtInCategory_noexisten.Count().ToString());
				// encontrar items sobrantes de "bics" al filtrarlo usando "builtInCategory_yaexisten"
				if (builtInCategory_noexisten.Count() == 0)
				{
					TaskDialog.Show("Dynoscript", "Ya existen todos los CLASHES Schedules! :) ");
				}
				else
				{
					// hallar builtInCategory de las schedules que no existen. 

					string msg = "";
					string msg3 = "";

					foreach (BuiltInCategory bic in builtInCategory_noexisten) // builtInCategory_noexisten esta encontrando 15 veces el mismo BUiltInCategory No funciona!
					{
						ViewSchedule clashSchedule = null;
						using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
						{
							transaction.Start();

							//FilteredElementCollector collector = new FilteredElementCollector(doc);
							//collector.OfCategory(bic);
							//Get first ElementId of AreaScheme.
							//ElementId elemId = collector.FirstElementId();
							//if (elemId != null && elemId != ElementId.InvalidElementId)
							//{
							// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
							// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
							// and the value of third argument must be ElementId of an AreaScheme.
							clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(bic));

							doc.Regenerate();

							//Add fields

							//}
							ScheduleDefinition definition = clashSchedule.Definition;

							IList<SchedulableField> schedulableFields = definition.GetSchedulableFields(); // [a,b,c,s,d,f,....]


							//List<SchedulableField> listashparam = [];
							//SchedulableField[] listashparam = {};
							List<SchedulableField> listashparam = new List<SchedulableField>();
							//List<ScheduleFieldId> clashId = [];
							//ScheduleFieldId[] clashId = {};
							List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();

							foreach (SchedulableField element in schedulableFields)
							{
								if (element.ParameterId.IntegerValue > 0)
								{
									listashparam.Add(element);
								}
							}


							double nro_items_listahpram = listashparam.Count();

							for (int i = 0; i < 1; i++)
							{
								if (listashparam[i].GetName(doc).ToString() == "Clash")
								{
									clashSchedule.Definition.AddField(listashparam[i]);
									ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId);
								}
								if (listashparam[i + 1].GetName(doc).ToString() == "Clash Category")
								{
									clashSchedule.Definition.AddField(listashparam[i + 1]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId2 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId2);
								}
								if (listashparam[i + 2].GetName(doc).ToString() == "Clash Comments")
								{
									clashSchedule.Definition.AddField(listashparam[i + 2]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId3);
								}
								if (listashparam[i + 3].GetName(doc).ToString() == "Clash Grid Location")
								{
									clashSchedule.Definition.AddField(listashparam[i + 3]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId3);
								}
								if (listashparam[i + 4].GetName(doc).ToString() == "Clash Solved")
								{
									clashSchedule.Definition.AddField(listashparam[i + 4]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId4 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId4);
								}
								if (listashparam[i + 5].GetName(doc).ToString() == "Done")
								{
									clashSchedule.Definition.AddField(listashparam[i + 5]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId5 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId5);
								}
								if (listashparam[i + 6].GetName(doc).ToString() == "ID Element")
								{
									clashSchedule.Definition.AddField(listashparam[i + 6]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId6 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId6);
								}
								if (listashparam[i + 7].GetName(doc).ToString() == "Percent Done")
								{
									clashSchedule.Definition.AddField(listashparam[i + 7]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId7 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId7);
								}
								if (listashparam[i + 8].GetName(doc).ToString() == "Zone")
								{
									clashSchedule.Definition.AddField(listashparam[i + 8]);
									//ScheduleField field = clashSchedule.Definition.GetField(0);
									ScheduleFieldId fielId8 = clashSchedule.Definition.GetFieldId(0);
									clashId.Add(fielId8);
								}
								msg3 = listashparam[i].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 1].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 2].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 3].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 4].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 5].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 6].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 7].GetName(doc).ToString() + Environment.NewLine
									+ listashparam[i + 8].GetName(doc).ToString() + Environment.NewLine;
							}


							ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());

							//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
							//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));

							if (null != clashSchedule)
							{
								transaction.Commit();
							}
							else
							{
								transaction.RollBack();
							}

							using (Transaction t = new Transaction(doc, "Add filter"))
							{
								t.Start();
								//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
								ScheduleFilter filter = new ScheduleFilter(foundField.FieldId, ScheduleFilterType.Equal, "YES");
								clashSchedule.Definition.AddFilter(filter);
								t.Commit();
							}
							using (Transaction tran = new Transaction(doc, "Cambiar nombre"))
							{
								tran.Start();
								TableData td = clashSchedule.GetTableData(); // get viewschedule table data
								TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
								string text = tsd.GetCellText(0, 0);
								tsd.SetCellText(0, 0, "CLASH " + bic.ToString() + " SCHEDULE");
								//
								// insert columns
								clashSchedule.Name = "CLASH " + bic.ToString() + " SCHEDULE";
								tsd.InsertColumn(0);
								tran.Commit();
							}
						}
					}
					TaskDialog.Show("Creating CLASH Schedule", msg + "Se agregaron las siguientes columnas a las tablas: \n\n" + msg3 + Environment.NewLine);
					// bic in bics
				}






			}// no funciona

			void DYNO_CreateClashSchedules()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				//List<Category> categories = ObtenerCategorias(doc);

				BuiltInCategory[] bics = new BuiltInCategory[]
				{
				BuiltInCategory.OST_CableTray,
				BuiltInCategory.OST_CableTrayFitting,
				BuiltInCategory.OST_Conduit,
				BuiltInCategory.OST_ConduitFitting,
				BuiltInCategory.OST_DuctCurves,
				BuiltInCategory.OST_DuctFitting,
				BuiltInCategory.OST_DuctTerminal,
				//BuiltInCategory.OST_ElectricalEquipment,
				BuiltInCategory.OST_ElectricalFixtures,
				BuiltInCategory.OST_LightingDevices,
				BuiltInCategory.OST_LightingFixtures,
				BuiltInCategory.OST_MechanicalEquipment,
				BuiltInCategory.OST_PipeCurves,
				BuiltInCategory.OST_PipeFitting,
				BuiltInCategory.OST_PlumbingFixtures,
				//BuiltInCategory.OST_SpecialityEquipment,
				BuiltInCategory.OST_Sprinklers
					//BuiltInCategory.OST_Wire,
				};

				string msg = "";
				string msg3 = "";

				foreach (BuiltInCategory bic in bics)
				{
					ViewSchedule clashSchedule = null;
					using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
					{
						transaction.Start();

						//FilteredElementCollector collector = new FilteredElementCollector(doc);
						//collector.OfCategory(bic);
						//Get first ElementId of AreaScheme.
						//ElementId elemId = collector.FirstElementId();
						//if (elemId != null && elemId != ElementId.InvalidElementId)
						//{
						// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
						// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
						// and the value of third argument must be ElementId of an AreaScheme.
						clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(bic));

						doc.Regenerate();

						//Add fields

						//}
						ScheduleDefinition definition = clashSchedule.Definition;

						IList<SchedulableField> schedulableFields = definition.GetSchedulableFields(); // [a,b,c,s,d,f,....]


						//List<SchedulableField> listashparam = [];
						//SchedulableField[] listashparam = {};
						List<SchedulableField> listashparam = new List<SchedulableField>();
						//List<ScheduleFieldId> clashId = [];
						//ScheduleFieldId[] clashId = {};
						List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();

						foreach (SchedulableField element in schedulableFields)
						{
							if (element.ParameterId.IntegerValue > 0)
							{
								listashparam.Add(element);
							}
						}


						double nro_items_listahpram = listashparam.Count();

						for (int i = 0; i < 1; i++)
						{
							if (listashparam[i].GetName(doc).ToString() == "Clash")
							{
								clashSchedule.Definition.AddField(listashparam[i]);
								ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId);
							}
							if (listashparam[i + 1].GetName(doc).ToString() == "Clash Category")
							{
								clashSchedule.Definition.AddField(listashparam[i + 1]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId2 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId2);
							}
							if (listashparam[i + 2].GetName(doc).ToString() == "Clash Comments")
							{
								clashSchedule.Definition.AddField(listashparam[i + 2]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId3);
							}
							if (listashparam[i + 3].GetName(doc).ToString() == "Clash Grid Location")
							{
								clashSchedule.Definition.AddField(listashparam[i + 3]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId3);
							}
							if (listashparam[i + 4].GetName(doc).ToString() == "Clash Solved")
							{
								clashSchedule.Definition.AddField(listashparam[i + 4]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId4 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId4);
							}
							if (listashparam[i + 5].GetName(doc).ToString() == "Done")
							{
								clashSchedule.Definition.AddField(listashparam[i + 5]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId5 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId5);
							}
							if (listashparam[i + 6].GetName(doc).ToString() == "ID Element")
							{
								clashSchedule.Definition.AddField(listashparam[i + 6]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId6 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId6);
							}
							if (listashparam[i + 7].GetName(doc).ToString() == "Percent Done")
							{
								clashSchedule.Definition.AddField(listashparam[i + 7]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId7 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId7);
							}
							if (listashparam[i + 8].GetName(doc).ToString() == "Zone")
							{
								clashSchedule.Definition.AddField(listashparam[i + 8]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId8 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId8);
							}
							msg3 = listashparam[i].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 1].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 2].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 3].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 4].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 5].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 6].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 7].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 8].GetName(doc).ToString() + Environment.NewLine;
						}


						ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());

						//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
						//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));

						if (null != clashSchedule)
						{
							transaction.Commit();
						}
						else
						{
							transaction.RollBack();
						}

						using (Transaction t = new Transaction(doc, "Add filter"))
						{
							t.Start();
							//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
							ScheduleFilter filter = new ScheduleFilter(foundField.FieldId, ScheduleFilterType.Equal, "YES");
							clashSchedule.Definition.AddFilter(filter);
							t.Commit();
						}
						using (Transaction tran = new Transaction(doc, "Cambiar nombre"))
						{
							tran.Start();
							TableData td = clashSchedule.GetTableData(); // get viewschedule table data
							TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
							string text = tsd.GetCellText(0, 0);
							tsd.SetCellText(0, 0, "CLASH " + bic.ToString() + " SCHEDULE");
							//
							// insert columns
							clashSchedule.Name = "CLASH " + bic.ToString() + " SCHEDULE";
							tsd.InsertColumn(0);
							tran.Commit();
						}
					}
				}
				TaskDialog.Show("Creation CLASH Parameters", msg + "Se crearon los siguientes Clash Parameters: \n\n" + msg3 + Environment.NewLine);
				// bic in bics

			}

			void DYNO_SectionBox()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;
				View view = doc.ActiveView;
				string view_name = view.Name.ToString(); // nombre de la vista activa 
				List<bool> listabool_3 = new List<bool>();
				List<bool> listabool_2 = new List<bool>();
				List<bool> listabool_1 = new List<bool>();
				string ee = "";
				using (var form = new Form3())
				{
					form.ShowDialog();

					if (form.DialogResult == forms.DialogResult.Cancel)
					{
						return;
					}

					if (form.DialogResult == forms.DialogResult.OK)
					{
						bool ee3 = form.checkBox_3; // TRUE aplicar
						listabool_3.Add(ee3);
						bool ee2 = form.checkBox_2; // TRUE aplicar
						listabool_2.Add(ee2);
						bool ee1 = form.checkBox_1; // TRUE aplicar
						listabool_1.Add(ee1);
					}

					ee = listabool_3.First().ToString() + Environment.NewLine
						+ listabool_2.First().ToString() + Environment.NewLine
						+ listabool_1.First().ToString() + Environment.NewLine;

					//TaskDialog.Show("primero", ee);
					if (listabool_3.First() && !(listabool_2.First()) && !(listabool_2.First()))
					{
						using (Transaction t = new Transaction(doc, "Create clash 3d view"))
						{
							t.Start();
							double Min_Z = double.MaxValue;

							// encontrar Min_Y , Min_X , Max_X , Max_Y
							double Min_X = double.MaxValue;
							double Min_Y = double.MaxValue;

							double Max_X = double.MinValue;
							double Max_Y = double.MinValue;
							double Max_Z = double.MinValue;

							List<ElementId> ids = new List<ElementId>();

							IList<Reference> references = uidoc.Selection.PickObjects(ObjectType.Element);

							foreach (Reference reference in references)
							{
								Element e = doc.GetElement(reference);
								ElementId eId = e.Id;
								ids.Add(eId);
							}


							foreach (ElementId id in ids)
							{
								Element elm = doc.GetElement(id);
								BoundingBoxXYZ box = elm.get_BoundingBox(view);
								if (box.Max.X > Max_X)
								{
									Max_X = box.Max.X;
								}
								if (box.Max.Y > Max_Y)
								{
									Max_Y = box.Max.Y;
								}
								if (box.Max.Z > Max_Z)
								{
									Max_Z = box.Max.Z;
								}

								if (box.Min.X < Min_X)
								{
									Min_X = box.Min.X;
								}
								if (box.Min.Y < Min_Y)
								{
									Min_Y = box.Min.Y;
								}
								if (box.Min.Z < Min_Z)
								{
									Min_Z = box.Min.Z;
								}
							}

							XYZ Max = new XYZ(Max_X, Max_Y, Max_Z);
							XYZ Min = new XYZ(Min_X, Min_Y, Min_Z);

							BoundingBoxXYZ myBox = new BoundingBoxXYZ();

							myBox.Min = Min;
							myBox.Max = Max;

							//ElementId dupleView_id = doc.ActiveView.Duplicate(ViewDuplicateOption.WithDetailing);

							// get a ViewFamilyType for a 3D View
							ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
														 OfClass(typeof(ViewFamilyType)).
														 Cast<ViewFamilyType>()
															 where v.ViewFamily == ViewFamily.ThreeDimensional
															 select v).First();
							// Create the 3d view
							//View3D dupleView = doc.GetElement(dupleView_id) as View3D;
							View3D dupleView = View3D.CreateIsometric(doc, viewFamilyType.Id);

							FilteredElementCollector DUFcoll = new FilteredElementCollector(doc).OfClass(typeof(View3D));
							List<View3D> views = new List<View3D>(); // lista vacia
							List<View3D> views_COORD = new List<View3D>(); // lista vacia
							int numero = 1;
							foreach (View3D ve in DUFcoll)
							{
								views.Add(ve); // lista con todos los ViewSchedule del proyecto
							}
							for (int i = 0; i < views.Count(); i++)
							{
								View3D ve = views[i];
								if (ve.Name.Contains(view_name + "  Copy "))
								{
									views_COORD.Add(ve); // todas la vistas con nombre igual + "copy"

									//numero = numero + 1;
								}

							}
							for (int i = 0; i < views_COORD.Count(); i++)
							{
								View3D ve = views_COORD[i];
								if (ve.Name.Contains(view_name + "  Copy "))
								{
									numero = numero + 1;
								}
								else
								{
									numero = 1; // solo COORD
								}
							}

							dupleView.Name = view_name + "  Copy " + (numero).ToString();



							(dupleView as View3D).SetSectionBox(myBox);

							//						if (!dupleView.IsTemplate) 
							//						{
							dupleView.DisplayStyle = DisplayStyle.Shading;
							dupleView.DetailLevel = ViewDetailLevel.Fine;
							//						}


							List<Element> riv = new List<Element>();
							FilteredElementCollector links = new FilteredElementCollector(doc, dupleView.Id);
							ElementCategoryFilter linkFilter = new ElementCategoryFilter(BuiltInCategory.OST_RvtLinks);
							links.WhereElementIsNotElementType();
							links.WherePasses(linkFilter);
							riv.AddRange(links.ToElements());
							//                		foreach (Element link in riv) 
							//                		{
							//                			
							//                		}

							t.Commit();

							uidoc.ActiveView = dupleView;
							DYNO_CreateClashFilterMultipleElementsInView_UI(dupleView);
							DYNO_CreateClashSOLVEDFilterMultipleElementsInView_UI(dupleView);

						}
						DYNO_CreateClashSOLVEDFilterMultipleElementsInView(); // Crea y coloca el filtro CLASH SOLVED a la vista FILTER VIEW
					}
					else if (listabool_1.First() && !(listabool_2.First()) && !(listabool_3.First()))
					{
						DYNO_create3DClashSectionBoxView_ELEMENT();

					}
					else if (listabool_2.First() && !(listabool_1.First()) && !(listabool_3.First()))
					{
						DYNO_create3DClashSectionBoxView_LEVELS();
					}
					else
					{
						TaskDialog.Show("FINAL", "Selecciona SOLAMENTE 1 CHECK a la vez para que funcione correctamente por favor! :)");
					}

				}



			}

			void DYNO_create3DClashSectionBoxView_ELEMENT()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				List<Element> clash_elements = new List<Element>();

				foreach (Element e in DYNO_GetAllClashElements_OnlyActiveView())
				{
					clash_elements.Add(e);
				}
				// get list of all levels
				//IList<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(l => l.Elevation).ToList();
				if (clash_elements.Count() == 0)
				{
					TaskDialog.Show("Dynoscript", "No se encontraron Elementos con Clash en la Vista Activa!");
					return;
				}
				else
				{
					// get a ViewFamilyType for a 3D View
					ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
													 OfClass(typeof(ViewFamilyType)).
													 Cast<ViewFamilyType>()
													 where v.ViewFamily == ViewFamily.ThreeDimensional
													 select v).First();

					List<View3D> tresDclashview = new List<View3D>();

					using (Transaction t = new Transaction(doc, "Create clash 3d view"))
					{
						int ctr = 0;
						// loop through all Elements
						foreach (Element elem in clash_elements)
						{
							t.Start();

							// Create the 3d view
							View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);

							Parameter param = elem.LookupParameter("Clash Category");

							string param_string = param.AsString();
							string param_string2 = param_string.Replace(':', '_');

							// Set the name of the view
							clashview.Name = "COORD - Section Box  " + elem.Name.ToString() + " / "
																	+ "ID  " + elem.Id.ToString() + " / "
																	+ " Clash Category " + param_string2;




							// Set the name of the transaction
							// A transaction can be renamed after it has been started
							t.SetName("Create view " + clashview.Name);

							// Create a new BoundingBoxXYZ to define a 3D rectangular space
							BoundingBoxXYZ elem_bb = elem.get_BoundingBox(null);

							clashview.SetSectionBox(elem_bb);

							//					if (!clashview.IsTemplate) 
							//					{
							clashview.DisplayStyle = DisplayStyle.Shading;
							clashview.DetailLevel = ViewDetailLevel.Fine;
							//					}

							t.Commit();

							// Open the just-created view
							// There cannot be an open transaction when the active view is set
							tresDclashview.Add(clashview);

							ctr++;
						}
						uidoc.ActiveView = tresDclashview.First();
						DYNO_CreateClashFilterMultipleElementsInMultipleViews_UI(tresDclashview);
						DYNO_CreateClashSOLVEDFilterMultipleElementsInMultipleViews_UI(tresDclashview);
					}
				}


			} // ONLY THE ACTIVE VIEW

			void DYNO_create3DClashSectionBoxView_LEVELS()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				// get list of all levels
				IList<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(l => l.Elevation).ToList();

				// get a ViewFamilyType for a 3D View
				ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
												 OfClass(typeof(ViewFamilyType)).
												 Cast<ViewFamilyType>()
												 where v.ViewFamily == ViewFamily.ThreeDimensional
												 select v).First();

				List<View3D> lista3dview = new List<View3D>();

				using (Transaction t = new Transaction(doc, "Create view"))
				{
					int ctr = 0;
					// loop through all levels
					foreach (Level level in levels)
					{
						t.Start();

						// Create the 3d view
						View3D view = View3D.CreateIsometric(doc, viewFamilyType.Id);

						// Set the name of the view
						view.Name = "COORD - Nivel " + level.Name;

						// Set the name of the transaction
						// A transaction can be renamed after it has been started
						t.SetName("Create view " + view.Name);

						// Create a new BoundingBoxXYZ to define a 3D rectangular space
						BoundingBoxXYZ boundingBoxXYZ = new BoundingBoxXYZ();

						// Set the lower left bottom corner of the box
						// Use the Z of the current level.
						// X & Y values have been hardcoded based on this RVT geometry
						boundingBoxXYZ.Min = new XYZ(-50, -100, level.Elevation);

						// Determine the height of the bounding box
						double zOffset = 0;
						// If there is another level above this one, use the elevation of that level
						if (levels.Count > ctr + 1)
							zOffset = levels.ElementAt(ctr + 1).Elevation;
						// If this is the top level, use an offset of 10 feet
						else
							zOffset = level.Elevation + 10;
						boundingBoxXYZ.Max = new XYZ(200, 125, zOffset);

						// Apply this bouding box to the view's section box
						(view as View3D).SetSectionBox(boundingBoxXYZ);
						lista3dview.Add(view);

						if (!view.IsTemplate)
						{
							view.DisplayStyle = DisplayStyle.Shading;
							view.DetailLevel = ViewDetailLevel.Fine;
						}

						t.Commit();

						// Open the just-created view
						// There cannot be an open transaction when the active view is set
						uidoc.ActiveView = view;

						ctr++;
					}
				}
				foreach (View3D view in lista3dview)
				{
					DYNO_CreateClashFilterMultipleElementsInView_UI(view);
					DYNO_CreateClashSOLVEDFilterMultipleElementsInView_UI(view);
				}


			}

			void DYNO_create3DClashSectionBoxView_ZONE_nofunciona()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<View3D> tresDclashview = new List<View3D>();

				List<Element> clash_elements = new List<Element>();

				foreach (Element e in DYNO_GetAllClashElements_OnlyActiveView())
				{
					clash_elements.Add(e);
				}


				List<Element> clash_elements_zone = new List<Element>();
				List<Element> clash_elements_zone_no = new List<Element>();

				List<ElementId> clash_elements_zone_ids = new List<ElementId>();



				foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
				{
					string intersc = kp.Value.ToString(); // 

					string intersec_key = kp.Key.ToString(); // B / 1 - 0
															 //TaskDialog.Show("primero", intersec_key);


					foreach (Element elem in clash_elements)
					{
						if (elem.LookupParameter("Clash Grid Location").AsString() == intersec_key)
						{
							clash_elements_zone.Add(elem);
						}
						else
						{
							clash_elements_zone_no.Add(elem);
						}
					}

					if (clash_elements_zone.Count() > 0)
					{

						// get a ViewFamilyType for a 3D View
						ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
														 OfClass(typeof(ViewFamilyType)).
														 Cast<ViewFamilyType>()
														 where v.ViewFamily == ViewFamily.ThreeDimensional
														 select v).First();

						using (Transaction t = new Transaction(doc, "Create clash 3d view"))
						{

							t.Start();
							// Create the 3d view
							View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);
							foreach (Element elem in clash_elements_zone)
							{
								clash_elements_zone_ids.Add(elem.Id);
							}

							List<double> pointsZmax = new List<double>();
							List<double> pointsZmin = new List<double>();

							List<double> pointsXmax = new List<double>();
							List<double> pointsXmax_sorted = new List<double>();
							List<double> pointsXmin = new List<double>();
							List<double> pointsXmin_sorted = new List<double>();

							List<double> pointsYmax = new List<double>();
							List<double> pointsYmax_sorted = new List<double>();
							List<double> pointsYmin = new List<double>();
							List<double> pointsYmin_sorted = new List<double>();

							// Example dictionary.
							var dictionary_ZX_max = new Dictionary<double, double>();
							var dictionary_ZY_max = new Dictionary<double, double>();
							var dictionary_ZX_min = new Dictionary<double, double>();
							var dictionary_ZY_min = new Dictionary<double, double>();

							foreach (ElementId id in clash_elements_zone_ids)
							{


								Element elm = doc.GetElement(id);
								BoundingBoxXYZ box = elm.get_BoundingBox(clashview);

								double maxZ = box.Max.Z;

								double maxX = box.Max.X;
								double maxY = box.Max.Y;

								double minZ = box.Min.Z;

								double minX = box.Min.X;
								double minY = box.Min.Y;

								pointsZmax.Add(maxZ); // keys
								pointsXmax.Add(maxX); // values
								pointsYmax.Add(maxY); // values

								pointsZmin.Add(minZ); // keys
								pointsXmin.Add(minX); // values
								pointsYmin.Add(minY); // values

								//					    dictionary_ZX_max.Add(maxZ, maxX);
								//					    dictionary_ZY_max.Add(maxZ, maxY);
								//					    
								//					    dictionary_ZX_min.Add(minZ, minX);
								//					    dictionary_ZY_min.Add(minZ, minY);
							}


							for (int i = 0; i < pointsZmax.Count; i++)
							{
								dictionary_ZX_max.Add(pointsZmax[i], pointsXmax[i]);
							}
							//Separately, sort our keys into alphabetical order
							//You need your keys in their own list. You may need to create this yourself if you don't already have it.
							pointsZmax.Sort();
							pointsZmax.Reverse();

							for (int i = 0; i < pointsZmax.Count; i++)
							{
								pointsXmax_sorted.Add(dictionary_ZX_max[pointsZmax[i]]);
							}
							// X MAX





							for (int i = 0; i < pointsZmax.Count; i++)
							{
								dictionary_ZY_max.Add(pointsZmax[i], pointsYmax[i]);
							}

							//Separately, sort our keys into alphabetical order
							//You need your keys in their own list. You may need to create this yourself if you don't already have it.
							pointsZmax.Sort();
							pointsZmax.Reverse();

							for (int i = 0; i < pointsZmax.Count; i++)
							{
								pointsYmax_sorted.Add(dictionary_ZY_max[pointsZmax[i]]);
							}
							// Y MAX






							for (int i = 0; i < pointsZmin.Count; i++)
							{
								dictionary_ZX_min.Add(pointsZmin[i], pointsXmin[i]);
							}
							//Separately, sort our keys into alphabetical order
							//You need your keys in their own list. You may need to create this yourself if you don't already have it.
							pointsZmin.Sort();
							//    				pointsZmin.Reverse();

							for (int i = 0; i < pointsZmin.Count; i++)
							{
								pointsXmin_sorted.Add(dictionary_ZX_min[pointsZmin[i]]);
							}
							// X MIN



							for (int i = 0; i < pointsZmin.Count; i++)
							{
								dictionary_ZY_min.Add(pointsZmin[i], pointsYmin[i]);
							}
							//Separately, sort our keys into alphabetical order
							//You need your keys in their own list. You may need to create this yourself if you don't already have it.
							pointsZmin.Sort();
							//    				pointsZmin.Reverse();

							for (int i = 0; i < pointsZmin.Count; i++)
							{
								pointsYmin_sorted.Add(dictionary_ZY_min[pointsZmin[i]]);
							}
							// Y MIN




							double Max_Z = pointsZmax.First();

							double Max_X = pointsXmax_sorted.First();
							double Max_Y = pointsYmax_sorted.First();

							double Min_Z = pointsZmin.First();

							double Min_X = pointsXmin_sorted.First();
							double Min_Y = pointsYmin_sorted.First();



							XYZ Max = new XYZ(Max_X, Max_Y, Max_Z);
							XYZ Min = new XYZ(Min_X, Min_Y, Min_Z);

							BoundingBoxXYZ myBox = new BoundingBoxXYZ();

							myBox.Min = Min;
							myBox.Max = Max;



							//BoundingBoxXYZ zone_bb = null;

							string number = (tresDclashview.Count() + 1).ToString();
							// Set the name of the view

							clashview.Name = "Zone    " + intersec_key + "     " + " Section Box    " + number;

							// Set the name of the transaction
							// A transaction can be renamed after it has been started
							t.SetName("Create view " + clashview.Name);

							clashview.SetSectionBox(myBox);
							// There cannot be an open transaction when the active view is set
							tresDclashview.Add(clashview);
							t.Commit();

						}
					}

				}
				uidoc.ActiveView = tresDclashview.First();

			}

			void DYNO_create3DClashSectionBoxView_ZONE_2()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<View3D> tresDclashview = new List<View3D>();

				List<Element> clash_elements = new List<Element>();

				foreach (Element e in DYNO_GetAllClashElements_OnlyActiveView())
				{
					clash_elements.Add(e);
				}


				//			List<Element> clash_elements_zone = new List<Element>();
				//		    List<Element> clash_elements_zone_no = new List<Element>();

				//		    List<ElementId> clash_elements_zone_ids = new List<ElementId>();

				string msg = "";
				try
				{
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						List<Element> clash_elements_zone = new List<Element>();
						List<Element> clash_elements_zone_no = new List<Element>();
						List<ElementId> clash_elements_zone_ids = new List<ElementId>();

						string intersc = kp.Value.ToString(); // 

						string intersec_key = kp.Key.ToString(); // B / 1 - 0
																 //TaskDialog.Show("primero", intersec_key);

						foreach (Element elem in clash_elements)
						{
							if (elem.LookupParameter("Clash Grid Location").AsString() == intersec_key)
							{
								clash_elements_zone.Add(elem);
							}
							else
							{
								clash_elements_zone_no.Add(elem);
							}
						}

						if (clash_elements_zone.Count() > 1)
						{

							// get a ViewFamilyType for a 3D View
							ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
															 OfClass(typeof(ViewFamilyType)).
															 Cast<ViewFamilyType>()
															 where v.ViewFamily == ViewFamily.ThreeDimensional
															 select v).First();


							using (Transaction t = new Transaction(doc, "Create clash 3d view"))
							{

								t.Start();
								// Create the 3d view
								View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);

								foreach (Element elem in clash_elements_zone)
								{
									clash_elements_zone_ids.Add(elem.Id);
								}

								//double Min_X = double.MaxValue;
								//double Min_X = 0;
								//double Min_Y = double.MaxValue;
								//double Min_Y = 0;

								double Min_Z = double.MaxValue;

								// encontrar Min_Y , Min_X , Max_X , Max_Y
								double Min_X = double.MaxValue;
								double Min_Y = double.MaxValue;

								double Max_X = double.MinValue;
								double Max_Y = double.MinValue;
								double Max_Z = double.MinValue;

								foreach (ElementId id in clash_elements_zone_ids)
								{
									Element elm = doc.GetElement(id);

									BoundingBoxXYZ box = elm.get_BoundingBox(null);

									if (box.Max.X > Max_X)
									{
										Max_X = box.Max.X;
									}
									if (box.Max.Y > Max_Y)
									{
										Max_Y = box.Max.Y;
									}
									if (box.Max.Z > Max_Z)
									{
										Max_Z = box.Max.Z;
									}



									if (box.Min.X < Min_X)
									{
										Min_X = box.Min.X;
									}
									if (box.Min.Y < Min_Y)
									{
										Min_Y = box.Min.Y;
									}
									if (box.Min.Z < Min_Z)
									{
										Min_Z = box.Min.Z;
									}
								}

								double m = 5;

								XYZ Max = new XYZ(Max_X - m, Max_Y - m, Max_Z - m);
								XYZ Min = new XYZ(Min_X + m, Min_Y + m, Min_Z + m);


								BoundingBoxXYZ myBox = new BoundingBoxXYZ();

								myBox.Min = Min;
								myBox.Max = Max;

								msg = msg + Max.ToString() + Environment.NewLine + Min.ToString() + Environment.NewLine + "-----------------------------------------------------------" + Environment.NewLine;

								//BoundingBoxXYZ zone_bb = null;

								string number = (tresDclashview.Count() + 1).ToString();
								// Set the name of the view

								clashview.Name = number + ".- Zone    " + intersec_key + "     " + " Section Box    ";

								// Set the name of the transaction
								// A transaction can be renamed after it has been started
								t.SetName("Create view " + clashview.Name);

								clashview.SetSectionBox(myBox);
								tresDclashview.Add(clashview);
								t.Commit();
								// There cannot be an open transaction when the active view is set



							}
						}


					}
					TaskDialog.Show("OUT", msg);
					uidoc.ActiveView = tresDclashview.First();
				}
				catch (Exception ex)
				{
					TaskDialog.Show("Error ex", ex.ToString());
					throw;
				}


			}

			void DYNO_create3DClashSectionBoxView_ZONE_2_nuevo()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<View3D> tresDclashview = new List<View3D>();

				List<Element> clash_elements = new List<Element>();

				foreach (Element e in DYNO_GetAllClashElements_OnlyActiveView())
				{
					clash_elements.Add(e);
				}


				//			List<Element> clash_elements_zone = new List<Element>();
				//		    List<Element> clash_elements_zone_no = new List<Element>();

				//		    List<ElementId> clash_elements_zone_ids = new List<ElementId>();



				string msg = "";
				try
				{

					//throw new AccessViolationException();
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						List<Element> clash_elements_zone = new List<Element>();
						List<Element> clash_elements_zone_no = new List<Element>();
						List<ElementId> clash_elements_zone_ids = new List<ElementId>();

						string intersc = kp.Value.ToString(); // 

						string intersec_key = kp.Key.ToString(); // B / 1 - 0
						TaskDialog.Show("primero", intersec_key);

						foreach (Element elem in clash_elements)
						{
							if (elem.LookupParameter("Clash Grid Location").AsString() == intersec_key)
							{
								clash_elements_zone.Add(elem);
							}

							else
							{

								clash_elements_zone_no.Add(elem);
							}
						}
						//						if (clash_elements_zone.Count()==0) 
						//						{
						//							TaskDialog.Show("mensaje", clash_elements_zone.Count().ToString());
						//							continue;
						//						}

						if (clash_elements_zone.Count() > 1)
						{

							// get a ViewFamilyType for a 3D View
							ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
															 OfClass(typeof(ViewFamilyType)).
															 Cast<ViewFamilyType>()
															 where v.ViewFamily == ViewFamily.ThreeDimensional
															 select v).First();


							using (Transaction t = new Transaction(doc, "Create clash 3d view"))
							{

								t.Start();
								// Create the 3d view
								View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);

								foreach (Element elem in clash_elements_zone)
								{
									clash_elements_zone_ids.Add(elem.Id);
								}

								//double Min_X = double.MaxValue;
								//double Min_X = 0;
								//double Min_Y = double.MaxValue;
								//double Min_Y = 0;

								double Min_Z = double.MaxValue;

								// encontrar Min_Y , Min_X , Max_X , Max_Y
								double Min_X = double.MaxValue;
								double Min_Y = double.MaxValue;

								double Max_X = double.MinValue;
								double Max_Y = double.MinValue;
								double Max_Z = double.MinValue;

								foreach (ElementId id in clash_elements_zone_ids)
								{
									Element elm = doc.GetElement(id);

									BoundingBoxXYZ box = elm.get_BoundingBox(clashview);

									if (box.Max.X > Max_X)
									{
										Max_X = box.Max.X;
									}
									if (box.Max.Y > Max_Y)
									{
										Max_Y = box.Max.Y;
									}
									if (box.Max.Z > Max_Z)
									{
										Max_Z = box.Max.Z;
									}



									if (box.Min.X < Min_X)
									{
										Min_X = box.Min.X;
									}
									if (box.Min.Y < Min_Y)
									{
										Min_Y = box.Min.Y;
									}
									if (box.Min.Z < Min_Z)
									{
										Min_Z = box.Min.Z;
									}
								}

								double m = 5;

								XYZ Max = new XYZ(Max_X - m, Max_Y - m, Max_Z - m);
								XYZ Min = new XYZ(Min_X + m, Min_Y + m, Min_Z + m);

								msg = msg + Max.ToString() + Environment.NewLine + Min.ToString() + Environment.NewLine + "-----------------------------------------------------------" + Environment.NewLine;




								BoundingBoxXYZ myBox = new BoundingBoxXYZ();

								myBox.Min = Min;
								myBox.Max = Max;

								//BoundingBoxXYZ zone_bb = null;

								string number = (tresDclashview.Count() + 1).ToString();
								// Set the name of the view

								clashview.Name = number + ".- Zone    " + intersec_key + "     " + " Section Box    ";

								// Set the name of the transaction
								// A transaction can be renamed after it has been started
								t.SetName("Create view " + clashview.Name);

								BoundingBoxXYZ bb = myBox;
								BoundingBoxXYZ bi = new BoundingBoxXYZ();

								clashview.SetSectionBox(bb);
								//							        if (bb==null) 
								//							        {
								//							        	clashview.SetSectionBox(bi);
								//							        }
								//							        else
								//							        {
								//							        	clashview.SetSectionBox(bb);
								//							        }
								tresDclashview.Add(clashview);
								t.Commit();
								// There cannot be an open transaction when the active view is set



							}
						}


					}
					TaskDialog.Show("OUT", msg);
					uidoc.ActiveView = tresDclashview.First();
				}

				catch (Exception ex)
				{
					TaskDialog.Show("Error ex", ex.ToString());
					throw;
				}
				finally
				{
					//		    		Dictionary<string,XYZ> intersectionPoints_existente = new Dictionary<string, XYZ>();
					//					List<string> listaclashgridlocation = new List<string>();
					//					List<string> listaclashgridlocation_unique = new List<string>();    
					//					
					//		    		foreach (Element elem in clash_elements)
					//				    {
					//				    	Parameter p = elem.LookupParameter("Clash Grid Location");
					//				    	string p_clashgridloc = p.ToString(); // A/6
					//				    	listaclashgridlocation.Add(p_clashgridloc);
					//				    }
					//		    		var unique_items = new HashSet<string>(listaclashgridlocation);
					//				    foreach (string s in unique_items)
					//				    {
					//				    	listaclashgridlocation_unique.Add(s);
					//				    }
					//		    		foreach (KeyValuePair<string,XYZ> kp in intersectionPoints)
					//				    {
					//				    	string intersec_key = kp.Key.ToString(); // A/6
					//
					//				    	foreach (string i in listaclashgridlocation_unique) 
					//				    	{
					//
					//					    		if (i == intersec_key)
					//					    		{
					//					    			intersectionPoints_existente.Add(kp.Key,kp.Value);
					//					    		}
					//
					//
					//				    	}
					//				    	
					//				    }



					//throw new AccessViolationException();
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						List<Element> clash_elements_zone = new List<Element>();
						List<Element> clash_elements_zone_no = new List<Element>();
						List<ElementId> clash_elements_zone_ids = new List<ElementId>();

						string intersc = kp.Value.ToString(); // 

						string intersec_key = kp.Key.ToString(); // B / 1 - 0
																 //TaskDialog.Show("primero", intersec_key);

						foreach (Element elem in clash_elements)
						{
							if (elem.LookupParameter("Clash Grid Location").AsString() == intersec_key)
							{
								clash_elements_zone.Add(elem);
							}

							else
							{

								clash_elements_zone_no.Add(elem);
							}
						}
						//						if (clash_elements_zone.Count()==0) 
						//						{
						//							TaskDialog.Show("mensaje", clash_elements_zone.Count().ToString());
						//							continue;
						//						}

						if (clash_elements_zone.Count() > 1)
						{

							// get a ViewFamilyType for a 3D View
							ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
															 OfClass(typeof(ViewFamilyType)).
															 Cast<ViewFamilyType>()
															 where v.ViewFamily == ViewFamily.ThreeDimensional
															 select v).First();


							using (Transaction t = new Transaction(doc, "Create clash 3d view"))
							{

								t.Start();
								// Create the 3d view
								View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);

								foreach (Element elem in clash_elements_zone)
								{
									clash_elements_zone_ids.Add(elem.Id);
								}

								//double Min_X = double.MaxValue;
								//double Min_X = 0;
								//double Min_Y = double.MaxValue;
								//double Min_Y = 0;

								double Min_Z = double.MaxValue;

								// encontrar Min_Y , Min_X , Max_X , Max_Y
								double Min_X = double.MaxValue;
								double Min_Y = double.MaxValue;

								double Max_X = double.MinValue;
								double Max_Y = double.MinValue;
								double Max_Z = double.MinValue;

								foreach (ElementId id in clash_elements_zone_ids)
								{
									Element elm = doc.GetElement(id);

									BoundingBoxXYZ box = elm.get_BoundingBox(null);

									if (box.Max.X > Max_X)
									{
										Max_X = box.Max.X;
									}
									if (box.Max.Y > Max_Y)
									{
										Max_Y = box.Max.Y;
									}
									if (box.Max.Z > Max_Z)
									{
										Max_Z = box.Max.Z;
									}



									if (box.Min.X < Min_X)
									{
										Min_X = box.Min.X;
									}
									if (box.Min.Y < Min_Y)
									{
										Min_Y = box.Min.Y;
									}
									if (box.Min.Z < Min_Z)
									{
										Min_Z = box.Min.Z;
									}
								}

								double m = 5;

								XYZ Max = new XYZ(Max_X - m, Max_Y - m, Max_Z - m);
								XYZ Min = new XYZ(Min_X + m, Min_Y + m, Min_Z + m);

								msg = msg + Max.ToString() + Environment.NewLine + Min.ToString() + Environment.NewLine + "-----------------------------------------------------------" + Environment.NewLine;




								BoundingBoxXYZ myBox = new BoundingBoxXYZ();

								myBox.Min = Min;
								myBox.Max = Max;

								//BoundingBoxXYZ zone_bb = null;

								string number = (tresDclashview.Count() + 1).ToString();
								// Set the name of the view

								clashview.Name = number + ".- Zone    " + intersec_key + "     " + " Section Box    ";

								// Set the name of the transaction
								// A transaction can be renamed after it has been started
								t.SetName("Create view " + clashview.Name);

								BoundingBoxXYZ bb = myBox;
								BoundingBoxXYZ bi = new BoundingBoxXYZ();

								clashview.SetSectionBox(bb);
								//							        if (bb==null) 
								//							        {
								//							        	clashview.SetSectionBox(bi);
								//							        }
								//							        else
								//							        {
								//							        	clashview.SetSectionBox(bb);
								//							        }
								tresDclashview.Add(clashview);
								t.Commit();
								// There cannot be an open transaction when the active view is set



							}
						}


					}
					TaskDialog.Show("OUT", msg);
					uidoc.ActiveView = tresDclashview.First();
				}


			}

			void DYNO_create3DClashSectionBoxView_ZONE_3_nofunciona()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);

				// Get Active View
				View activeView = uidoc.ActiveView;

				List<View3D> tresDclashview = new List<View3D>();

				List<Element> clash_elements = new List<Element>();

				foreach (Element e in DYNO_GetAllClashElements_OnlyActiveView())
				{
					clash_elements.Add(e);
				}


				List<Element> clash_elements_zone = new List<Element>();
				List<Element> clash_elements_zone_no = new List<Element>();

				List<ElementId> clash_elements_zone_ids = new List<ElementId>();

				string msg = "";

				foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
				{
					string intersc = kp.Value.ToString(); // 

					string intersec_key = kp.Key.ToString(); // B / 1 - 0
															 //TaskDialog.Show("primero", intersec_key);

					foreach (Element elem in clash_elements)
					{
						if (elem.LookupParameter("Clash Grid Location").AsString() == intersec_key)
						{
							clash_elements_zone.Add(elem);
						}
						else
						{
							clash_elements_zone_no.Add(elem);
						}
					}

					if (clash_elements_zone.Count() > 1)
					{

						// get a ViewFamilyType for a 3D View
						ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
														 OfClass(typeof(ViewFamilyType)).
														 Cast<ViewFamilyType>()
														 where v.ViewFamily == ViewFamily.ThreeDimensional
														 select v).First();


						using (Transaction t = new Transaction(doc, "Create clash 3d view"))
						{

							t.Start();
							// Create the 3d view
							View3D clashview = View3D.CreateIsometric(doc, viewFamilyType.Id);

							foreach (Element elem in clash_elements_zone)
							{
								clash_elements_zone_ids.Add(elem.Id);
							}

							//							double Min_X = double.MaxValue;
							//							//double Min_X = 0;
							//							double Min_Y = double.MaxValue;
							//							//double Min_Y = 0;
							//							
							//							double Min_Z = double.MaxValue;
							//							
							//							// encontrar Min_Y , Min_X , Max_X , Max_Y
							//							
							//							
							//							
							//							double Max_X = Min_X;
							//							double Max_Y = Min_Y;
							//							double Max_Z = Min_Z;

							List<XYZ> lista_pMin = new List<XYZ>();
							List<XYZ> lista_pMax = new List<XYZ>();



							foreach (ElementId id in clash_elements_zone_ids)
							{
								Element elm = doc.GetElement(id);
								BoundingBoxXYZ box = elm.get_BoundingBox(clashview);

								XYZ pMin = box.Min; // lower, left , rear
								XYZ pMax = box.Max; // upper, right , front 

								lista_pMin.Add(pMin);
								lista_pMax.Add(pMax);

							}

							List<double> Value_min = new List<double>(); // Z
							List<string> KeyX_min = new List<string>(); // X as Strings
							List<string> KeyY_min = new List<string>(); // Y as Strings


							List<double> Value_max = new List<double>(); // Z
							List<string> KeyX_max = new List<string>(); // X as Strings
							List<string> KeyY_max = new List<string>(); // Y as Strings


							foreach (XYZ p in lista_pMin)
							{
								double Z = p.Z;
								Value_min.Add(Z);

								//double X = p.X;
								string X = p.X.ToString();
								KeyX_min.Add(X);

								//double Y = p.Y;
								string Y = p.Y.ToString();
								KeyY_min.Add(Y);
							}
							foreach (XYZ p in lista_pMax)
							{
								double Z = p.Z;
								Value_max.Add(Z);

								//double X = p.X;
								string X = p.X.ToString();
								KeyX_max.Add(X);

								//double Y = p.Y;
								string Y = p.Y.ToString();
								KeyY_max.Add(Y);
							}

							//min

							Dictionary<double, string> dict_min_X = new Dictionary<double, string>();
							Dictionary<double, string> dict_min_Y = new Dictionary<double, string>();
							for (int i = 0; i < Value_min.Count; i++)
							{
								dict_min_X.Add(Value_min[i], KeyX_min[i]);
								dict_min_Y.Add(Value_min[i], KeyY_min[i]);
							}
							Value_min.Sort();
							Value_min.Reverse(); // Z minimo, el de mas abajo
												 //Now look up our dictionary according to our sorted keys
							List<string> sortedVals_min_X = new List<string>();
							List<string> sortedVals_min_Y = new List<string>();
							for (int i = 0; i < Value_min.Count; i++)
							{
								sortedVals_min_X.Add(dict_min_X[Value_min[i]]);
								sortedVals_min_Y.Add(dict_min_Y[Value_min[i]]);
							}
							// valores string ordenados
							// MIN
							double Zmin = Value_min.First();
							string Xmin = sortedVals_min_X.First();
							string Ymin = sortedVals_min_Y.First();

							// max

							Dictionary<double, string> dict_max_X = new Dictionary<double, string>();
							Dictionary<double, string> dict_max_Y = new Dictionary<double, string>();
							for (int i = 0; i < Value_max.Count; i++)
							{
								dict_max_X.Add(Value_max[i], KeyX_max[i]);
								dict_max_Y.Add(Value_max[i], KeyY_max[i]);
							}
							Value_max.Sort();
							//Value_max.Reverse(); // Z minimo, el de mas abajo
							//Now look up our dictionary according to our sorted keys
							List<string> sortedVals_max_X = new List<string>();
							List<string> sortedVals_max_Y = new List<string>();
							for (int i = 0; i < Value_max.Count; i++)
							{
								sortedVals_max_X.Add(dict_max_X[Value_max[i]]);
								sortedVals_max_Y.Add(dict_max_Y[Value_max[i]]);
							}
							// valores string ordenados
							// MAX
							double Zmax = Value_max.First();
							string Xmax = sortedVals_max_X.First();
							string Ymax = sortedVals_max_Y.First();


							double Max_X = Convert.ToDouble(Xmax);
							double Max_Y = Convert.ToDouble(Ymax);
							double Max_Z = Zmax;

							double Min_X = Convert.ToDouble(Xmin);
							double Min_Y = Convert.ToDouble(Ymin);
							double Min_Z = Zmin;


							XYZ Max = new XYZ(Max_X, Max_Y, Max_Z);

							XYZ Min = new XYZ(Min_X, Min_Y, Min_Z);

							msg = msg + Max.ToString() + Environment.NewLine + Min.ToString() + Environment.NewLine + "-----------------------------------------------------------" + Environment.NewLine;




							BoundingBoxXYZ myBox = new BoundingBoxXYZ();

							myBox.Min = Min;
							myBox.Max = Max;

							//BoundingBoxXYZ zone_bb = null;

							string number = (tresDclashview.Count() + 1).ToString();
							// Set the name of the view

							clashview.Name = number + ".- Zone    " + intersec_key + "     " + " Section Box    ";

							// Set the name of the transaction
							// A transaction can be renamed after it has been started
							t.SetName("Create view " + clashview.Name);

							clashview.SetSectionBox(myBox);
							clashview.GetSectionBox();

							//cambiar dimensiones de sectionbox, que acote las dimensiones de los elementos.

							tresDclashview.Add(clashview);
							t.Commit();
							// There cannot be an open transaction when the active view is set



						}
					}
					else
					{

					}


				}
				TaskDialog.Show("OUT", msg);
				uidoc.ActiveView = tresDclashview.First();

			}

			List<Element> DYNO_GetAllClashElements_OnlyActiveView()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;


				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
						BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				List<Element> clash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				List<Element> clash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();



					foreach (Element elem in mechanicalequipment)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							clash.Add(elem);
						}
						else
						{
							clash_no.Add(elem);
						}
					}

				}

				// ELements
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				return clash;

			}

			List<Element> DYNO_GetAllNOClashElements_OnlyActiveView()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;


				// FAMILY INSTANCES

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
					    //BuiltInCategory.OST_CableTray,
					    BuiltInCategory.OST_CableTrayFitting,
					    //BuiltInCategory.OST_Conduit,
					    BuiltInCategory.OST_ConduitFitting,
					    //BuiltInCategory.OST_DuctCurves,
					    BuiltInCategory.OST_DuctFitting,
						BuiltInCategory.OST_DuctTerminal,
						BuiltInCategory.OST_ElectricalEquipment,
						BuiltInCategory.OST_ElectricalFixtures,
						BuiltInCategory.OST_LightingDevices,
						BuiltInCategory.OST_LightingFixtures,
						BuiltInCategory.OST_MechanicalEquipment,
					    //BuiltInCategory.OST_PipeCurves,
					    BuiltInCategory.OST_FlexDuctCurves,
						BuiltInCategory.OST_FlexPipeCurves,
						BuiltInCategory.OST_PipeFitting,
						BuiltInCategory.OST_PlumbingFixtures,
						BuiltInCategory.OST_SpecialityEquipment,
						BuiltInCategory.OST_Sprinklers,
						BuiltInCategory.OST_Wire,
					};

				// get elements with "clash" parameter value == "YES"
				List<Element> clash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				List<Element> clash_no = new List<Element>();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector MEcoll = new FilteredElementCollector(doc, activeView.Id);
					IList<Element> mechanicalequipment = MEcoll.WherePasses(MEInstancesFilter).ToElements();



					foreach (Element elem in mechanicalequipment)
					{
						if (elem.LookupParameter("Clash").AsString() == "YES")
						{
							clash.Add(elem);

						}
						else
						{
							clash_no.Add(elem);
						}
					}

				}

				// ELements
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(Pipe));
				ElementClassFilter elemFilter3 = new ElementClassFilter(typeof(Conduit));
				ElementClassFilter elemFilter4 = new ElementClassFilter(typeof(CableTray));
				ElementClassFilter elemFilter5 = new ElementClassFilter(typeof(FlexDuct));
				ElementClassFilter elemFilter6 = new ElementClassFilter(typeof(FlexPipe));

				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves);
				ElementCategoryFilter DUCategoryfilter3 = new ElementCategoryFilter(BuiltInCategory.OST_Conduit);
				ElementCategoryFilter DUCategoryfilter4 = new ElementCategoryFilter(BuiltInCategory.OST_CableTray);
				ElementCategoryFilter DUCategoryfilter5 = new ElementCategoryFilter(BuiltInCategory.OST_FlexDuctCurves);
				ElementCategoryFilter DUCategoryfilter6 = new ElementCategoryFilter(BuiltInCategory.OST_FlexPipeCurves);


				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter2, DUCategoryfilter2);
				LogicalAndFilter DUInstancesFilter3 = new LogicalAndFilter(elemFilter3, DUCategoryfilter3);
				LogicalAndFilter DUInstancesFilter4 = new LogicalAndFilter(elemFilter4, DUCategoryfilter4);
				LogicalAndFilter DUInstancesFilter5 = new LogicalAndFilter(elemFilter5, DUCategoryfilter5);
				LogicalAndFilter DUInstancesFilter6 = new LogicalAndFilter(elemFilter6, DUCategoryfilter6);

				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> pipes = DUcoll2.WherePasses(DUInstancesFilter2).ToElements();

				FilteredElementCollector DUcoll3 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> conduits = DUcoll3.WherePasses(DUInstancesFilter3).ToElements();

				FilteredElementCollector DUcoll4 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> cabletrays = DUcoll4.WherePasses(DUInstancesFilter4).ToElements();

				FilteredElementCollector DUcoll5 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexducts = DUcoll5.WherePasses(DUInstancesFilter5).ToElements();

				FilteredElementCollector DUcoll6 = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> flexpipes = DUcoll6.WherePasses(DUInstancesFilter6).ToElements();


				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in pipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in conduits)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in cabletrays)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				foreach (Element elem in flexpipes)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						clash.Add(elem);
					}
					else
					{
						clash_no.Add(elem);
					}
				}
				return clash_no;

			}


			#endregion // DYNO

			#region DRAFTS 
			void CreateClashParameters_Elements()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;

				CategorySet categorySet = app.Create.NewCategorySet();

				Category category = doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves);
				Category category2 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves);
				Category category3 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Conduit);
				Category category4 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTray);
				Category category5 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexDuctCurves);
				Category category6 = doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexPipeCurves);

				categorySet.Insert(category);
				categorySet.Insert(category2);
				categorySet.Insert(category3);
				categorySet.Insert(category4);
				categorySet.Insert(category5);
				categorySet.Insert(category6);

				string originalFile = app.SharedParametersFilename;
				//string tempFile = @"C:\Users\Public\Documents\Dynoscript\DynoscriptClashParameters.txt";

				try
				{
					app.SharedParametersFilename = originalFile;

					DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();

					foreach (DefinitionGroup dg in sharedParameterFile.Groups)
					{
						if (dg.Name == "ClashParameters")
						{
							ExternalDefinition externalDefinition = dg.Definitions.get_Item("Clash") as ExternalDefinition;
							ExternalDefinition externalDefinition2 = dg.Definitions.get_Item("Clash Category") as ExternalDefinition;
							ExternalDefinition externalDefinition3 = dg.Definitions.get_Item("Clash Comments") as ExternalDefinition;
							ExternalDefinition externalDefinition4 = dg.Definitions.get_Item("Clash Grid Location") as ExternalDefinition;
							ExternalDefinition externalDefinition5 = dg.Definitions.get_Item("Clash Solved") as ExternalDefinition;
							ExternalDefinition externalDefinition6 = dg.Definitions.get_Item("Done") as ExternalDefinition;
							ExternalDefinition externalDefinition7 = dg.Definitions.get_Item("ID Element") as ExternalDefinition;
							ExternalDefinition externalDefinition8 = dg.Definitions.get_Item("Percent Done") as ExternalDefinition;
							ExternalDefinition externalDefinition9 = dg.Definitions.get_Item("Zone") as ExternalDefinition;

							using (Transaction t = new Transaction(doc))
							{
								t.Start("Create Clash Parameters");
								//parameter binding
								InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB2 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB3 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB4 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB5 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB6 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB7 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB8 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB9 = app.Create.NewInstanceBinding(categorySet);

								//parameter group to constraints
								doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition2, newIB2, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition3, newIB3, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition4, newIB4, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition5, newIB5, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition6, newIB6, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition7, newIB7, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition8, newIB8, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition9, newIB9, BuiltInParameterGroup.PG_CONSTRAINTS);

								t.Commit();
							}
						}

					}
				}
				catch
				{

				}
				finally
				{
					//reset to original file
					app.SharedParametersFilename = originalFile;
				}
			}

			void CreateClashParameters_FamilyInstance()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////List<Category> categorias = ObtenerCategorias(doc);
				//Application app = this.Application;

				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_FlexDuctCurves,
					BuiltInCategory.OST_FlexPipeCurves,
					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};

				CategorySet categorySet = app.Create.NewCategorySet();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					Category category = doc.Settings.Categories.get_Item(bic);
					categorySet.Insert(category);
				}

				string originalFile = app.SharedParametersFilename;
				//string tempFile = @"C:\Users\Public\Documents\Dynoscript\DynoscriptClashParameters.txt";

				try
				{
					app.SharedParametersFilename = originalFile;

					DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();

					foreach (DefinitionGroup dg in sharedParameterFile.Groups)
					{
						if (dg.Name == "ClashParameters" || dg.Name == "ClashParameters_families")
						{
							ExternalDefinition externalDefinition = dg.Definitions.get_Item("Clash") as ExternalDefinition;
							ExternalDefinition externalDefinition2 = dg.Definitions.get_Item("Clash Category") as ExternalDefinition;
							ExternalDefinition externalDefinition3 = dg.Definitions.get_Item("Clash Comments") as ExternalDefinition;
							ExternalDefinition externalDefinition4 = dg.Definitions.get_Item("Clash Grid Location") as ExternalDefinition;
							ExternalDefinition externalDefinition5 = dg.Definitions.get_Item("Clash Solved") as ExternalDefinition;
							ExternalDefinition externalDefinition6 = dg.Definitions.get_Item("Done") as ExternalDefinition;
							ExternalDefinition externalDefinition7 = dg.Definitions.get_Item("ID Element") as ExternalDefinition;
							ExternalDefinition externalDefinition8 = dg.Definitions.get_Item("Percent Done") as ExternalDefinition;
							ExternalDefinition externalDefinition9 = dg.Definitions.get_Item("Zone") as ExternalDefinition;

							using (Transaction t = new Transaction(doc))
							{
								t.Start("Create Clash Parameters");
								//parameter binding
								InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB2 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB3 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB4 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB5 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB6 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB7 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB8 = app.Create.NewInstanceBinding(categorySet);
								InstanceBinding newIB9 = app.Create.NewInstanceBinding(categorySet);

								//parameter group to constraints
								doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition2, newIB2, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition3, newIB3, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition4, newIB4, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition5, newIB5, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition6, newIB6, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition7, newIB7, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition8, newIB8, BuiltInParameterGroup.PG_CONSTRAINTS);
								doc.ParameterBindings.Insert(externalDefinition9, newIB9, BuiltInParameterGroup.PG_CONSTRAINTS);

								t.Commit();
							}
						}

					}
				}
				catch
				{

				}
				finally
				{
					//reset to original file
					app.SharedParametersFilename = originalFile;
				}
			}

			void CreateClashParameters_Family()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;


				// Parameters strings
				string paramName = "Clash";
				string paramName2 = "Clash Category";
				string paramName3 = "Clash Comments";
				string paramName4 = "Clash Grid Location";
				string paramName5 = "Clash Solved";
				string paramName6 = "Done";
				string paramName7 = "ID Element";
				string paramName8 = "Percent Done";
				string paramName9 = "Zone";

				// open shared parameter file
				DefinitionFile myDefinitionFile = app.OpenSharedParameterFile();

				// get a group
				//Only works with a GroupParameterName which already exists in the SharedParameters file, e.g. "Dimensions". Not ideal!
				//Also this seems quite redundant because later I am inserting the new parameter on the "Data" ParameterGroup… How to go around this?
				DefinitionGroup myGroup = myDefinitionFile.Groups.Create("ClashParameters_families");

				// create an instance definition in definition group MyParameters
				ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text);
				ExternalDefinitionCreationOptions option2 = new ExternalDefinitionCreationOptions(paramName2, ParameterType.Text);
				ExternalDefinitionCreationOptions option3 = new ExternalDefinitionCreationOptions(paramName3, ParameterType.Text);
				ExternalDefinitionCreationOptions option4 = new ExternalDefinitionCreationOptions(paramName4, ParameterType.Text);
				ExternalDefinitionCreationOptions option5 = new ExternalDefinitionCreationOptions(paramName5, ParameterType.YesNo);
				ExternalDefinitionCreationOptions option6 = new ExternalDefinitionCreationOptions(paramName6, ParameterType.YesNo);
				ExternalDefinitionCreationOptions option7 = new ExternalDefinitionCreationOptions(paramName7, ParameterType.Text);
				ExternalDefinitionCreationOptions option8 = new ExternalDefinitionCreationOptions(paramName8, ParameterType.Text);
				ExternalDefinitionCreationOptions option9 = new ExternalDefinitionCreationOptions(paramName9, ParameterType.Text);


				// let the user modify the value, only the API
				option.UserModifiable = true;
				option2.UserModifiable = true;
				option3.UserModifiable = true;
				option4.UserModifiable = true;
				option5.UserModifiable = true;
				option6.UserModifiable = true;
				option7.UserModifiable = true;
				option8.UserModifiable = true;
				option9.UserModifiable = true;

				// Set tooltip
				option.Description = "Determina si el elemento colisiona con otro o no";
				option2.Description = "Determina si el elemento colisiona con otro o no";
				option3.Description = "Determina si el elemento colisiona con otro o no";
				option4.Description = "Determina si el elemento colisiona con otro o no";
				option5.Description = "Determina si el elemento colisiona con otro o no";
				option6.Description = "Determina si el elemento colisiona con otro o no";
				option7.Description = "Determina si el elemento colisiona con otro o no";
				option8.Description = "Determina si el elemento colisiona con otro o no";
				option9.Description = "Determina si el elemento colisiona con otro o no";

				Definition myDefinition_ProductDate = myGroup.Definitions.Create(option);
				Definition myDefinition_ProductDate2 = myGroup.Definitions.Create(option2);
				Definition myDefinition_ProductDate3 = myGroup.Definitions.Create(option3);
				Definition myDefinition_ProductDate4 = myGroup.Definitions.Create(option4);
				Definition myDefinition_ProductDate5 = myGroup.Definitions.Create(option5);
				Definition myDefinition_ProductDate6 = myGroup.Definitions.Create(option6);
				Definition myDefinition_ProductDate7 = myGroup.Definitions.Create(option7);
				Definition myDefinition_ProductDate8 = myGroup.Definitions.Create(option8);
				Definition myDefinition_ProductDate9 = myGroup.Definitions.Create(option9);


				BuiltInCategory[] bics_familyIns = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				    //BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_FlexDuctCurves,
					BuiltInCategory.OST_FlexPipeCurves,
					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,
					};

				CategorySet categories = app.Create.NewCategorySet();

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					Category category = doc.Settings.Categories.get_Item(bic);
					categories.Insert(category);
				}


				// insert the new parameter
				InstanceBinding instanceBinding = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding2 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding3 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding4 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding5 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding6 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding7 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding8 = app.Create.NewInstanceBinding(categories);
				InstanceBinding instanceBinding9 = app.Create.NewInstanceBinding(categories);

				using (Transaction t = new Transaction(doc, "CreateParameters"))
				{
					t.Start();
					doc.ParameterBindings.Insert(myDefinition_ProductDate, instanceBinding, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate2, instanceBinding2, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate3, instanceBinding3, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate4, instanceBinding4, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate5, instanceBinding5, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate6, instanceBinding6, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate7, instanceBinding7, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate8, instanceBinding8, BuiltInParameterGroup.PG_CONSTRAINTS);
					doc.ParameterBindings.Insert(myDefinition_ProductDate9, instanceBinding9, BuiltInParameterGroup.PG_CONSTRAINTS);

					t.Commit();
				}

				foreach (BuiltInCategory bic in bics_familyIns)
				{
					// category and are family.
					ElementClassFilter familyFilter = new ElementClassFilter(typeof(Family));
					// Create a category filter for MechanicalEquipment
					ElementCategoryFilter MECategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment FamilyInstances
					LogicalAndFilter MEInstancesFilter = new LogicalAndFilter(familyFilter, MECategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector coll = new FilteredElementCollector(doc);
					IList<Element> mechanicalEquipment = coll.WherePasses(MEInstancesFilter).ToElements();

					foreach (Family f in mechanicalEquipment)
					{

						if (!f.IsEditable)
							continue;

						Document famdoc = doc.EditFamily(f);

						using (Transaction t = new Transaction(famdoc, "CreateParametersFamily"))
						{
							t.Start();
							FamilyParameter FamParam = famdoc.FamilyManager.AddParameter(paramName, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam2 = famdoc.FamilyManager.AddParameter(paramName2, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam3 = famdoc.FamilyManager.AddParameter(paramName3, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam4 = famdoc.FamilyManager.AddParameter(paramName4, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam5 = famdoc.FamilyManager.AddParameter(paramName5, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam6 = famdoc.FamilyManager.AddParameter(paramName6, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam7 = famdoc.FamilyManager.AddParameter(paramName7, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam8 = famdoc.FamilyManager.AddParameter(paramName8, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);
							FamilyParameter FamParam9 = famdoc.FamilyManager.AddParameter(paramName9, BuiltInParameterGroup.PG_CONSTRAINTS, ParameterType.Text, true);

							t.Commit();
						}

					}
				}



			}

			void IntersectElementToFamilyInstances()
			{
				// Find intersections between family instances and a selected element 

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "Select element that will be checked for intersection with all family instances");

				Element e = doc.GetElement(reference);

				GeometryElement geomElement = e.get_Geometry(new Options());

				Solid solid = null;
				foreach (GeometryObject geomObj in geomElement)
				{
					GeometryElement geoInstance = (geomObj as GeometryInstance).GetInstanceGeometry();
					foreach (GeometryObject geomObje2 in geoInstance)
					{
						solid = geomObje2 as Solid;
						if (solid != null) break;
					}

				}
				TaskDialog.Show("jeje", solid.ToString() + Environment.NewLine);
				// Find intersections between family instances and a selected element
				FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id).WhereElementIsNotElementType();
				collector.OfClass(typeof(FamilyInstance));
				List<Element> lstElemFound = collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements().ToList(); // Apply intersection filter to find matches


				TaskDialog.Show("Revit", lstElemFound.Count() + " family instances intersect with the selected element (" + e.Category.Name + " id:" + e.Id.ToString() + ")");
			}

			void IntersectElementToCategory()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;


				Reference reference = uidoc.Selection.PickObject(ObjectType.Element, "Select element that will be checked for intersection with all category elements");

				Element e = doc.GetElement(reference);

				ElementId eID = e.Id;

				GeometryElement geomElement = e.get_Geometry(new Options());

				Solid solid = null;
				foreach (GeometryObject geomObj in geomElement)
				{
					solid = geomObj as Solid;
					if (solid != null) break;
				}
				// Find intersections between family instances and a selected element
				// category Duct.
				ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);

				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(DUFilter, DUCategoryfilter);

				ICollection<ElementId> collectoreID = new List<ElementId>();
				collectoreID.Add(eID);

				ExclusionFilter filter = new ExclusionFilter(collectoreID);
				FilteredElementCollector collector = new FilteredElementCollector(doc);
				collector.OfClass(typeof(Duct));
				collector.WherePasses(DUInstancesFilter);
				collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
				collector.WherePasses(filter);

				TaskDialog.Show("Revit", collector.Count() + " elements intersect with the selected element (" + e.Category.Name + " id:" + eID.ToString() + ")");
			}
			// lista de elementos todas categorias vs lista de elementos todas categorias
			void Listas_IntersectMultipleElementsToCategory()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//GetConnectorElements(doc, false);

				BuiltInCategory[] bics = new BuiltInCategory[]
					{
					BuiltInCategory.OST_CableTray,
				    //BuiltInCategory.OST_CableTrayFitting,
				    BuiltInCategory.OST_Conduit,
				    //BuiltInCategory.OST_ConduitFitting,
				    BuiltInCategory.OST_DuctCurves,
				    //BuiltInCategory.OST_DuctFitting,
				    //BuiltInCategory.OST_DuctTerminal,
				    //BuiltInCategory.OST_ElectricalEquipment,
				    //BuiltInCategory.OST_ElectricalFixtures,
				    //BuiltInCategory.OST_LightingDevices,
				    //BuiltInCategory.OST_LightingFixtures,
				    //BuiltInCategory.OST_MechanicalEquipment,
				    BuiltInCategory.OST_PipeCurves,
						//BuiltInCategory.OST_PipeFitting,
						//BuiltInCategory.OST_PlumbingFixtures,
						//BuiltInCategory.OST_SpecialityEquipment,
						//BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,

						//BuiltInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming   
					};

				BuiltInCategory[] bics2 = new BuiltInCategory[]
					{
					BuiltInCategory.OST_CableTray,
				    //BuiltInCategory.OST_CableTrayFitting,
				    BuiltInCategory.OST_Conduit,
				    //BuiltInCategory.OST_ConduitFitting,
				    BuiltInCategory.OST_DuctCurves,
				    //BuiltInCategory.OST_DuctFitting,
				    //BuiltInCategory.OST_DuctTerminal,
				    //BuiltInCategory.OST_ElectricalEquipment,
				    //BuiltInCategory.OST_ElectricalFixtures,
				    //BuiltInCategory.OST_LightingDevices,
				    //BuiltInCategory.OST_LightingFixtures,
				    //BuiltInCategory.OST_MechanicalEquipment,
				    BuiltInCategory.OST_PipeCurves,
						//BuiltInCategory.OST_PipeFitting,
						//BuiltInCategory.OST_PlumbingFixtures,
						//BuiltInCategory.OST_SpecialityEquipment,
						//BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,

						//BuiltInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming   
					};

				BuiltInCategory[] bics_fi = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				   // BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
				    //BuiltInCategory.OST_Wire,
				    
				    //builtInCategory.OST_Walls,
				    //builtInCategory.OST_Ceilings,
				    BuiltInCategory.OST_StructuralFraming,
					};

				string mensaje = "";
				string mensaje2 = "";
				string mensaje3 = "------**////.....CLASH DETECTION TOOL - DYNOSCRIPT.........////**--------" + Environment.NewLine;
				string msg = "";
				List<Element> clash_yesA = new List<Element>();
				foreach (BuiltInCategory bic in bics)
				{
					IList<ElementFilter> a = new List<ElementFilter>(bics.Count());

					foreach (BuiltInCategory i in bics)
					{
						a.Add(new ElementCategoryFilter(i));
					}
					LogicalOrFilter categoryFilter = new LogicalOrFilter(a);

					IList<ElementFilter> b = new List<ElementFilter>(4);

					b.Add(new ElementClassFilter(typeof(CableTray)));
					b.Add(new ElementClassFilter(typeof(Conduit)));
					b.Add(new ElementClassFilter(typeof(Duct)));
					b.Add(new ElementClassFilter(typeof(Pipe)));

					LogicalOrFilter classFilter = new LogicalOrFilter(b);
					// Create a category filter for Ducts
					ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(bic);
					// Create a logic And filter for all MechanicalEquipment Family
					LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(categoryFilter, DUCategoryfilter);
					// Apply the filter to the elements in the active document
					FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
					IList<Element> DU = DUcoll.WherePasses(DUInstancesFilter).ToElements();

					//List<Element> clash_yesA = new List<Element>();

					foreach (Element e in DU)
					{

						ElementId eID = e.Id;

						GeometryElement geomElement = e.get_Geometry(new Options());

						Solid solid = null;

						foreach (GeometryObject geomObj in geomElement)
						{
							solid = geomObj as Solid;
							if (solid != null)
							{
								break;
							}

						}// solid = geomObj;
						 // Find intersections 

						// category Duct.
						ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
						// Create a category filter for Ducts
						ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);

						LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

						ICollection<ElementId> collectoreID = new List<ElementId>();
						collectoreID.Add(eID);

						//Collector = Clashes
						ExclusionFilter filter = new ExclusionFilter(collectoreID);
						FilteredElementCollector collector = new FilteredElementCollector(doc);
						collector.OfClass(typeof(Duct));
						collector.WherePasses(DU2InstancesFilter);
						collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
						collector.WherePasses(filter);

						if (collector.Count() > 0)
						{
							clash_yesA.Add(e);
						}

						foreach (Element elem in collector)
						{
							clash_yesA.Add(elem);
						}

						mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " elements intersect with the selected element ("
																					+ e.Name.ToString()
																					+ e.Category.Name.ToString() + " id:"
																					+ eID.ToString() + ")";

					}

					foreach (Element elem in clash_yesA)
					{
						msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
						// set clash YES
						Parameter param = elem.LookupParameter("Clash");
						string clash = "YES";
						using (Transaction t = new Transaction(doc, "Clash YES"))
						{
							t.Start();
							param.Set(clash);
							t.Commit();
						}
					}
					mensaje2 = mensaje2 + mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
					//TaskDialog.Show("Revit", mensaje2);

				}
				mensaje3 = mensaje3 + mensaje2 + Environment.NewLine;
				TaskDialog.Show("Revit", mensaje3);
			}
			// ductos vs ductos // Elements vs Elements
			void IntersectMultipleElementsToCategory()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//GetConnectorElements(doc, false);
				// Get Active View
				View activeView = uidoc.ActiveView;

				BuiltInCategory[] bics = new BuiltInCategory[]
					{
					BuiltInCategory.OST_CableTray,
				    //BuiltInCategory.OST_CableTrayFitting,
				    BuiltInCategory.OST_Conduit,
				    //BuiltInCategory.OST_ConduitFitting,
				    BuiltInCategory.OST_DuctCurves,
				    //BuiltInCategory.OST_DuctFitting,
				    //BuiltInCategory.OST_DuctTerminal,
				    //BuiltInCategory.OST_ElectricalEquipment,
				    //BuiltInCategory.OST_ElectricalFixtures,
				    //BuiltInCategory.OST_LightingDevices,
				    //BuiltInCategory.OST_LightingFixtures,
				    //BuiltInCategory.OST_MechanicalEquipment,
				    BuiltInCategory.OST_PipeCurves,
					BuiltInCategory.OST_FlexPipeCurves,
					BuiltInCategory.OST_FlexDuctCurves,
						//BuiltInCategory.OST_PipeFitting,
						//BuiltInCategory.OST_PlumbingFixtures,
						//BuiltInCategory.OST_SpecialityEquipment,
						//BuiltInCategory.OST_Sprinklers,
						//BuiltInCategory.OST_Wire,

						//BuiltInCategory.OST_Walls,
						//builtInCategory.OST_Ceilings,
						//BuiltInCategory.OST_StructuralFraming   
					};

				BuiltInCategory[] bics_fi = new BuiltInCategory[]
					{
				    //BuiltInCategory.OST_CableTray,
				    BuiltInCategory.OST_CableTrayFitting,
				   // BuiltInCategory.OST_Conduit,
				    BuiltInCategory.OST_ConduitFitting,
				    //BuiltInCategory.OST_DuctCurves,
				    BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
				    //BuiltInCategory.OST_PipeCurves,
				    BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
				    //BuiltInCategory.OST_Wire,
				    
				    //builtInCategory.OST_Walls,
				    //builtInCategory.OST_Ceilings,
				    BuiltInCategory.OST_StructuralFraming,
					};

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				List<Element> clash_yesA = new List<Element>();
				string mensaje = "";
				foreach (Element e in ducts)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;

					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null)
						{
							break;
						}

					}// solid = geomObj;
					 // Find intersections 

					// category Duct.
					ElementClassFilter DUFilter = new ElementClassFilter(typeof(Duct));
					// Create a category filter for Ducts
					ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);

					LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					//Collector = Clashes
					ExclusionFilter filter = new ExclusionFilter(collectoreID);
					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
					collector.OfClass(typeof(Duct));
					collector.WherePasses(DU2InstancesFilter);
					collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
					collector.WherePasses(filter);

					if (collector.Count() > 0)
					{
						clash_yesA.Add(e);
					}

					foreach (Element elem in collector)
					{
						clash_yesA.Add(elem);
					}

					mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " elements intersect with the selected element ("
																				+ e.Name.ToString()
																				+ e.Category.Name.ToString() + " id:"
																				+ eID.ToString() + ")";

				}
				string mensaje2 = "";
				string msg = "";
				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			}
			// ductos vs Mechanical Equipments // Elements vs FamilyInstances
			void IntersectMultipleElementsToFamilyInstances()
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				// Create a category filter for Ducts
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
				// Apply the filter to the elements in the active document
				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				string numero_ductos = ducts.Count().ToString();
				string mensaje = "Iteraciones de Ductos : " + numero_ductos + "\n\n" + Environment.NewLine;

				List<Element> clash_yesA = new List<Element>();

				foreach (Element e in ducts)
				{

					ElementId eID = e.Id;

					GeometryElement geomElement = e.get_Geometry(new Options());

					Solid solid = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						solid = geomObj as Solid;
						if (solid != null) break;
					}

					// Find intersections between family instances and a selected element
					// category Mechanical Euqipment
					ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
					// Create a category filter for Mechanical Euqipment
					ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_MechanicalEquipment);

					LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);

					ICollection<ElementId> collectoreID = new List<ElementId>();
					collectoreID.Add(eID);

					ExclusionFilter filter = new ExclusionFilter(collectoreID);
					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
					collector.OfClass(typeof(FamilyInstance));
					collector.WherePasses(DU2InstancesFilter);
					collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
																								 //collector.WherePasses(filter);

					if (collector.Count() > 0)
					{
						clash_yesA.Add(e);
					}

					foreach (Element elem in collector)
					{
						clash_yesA.Add(elem);
					}
					mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
																				+ e.Name.ToString()
																				+ e.Category.Name.ToString() + " id:"
																				+ eID.ToString() + ")";
				}
				string mensaje2 = "";
				string msg = "";
				foreach (Element elem in clash_yesA)
				{
					msg += "Nombre : " + elem.Name.ToString() + " ID : " + elem.Id.ToString() + "Categoria : " + elem.Category.Name.ToString() + Environment.NewLine;
					// set clash YES
					Parameter param = elem.LookupParameter("Clash");
					string clash = "YES";
					using (Transaction t = new Transaction(doc, "Clash YES"))
					{
						t.Start();
						param.Set(clash);
						t.Commit();
					}
				}
				mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				TaskDialog.Show("Revit", mensaje2);
			}

			void FilterClashElementsInActiveView()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// get elements from active view
				FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, activeView.Id);
				allElementsInView.ToElements();

				// ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				// ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				// LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(familyFilter, DUFCategoryfilter);
				// 
				// IList<Element> ductFittings = allElementsInView.WherePasses(DUFInstancesFilter).ToElements();
				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();

				// get elements with "clash" parameter value != "YES" or == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}
				string msg = "";
				foreach (Element elem in ductsclash)
				{
					msg += elem.Id.ToString() + elem.Name.ToString() + " ID: " + elem.Id.ToString() + Environment.NewLine;
				}
				TaskDialog.Show("jfjfjfj", msg);
				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				OverrideGraphicSettings ogs1 = new OverrideGraphicSettings();
				ogs1.SetProjectionLineColor(new Color(250, 0, 0));
				ogs1.SetSurfaceBackgroundPatternColor(new Color(250, 0, 0));
				ogs1.SetSurfaceBackgroundPatternVisible(true);
				ogs1.SetSurfaceBackgroundPatternId(solidFillPattern.Id);

				OverrideGraphicSettings ogs2 = new OverrideGraphicSettings();
				ogs2.SetProjectionLineColor(new Color(192, 192, 192));
				ogs2.SetSurfaceBackgroundPatternColor(new Color(192, 192, 192));
				ogs2.SetSurfaceBackgroundPatternVisible(true);
				ogs2.SetSurfaceBackgroundPatternId(solidFillPattern.Id);
				ogs2.SetHalftone(true);

				using (Transaction t = new Transaction(doc, "Set clash Override"))
				{
					t.Start();
					foreach (Element elem in ductsclash)
					{
						activeView.SetElementOverrides(elem.Id, ogs1);
					}
					foreach (Element elem in ductsclash_no)
					{
						activeView.SetElementOverrides(elem.Id, ogs2);
					}
					t.Commit();
				}
			} // Colorea encima las instancias

			void SetClashViewTemplateFilter()
			{
				//Document doc = this.ActiveUIDocument.Document;

				View viewTemplate = (from v in new FilteredElementCollector(doc)
									.OfClass(typeof(View))
									.Cast<View>()
									 where v.IsTemplate == true && v.Name == "CLASH VIEW FILTER"
									 select v)
									.First();

				using (Transaction t = new Transaction(doc, "Set View Template"))
				{
					t.Start();
					doc.ActiveView.ViewTemplateId = viewTemplate.Id;
					t.Commit();
				}
			}

			void SetNoValueClashParameter()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// get elements from active view
				FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, activeView.Id);
				allElementsInView.ToElements();

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}

				foreach (Element e in ductsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Comments");
					Parameter param3 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(param_value);
						param3.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(1);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				// category ducts fittings
				ElementClassFilter DUFelemFilter = new ElementClassFilter(typeof(FamilyInstance));
				// Create a category filter for Ducts
				ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				// Create a logic And filter for all MechanicalEquipment Family
				LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(DUFelemFilter, DUFCategoryfilter);
				// Apply the filter to the elements in the active document
				FilteredElementCollector DUFcoll = new FilteredElementCollector(doc);
				IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductfittingsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductfittingsclash_no = new List<Element>();

				foreach (Element elem in ductfittings)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductfittingsclash.Add(elem);
					}
					else
					{
						ductfittingsclash_no.Add(elem);
					}
				}


				foreach (Element e in ductfittingsclash)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Solved");
					Parameter param3 = e.LookupParameter("Clash Comments");
					Parameter param4 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(1);
						param3.Set(param_value);
						param4.Set(param_value);
						t.Commit();
					}
				}

				foreach (Element e in ductfittingsclash_no)
				{
					Parameter param = e.LookupParameter("Clash");
					Parameter param2 = e.LookupParameter("Clash Comments");
					Parameter param3 = e.LookupParameter("Clash Grid Location");
					string param_value = "";

					using (Transaction t = new Transaction(doc, "Set No value to Clash elements in Active View"))
					{
						t.Start();
						param.Set(param_value);
						param2.Set(param_value);
						param3.Set(param_value);
						t.Commit();
					}
				}

			}

			void ClashComments()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				// get elements from active view
				FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, activeView.Id);
				allElementsInView.ToElements();

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

				// get elements with "clash" parameter value == "YES"
				IList<Element> ductsclash = new List<Element>();
				// get elements with "clash" parameter value == "NO"
				IList<Element> ductsclash_no = new List<Element>();

				foreach (Element elem in ducts)
				{
					if (elem.LookupParameter("Clash").AsString() == "YES")
					{
						ductsclash.Add(elem);
					}
					else
					{
						ductsclash_no.Add(elem);
					}
				}

				using (var form = new Form1())
				{
					form.ShowDialog();

					if (form.DialogResult == forms.DialogResult.Cancel) return;

					string param_value = form.textString.ToString();

					foreach (Element e in ductsclash)
					{
						Parameter param = e.LookupParameter("Clash Comments");
						Parameter param2 = e.LookupParameter("Clash Solved");

						using (Transaction t = new Transaction(doc, "Set Comment value to Clash comments paramtere in Active View"))
						{
							t.Start();
							param.Set(param_value);
							param2.Set(0);
							t.Commit();
						}
					}
				}


			}

			void SetClashGridLocation_FamilyInstance()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;
				// Get Active View
				View activeView = uidoc.ActiveView;

				// category ductfittings

				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				// Create a category filter for duct fittings
				ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				// Create a logic And filter for all duct fittings
				LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(familyFilter, DUFCategoryfilter);
				// Apply the filter to the elements in the active document
				FilteredElementCollector DUFcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();


				ICollection<ElementId> ductfittings_id = DUFcoll.WherePasses(DUFInstancesFilter).ToElementIds() as ICollection<ElementId>;

				ICollection<ElementId> ecol = DUFcoll.WherePasses(DUFInstancesFilter).ToElementIds() as ICollection<ElementId>;

				// transformar lista de Elementos hacia Selection

				//ICollection<ElementId> ecol = uidoc.Selection.GetElementIds();

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				foreach (ElementId eid in ecol)
				{
					Element e = doc.GetElement(eid);
					LocationPoint p = e.Location as LocationPoint;

					XYZ xyz = new XYZ(p.Point.X, p.Point.Y, 0);
					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Clash Grid Location : " + param_value + "      Category:" + e.Category.Name.ToString() + "      ID : " + e.Id.ToString() + Environment.NewLine;
				}
				TaskDialog.Show("Clash Grid Location", msg);

			}

			void SetClashGridLocation_Element()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;
				// Get Active View
				View activeView = uidoc.ActiveView;

				// category Duct.
				ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
				ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
				LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);

				FilteredElementCollector DUcoll = new FilteredElementCollector(doc, activeView.Id);
				IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();







				ICollection<ElementId> ducts_id = DUcoll.WherePasses(DUInstancesFilter).ToElementIds() as ICollection<ElementId>;

				ICollection<ElementId> ecol = DUcoll.WherePasses(DUInstancesFilter).ToElementIds() as ICollection<ElementId>;

				// transformar lista de Elementos hacia Selection

				//ICollection<ElementId> ecol = uidoc.Selection.GetElementIds();

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				XYZ p1 = new XYZ();
				XYZ p2 = new XYZ();

				foreach (ElementId eid in ecol)
				{
					Element e = doc.GetElement(eid);

					Options op = new Options();
					//            		op.View = doc.ActiveView;
					//            		op.ComputeReferences = true;
					GeometryElement gm = e.get_Geometry(op);
					Solid so = gm.First() as Solid;
					XYZ p = so.ComputeCentroid();

					XYZ xyz = new XYZ(p.X, p.Y, 0);

					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Centroid Point : (" + p.X.ToString() + ", " + p.Y.ToString() + ", 0)  :  " + "Clash Grid Location : " + param_value + "  Category:" + e.Category.Name.ToString() + "  ID : " + e.Id.ToString() + Environment.NewLine;
				}
				TaskDialog.Show("Clash Grid Location", msg);

			}

			void getNearestGridIntersectionPointFromSelection_FamilyInstance()

			{
				//UIDocument uidoc = ActiveUIDocument;
				//Document doc = ActiveUIDocument.Document;
				// Get Active View
				View activeView = uidoc.ActiveView;

				// category ductfittings

				ElementClassFilter familyFilter = new ElementClassFilter(typeof(FamilyInstance));
				// Create a category filter for duct fittings
				ElementCategoryFilter DUFCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
				// Create a logic And filter for all duct fittings
				LogicalAndFilter DUFInstancesFilter = new LogicalAndFilter(familyFilter, DUFCategoryfilter);
				// Apply the filter to the elements in the active document
				FilteredElementCollector DUFcoll = new FilteredElementCollector(doc, activeView.Id);

				IList<Element> ductfittings = DUFcoll.WherePasses(DUFInstancesFilter).ToElements();


				ICollection<ElementId> ecol = uidoc.Selection.GetElementIds();

				//				IList<ElementId> ductfittings_id = DUFcoll.WherePasses(DUFInstancesFilter).ToElementIds() as IList<ElementId>;
				//				
				//				IList<ElementId> ecol = uidoc.Selection.SetElementIds(ductfittings_id);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);
				string intersection = "??";
				string msg = "Los resultados son:\n" + Environment.NewLine;

				foreach (ElementId eid in ecol)
				{
					Element e = doc.GetElement(eid);
					LocationPoint p = e.Location as LocationPoint;
					// si es una Familia instance
					//					if (eid.idfamilyinstance == familuinstance type) 
					//					{
					//						LocationPoint p = e.Location as LocationPoint;
					//					}
					//					else // sino es elemento no familia instance
					//					{
					//						LocationCurve p = e.Location as LocationCurve;
					//					}



					XYZ xyz = new XYZ(p.Point.X, p.Point.Y, 0);
					double distanceMin = 0;
					double distance = 0;

					distanceMin = xyz.DistanceTo(intersectionPoints.First().Value);
					foreach (KeyValuePair<string, XYZ> kp in intersectionPoints)
					{
						distance = xyz.DistanceTo(kp.Value);
						if (distance < distanceMin)
						{
							distanceMin = distance;
							intersection = kp.Key;
						}

					}

					double distanceInMeter = distanceMin / 3.281;
					//TaskDialog.Show("Clash Grid Location", intersection);

					Parameter param = e.LookupParameter("Clash Grid Location");
					string param_value = intersection;

					using (Transaction t = new Transaction(doc, "Set Clash grid location to element"))
					{
						t.Start();
						param.Set(param_value);
						t.Commit();
					}
					msg += "Clash Grid Location : " + param_value + "      Category:" + e.Category.Name.ToString() + "      ID : " + e.Id.ToString() + Environment.NewLine;
				}
				TaskDialog.Show("Clash Grid Location", msg);

			}

			void CreateClashSchedule()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ViewSchedule clashSchedule = null;

				using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
				{
					transaction.Start();

					FilteredElementCollector collector = new FilteredElementCollector(doc);
					collector.OfCategory(BuiltInCategory.OST_DuctCurves);
					//Get first ElementId of AreaScheme.
					ElementId areaSchemeId = collector.FirstElementId();
					if (areaSchemeId != null && areaSchemeId != ElementId.InvalidElementId)
					{
						// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
						// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
						// and the value of third argument must be ElementId of an AreaScheme.
						clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.OST_DuctCurves));

						doc.Regenerate();

						//Add fields

					}
					//    			ScheduleDefinition definition = clashSchedule.Definition;
					//    			
					//    			IList<SchedulableField> schedulableFields = definition.GetSchedulableFields();
					//    			
					//    			List<SchedulableField> listashparam = new List<SchedulableField>();
					//    			
					//    			foreach (SchedulableField element in schedulableFields) 
					//    			{
					//    				if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Clash" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}	
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Clash Category" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Clash Comments" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Clash Grid Location" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Clash Solved" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Done" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "ID Element" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Percent Done" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//					if (element.ParameterId.IntegerValue > 0 && element.GetName(doc).ToString() == "Zone" )
					//    				{
					//    					listashparam.Add(element);	
					//    				}
					//
					//    			}
					//    			
					//    			string msg = "";
					//    			
					//    			
					//    			List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();

					//    			foreach (SchedulableField element in listashparam)
					//    			{
					//					
					////    				if (element.ParameterId.IntegerValue > 0 )
					////    				{
					////    					listashparam.Add(element);	
					////    				}	
					//					if (element.GetName(doc).ToString() == "Clash") 
					////    				if (element.GetName(doc).ToString() == "Clash" || 
					////    				    element.GetName(doc).ToString() == "Clash Category" ||
					////    				    element.GetName(doc).ToString() == "Clash Comments" ||
					////    				    element.GetName(doc).ToString() == "Clash Grid Location" ||
					////    				    element.GetName(doc).ToString() == "Clash Solved" ||
					////    				    element.GetName(doc).ToString() == "Done" ||
					////    				    element.GetName(doc).ToString() == "ID Element" ||
					////    				    element.GetName(doc).ToString() == "Percent Done" ||
					////    				    element.GetName(doc).ToString() == "Zone")
					//    				{
					//    					clashSchedule.Definition.AddField(element);
					//    					msg += element.GetName(doc).ToString() + Environment.NewLine;
					//    				}
					//    				if (element.GetName(doc).ToString() == "Clash") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(0);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Clash Category") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(1);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(1);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Clash Comments") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(2);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(2);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Clash Grid Location") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(3);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(3);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Clash Solved") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(4);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(4);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Done") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(5);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(5);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "ID Element") 
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(6);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(6);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Percent Done")
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(7);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(7);
					//    					clashId.Add(fielId);
					//    				}
					//    				if (element.GetName(doc).ToString() == "Zone")
					//    				{
					//    					ScheduleField field = clashSchedule.Definition.GetField(8);
					//    					ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(8);
					//    					clashId.Add(fielId);
					//    				}
					//
					//    			
					//    			}

					//	   			ScheduleField foundField_clash = clashSchedule.Definition.GetField(clashId.FirstOrDefault());
					////	   			ScheduleField foundField_clashcategory = clashSchedule.Definition.GetField(clashId.ElementAt(1));
					////	   			ScheduleField foundField_clashcomments = clashSchedule.Definition.GetField(clashId.ElementAt(2));
					////	   			ScheduleField foundField_clashlocation = clashSchedule.Definition.GetField(clashId.ElementAt(3));
					////	   			ScheduleField foundField_clashsolved = clashSchedule.Definition.GetField(clashId.ElementAt(4));
					////	   			ScheduleField foundField_clashdone = clashSchedule.Definition.GetField(clashId.ElementAt(5));
					////	   			ScheduleField foundField_clashID = clashSchedule.Definition.GetField(clashId.ElementAt(6));
					////	   			ScheduleField foundField_clashpercent = clashSchedule.Definition.GetField(clashId.ElementAt(7));
					////	   			ScheduleField foundField_clashzone = clashSchedule.Definition.GetField(clashId.ElementAt(8));
					//	   			
					//	   			TaskDialog.Show("ffdfdf", msg + Environment.NewLine + foundField_clash.GetSchedulableField().GetName(doc).ToString());
					////	   															+ foundField_clashcategory.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashcomments.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashlocation.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashsolved.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashdone.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashID.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashpercent.GetSchedulableField().GetName(doc).ToString()
					////												   				+ foundField_clashzone.GetSchedulableField().GetName(doc).ToString());
					//	   			
					//    			AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
					//			    AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));
					//			    
					//				if (null != clashSchedule)
					//    			{
					//        			transaction.Commit();
					//    			}
					//    			else
					//    			{
					//        		transaction.RollBack();
					//    			}

					//    			using (Transaction t = new Transaction(doc, "Add filter"))
					//			    {
					//			    	t.Start();
					//			    	//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
					//			    	ScheduleFilter filter = new ScheduleFilter(foundField_clash.FieldId, ScheduleFilterType.Equal, "YES");
					//			    	clashSchedule.Definition.AddFilter(filter);
					//			    	t.Commit();
					//			    }
					//    			using (Transaction tran = new Transaction(doc, "Cambiar nombre"))
					//    			{
					//    				tran.Start();
					//					TableData td = clashSchedule.GetTableData(); // get viewschedule table data
					//					TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
					//					string text = tsd.GetCellText(0, 0);
					//					tsd.SetCellText(0, 0, "CLASH " +  "DUCTS" + " SCHEDULE");
					//					//
					//					// insert columns
					//					tsd.InsertColumn(0);
					//					tran.Commit();
					//    			}
				}
			}

			void CreateClashSchedule_2_cyp()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ViewSchedule clashSchedule = null;
				List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();
				using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
				{
					transaction.Start();

					FilteredElementCollector collector = new FilteredElementCollector(doc);
					collector.OfCategory(BuiltInCategory.OST_DuctCurves);
					//Get first ElementId of AreaScheme.
					ElementId areaSchemeId = collector.FirstElementId();
					if (areaSchemeId != null && areaSchemeId != ElementId.InvalidElementId)
					{
						// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
						// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
						// and the value of third argument must be ElementId of an AreaScheme.
						clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.OST_DuctCurves));

						doc.Regenerate();

						//Add fields

					}
					ScheduleDefinition definition = clashSchedule.Definition;

					IList<SchedulableField> schedulableFields = definition.GetSchedulableFields();

					string msg = "";

					List<SchedulableField> listashparam = new List<SchedulableField>();


					foreach (SchedulableField element in schedulableFields)
					{
						if (element.ParameterId.IntegerValue > 0)
						{
							listashparam.Add(element);
						}


						bool fieldAlreadyAdded = false;
						IList<ScheduleFieldId> ids = clashSchedule.Definition.GetFieldOrder();
						foreach (ScheduleFieldId id in ids)
						{
							if (definition.GetField(id).GetSchedulableField() == element)
							{
								fieldAlreadyAdded = true;
								break;
							}
						}
						if (fieldAlreadyAdded == false && element.ParameterId.IntegerValue > 0)
						{
							if (element.GetName(doc).ToString() == "Clash" ||
							element.GetName(doc).ToString() == "Clash Category" ||
							element.GetName(doc).ToString() == "Clash Comments" ||
							element.GetName(doc).ToString() == "Clash Grid Location" ||
							element.GetName(doc).ToString() == "Clash Solved" ||
							element.GetName(doc).ToString() == "Done" ||
							element.GetName(doc).ToString() == "ID Element" ||
							element.GetName(doc).ToString() == "Percent Done" ||
							element.GetName(doc).ToString() == "Zone")
							{
								clashSchedule.Definition.AddField(element);
								msg += element.GetName(doc).ToString() + Environment.NewLine;
							}
							if (element.GetName(doc).ToString() == "Clash")
							{
								ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId);
							}

						}



					}

					ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());
					TaskDialog.Show("Creating CLASH Schedule", msg + Environment.NewLine + foundField.GetSchedulableField().GetName(doc).ToString());
					AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
					AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));

					if (null != clashSchedule)
					{
						transaction.Commit();
					}
					else
					{
						transaction.RollBack();
					}
				}
				using (Transaction t = new Transaction(doc, "Add filter"))
				{
					t.Start();
					//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
					ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());
					ScheduleFilter filter = new ScheduleFilter(foundField.FieldId, ScheduleFilterType.Equal, "YES");
					clashSchedule.Definition.AddFilter(filter);
					t.Commit();
				}
				using (Transaction tran = new Transaction(doc, "Cambiar Head nombre"))
				{
					tran.Start();
					TableData td = clashSchedule.GetTableData(); // get viewschedule table data
					TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
					string text = tsd.GetCellText(0, 0);
					tsd.SetCellText(0, 0, "CLASH " + "DUCTS" + " SCHEDULE");
					//
					// insert columns
					tsd.InsertColumn(0);
					tran.Commit();
				}
			}

			void CreateClashSchedule_AllCategories_cyp()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				//List<Category> categories = ObtenerCategorias(doc);

				BuiltInCategory[] bics = new BuiltInCategory[]
				{
				BuiltInCategory.OST_CableTray,
				BuiltInCategory.OST_CableTrayFitting,
				BuiltInCategory.OST_Conduit,
				BuiltInCategory.OST_ConduitFitting,
				BuiltInCategory.OST_DuctCurves,
				BuiltInCategory.OST_DuctFitting,
				BuiltInCategory.OST_DuctTerminal,
				//BuiltInCategory.OST_ElectricalEquipment,
				BuiltInCategory.OST_ElectricalFixtures,
				BuiltInCategory.OST_LightingDevices,
				BuiltInCategory.OST_LightingFixtures,
				BuiltInCategory.OST_MechanicalEquipment,
				BuiltInCategory.OST_PipeCurves,
				BuiltInCategory.OST_PipeFitting,
				BuiltInCategory.OST_PlumbingFixtures,
				//BuiltInCategory.OST_SpecialityEquipment,
				BuiltInCategory.OST_Sprinklers
					//BuiltInCategory.OST_Wire,
				};

				string msg = "";
				string msg3 = "";

				foreach (BuiltInCategory bic in bics)
				{
					ViewSchedule clashSchedule = null;
					using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
					{
						transaction.Start();

						//FilteredElementCollector collector = new FilteredElementCollector(doc);
						//collector.OfCategory(bic);
						//Get first ElementId of AreaScheme.
						//ElementId elemId = collector.FirstElementId();
						//if (elemId != null && elemId != ElementId.InvalidElementId)
						//{
						// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
						// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
						// and the value of third argument must be ElementId of an AreaScheme.
						clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(bic));

						doc.Regenerate();

						//Add fields

						//}
						ScheduleDefinition definition = clashSchedule.Definition;

						IList<SchedulableField> schedulableFields = definition.GetSchedulableFields(); // [a,b,c,s,d,f,....]


						//List<SchedulableField> listashparam = [];
						//SchedulableField[] listashparam = {};
						List<SchedulableField> listashparam = new List<SchedulableField>();
						//List<ScheduleFieldId> clashId = [];
						//ScheduleFieldId[] clashId = {};
						List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();

						foreach (SchedulableField element in schedulableFields)
						{
							if (element.ParameterId.IntegerValue > 0)
							{
								listashparam.Add(element);
							}
						}


						double nro_items_listahpram = listashparam.Count();

						for (int i = 0; i < 1; i++)
						{
							if (listashparam[i].GetName(doc).ToString() == "Clash")
							{
								clashSchedule.Definition.AddField(listashparam[i]);
								ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId);
							}
							if (listashparam[i + 1].GetName(doc).ToString() == "Clash Category")
							{
								clashSchedule.Definition.AddField(listashparam[i + 1]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId2 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId2);
							}
							if (listashparam[i + 2].GetName(doc).ToString() == "Clash Comments")
							{
								clashSchedule.Definition.AddField(listashparam[i + 2]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId3);
							}
							if (listashparam[i + 3].GetName(doc).ToString() == "Clash Grid Location")
							{
								clashSchedule.Definition.AddField(listashparam[i + 3]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId3 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId3);
							}
							if (listashparam[i + 4].GetName(doc).ToString() == "Clash Solved")
							{
								clashSchedule.Definition.AddField(listashparam[i + 4]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId4 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId4);
							}
							if (listashparam[i + 5].GetName(doc).ToString() == "Done")
							{
								clashSchedule.Definition.AddField(listashparam[i + 5]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId5 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId5);
							}
							if (listashparam[i + 6].GetName(doc).ToString() == "ID Element")
							{
								clashSchedule.Definition.AddField(listashparam[i + 6]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId6 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId6);
							}
							if (listashparam[i + 7].GetName(doc).ToString() == "Percent Done")
							{
								clashSchedule.Definition.AddField(listashparam[i + 7]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId7 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId7);
							}
							if (listashparam[i + 8].GetName(doc).ToString() == "Zone")
							{
								clashSchedule.Definition.AddField(listashparam[i + 8]);
								//ScheduleField field = clashSchedule.Definition.GetField(0);
								ScheduleFieldId fielId8 = clashSchedule.Definition.GetFieldId(0);
								clashId.Add(fielId8);
							}
							msg3 = msg3 + listashparam[i].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 1].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 2].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 3].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 4].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 5].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 6].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 7].GetName(doc).ToString() + Environment.NewLine
								+ listashparam[i + 8].GetName(doc).ToString() + Environment.NewLine;
						}


						ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());

						//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
						//AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));

						if (null != clashSchedule)
						{
							transaction.Commit();
						}
						else
						{
							transaction.RollBack();
						}

						using (Transaction t = new Transaction(doc, "Add filter"))
						{
							t.Start();
							//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
							ScheduleFilter filter = new ScheduleFilter(foundField.FieldId, ScheduleFilterType.Equal, "YES");
							clashSchedule.Definition.AddFilter(filter);
							t.Commit();
						}
						using (Transaction tran = new Transaction(doc, "Cambiar nombre"))
						{
							tran.Start();
							TableData td = clashSchedule.GetTableData(); // get viewschedule table data
							TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
							string text = tsd.GetCellText(0, 0);
							tsd.SetCellText(0, 0, "CLASH " + bic.ToString() + " SCHEDULE");
							//
							// insert columns
							tsd.InsertColumn(0);
							tran.Commit();
						}
					}
				}
				TaskDialog.Show("Creating CLASH Schedule", msg + msg3 + Environment.NewLine);
				// bic in bics

			}

			void CreateClashSchedule_cyp()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				View activeView = uidoc.ActiveView;

				ViewSchedule clashSchedule = null;

				using (Transaction transaction = new Transaction(doc, "Creating CLASH Schedule"))
				{
					transaction.Start();

					FilteredElementCollector collector = new FilteredElementCollector(doc);
					collector.OfCategory(BuiltInCategory.OST_DuctCurves);
					//Get first ElementId of AreaScheme.
					ElementId areaSchemeId = collector.FirstElementId();
					if (areaSchemeId != null && areaSchemeId != ElementId.InvalidElementId)
					{
						// If you want to create an area schedule, you must use CreateSchedule method with three arguments. 
						// The value of the second argument must be ElementId of BuiltInCategory.OST_Areas category
						// and the value of third argument must be ElementId of an AreaScheme.
						clashSchedule = Autodesk.Revit.DB.ViewSchedule.CreateSchedule(doc, new ElementId(BuiltInCategory.OST_DuctCurves));

						doc.Regenerate();

						//Add fields

					}
					ScheduleDefinition definition = clashSchedule.Definition;

					IList<SchedulableField> schedulableFields = definition.GetSchedulableFields();

					string msg = "";

					List<SchedulableField> listashparam = new List<SchedulableField>();
					List<ScheduleFieldId> clashId = new List<ScheduleFieldId>();

					foreach (SchedulableField element in schedulableFields)
					{

						if (element.ParameterId.IntegerValue > 0)
						{
							listashparam.Add(element);
						}

						if (element.GetName(doc).ToString() == "Clash" ||
							element.GetName(doc).ToString() == "Clash Category" ||
							element.GetName(doc).ToString() == "Clash Comments" ||
							element.GetName(doc).ToString() == "Clash Grid Location" ||
							element.GetName(doc).ToString() == "Clash Solved" ||
							element.GetName(doc).ToString() == "Done" ||
							element.GetName(doc).ToString() == "ID Element" ||
							element.GetName(doc).ToString() == "Percent Done" ||
							element.GetName(doc).ToString() == "Zone")
						{
							clashSchedule.Definition.AddField(element);
							msg += element.GetName(doc).ToString() + Environment.NewLine;
						}
						if (element.GetName(doc).ToString() == "Clash")
						{
							ScheduleField field = clashSchedule.Definition.GetField(0);
							ScheduleFieldId fielId = clashSchedule.Definition.GetFieldId(0);
							clashId.Add(fielId);
						}

					}

					ScheduleField foundField = clashSchedule.Definition.GetField(clashId.FirstOrDefault());
					TaskDialog.Show("ffdfdf", msg + Environment.NewLine + foundField.GetSchedulableField().GetName(doc).ToString());
					AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.RBS_CALCULATED_SIZE));
					AddRegularFieldToSchedule(clashSchedule, new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH));

					if (null != clashSchedule)
					{
						transaction.Commit();
					}
					else
					{
						transaction.RollBack();
					}

					using (Transaction t = new Transaction(doc, "Add filter"))
					{
						t.Start();
						//foundField = clashSchedule.Definition.AddField(ScheduleFieldType.Instance, foundField.ParameterId);
						ScheduleFilter filter = new ScheduleFilter(foundField.FieldId, ScheduleFilterType.Equal, "YES");
						clashSchedule.Definition.AddFilter(filter);
						t.Commit();
					}
					using (Transaction tran = new Transaction(doc, "Cambiar nombre"))
					{
						tran.Start();
						TableData td = clashSchedule.GetTableData(); // get viewschedule table data
						TableSectionData tsd = td.GetSectionData(SectionType.Header); // get header section data
						string text = tsd.GetCellText(0, 0);
						tsd.SetCellText(0, 0, "CLASH " + "DUCTS" + " SCHEDULE");
						//
						// insert columns
						tsd.InsertColumn(0);
						tran.Commit();
					}
				}
			}
			#endregion

			#region LIBRARIES SUPPORT
			/// <summary> Obtiene una Lista de Categorias existentes en el Modelo </summary>
			void ObtenerCategoriasModelo()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;

				FilteredElementCollector colector = new FilteredElementCollector(doc);
				List<Element> elementos = colector.WhereElementIsNotElementType().ToList();
				List<Element> filtrados = (from elem in elementos
										   where elem.Category != null
										   && elem.Category.CategoryType == CategoryType.Model
										   select elem).ToList();
				string mensaje = "";
				// Lista vacias de categorias
				List<Category> categorias = new List<Category>();
				// Recorrer los elementos filtrados
				foreach (Element elem in filtrados)
				{
					// Validar que la categoria NO exista en la lista
					if (!categorias.Exists(x => x.Id == elem.Category.Id))
					{
						categorias.Add(elem.Category);
						mensaje = mensaje + elem.Category.Name.ToString() + " : " + Environment.NewLine;
					}
				}
				TaskDialog.Show("out", mensaje);
			}

			List<Category> ObtenerCategorias(Document _doc)
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				////Document doc = this.ActiveUIDocument.Document;

				////UIApplication uiapp = this.Application;
				//Application app = this.Application;

				FilteredElementCollector colector = new FilteredElementCollector(_doc);
				List<Element> elementos = colector.WhereElementIsNotElementType().ToList();
				List<Element> filtrados = (from elem in elementos
										   where elem.Category != null
										   && elem.Category.CategoryType == CategoryType.Model
										   select elem).ToList();
				string mensaje = "";
				// Lista vacias de categorias
				List<Category> categorias = new List<Category>();
				// Recorrer los elementos filtrados
				foreach (Element elem in filtrados)
				{
					// Validar que la categoria NO exista en la lista
					if (!categorias.Exists(x => x.Id == elem.Category.Id))
					{
						categorias.Add(elem.Category);
						mensaje = mensaje + elem.Category.Name.ToString() + " : " + Environment.NewLine;
					}
				}
				TaskDialog.Show("out", mensaje);
				return categorias;
			}

			void builtInParamsForElement()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = uidoc.Document;

				Element e = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element));
				string data = "";
				foreach (BuiltInParameter bip in Enum.GetValues(typeof(BuiltInParameter)))
				{
					try
					{
						Parameter p = e.get_Parameter(bip);
						data += bip.ToString() + ": " + p.Definition.Name + ": ";
						if (p.StorageType == StorageType.String)
							data += p.AsString();
						else if (p.StorageType == StorageType.Integer)
							data += p.AsInteger();
						else if (p.StorageType == StorageType.Double)
							data += p.AsDouble();
						else if (p.StorageType == StorageType.ElementId)
							data += "ID " + p.AsElementId().IntegerValue;
						data += "\n";
					}
					catch
					{
					}
				}
				TaskDialog.Show("BI Params", data);
			}

			void keynotesArea()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;

				List<Element> elementss = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToList();
				List<Tuple<ElementId, double, string>> results = new List<Tuple<ElementId, double, string>>();


				foreach (Element e in elementss)
				{
					Parameter areaParam = e.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
					if (areaParam == null)
						continue;

					Tuple<ElementId, double, string> thisElementData = new Tuple<ElementId, double, string>(e.Id, 0, "");

					results.Add(thisElementData);

				}

			}

			void GetAllElementsModel()
			{
				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				FilteredElementCollector coll = new FilteredElementCollector(doc); ;

				IList<Element> elementsCollection = coll.WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(false), new ElementIsElementTypeFilter(true))).ToElements();
				string mensaje = "";

				IList<Element> modelElements = new List<Element>();

				foreach (Element e in elementsCollection)
				{

					if ((null != e.Category)
				 && (null != e.LevelId)
				 && (null != e.get_Geometry(new Options())))
					{
						modelElements.Add(e);
					}
				}
				foreach (Element elem in modelElements)
				{
					mensaje = mensaje + elem.Name.ToString() + " : " + Environment.NewLine;
				}
				TaskDialog.Show("out", mensaje);
				//return modelElements;
			}

			void GetViewProperties()
			{
				//Get document and current view
				//Document doc = this.ActiveUIDocument.Document;
				View currentView = uidoc.ActiveView;

				//Find the view family type that matches the active view
				var VfamType = (ViewFamilyType)doc.GetElement(currentView.GetTypeId());
				//Find the level that matches the active view
				Level lev = currentView.GenLevel;
				//Get the view's current name
				string viewname = currentView.Name;
				//Get the name of the view family type
				string typename = VfamType.Name;
				//Get the name of the level
				string levelname = lev.Name;
				//Combine results for task dialog
				string Output = "View: " + viewname + "\n" + typename + "-" + levelname;
				//Show results
				TaskDialog.Show("View Properties Test", Output);
			}

			Dictionary<string, XYZ> getIntersections(Document _doc)
			{

				string coordinate = null;
				string coordA = null;
				string coordB = null;
				string coordC = null;
				Dictionary<string, XYZ> intersectionPoints = new Dictionary<string, XYZ>();


				ICollection<ElementId> grids = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Grids)).WhereElementIsNotElementType().ToElementIds();

				ICollection<ElementId> refgrid = new FilteredElementCollector(doc).WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Grids)).WhereElementIsNotElementType().ToElementIds();
				foreach (ElementId eid in grids)

				{
					Grid g = _doc.GetElement(eid) as Grid;

					coordA = g.Name;

					Curve c = g.Curve;

					refgrid.Remove(eid);

					foreach (ElementId eid2 in refgrid)
					{
						Grid g2 = _doc.GetElement(eid2) as Grid;

						coordB = g2.Name;
						Curve c2 = g2.Curve;

						IntersectionResultArray results;

						SetComparisonResult result = c.Intersect(c2, out results);

						if (result != SetComparisonResult.Overlap)
						{ }

						else if (results == null || results.Size != 1)

						{ }

						else if (results.Size > 0)
						{
							for (int i = 0; i < results.Size; i++)
							{
								IntersectionResult iresult = results.get_Item(i);
								coordC = i.ToString();

								coordinate = coordA + " / " + coordB + " - " + coordC;

								XYZ point = iresult.XYZPoint;

								intersectionPoints.Add(coordinate, point);

							}
						}
						continue;
					}

				}
				return intersectionPoints;
			}

			FilteredElementCollector GetConnectorElements(Document _doc, bool include_wires)
			{
				BuiltInCategory[] bics = new BuiltInCategory[]
				{
					BuiltInCategory.OST_CableTray,
					BuiltInCategory.OST_CableTrayFitting,
					BuiltInCategory.OST_Conduit,
					BuiltInCategory.OST_ConduitFitting,
					BuiltInCategory.OST_DuctCurves,
					BuiltInCategory.OST_DuctFitting,
					BuiltInCategory.OST_DuctTerminal,
					BuiltInCategory.OST_ElectricalEquipment,
					BuiltInCategory.OST_ElectricalFixtures,
					BuiltInCategory.OST_LightingDevices,
					BuiltInCategory.OST_LightingFixtures,
					BuiltInCategory.OST_MechanicalEquipment,
					BuiltInCategory.OST_PipeCurves,
					BuiltInCategory.OST_PipeFitting,
					BuiltInCategory.OST_PlumbingFixtures,
					BuiltInCategory.OST_SpecialityEquipment,
					BuiltInCategory.OST_Sprinklers,
					BuiltInCategory.OST_Wire,
				};

				IList<ElementFilter> a = new List<ElementFilter>(bics.Count());

				foreach (BuiltInCategory bic in bics)
				{
					a.Add(new ElementCategoryFilter(bic));
				}

				LogicalOrFilter categoryFilter = new LogicalOrFilter(a);

				LogicalAndFilter familyInstanceFilter = new LogicalAndFilter(categoryFilter, new ElementClassFilter(typeof(FamilyInstance)));

				IList<ElementFilter> b = new List<ElementFilter>(6);

				b.Add(new ElementClassFilter(typeof(CableTray)));
				b.Add(new ElementClassFilter(typeof(Conduit)));
				b.Add(new ElementClassFilter(typeof(Duct)));
				b.Add(new ElementClassFilter(typeof(Pipe)));

				if (include_wires)
				{
					b.Add(new ElementClassFilter(typeof(Wire)));
				}

				b.Add(familyInstanceFilter);

				LogicalOrFilter classFilter = new LogicalOrFilter(b);

				FilteredElementCollector collector = new FilteredElementCollector(_doc);

				collector.WherePasses(classFilter);

				return collector;
			}

			void CreateSingleCategorySchedule(Document _doc)
			{
				using (Transaction t = new Transaction(_doc, "Create single-category"))
				{
					t.Start();

					// Create schedule
					ViewSchedule vs = ViewSchedule.CreateSchedule(_doc, new ElementId(BuiltInCategory.OST_Windows));

					doc.Regenerate();

					// Add fields to the schedule
					AddRegularFieldToSchedule(vs, new ElementId(BuiltInParameter.WINDOW_HEIGHT));
					AddRegularFieldToSchedule(vs, new ElementId(BuiltInParameter.WINDOW_WIDTH));

					t.Commit();
				}
			}

			void AddRegularFieldToSchedule(ViewSchedule schedule, ElementId paramId)
			{
				ScheduleDefinition definition = schedule.Definition;

				// Find a matching SchedulableField
				SchedulableField schedulableField =
				definition.GetSchedulableFields().FirstOrDefault(sf => sf.ParameterId == paramId);

				if (schedulableField != null)
				{
					// Add the found field
					definition.AddField(schedulableField);
				}
			}

			void create3DView()
			{
				//Document doc = this.ActiveUIDocument.Document;

				// get a ViewFamilyType for a 3D View
				ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
							 OfClass(typeof(ViewFamilyType)).
							 Cast<ViewFamilyType>()
												 where v.ViewFamily == ViewFamily.ThreeDimensional
												 select v).First();

				using (Transaction t = new Transaction(doc, "Create view"))
				{
					t.Start();
					View3D view = View3D.CreateIsometric(doc, viewFamilyType.Id);
					t.Commit();
				}
			}

			void create3DViewsWithSectionBox()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				// get list of all levels
				IList<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().OrderBy(l => l.Elevation).ToList();

				// get a ViewFamilyType for a 3D View
				ViewFamilyType viewFamilyType = (from v in new FilteredElementCollector(doc).
												 OfClass(typeof(ViewFamilyType)).
												 Cast<ViewFamilyType>()
												 where v.ViewFamily == ViewFamily.ThreeDimensional
												 select v).First();

				using (Transaction t = new Transaction(doc, "Create view"))
				{
					int ctr = 0;
					// loop through all levels
					foreach (Level level in levels)
					{
						t.Start();

						// Create the 3d view
						View3D view = View3D.CreateIsometric(doc, viewFamilyType.Id);

						// Set the name of the view
						view.Name = level.Name + " Section Box";

						// Set the name of the transaction
						// A transaction can be renamed after it has been started
						t.SetName("Create view " + view.Name);

						// Create a new BoundingBoxXYZ to define a 3D rectangular space
						BoundingBoxXYZ boundingBoxXYZ = new BoundingBoxXYZ();

						// Set the lower left bottom corner of the box
						// Use the Z of the current level.
						// X & Y values have been hardcoded based on this RVT geometry
						boundingBoxXYZ.Min = new XYZ(-50, -100, level.Elevation);

						// Determine the height of the bounding box
						double zOffset = 0;
						// If there is another level above this one, use the elevation of that level
						if (levels.Count > ctr + 1)
							zOffset = levels.ElementAt(ctr + 1).Elevation;
						// If this is the top level, use an offset of 10 feet
						else
							zOffset = level.Elevation + 10;
						boundingBoxXYZ.Max = new XYZ(200, 125, zOffset);

						// Apply this bouding box to the view's section box
						//view.SectionBox = boundingBoxXYZ;
						view.SetSectionBox(boundingBoxXYZ);

						t.Commit();

						// Open the just-created view
						// There cannot be an open transaction when the active view is set
						uidoc.ActiveView = view;

						ctr++;
					}
				}
			}
			#endregion


			#endregion
			// Terminar de Escribir el código acá

			return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }
    }
}
