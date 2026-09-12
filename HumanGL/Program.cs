using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// OpenGL 4.1 avec un profil Core, compatible avec macOS.
var settings = new NativeWindowSettings
{
    Title = "HumanGL",
    ClientSize = new Vector2i(800, 600),
    APIVersion = new Version(4, 1),
    Profile = ContextProfile.Core,
    Flags = ContextFlags.ForwardCompatible
};

using var window = new GameWindow(GameWindowSettings.Default, settings);

window.Load += () =>
{
    window.VSync = VSyncMode.On;
    GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
    GL.Viewport(0, 0, window.FramebufferSize.X, window.FramebufferSize.Y);
};

// Adapter la zone de dessin au redimensionnement.
window.FramebufferResize += size => GL.Viewport(0, 0, size.Width, size.Height);

window.UpdateFrame += _ =>
{
    if (window.KeyboardState.IsKeyDown(Keys.Escape))
        window.Close();
};

window.RenderFrame += _ =>
{
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // Le dessin du personnage viendra ici.

    window.SwapBuffers();
};

window.Run();
