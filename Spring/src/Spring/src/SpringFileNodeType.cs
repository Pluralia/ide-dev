using System;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.Spring
{
    internal class SpringFileNodeType : CompositeNodeType
    {
        public SpringFileNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringFileNodeType Instance = new SpringFileNodeType("Spring_FILE", 0);

        public override CompositeElement Create()
        {
            return new SpringFile();
        }
    }
    internal class SpringCompositeNodeType : CompositeNodeType
    {
        public SpringCompositeNodeType(string s, int index) : base(s, index)
        {
        }

        public static readonly SpringCompositeNodeType IDENT       = new SpringCompositeNodeType("IDENT", 0);
        public static readonly SpringCompositeNodeType DECL        = new SpringCompositeNodeType("DECL", 12);
        public static readonly SpringCompositeNodeType CTORNAME    = new SpringCompositeNodeType("CTORNAME", 13);
        public static readonly SpringCompositeNodeType CTOR        = new SpringCompositeNodeType("CTOR", 1);
        public static readonly SpringCompositeNodeType TERM        = new SpringCompositeNodeType("TERM", 2);
        public static readonly SpringCompositeNodeType UNIFICATION = new SpringCompositeNodeType("UNIFICATION", 3);
        public static readonly SpringCompositeNodeType INVOKE      = new SpringCompositeNodeType("INVOKE", 4);
        public static readonly SpringCompositeNodeType PAT         = new SpringCompositeNodeType("PAT", 5);
        public static readonly SpringCompositeNodeType FRESH       = new SpringCompositeNodeType("FRESH", 6);
        public static readonly SpringCompositeNodeType DISJ        = new SpringCompositeNodeType("DISJ", 7);
        public static readonly SpringCompositeNodeType CONJ        = new SpringCompositeNodeType("CONJ", 8);
        public static readonly SpringCompositeNodeType GOAL        = new SpringCompositeNodeType("GOAL", 9);
        public static readonly SpringCompositeNodeType DEF         = new SpringCompositeNodeType("DEF", 10);

        public override CompositeElement Create()
        {
            if (this == IDENT)
                return new SpringIdent();
            if (this == DECL)
                return new SpringDecl();
            if (this == CTORNAME)
                return new SpringCtorName();
            if (this == CTOR)
                return new SpringCtor();
            if (this == TERM)
                return new SpringTerm();
            if (this == DEF)
                return new SpringDef();
            if (this == UNIFICATION)
                return new SpringUnification();
            if (this == INVOKE)
                return new SpringInvoke();
            if (this == PAT)
                return new SpringPat();
            if (this == FRESH)
                return new SpringFresh();
            if (this == DISJ)
                return new SpringDisj();
            if (this == CONJ)
                return new SpringConj();
            if (this == GOAL)
                return new SpringGoal();
            throw new InvalidOperationException();
        }
    }

}
