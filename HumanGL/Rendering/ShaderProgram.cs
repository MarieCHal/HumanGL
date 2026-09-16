using OpenTK.Graphics.OpenGL4;

namespace HumanGL.Rendering;

public sealed class ShaderProgram : IDisposable
{
    private int _handle;

    public ShaderProgram(string vertexPath, string fragmentPath)
    {
        int vertexShader = 0;
        int fragmentShader = 0;

        try
        {
            vertexShader = CompileShader(ShaderType.VertexShader, vertexPath);
            fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentPath);
            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);
            GL.LinkProgram(_handle);

            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
                throw new InvalidOperationException(GL.GetProgramInfoLog(_handle));

            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            if (vertexShader != 0)
                GL.DeleteShader(vertexShader);
            if (fragmentShader != 0)
                GL.DeleteShader(fragmentShader);
        }
    }

    public void Use() => GL.UseProgram(_handle);

    public void Dispose()
    {
        if (_handle == 0)
            return;

        GL.DeleteProgram(_handle);
        _handle = 0;
    }

    private static int CompileShader(ShaderType type, string path)
    {
        string source = File.ReadAllText(path);
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);

        if (success == 0)
        {
            string error = GL.GetShaderInfoLog(shader);
            GL.DeleteShader(shader);
            throw new InvalidOperationException($"Erreur dans {path} : {error}");
        }

        return shader;
    }
}
