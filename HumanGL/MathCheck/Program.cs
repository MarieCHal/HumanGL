using HumanGL.Animation;
using HumanGL.Mathematics;
using HumanGL.Models;

int failed = 0;

Check("Identité : 1 sur la diagonale", () =>
{
    Matrix4x4 id = Matrix4x4.Identity();
    return Near(id.M[0, 0], 1f) && Near(id.M[1, 1], 1f)
        && Near(id.M[2, 2], 1f) && Near(id.M[3, 3], 1f)
        && Near(id.M[0, 3], 0f);
});

Check("Translation : le déplacement est dans la 4e colonne", () =>
{
    Matrix4x4 t = Matrix4x4.Translation(new Vector3(2f, 3f, 4f));
    return Near(t.M[0, 3], 2f) && Near(t.M[1, 3], 3f) && Near(t.M[2, 3], 4f);
});

Check("Scale : la taille est sur la diagonale", () =>
{
    Matrix4x4 s = Matrix4x4.Scale(new Vector3(0.4f, 1f, 0.4f));
    return Near(s.M[0, 0], 0.4f) && Near(s.M[1, 1], 1f) && Near(s.M[2, 2], 0.4f);
});

Check("RotationX(90°) : cos = 0, sin = 1", () =>
{
    Matrix4x4 r = Matrix4x4.RotationX(90f);
    return Near(r.M[1, 1], 0f) && Near(r.M[1, 2], -1f)
        && Near(r.M[2, 1], 1f) && Near(r.M[2, 2], 0f);
});

Check("T × S ≠ S × T", () =>
{
    Matrix4x4 t = Matrix4x4.Translation(new Vector3(1f, 0f, 0f));
    Matrix4x4 s = Matrix4x4.Scale(new Vector3(2f, 2f, 2f));
    Matrix4x4 ts = t * s;
    Matrix4x4 st = s * t;
    // T × S garde le déplacement de 1. S × T le double.
    return Near(ts.M[0, 3], 1f) && Near(st.M[0, 3], 2f);
});

Check("Pile : Push isole l'enfant, Pop revient au parent", () =>
{
    var stack = new MatrixStack();
    stack.Push();
    stack.Multiply(Matrix4x4.Translation(new Vector3(0f, 2f, 0f)));
    bool childMoved = Near(stack.GetCurrent().M[1, 3], 2f);
    stack.Pop();
    bool backToParent = Near(stack.GetCurrent().M[1, 3], 0f);
    return childMoved && backToParent;
});

Check("Squelette : torse, avant-bras trouvé, matrice locale TRS", () =>
{
    SceneNode torso = SceneNode.InitCharacter();
    SceneNode? forearm = torso.FindNode("LeftForearm");
    Matrix4x4 local = torso.GetLocalMatrix();
    return torso.Children.Count == 5
        && forearm != null
        && Near(local.M[0, 0], 1.5f)
        && Near(local.M[1, 1], 2f);
});

Check("Marche : bras opposés, angle de 30° au sommet du sinus", () =>
{
    SceneNode torso = SceneNode.InitCharacter();
    var animator = new Animator();
    animator.SetState(AnimState.Walk);
    // sin(time * 5) = 1 quand time * 5 = π/2
    animator.Update(torso, MathF.PI / 2f / 5f, 0.016f);

    SceneNode? left = torso.FindNode("LeftUpperArm");
    SceneNode? right = torso.FindNode("RightUpperArm");
    return left != null && right != null
        && Near(left.LocalRotation.X, 30f)
        && Near(right.LocalRotation.X, -30f);
});

Check("Saut : le torse monte, puis retombe et revient en idle", () =>
{
    SceneNode torso = SceneNode.InitCharacter();
    var animator = new Animator();
    animator.SetState(AnimState.Jump);
    animator.Update(torso, 0f, 0.1f);
    bool goingUp = torso.LocalPosition.Y > 0.4f;

    animator.Update(torso, 0f, 2f);
    bool landed = animator.State == AnimState.Idle && Near(torso.LocalPosition.Y, 0f);
    return goingUp && landed;
});

Console.WriteLine();
Console.WriteLine(failed == 0
    ? "Tout est OK."
    : $"{failed} test(s) en échec.");
return failed == 0 ? 0 : 1;

void Check(string name, Func<bool> test)
{
    bool ok = test();
    if (!ok)
        failed++;
    Console.WriteLine($"{(ok ? "OK  " : "ECHEC")}  {name}");
}

static bool Near(float a, float b) => MathF.Abs(a - b) < 0.001f;
