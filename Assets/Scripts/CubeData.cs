using UnityEngine;

public class CubeData
{
    /// <summary>
    /// Front,Back,Left,Right,Top,Bottom
    /// </summary>
    /// <param name="quads"></param>
    public CubeData(QuadData[] quads)
    {
        _quads = quads;
    }
    public CubeData(Vector3 offset)
    {
        _position = offset;

        QuadData frontQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (-0.5f, -0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, -0.5f, -0.5f) + offset,
                               new Vector3 (-0.5f, 0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, -0.5f) + offset,},
                new Vector3(0, 0, -1),
                offset
            );
        QuadData rightQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (0.5f, -0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, 0.5f) + offset,},
                new Vector3(1, 0, 0), offset
            );
        QuadData topQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (-0.5f, 0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, -0.5f) + offset,
                               new Vector3 (-0.5f, 0.5f, 0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, 0.5f) + offset,},
                new Vector3(0, 1, 0), offset
            );
        QuadData leftQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (-0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (-0.5f, -0.5f,-0.5f) + offset,
                               new Vector3 (-0.5f, 0.5f, 0.5f) + offset,
                               new Vector3 (-0.5f, 0.5f, -0.5f) + offset,},
               new Vector3(-1, 0, 0), offset
            );
        QuadData backQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (-0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (0.5f, 0.5f, 0.5f) + offset,
                               new Vector3 (-0.5f, 0.5f, 0.5f) + offset,},
               new Vector3(0, 0, 1), offset
            );
        QuadData bottomQuad = new QuadData
            (
                new Vector3[4]{new Vector3 (-0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (0.5f, -0.5f, 0.5f) + offset,
                               new Vector3 (-0.5f, -0.5f, -0.5f) + offset,
                               new Vector3 (0.5f, -0.5f, -0.5f) + offset,},
                new Vector3(0, -1, 0), offset
            );
        _quads = new QuadData[6]
                {
                    frontQuad, backQuad, leftQuad, rightQuad, topQuad, bottomQuad
                };
    }
    private QuadData[] _quads;
    private Vector3 _position;
    public Vector3 Position => _position;
    public QuadData GetQuad(QuadPosition position)
    {
        return _quads[(int)position];
    }
}
