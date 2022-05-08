using UnityEngine;

public class QuadData
{
    /// <summary>
    /// Points bl - br - tl - tr
    /// </summary>
    /// <param name="points"></param>
    /// <param name="normal"></param>
    public QuadData(Vector3[] points, Vector3 normal, Vector3 position, bool visible = true)
    {
        _points = points;
        _triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        _normal = normal;
        _visible = visible;
        _position = position;
    }
    private Vector3 _position;
    private Vector3[] _points;
    private int[] _triangles;
    private Vector3 _normal;
    private bool _visible;

    public Vector3[] Points { get => _points; set => _points = value; }
    public int[] Triangles { get => _triangles; set => _triangles = value; }
    public Vector3 Normal { get => _normal; set => _normal = value; }
    public bool Visible { get => _visible; set => _visible = value; }
    public Vector3 Position { get => _position; set => _position = value; }
}