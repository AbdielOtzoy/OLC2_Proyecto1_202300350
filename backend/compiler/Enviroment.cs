public class Enviorment
{
    private Dictionary<string, ValueWrapper> variables = new Dictionary<string, ValueWrapper>();

    private Enviorment? parent;

    public Enviorment(Enviorment? parent = null)
    {
        this.parent = parent;
    }

    public ValueWrapper Get(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name];
        }
        else if (parent != null)
        {
            return parent.Get(name);
        }
        else
        {
            throw new Exception($"Variable {name} not found");
        }
    }

    public void Declare(string name, ValueWrapper value)
    {
        if (variables.ContainsKey(name))
        {
            throw new Exception($"Variable {name} already declared");
        }
        variables[name] = value;
    }

    public ValueWrapper Assign(string name, ValueWrapper value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return value;
        }
        else if (parent != null)
        {
            return parent.Assign(name, value);
        }
        else
        {
            throw new Exception($"Variable {name} not found");
        }
    }


}