using System;
using System.Collections.Generic;
using JetBrains.Application.Settings;
using JetBrains.DocumentModel;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon.CSharp.Errors;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.SelectEmbracingConstruct;
using JetBrains.ReSharper.I18n.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.TreeBuilder;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class SpringParser : IParser
    {
        private readonly ILexer myLexer;

        public SpringParser(ILexer lexer)
        {
            myLexer = lexer;
        }

        public IFile ParseFile()
        {
            using (var def = Lifetime.Define())
            {
                var builder = new PsiBuilder(myLexer, SpringFileNodeType.Instance, new TokenFactory(), def.Lifetime);
                var fileMark = builder.Mark();
                
                ParseProgram(builder);
                
                builder.Done(fileMark, SpringFileNodeType.Instance, null);
                var file = (IFile)builder.BuildTree();
                return file;
            }
        }

        private bool ParseProgram(PsiBuilder builder)
        {
            SkipWS(builder);
            var programMark = builder.Mark();
            while (!builder.Eof())
            {
                if (!ParseDef(builder) && !builder.Eof())
                {
                    builder.AdvanceLexer();

                }
                SkipWS(builder);
            }
            builder.Done(programMark, SpringCompositeNodeType.PROGRAM, null);
            return true;
        }

        private bool ParseDef(PsiBuilder builder)
        {
            SkipWS(builder);
            var defMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.DOUBLE_COLON)
            {
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseDecl(builder);
                SkipWS(builder);
                if (!ParseList(builder, ParseDecl, SpringTokenType.WS, SpringTokenType.EQ))
                {
                    builder.Error(defMark, "Expected '='!");
                    return false;
                }
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseGoal(builder);
                SkipWS(builder);
                builder.Done(defMark, SpringCompositeNodeType.DEF, null);
                return true;
            }
            builder.Error(defMark, "Expected definition!");
            return false;
        }

        private bool ParseGoal(PsiBuilder builder)
        {
            SkipWS(builder);
            var goalMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (isFresh(tt))
            {
                ParseFresh(builder);
            }
            else if (isDisj(tt))
            {
                ParseDisj(builder);
            }
            else
            {
                builder.Error(goalMark, "Expected goal!");
                return false;
            }
            builder.Done(goalMark, SpringCompositeNodeType.GOAL, null);
            return true;
        }

        private bool isDisj(TokenNodeType tt)
        {
            return isPat(tt);
        }
        private bool ParseDisj(PsiBuilder builder)
        {
            return ParseOp(builder,
                isConj,
                ParseConj,
                SpringTokenType.OR,
                "Expected conjunction!",
                "Expected disjunction!",
                SpringCompositeNodeType.DISJ);
        }
        
        private bool isConj(TokenNodeType tt)
        {
            return isPat(tt);
        }

        private bool ParseConj(PsiBuilder builder)
        {
            return ParseOp(builder,
                isPat,
                ParsePat,
                SpringTokenType.AND,
                "Expected unification, invoke or fresh!",
                "Expected conjunction!",
                SpringCompositeNodeType.CONJ);
        }

        private bool ParseOp(PsiBuilder builder,
            Predicate<TokenNodeType> isItem,
            Predicate<PsiBuilder> parseItem,
            SpringTokenType delimTT,
            String errorMsgForItem,
            String errorMsgForOp,
            SpringCompositeNodeType nodeType)
        {
            var opMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (isItem(tt))
            {
                parseItem(builder);
                SkipWS(builder);
                while (builder.GetTokenType() == delimTT && !builder.Eof())
                {
                    SkipDelim(builder, delimTT);
                    if (isPat(builder.GetTokenType()) && !builder.Eof())
                    {
                        parseItem(builder);
                        SkipWS(builder);
                    }
                    else
                    {
                        builder.Error(opMark, errorMsgForItem);
                        return false;
                    }
                }
                builder.Done(opMark, nodeType, null);
                return true;
            }
            builder.Error(opMark, errorMsgForOp);
            return false;
        }        
        
        private bool isFresh(TokenNodeType tt)
        {
            return tt == SpringTokenType.L_SQUARE;
        }

        private bool ParseFresh(PsiBuilder builder)
        {
            var freshMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.L_SQUARE)
            {
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseDecl(builder);
                if (!ParseList(builder, ParseDecl, SpringTokenType.WS, SpringTokenType.COLON))
                {
                    builder.Error(freshMark, "Expected ':'!");
                    return false;
                }
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseGoal(builder);
                SkipWS(builder);
                if (builder.GetTokenType() == SpringTokenType.R_SQUARE)
                {
                    builder.AdvanceLexer();
                    builder.Done(freshMark, SpringCompositeNodeType.FRESH, null);
                    return true;
                }
                builder.Error(freshMark, "Expected ']'!");
                return false;
            }
            builder.Error(freshMark, "Expected fresh!");
            return false;
        }

        private bool isPat(TokenNodeType tt)
        {
            return isUnification(tt) ||
                   tt == SpringTokenType.L_ROUND ||
                   isFresh(tt) ||
                   isInvoke(tt);
        }
        
        private bool ParsePat(PsiBuilder builder)
        {
            var patMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (isUnification(tt))
            {
                ParseUnification(builder);
            }
            else if (tt == SpringTokenType.L_ROUND)
            {
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseDisj(builder);
                SkipWS(builder);
                if (builder.GetTokenType() == SpringTokenType.R_ROUND)
                {
                    builder.AdvanceLexer();
                }
                else
                {
                    builder.Error(patMark, "Expected ')'!");
                    return false;
                }
            }
            else if (isFresh(tt))
            {
                ParseFresh(builder);
            }
            else if (isInvoke(tt))
            {
                ParseInvoke(builder);
            }
            else
            {
                builder.Error(patMark, "Expected unification, invoke or fresh!");
                return false;
            }

            builder.Done(patMark, SpringCompositeNodeType.PAT, null);
            return true;
        }
        
        private bool isUnification(TokenNodeType tt)
        {
            return isTerm(tt);
        }

        private bool ParseUnification(PsiBuilder builder)
        {
            var unificationMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (isTerm(tt))
            {
                ParseTerm(builder);
                SkipDelim(builder, SpringTokenType.UNIFICATION);
                ParseTerm(builder);
            }
            else
            {
                builder.Error(unificationMark, "Expected unification!");
                return false;
            }
            
            builder.Done(unificationMark, SpringCompositeNodeType.TERM, null);
            return true;
        }

        private bool isInvoke(TokenNodeType tt)
        {
            return tt == SpringTokenType.L_CURVY;
        }

        private bool ParseInvoke(PsiBuilder builder)
        {
            var invokeMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.L_CURVY)
            {
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseIdent(builder);
                SkipWS(builder);
                if (ParseList(builder, ParseTerm, SpringTokenType.WS, SpringTokenType.R_CURVY))
                {
                    builder.AdvanceLexer();
                    builder.Done(invokeMark, SpringCompositeNodeType.INVOKE, null);
                    return true;
                }
                builder.Error(invokeMark, "Expected '}'!");
                return false;
            }
            builder.Error(invokeMark, "Expected invoke!");
            return false;
        }

        private bool isTerm(TokenNodeType tt)
        {
            return tt == SpringTokenType.IDENT || tt == SpringTokenType.L_ANGLE;
        }

        private bool ParseTerm(PsiBuilder builder)
        {
            var termMark = builder.Mark();
            var tt = builder.GetTokenType();
            if (tt == SpringTokenType.IDENT)
            {
                ParseIdent(builder);
            }
            else if (tt == SpringTokenType.L_ANGLE)
            {
                var mark = builder.Mark();
                builder.AdvanceLexer();
                SkipWS(builder);
                ParseIdent(builder);
                SkipDelim(builder, SpringTokenType.COLON);
                if (ParseList(builder, ParseTerm, SpringTokenType.WS, SpringTokenType.R_ANGLE))
                {
                    builder.AdvanceLexer();
                    builder.Done(mark, SpringCompositeNodeType.CTOR, null);
                }
                else
                {
                    builder.Error(mark, "Expected '>'!");
                }
            }
            else
            {
                builder.Error(termMark, "Expected term!");
                return false;
            }
            
            builder.Done(termMark, SpringCompositeNodeType.TERM, null);
            return true;
        }
        
        private static bool ParseDecl(PsiBuilder builder)
        {
            var declMark = builder.Mark();
            if (builder.GetTokenType() == SpringTokenType.IDENT)
            {
                builder.AdvanceLexer();
                builder.Done(declMark, SpringCompositeNodeType.DECL, null);
                return true;
            }
            builder.Error(declMark, "Expected declaration!");
            return false;
        }
        
        private static bool ParseIdent(PsiBuilder builder)
        {
            var identMark = builder.Mark();
            if (builder.GetTokenType() == SpringTokenType.IDENT)
            {
                builder.AdvanceLexer();
                builder.Done(identMark, SpringCompositeNodeType.IDENT, null);
                return true;
            }
            builder.Error(identMark, "Expected identifier!");
            return false;
        }

        private bool ParseList(PsiBuilder builder,
            Predicate<PsiBuilder> parseItem, 
            SpringTokenType delimTT,
            SpringTokenType finTT)
        {
            while (builder.GetTokenType() != finTT && !builder.Eof())
            {
                SkipDelim(builder, delimTT);
                if (!parseItem(builder) && !builder.Eof())
                {
                    builder.AdvanceLexer();
                }
                SkipWS(builder);
            }
            return builder.GetTokenType() == finTT;
        }

        private static void SkipDelim(PsiBuilder builder, SpringTokenType delim)
        {
            if (delim == SpringTokenType.WS)
            {
                SkipWS(builder);
                return;
            }
            SkipWS(builder);
            SkipIt(builder, delim);
            SkipWS(builder);
        }


        private static void SkipIt(PsiBuilder builder, SpringTokenType tt)
        {
            if (builder.GetTokenType() == tt)
            {
                builder.AdvanceLexer();
            }
            else
            {
                builder.Error("Expected " + tt + "!");
            }
        }
        
        private static void SkipWS(PsiBuilder builder)
        {
            while (!builder.Eof() && 
                   (builder.GetTokenType() == SpringTokenType.WS || 
                   builder.GetTokenType() == SpringTokenType.COMMENT))
            {
                builder.AdvanceLexer();
            }
        }
    }

    [DaemonStage]
    class SpringDaemonStage : DaemonStageBase<SpringFile>
    {
        protected override IDaemonStageProcess CreateDaemonProcess(IDaemonProcess process, 
            DaemonProcessKind processKind, SpringFile file,
            IContextBoundSettingsStore settingsStore)
        {
            return new SpringDaemonProcess(process, file);
        }

        internal class SpringDaemonProcess : IDaemonStageProcess
        {
            private readonly SpringFile myFile;
            public SpringDaemonProcess(IDaemonProcess process, SpringFile file)
            {
                myFile = file;
                DaemonProcess = process;
            }

            public void Execute(Action<DaemonStageResult> committer)
            {
                var highlightings = new List<HighlightingInfo>();
                foreach (var treeNode in myFile.Descendants())
                {
                    if (treeNode is PsiBuilderErrorElement error)
                    {
                        var range = error.GetDocumentRange();
                        highlightings.Add(new HighlightingInfo(range, 
                            new CSharpSyntaxError(error.ErrorDescription, range)));
                    }
                    // else if (treeNode)
                    // {
                    // }
                }
                
                var result = new DaemonStageResult(highlightings);
                committer(result);
            }

            public IDaemonProcess DaemonProcess { get; }
        }

        protected override IEnumerable<SpringFile> GetPsiFiles(IPsiSourceFile sourceFile)
        {
            yield return (SpringFile)sourceFile.GetDominantPsiFile<SpringLanguage>();
        }
    } 

    internal class TokenFactory : IPsiBuilderTokenFactory
    {
        public LeafElementBase CreateToken(TokenNodeType tokenNodeType, IBuffer buffer, int startOffset, int endOffset)
        {
            return tokenNodeType.Create(buffer, new TreeOffset(startOffset), new TreeOffset(endOffset));
        }
    }

    [ProjectFileType(typeof (SpringProjectFileType))]
    public class SelectEmbracingConstructProvider : ISelectEmbracingConstructProvider
    {
        public bool IsAvailable(IPsiSourceFile sourceFile)
        {
            return sourceFile.LanguageType.Is<SpringProjectFileType>();
        }

        public ISelectedRange GetSelectedRange(IPsiSourceFile sourceFile, DocumentRange documentRange)
        {
            var file = (SpringFile) sourceFile.GetDominantPsiFile<SpringLanguage>();
            var node = file.FindNodeAt(documentRange);
            return new SpringTreeNodeSelection(file, node);
        }

        public class SpringTreeNodeSelection : TreeNodeSelection<SpringFile>
        {
            public SpringTreeNodeSelection(SpringFile fileNode, ITreeNode node) : base(fileNode, node)
            {
            }

            public override ISelectedRange Parent => new SpringTreeNodeSelection(FileNode, TreeNode.Parent);
        }
    }
}
