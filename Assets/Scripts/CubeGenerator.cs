using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class CubeGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector3 _position;
    private Vector3[] vertices =
        {
            //forward
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 (-0.5f, 0.5f, -0.5f),
            new Vector3 (0.5f, -0.5f, -0.5f),
            new Vector3 (0.5f, 0.5f, -0.5f),
            //right
            new Vector3 (0.5f, 0.5f, -0.5f),
            new Vector3 (0.5f, -0.5f, -0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, 0.5f),
            //top
            new Vector3 (-0.5f, 0.5f, -0.5f),
            new Vector3 (0.5f, 0.5f, -0.5f),
            new Vector3 (-0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            //left
            new Vector3 (-0.5f, -0.5f, 0.5f),
            new Vector3 (-0.5f, 0.5f, 0.5f),
            new Vector3 (-0.5f, 0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f, -0.5f),
            //back
            new Vector3 (-0.5f, -0.5f, 0.5f),
            new Vector3 (-0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, 0.5f),
            //bot
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, -0.5f),
        };
    private int[] triangles =
        { 
            //forward
            0,1,2,
            1,3,2,
            //right
            5,4,7,
            4,6,7,
            //top
            8,10,9,
            10,11,9,
            //left
            12,13,14,
            14,15,12,
            //back
            16,18,17,
            16,19,18,
            //bot
            20,22,21,
            20,23,22
        };
    private Vector3[] normals =
        {
            //forward
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            //right
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            //top
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 0),
            //left
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(-1, 0, 0),
            //back
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            //bot
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, -1, 0),
        };

    // Start is called before the first frame update
    void Start()
    {
        CreateCube(Vector3.zero);
    }

    private void CreateCube(Vector3 offset)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += offset;
        }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
    }
}
