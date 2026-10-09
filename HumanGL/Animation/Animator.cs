using HumanGL.Mathematics;
using HumanGL.Models;

namespace HumanGL.Animation;

public enum AnimState
{
    Idle,
    Walk,
    Jump
}

public class Animator
{
    const float WalkSpeed = 5f;
    const float WalkAmplitude = 30f;
    const float JumpImpulse = 5f;
    const float Gravity = 9.81f;
    const float GroundY = 0f;
    const float CrouchDuration = 0.28f;

    public AnimState State { get; private set; } = AnimState.Idle;

    public float JumpElapsed { get; private set; }

    public void SetState(AnimState state)
    {
        if (state == AnimState.Jump && State != AnimState.Jump)
            JumpElapsed = 0f;

        State = state;
    }

    public void Update(Limb root, float time, float deltaTime)
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

    private static void UpdateWalk(Limb root, float time)
    {

        float walkAngle = MathF.Sin(time * WalkSpeed) * WalkAmplitude;

        SetRotationX(root, "LeftUpperArm", walkAngle);
        SetRotationX(root, "RightUpperArm", -walkAngle);
        SetRotationX(root, "LeftThigh", -walkAngle);
        SetRotationX(root, "RightThigh", walkAngle);

        SetRotationX(root, "LeftForearm", -MathF.Abs(walkAngle) * 0.5f);
        SetRotationX(root, "RightForearm", -MathF.Abs(walkAngle) * 0.5f);

        SetRotationX(root, "LeftCalf", MathF.Abs(walkAngle) * 0.6f);
        SetRotationX(root, "RightCalf", MathF.Abs(walkAngle) * 0.6f);

        root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY, root.LocalPosition.Z);
    }

    private void UpdateJump(Limb root, float deltaTime)
    {
        JumpElapsed += deltaTime;

        if (JumpElapsed < CrouchDuration)
        {
            float crouch = JumpElapsed / CrouchDuration;
            root.LocalPosition = new Vector3(root.LocalPosition.X, GroundY - 0.2f * crouch, root.LocalPosition.Z);

            SetRotationX(root, "LeftThigh", -55f * crouch);
            SetRotationX(root, "RightThigh", -55f * crouch);
            SetRotationX(root, "LeftCalf", 75f * crouch);
            SetRotationX(root, "RightCalf", 75f * crouch);
            SetRotationX(root, "LeftUpperArm", 20f * crouch);
            SetRotationX(root, "RightUpperArm", 20f * crouch);

            SetRotationX(root, "LeftForearm", -60f * crouch);
            SetRotationX(root, "RightForearm", -60f * crouch);
            return;
        }

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

    private static void ResetPose(Limb node)
    {
        node.LocalRotation = Vector3.Zero;
        if (node.Name == "Torso")
            node.LocalPosition = Vector3.Zero;

        foreach (Limb child in node.Children)
            ResetPose(child);
    }

    private static void SetRotationX(Limb root, string name, float degrees)
    {
        Limb? node = root.FindNode(name);
        if (node == null)
            return;

        node.LocalRotation = new Vector3(degrees, node.LocalRotation.Y, node.LocalRotation.Z);
    }
}
