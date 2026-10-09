namespace HumanGL.Mathematics;

public struct Matrix4x4
{

    public float[,] M;

    public Matrix4x4()
    {
        M = new float[4, 4];
    }

    public static Matrix4x4 Identity()
    {
        var mat = new Matrix4x4();
        mat.M[0, 0] = 1f;
        mat.M[1, 1] = 1f;
        mat.M[2, 2] = 1f;
        mat.M[3, 3] = 1f;
        return mat;
    }

    public static Matrix4x4 Translation(Vector3 pos)
    {
        var mat = Identity();
        mat.M[0, 3] = pos.X;
        mat.M[1, 3] = pos.Y;
        mat.M[2, 3] = pos.Z;
        return mat;
    }

    public static Matrix4x4 Scale(Vector3 scale)
    {
        var mat = Identity();
        mat.M[0, 0] = scale.X;
        mat.M[1, 1] = scale.Y;
        mat.M[2, 2] = scale.Z;

        return mat;
    }

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

    public static Matrix4x4 RotationXYZ(Vector3 degrees)
    {
        return RotationZ(degrees.Z) * RotationY(degrees.Y) * RotationX(degrees.X);
    }

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
