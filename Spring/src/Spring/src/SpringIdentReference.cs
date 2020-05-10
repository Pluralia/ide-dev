using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Resolve;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringIdentReference : TreeReferenceBase<SpringIdent>
    {
        private readonly SpringIdent ident;

        public SpringIdentReference([NotNull] SpringIdent inIdent) : base(inIdent)
        {
            ident = inIdent;
        }

        public override ResolveResultWithInfo ResolveWithoutCache()
        {
            var springDecl = ident.Context.GetDecl(GetName());
            return springDecl == null ? 
                ResolveResultWithInfo.Unresolved : 
                new ResolveResultWithInfo(new SimpleResolveResult(springDecl.DeclaredElement), ResolveErrorType.OK);
        }

        public override string GetName() => ident.GetText();
        public override ISymbolTable GetReferenceSymbolTable(bool useReferenceName) => 
            throw new NotImplementedException();

        public override TreeTextRange GetTreeTextRange() => ident.GetNameRange();

        public override IReference BindTo(IDeclaredElement element) => this;
        public override IReference BindTo(IDeclaredElement element, ISubstitution substitution) => this;

        public override IAccessContext GetAccessContext() => new DefaultAccessContext(ident);
        public override bool IsValid() => myOwner.IsValid();
    }
}
