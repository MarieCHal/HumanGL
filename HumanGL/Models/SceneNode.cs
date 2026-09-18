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

    // Couleur RGB du membre (valeurs de 0 à 1).
    // Blanc par défaut ; chaque membre possède sa propre couleur.
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
    /// T × R × S × T_pivot.
    /// Le pivot place le joint au bout du cube, pas au centre —
    /// sinon la moitié du bras remonterait dans la tête.
    /// </summary>
    public Matrix4x4 GetLocalMatrix()
    {
        return Matrix4x4.Translation(LocalPosition)
             * Matrix4x4.RotationXYZ(LocalRotation)
             * Matrix4x4.Scale(LocalScale)
             * Matrix4x4.Translation(JointPivot);
    }

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

        // Tailles VISIBLES (monde). Le scale du parent se multiplie avec l'enfant,
        // donc le LocalScale enfant = tailleWanted / scaleMondeParent.
        Vector3 torsoWorld = new(1.4f, 1.5f, 0.75f);
        Vector3 headWorld = new(0.55f, 0.55f, 0.55f);   // cube
        Vector3 armWorld = new(0.35f, 0.85f, 0.35f);     // bras = avant-bras (largeur)
        Vector3 forearmWorld = new(0.35f, 0.75f, 0.35f);
        Vector3 thighWorld = new(0.5f, 0.95f, 0.5f);     // cuisse = tibia (largeur)
        Vector3 calfWorld = new(0.5f, 0.9f, 0.5f);

        // Palette du personnage : modifier ces valeurs pour changer son apparence.
        Vector3 skinColor = new(1.0f, 0.77f, 0.65f);
        Vector3 shirtColor = new(0.13f, 0.53f, 0.13f);
        Vector3 pantsColor = new(0.04f, 0.37f, 0.64f);

        SceneNode torso = new("Torso", Vector3.Zero, Vector3.Zero, torsoWorld, center) { Color = shirtColor };

        torso.AddChild(new SceneNode(
            "Head", new Vector3(0f, 0.5f, 0f), Vector3.Zero,
            Divide(headWorld, torsoWorld), sitOnBottom) { Color = skinColor });

        // Accroché à l'extérieur du torse : bord (0.5) + demi-largeur du bras
        // (sinon la moitié du bras rentre dans le volume du torse).
        float shoulderX = 0.5f + (armWorld.X * 0.5f) / torsoWorld.X;

        SceneNode leftUpperArm = new(
            "LeftUpperArm", new Vector3(-shoulderX, 0.5f, 0f), Vector3.Zero,
            Divide(armWorld, torsoWorld), hangFromTop) { Color = skinColor };
        leftUpperArm.AddChild(new SceneNode(
            "LeftForearm", new Vector3(0f, -0.5f, 0f), Vector3.Zero,
            Divide(forearmWorld, armWorld), hangFromTop) { Color = skinColor });
        torso.AddChild(leftUpperArm);

        SceneNode rightUpperArm = new(
            "RightUpperArm", new Vector3(shoulderX, 0.5f, 0f), Vector3.Zero,
            Divide(armWorld, torsoWorld), hangFromTop) { Color = skinColor };
        rightUpperArm.AddChild(new SceneNode(
            "RightForearm", new Vector3(0f, -0.5f, 0f), Vector3.Zero,
            Divide(forearmWorld, armWorld), hangFromTop) { Color = skinColor });
        torso.AddChild(rightUpperArm);

        SceneNode leftThigh = new(
            "LeftThigh", new Vector3(-0.28f, -0.5f, 0f), Vector3.Zero,
            Divide(thighWorld, torsoWorld), hangFromTop) { Color = pantsColor };
        leftThigh.AddChild(new SceneNode(
            "LeftCalf", new Vector3(0f, -0.5f, 0f), Vector3.Zero,
            Divide(calfWorld, thighWorld), hangFromTop) { Color = pantsColor });
        torso.AddChild(leftThigh);

        SceneNode rightThigh = new(
            "RightThigh", new Vector3(0.28f, -0.5f, 0f), Vector3.Zero,
            Divide(thighWorld, torsoWorld), hangFromTop) { Color = pantsColor };
        rightThigh.AddChild(new SceneNode(
            "RightCalf", new Vector3(0f, -0.5f, 0f), Vector3.Zero,
            Divide(calfWorld, thighWorld), hangFromTop) { Color = pantsColor });
        torso.AddChild(rightThigh);

        return torso;
    }

    /// <summary>Scale local pour obtenir desiredWorld malgré le scale déjà appliqué par le parent.</summary>
    private static Vector3 Divide(Vector3 desiredWorld, Vector3 parentWorld) =>
        new(desiredWorld.X / parentWorld.X,
            desiredWorld.Y / parentWorld.Y,
            desiredWorld.Z / parentWorld.Z);
}
