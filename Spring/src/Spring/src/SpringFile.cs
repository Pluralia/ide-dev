using System.Xml;
using JetBrains.Platform.MsBuildTask.Utils;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFile : FileElementBase
    {
        public override NodeType NodeType => SpringFileNodeType.Instance;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        public SpringContext Context = new SpringContext();
    }
    
    public class SpringCtor : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.CTOR;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringTerm : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.TERM;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringUnification : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.UNIFICATION;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringInvoke : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.INVOKE;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringPat : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PAT;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringFresh : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.FRESH;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        public SpringContext Context = new SpringContext();
    }
    
    public class SpringDisj : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.DISJ;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringConj : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.CONJ;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringGoal : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.GOAL;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringDef : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.DEF;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        public SpringContext Context = new SpringContext();
    }
    
    public class SpringCtorName : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.CTORNAME;
        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringDecl : CompositeElement, IDeclaration
    {
        public override NodeType NodeType => SpringCompositeNodeType.DECL;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        
        private TreeTextRange IdentRange
        {
            get
            {
                var child = this.Children()
                    .TryGetSingleItem(it => it.NodeType == SpringTokenType.IDENT);
                return child?.GetTreeTextRange() ?? this.GetTreeTextRange();
            }
        }
        
        public TreeTextRange GetNameRange() => IdentRange;
        public XmlNode GetXMLDoc(bool inherit) => null;

        public void SetName(string name)
        {
        }

        public bool IsSynthetic() => false;
        public IDeclaredElement DeclaredElement => new SpringIdentDeclaredElement(this);
        public string DeclaredName => GetText();
    }

    public class SpringIdent : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.IDENT;
        public override PsiLanguageType Language => SpringLanguage.Instance;
        public SpringContext Context = new SpringContext();
        
        private TreeTextRange IdentRange
        {
            get
            {
                var child = this.Children()
                    .TryGetSingleItem(it => it.NodeType == SpringTokenType.IDENT);
                return child?.GetTreeTextRange() ?? this.GetTreeTextRange();
            }
        }
        
        public TreeTextRange GetNameRange() => IdentRange;
        
        public override ReferenceCollection GetFirstClassReferences() =>
            new ReferenceCollection(new SpringIdentReference(this));
    }
}