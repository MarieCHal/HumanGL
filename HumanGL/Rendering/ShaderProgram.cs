using HumanGL.Mathematics;
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
            // Crée le programme OpenGL qui réunira les shaders de sommets et de fragments.
            _handle = GL.CreateProgram();
            // Attache ce shader compilé au programme avant de les lier.
            GL.AttachShader(_handle, vertexShader);
            // Attache ce shader compilé au programme avant de les lier.
            GL.AttachShader(_handle, fragmentShader);
            // Lie les shaders pour obtenir un programme utilisable par le GPU.
            GL.LinkProgram(_handle);

            // Vérifie si la liaison du programme a réussi (success différent de 0).
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
                // Récupère le message du pilote expliquant l'échec de la liaison.
                throw new InvalidOperationException(GL.GetProgramInfoLog(_handle));

            // Détache le shader : le programme lié conserve le résultat compilé.
            GL.DetachShader(_handle, vertexShader);
            // Détache le shader : le programme lié conserve le résultat compilé.
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
                // Demande la suppression du shader, effective lorsqu'il n'est plus attaché.
                GL.DeleteShader(vertexShader);
            if (fragmentShader != 0)
                // Demande la suppression du shader, effective lorsqu'il n'est plus attaché.
                GL.DeleteShader(fragmentShader);
        }
    }

    // Active ce programme de shaders pour les prochains dessins et envois de variables uniformes.
    public void Use() => GL.UseProgram(_handle);

    // Le programme doit être actif (Use) avant l'envoi d'une matrice.
    public void SetMatrix4(string name, float[] values)
    {
        // Cherche l'emplacement de la variable uniform nommée dans le shader (-1 si absente ou inactive).
        int location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
            throw new InvalidOperationException($"Matrice introuvable dans le shader : {name}");
        // Envoie une matrice 4x4 à cet emplacement dans le programme actif, sans la transposer.
        GL.UniformMatrix4(location, 1, false, values);
    }

    // Envoie trois composantes, par exemple rouge, vert et bleu pour une couleur.
    // Le programme doit être actif (Use) avant cet appel.
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(_handle, name);
        if (location == -1)
            throw new InvalidOperationException($"Vecteur introuvable dans le shader : {name}");

        GL.Uniform3(location, value.X, value.Y, value.Z);
    }

    public void Dispose()
    {
        if (_handle == 0)
            return;

        // Demande la suppression du programme ; OpenGL le libère quand il n'est plus actif.
        GL.DeleteProgram(_handle);
        _handle = 0;
    }

    private static int CompileShader(ShaderType type, string path)
    {
        string source = File.ReadAllText(path);
        // Crée un shader du type demandé : sommets (vertex) ou fragments (fragment).
        int shader = GL.CreateShader(type);
        // Fournit au shader le texte du code GLSL chargé depuis le fichier.
        GL.ShaderSource(shader, source);
        // Demande au pilote graphique de compiler ce code GLSL.
        GL.CompileShader(shader);
        // Vérifie si la compilation du shader a réussi (success différent de 0).
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);

        if (success == 0)
        {
            // Récupère le détail des erreurs de compilation du shader.
            string error = GL.GetShaderInfoLog(shader);
            // Demande la suppression du shader, effective lorsqu'il n'est plus attaché.
            GL.DeleteShader(shader);
            throw new InvalidOperationException($"Erreur dans {path} : {error}");
        }

        return shader;
    }
}
