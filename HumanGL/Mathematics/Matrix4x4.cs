namespace HumanGL.Mathematics;

public struct Matrix4x4
{
    // [ligne, colonne] — 16 floats pour une transformation 3D homogène
    public float[,] M;

    public Matrix4x4()
    {
        M = new float[4, 4]; // tout à 0 par défaut
    }

    /// <summary>
    /// Matrice identité : ne change aucun point (1 sur la diagonale, 0 ailleurs).
    /// </summary>
    public static Matrix4x4 Identity()
    {
        var mat = new Matrix4x4();
        mat.M[0, 0] = 1f;
        mat.M[1, 1] = 1f;
        mat.M[2, 2] = 1f;
        mat.M[3, 3] = 1f;
        return mat;
    }

    /// <summary>
    /// Déplace un point de (pos.X, pos.Y, pos.Z).
    /// On part de l'identité, puis on remplit la 4ᵉ colonne.
    /// </summary>
    public static Matrix4x4 Translation(Vector3 pos)
    {
        var mat = Identity();
        mat.M[0, 3] = pos.X;
        mat.M[1, 3] = pos.Y;
        mat.M[2, 3] = pos.Z;
        return mat;
    }

    /// <summary>
    /// Étire le cube 1×1×1 : sx en largeur, sy en hauteur, sz en profondeur.
    /// On remplace les 1 de la diagonale par les facteurs d'échelle.
    /// </summary>
    public static Matrix4x4 Scale(Vector3 scale)
    {
        var mat = Identity();
        mat.M[0, 0] = scale.X;
        mat.M[1, 1] = scale.Y;
        mat.M[2, 2] = scale.Z;
        // M[3,3] reste à 1 (coordonnée homogène w)
        return mat;
    }

    /// <summary>
    /// Produit A × B : chaque case = ligne de A · colonne de B.
    /// Attention : A × B ≠ B × A (ordre important pour TRS).
    /// </summary>
    public static Matrix4x4 Multiply(Matrix4x4 a, Matrix4x4 b)
    {
        var result = new Matrix4x4();

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                result.M[row, col] =
                    a.M[row, 0] * b.M[0, col] +
                    a.M[row, 1] * b.M[1, col] +
                    a.M[row, 2] * b.M[2, col] +
                    a.M[row, 3] * b.M[3, col];
            }
        }

        return result;
    }

    public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b) => Multiply(a, b);

    /// <summary>
    /// Rotation combinée. Les angles sont en degrés.
    /// Ordre choisi : Rz × Ry × Rx — le point subit X, puis Y, puis Z.
    /// Pour la marche, on n'utilise en pratique que l'axe X (balancement avant/arrière).
    /// </summary>
    public static Matrix4x4 RotationXYZ(Vector3 degrees)
    {
        return RotationZ(degrees.Z) * RotationY(degrees.Y) * RotationX(degrees.X);
    }

    /// <summary>Tourne autour de X (axe gauche-droite). Utile pour balancer un bras ou une jambe.</summary>
    public static Matrix4x4 RotationX(float degrees)
    {
        float rad = DegreesToRadians(degrees);
        float c = MathF.Cos(rad);
        float s = MathF.Sin(rad);

        var mat = Identity();
        mat.M[1, 1] = c;
        mat.M[1, 2] = -s;
        mat.M[2, 1] = s;
        mat.M[2, 2] = c;
        return mat;
    }

    /// <summary>Tourne autour de Y (axe vertical).</summary>
    public static Matrix4x4 RotationY(float degrees)
    {
        float rad = DegreesToRadians(degrees);
        float c = MathF.Cos(rad);
        float s = MathF.Sin(rad);

        var mat = Identity();
        mat.M[0, 0] = c;
        mat.M[0, 2] = s;
        mat.M[2, 0] = -s;
        mat.M[2, 2] = c;
        return mat;
    }

    /// <summary>Tourne autour de Z (axe devant-derrière).</summary>
    public static Matrix4x4 RotationZ(float degrees)
    {
        float rad = DegreesToRadians(degrees);
        float c = MathF.Cos(rad);
        float s = MathF.Sin(rad);

        var mat = Identity();
        mat.M[0, 0] = c;
        mat.M[0, 1] = -s;
        mat.M[1, 0] = s;
        mat.M[1, 1] = c;
        return mat;
    }

    private static float DegreesToRadians(float degrees) => degrees * (MathF.PI / 180f);

    /// <summary>
    /// OpenGL attend 16 floats en column-major.
    /// Notre M[row, col] est en row-major mathématique : on réordonne à l'export.
    /// </summary>
    public float[] ToColumnMajorArray()
    {
        return
        [
            M[0, 0], M[1, 0], M[2, 0], M[3, 0],
            M[0, 1], M[1, 1], M[2, 1], M[3, 1],
            M[0, 2], M[1, 2], M[2, 2], M[3, 2],
            M[0, 3], M[1, 3], M[2, 3], M[3, 3]
        ];
    }
}
