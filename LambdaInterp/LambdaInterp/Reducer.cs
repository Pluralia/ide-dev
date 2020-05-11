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
        
        private HashSet<string> Vars(IExpression expression)
        {
            HashSet<string> vars = new HashSet<string>();
            
            if (expression is Variable variable)
            {
                vars.Add(variable.Name);
            }
            else if (expression is Application application)
            {
                vars = Vars(application.Function);
                vars.UnionWith(Vars(application.Argument));
            }
            else if (expression is Abstraction abstraction)
            {
                vars = FreeVars(abstraction.Body);
                vars.Add(abstraction.Variable.Name);
            }

            return vars;
        }

        private string NumToVar(int n)
        {
            if (n < 0)
            {
                Console.WriteLine("NumToVar: number is less then 0!");
                return null;
            }

            if (n < 26)
                return Convert.ToChar(n).ToString();

            return Convert.ToChar(n % 26) + NumToVar(n / 26);
        }
        
        private int VarToNum(string v)
        {
            if (v.Equals(""))
            {
                return 0;
            }

            return Convert.ToInt32(v[0]) - 97 + 26 * VarToNum(v.Substring(1));
        }

        private IExpression Rename(string cur, IExpression term)
        {
            return Go(cur, Next(cur, term), term);
        }
        
        private string Next(string cur, IExpression term)
        {
            if (FreeVars(term).Contains(cur))
                return cur;
            
            var max = 0;
            foreach (var var in Vars(term))
            {
                var num = VarToNum(var);
                if (max < num)
                    max = num;
            }
            return NumToVar(max + 1);
        }

        private IExpression Go(string cur, string next, IExpression term)
        {
            if (term is Variable variable)
            {
                return variable.Name == cur ? new Variable(next) : term;
            }

            if (term is Application application)
            {
                return new Application(Go(cur, next, application.Function), 
                    Go(cur, next, application.Argument));
            }

            if (term is Abstraction abstraction)
            {
                var x = abstraction.Variable;
                if (x.Name == cur)
                    return new Abstraction(new Variable(next), Go(cur, next, abstraction.Body));
                return new Abstraction(x, Go(cur, next, abstraction.Body));
            }
            
            Console.WriteLine("RENAME-GO: impossible case!");
            return null;
        }

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
                return Subst(x, s, Rename(y.Name, term));
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
            if (!reducedFunction.Equals(application.Function))
            {
                return new Application(reducedFunction, application.Argument);
            }
            
            IExpression reducedArgument = application.Argument.Accept(this);
            if (!reducedArgument.Equals(application.Argument))
            {
                return new Application(application.Function, reducedArgument);
                
            }
            Console.WriteLine("REDUCE APPLICATION: all parts are null!");
            return application;
        }
        
        public IExpression Visit(Abstraction abstraction)
        {
            IExpression reducedBody = abstraction.Body.Accept(this);

            if (!reducedBody.Equals(abstraction.Body))
            {
                return new Abstraction(abstraction.Variable, reducedBody);
            }
            Console.WriteLine("REDUCE ABSTRACTION: body is null!");
            return abstraction;
        }
        public IExpression Visit(Variable expression)
        {
            return expression;
        }
    }
}