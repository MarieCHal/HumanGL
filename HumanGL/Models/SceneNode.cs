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
    public List<SceneNode> Children { get; } = new();

    public SceneNode(string name, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Name = name;
        LocalPosition = position;
        LocalRotation = rotation;
        LocalScale = scale;
    }

    /// <summary>
    /// Matrice locale : T × R × S.
    /// Appliquée à un sommet : d'abord la taille, puis la rotation, puis le déplacement.
    /// </summary>
    public Matrix4x4 GetLocalMatrix()
    {
        return Matrix4x4.Translation(LocalPosition)
             * Matrix4x4.RotationXYZ(LocalRotation)
             * Matrix4x4.Scale(LocalScale);
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

    /// <summary>
    /// Cube centré : il va de -0.5 à +0.5.
    /// On accroche les enfants au bord (0.5), pas à la taille du parent.
    /// Le scale du parent, déjà dans la pile, les écarte tout seul si on allonge un membre.
    /// </summary>
    public static SceneNode InitCharacter()
    {
        SceneNode torso = new("Torso", Vector3.Zero, Vector3.Zero, new Vector3(1.5f, 2.0f, 1.0f));

        torso.AddChild(new SceneNode("Head", new Vector3(0f, 0.5f, 0f), Vector3.Zero, new Vector3(0.8f, 0.8f, 0.8f)));

        SceneNode leftUpperArm = new("LeftUpperArm", new Vector3(-0.5f, 0.35f, 0f), Vector3.Zero, new Vector3(0.4f, 1.0f, 0.4f));
        leftUpperArm.AddChild(new SceneNode("LeftForearm", new Vector3(0f, -0.5f, 0f), Vector3.Zero, new Vector3(0.35f, 0.9f, 0.35f)));
        torso.AddChild(leftUpperArm);

        SceneNode rightUpperArm = new("RightUpperArm", new Vector3(0.5f, 0.35f, 0f), Vector3.Zero, new Vector3(0.4f, 1.0f, 0.4f));
        rightUpperArm.AddChild(new SceneNode("RightForearm", new Vector3(0f, -0.5f, 0f), Vector3.Zero, new Vector3(0.35f, 0.9f, 0.35f)));
        torso.AddChild(rightUpperArm);

        SceneNode leftThigh = new("LeftThigh", new Vector3(-0.15f, -0.5f, 0f), Vector3.Zero, new Vector3(0.5f, 1.2f, 0.5f));
        leftThigh.AddChild(new SceneNode("LeftCalf", new Vector3(0f, -0.5f, 0f), Vector3.Zero, new Vector3(0.45f, 1.1f, 0.45f)));
        torso.AddChild(leftThigh);

        SceneNode rightThigh = new("RightThigh", new Vector3(0.15f, -0.5f, 0f), Vector3.Zero, new Vector3(0.5f, 1.2f, 0.5f));
        rightThigh.AddChild(new SceneNode("RightCalf", new Vector3(0f, -0.5f, 0f), Vector3.Zero, new Vector3(0.45f, 1.1f, 0.45f)));
        torso.AddChild(rightThigh);

        return torso;
    }
}
