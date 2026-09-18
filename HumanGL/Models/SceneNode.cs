using HumanGL.Mathematics;

namespace HumanGL.Models;

/// <summary>
/// Un membre du bonhomme. On stocke ici la recette (position, rotation, scale),
/// pas la matrice. La matrice est recalculée à partir de ces trois vecteurs.
/// </summary>
public class SceneNode
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
    public List<SceneNode> Children { get; } = new();

    public SceneNode(string name, Vector3 position, Vector3 rotation, Vector3 scale, Vector3 jointPivot)
    {
        Name = name;
        LocalPosition = position;
        LocalRotation = rotation;
        LocalScale = scale;
        JointPivot = jointPivot;
    }

    /// <summary>
    /// Articulation seule : T × R.
    /// Le scale ne passe PAS aux enfants — sinon une rotation déforme / « grossit » le membre.
    /// </summary>
    public Matrix4x4 GetJointMatrix()
    {
        return Matrix4x4.Translation(LocalPosition)
             * Matrix4x4.RotationXYZ(LocalRotation);
    }

    /// <summary>Forme du cube : S × T_pivot (uniquement pour le dessin).</summary>
    public Matrix4x4 GetGeometryMatrix()
    {
        return Matrix4x4.Scale(LocalScale)
             * Matrix4x4.Translation(JointPivot);
    }

    /// <summary>Matrice locale complète d'un membre isolé : T × R × S × T_pivot.</summary>
    public Matrix4x4 GetLocalMatrix() => GetJointMatrix() * GetGeometryMatrix();

    public void AddChild(SceneNode child) => Children.Add(child);

    public SceneNode? FindNode(string nameToFind)
    {
        if (Name == nameToFind)
            return this;

        foreach (SceneNode child in Children)
        {
            SceneNode? found = child.FindNode(nameToFind);
            if (found != null)
                return found;
        }

        return null;
    }

    public static SceneNode InitCharacter()
    {
        Vector3 center = Vector3.Zero;
        Vector3 hangFromTop = new(0f, -0.5f, 0f);
        Vector3 sitOnBottom = new(0f, 0.5f, 0f);

        // Tailles monde directes : plus besoin de compenser, le scale n'est plus dans la pile.
        Vector3 torso = new(1.4f, 1.5f, 0.75f);
        Vector3 head = new(0.55f, 0.55f, 0.55f);
        Vector3 arm = new(0.28f, 0.85f, 0.28f);
        Vector3 forearm = new(0.28f, 0.75f, 0.28f);
        Vector3 thigh = new(0.5f, 0.95f, 0.5f);
        Vector3 calf = new(0.5f, 0.9f, 0.5f);

        Vector3 skinColor = new(0.65f, 0.40f, 0.28f);
        Vector3 shirtColor = new(1.0f, 0.40f, 0.70f);
        Vector3 pantsColor = new(0.0f, 0.85f, 0.90f);

        SceneNode torsoNode = new("Torso", Vector3.Zero, Vector3.Zero, torso, center) { Color = shirtColor };

        // Positions dans l'espace d'articulation du parent (sans son scale).
        torsoNode.AddChild(new SceneNode(
            "Head", new Vector3(0f, torso.Y * 0.5f, 0f), Vector3.Zero, head, sitOnBottom) { Color = skinColor });

        float shoulderX = torso.X * 0.5f + arm.X * 0.5f;
        float shoulderY = torso.Y * 0.38f;

        SceneNode leftUpperArm = new(
            "LeftUpperArm", new Vector3(-shoulderX, shoulderY, 0f), Vector3.Zero, arm, hangFromTop) { Color = skinColor };
        leftUpperArm.AddChild(new SceneNode(
            "LeftForearm", new Vector3(0f, -arm.Y, 0f), Vector3.Zero, forearm, hangFromTop) { Color = skinColor });
        torsoNode.AddChild(leftUpperArm);

        SceneNode rightUpperArm = new(
            "RightUpperArm", new Vector3(shoulderX, shoulderY, 0f), Vector3.Zero, arm, hangFromTop) { Color = skinColor };
        rightUpperArm.AddChild(new SceneNode(
            "RightForearm", new Vector3(0f, -arm.Y, 0f), Vector3.Zero, forearm, hangFromTop) { Color = skinColor });
        torsoNode.AddChild(rightUpperArm);

        float hipX = 0.31f;
        SceneNode leftThigh = new(
            "LeftThigh", new Vector3(-hipX, -torso.Y * 0.5f, 0f), Vector3.Zero, thigh, hangFromTop) { Color = pantsColor };
        leftThigh.AddChild(new SceneNode(
            "LeftCalf", new Vector3(0f, -thigh.Y, 0f), Vector3.Zero, calf, hangFromTop) { Color = pantsColor });
        torsoNode.AddChild(leftThigh);

        SceneNode rightThigh = new(
            "RightThigh", new Vector3(hipX, -torso.Y * 0.5f, 0f), Vector3.Zero, thigh, hangFromTop) { Color = pantsColor };
        rightThigh.AddChild(new SceneNode(
            "RightCalf", new Vector3(0f, -thigh.Y, 0f), Vector3.Zero, calf, hangFromTop) { Color = pantsColor });
        torsoNode.AddChild(rightThigh);

        return torsoNode;
    }
}
