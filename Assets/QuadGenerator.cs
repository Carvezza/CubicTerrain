using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class QuadGenerator : MonoBehaviour
{
    public void Generate(QuadData quadData)
    {
        CreateQuad(quadData);
    }
    private void CreateQuad(QuadData quadData)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = quadData.Points;
        mesh.triangles = quadData.Triangles;
        mesh.normals = new Vector3[4] { quadData.Normal, quadData.Normal, quadData.Normal, quadData.Normal };
    }
}
