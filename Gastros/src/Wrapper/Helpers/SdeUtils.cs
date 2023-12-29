using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GastrOs.Sde;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using GastrOs.Wrapper.DataObjects;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;

namespace GastrOs.Wrapper.Helpers
{
    internal class SdeUtils
    {
        private static Dictionary<string, Type> tempTypeMap = new Dictionary<string, Type>();
        static SdeUtils()
        {
            tempTypeMap["ELEMENT"] = typeof (Element);
            tempTypeMap["CLUSTER"] = typeof (Cluster);
        }

        /// <summary>
        /// Convenience method for attempting to generate GUI based on given examination. Returns null if unsuccessful.
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="opt"></param>
        /// <param name="concept"></param>
        /// <returns></returns>
        public static ViewControl TryGenerateView(Examination exam, string opt, SdeConcept concept)
        {
            //first try setting (and loading) the operational template
            try
            {
                GastrOsService.OperationalTemplateName = opt;
            }
            catch (Exception e)
            {
                //TODO elaborate
                MessageBox.Show("GastrOS can't locate the operational template " + GastrOsService.FullOptName + " (" +
                                e.Message + ")");
                return null;
            }
            
            Locatable valueInstance = null;
            string serialisedValueInstance = exam.GetSerialisedValue(concept);
            if (!string.IsNullOrEmpty(serialisedValueInstance))
            {
                try
                {
                    CArchetypeRoot root = GastrOsService.OperationalTemplate.LocateArchetypeById(concept.ArchetypeId);
                    Type rmType = tempTypeMap[root.RmTypeName];
                    valueInstance = rmType.GetConstructor(new Type[0]).Invoke(new object[0]) as Locatable;
                    if (valueInstance != null)
                        EhrSerialiser.LoadFromXmlString(valueInstance, serialisedValueInstance);
                }
                catch (Exception e)
                {
                    Logger.Error("Error while loading archetype " + concept.ArchetypeId +
                        " value instance. Using empty one instead", e);
                }
            }

            ViewControl widget;
            try
            {
                //try loading existing instance, if exists
                if (valueInstance != null)
                {
                    try
                    {
                        widget = GastrOsService.GenerateView(concept.ArchetypeId, valueInstance);
                    }
                    catch (Exception)
                    {
                        //TODO this is pretty hacky. The exception could be because the wrong value instance
                        //was fed into the generator. So try again with a fresh instance.
                        //TODO replace this with validation - i.e. if validates, then load existing instance;
                        //otherwise load fresh one.
                        widget = GastrOsService.GenerateView(concept.ArchetypeId);
                    }
                }
                else
                {
                    widget = GastrOsService.GenerateView(concept.ArchetypeId);
                }
            }
            catch (Exception e)
            {
                //If you reached here, it means the GUI generation failed for some illegitimate reason.
                Logger.Error("Error while loading archetype " + concept.ArchetypeId + " & value instance", e);
                //TODO elaborate
                MessageBox.Show("Error while generating GUI (using operational template '" +
                                GastrOsService.FullOptName + "').\nReason: " + e.Message);
                return null;
            }

            return widget;
        }

        /// <summary>
        /// Convenience method for attempting to generate GUI based on given operational template,
        /// archetype ID and value instance. Returns null if unsuccessful.
        /// </summary>
        /// <param name="opt"></param>
        /// <param name="archetypeId"></param>
        /// <param name="valueInstance"></param>
        /// <returns></returns>
        public static ViewControl TryGenerateView(string opt, string archetypeId, Locatable valueInstance)
        {
            ViewControl widget;

            //first try setting (and loading) the operational template
            try
            {
                GastrOsService.OperationalTemplateName = opt;
            }
            catch (Exception e)
            {
                //TODO elaborate
                MessageBox.Show("GastrOS can't locate the operational template " + GastrOsService.FullOptName + " (" +
                                e.Message + ")");
                return null;
            }

            try
            {
                //try loading existing instance, if exists
                if (valueInstance != null)
                {
                    try
                    {
                        widget = GastrOsService.GenerateView(archetypeId, valueInstance);
                    }
                    catch (Exception)
                    {
                        //TODO this is pretty hacky. The exception could be because the wrong value instance
                        //was fed into the generator. So try again with a fresh instance.
                        //TODO replace this with validation - i.e. if validates, then load existing instance;
                        //otherwise load fresh one.
                        widget = GastrOsService.GenerateView(archetypeId);
                    }
                }
                else
                {
                    widget = GastrOsService.GenerateView(archetypeId);
                }
            }
            catch (Exception e)
            {
                //If you reached here, it means the GUI generation failed for some illegitimate reason.
                Logger.Error("Error while loading archetype " + archetypeId + " & value instance", e);
                //TODO elaborate
                MessageBox.Show("Error while generating GUI (using operational template '" +
                                GastrOsService.FullOptName + "').\nReason: " + e.Message);
                return null;
            }

            return widget;
        }
    }
}
