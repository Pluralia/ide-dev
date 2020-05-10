using System.Collections.Generic;
using System.Linq;
using JetBrains.Platform.MsBuildTask.Utils;
using JetBrains.ReSharper.Plugins.Spring;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringContext
    {
        private readonly SpringContext parentContext;
        private readonly ISet<SpringDecl> currentContext = new HashSet<SpringDecl>();
        
        public SpringContext(SpringContext inParentContext = null)
        {
            parentContext = inParentContext;
        }

        public SpringDecl GetDecl(string ident)
        {
            var decl = currentContext.TryGetSingleItem(it => it.DeclaredName == ident);
            if (decl != null)
                return decl;
            return parentContext?.GetDecl(ident);
        }

        private void Add(SpringDecl decl)
        {
            currentContext.Add(decl);
        }

        public static void InitFileContext(ITreeNode node)
        {
            if (!(node is SpringFile file)) return;
            foreach (var child in file.Children())
            {
                if (child is SpringDef def && def.FirstChild?.NextSibling?.NextSibling is SpringDecl decl)
                {
                    file.Context.Add(decl);
                }
            }

            foreach (var child in file.Children())
            {
                InitContext(child, file.Context);
            }
        }

        private static void InitContext(ITreeNode node, SpringContext parentContext)
        {
            if (node is SpringFresh fresh)
            {
                fresh.Context = new SpringContext(parentContext);
                foreach (var child in fresh.Children())
                {
                    if (child is SpringDecl childDecl)
                    {
                        fresh.Context.Add(childDecl);
                    }
                }
                foreach (var child in fresh.Children())
                {
                    if (!(child is SpringDecl _))
                    {
                        InitContext(child, fresh.Context);
                    }
                }
                return;
            }
            if (node is SpringDef def)
            {
                def.Context = new SpringContext(parentContext);
                foreach (var child in def.Children())
                {
                    if (child is SpringDecl childDecl)
                    {
                        def.Context.Add(childDecl);
                    }
                }
                foreach (var child in def.Children())
                {
                    if (!(child is SpringDecl _))
                    {
                        InitContext(child, def.Context);
                    }
                }
                return;
            }
            if (node is SpringIdent ident)
            {
                ident.Context = parentContext;
                return;
            }
            
            foreach (var child in node.Children())
            {
                InitContext(child, parentContext);
            }

        }
    }
}