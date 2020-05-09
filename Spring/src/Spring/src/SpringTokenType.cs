using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Text;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Spring
{
    class SpringTokenType : TokenNodeType
    {
        public static SpringTokenType DOUBLE_COLON = new SpringTokenType("DOUBLE_COLON", 0);
        public static SpringTokenType COLON        = new SpringTokenType("COLON", 1);
        public static SpringTokenType EQ           = new SpringTokenType("EQ", 2);
        public static SpringTokenType UNIFICATION  = new SpringTokenType("UNIFICATION", 3);
        public static SpringTokenType AND          = new SpringTokenType("AND", 4);
        public static SpringTokenType OR           = new SpringTokenType("OR", 5);
        
        public static SpringTokenType IDENT = new SpringTokenType("IDENT", 8);

        public static SpringTokenType L_ROUND = new SpringTokenType("L_ROUND", 9);
        public static SpringTokenType R_ROUND = new SpringTokenType("R_ROUND", 10);
        public static SpringTokenType L_CURVY = new SpringTokenType("L_CURVY", 11);
        public static SpringTokenType R_CURVY = new SpringTokenType("R_CURVY", 12);
        public static SpringTokenType L_ANGLE = new SpringTokenType("L_ANGLE", 13);
        public static SpringTokenType R_ANGLE = new SpringTokenType("R_ANGLE", 14);
        public static SpringTokenType L_SQUARE = new SpringTokenType("L_SQUARE", 15);
        public static SpringTokenType R_SQUARE = new SpringTokenType("R_SQUARE", 16);

        public static SpringTokenType WS            = new SpringTokenType("WS", 17);
        public static SpringTokenType COMMENT       = new SpringTokenType("COMMENT", 18);
        public static SpringTokenType BAD_CHARACTER = new SpringTokenType("BAD_CHARACTER", 19);
        
        private static ISet<SpringTokenType> _keywordTokenTypes = new HashSet<SpringTokenType>
        {
            DOUBLE_COLON, COLON, EQ, UNIFICATION, AND, OR
        };
        
        private static ISet<SpringTokenType> _litTokenTypes = new HashSet<SpringTokenType>
        {
            R_ROUND, L_ROUND, R_CURVY, L_CURVY, R_ANGLE, L_ANGLE, R_SQUARE, L_SQUARE
        };
        
        public SpringTokenType(string s, int index) : base(s, index)
        {
        }

        public override LeafElementBase Create(IBuffer buffer, TreeOffset startOffset, TreeOffset endOffset)
        {
            return new SpringToken(buffer.GetText(new TextRange(startOffset.Offset, endOffset.Offset)), this);
        }

        public override bool IsWhitespace => this == WS;
        public override bool IsComment => this == COMMENT;
        public override bool IsStringLiteral => _litTokenTypes.Contains(this);
        public override bool IsConstantLiteral => false;
        public override bool IsIdentifier => this == IDENT;
        public override bool IsKeyword => _keywordTokenTypes.Contains(this);
        public override string TokenRepresentation { get; }

        public class SpringToken : LeafElementBase, ITokenNode
        {
            private readonly string _text;
            private SpringTokenType _type;
        
            public SpringToken(string text, SpringTokenType tokenType)
            {
                _text = text;
                _type = tokenType;
            }
            
            public override int GetTextLength()
            {
                return _text.Length;
            }
        
            public override string GetText()
            {
                return _text;
            }
        
            public override StringBuilder GetText(StringBuilder to)
            {
                to.Append(GetText());
                return to;
            }
        
            public override IBuffer GetTextAsBuffer()
            {
                return new StringBuffer(GetText());
            }
            
            public override NodeType NodeType => _type;
            public override PsiLanguageType Language => SpringLanguage.Instance;
            public TokenNodeType GetTokenType()
            {
                return _type;
            }
        }
    }
}