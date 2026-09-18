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

    // Matrices de démonstration : 16 valeurs, rangées colonne par colonne.
    // À remplacer ensuite par nos matrices.
    private readonly float[] _model =
    {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1
    };

    private readonly OrbitCamera _camera = new();

    private readonly float[] _projection = new float[16];

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
        // Définit la couleur de fond (rouge, vert, bleu, opacité), utilisée par GL.Clear.
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        // Active le test de profondeur : les surfaces proches cachent les surfaces derrière.
        GL.Enable(EnableCap.DepthTest);
        UpdateProjection(FramebufferSize.X, FramebufferSize.Y);
        _cube = new CubeRenderer();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        // Ne pas déplacer la caméra lorsque l'utilisateur travaille dans une autre fenêtre.
        if (!IsFocused)
            return;

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
            return;
        }

        // IsKeyDown reste vrai tant que la touche est maintenue : mouvement continu.
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
        // Efface l'image précédente avec la couleur de fond et réinitialise le buffer de profondeur.
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _cube?.Draw(_model, _camera.ViewMatrix, _projection);
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        // Libérer les ressources pendant que le contexte OpenGL existe encore.
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
        // Définit la zone de dessin en pixels : ici, toute la surface de la fenêtre.
        GL.Viewport(0, 0, width, height);
        if (width <= 0 || height <= 0)
            return;

        // Perspective de démonstration : champ vertical de 60°, plans à 0.1 et 10.
        // Seule l'échelle horizontale varie avec le format de la fenêtre.
        const float focalLength = 1.732051f; // 1 / tan(60° / 2)
        const float near = 0.1f;
        const float far = 10.0f;
        float aspectRatio = (float)width / height;
        _projection[0] = focalLength / aspectRatio;
        _projection[5] = focalLength;
        _projection[10] = -(far + near) / (far - near);
        _projection[11] = -1;
        _projection[14] = -(2 * far * near) / (far - near);
    }
}
