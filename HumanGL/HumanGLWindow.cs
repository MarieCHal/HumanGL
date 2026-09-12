using HumanGL.Rendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HumanGL;

public class HumanGLWindow : GameWindow
{
    private SquareRenderer? _square;

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
        _square = new SquareRenderer();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _square?.Draw();
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        // Libérer les ressources pendant que le contexte OpenGL existe encore.
        _square?.Dispose();
        base.OnUnload();
    }
}
