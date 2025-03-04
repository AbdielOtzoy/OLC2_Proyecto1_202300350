public abstract record ValueWrapper;

public record IntValueWrapper(int Value) : ValueWrapper;

public record FloatValueWrapper(float Value) : ValueWrapper;

public record StringValueWrapper(string Value) : ValueWrapper;

public record BoolValueWrapper(bool Value) : ValueWrapper;

public record RuneValueWrapper(char Value) : ValueWrapper;

public record ArrayValueWrapper(ValueWrapper[] Value) : ValueWrapper;

public record VoidBody : ValueWrapper;