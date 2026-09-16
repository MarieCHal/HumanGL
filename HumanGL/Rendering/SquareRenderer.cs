using OpenTK.Graphics.OpenGL4;

namespace HumanGL.Rendering;

public class SquareRenderer : IDisposable
{
    private int _vao;
    private int _vbo;
    private readonly ShaderProgram _shader;

    public SquareRenderer()
    {
        // Deux triangles forment le carré. Chaque sommet contient x, y et z.
        float[] vertices =
        {
            -0.1f, -0.2f, 0.0f,
             0.1f, -0.2f, 0.0f,
             0.1f,  0.2f, 0.0f,

            -0.1f, -0.2f, 0.0f,
             0.1f,  0.2f, 0.0f,
            -0.1f,  0.2f, 0.0f
        };

        string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "Shaders");
        _shader = new ShaderProgram(
            Path.Combine(shaderDirectory, "square.vert"),
            Path.Combine(shaderDirectory, "square.frag"));

        // Envoyer les sommets une seule fois.
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),
            vertices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void Draw()
    {
        _shader.Use();
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        _vbo = 0;
        _vao = 0;
        _shader.Dispose();
    }
}
