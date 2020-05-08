﻿using System.Collections.Generic;
using JetBrains.Annotations;
 using JetBrains.ReSharper.Daemon.SyntaxHighlighting;
 using JetBrains.ReSharper.Host.Features.SyntaxHighlighting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Caches2;
 using JetBrains.ReSharper.Psi.Impl;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TestFramework;
using JetBrains.Text;
 using JetBrains.Util;
using NUnit.Framework;
 using Sample;

 namespace JetBrains.ReSharper.Plugins.Spring
{
  [Language(typeof(SpringLanguage))]
  class SpringLanguageService : LanguageService
  {
    public SpringLanguageService([NotNull] PsiLanguageType psiLanguageType,
      [NotNull] IConstantValueService constantValueService) : base(psiLanguageType, constantValueService)
    {
    }

    public override ILexerFactory GetPrimaryLexerFactory()
    {
      return new SpringLexerFactory();
    }

    public override ILexer CreateFilteringLexer(ILexer lexer)
    {
      return lexer;
    }

    public override IParser CreateParser(ILexer lexer, IPsiModule module, IPsiSourceFile sourceFile)
    {
      return new SpringParser(lexer);
    }

    public override IEnumerable<ITypeDeclaration> FindTypeDeclarations(IFile file)
    {
      return EmptyList<ITypeDeclaration>.Instance;
    }

    public override ILanguageCacheProvider CacheProvider => null;
    public override bool IsCaseSensitive => true;
    public override bool SupportTypeMemberCache => false;
    public override ITypePresenter TypePresenter => CLRTypePresenter.Instance;

    internal class SpringLexerFactory : ILexerFactory
    {
      public ILexer CreateLexer(IBuffer buffer)
      {
        return new SpringLexer(buffer);
      }
    }
  }
}
