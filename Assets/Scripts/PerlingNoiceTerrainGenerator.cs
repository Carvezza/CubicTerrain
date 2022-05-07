using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlingNoiceTerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private CubeGenerator _cubeGeneratorPrefab;

    [SerializeField]
    private int _terrainHeight;
    [SerializeField]
    private int _chunkSize;

    [SerializeField]
    private Vector2 _offset;

    private int[,] _grid;

    // Start is called before the first frame update
    void Start()
    {
        Generate();
        Display();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Generate()
    {
        _grid = new int[_chunkSize, _chunkSize];

        // Use unity Random for initial shape
        for (int i = 0; i < _chunkSize; i++)
        {
            for (int j = 0; j < _chunkSize; j++)
            {
                float noice = Mathf.PerlinNoise((float)(i + _offset.x)/ _chunkSize, (float)(j + _offset.y)/ _chunkSize);
                _grid[i, j] = (int)(noice * _terrainHeight);
            }
        }
    }

   


    private void Display()
    {
        for (int i = 0; i < _chunkSize; i++)
        {
            for (int j = 0; j < _chunkSize; j++)
            {
                for (int k = 0; k < _grid[i,j]; k++)
                {
                    Instantiate(_cubeGeneratorPrefab, new Vector3(i, k, j), Quaternion.identity);
                }
            }
        }
    }
}