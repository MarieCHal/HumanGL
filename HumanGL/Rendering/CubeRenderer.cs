using HumanGL.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace HumanGL.Rendering;

public sealed class CubeRenderer : IDisposable
{
    // VAO : memorise comment lire les sommets.
    private int _vertexLayout;
    // VBO : stocke les coordonnees des sommets sur la carte graphique.
    private int _vertexBuffer;
    private readonly ShaderProgram _shaderProgram;

    public CubeRenderer()
    {
        string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "Shaders");
        _shaderProgram = new ShaderProgram(
            Path.Combine(shaderDirectory, "cube.vert"),
            Path.Combine(shaderDirectory, "cube.frag"));

        PrepareCubeVertices();
    }

    private void PrepareCubeVertices()
    {
        // Cube 1x1x1 centré à l'origine : chaque coordonnée vaut -0.5 ou +0.5.
        // 6 faces × 2 triangles × 3 sommets = 36 sommets.
        // Chaque ligne contient uniquement une position (x, y, z).
        float[] vertexPositions =
        {
            // Avant (+Z).
            -0.5f, -0.5f,  0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,

            // Arrière (-Z).
             0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,

            // Droite (+X).
             0.5f, -0.5f,  0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f,  0.5f,  0.5f,

            // Gauche (-X).
            -0.5f, -0.5f, -0.5f,
            -0.5f, -0.5f,  0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,
            -0.5f,  0.5f,  0.5f,
            -0.5f,  0.5f, -0.5f,

            // Dessus (+Y).
            -0.5f,  0.5f,  0.5f,
             0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f,  0.5f,
             0.5f,  0.5f, -0.5f,
            -0.5f,  0.5f, -0.5f,

            // Dessous (-Y).
            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f, -0.5f,
             0.5f, -0.5f,  0.5f,
            -0.5f, -0.5f,  0.5f
        };

        // Cree le stockage des sommets et leur configuration de lecture, vide.
        _vertexLayout = GL.GenVertexArray();
        _vertexBuffer = GL.GenBuffer();
        GL.BindVertexArray(_vertexLayout);

        UploadVertexPositions(vertexPositions);

        // Chaque sommet contient 3 float : x, y, z. OpenGL demande une taille en octets
        const int bytesPerVertex = 3 * sizeof(float);

        // Entree 0 du shader : lit x, y, z depuis le debut, avec 12 octets entre sommets.
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, bytesPerVertex, 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);
    }

    // Envoie les positions dans le stockage du cube sur la carte graphique.
    private void UploadVertexPositions(float[] vertexPositions)
    {
        // Selectionne le buffer vide.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertexPositions.Length * sizeof(float),
            vertexPositions,
            BufferUsageHint.StaticDraw);
    }

    public void Draw(float[] modelMatrix, float[] viewMatrix, float[] projectionMatrix, Vector3 color)
    {
        // Active les shaders et leur transmet les matrices et la couleur.
        _shaderProgram.Use();
        _shaderProgram.SetMatrix4("model", modelMatrix); // position, rotation taille du membre
        _shaderProgram.SetMatrix4("view", viewMatrix); // point de vue de la caméra
        _shaderProgram.SetMatrix4("projection", projectionMatrix); // perspective de la caméra
        _shaderProgram.SetVector3("objectColor", color);
        // Selectionne les sommets du cube.
        GL.BindVertexArray(_vertexLayout);
        // Dessine les 12 triangles du cube en un seul appel.
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public void Dispose()
    {
        // Libere les ressources de la carte graphique.
        GL.DeleteBuffer(_vertexBuffer);
        GL.DeleteVertexArray(_vertexLayout);
        _vertexBuffer = 0;
        _vertexLayout = 0;
        _shaderProgram.Dispose();
    }
}
