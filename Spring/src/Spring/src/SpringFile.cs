using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringFile : FileElementBase
    {
        public override NodeType NodeType => SpringFileNodeType.Instance;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }

    public class SpringIdent : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.IDENT;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
    
    public class SpringDecl : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.DECL;

        public override PsiLanguageType Language => SpringLanguage.Instance;
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
    }
    
    public class SpringProgram : CompositeElement
    {
        public override NodeType NodeType => SpringCompositeNodeType.PROGRAM;

        public override PsiLanguageType Language => SpringLanguage.Instance;
    }
}