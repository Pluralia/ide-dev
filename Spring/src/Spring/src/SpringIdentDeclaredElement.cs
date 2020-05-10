using System.Collections.Generic;
using System.Xml;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util.DataStructures;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringIdentDeclaredElement : IDeclaredElement
    {
        private readonly SpringDecl decl;

        public SpringIdentDeclaredElement(SpringDecl inDecl)
        {
            decl = inDecl;
        }

        public IPsiServices GetPsiServices() => decl.GetPsiServices();
        public IList<IDeclaration> GetDeclarations() => new List<IDeclaration> {decl};
        
        public IList<IDeclaration> GetDeclarationsIn(IPsiSourceFile sourceFile) => 
            new List<IDeclaration> {decl};
        
        public DeclaredElementType GetElementType() => CLRDeclaredElementType.LOCAL_VARIABLE;
        public XmlNode GetXMLDoc(bool inherit) => null;
        public XmlNode GetXMLDescriptionSummary(bool inherit) => null;
        public bool IsValid() => decl.IsValid();
        public bool IsSynthetic() => false;
        
        public HybridCollection<IPsiSourceFile> GetSourceFiles()
        {
            var file = decl.GetSourceFile();
            return file == null ? 
                HybridCollection<IPsiSourceFile>.Empty : 
                new HybridCollection<IPsiSourceFile>(file);
        }
        
        public bool HasDeclarationsIn(IPsiSourceFile sourceFile) =>
            sourceFile.Equals(decl.GetSourceFile());
        
        public string ShortName => decl.DeclaredName;
        public bool CaseSensitiveName => true;
        public PsiLanguageType PresentationLanguage => SpringLanguage.Instance;
    }
}