using OpenTK.Graphics.OpenGL4;

namespace HumanGL.Rendering;

public sealed class CubeRenderer : IDisposable
{
    private int _vao;
    private int _vbo;
    private readonly ShaderProgram _shader;

    public CubeRenderer()
    {
        // Cube 1x1x1 centré à l'origine : chaque coordonnée vaut -0.5 ou +0.5.
        // 6 faces × 2 triangles × 3 sommets = 36 sommets.
        // Chaque ligne contient une position (x, y, z) puis une couleur (r, g, b).
        float[] vertices =
        {
            // Avant (+Z) : rouge.
            -0.5f, -0.5f,  0.5f,  0.9f, 0.3f, 0.3f,
             0.5f, -0.5f,  0.5f,  0.9f, 0.3f, 0.3f,
             0.5f,  0.5f,  0.5f,  0.9f, 0.3f, 0.3f,
            -0.5f, -0.5f,  0.5f,  0.9f, 0.3f, 0.3f,
             0.5f,  0.5f,  0.5f,  0.9f, 0.3f, 0.3f,
            -0.5f,  0.5f,  0.5f,  0.9f, 0.3f, 0.3f,

            // Arrière (-Z) : orange.
             0.5f, -0.5f, -0.5f,  1.0f, 0.6f, 0.2f,
            -0.5f, -0.5f, -0.5f,  1.0f, 0.6f, 0.2f,
            -0.5f,  0.5f, -0.5f,  1.0f, 0.6f, 0.2f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.6f, 0.2f,
            -0.5f,  0.5f, -0.5f,  1.0f, 0.6f, 0.2f,
             0.5f,  0.5f, -0.5f,  1.0f, 0.6f, 0.2f,

            // Droite (+X) : bleu.
             0.5f, -0.5f,  0.5f,  0.3f, 0.5f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.3f, 0.5f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.3f, 0.5f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.3f, 0.5f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.3f, 0.5f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.3f, 0.5f, 1.0f,

            // Gauche (-X) : violet.
            -0.5f, -0.5f, -0.5f,  0.7f, 0.3f, 0.9f,
            -0.5f, -0.5f,  0.5f,  0.7f, 0.3f, 0.9f,
            -0.5f,  0.5f,  0.5f,  0.7f, 0.3f, 0.9f,
            -0.5f, -0.5f, -0.5f,  0.7f, 0.3f, 0.9f,
            -0.5f,  0.5f,  0.5f,  0.7f, 0.3f, 0.9f,
            -0.5f,  0.5f, -0.5f,  0.7f, 0.3f, 0.9f,

            // Dessus (+Y) : vert.
            -0.5f,  0.5f,  0.5f,  0.3f, 0.8f, 0.5f,
             0.5f,  0.5f,  0.5f,  0.3f, 0.8f, 0.5f,
             0.5f,  0.5f, -0.5f,  0.3f, 0.8f, 0.5f,
            -0.5f,  0.5f,  0.5f,  0.3f, 0.8f, 0.5f,
             0.5f,  0.5f, -0.5f,  0.3f, 0.8f, 0.5f,
            -0.5f,  0.5f, -0.5f,  0.3f, 0.8f, 0.5f,

            // Dessous (-Y) : jaune.
            -0.5f, -0.5f, -0.5f,  0.9f, 0.8f, 0.2f,
             0.5f, -0.5f, -0.5f,  0.9f, 0.8f, 0.2f,
             0.5f, -0.5f,  0.5f,  0.9f, 0.8f, 0.2f,
            -0.5f, -0.5f, -0.5f,  0.9f, 0.8f, 0.2f,
             0.5f, -0.5f,  0.5f,  0.9f, 0.8f, 0.2f,
            -0.5f, -0.5f,  0.5f,  0.9f, 0.8f, 0.2f
        };

        string shaderDirectory = Path.Combine(AppContext.BaseDirectory, "Shaders");
        _shader = new ShaderProgram(
            Path.Combine(shaderDirectory, "cube.vert"),
            Path.Combine(shaderDirectory, "cube.frag"));

        // Les données sont envoyées au GPU une seule fois, à la création du cube.
        // Crée un identifiant de VAO, qui mémorise la configuration des attributs des sommets.
        _vao = GL.GenVertexArray();
        // Crée un identifiant de buffer, qui contiendra les données des sommets (VBO).
        _vbo = GL.GenBuffer();
        // Active le VAO du cube pour utiliser ou configurer ses attributs de sommets.
        GL.BindVertexArray(_vao);
        // Sélectionne le VBO comme buffer de sommets à remplir ou à décrire.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        // Alloue le buffer et y copie les sommets ; StaticDraw indique des données rarement modifiées.
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),
            vertices, BufferUsageHint.StaticDraw);

        const int stride = 6 * sizeof(float);
        // Attribut 0 : position = 3 float au début de chaque sommet ; stride sépare deux sommets.
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        // Active l'attribut 0 (position) pour le vertex shader.
        GL.EnableVertexAttribArray(0);
        // Attribut 1 : couleur = 3 float après la position, donc à 3 × sizeof(float) octets.
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        // Active l'attribut 1 (couleur) pour le vertex shader.
        GL.EnableVertexAttribArray(1);
        // Désactive le VAO pour éviter de modifier sa configuration par accident.
        GL.BindVertexArray(0);
    }

    public void Draw(float[] model, float[] view, float[] projection)
    {
        _shader.Use();
        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        // Active le VAO du cube pour utiliser ou configurer ses attributs de sommets.
        GL.BindVertexArray(_vao);
        // Un seul appel de dessin pour tout le cube.
        // Dessine les 36 sommets par groupes de 3 : les 12 triangles du cube.
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }

    public void Dispose()
    {
        // Supprime le VBO et libère le stockage des sommets quand il n'est plus utilisé.
        GL.DeleteBuffer(_vbo);
        // Supprime le VAO qui mémorisait la configuration des sommets.
        GL.DeleteVertexArray(_vao);
        _vbo = 0;
        _vao = 0;
        _shader.Dispose();
    }
}
