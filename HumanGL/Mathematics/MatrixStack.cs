namespace HumanGL.Mathematics;

/// <summary>
/// Pile de matrices : on empile pour descendre dans un membre,
/// on dépile pour revenir au parent sans garder le mouvement de l'enfant.
/// </summary>
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

    /// <summary>Copie la matrice du parent et l'empile, pour isoler la branche enfant.</summary>
    public void Push()
    {
        _stack.Add(GetCurrent());
    }

    /// <summary>Retire la matrice du membre courant pour revenir à celle du parent.</summary>
    public void Pop()
    {
        if (_stack.Count > 1)
            _stack.RemoveAt(_stack.Count - 1);
    }

    /// <summary>Applique la matrice locale du membre sur celle du parent (sommet × locale).</summary>
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
