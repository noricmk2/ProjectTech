using UnityEngine;

// Catmull-Rom Spline Interpolating
public class CatmullRom
{
    //Curve interpolation function 2D: It is more convenient if you want to display all array data (can display head and tail, two points can also be displayed)
    public static Vector3 EasyInterp2D(Vector2[] pts, float t)
    {
        Vector2[] v = new Vector2[pts.Length + 2];
        v[0] = pts[0];
        v[v.Length - 1] = pts[pts.Length - 1];
        for (int i = 0; i < pts.Length; i++)
        {
            v[i + 1] = pts[i];
        }
        return Interp2D(v, t);
    }

    //Curve interpolation function 3D: it is more convenient if you want to display all array data (can display head and tail, two points can also be displayed)
    public static Vector3 EasyInterp3D(Vector3[] pts, float t)
    {
        Vector3[] v = new Vector3[pts.Length + 2];
        v[0] = pts[0];
        v[v.Length - 1] = pts[pts.Length - 1];
        for (int i = 0; i < pts.Length; i++)
        {
            v[i + 1] = pts[i];
        }
        return Interp3D(v, t);
    }

    //Curve interpolation function 2D: original method (head and tail are not displayed, note: it needs more than four points (inclusive)!)
    public static Vector3 Interp2D(Vector2[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector2 a = pts[currPt];
        Vector2 b = pts[currPt + 1];
        Vector2 c = pts[currPt + 2];
        Vector2 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    //Curve interpolation function 3D: original method (not shown at the beginning and end)
    public static Vector3 Interp3D(Vector3[] pts, float t)
    {
        int numSections = pts.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = pts[currPt];
        Vector3 b = pts[currPt + 1];
        Vector3 c = pts[currPt + 2];
        Vector3 d = pts[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }
}
