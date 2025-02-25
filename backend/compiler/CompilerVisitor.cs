using analyzer;

class CompilerVisitor : LanguageBaseVisitor<ValueWrapper>
{

    private ValueWrapper defaultValue = new VoidBody();
    public string output = "";
    private Enviorment env = new Enviorment();

    // VisitProgram
    public override ValueWrapper VisitProgram(LanguageParser.ProgramContext context)
    {
        foreach (var declaration in context.declaration())
        {
            Visit(declaration);
        }
        return defaultValue;
    }

    // VisitVarDeclaration
    public override ValueWrapper VisitVarDeclaration(LanguageParser.VarDeclarationContext context)
    {
        string name = context.ID().GetText();
        // si no tiene valor asignado, se le asigna un valor por defecto segun el tipo
        if(context.expr() == null)
        {
            string type = context.TYPE().GetText();
            ValueWrapper inferValue = type switch
            {
                "int" => new IntValueWrapper(0),
                "float64" => new FloatValueWrapper(0),
                "string" => new StringValueWrapper(""),
                "bool" => new BoolValueWrapper(false),
                "rune" => new RuneValueWrapper(' '),
                _ => throw new Exception("Invalid type")
            };
            env.Declare(name, inferValue);
            return defaultValue;
        } 
        ValueWrapper value = Visit(context.expr());
        env.Declare(name, value);
        return defaultValue;
    }

    // VisitExprStmt
    public override ValueWrapper VisitExprStmt(LanguageParser.ExprStmtContext context)
    {
        Visit(context.expr());
        return defaultValue;
    }

    // VisitPrintStmt
    public override ValueWrapper VisitPrintStmt(LanguageParser.PrintStmtContext context)
    {
        // | 'fmt.Println' '(' exprList ')' # PrintStmt
        // exprList: expr (',' expr)*;
        foreach (var expr in context.exprList().expr())
        {
            ValueWrapper value = Visit(expr);
            output += value switch
                {
                    IntValueWrapper intValue => intValue.Value.ToString() + " ",
                    FloatValueWrapper floatValue => floatValue.Value.ToString() + " ",
                    StringValueWrapper stringValue => stringValue.Value + " ",
                    BoolValueWrapper boolValue => boolValue.Value.ToString() + " ",
                    RuneValueWrapper runeValue => runeValue.Value.ToString() + " ",
                    _ => throw new Exception("Invalid value")
                };
        }
        output += "\n"; 
        return defaultValue;

    }


    // VisitBlockStmt
    public override ValueWrapper VisitBlockStmt(LanguageParser.BlockStmtContext context)
    {
        Enviorment prevEnv = env;
        env = new Enviorment(env);

        foreach (var statement in context.declaration())
        {
            Visit(statement);
        }

        env = prevEnv;
        return defaultValue;
    }

    // VisitIfStmt
    public override ValueWrapper VisitIfStmt(LanguageParser.IfStmtContext context)
    {
        ValueWrapper condition = Visit(context.expr());
        if (condition is BoolValueWrapper boolValue)
        {
            if (boolValue.Value)
            {
                Visit(context.statement(0));
            }
            else if (context.statement().Length > 1)
            {
                Visit(context.statement(1));
            }
        }
        else
        {
            throw new Exception("Invalid condition");
        }
        return defaultValue;
    }

        // VisitSwitchStmt
    public override ValueWrapper VisitSwitchStmt(LanguageParser.SwitchStmtContext context)
    {
        ValueWrapper switchValue = Visit(context.expr());
        bool caseMatched = false;

        foreach (var caseClause in context.caseClauseStmt())
        {
            if (caseClause is LanguageParser.CaseClauseContext caseClauseStmt)
            {
                ValueWrapper caseValue = Visit(caseClauseStmt.expr());
                if (switchValue.Equals(caseValue))
                {
                    caseMatched = true;
                    foreach (var declaration in caseClauseStmt.declaration())
                    {
                        Visit(declaration);
                    }
                    break; 
                }
            }
            else if (caseClause is LanguageParser.DefaultClauseContext defaultContext)
            {
                if (!caseMatched)
                {
                    foreach (var declaration in defaultContext.declaration())
                    {
                        Visit(declaration);
                    }
                }
            }
        }

        return defaultValue;
    }

    // VisitCaseClause
    public override ValueWrapper VisitCaseClause(LanguageParser.CaseClauseContext context)
    {
        return defaultValue;
    }

    // VisitDefaultClause
    public override ValueWrapper VisitDefaultClause(LanguageParser.DefaultClauseContext context)
    {
        return defaultValue;
    }
    

