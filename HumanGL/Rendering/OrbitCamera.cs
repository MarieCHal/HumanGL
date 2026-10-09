using HumanGL.Mathematics;

namespace HumanGL.Rendering;

// Caméra de démonstration : tourne autour de l'origine en regardant toujours le cube.
// Les calculs utilisent notre bibliothèque mathématique.
public sealed class OrbitCamera
{
    private const float Distance = 6.0f;
    private const float RotationSpeed = 90f; // 90 degrés par seconde.

    private float _horizontalAngle = 60f;
    private float _elevation = 0f;

    // Matrice de vue rangée colonne par colonne, comme attendu par notre shader.
    public float[] ViewMatrix { get; } = new float[16];

    public OrbitCamera()
    {
        UpdateView();
    }

    // horizontal et vertical valent -1, 0 ou 1 selon les flèches maintenues.
    public void Rotate(float horizontal, float vertical, double elapsedSeconds)
    {
        float step = RotationSpeed * (float)Math.Clamp(elapsedSeconds, 0, 0.1);
        _horizontalAngle += horizontal * step;
        _elevation += vertical * step;
        UpdateView();
    }

    private void UpdateView()
    {
        // Place l'origine devant la caméra et applique les angles de vue.
        var view =
            Matrix4x4.Translation(new Vector3(0f, 0f, -Distance))
            * Matrix4x4.RotationX(_elevation)
            * Matrix4x4.RotationY(-_horizontalAngle);

        // Prépare la matrice pour le shader.
        view.ToColumnMajorArray().CopyTo(ViewMatrix, 0);
    }
}
