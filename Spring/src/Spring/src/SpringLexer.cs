using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;

namespace JetBrains.ReSharper.Plugins.Spring
{
    public class SpringLexer : Sample.SampleLexerGenerated
    {
        public SpringLexer(IBuffer buffer) : base(buffer) { }

        public SpringLexer(IBuffer buffer, int startOffset, int endOffset) : base(buffer, startOffset, endOffset) { }
    }    
}

namespace Sample
{
    public partial class SampleLexerGenerated : ILexer
    {
        private void LocateToken()
        {
            if (currentTokenType == null)
            {
                currentTokenType = _locateToken();
            }
        }
        
        public void Start()
        {
            Start(0, yy_buffer.Length, YYINITIAL);
        }

        public void Start(int startOffset, int endOffset, uint state)
        {
            yy_buffer_index = startOffset;
            yy_buffer_start = startOffset;
            yy_buffer_end = startOffset;
            yy_eof_pos = endOffset;
            yy_lexical_state = (int) state;
            currentTokenType = null;
        }

        public void Advance()
        {
            currentTokenType = null;
            LocateToken();
        }

        public object CurrentPosition
        {
            get
            {
                TokenPosition tokenPosition;
                tokenPosition.CurrentTokenType = currentTokenType;
                tokenPosition.YyBufferIndex = yy_buffer_index;
                tokenPosition.YyBufferStart = yy_buffer_start;
                tokenPosition.YyBufferEnd = yy_buffer_end;
                tokenPosition.YyLexicalState = yy_lexical_state;
                return tokenPosition;
            }

            set
            {
                var tokenPosition = (TokenPosition) value;
                currentTokenType = tokenPosition.CurrentTokenType;
                yy_buffer_index = tokenPosition.YyBufferIndex;
                yy_buffer_start = tokenPosition.YyBufferStart;
                yy_buffer_end = tokenPosition.YyBufferEnd;
                yy_lexical_state = tokenPosition.YyLexicalState;
            }
        }

        public TokenNodeType TokenType
        {
            get
            {
                LocateToken();
                return currentTokenType;
            }
        }

        public int TokenStart
        {
            get
            {
                LocateToken();
                return yy_buffer_start;
            }
        }

        public int TokenEnd
        {
            get
            {
                LocateToken();
                return yy_buffer_end;
            }
        }

        public IBuffer Buffer => yy_buffer;
    }

    public struct TokenPosition
    {
        public TokenNodeType CurrentTokenType;
        public int YyBufferStart;
        public int YyBufferIndex;
        public int YyBufferEnd;
        public int YyLexicalState;
    }
}