    // VisitNegate 
    public override ValueWrapper VisitNegate(LanguageParser.NegateContext context)
    {
        ValueWrapper value = Visit(context.expr());
        return value switch
        {
            IntValueWrapper intValue => new IntValueWrapper(-intValue.Value),
            FloatValueWrapper floatValue => new FloatValueWrapper(-floatValue.Value),
            _ => throw new Exception("Invalid value")
        };
    }

    // VisitFloat
    public override ValueWrapper VisitFloat(LanguageParser.FloatContext context)
    {
        return new FloatValueWrapper(float.Parse(context.GetText()));
    }

    // VisitIdentifier
    public override ValueWrapper VisitIdentifier(LanguageParser.IdentifierContext context)
    {
        string name = context.ID().GetText();
        return env.Get(name);
    }

    // VisitNumber
    public override ValueWrapper VisitNumber(LanguageParser.NumberContext context)
    {
        return new IntValueWrapper(int.Parse(context.GetText()));
    }

    //VisitRune
    public override ValueWrapper VisitRune(LanguageParser.RuneContext context)
    {
        return new RuneValueWrapper(context.GetText()[1]);
    }


    // VisitBool
    public override ValueWrapper VisitBool(LanguageParser.BoolContext context)
    {
        return new BoolValueWrapper(bool.Parse(context.GetText()));
    }


