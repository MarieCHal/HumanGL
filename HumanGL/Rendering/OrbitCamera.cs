namespace HumanGL.Rendering;

// Caméra de démonstration : tourne autour de l'origine en regardant toujours le cube.
// Les calculs pourront ensuite utiliser notre bibliothèque mathématique.
public sealed class OrbitCamera
{
    private const float Distance = 6.0f;
    private const float RotationSpeed = MathF.PI / 2; // 90 degrés par seconde.
    private const float MaxElevation = 80 * MathF.PI / 180;

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
        _azimuth = MathF.IEEERemainder(_azimuth + horizontal * step, 2 * MathF.PI);

        // On s'arrête avant les pôles pour éviter de retourner la caméra.
        _elevation += vertical * step;
        UpdateView();
    }

    private void UpdateView()
    {
        float sinAzimuth = MathF.Sin(_azimuth);
        float cosAzimuth = MathF.Cos(_azimuth);
        float sinElevation = MathF.Sin(_elevation);
        float cosElevation = MathF.Cos(_elevation);

        // Les trois lignes décrivent les axes droite, haut et arrière de la caméra.
        // On stocke ici leurs composantes colonne par colonne pour OpenGL.
        ViewMatrix[0] = cosAzimuth;
        ViewMatrix[1] = -sinElevation * sinAzimuth;
        ViewMatrix[2] = cosElevation * sinAzimuth;
        ViewMatrix[3] = 0;

        ViewMatrix[4] = 0;
        ViewMatrix[5] = cosElevation;
        ViewMatrix[6] = sinElevation;
        ViewMatrix[7] = 0;

        ViewMatrix[8] = -sinAzimuth;
        ViewMatrix[9] = -sinElevation * cosAzimuth;
        ViewMatrix[10] = cosElevation * cosAzimuth;
        ViewMatrix[11] = 0;

        // L'origine reste devant la caméra, à une distance constante.
        ViewMatrix[12] = 0;
        ViewMatrix[13] = 0;
        ViewMatrix[14] = -Distance;
        ViewMatrix[15] = 1;
    }
}
