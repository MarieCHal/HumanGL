namespace HumanGL.Mathematics;

public class MatrixStack
{
    private readonly List<Matrix4x4> _stack = new();

    public MatrixStack()
    {
        Init();
    }

    public void Init()
    {
        _stack.Clear();
        _stack.Add(Matrix4x4.Identity());
    }

    public void Push()
    {
        _stack.Add(GetCurrent());
    }

    public void Pop()
    {
        if (_stack.Count > 1)
            _stack.RemoveAt(_stack.Count - 1);
    }

    public void Multiply(Matrix4x4 localMatrix)
    {
        int top = _stack.Count - 1;
        _stack[top] = _stack[top] * localMatrix;
    }

    public Matrix4x4 GetCurrent()
    {
        return _stack[_stack.Count - 1];
    }
}
