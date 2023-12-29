using OpenEhr.AM.Archetype.ConstraintModel;
using OpenEhr.Futures.OperationalTemplate;

namespace GastrOs.Sde.Support
{
    public delegate void VisitCObject(CObject obj, CArchetypeRoot root, params object[] args);

    public delegate void VisitCAttribute(CAttribute attr, CArchetypeRoot root, params object[] args);

    /// <summary>
    /// A visitor that alternatively traverses through the CObjects and CAttributes
    /// in an archetype object's heirarchy
    /// </summary>
    public static class SimpleAomVisitor
    {
        /// <summary>
        /// Traverse through the given archetype root
        /// </summary>
        /// <param name="root">the archetype root</param>
        /// <param name="cObjectVisitMethod">visitor method for CObject</param>
        /// <param name="cAttributeVisitMethod">visitor method for CAttribute</param>
        /// <param name="args">Any arguments to be passed on to visitor methods</param>
        public static void Visit(CArchetypeRoot root, VisitCObject cObjectVisitMethod,
                                 VisitCAttribute cAttributeVisitMethod, params object[] args)
        {
            TraverseCObject(root, root, cObjectVisitMethod, cAttributeVisitMethod, args);
        }

        private static void TraverseCObject(CObject obj,
                                            CArchetypeRoot root,
                                            VisitCObject cObjectVisitMethod,
                                            VisitCAttribute cAttributeVisitMethod,
                                            params object[] args)
        {
            //designate new root
            if (obj != root && obj is CArchetypeRoot)
                root = obj as CArchetypeRoot;

            cObjectVisitMethod(obj, root, args);

            if (obj is CComplexObject)
            {
                //in case of complex object, recurse through its attributes
                CComplexObject complexObj = obj as CComplexObject;
                if (complexObj.Attributes != null)
                {
                    foreach (CAttribute attribute in complexObj.Attributes)
                    {
                        TraverseCAttribute(attribute, root, cObjectVisitMethod, cAttributeVisitMethod, args);
                    }
                }
            }
        }

        private static void TraverseCAttribute(CAttribute attr,
                                               CArchetypeRoot root,
                                               VisitCObject cObjectVisitMethod,
                                               VisitCAttribute cAttributeVisitMethod,
                                               params object[] args)
        {
            cAttributeVisitMethod(attr, root, args);

            if (attr.Children != null)
            {
                //go through children, i.e. constraint objects (if any)
                foreach (CObject child in attr.Children)
                {
                    TraverseCObject(child, root, cObjectVisitMethod, cAttributeVisitMethod, args);
                }
            }
        }
    }
}