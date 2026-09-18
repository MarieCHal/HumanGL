using HumanGL.Mathematics;
using HumanGL.Models;

namespace HumanGL.Animation;

public enum AnimState
{
    Idle,
    Walk,
    Jump
}

/// <summary>
/// Change la recette des membres selon le temps.
/// Ne dessine rien : juste des angles et une hauteur.
/// </summary>
public class Animator
{
    const float WalkSpeed = 5f;
    const float WalkAmplitude = 30f;
    const float JumpImpulse = 5f;
    const float Gravity = 9.81f;
    const float GroundY = 0f;

    public AnimState State { get; private set; } = AnimState.Idle;

    /// <summary>Secondes depuis le début du saut.</summary>
    public float JumpElapsed { get; private set; }

    public void SetState(AnimState state)
    {
        if (state == AnimState.Jump && State != AnimState.Jump)
            JumpElapsed = 0f;

        State = state;
    }

    /// <summary>
    /// time : temps global, pour la marche (sin).
    /// deltaTime : temps depuis la frame précédente, pour le saut (gravité).
    /// </summary>
    public void Update(SceneNode root, float time, float deltaTime)
    {
        switch (State)
        {
            case AnimState.Walk:
                UpdateWalk(root, time);
                break;
            case AnimState.Jump:
                UpdateJump(root, deltaTime);
                break;
            default:
                ResetPose(root);
                break;
        }
    }

    private static void UpdateWalk(SceneNode root, float time)
    {
        // sin oscille entre -1 et 1 → angle entre -30° et +30°
        float walkAngle = MathF.Sin(time * WalkSpeed) * WalkAmplitude;

        SetRotationX(root, "LeftUpperArm", walkAngle);
        SetRotationX(root, "RightUpperArm", -walkAngle);
        SetRotationX(root, "LeftThigh", -walkAngle);
        SetRotationX(root, "RightThigh", walkAngle);

        // Coudes un peu pliés, toujours dans le même sens
        SetRotationX(root, "LeftForearm", MathF.Abs(walkAngle) * 0.5f);
        SetRotationX(root, "RightForearm", MathF.Abs(walkAngle) * 0.5f);

        // Genoux : le mollet reste un peu en arrière par rapport à la cuisse
        SetRotationX(root, "LeftCalf", MathF.Abs(walkAngle) * 0.6f);
        SetRotationX(root, "RightCalf", MathF.Abs(walkAngle) * 0.6f);

        root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY, root.LocalPosition.Z);
    }

    private void UpdateJump(SceneNode root, float deltaTime)
    {
        JumpElapsed += deltaTime;

        // y(t) = y0 + v0*t - 0.5*g*t²
        float y = GroundY + JumpImpulse * JumpElapsed - 0.5f * Gravity * JumpElapsed * JumpElapsed;

        if (y <= GroundY)
        {
            root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY, root.LocalPosition.Z);
            JumpElapsed = 0f;
            State = AnimState.Idle;
            ResetPose(root);
            return;
        }

        root.LocalPosition = new Vector3(root.LocalPosition.X, y, root.LocalPosition.Z);
        SetRotationX(root, "LeftThigh", -45f);
        SetRotationX(root, "RightThigh", -45f);
    }

    private static void ResetPose(SceneNode node)
    {
        node.LocalRotation = Vector3.Zero;
        if (node.Name == "Torso")
            node.LocalPosition = Vector3.Zero;

        foreach (SceneNode child in node.Children)
            ResetPose(child);
    }

    /// <summary>
    /// Vector3 est un struct : on ne peut pas écrire node.LocalRotation.X = …,
    /// ça modifierait une copie. On remplace tout le vecteur.
    /// </summary>
    private static void SetRotationX(SceneNode root, string name, float degrees)
    {
        SceneNode? node = root.FindNode(name);
        if (node == null)
            return;

        node.LocalRotation = new Vector3(degrees, node.LocalRotation.Y, node.LocalRotation.Z);
    }
}