    // VisitMulDiv
    public override ValueWrapper VisitMulDiv(LanguageParser.MulDivContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;   

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "*") => new IntValueWrapper(leftInt.Value * rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "*") => new FloatValueWrapper(leftFloat.Value * rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "*") => new FloatValueWrapper(leftInt.Value * rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "*") => new FloatValueWrapper(leftFloat.Value * rightFloat.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "/") => new IntValueWrapper(leftInt.Value / rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "/") => new FloatValueWrapper(leftFloat.Value / rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "/") => new FloatValueWrapper(leftInt.Value / rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "/") => new FloatValueWrapper(leftFloat.Value / rightFloat.Value),
            _ => throw new Exception("Invalid operation")
        };

        
    }

    // VisitMod
    public override ValueWrapper VisitMod(LanguageParser.ModContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));

        return (left, right) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value % rightInt.Value),
            _ => throw new Exception("Invalid operation")
        };
    }

    // VisitAddSub
    public override ValueWrapper VisitAddSub(LanguageParser.AddSubContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "+") => new IntValueWrapper(leftInt.Value + rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "+") => new FloatValueWrapper(leftInt.Value + rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "+") => new FloatValueWrapper(leftFloat.Value + rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "+") => new FloatValueWrapper(leftFloat.Value + rightFloat.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "+") => new StringValueWrapper(leftString.Value + rightString.Value),
            (StringValueWrapper leftString, IntValueWrapper rightInt, "+") => new StringValueWrapper(leftString.Value + rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "-") => new IntValueWrapper(leftInt.Value - rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "-") => new FloatValueWrapper(leftFloat.Value - rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "-") => new FloatValueWrapper(leftInt.Value - rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "-") => new FloatValueWrapper(leftFloat.Value - rightFloat.Value),
            _ => throw new Exception("Invalid operation")
        };
    }

    // VisitParens  
    public override ValueWrapper VisitParens(LanguageParser.ParensContext context)
    {
        return Visit(context.expr());
    }

    // VisitRelational
    public override ValueWrapper VisitRelational(LanguageParser.RelationalContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        /*
            int op int -> bool
            float op float -> bool
            int op float -> bool
            float op int -> bool
            rune op rune -> bool
        */

        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "<") => new BoolValueWrapper(leftInt.Value < rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, ">") => new BoolValueWrapper(leftInt.Value > rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "<=") => new BoolValueWrapper(leftInt.Value <= rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, ">=") => new BoolValueWrapper(leftInt.Value >= rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "<") => new BoolValueWrapper(leftFloat.Value < rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, ">") => new BoolValueWrapper(leftFloat.Value > rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "<=") => new BoolValueWrapper(leftFloat.Value <= rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, ">=") => new BoolValueWrapper(leftFloat.Value >= rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "<") => new BoolValueWrapper(leftInt.Value < rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, ">") => new BoolValueWrapper(leftInt.Value > rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "<=") => new BoolValueWrapper(leftInt.Value <= rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, ">=") => new BoolValueWrapper(leftInt.Value >= rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "<") => new BoolValueWrapper(leftFloat.Value < rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, ">") => new BoolValueWrapper(leftFloat.Value > rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "<=") => new BoolValueWrapper(leftFloat.Value <= rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, ">=") => new BoolValueWrapper(leftFloat.Value >= rightInt.Value),
            _ => throw new Exception("Invalid operation")
            };
    }

    //VisitLogical
    public override ValueWrapper VisitLogical(LanguageParser.LogicalContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;

        return (left, right, op) switch
        {
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "&&") => new BoolValueWrapper(leftBool.Value && rightBool.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "||") => new BoolValueWrapper(leftBool.Value || rightBool.Value),
            _ => throw new Exception("Invalid operation")
        };
    }

    //VisitNot
    public override ValueWrapper VisitNot(LanguageParser.NotContext context)
    {
        ValueWrapper value = Visit(context.expr());
        return value switch
        {
            BoolValueWrapper boolValue => new BoolValueWrapper(!boolValue.Value),
            _ => throw new Exception("Invalid value")
        };
    }

    // VisitAssign
    public override ValueWrapper VisitAssign(LanguageParser.AssignContext context)
    {
        ValueWrapper value = Visit(context.expr(1));

        if (context.expr(0) is LanguageParser.AssignContext leftAssign)
        {
            value = VisitAssign(leftAssign);
        }
        else
        {
            string name = context.expr(0).GetText();
            env.Assign(name, value);
        }

        return value; 
    }


    // VisitString
    public override ValueWrapper VisitString(LanguageParser.StringContext context)
    {
        string text = context.GetText();
        string processedText = text.Substring(1, text.Length - 2); // Remueve las comillas inicial y final
        return new StringValueWrapper(processedText);
    }


    // VisitEquality
    public override ValueWrapper VisitEquality(LanguageParser.EqualityContext context)
    {
        ValueWrapper left = Visit(context.expr(0));
        ValueWrapper right = Visit(context.expr(1));
        var op = context.op.Text;


        return (left, right, op) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "==") => new BoolValueWrapper(leftInt.Value == rightInt.Value),
            (IntValueWrapper leftInt, IntValueWrapper rightInt, "!=") => new BoolValueWrapper(leftInt.Value != rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "==") => new BoolValueWrapper(leftFloat.Value == rightFloat.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat, "!=") => new BoolValueWrapper(leftFloat.Value != rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "==") => new BoolValueWrapper(leftInt.Value == rightFloat.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat, "!=") => new BoolValueWrapper(leftInt.Value != rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "==") => new BoolValueWrapper(leftFloat.Value == rightInt.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt, "!=") => new BoolValueWrapper(leftFloat.Value != rightInt.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "==") => new BoolValueWrapper(leftBool.Value == rightBool.Value),
            (BoolValueWrapper leftBool, BoolValueWrapper rightBool, "!=") => new BoolValueWrapper(leftBool.Value != rightBool.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "==") => new BoolValueWrapper(leftString.Value == rightString.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString, "!=") => new BoolValueWrapper(leftString.Value != rightString.Value),
            (RuneValueWrapper leftRune, RuneValueWrapper rightRune, "==") => new BoolValueWrapper(leftRune.Value == rightRune.Value),
            (RuneValueWrapper leftRune, RuneValueWrapper rightRune, "!=") => new BoolValueWrapper(leftRune.Value != rightRune.Value),
            _ => throw new Exception("Invalid operation")
        };
    }

    // VisitorAddAssign
    public override ValueWrapper VisitAddAssign(LanguageParser.AddAssignContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        ValueWrapper left = env.Get(name);

        ValueWrapper result = (left, value) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value + rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftInt.Value + rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt) => new FloatValueWrapper(leftFloat.Value + rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftFloat.Value + rightFloat.Value),
            (StringValueWrapper leftString, StringValueWrapper rightString) => new StringValueWrapper(leftString.Value + rightString.Value),
            (StringValueWrapper leftString, IntValueWrapper rightInt) => new StringValueWrapper(leftString.Value + rightInt.Value),
            _ => throw new Exception("Invalid operation")
        };

        env.Assign(name, result);
        return defaultValue;
    }

    // VisitSubAssign
    public override ValueWrapper VisitSubAssign(LanguageParser.SubAssignContext context)
    {
        string name = context.ID().GetText();
        ValueWrapper value = Visit(context.expr());
        ValueWrapper left = env.Get(name);

        ValueWrapper result = (left, value) switch
        {
            (IntValueWrapper leftInt, IntValueWrapper rightInt) => new IntValueWrapper(leftInt.Value - rightInt.Value),
            (IntValueWrapper leftInt, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftInt.Value - rightFloat.Value),
            (FloatValueWrapper leftFloat, IntValueWrapper rightInt) => new FloatValueWrapper(leftFloat.Value - rightInt.Value),
            (FloatValueWrapper leftFloat, FloatValueWrapper rightFloat) => new FloatValueWrapper(leftFloat.Value - rightFloat.Value),
            _ => throw new Exception("Invalid operation")
        };

        env.Assign(name, result);
        return defaultValue;
    }


}