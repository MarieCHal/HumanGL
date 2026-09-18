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
    private SceneNode? _character;
    private readonly Animator _animator = new();
    private readonly MatrixStack _stack = new();
    private readonly OrbitCamera _camera = new();
    private readonly float[] _projection = new float[16];
    private float _time;

    public HumanGLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings
    {
        Title = "HumanGL",
        ClientSize = new Vector2i(800, 600),
        APIVersion = new Version(4, 1),
        Profile = ContextProfile.Core,
        Flags = ContextFlags.ForwardCompatible
    })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        VSync = VSyncMode.On;
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        UpdateProjection(FramebufferSize.X, FramebufferSize.Y);

        _cube = new CubeRenderer();
        _character = SceneNode.InitCharacter();
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

        // Animations : I = idle, W = marche, Espace = saut
        if (KeyboardState.IsKeyPressed(Keys.I))
            _animator.SetState(AnimState.Idle);
        if (KeyboardState.IsKeyPressed(Keys.W))
            _animator.SetState(AnimState.Walk);
        if (KeyboardState.IsKeyPressed(Keys.Space))
            _animator.SetState(AnimState.Jump);

        float deltaTime = (float)args.Time;
        _time += deltaTime;
        if (_character != null)
            _animator.Update(_character, _time, deltaTime);

        int horizontal = (KeyboardState.IsKeyDown(Keys.Right) ? 1 : 0)
                       - (KeyboardState.IsKeyDown(Keys.Left) ? 1 : 0);
        int vertical = (KeyboardState.IsKeyDown(Keys.Up) ? 1 : 0)
                     - (KeyboardState.IsKeyDown(Keys.Down) ? 1 : 0);

        if (horizontal != 0 || vertical != 0)
            _camera.Rotate(horizontal, vertical, args.Time);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        if (_cube != null && _character != null)
        {
            _stack.Init();
            RenderNode(_character);
        }

        SwapBuffers();
    }

    /// <summary>
    /// Point de raccord : ta matrice monde (pile) → son Draw(cube).
    /// </summary>
    private void RenderNode(SceneNode node)
    {
        _stack.Push();
        _stack.Multiply(node.GetLocalMatrix());

        float[] model = _stack.GetCurrent().ToColumnMajorArray();
        _cube!.Draw(model, _camera.ViewMatrix, _projection);

        foreach (SceneNode child in node.Children)
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

        const float focalLength = 1.732051f; // 1 / tan(60° / 2)
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
