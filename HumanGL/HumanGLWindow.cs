using HumanGL.Animation;
using HumanGL.Mathematics;
using HumanGL.Models;
using HumanGL.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HumanGL;

public class HumanGLWindow : GameWindow
{
    private CubeRenderer? _cube;
    private Limb? _character;
    private readonly Animator _animator = new();
    private readonly MatrixStack _stack = new();
    private readonly OrbitCamera _camera = new();
    private readonly float[] _projection = new float[16];
    private float _time;
    private const float ScaleStep = 0.15f;
    private static readonly string[] ScaleTargets =
    [
        "Head", "Torso", "LeftUpperArm", "LeftForearm",
        "LeftThigh", "LeftCalf", "RightUpperArm", "RightForearm"
    ];
    private int _scaleTargetIndex = 2;
    private string ScaleTarget => ScaleTargets[_scaleTargetIndex];

    public HumanGLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "HumanGL",
        //TODO
        ClientSize = new Vector2i(800, 600),
        APIVersion = new Version(4, 1),
        Profile = ContextProfile.Core,
        Flags = ContextFlags.ForwardCompatible
    })
    {
    }

    protected override void OnLoad()
    {
        // Appelle la méthode de GameWindow, qui déclenche l'événement Load.
        base.OnLoad();
        VSync = VSyncMode.On;
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        // Les surfaces proches cachent les surfaces situées derrière elles.
        GL.Enable(EnableCap.DepthTest);
        UpdateProjection(FramebufferSize.X, FramebufferSize.Y);

        _cube = new CubeRenderer();
        _character = Limb.InitCharacter();
        PrintControls();
    }

    private static void PrintControls()
    {
        Console.WriteLine("HumanGL — cliquer dans la fenêtre pour le focus");
        Console.WriteLine("  I        repos");
        Console.WriteLine("  W        marche");
        Console.WriteLine("  Espace   saut");
        Console.WriteLine("  T        membre suivant (resize)");
        Console.WriteLine("  R        agrandir");
        Console.WriteLine("  F        réduire");
        Console.WriteLine("  Flèches  tourner la caméra");
        Console.WriteLine("  Échap    quitter");
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (!IsFocused)
            return;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
            return;
        }

        HandleAnimationInput();
        HandleMemberScaling();
        UpdateAnimation((float)args.Time);
        HandleCameraInput(args.Time);
    }

    private void HandleAnimationInput()
    {
        // IsKeyPressed réagit une seule fois à chaque appui.
        if (KeyboardState.IsKeyPressed(Keys.I))
            _animator.SetState(AnimState.Idle);
        if (KeyboardState.IsKeyPressed(Keys.W))
            _animator.SetState(AnimState.Walk);
        if (KeyboardState.IsKeyPressed(Keys.Space))
            _animator.SetState(AnimState.Jump);

    }

    private void HandleMemberScaling()
    {
        if (KeyboardState.IsKeyPressed(Keys.T))
        {
            _scaleTargetIndex = (_scaleTargetIndex + 1) % ScaleTargets.Length;
            Title = $"HumanGL — cible : {ScaleTarget}";
            Console.WriteLine($"[scale] cible = {ScaleTarget}");
        }

        if (_character != null)
        {
            if (KeyboardState.IsKeyPressed(Keys.R))
            {
                _character.ScaleMember(ScaleTarget, +ScaleStep);
                Title = $"HumanGL — {ScaleTarget} +";
                Console.WriteLine($"[scale] {ScaleTarget} +{ScaleStep}");
            }
            if (KeyboardState.IsKeyPressed(Keys.F))
            {
                _character.ScaleMember(ScaleTarget, -ScaleStep);
                Title = $"HumanGL — {ScaleTarget} -";
                Console.WriteLine($"[scale] {ScaleTarget} -{ScaleStep}");
            }
        }

    }

    private void UpdateAnimation(float deltaTime)
    {
        // deltaTime est le temps écoulé depuis la mise à jour précédente, en secondes.
        _time += deltaTime;
        if (_character != null)
            _animator.Update(_character, _time, deltaTime);

    }

    private void HandleCameraInput(double deltaTime)
    {
        // IsKeyDown reste vrai tant que la touche est maintenue.
        int horizontal = (KeyboardState.IsKeyDown(Keys.Right) ? 1 : 0)
                       - (KeyboardState.IsKeyDown(Keys.Left) ? 1 : 0);
        int vertical = (KeyboardState.IsKeyDown(Keys.Up) ? 1 : 0)
                     - (KeyboardState.IsKeyDown(Keys.Down) ? 1 : 0);

        if (horizontal != 0 || vertical != 0)
            _camera.Rotate(horizontal, vertical, deltaTime);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        // Efface l'image et les profondeurs avant de dessiner la nouvelle image.
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (_cube != null && _character != null)
        {
            _stack.Init();
            RenderNode(_character);
        }

        SwapBuffers();
    }

    private void RenderNode(Limb node)
    {
        _stack.Push();
        _stack.Multiply(node.GetStackMatrix());

        Matrix4x4 model = _stack.GetCurrent() * node.GetPivotMatrix();
        _cube!.Draw(model.ToColumnMajorArray(), _camera.ViewMatrix, _projection, node.Color);

        foreach (Limb child in node.Children)
            RenderNode(child);

        _stack.Pop();
    }

    protected override void OnUnload()
    {
        _cube?.Dispose();
        base.OnUnload();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);
        UpdateProjection(args.Width, args.Height);
    }

    private void UpdateProjection(int width, int height)
    {
        GL.Viewport(0, 0, width, height);
        if (width <= 0 || height <= 0)
            return;

        const float focalLength = 1.732051f;
        const float near = 0.1f;
        const float far = 50.0f;
        float aspectRatio = (float)width / height;
        Array.Clear(_projection);
        _projection[0] = focalLength / aspectRatio;
        _projection[5] = focalLength;
        _projection[10] = -(far + near) / (far - near);
        _projection[11] = -1;
        _projection[14] = -(2 * far * near) / (far - near);
        _projection[15] = 0;
    }
}
