using HumanGL.Mathematics;

namespace HumanGL.Rendering;

// Caméra de démonstration : tourne autour de l'origine en regardant toujours le cube.
// Les calculs utilisent notre bibliothèque mathématique.
public sealed class OrbitCamera
{
    private const float Distance = 6.0f;
    private const float RotationSpeed = MathF.PI / 2; // 90 degrés par seconde.

    private float _horizontalAngle = MathF.PI / 3;
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
        // La caméra stocke des radians ; nos rotations attendent des degrés.
        float horizontalAngleDegrees = _horizontalAngle * 180f / MathF.PI;
        float elevationDegrees = _elevation * 180f / MathF.PI;

        // La vue transforme la scène dans le repère de la caméra.
        // L'origine reste devant la caméra, à une distance constante.
        var view =
            Matrix4x4.Translation(new Vector3(0f, 0f, -Distance))
            * Matrix4x4.RotationX(elevationDegrees)
            * Matrix4x4.RotationY(-horizontalAngleDegrees);

        view.ToColumnMajorArray().CopyTo(ViewMatrix, 0);
    }
}
