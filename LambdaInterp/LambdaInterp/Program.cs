using System;
using System.Text;

namespace LambdaInterp
{
  interface IExpression
  {
    T Accept<T>(IVisitor<T> visitor);
  }
  interface IVisitor<T>
  {
    T Visit(Application expression);
    T Visit(Abstraction expression);
    T Visit(Variable expression);
  }
  class Application : IExpression
  {
    public IExpression Function { get; }
    public IExpression Argument { get; }
    public Application(IExpression function, IExpression argument)
    {
      Function = function;
      Argument = argument;
    }
    public T Accept<T>(IVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
    public override string ToString()
    {
      return Presenter.ToString(this);
    }
  }
  class Abstraction : IExpression
  {
    public Variable Variable { get; }
    public IExpression Body { get; }
    public Abstraction(Variable variable, IExpression body)
    {
      Variable = variable;
      Body = body;
    }
    public T Accept<T>(IVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
    public override string ToString()
    {
      return Presenter.ToString(this);
    }
  }
  class Variable : IExpression
  {
    public string Name { get; }
    public Variable(string name)
    {
      Name = name;
    }
    public T Accept<T>(IVisitor<T> visitor)
    {
      return visitor.Visit(this);
    }
    public override string ToString()
    {
      return Presenter.ToString(this);
    }
    protected bool Equals(Variable other)
    {
      return Name == other.Name;
    }
    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((Variable) obj);
    }
    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }
  }
  // class Presenter : IVisitor<StringBuilder>
  // {
  //   private readonly StringBuilder myBuilder;
  //   public Presenter(StringBuilder builder)
  //   {
  //     myBuilder = builder;
  //   }
  //   public StringBuilder Visit(Application expression)
  //   {
  //     if (expression.Function is Abstraction abstraction)
  //     {
  //       myBuilder.Append("let ");
  //       abstraction.Variable.Accept(this);
  //       myBuilder.Append(" = ");
  //       expression.Argument.Accept(this);
  //       myBuilder.Append(" in ");
  //       
  //       if (abstraction.Body is Application application && application.Function is Abstraction)
  //         myBuilder.AppendLine();
  //       return abstraction.Body.Accept(this);
  //     }
  //     else
  //     {
  //       expression.Function.Accept(this);
  //       myBuilder.Append("(");
  //       expression.Argument.Accept(this);
  //       return myBuilder.Append(")");
  //     }
  //   }
  //   public StringBuilder Visit(Abstraction expression)
  //   {
  //     myBuilder.Append("\\");
  //     expression.Variable.Accept(this);
  //     myBuilder.Append(" => ");
  //     return expression.Body.Accept(this);
  //   }
  //   public StringBuilder Visit(Variable expression)
  //   {
  //     return myBuilder.Append(expression.Name);
  //   }
  //   public static string ToString(IExpression expression)
  //   {
  //    return expression.Accept(new Presenter(new StringBuilder())).ToString();
  //   }
  // }
  
  class Presenter : IVisitor<StringBuilder>
  {
    private readonly StringBuilder myBuilder;
    public Presenter(StringBuilder builder)
    {
      myBuilder = builder;
    }
    public StringBuilder Visit(Application expression)
    {
      myBuilder.Append("(");
      myBuilder.Append(expression.Function);
      myBuilder.Append(" @ ");
      myBuilder.Append(expression.Argument);
      return myBuilder.Append(")");
    }
    public StringBuilder Visit(Abstraction expression)
    {
      myBuilder.Append("[\\");
      myBuilder.Append(expression.Variable);
      myBuilder.Append(" ");
      myBuilder.Append(expression.Body);
      myBuilder.Append("]");
      return myBuilder;
    }
    public StringBuilder Visit(Variable expression)
    {
      return myBuilder.Append(expression.Name);
    }
    public static string ToString(IExpression expression)
    {
     return expression.Accept(new Presenter(new StringBuilder())).ToString();
    }
  }
  class Program
  {

    static void Test(IExpression expr)
    {
      Console.WriteLine(Presenter.ToString(expr));

      var prevExpr = expr;
      do
      {
        prevExpr = expr;
        expr = expr.Accept(new Reducer());
        Console.WriteLine("\n" + Presenter.ToString(expr));
      } while (!prevExpr.Equals(expr));
      
      Console.WriteLine("\n============================================\n");
    }
    
