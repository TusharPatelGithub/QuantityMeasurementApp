namespace ModelLayer.Enums;

public sealed class ArithmeticOperation
{
    public static readonly ArithmeticOperation ADD      = new ArithmeticOperation("ADD",      (a, b) => a + b);
    public static readonly ArithmeticOperation SUBTRACT = new ArithmeticOperation("SUBTRACT", (a, b) => a - b);
    public static readonly ArithmeticOperation DIVIDE   = new ArithmeticOperation("DIVIDE",   (a, b) =>
    {
        if (b == 0.0)
            throw new ArithmeticException("Cannot divide by zero.");
        return a / b;
    });

    private readonly string name;
    private readonly Func<double, double, double> operation;

    private ArithmeticOperation(string name, Func<double, double, double> operation)
    {
        this.name      = name;
        this.operation = operation;
    }

    public double Compute(double left, double right) => this.operation(left, right);
    public override string ToString()                => this.name;
}
