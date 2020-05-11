using System;
using System.Collections.Generic;
using System.IO;

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

        // private IExpression Rename(string cur, IExpression term)
        // {
        //     
        // }
        //
        // private string Next(string cur, IExpression term)
        // {
        //     if (FreeVars(term).Contains(cur))
        //         return cur;
        //     
        // }

        private IExpression Subst(string x, IExpression s, IExpression term)
        {
            if (term is Variable variable)
            {
                return x == variable.Name ? s : term;
            }
            
            if (term is Application application)
            {
                return new Application(Subst(x, s, application.Function), Subst(x, s, application.Argument));
            }

            if (term is Abstraction abstraction)
            {
                var y = abstraction.Variable;
                
                if (x == y.Name)
                    return term;

                var freeVars = FreeVars(s);
                if (!freeVars.Contains(y.Name))
                {
                    return new Abstraction(y, Subst(x, s, abstraction.Body));
                }
                Console.WriteLine("SUBST: RENAME");
                return null;
                // return Subst(x, s, Rename(y, term));
            }

            Console.WriteLine("SUBST: impossible case!");
            return null;
        }
        public IExpression Visit(Application application)
        {
            if (application.Function is Abstraction abstraction)
            {
                return Subst(abstraction.Variable.Name, application.Argument, abstraction.Body);
            }
            
            IExpression reducedFunction = application.Function.Accept(this);
            if (!(reducedFunction is null))
            {
                return new Application(reducedFunction, application.Argument);
            }
            
            IExpression reducedArgument = application.Argument.Accept(this);
            if (!(reducedArgument is null))
            {
                return new Application(application.Function, reducedArgument);
                
            }
            Console.WriteLine("REDUCE APPLICATION: all parts are null!");
            return null;
        }
        
        public IExpression Visit(Abstraction abstraction)
        {
            IExpression reducedBody = abstraction.Body.Accept(this);

            if (reducedBody is null)
            {
                Console.WriteLine("REDUCE ABSTRACTION: body is null!");
                return null;
            }
            return new Abstraction(abstraction.Variable, reducedBody);
        }
        public IExpression Visit(Variable expression)
        {
            return expression;
        }
    }
}