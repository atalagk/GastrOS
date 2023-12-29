using System.Collections.Generic;
using System.Linq;
using GastrOs.Sde.Support;
using GastrOs.Wrapper.DataObjects;
using GastrOs.Wrapper.Reports.Representation;
using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.Futures.OperationalTemplate;
using OpenEhr.RM.DataStructures.ItemStructure.Representation;
using OpenEhr.RM.DataTypes.Basic;
using OpenEhr.RM.DataTypes.Quantity;
using OpenEhr.RM.DataTypes.Text;

namespace GastrOs.Wrapper.Reports
{
    /// <summary>
    /// Converts value instances into "report-friendly" representations of MST
    /// items.
    /// </summary>
    internal static class ReportExtractor
    {
        private const string UpperLower = "at0005";
        private const string ExamPreparation = "at0125";
        private const string ExamPreparationQuality = "at0128";
        private const string ExamExtent = "at0134";
        private const string FreeText = "at0.179";

        //TODO make me configurable
        private static readonly HashSet<string> SitesIds = new HashSet<string> { "at0110", "at0016", "at0500" };

        private const string NormalTerm = "at0100";

        /// <summary>
        /// Converts specified findings constraint and value instance into an intermediate
        /// "report-friendly" representation.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="findingsCluster"></param>
        /// <param name="organ"></param>
        /// <returns></returns>
        internal static Findings ExtractFindingsOrInterventions(CArchetypeRoot root, Cluster findingsCluster, SdeConcept organ)
        {
            Findings findings = new Findings(organ.Term);
            //go through each heading
            foreach (CComplexObject headingConstraint in root.ExtractChildConstraints())
            {
                //go through each instance of the heading (should really only be one)
                foreach (Cluster heading in findingsCluster.ChildInstances(headingConstraint))
                {
                    //go through each term
                    foreach (CComplexObject termConstraint in headingConstraint.ExtractChildConstraints())
                    {
                        IEnumerable<Item> instances = heading.ChildInstances(termConstraint);
                        IList<Term> termReportInstances = new List<Term>();
                        //go through each instance of the term
                        foreach (Item termItem in instances)
                        {
                            if (termItem is Element)
                            {
                                Element termElem = termItem as Element;
                                //TODO process me
                            }
                            else if (termItem is Cluster)
                            {
                                Cluster termCluster = termItem as Cluster;
                                //Extract term object for reporting
                                Term termReport = ExtractTerm(termConstraint, termCluster, findings);
                                if (termReport != null)
                                {
                                    //Now, if extracted term is "normal", treat it specially
                                    if (termReport.SpellOutPresence)
                                        termReportInstances.Add(termReport);
                                    else
                                        findings.NormalTerm = termReport;
                                }   
                            }
                        }
                        //Add the group of term instances at once
                        if (termReportInstances.Count > 0)
                            findings.AddTermInstances(termReportInstances);
                    }
                }
            }

            return findings;
        }

        /// <summary>
        /// Converts specified term constraint and value instance into an intermediate
        /// "report-friendly" representation.
        /// </summary>
        /// <param name="termConstraint"></param>
        /// <param name="termCluster"></param>
        /// <param name="findings"></param>
        internal static Term ExtractTerm(CComplexObject termConstraint, Cluster termCluster, Findings findings)
        {
            CComplexObject sitesConstraint = null;
            CComplexObject presenceConstraint = termConstraint.GetPresenceConstraint();
            if (presenceConstraint == null)
            {
                //means non-coreconcept
                //TODO check how to record e.g. Distance of Z line
                return null;
            }

            PresenceState presence = termCluster.GetPresence(termConstraint);
            //No need to record non-entered data
            if (presence == PresenceState.Null)
                return null;

            //TODO change this if the semantics of "Normal" changes
            bool isNormal = termConstraint.NodeId.Equals(NormalTerm);
            Term term = new Term(termConstraint.ExtractOntologyText(), presence, !isNormal);

            //go through each site/attribute
            foreach (CComplexObject childConstraint in termConstraint.ExtractChildConstraints())
            {
                //skip presence element
                if (childConstraint.DenotesPresence())
                    continue;
                //mark sites constraint and treat it sepcially (later)
                if (SitesIds.Contains(childConstraint.NodeId))
                {
                    sitesConstraint = childConstraint;
                    continue;
                }
                //extract each "attribute" item
                foreach (Element instance in termCluster.ChildInstances(childConstraint).Cast<Element>())
                {
                    string value = FormatDataValue(termConstraint.GetArchetypeRoot(), instance);
                    if (string.IsNullOrEmpty(value))
                        continue;
                    Attribute att = new Attribute(childConstraint.ExtractOntologyText(), value);
                    term.Attributes.Add(att);
                }
            }

            //extract sites
            if (sitesConstraint != null)
            {
                foreach (Element instance in termCluster.ChildInstances(sitesConstraint).Cast<Element>())
                {
                    string value = FormatDataValue(termConstraint.GetArchetypeRoot(), instance);
                    if (string.IsNullOrEmpty(value))
                        continue;
                    Site site = new Site(value);
                    term.Sites.Add(site);
                }
            }
            return term;
        }

