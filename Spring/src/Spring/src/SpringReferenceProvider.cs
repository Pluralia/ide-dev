using System.Collections.Generic;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    [ReferenceProviderFactory]
    public class SpringReferenceProvider : IReferenceProviderFactory
    {
        public SpringReferenceProvider(Lifetime lifetime)
        {
            Changed = new DataFlow.Signal<IReferenceProviderFactory>(lifetime, GetType().FullName);
        }

        public IReferenceFactory CreateFactory(IPsiSourceFile sourceFile, IFile file, IWordIndex wordIndexForChecks)
        {
            return sourceFile.PrimaryPsiLanguage.Is<SpringLanguage>() ? new SpringReferenceFactory() : null;
        }

        public DataFlow.ISignal<IReferenceProviderFactory> Changed { get; }
    }


    public class SpringReferenceFactory : IReferenceFactory
    {
        public ReferenceCollection GetReferences(ITreeNode element, ReferenceCollection oldReferences)
        {
            return element is SpringIdent variable
                ? new ReferenceCollection(new List<IReference> {new SpringIdentReference(variable)})
                : ReferenceCollection.Empty;
        }

        public bool HasReference(ITreeNode element, IReferenceNameContainer names)
        {
            if (!(element is SpringIdent variable)) return false;
            var name = variable.GetText();
            return names.Contains(name);
        }
    }
}