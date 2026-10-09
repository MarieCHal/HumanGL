using HumanGL.Mathematics;

namespace HumanGL.Rendering;

// Caméra de démonstration : tourne autour de l'origine en regardant toujours le cube.
// Les calculs utilisent notre bibliothèque mathématique.
public sealed class OrbitCamera
{
    private const float Distance = 6.0f;
    private const float RotationSpeed = MathF.PI / 2; // 90 degrés par seconde.

    private float _azimuth = 0f;
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
        // Le temps écoulé rend la vitesse indépendante du nombre d'images par seconde.
        // Limiter un pas à 0.1 s évite un grand saut après une pause de l'application.
        float step = RotationSpeed * (float)Math.Clamp(elapsedSeconds, 0, 0.1);
        // Ramener les angles modulo un tour permet de tourner sans limite
        // tout en évitant de perdre en précision avec des angles trop grands.
        _azimuth = MathF.IEEERemainder(_azimuth + horizontal * step, 2 * MathF.PI);
        _elevation = MathF.IEEERemainder(_elevation + vertical * step, 2 * MathF.PI);
        UpdateView();
    }

    private void UpdateView()
    {
        // La caméra stocke des radians ; nos rotations attendent des degrés.
        float azimuthDegrees = _azimuth * 180f / MathF.PI;
        float elevationDegrees = _elevation * 180f / MathF.PI;

        // La vue transforme la scène dans le repère de la caméra.
        // L'origine reste devant la caméra, à une distance constante.
        var view =
            Matrix4x4.Translation(new Vector3(0f, 0f, -Distance))
            * Matrix4x4.RotationX(elevationDegrees)
            * Matrix4x4.RotationY(-azimuthDegrees);

        view.ToColumnMajorArray().CopyTo(ViewMatrix, 0);
    }
}
