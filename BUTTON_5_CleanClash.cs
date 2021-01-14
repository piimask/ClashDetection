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
    public class BUTTON_5_CleanClash : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects
            UIApplication uiApp = commandData.Application;
			UIDocument uidoc = uiApp.ActiveUIDocument;
			Document doc = uiApp.ActiveUIDocument.Document;
			Application app = uiApp.Application;

			// Get Active View
			View activeView = uidoc.ActiveView;

			//View activeView = doc.ActiveView;
			string ruta = App.ExecutingAssemblyPath;

			// Empezar a Escribir el código acá
			#region MACRO CODE


			#region BUTTONS 
			void BUTTON_1_CreateClashParameters()
			{

				try
				{
					#region try region
					//UIDocument uidoc = this.ActiveUIDocument; // documento activo
					//Document doc = this.ActiveUIDocument.Document; // documento
					//Application app = this.Application; // archivo aplication

					List<DefinitionGroup> defGroups = new List<DefinitionGroup>(); // lista vacia

					List<DefinitionGroup> grupos = new List<DefinitionGroup>(); // lista vacia
					List<DefinitionGroup> grupos_existe = new List<DefinitionGroup>(); // lista vacia

					//				Uri dynoscript = new Uri("https://www.dynoscript.com/quickclash/");
					//				string param1 = HttpUtility.ParseQueryString(dynoscript.Query).Get("param1");

					TaskDialog.Show("Dynoscript", "Recuerda:\n\n1.- Tener los Worksets del proyecto en modo Editables. \n\n2.- Si el Modelo es muy grande algunas tareas pueden demorar varios minutos. "
									+ Environment.NewLine + "\n3.- Si ya probaste Quick Clash por favor déjanos tus comentarios en nuestra página : \n" + Environment.NewLine + new Uri("https://www.dynoscript.com/quickclash/")
									+ " \n\nSe recibe cualquier tipo de FeedBack. Muchas Gracias! :) ", TaskDialogCommonButtons.Ok);
					#region Abrimos el Shared Parameter File actual
					DefinitionFile sharedParameterFile = app.OpenSharedParameterFile(); // Abrimos el archivo .txt de shared parameters

					foreach (DefinitionGroup dg in sharedParameterFile.Groups)
					{
						defGroups.Add(dg);
					}
					#endregion          // defGroups

					for (int i = 0; i < defGroups.Count(); i++) // Para cada Grupo en Shared Parameters de proyecto
					{
						DefinitionGroup dg = defGroups[i]; // 1er grupo de parametro, primero
						if (dg.Name.ToString() == "ClashParameters") // si YA EXISTE el grupo "ClashParameters" usamos ese grupo.
						{
							#region Si ya existe el Grupo Clash Parameters en el Shared Parameter actual
							grupos_existe.Add(dg);
							ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
							ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
							LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
							FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
							IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements(); // recolectamos todos los ductos del proyecto

							Element e = ducts.First(); // primer ducto

							Parameter param = e.LookupParameter("Clash"); // buscamos el parametro "Clash" con el elemento e

							ElementClassFilter elemFilter2 = new ElementClassFilter(typeof(FamilyInstance));
							ElementCategoryFilter DUCategoryfilter2 = new ElementCategoryFilter(BuiltInCategory.OST_DuctFitting);
							LogicalAndFilter DUInstancesFilter2 = new LogicalAndFilter(elemFilter, DUCategoryfilter);
							FilteredElementCollector DUcoll2 = new FilteredElementCollector(doc);
							IList<Element> ductsfit = DUcoll2.WherePasses(DUInstancesFilter).ToElements(); //recolectamos todos los ducts fittings del proyecto

							Element e2 = ductsfit.First(); // primer duct fitting

							Parameter param2 = e2.LookupParameter("Clash"); // buscamos el parametro "clash" con el elemento e2 = ducto fitting o fammily instance
							string aplicar = "siaplicar";
							if (null == param) // si no existe el parametro "Clash" entonces tenemos que:
							{
								// crear los parametros obteniendo informacion del archivo de texto: Shared Parameters existente


								//							TaskDialog.Show("MENSAJE DE ALERTA", "Si te aparece este error: \n\n" 
								//							                +" 1.- Borrar el Grupo 'ClashParameters' del archivo de parametros compartidos. Junto con el grupo borra todos los Parametros que este grupo contiene."
								//							                +"\n2.- Despúes de borrar, finalmente borra los Project Parameters que se hayan creado y vuelve a ejecutar, esta vez sí funcionará creando todos los CLASH Parameters necesarios. Suerte!"
								//							                + Environment.NewLine
								//							               	+ "\nPara más información sobre cómo hacer esto visita: wwww.dynoscript.com");

								// crear parametros obteniendo informacion del archivo de texto: Shared Parameters
								DYNO_CreateClashParameters_ModelElements_SharedParameterExisting();
								DYNO_SetEmptyYesNoParameters();
								DYNO_SetIDValue();
								//DYNO_SetNoValueAllParameters_doc();  // Coloca valor vacio a todos los parametros nuevos.
								DYNO_CreateClashSchedules(); // crea todas las CLASH Schedules para todas las categorias
															 //DYNO_CreateClashFilterMultipleElementsInView(); // Crea y coloca los filtros YES CLASH y NO CLASH a la vista FILTER VIEW

							}
							else // SI existe todos los parametros y el parametro "Clash"
							{
								//TaskDialog.Show("Mensaje", "Los Clash Parameters ya existen! Todos sus valores volverán a su valor por defecto.",TaskDialogCommonButtons.Yes);
								//forms.MessageBox.Show("Mensaje", "Los Clash Parameters ya existen! Todos sus valores volverán a su valor por defecto.", forms.MessageBoxButtons.YesNo);
								if (forms.MessageBox.Show("Los Clash Parameters ya existen! Todos sus valores volverán a su valor por defecto." + Environment.NewLine + " \n¿Desea continuar?\n", "Dynoscript", forms.MessageBoxButtons.YesNo) == forms.DialogResult.Yes)
								{
									DYNO_SetEmptyYesNoParameters();
									DYNO_SetIDValue();
									DYNO_SetNoValueAllParameters_doc();  // Coloca valor vacio a todos los parametros nuevos.
																		 //TaskDialog.Show("fdfdd", "funciono mierda");
									aplicar = "noaplicar";
									continue;
								}
								else
								{
									//						        MessageBox.Show("The application has been closed successfully.", "Application Closed!", MessageBoxButtons.OK);

									forms.Application.Exit();
									break;
									//return;

									//						    	this.Activate();
								}
							}


							//CreateClashParameters_Family();
							//CreateClashParameters_Elements();
							//CreateClashParameters_FamilyInstance();


							//DYNO_CreateClashSchedules();
							//DYNO_CreateClashFilterMultipleElementsInView();

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
									views.Add(ve); // lista con todos los View3d del proyecto
								}
								for (int n = 0; n < views.Count(); n++)
								{
									View3D ve = views[n];
									if (ve.Name.Contains("COORD"))
									{
										views_COORD.Add(ve); // todas la vistas3d con nombre que contiene "COORD"

										//numero = numero + 1;
									}

								}

								if (views_COORD.Count() == 0)
								{
									COORD.Name = "COORD";
								}
								else
								{
									for (int m = 0; m < views_COORD.Count(); m++)
									{
										View3D ve = views_COORD[m];
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
							if (aplicar == "siaplicar")
							{
								DYNO_CreateClashFilterMultipleElementsInView(); // Crea y coloca los filtros YES CLASH y NO CLASH a la vista FILTER VIEW
								DYNO_CreateClashSOLVEDFilterMultipleElementsInView(); // Crea y coloca el filtro CLASH SOLVED a la vista FILTER VIEW
							}
							#endregion
						}
						else // si NO EXISTE el grupo "ClashParameters" en shared parameters tenemos que crearlo por PRIMERA VEZ
						{
							grupos.Add(dg);

						}
					}

					//bool isEmpty = !grupos_existe.Any(); 
					bool isEmpty = (grupos_existe.Any() == false); //¡esta vacio? Rpta: TRUE
																   //TaskDialog.Show("GRUPOS EXISTE", isEmpty.ToString());
					if (isEmpty) // si esta vacio TRUE significa que NO EXISTE Group = "ClashParameters" y crearemos todo por PRIMERA VEZ
					{
						#region Cuando no existe el GRupo "ClashParameters" en el Shared Parameter File actual
						ElementClassFilter elemFilter = new ElementClassFilter(typeof(Duct));
						ElementCategoryFilter DUCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_DuctCurves);
						LogicalAndFilter DUInstancesFilter = new LogicalAndFilter(elemFilter, DUCategoryfilter);
						FilteredElementCollector DUcoll = new FilteredElementCollector(doc);
						IList<Element> ducts = DUcoll.WherePasses(DUInstancesFilter).ToElements();

						Element e = ducts.First();

						Parameter param = e.LookupParameter("Clash"); // obtenemos el parametro "Clash"
						if (null == param) // si no existe el parametro "Clash"
						{
							DYNO_CreateClashParameters_ModelElements(); // crea todos los parametros y grupo "clashparameters" por primera vez
							DYNO_SetEmptyYesNoParameters(); // coloca valor vacio a parametros "ClashSolved" y  "Done" a todos los elementos y categorias
							DYNO_SetIDValue(); // coloca valor ID a todos los elementos

							DYNO_CreateClashSchedules(); // crea todas las CLASH Schedules para todas las categorias
							DYNO_CreateClashFilterMultipleElementsInView(); // Crea y coloca los filtros YES CLASH y NO CLASH a la vista FILTER VIEW
							DYNO_CreateClashSOLVEDFilterMultipleElementsInView(); // Crea y coloca el filtro CLASH SOLVED a la vista FILTER VIEW
						}
						else // si el parametro "Clash" si existe 
						{
							DYNO_SetEmptyYesNoParameters(); // coloca valor vacio a parametros "ClashSolved" y  "Done" a todos los elementos y categorias TOdo el documento
							DYNO_SetIDValue();  // coloca valor ID a todos los elementos TOdo el documento
							DYNO_SetNoValueAllParameters_doc();  // Coloca valor vacio a todos los parametros nuevos.
																 //DYNO_CreateClashSchedules(); // crea todas las CLASH Schedules para todas las categorias
																 //DYNO_CreateClashFilterMultipleElementsInView(); // Crea y coloca los filtros YES CLASH y NO CLASH a la vista FILTER VIEW
						}
						#endregion
					}

					else // si esta vacio FALSE significa que SI EXISTE Group = "ClashParameters" y rellenaremos todos los valores de parametros vacios ya creados previamente.
					{
						//DYNO_CreateClashFilterMultipleElementsInView(); // Crea y coloca los filtros YES CLASH y NO CLASH a la vista FILTER VIEW
					}
					#endregion
				}

				catch (Exception ex)
				{


					//				TaskDialog.Show("Error ex", ex.ToString());
					//				throw;
				}
				finally
				{
					//DYNO_CreateClashFilterMultipleElementsInView();
				}

			} // Funciona para todos los casos. Solo crea 1 vez ClashSchedules y ClashViewFilter OK!

			void BUTTON_2_ClashManage() // IntersectCheckedListTest
			{
				try
				{
					DYNO_ClashManage();
				}
				catch (Exception ex)
				{
					//				TaskDialog.Show("Dynoscript Error", "Tienes que primero Crear todos los Clash Parameteres para poder usar este comando!\n\nDa click en el botón 'Start Clash' del Panel Clash Manage"
					//				                +Environment.NewLine+ ex.Message.ToString());
					//				return;
					//				throw;
				}
			} // DYNO_IntersectCheckedListTest OK!

			void BUTTON_3_SectionBox()
			{
				try
				{
					DYNO_SectionBox();
				}
				catch (Exception ex)
				{
					//				TaskDialog.Show("Dynoscript Error", "Tienes que primero Crear todos los Clash Parameteres para poder usar este comando!\n\nDa click en el botón 'Start Clash' del Panel Clash Manage"
					//				                +Environment.NewLine+ ex.Message.ToString());
					//				return;
					//				throw;
				}
			} // Funciona para todos los casos. OK!

			void BUTTON_4_QuickClash() // clash rapido solo vista activa
			{
				try
				{
					List<Element> clash_yes = new List<Element>();

					DYNO_SetNoValueClashParameter(); // Vista Activa , "Clash" y "Clash Grid Location" = " " vacio.
					DYNO_IntersectMultipleElementsToMultipleCategory(); // Vista Activa 
					DYNO_IntersectMultipleElementsToMultipleFamilyInstances();// Vista Activa
					DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox();
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
					//				TaskDialog.Show("Dynoscript Error", "Tienes que primero Crear todos los Clash Parameteres para poder usar este comando!\n\nDa click en el botón 'Start Clash' del Panel Clash Manage"
					//				                +Environment.NewLine+ ex.Message.ToString());
					//				return;
					//				throw;
				}
			} // clash rapido solo vista activa OK!

			//void BUTTON_5_CleanClash() // Vista Activa DYNO_SetNoValueClashParameter
			//{
				try
				{
					DYNO_SetNoValueClashParameter(); // Vista Activa , "Clash" y "Clash Grid Location" = " " vacio.

				}
				catch (Exception ex)
				{
					//				TaskDialog.Show("Dynoscript Error", "Tienes que primero Crear todos los Clash Parameteres para poder usar este comando!\n\nDa click en el botón 'Start Clash' del Panel Clash Manage"
					//				                +Environment.NewLine+ ex.Message.ToString());
					//				return;
					//				throw;
				}
			//} // Vista Activa DYNO_SetNoValueClashParameter OK!

			void BUTTON_6_ClashComments()
			{
				try
				{
					DYNO_ClashComments();
				}
				catch (Exception ex)
				{
					//				TaskDialog.Show("Dynoscript Error", "Tienes que primero Crear todos los Clash Parameteres para poder usar este comando!\n\nDa click en el botón 'Start Clash' del Panel Clash Manage"
					//				                +Environment.NewLine+ ex.Message.ToString());
					//				return;
					//				throw;
				}
			} // Clash Comments OK! Coloca comentario a todos los elementos con Clash de la vista actica.
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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
					DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox_UI(UI_list2, UI_list4);
					//DYNO_IntersectMultipleFamilyInstanceToMultipleCategory_UI(UI_list2, UI_list3);
				}
				else if (checkBox_3s && !checkBox_1s) // TRUE Todo documento
				{
					DYNO_IntersectMultipleElementsToMultipleCategory_UI_doc(UI_list1, UI_list3);
					DYNO_IntersectMultipleElementsToMultipleFamilyInstances_UI_doc(UI_list1, UI_list4);
					DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox_UI_doc(UI_list2, UI_list4);
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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox_UI_doc(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list4_) // Family Instance vs Family Instance
			{

				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				// FAMILY INSTANCES

				// Family Instance
				#region familyinstances BuiltinCategory

				//			BuiltInCategory[] bics_finst = new BuiltInCategory[] 	
				//				{
				//			
				//				    BuiltInCategory.OST_CableTrayFitting,
				//
				//				    BuiltInCategory.OST_ConduitFitting,
				//	
				//				    BuiltInCategory.OST_DuctFitting,
				//				    BuiltInCategory.OST_DuctTerminal,
				//				    BuiltInCategory.OST_ElectricalEquipment,
				//				    BuiltInCategory.OST_ElectricalFixtures,
				//				    BuiltInCategory.OST_LightingDevices,
				//				    BuiltInCategory.OST_LightingFixtures,
				//				    BuiltInCategory.OST_MechanicalEquipment,
				//		
				//				    BuiltInCategory.OST_PipeFitting,
				//				    BuiltInCategory.OST_PlumbingFixtures,
				//				    BuiltInCategory.OST_SpecialityEquipment,
				//				    BuiltInCategory.OST_Sprinklers,
				//		
				//				};
				//			
				//				BuiltInCategory[] bics_finst_2 = new BuiltInCategory[] 	
				//				{
				//			
				//				    BuiltInCategory.OST_CableTrayFitting,
				//
				//				    BuiltInCategory.OST_ConduitFitting,
				//	
				//				    BuiltInCategory.OST_DuctFitting,
				//				    BuiltInCategory.OST_DuctTerminal,
				//				    BuiltInCategory.OST_ElectricalEquipment,
				//				    BuiltInCategory.OST_ElectricalFixtures,
				//				    BuiltInCategory.OST_LightingDevices,
				//				    BuiltInCategory.OST_LightingFixtures,
				//				    BuiltInCategory.OST_MechanicalEquipment,
				//		
				//				    BuiltInCategory.OST_PipeFitting,
				//				    BuiltInCategory.OST_PlumbingFixtures,
				//				    BuiltInCategory.OST_SpecialityEquipment,
				//				    BuiltInCategory.OST_Sprinklers,
				//		
				//				};
				#endregion

				IList<BuiltInCategory> bics_familyIns = UI_list2;
				//			IList<BuiltInCategory> bics_familyIns = bics_finst;

				IList<BuiltInCategory> bics_finst_4 = UI_list4;

				List<Element> clash_familyinstance = new List<Element>();



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

						clash_familyinstance.Add(elem);
					}

				}
				List<Element> clash_yesA = new List<Element>();

				for (int i = 0; i < clash_familyinstance.Count(); i++)
				{
					Element elem = clash_familyinstance[i];

					BoundingBoxXYZ elem_bb = elem.get_BoundingBox(null);

					IList<Element> salida = new List<Element>();


					foreach (BuiltInCategory bic in bics_finst_4)
					{

						Outline outline_2 = new Outline(elem_bb.Min, elem_bb.Max);
						BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);

						ElementClassFilter famFil = new ElementClassFilter(typeof(FamilyInstance));

						ElementCategoryFilter Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter InstancesFilter = new LogicalAndFilter(famFil, Categoryfilter);

						FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc); // 2 muros que intercepta con la ventana siguiente

						IList<Element> elementss = coll_outline_2.WherePasses(bbfilter_2).WherePasses(InstancesFilter).ToElements();

						//					FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
						//					IList<Element> elements = coll_outline_2.WherePasses(bbfilter_2).OfClass(typeof(FamilyInstance)).ToElements();


						//
						//					
						//					foreach (Element n in elements) 
						//					{
						//						salida.Add(n);
						//					}

						if (elementss.Count() > 0) // clash
						{

							if (!clash_yesA.Contains(elem))
							{


								foreach (Element pp in elementss)
								{
									if (!(pp.Id == elem.Id))
									{

										clash_yesA.Add(elem);

										Parameter param = elem.LookupParameter("Clash Category");
										Parameter paramID = elem.LookupParameter("ID Element");
										string elemcategory = elementss.First().Category.Name.ToString() + " / ID: " + elementss.First().Id.ToString();

										using (Transaction t = new Transaction(doc, "Clash Category"))
										{
											t.Start();

											param.Set(elemcategory);

											t.Commit();
										}
									}
								}

							}

						}


					}




				}



				#region borrador

				//	 		string numero_ductos = clashID_familyinstance.Count().ToString();
				//	 		string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;
				//	 		
				//	 		
				//			foreach (ElementId eID in clashID_familyinstance) 
				//	 		{
				//	 
				//	 			//ElementId eID = e.Id;
				//	 			Element e = doc.GetElement(eID);
				//				//LocationPoint p = e.Location as LocationPoint;
				//	 			
				//				//XYZ xyz = new XYZ(p.Point.X, p.Point.Y,p.Point.Z);
				//				
				//				//XYZ p = so.ComputeCentroid();
				//				//FamilyInstance efi = e as FamilyInstance;
				//				
				//				GeometryElement geomElement = e.get_Geometry(new Options() );
				//				
				//				GeometryInstance geomFinstace = geomElement.First() as GeometryInstance;
				//
				//				GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();
				//				
				//				//Solid solid = gSymbol.First() as Solid;
				//	 
				//	 			Solid solid = null;
				//	 			foreach(GeometryObject geomObj in gSymbol)
				//	 			{
				//	 				solid = geomObj as Solid;
				//	 				if( solid != null ) break;
				//	 			}
				//
				////	 			IList<BuiltInCategory> bics_fi2 = UI_list4;
				//	 			IList<BuiltInCategory> bics_fi2 = bics_finst;
				//	 			
				//	 			
				//	 			foreach (BuiltInCategory bic in bics_fi2) 
				//	 			{
				//					// Find intersections between family instances and a selected element
				//		 			// category Mechanical Euqipment
				//		 			ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 			// Create a category filter for Mechanical Euqipment
				//		 			ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);
				//		 
				//		 			LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
				//		 
				//		 			ICollection<ElementId> collectoreID = new List<ElementId>();
				//		 			if (collectoreID.Contains(e.Id)==false)
				//		 			{
				//						collectoreID.Add(eID);
				//					}
				////		 			
				////		 
				//		 			ExclusionFilter filter = new ExclusionFilter(collectoreID);
				//					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
				//		 			
				//					if (bic == BuiltInCategory.OST_CableTray) 
				//		 			{
				//						collector.OfClass(typeof(CableTray));
				//					}
				//		 			if (bic == BuiltInCategory.OST_DuctCurves) 
				//		 			{
				//						collector.OfClass(typeof(Duct));
				//					}
				//		 			if (bic == BuiltInCategory.OST_PipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(Pipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_Conduit) 
				//		 			{
				//						collector.OfClass(typeof(Conduit));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//
				//		 			collector.WherePasses(DU2InstancesFilter);
				//		 			collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
				//		 			collector.WherePasses(filter);
				//		 			
				//		 			if (collector.Count() > 0) 
				//			 		{
				//		 				if (clash_yesA.Contains(e)==false)
				//			 			{
				//							clash_yesA.Add(e);
				//						}
				//			        }
				//			 		
				//			 		foreach (Element elem in collector) 
				//			 		{
				//		 				if (clash_yesA.Contains(elem)==false)
				//			 			{
				//							clash_yesA.Add(elem);
				//						}
				//					}
				//		 			mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
				//			 																	+ e.Name.ToString() 
				//			 																	+ e.Category.Name.ToString() + " id:"
				//																 				+ eID.ToString() + ")"
				//																				+ Environment.NewLine;
				//				}
				//	 		}
				#endregion

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

			} // Family Instance vs Family Instance

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox_UI(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list4_) // Family Instance vs Family Instance
			{

				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				// FAMILY INSTANCES

				// Family Instance
				#region familyinstances BuiltinCategory

				//			BuiltInCategory[] bics_finst = new BuiltInCategory[] 	
				//				{
				//			
				//				    BuiltInCategory.OST_CableTrayFitting,
				//
				//				    BuiltInCategory.OST_ConduitFitting,
				//	
				//				    BuiltInCategory.OST_DuctFitting,
				//				    BuiltInCategory.OST_DuctTerminal,
				//				    BuiltInCategory.OST_ElectricalEquipment,
				//				    BuiltInCategory.OST_ElectricalFixtures,
				//				    BuiltInCategory.OST_LightingDevices,
				//				    BuiltInCategory.OST_LightingFixtures,
				//				    BuiltInCategory.OST_MechanicalEquipment,
				//		
				//				    BuiltInCategory.OST_PipeFitting,
				//				    BuiltInCategory.OST_PlumbingFixtures,
				//				    BuiltInCategory.OST_SpecialityEquipment,
				//				    BuiltInCategory.OST_Sprinklers,
				//		
				//				};
				//			
				//				BuiltInCategory[] bics_finst_2 = new BuiltInCategory[] 	
				//				{
				//			
				//				    BuiltInCategory.OST_CableTrayFitting,
				//
				//				    BuiltInCategory.OST_ConduitFitting,
				//	
				//				    BuiltInCategory.OST_DuctFitting,
				//				    BuiltInCategory.OST_DuctTerminal,
				//				    BuiltInCategory.OST_ElectricalEquipment,
				//				    BuiltInCategory.OST_ElectricalFixtures,
				//				    BuiltInCategory.OST_LightingDevices,
				//				    BuiltInCategory.OST_LightingFixtures,
				//				    BuiltInCategory.OST_MechanicalEquipment,
				//		
				//				    BuiltInCategory.OST_PipeFitting,
				//				    BuiltInCategory.OST_PlumbingFixtures,
				//				    BuiltInCategory.OST_SpecialityEquipment,
				//				    BuiltInCategory.OST_Sprinklers,
				//		
				//				};
				#endregion

				IList<BuiltInCategory> bics_familyIns = UI_list2;
				//			IList<BuiltInCategory> bics_familyIns = bics_finst;

				IList<BuiltInCategory> bics_finst_4 = UI_list4;

				List<Element> clash_familyinstance = new List<Element>();



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

						clash_familyinstance.Add(elem);
					}

				}
				List<Element> clash_yesA = new List<Element>();

				for (int i = 0; i < clash_familyinstance.Count(); i++)
				{
					Element elem = clash_familyinstance[i];

					BoundingBoxXYZ elem_bb = elem.get_BoundingBox(null);

					IList<Element> salida = new List<Element>();


					foreach (BuiltInCategory bic in bics_finst_4)
					{

						Outline outline_2 = new Outline(elem_bb.Min, elem_bb.Max);
						BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);

						ElementClassFilter famFil = new ElementClassFilter(typeof(FamilyInstance));

						ElementCategoryFilter Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter InstancesFilter = new LogicalAndFilter(famFil, Categoryfilter);

						FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente

						IList<Element> elementss = coll_outline_2.WherePasses(bbfilter_2).WherePasses(InstancesFilter).ToElements();

						//					FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
						//					IList<Element> elements = coll_outline_2.WherePasses(bbfilter_2).OfClass(typeof(FamilyInstance)).ToElements();


						//
						//					
						//					foreach (Element n in elements) 
						//					{
						//						salida.Add(n);
						//					}

						if (elementss.Count() > 0) // clash
						{

							if (!clash_yesA.Contains(elem))
							{


								foreach (Element pp in elementss)
								{
									if (!(pp.Id == elem.Id))
									{

										clash_yesA.Add(elem);

										Parameter param = elem.LookupParameter("Clash Category");
										Parameter paramID = elem.LookupParameter("ID Element");
										string elemcategory = elementss.First().Category.Name.ToString() + " / ID: " + elementss.First().Id.ToString();

										using (Transaction t = new Transaction(doc, "Clash Category"))
										{
											t.Start();

											param.Set(elemcategory);

											t.Commit();
										}
									}
								}

							}

						}


					}




				}



				#region borrador

				//	 		string numero_ductos = clashID_familyinstance.Count().ToString();
				//	 		string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;
				//	 		
				//	 		
				//			foreach (ElementId eID in clashID_familyinstance) 
				//	 		{
				//	 
				//	 			//ElementId eID = e.Id;
				//	 			Element e = doc.GetElement(eID);
				//				//LocationPoint p = e.Location as LocationPoint;
				//	 			
				//				//XYZ xyz = new XYZ(p.Point.X, p.Point.Y,p.Point.Z);
				//				
				//				//XYZ p = so.ComputeCentroid();
				//				//FamilyInstance efi = e as FamilyInstance;
				//				
				//				GeometryElement geomElement = e.get_Geometry(new Options() );
				//				
				//				GeometryInstance geomFinstace = geomElement.First() as GeometryInstance;
				//
				//				GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();
				//				
				//				//Solid solid = gSymbol.First() as Solid;
				//	 
				//	 			Solid solid = null;
				//	 			foreach(GeometryObject geomObj in gSymbol)
				//	 			{
				//	 				solid = geomObj as Solid;
				//	 				if( solid != null ) break;
				//	 			}
				//
				////	 			IList<BuiltInCategory> bics_fi2 = UI_list4;
				//	 			IList<BuiltInCategory> bics_fi2 = bics_finst;
				//	 			
				//	 			
				//	 			foreach (BuiltInCategory bic in bics_fi2) 
				//	 			{
				//					// Find intersections between family instances and a selected element
				//		 			// category Mechanical Euqipment
				//		 			ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 			// Create a category filter for Mechanical Euqipment
				//		 			ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);
				//		 
				//		 			LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
				//		 
				//		 			ICollection<ElementId> collectoreID = new List<ElementId>();
				//		 			if (collectoreID.Contains(e.Id)==false)
				//		 			{
				//						collectoreID.Add(eID);
				//					}
				////		 			
				////		 
				//		 			ExclusionFilter filter = new ExclusionFilter(collectoreID);
				//					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
				//		 			
				//					if (bic == BuiltInCategory.OST_CableTray) 
				//		 			{
				//						collector.OfClass(typeof(CableTray));
				//					}
				//		 			if (bic == BuiltInCategory.OST_DuctCurves) 
				//		 			{
				//						collector.OfClass(typeof(Duct));
				//					}
				//		 			if (bic == BuiltInCategory.OST_PipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(Pipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_Conduit) 
				//		 			{
				//						collector.OfClass(typeof(Conduit));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//
				//		 			collector.WherePasses(DU2InstancesFilter);
				//		 			collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
				//		 			collector.WherePasses(filter);
				//		 			
				//		 			if (collector.Count() > 0) 
				//			 		{
				//		 				if (clash_yesA.Contains(e)==false)
				//			 			{
				//							clash_yesA.Add(e);
				//						}
				//			        }
				//			 		
				//			 		foreach (Element elem in collector) 
				//			 		{
				//		 				if (clash_yesA.Contains(elem)==false)
				//			 			{
				//							clash_yesA.Add(elem);
				//						}
				//					}
				//		 			mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
				//			 																	+ e.Name.ToString() 
				//			 																	+ e.Category.Name.ToString() + " id:"
				//																 				+ eID.ToString() + ")"
				//																				+ Environment.NewLine;
				//				}
				//	 		}
				#endregion

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

			} // Family Instance vs Family Instance

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_BBox() // Family Instance vs Family Instance
			{

				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				//			List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				//			List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				// FAMILY INSTANCES

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

				BuiltInCategory[] bics_finst_2 = new BuiltInCategory[]
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

				//			IList<BuiltInCategory> bics_familyIns = UI_list2;
				IList<BuiltInCategory> bics_familyIns = bics_finst;

				List<Element> clash_familyinstance = new List<Element>();



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

						clash_familyinstance.Add(elem);
					}

				}
				List<Element> clash_yesA = new List<Element>();

				for (int i = 0; i < clash_familyinstance.Count(); i++)
				{
					Element elem = clash_familyinstance[i];

					BoundingBoxXYZ elem_bb = elem.get_BoundingBox(null);

					IList<Element> salida = new List<Element>();


					foreach (BuiltInCategory bic in bics_finst_2)
					{

						Outline outline_2 = new Outline(elem_bb.Min, elem_bb.Max);
						BoundingBoxIntersectsFilter bbfilter_2 = new BoundingBoxIntersectsFilter(outline_2);

						ElementClassFilter famFil = new ElementClassFilter(typeof(FamilyInstance));

						ElementCategoryFilter Categoryfilter = new ElementCategoryFilter(bic);

						LogicalAndFilter InstancesFilter = new LogicalAndFilter(famFil, Categoryfilter);

						FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente

						IList<Element> elementss = coll_outline_2.WherePasses(bbfilter_2).WherePasses(InstancesFilter).ToElements();

						//					FilteredElementCollector coll_outline_2 = new FilteredElementCollector(doc, doc.ActiveView.Id); // 2 muros que intercepta con la ventana siguiente
						//					IList<Element> elements = coll_outline_2.WherePasses(bbfilter_2).OfClass(typeof(FamilyInstance)).ToElements();


						//
						//					
						//					foreach (Element n in elements) 
						//					{
						//						salida.Add(n);
						//					}

						if (elementss.Count() > 0) // clash
						{

							if (!clash_yesA.Contains(elem))
							{


								foreach (Element pp in elementss)
								{
									if (!(pp.Id == elem.Id))
									{

										clash_yesA.Add(elem);

										Parameter param = elem.LookupParameter("Clash Category");
										Parameter paramID = elem.LookupParameter("ID Element");
										string elemcategory = elementss.First().Category.Name.ToString() + " / ID: " + elementss.First().Id.ToString();

										using (Transaction t = new Transaction(doc, "Clash Category"))
										{
											t.Start();

											param.Set(elemcategory);

											t.Commit();
										}
									}
								}

							}

						}


					}




				}



				#region borrador

				//	 		string numero_ductos = clashID_familyinstance.Count().ToString();
				//	 		string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;
				//	 		
				//	 		
				//			foreach (ElementId eID in clashID_familyinstance) 
				//	 		{
				//	 
				//	 			//ElementId eID = e.Id;
				//	 			Element e = doc.GetElement(eID);
				//				//LocationPoint p = e.Location as LocationPoint;
				//	 			
				//				//XYZ xyz = new XYZ(p.Point.X, p.Point.Y,p.Point.Z);
				//				
				//				//XYZ p = so.ComputeCentroid();
				//				//FamilyInstance efi = e as FamilyInstance;
				//				
				//				GeometryElement geomElement = e.get_Geometry(new Options() );
				//				
				//				GeometryInstance geomFinstace = geomElement.First() as GeometryInstance;
				//
				//				GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();
				//				
				//				//Solid solid = gSymbol.First() as Solid;
				//	 
				//	 			Solid solid = null;
				//	 			foreach(GeometryObject geomObj in gSymbol)
				//	 			{
				//	 				solid = geomObj as Solid;
				//	 				if( solid != null ) break;
				//	 			}
				//
				////	 			IList<BuiltInCategory> bics_fi2 = UI_list4;
				//	 			IList<BuiltInCategory> bics_fi2 = bics_finst;
				//	 			
				//	 			
				//	 			foreach (BuiltInCategory bic in bics_fi2) 
				//	 			{
				//					// Find intersections between family instances and a selected element
				//		 			// category Mechanical Euqipment
				//		 			ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 			// Create a category filter for Mechanical Euqipment
				//		 			ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);
				//		 
				//		 			LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
				//		 
				//		 			ICollection<ElementId> collectoreID = new List<ElementId>();
				//		 			if (collectoreID.Contains(e.Id)==false)
				//		 			{
				//						collectoreID.Add(eID);
				//					}
				////		 			
				////		 
				//		 			ExclusionFilter filter = new ExclusionFilter(collectoreID);
				//					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
				//		 			
				//					if (bic == BuiltInCategory.OST_CableTray) 
				//		 			{
				//						collector.OfClass(typeof(CableTray));
				//					}
				//		 			if (bic == BuiltInCategory.OST_DuctCurves) 
				//		 			{
				//						collector.OfClass(typeof(Duct));
				//					}
				//		 			if (bic == BuiltInCategory.OST_PipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(Pipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_Conduit) 
				//		 			{
				//						collector.OfClass(typeof(Conduit));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//
				//		 			collector.WherePasses(DU2InstancesFilter);
				//		 			collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
				//		 			collector.WherePasses(filter);
				//		 			
				//		 			if (collector.Count() > 0) 
				//			 		{
				//		 				if (clash_yesA.Contains(e)==false)
				//			 			{
				//							clash_yesA.Add(e);
				//						}
				//			        }
				//			 		
				//			 		foreach (Element elem in collector) 
				//			 		{
				//		 				if (clash_yesA.Contains(elem)==false)
				//			 			{
				//							clash_yesA.Add(elem);
				//						}
				//					}
				//		 			mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
				//			 																	+ e.Name.ToString() 
				//			 																	+ e.Category.Name.ToString() + " id:"
				//																 				+ eID.ToString() + ")"
				//																				+ Environment.NewLine;
				//				}
				//	 		}
				#endregion

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

			} // Family Instance vs Family Instance

			void DYNO_IntersectMultipleFamilyInstanceToMultipleFamilyInstances_UI(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list4_) // Family Instance vs Family Instance
			{

				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

				List<BuiltInCategory> UI_list2 = UI_list2_; // Family instance grupo 1
				List<BuiltInCategory> UI_list4 = UI_list4_; // Family instance grupo 2

				// FAMILY INSTANCES

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

				//			IList<BuiltInCategory> bics_familyIns = UI_list2;
				IList<BuiltInCategory> bics_familyIns = bics_finst;

				List<Element> clash_familyinstance = new List<Element>();
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
						clash_familyinstance.Add(elem);
					}

				}
				List<Element> clash_yesA = new List<Element>();
				foreach (Element elem in clash_familyinstance)
				{


					GeometryElement geomElement = elem.get_Geometry(new Options());

					Point pto = null;
					foreach (GeometryObject geomObj in geomElement)
					{
						pto = geomObj as Point;

						XYZ family_point = new XYZ(pto.Coord.X, pto.Coord.Y, pto.Coord.Z);

						BoundingBoxContainsPointFilter filter_box = new BoundingBoxContainsPointFilter(family_point);
						FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
						IList<Element> elementss = collector.OfClass(typeof(FamilyInstance)).WherePasses(filter_box).ToElements();

						if (elementss.Count() > 0) // clash
						{

							clash_yesA.Add(elem);


						}
						if (pto != null) break;
					}

				}


				#region borrador

				//	 		string numero_ductos = clashID_familyinstance.Count().ToString();
				//	 		string mensaje = "Iteraciones de elementos : " + numero_ductos + "\n\n" + Environment.NewLine;
				//	 		
				//	 		
				//			foreach (ElementId eID in clashID_familyinstance) 
				//	 		{
				//	 
				//	 			//ElementId eID = e.Id;
				//	 			Element e = doc.GetElement(eID);
				//				//LocationPoint p = e.Location as LocationPoint;
				//	 			
				//				//XYZ xyz = new XYZ(p.Point.X, p.Point.Y,p.Point.Z);
				//				
				//				//XYZ p = so.ComputeCentroid();
				//				//FamilyInstance efi = e as FamilyInstance;
				//				
				//				GeometryElement geomElement = e.get_Geometry(new Options() );
				//				
				//				GeometryInstance geomFinstace = geomElement.First() as GeometryInstance;
				//
				//				GeometryElement gSymbol = geomFinstace.GetSymbolGeometry();
				//				
				//				//Solid solid = gSymbol.First() as Solid;
				//	 
				//	 			Solid solid = null;
				//	 			foreach(GeometryObject geomObj in gSymbol)
				//	 			{
				//	 				solid = geomObj as Solid;
				//	 				if( solid != null ) break;
				//	 			}
				//
				////	 			IList<BuiltInCategory> bics_fi2 = UI_list4;
				//	 			IList<BuiltInCategory> bics_fi2 = bics_finst;
				//	 			
				//	 			
				//	 			foreach (BuiltInCategory bic in bics_fi2) 
				//	 			{
				//					// Find intersections between family instances and a selected element
				//		 			// category Mechanical Euqipment
				//		 			ElementClassFilter DUFilter = new ElementClassFilter(typeof(FamilyInstance));
				//		 			// Create a category filter for Mechanical Euqipment
				//		 			ElementCategoryFilter DU2Categoryfilter = new ElementCategoryFilter(bic);
				//		 
				//		 			LogicalAndFilter DU2InstancesFilter = new LogicalAndFilter(DUFilter, DU2Categoryfilter);
				//		 
				//		 			ICollection<ElementId> collectoreID = new List<ElementId>();
				//		 			if (collectoreID.Contains(e.Id)==false)
				//		 			{
				//						collectoreID.Add(eID);
				//					}
				////		 			
				////		 
				//		 			ExclusionFilter filter = new ExclusionFilter(collectoreID);
				//					FilteredElementCollector collector = new FilteredElementCollector(doc, activeView.Id);
				//		 			
				//					if (bic == BuiltInCategory.OST_CableTray) 
				//		 			{
				//						collector.OfClass(typeof(CableTray));
				//					}
				//		 			if (bic == BuiltInCategory.OST_DuctCurves) 
				//		 			{
				//						collector.OfClass(typeof(Duct));
				//					}
				//		 			if (bic == BuiltInCategory.OST_PipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(Pipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_Conduit) 
				//		 			{
				//						collector.OfClass(typeof(Conduit));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//		 			if (bic == BuiltInCategory.OST_FlexPipeCurves) 
				//		 			{
				//						collector.OfClass(typeof(FlexPipe));
				//					}
				//
				//		 			collector.WherePasses(DU2InstancesFilter);
				//		 			collector.WherePasses(new ElementIntersectsSolidFilter(solid)).ToElements(); // Apply intersection filter to find matches
				//		 			collector.WherePasses(filter);
				//		 			
				//		 			if (collector.Count() > 0) 
				//			 		{
				//		 				if (clash_yesA.Contains(e)==false)
				//			 			{
				//							clash_yesA.Add(e);
				//						}
				//			        }
				//			 		
				//			 		foreach (Element elem in collector) 
				//			 		{
				//		 				if (clash_yesA.Contains(elem)==false)
				//			 			{
				//							clash_yesA.Add(elem);
				//						}
				//					}
				//		 			mensaje = mensaje + Environment.NewLine + collector.Count().ToString() + " Family Instance intersect with the selected element ("
				//			 																	+ e.Name.ToString() 
				//			 																	+ e.Category.Name.ToString() + " id:"
				//																 				+ eID.ToString() + ")"
				//																				+ Environment.NewLine;
				//				}
				//	 		}
				#endregion

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
				//			mensaje2 = mensaje + Environment.NewLine + "\nSon: " + clash_yesA.Count().ToString() + " el numero de Elementos con Clashes : " + Environment.NewLine + msg;
				//	 		TaskDialog.Show("Revit", mensaje2);
			} // Family Instance vs Family Instance

			void DYNO_IntersectMultipleFamilyInstanceToMultipleCategory_UI_nofunciona(List<BuiltInCategory> UI_list2_, List<BuiltInCategory> UI_list3_) // Family Instance vs Element
			{
				// Find intersections between Ducts category with selected element

				//UIDocument uidoc = this.ActiveUIDocument;
				//Document doc = this.ActiveUIDocument.Document;
				//Application app = this.Application;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
					activeView = view3d;

					List<ParameterFilterElement> lista_ParameterFilterElement1 = new List<ParameterFilterElement>();
					List<ParameterFilterElement> lista_ParameterFilterElement2 = new List<ParameterFilterElement>();
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

						List<ElementFilter> elementFilterList1 = new List<ElementFilter>();
						List<ElementFilter> elementFilterList_no = new List<ElementFilter>();

						FilterRule[] filterRule_lista = new FilterRule[]
						{
						ParameterFilterRuleFactory.CreateEqualsRule(param.Id,"YES", true),
						ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id,(int)0)
						};

						elementFilterList1.Add(new ElementParameterFilter(filterRule_lista));
						elementFilterList_no.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true)));


						for (int i = 0; i < lista_filtros.Count(); i++)
						{
							if (lista_filtros[i].Name == "CLASH YES FILTER")
							{
								lista_ParameterFilterElement1.Add(lista_filtros[i]);
								i = lista_filtros.Count();
								break;
							}

						}

						if (lista_ParameterFilterElement1.Count() == 0)
						{

							ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, elementFilterList1.First()); // ingresar un ElementFilter	
							lista_ParameterFilterElement1.Add(parameterFilterElement);


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
							ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, elementFilterList_no.First());
							lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
						}

						ParameterFilterElement aa = lista_ParameterFilterElement1.First();
						ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();


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
					activeView = view3d;

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

						List<ElementFilter> elementFilterList = new List<ElementFilter>();

						elementFilterList.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1))); // Clash Solved , EQUAL,  False(int=0),


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
							ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, elementFilterList.First());
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

			void DYNO_CreateClashFilterMultipleElementsInView_UI(View3D view_3d) // Crea filtros en la vista ingresada
			{

				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
				activeView = view_3d;

				FilteredElementCollector elementss = new FilteredElementCollector(doc);
				FillPatternElement solidFillPattern = elementss.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);

				#region cats
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
				#endregion


				List<ParameterFilterElement> lista_ParameterFilterElement1 = new List<ParameterFilterElement>();
				List<ParameterFilterElement> lista_ParameterFilterElement2 = new List<ParameterFilterElement>();
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

					List<ElementFilter> elementFilterList1 = new List<ElementFilter>();
					List<ElementFilter> elementFilterList_no = new List<ElementFilter>();

					FilterRule[] filterRule_lista = new FilterRule[]
					{
						ParameterFilterRuleFactory.CreateEqualsRule(param.Id,"YES", true),
						ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id,(int)0)
					};


					elementFilterList1.Add(new ElementParameterFilter(filterRule_lista));

					elementFilterList_no.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true)));


					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH YES FILTER")
						{
							lista_ParameterFilterElement1.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement1.Count() == 0)
					{

						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, elementFilterList1.First()); // ingresar un ElementFilter

						lista_ParameterFilterElement1.Add(parameterFilterElement);



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
						ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, elementFilterList_no.First());

						lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
					}

					ParameterFilterElement aa = lista_ParameterFilterElement1.First();
					ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();

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
				activeView = view_3d;

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

					List<ElementFilter> elementFilterList = new List<ElementFilter>();

					elementFilterList.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1))); // Clash Solved , EQUAL,  False(int=0),


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
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, elementFilterList.First());

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
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


				List<ParameterFilterElement> lista_ParameterFilterElement1 = new List<ParameterFilterElement>();
				List<ParameterFilterElement> lista_ParameterFilterElement2 = new List<ParameterFilterElement>();
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

					List<ElementFilter> elementFilterList1 = new List<ElementFilter>();
					List<ElementFilter> elementFilterList_no = new List<ElementFilter>();

					FilterRule[] filterRule_lista = new FilterRule[]
					{
						ParameterFilterRuleFactory.CreateEqualsRule(param.Id,"YES", true),
						ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id,(int)0)
					};


					elementFilterList1.Add(new ElementParameterFilter(filterRule_lista));
					elementFilterList_no.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateNotContainsRule(param.Id, "YES", true)));


					for (int i = 0; i < lista_filtros.Count(); i++)
					{
						if (lista_filtros[i].Name == "CLASH YES FILTER")
						{
							lista_ParameterFilterElement1.Add(lista_filtros[i]);
							i = lista_filtros.Count();
							break;
						}

					}

					if (lista_ParameterFilterElement1.Count() == 0)
					{

						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH YES FILTER", cats, elementFilterList1.First()); // ingresar un ElementFilter
						lista_ParameterFilterElement1.Add(parameterFilterElement);


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
						ParameterFilterElement parameterFilterElement_no = ParameterFilterElement.Create(doc, "CLASH NO FILTER", cats, elementFilterList_no.First());

						lista_ParameterFilterElement_no.Add(parameterFilterElement_no);
					}

					ParameterFilterElement aa = lista_ParameterFilterElement1.First();

					ParameterFilterElement aa_no = lista_ParameterFilterElement_no.First();

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;
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

					List<ElementFilter> elementFilterList = new List<ElementFilter>();

					elementFilterList.Add(new ElementParameterFilter(ParameterFilterRuleFactory.CreateEqualsRule(param_solved.Id, (int)1))); // Clash Solved , EQUAL,  False(int=0),


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
						ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "CLASH SOLVED FILTER", cats, elementFilterList.First());

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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
				string view_name = "COORD"; // nombre de la vista activa 
				List<bool> listabool_3 = new List<bool>();
				List<bool> listabool_2 = new List<bool>();
				List<bool> listabool_1 = new List<bool>();
				List<bool> listabool_4 = new List<bool>();

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
						bool ee4 = form.checkBox_4; // TRUE aplicar
						listabool_4.Add(ee4);
						bool ee3 = form.checkBox_3; // TRUE aplicar
						listabool_3.Add(ee3);
						bool ee2 = form.checkBox_2; // TRUE aplicar
						listabool_2.Add(ee2);
						bool ee1 = form.checkBox_1; // TRUE aplicar
						listabool_1.Add(ee1);
					}

					//				ee = listabool_3.First().ToString()+Environment.NewLine
					//					+ listabool_2.First().ToString()+Environment.NewLine
					//					+ listabool_1.First().ToString()+Environment.NewLine;

					//TaskDialog.Show("primero", ee);
					if (listabool_3.First() && !(listabool_2.First()) && !(listabool_1.First()) && !(listabool_4.First()))
					{
						#region Create clash 3d view
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
								if (ve.Name.Contains(view_name))
								{
									views_COORD.Add(ve); // todas la vistas con nombre igual COORD

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
						//DYNO_CreateClashSOLVEDFilterMultipleElementsInView(); // Crea y coloca el filtro CLASH SOLVED a la vista FILTER VIEW
						#endregion
					}
					else if (listabool_1.First() && !(listabool_2.First()) && !(listabool_3.First()) && !(listabool_4.First()))
					{
						DYNO_create3DClashSectionBoxView_ELEMENT();

					}
					else if (listabool_2.First() && !(listabool_1.First()) && !(listabool_3.First()) && !(listabool_4.First()))
					{
						DYNO_create3DClashSectionBoxView_LEVELS();
					}
					else if (listabool_4.First() && !(listabool_1.First()) && !(listabool_3.First()) && !(listabool_2.First()))
					{
						DYNO_create3DClashSectionBoxView_ZONE();
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




							double offset = 2;

							var SectionBox = clashview.GetSectionBox();
							var vMax = SectionBox.Max + SectionBox.Transform.Origin;
							var vMin = SectionBox.Min + SectionBox.Transform.Origin;
							var bbMax = elem_bb.Max; //Point
							var bbMin = elem_bb.Min; //Point




							double Max_X = elem_bb.Max.X;


							double Max_Y = elem_bb.Max.Y;


							double Max_Z = elem_bb.Max.Z;



							double Min_X = elem_bb.Min.X;


							double Min_Y = elem_bb.Min.Y;


							double Min_Z = elem_bb.Min.Z;



							XYZ Max = new XYZ(Max_X + offset, Max_Y + offset, Max_Z + offset);
							XYZ Min = new XYZ(Min_X - offset, Min_Y - offset, Min_Z - offset);

							BoundingBoxXYZ myBox = new BoundingBoxXYZ();

							myBox.Min = Min;
							myBox.Max = Max;

							clashview.SetSectionBox(myBox);

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

			void DYNO_create3DClashSectionBoxView_ZONE() // OK!!!! CLASH ZONE
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = new UIDocument(doc);

				Dictionary<string, XYZ> intersectionPoints = getIntersections(doc);

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;

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

								double Min_Z1 = double.MaxValue;

								// encontrar Min_Y , Min_X , Max_X , Max_Y
								double Min_X1 = double.MaxValue;
								double Min_Y1 = double.MaxValue;

								double Max_X1 = double.MinValue;
								double Max_Y1 = double.MinValue;
								double Max_Z1 = double.MinValue;

								List<double> lista_Min_Z1 = new List<double>();
								List<double> lista_Min_X1 = new List<double>();
								List<double> lista_Min_Y1 = new List<double>();

								List<double> lista_Max_X1 = new List<double>();
								List<double> lista_Max_Y1 = new List<double>();
								List<double> lista_Max_Z1 = new List<double>();


								foreach (ElementId id in clash_elements_zone_ids)
								{
									Element elm = doc.GetElement(id);

									BoundingBoxXYZ box = elm.get_BoundingBox(null);

									if (box.Max.X > Max_X1)
									{
										Max_X1 = box.Max.X;
										lista_Max_X1.Add(Max_X1);
									}
									if (box.Max.Y > Max_Y1)
									{
										Max_Y1 = box.Max.Y;
										lista_Max_Y1.Add(Max_Y1);
									}
									if (box.Max.Z > Max_Z1)
									{
										Max_Z1 = box.Max.Z;
										lista_Max_Z1.Add(Max_Z1);
									}



									if (box.Min.X < Min_X1)
									{
										Min_X1 = box.Min.X;
										lista_Min_X1.Add(Min_X1);
									}
									if (box.Min.Y < Min_Y1)
									{
										Min_Y1 = box.Min.Y;
										lista_Min_Y1.Add(Min_Y1);
									}
									if (box.Min.Z < Min_Z1)
									{
										Min_Z1 = box.Min.Z;
										lista_Min_Z1.Add(Min_Z1);
									}
								}

								// menor a mayor
								lista_Max_X1.Sort();
								lista_Max_Y1.Sort();
								lista_Max_Z1.Sort();
								lista_Min_X1.Sort();
								lista_Min_Y1.Sort();
								lista_Min_Z1.Sort();

								//mayor a menor
								lista_Max_X1.Reverse();
								lista_Max_Y1.Reverse();
								lista_Max_Z1.Reverse();

								XYZ Max1 = new XYZ(lista_Max_X1.First(), lista_Max_Y1.First(), lista_Max_Z1.First());
								XYZ Min1 = new XYZ(lista_Min_X1.First(), lista_Min_Y1.First(), lista_Min_Z1.First());


								BoundingBoxXYZ bb_zone = new BoundingBoxXYZ();

								bb_zone.Min = Min1;
								bb_zone.Max = Max1;


								// Create a new BoundingBoxXYZ to define a 3D rectangular space
								BoundingBoxXYZ elem_bb = bb_zone;


								double offset = 2;

								var SectionBox = clashview.GetSectionBox();
								var vMax = SectionBox.Max + SectionBox.Transform.Origin;
								var vMin = SectionBox.Min + SectionBox.Transform.Origin;
								var bbMax = elem_bb.Max; //Point
								var bbMin = elem_bb.Min; //Point


								double Max_X = elem_bb.Max.X;


								double Max_Y = elem_bb.Max.Y;


								double Max_Z = elem_bb.Max.Z;



								double Min_X = elem_bb.Min.X;


								double Min_Y = elem_bb.Min.Y;


								double Min_Z = elem_bb.Min.Z;



								XYZ Max = new XYZ(Max_X + offset, Max_Y + offset, Max_Z + offset);
								XYZ Min = new XYZ(Min_X - offset, Min_Y - offset, Min_Z - offset);

								BoundingBoxXYZ myBox = new BoundingBoxXYZ();

								myBox.Min = Min;
								myBox.Max = Max;

								clashview.SetSectionBox(myBox);

								//					if (!clashview.IsTemplate) 
								//					{
								clashview.DisplayStyle = DisplayStyle.Shading;
								clashview.DetailLevel = ViewDetailLevel.Fine;



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
					uidoc.ActiveView = tresDclashview.First();
					DYNO_CreateClashFilterMultipleElementsInMultipleViews_UI(tresDclashview);
					DYNO_CreateClashSOLVEDFilterMultipleElementsInMultipleViews_UI(tresDclashview);

				}
				catch (Exception ex)
				{
					//TaskDialog.Show("Error ex", ex.ToString());
					throw;
				}


			} // OK!!!! CLASH ZONE

			List<Element> DYNO_GetAllClashElements_OnlyActiveView()
			{
				//Document doc = this.ActiveUIDocument.Document;
				//UIDocument uidoc = this.ActiveUIDocument;

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;


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

				//// Get Active View
				//View activeView = this.ActiveUIDocument.ActiveView;


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
