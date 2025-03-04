public class Enviorment
{
    private Dictionary<string, ValueWrapper> variables = new Dictionary<string, ValueWrapper>();

    private Enviorment? parent;

    public Enviorment(Enviorment? parent = null)
    {
        this.parent = parent;
    }

    public ValueWrapper Get(string name, Antlr4.Runtime.IToken token)
    
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else if (parent != null)
        {
            return parent.Get(name, token);
        }
        else
        {
            throw new SemanticError($"Variable {name} not found", token);
        }
    }

    public void Declare(string name, ValueWrapper value,  Antlr4.Runtime.IToken token)
    {
        if (variables.ContainsKey(name))
        {
            throw new SemanticError($"Variable {name} already declared", token);
        }
        variables[name] = value;
    }

    public ValueWrapper Assign(string name, ValueWrapper value, Antlr4.Runtime.IToken token)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return value;
        }
        else if (parent != null)
        {
            return parent.Assign(name, value, token);
        }
        else
        {
            throw new SemanticError($"Variable {name} not found", token);
        }
    }  


}