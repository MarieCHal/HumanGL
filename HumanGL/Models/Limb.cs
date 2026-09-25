using HumanGL.Mathematics;

namespace HumanGL.Models;

/// <summary>
/// Un membre du bonhomme. On stocke ici la recette (position, rotation, scale),
/// pas la matrice. La matrice est recalculée à partir de ces trois vecteurs.
/// </summary>
public class Limb
{
    public string Name { get; set; }
    public Vector3 LocalPosition { get; set; }
    public Vector3 LocalRotation { get; set; }
    public Vector3 LocalScale { get; set; }

    /// <summary>
    /// Décalage du cube unitaire pour placer l'articulation.
    /// (0, -0.5, 0) = joint en haut du membre (bras, jambe).
    /// (0, +0.5, 0) = joint en bas (tête sur le cou).
    /// (0, 0, 0) = centré (torse).
    /// </summary>
    public Vector3 JointPivot { get; set; }

    public Vector3 Color { get; set; } = new(1f, 1f, 1f);
    public List<Limb> Children { get; } = new();

    public Limb(string name, Vector3 position, Vector3 rotation, Vector3 scale, Vector3 jointPivot)
    {
        Name = name;
        LocalPosition = position;
        LocalRotation = rotation;
        LocalScale = scale;
        JointPivot = jointPivot;
    }

    /// <summary>
    /// Matrice poussée sur la pile : T × R × S.
    /// Le scale est dans la stack → les enfants héritent de la taille du parent.
    /// Le pivot géométrie est appliqué seulement au dessin.
    /// </summary>
    public Matrix4x4 GetStackMatrix()
    {
        return Matrix4x4.Translation(LocalPosition)
             * Matrix4x4.RotationXYZ(LocalRotation)
             * Matrix4x4.Scale(LocalScale);
    }

    public Matrix4x4 GetPivotMatrix() => Matrix4x4.Translation(JointPivot);

    public void AddChild(Limb child) => Children.Add(child);

    public Limb? FindNode(string nameToFind)
    {
        if (Name == nameToFind)
            return this;

        foreach (Limb child in Children)
        {
            Limb? found = child.FindNode(nameToFind);
            if (found != null)
                return found;
        }

        return null;
    }

    /// <summary>
    /// Resize runtime : membre + delta (+/−) sur X, Y, Z.
    /// Grâce au S dans la pile, les enfants grossissent et se repositionnent tout seuls
    /// (leurs offsets sont en espace unitaire, ex. avant-bras à y = -1).
    /// </summary>
    public bool ScaleMember(string memberName, float deltaUnits)
    {
        Limb? node = FindNode(memberName);
        if (node == null)
            return false;

        node.LocalScale = new Vector3(
            ClampSize(node.LocalScale.X + deltaUnits),
            ClampSize(node.LocalScale.Y + deltaUnits),
            ClampSize(node.LocalScale.Z + deltaUnits));

        // Épaules / hanches : ratios qui dépendent des largeurs relatives.
        RefreshUnitAnchors();
        return true;
    }

    /// <summary>
    /// Accroches en espace UNITAIRE du parent (avant son scale).
    /// Ex. avant-bras à (0,-1,0) : le S du bras l'envoie au bout et l'échelle.
    /// </summary>
    public void RefreshUnitAnchors()
    {
        foreach (Limb child in Children)
        {
            child.LocalPosition = ComputeUnitAnchor(child);
            child.RefreshUnitAnchors();
        }
    }

    private Vector3 ComputeUnitAnchor(Limb child)
    {
        return child.Name switch
        {
            "Head" => new Vector3(0f, 0.5f, 0f),

            "LeftUpperArm" => new Vector3(
                -(0.5f + 0.5f * child.LocalScale.X),
                0.38f,
                0f),
            "RightUpperArm" => new Vector3(
                0.5f + 0.5f * child.LocalScale.X,
                0.38f,
                0f),

            "LeftThigh" => new Vector3(-0.22f, -0.5f, 0f),
            "RightThigh" => new Vector3(0.22f, -0.5f, 0f),

            "LeftForearm" or "RightForearm" or "LeftCalf" or "RightCalf"
                => new Vector3(0f, -1f, 0f),

            _ => child.LocalPosition
        };
    }

    private static float ClampSize(float value) => Math.Clamp(value, 0.15f, 4f);

    public static Limb InitCharacter()
    {
        Vector3 center = Vector3.Zero;
        Vector3 hangFromTop = new(0f, -0.5f, 0f);
        Vector3 sitOnBottom = new(0f, 0.5f, 0f);

        // Scales locaux : le S parent se multiplie avec l'enfant dans la pile.
        // Pour une taille visible V sous un parent de scale P : local = V / P (composante par composante).
        Vector3 torso = new(1.4f, 1.5f, 0.75f);
        Vector3 head = Divide(new Vector3(0.55f, 0.55f, 0.55f), torso);
        Vector3 arm = Divide(new Vector3(0.28f, 0.85f, 0.28f), torso);
        Vector3 forearm = Divide(new Vector3(0.28f, 0.75f, 0.28f), new Vector3(0.28f, 0.85f, 0.28f));
        Vector3 thigh = Divide(new Vector3(0.5f, 0.95f, 0.5f), torso);
        Vector3 calf = Divide(new Vector3(0.5f, 0.9f, 0.5f), new Vector3(0.5f, 0.95f, 0.5f));

        Vector3 skinColor = new(0.65f, 0.40f, 0.28f);
        Vector3 shirtColor = new(1.0f, 0.40f, 0.70f);
        Vector3 pantsColor = new(0.0f, 0.85f, 0.90f);

        Limb torsoNode = new("Torso", Vector3.Zero, Vector3.Zero, torso, center) { Color = shirtColor };

        torsoNode.AddChild(new Limb("Head", Vector3.Zero, Vector3.Zero, head, sitOnBottom) { Color = skinColor });

        Limb leftUpperArm = new("LeftUpperArm", Vector3.Zero, Vector3.Zero, arm, hangFromTop) { Color = skinColor };
        leftUpperArm.AddChild(new Limb("LeftForearm", Vector3.Zero, Vector3.Zero, forearm, hangFromTop) { Color = skinColor });
        torsoNode.AddChild(leftUpperArm);

        Limb rightUpperArm = new("RightUpperArm", Vector3.Zero, Vector3.Zero, arm, hangFromTop) { Color = skinColor };
        rightUpperArm.AddChild(new Limb("RightForearm", Vector3.Zero, Vector3.Zero, forearm, hangFromTop) { Color = skinColor });
        torsoNode.AddChild(rightUpperArm);

        Limb leftThigh = new("LeftThigh", Vector3.Zero, Vector3.Zero, thigh, hangFromTop) { Color = pantsColor };
        leftThigh.AddChild(new Limb("LeftCalf", Vector3.Zero, Vector3.Zero, calf, hangFromTop) { Color = pantsColor });
        torsoNode.AddChild(leftThigh);

        Limb rightThigh = new("RightThigh", Vector3.Zero, Vector3.Zero, thigh, hangFromTop) { Color = pantsColor };
        rightThigh.AddChild(new Limb("RightCalf", Vector3.Zero, Vector3.Zero, calf, hangFromTop) { Color = pantsColor });
        torsoNode.AddChild(rightThigh);

        torsoNode.RefreshUnitAnchors();
        return torsoNode;
    }

    private static Vector3 Divide(Vector3 desiredWorld, Vector3 parentWorld) =>
        new(desiredWorld.X / parentWorld.X,
            desiredWorld.Y / parentWorld.Y,
            desiredWorld.Z / parentWorld.Z);
}