        internal static ExamInfo ExtractExamInfo(CArchetypeRoot root, Cluster examCluster, SdeConcept concept)
        {
            CObject upperLowerConstraint =
                root.ExtractChildConstraints().FirstOrDefault(c => c.NodeId.Equals(UpperLower));
            if (upperLowerConstraint == null)
                return null; //means not upper or lower GIS. skip!
            Cluster upperLower = examCluster.ChildInstances(upperLowerConstraint).FirstOrDefault() as Cluster;
            if (upperLower == null) //means not upper or lower GIS. skip!
                return null;

            ExamInfo exam = new ExamInfo();

            //1. grab extent -> site
            Cluster extent = upperLower.Items.FirstOrDefault(i => i.ArchetypeNodeId.Equals(ExamExtent)) as Cluster;
            if (extent != null)
            {
                Element extentSiteElem = extent.Items.FirstOrDefault(i => SitesIds.Contains(i.ArchetypeNodeId)) as Element;
                string value = FormatDataValue(root, extentSiteElem);
                if (!string.IsNullOrEmpty(value))
                    exam.ExtentSite = new Attribute(AomHelper.ExtractOntologyText(extent.ArchetypeNodeId, root), value);
            }

            //2. grab preparations
            Cluster prep = upperLower.Items.FirstOrDefault(i => i.ArchetypeNodeId.Equals(ExamPreparation)) as Cluster;
            if (prep != null)
            {
                //2.a) grab preparations -> quality
                Element prepQualityElem =
                    prep.Items.FirstOrDefault(i => i.ArchetypeNodeId.Equals(ExamPreparationQuality)) as Element;
                if (prepQualityElem != null)
                {
                    string value = FormatDataValue(root, prepQualityElem);
                    if (!string.IsNullOrEmpty(value))
                        exam.PreparationQuality =
                            new Attribute(AomHelper.ExtractOntologyText(prepQualityElem.ArchetypeNodeId, root), value);
                }
                
                //2.b) grab preparations -> site
                Element prepSiteElem = prep.Items.FirstOrDefault(i => SitesIds.Contains(i.ArchetypeNodeId)) as Element;
                if (prepSiteElem != null)
                {
                    string value = FormatDataValue(root, prepSiteElem);
                    if (!string.IsNullOrEmpty(value))
                        exam.PreparationSite =
                            new Attribute(AomHelper.ExtractOntologyText(prepSiteElem.ArchetypeNodeId, root), value);
                }
            }
            
            return exam;
        }

        internal static Diagnoses ExtractDiagnoses(CArchetypeRoot root, Cluster diagnosesCluster, SdeConcept concept)
        {
            Diagnoses diagnoses = new Diagnoses();
            
            //MST diagnoses -> [Upper/Lower/ERCP] -> Organ *
            CComplexObject tractConstraint = root.ExtractChildConstraints().FirstOrDefault() as CComplexObject;
            Cluster tract = diagnosesCluster.ChildInstances(tractConstraint).FirstOrDefault() as Cluster;
            if (tract == null || tractConstraint == null)
                return diagnoses;

            foreach (CComplexObject organConstraint in tractConstraint.ExtractChildConstraints())
            {
                //"<Organ>: Dx1, Dx2, ... Dx4"
                foreach (Cluster organ in tract.ChildInstances(organConstraint))
                {
                    OrganDiagnoses organDiagnoses = new OrganDiagnoses(organConstraint.ExtractOntologyText());
                    foreach (Element diag in organ.Items.Cast<Element>())
                    {
                        string value = FormatDataValue(root, diag);
                        if (string.IsNullOrEmpty(value))
                            continue;
                        if (diag.ArchetypeNodeId.Equals(FreeText))
                            organDiagnoses.FreeText = value;
                        else
                            organDiagnoses.Diagnoses.Add(value);
                    }
                    diagnoses.Organs.Add(organDiagnoses);
                }
            }

            return diagnoses;
        }

        public static string FormatDataValue(CArchetypeRoot root, Element elem)
        {
            if (elem == null)
                return null;
            DataValue value = elem.Value;
            if (value == null)
                return null;
            if (value is DvCodedText)
            {
                DvCodedText code = value as DvCodedText;
                OntologyItem ontology = AomHelper.ExtractOntology(code.Value, root);
                //this is possible in case of "atxxxx" - must fix this!!
                if (ontology == null)
                    return null;
                return ontology.Text;
            }
            if (value is DvCount && ((DvCount)value).Magnitude == 0)
            {
                return null;
            }
            if (value is DvQuantity && ((DvQuantity)value).Magnitude == 0)
            {
                return null;
            }
            return value.ToString();
        }
    }
}
