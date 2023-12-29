using GastrOs.Sde.Configuration;
using GastrOs.Sde.Support;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde.Test.GuiTests
{
    public class ModelViewTestBase
    {
        /// <summary>
        /// <para>Structure:</para>
        /// <example>
        /// Cluster [at0000] (Testing)
        ///   Cluster [at6000] (EXCAVATED LESIONS) - {isOrganiser}
        ///     Cluster [at6200] (Ulcer) - {isCoreConcept}
        ///       Element [at0105] (Present?)
        ///         DvCodedText
        ///       Element [at6210] (Number)
        ///         DvCount
        ///       Element [at6220] (Size)
        ///         DvQuantity
        ///       Element [at6230] (Shape)
        ///         DvText
        ///       Element [at6250] (Stigmata of bleeding)
        ///         DvCodedText
        ///       Element [at0500] (Sites(s)) - {showAs(splash,smart);showWithParent(simple)}
        ///         DvCodedText
        /// </example>
        /// </summary>
        protected CArchetypeRoot root;

        protected IUnityContainer container;

        public ModelViewTestBase()
        {
            root = EhrSerialiser.Load<OperationalTemplate>("TestTemplate.opt").Definition;
            Assert.IsNotNull(root);

            container = new UnityContainer();
            GastrOsConfig.UnityConfig.Configure(container, GastrOsConfig.EngineConfig.UnityContainerName);
        }
    }
}
