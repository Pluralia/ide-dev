using System;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;
using JetBrains.ReSharper.Plugins.Spring;

%%
%{
TokenNodeType currentTokenType;
%}

%unicode

%init{
  currentTokenType = null;
%init}

%namespace Sample
%class SampleLexerGenerated

%function _locateToken
%public
%type TokenNodeType
%ignorecase

%eofval{
  currentTokenType = null; return currentTokenType;
%eofval}


ALPHA=[A-Za-z]
DIGIT=[0-9]
NEWLINE=((\r\n)|\n)
NONNEWLINE_WHITE_SPACE_CHAR=[\ \t\b\012]
WHITE_SPACE_CHAR=({NEWLINE}|{NONNEWLINE_WHITE_SPACE_CHAR})
IDENT=({ALPHA}+{DIGIT}*)
COMMENT=("--"[^\n]*\n)

%%
<YYINITIAL> "::" { return currentTokenType = SpringTokenType.DOUBLE_COLON; }
<YYINITIAL> ":" { return currentTokenType = SpringTokenType.COLON; }
<YYINITIAL> "=" { return currentTokenType = SpringTokenType.EQ; }
<YYINITIAL> "===" { return currentTokenType = SpringTokenType.UNIFICATION; }
<YYINITIAL> "|||" { return currentTokenType = SpringTokenType.OR; }
<YYINITIAL> "&&&" { return currentTokenType = SpringTokenType.AND; }

<YYINITIAL> conde { return currentTokenType = SpringTokenType.CONDE; }
<YYINITIAL> {IDENT}+ { return currentTokenType = SpringTokenType.IDENT; }

<YYINITIAL> "(" { return currentTokenType = SpringTokenType.L_ROUND; }
<YYINITIAL> ")" { return currentTokenType = SpringTokenType.R_ROUND; }
<YYINITIAL> "{" { return currentTokenType = SpringTokenType.L_CURVY; }
<YYINITIAL> "}" { return currentTokenType = SpringTokenType.R_CURVY; }
<YYINITIAL> "<" { return currentTokenType = SpringTokenType.L_ANGLE; }
<YYINITIAL> ">" { return currentTokenType = SpringTokenType.R_ANGLE; }

<YYINITIAL> {WHITE_SPACE_CHAR}+ { return currentTokenType = SpringTokenType.WS; }
<YYINITIAL> {COMMENT} { return currentTokenType = SpringTokenType.COMMENT; }
<YYINITIAL> . { return currentTokenType = SpringTokenType.BAD_CHARACTER; }	