    static void Main(string[] args)
    {
//      IExpression expr = new Application(new Abstraction(new Variable("zero"), new Application(new Abstraction(new Variable("one"), new Application(new Abstraction(new Variable("two"), new Application(new Abstraction(new Variable("succ"), new Application(new Abstraction(new Variable("plus"), new Application(new Abstraction(new Variable("mult"), new Application(new Abstraction(new Variable("pred"), new Application(new Abstraction(new Variable("true"), new Application(new Abstraction(new Variable("false"), new Application(new Abstraction(new Variable("if"), new Application(new Abstraction(new Variable("izzero"), new Application(new Abstraction(new Variable("fix"), new Application(new Abstraction(new Variable("factorial"), new Application(new Variable("factorial"), new Application(new Application(new Variable("plus"), new Application(new Application(new Variable("mult"), new Variable("two")), new Variable("two"))), new Variable("two")))), new Application(new Variable("fix"), new Abstraction(new Variable("f"), new Abstraction(new Variable("n"), new Application(new Application(new Application(new Variable("if"), new Application(new Variable("izzero"), new Variable("n"))), new Variable("one")), new Application(new Application(new Variable("mult"), new Variable("n")), new Application(new Variable("f"), new Application(new Variable("pred"), new Variable("n")))))))))), new Abstraction(new Variable("g"), new Application(new Abstraction(new Variable("x"), new Application(new Variable("g"), new Application(new Variable("x"), new Variable("x")))), new Abstraction(new Variable("x"), new Application(new Variable("g"), new Application(new Variable("x"), new Variable("x")))))))), new Abstraction(new Variable("n"), new Application(new Application(new Variable("n"), new Abstraction(new Variable("x"), new Variable("false"))), new Variable("true"))))), new Abstraction(new Variable("p"), new Abstraction(new Variable("x"), new Abstraction(new Variable("y"), new Application(new Application(new Variable("p"), new Variable("x")), new Variable("y"))))))), new Abstraction(new Variable("x"), new Abstraction(new Variable("y"), new Variable("y"))))), new Abstraction(new Variable("x"), new Abstraction(new Variable("y"), new Variable("x"))))), new Abstraction(new Variable("n"), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Application(new Application(new Application(new Variable("n"), new Abstraction(new Variable("g"), new Abstraction(new Variable("h"), new Application(new Variable("h"), new Application(new Variable("g"), new Variable("f")))))), new Abstraction(new Variable("u"), new Variable("x"))), new Abstraction(new Variable("u"), new Variable("u")))))))), new Abstraction(new Variable("m"), new Abstraction(new Variable("n"), new Application(new Application(new Variable("m"), new Application(new Variable("plus"), new Variable("n"))), new Variable("zero")))))), new Abstraction(new Variable("m"), new Abstraction(new Variable("n"), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Application(new Application(new Variable("n"), new Variable("f")), new Application(new Application(new Variable("m"), new Variable("f")), new Variable("x"))))))))), new Abstraction(new Variable("n"), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Application(new Variable("f"), new Application(new Application(new Variable("n"), new Variable("f")), new Variable("x")))))))), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Application(new Variable("f"), new Application(new Variable("f"), new Variable("x"))))))), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Application(new Variable("f"), new Variable("x")))))), new Abstraction(new Variable("f"), new Abstraction(new Variable("x"), new Variable("x"))));

      IExpression simpleTest = new Application(new Abstraction(new Variable("if"), new Application(new Application(new Application(new Variable("if"),new Abstraction(new Variable("x"),new Abstraction(new Variable("y"),new Variable("x")))),new Variable("a")),new Variable("b"))),new Abstraction(new Variable("p"),new Abstraction(new Variable("x"),new Abstraction(new Variable("y"),new Application(new Application(new Variable("p"),new Variable("x")),new Variable("y"))))));

      IExpression renamerTest = new Application(new Application(new Abstraction(new Variable("x"), new Abstraction(new Variable("y"), new Application(new Variable("y"), new Variable("x")))), new Variable("y")), new Variable("x"));
      
      Test(simpleTest);
      Test(renamerTest);

      ////////////////////////////
      Console.WriteLine("HURRAY");
      //////////////////////////
    }
  }
}