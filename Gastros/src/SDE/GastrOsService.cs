using System;
using GastrOs.Sde.Engine;
using GastrOs.Sde.Support;
using GastrOs.Sde.ViewControls;
using OpenEhr.RM.Common.Archetyped.Impl;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde
{
    /// <summary>
    /// Provides a gateway for clients of GastrOS to generate views from
    /// archetypes (operational templates).
    /// 
    /// TODO implement some kind of pooling mechanism to save resources
    /// </summary>
    public static class GastrOsService
    {
        private static OperationalTemplate currentOpt;
        private static string currentOptName;
        private static string language = "en-NZ";

        /// <summary>
        /// Gets or sets the name of the operational template to generate
        /// views from
        /// </summary>
        public static string OperationalTemplateName
        {
            get
            {
                return currentOptName;
            }
            set
            {
                if (string.Equals(currentOptName, value))
                    return;
                currentOptName = value;
                RefreshOpt();
            }
        }

        public static OperationalTemplate OperationalTemplate
        {
            get
            {
                return currentOpt;
            }
        }

        /// <summary>
        /// Gets or sets the language (e.g. en, tr) of the operational
        /// template
        /// </summary>
        public static string Language
        {
            get
            {
                return language;
            }
            set
            {
                if (string.Equals(language, value))
                    return;
                language = value;
                if (currentOptName != null)
                    RefreshOpt();
            }
        }

        public static string FullOptName
        {
            get
            {
                return GetFullOptName(currentOptName, language);
            }
        }

        private static void RefreshOpt()
        {
            currentOpt = EhrSerialiser.Load<OperationalTemplate>(FullOptName);
            if (currentOpt == null)
                throw new ArgumentException("Could not successfully load operational template file " + FullOptName);
        }

        public static ViewControl GenerateView(string archId, Locatable instance)
        {
            if (currentOpt == null)
                throw new InvalidOperationException("Operational template hasn't been specified yet");
            Coordinator coordinator = new Coordinator(currentOpt);
            if (instance == null)
                instance = RmFactory.Instantiate(archId, currentOpt);
            return coordinator.GenerateView(instance, archId);
        }

        public static ViewControl GenerateView(string archId)
        {
            return GenerateView(archId, null);
        }

        public static string GetFullOptName(string operationalTemplate, string lang)
        {
            //strip the .opt extension, if exists
            int dotIndex = operationalTemplate.IndexOf(".opt");
            operationalTemplate = dotIndex > 0 ? operationalTemplate.Substring(0, dotIndex) : operationalTemplate;
            //then append underscore followed by language (unless language is english or null)
            operationalTemplate = (lang != null && !lang.StartsWith("en-"))
                                      ? operationalTemplate + "." + lang
                                      : operationalTemplate;
            return operationalTemplate + ".opt";
        }
    }
}