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
    const float CrouchDuration = 0.28f; // fléchit les genoux avant de décoller

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

        // Coudes vers l'avant, genoux vers l'arrière (sens opposés).
        SetRotationX(root, "LeftForearm", -MathF.Abs(walkAngle) * 0.5f);
        SetRotationX(root, "RightForearm", -MathF.Abs(walkAngle) * 0.5f);

        SetRotationX(root, "LeftCalf", MathF.Abs(walkAngle) * 0.6f);
        SetRotationX(root, "RightCalf", MathF.Abs(walkAngle) * 0.6f);

        root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY, root.LocalPosition.Z);
    }

    private void UpdateJump(SceneNode root, float deltaTime)
    {
        JumpElapsed += deltaTime;

        // 1) Accroupissement au sol : genoux pliés, légère descente du torse
        if (JumpElapsed < CrouchDuration)
        {
            float crouch = JumpElapsed / CrouchDuration; // 0 → 1
            root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY - 0.2f * crouch, root.LocalPosition.Z);

            SetRotationX(root, "LeftThigh", -55f * crouch);
            SetRotationX(root, "RightThigh", -55f * crouch);
            SetRotationX(root, "LeftCalf", 75f * crouch);
            SetRotationX(root, "RightCalf", 75f * crouch);
            SetRotationX(root, "LeftUpperArm", 20f * crouch);
            SetRotationX(root, "RightUpperArm", 20f * crouch);
            // Coudes vers l'avant, opposés aux genoux
            SetRotationX(root, "LeftForearm", -60f * crouch);
            SetRotationX(root, "RightForearm", -60f * crouch);
            return;
        }

        // 2) Vol : chronomètre du décollage seulement (après le crouch)
        float flightTime = JumpElapsed - CrouchDuration;
        float y = GroundY + JumpImpulse * flightTime - 0.5f * Gravity * flightTime * flightTime;

        if (y <= GroundY)
        {
            root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY, root.LocalPosition.Z);
            JumpElapsed = 0f;
            State = AnimState.Idle;
            ResetPose(root);
            return;
        }

        root.LocalPosition = new Vector3(root.LocalPosition.X, y, root.LocalPosition.Z);
        SetRotationX(root, "LeftThigh", -35f);
        SetRotationX(root, "RightThigh", -35f);
        SetRotationX(root, "LeftCalf", 50f);
        SetRotationX(root, "RightCalf", 50f);
        SetRotationX(root, "LeftForearm", -45f);
        SetRotationX(root, "RightForearm", -45f);
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
