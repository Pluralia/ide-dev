using System;
using System.Collections.Generic;

namespace LambdaInterp
{
    class Reducer : IVisitor<IExpression>
    {
        private HashSet<string> FreeVars(IExpression expression)
        {
            HashSet<string> freeVars = new HashSet<string>();
            
            if (expression is Variable variable)
            {
                freeVars.Add(variable.Name);
            }
            else if (expression is Application application)
            {
                freeVars = FreeVars(application.Function);
                freeVars.UnionWith(FreeVars(application.Argument));
            }
            else if (expression is Abstraction abstraction)
            {
                freeVars = FreeVars(abstraction.Body);
                freeVars.Remove(abstraction.Variable.Name);
            }

            return freeVars;
        }

        private IExpression Subst(string x, IExpression s, IExpression t)
        {
            if (t is Variable variable)
            {
                if (x == variable.Name)
                {
                    return s;
                }

                return t;
            }
            
            if (t is Application application)
            {
                return new Application(Subst(x, s, application.Function), Subst(x, s, application.Argument));
            }

            if (t is Abstraction abstraction)
            {
                string y = abstraction.Variable.Name;
                
                if (x == y)
                {
                    return t;
                }

                HashSet<string> freeVars = FreeVars(s);
                if (freeVars.Contains(y))
                {
                    Console.WriteLine("RENAME");
                    return null;
                }
                return new Abstraction(abstraction.Variable, Subst(x, s, abstraction.Body));
            }

            Console.WriteLine("SUBST: impossible case!!");
            return null;
        }
        public IExpression Visit(Application application)
        {
            if (application.Function is Abstraction abstraction)
            {
                return Subst(abstraction.Variable.Name, application.Argument, abstraction.Body);
            }
            
            IExpression reducedFunction = application.Function.Accept(this);
            IExpression reducedArg = application.Argument.Accept(this);

            if (reducedFunction is null)
            {
                if (reducedArg is null)
                {
                    return null;
                }
                return new Application(application.Function, reducedArg);
            }
            return new Application(reducedFunction, reducedArg);
        }
        public IExpression Visit(Abstraction abstraction)
        {
            IExpression reducedBody = abstraction.Body.Accept(this);

            if (reducedBody is null)
            {
                return null;
            }
            return new Abstraction(abstraction.Variable, reducedBody);
        }
        public IExpression Visit(Variable expression)
        {
            return null;
        }
    }
}