public class Embeded {
    public static void Generate(Enviorment env) {
        env.Declare("time", new FunctionValue(new TimeEmbeded(), "time"), null);
        env.Declare("strconv.Atoi", new FunctionValue(new AtoiEmbeded(), "strconv.Atoi"), null);
        env.Declare("strconv.ParseFloat", new FunctionValue(new ParseFloatEmbeded(), "strconv.ParseFloat"), null);
        env.Declare("reflect.TypeOf", new FunctionValue(new TypeOfEmbeded(), "reflect.TypeOf"), null);
    }
}

public class TimeEmbeded : Invocable {
    public int Arity() {
        return 0;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, CompilerVisitor visitor) {
        return new StringValueWrapper(DateTime.Now.ToString());
    }
}

public class AtoiEmbeded() : Invocable {
    public int Arity() {
        return 1;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, CompilerVisitor visitor) {
        if (args[0] is StringValueWrapper str) {
            if (int.TryParse(str.Value, out int result)) {
                return new IntValueWrapper(result);
            }
            else {
                throw new SemanticError("strconv.Atoi: parsing error", null);
            }
        }
        else {
            throw new SemanticError("strconv.Atoi: argument must be a string", null);
        }
    }
}

public class ParseFloatEmbeded() : Invocable {
    public int Arity() {
        return 1;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, CompilerVisitor visitor) {
        if (args[0] is StringValueWrapper str) {
            if (float.TryParse(str.Value, out float result)) {
                return new FloatValueWrapper(result);
            }
            else {
                throw new SemanticError("strconv.ParseFloat: parsing error", null);
            }
        }
        else {
            throw new SemanticError("strconv.ParseFloat: argument must be a string", null);
        }
    }
}

public class TypeOfEmbeded() : Invocable {
    public int Arity() {
        return 1;
    }

    public ValueWrapper Invoke(List<ValueWrapper> args, CompilerVisitor visitor) {
        switch (args[0]) {
            case IntValueWrapper _:
                return new StringValueWrapper("int");
            case FloatValueWrapper _:
                return new StringValueWrapper("float");
            case StringValueWrapper _:
                return new StringValueWrapper("string");
            case BoolValueWrapper _:
                return new StringValueWrapper("bool");
            case RuneValueWrapper _:
                return new StringValueWrapper("rune");
            case ArrayValueWrapper _:
                return new StringValueWrapper("array");
            case FunctionValue _:
                return new StringValueWrapper("function");
            case VoidBody _:
                return new StringValueWrapper("void");
            default:
                return new StringValueWrapper("unknown");
        }
    }
}