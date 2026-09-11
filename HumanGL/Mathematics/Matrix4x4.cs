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
}
