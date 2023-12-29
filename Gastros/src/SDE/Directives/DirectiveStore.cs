using System.Collections.Generic;
using OpenEhr.AM.Archetype.ConstraintModel;

namespace GastrOs.Sde.Directives
{
    /// <summary>
    /// Stores and manages GUI directives
    /// </summary>
    public class DirectiveStore
    {
        private Dictionary<CObject, ICollection<IDirective>> innerMap;

        public DirectiveStore()
        {
            innerMap = new Dictionary<CObject, ICollection<IDirective>>();
        }

        public ICollection<IDirective> GetDirectivesFor(CObject node)
        {
            if (innerMap.ContainsKey(node) && innerMap[node] != null)
                return innerMap[node];
            return new List<IDirective>();
        }

        public TD GetDirectiveOfType<TD>(CObject node) where TD : class, IDirective
        {
            return GetDirectivesFor(node).GetDirectiveOfType<TD>();
        }

        public bool HasDirectiveOfType<TD>(CObject node) where TD : class, IDirective
        {
            return GetDirectivesFor(node).HasDirectiveOfType<TD>();
        }

        private ICollection<IDirective> RetrieveDirectivesFor(CObject node)
        {
            ICollection<IDirective> directives;
            if (innerMap.ContainsKey(node) && innerMap[node] != null)
            {
                directives = innerMap[node];
            }
            else
            {
                directives = new List<IDirective>();
                innerMap[node] = directives;
            }
            return directives;
        }

        public void AddDirectiveFor(CObject node, IDirective directive)
        {
            RetrieveDirectivesFor(node).Add(directive);
        }

        public void RemoveDirectiveFor(CObject node, IDirective directive)
        {
            if (!innerMap.ContainsKey(node))
                return;
            ICollection<IDirective> directives = innerMap[node];
            if (directives != null)
            {
                directives.Remove(directive);
                if (directives.Count == 0)
                    innerMap.Remove(node);
            }
        }
    }
}