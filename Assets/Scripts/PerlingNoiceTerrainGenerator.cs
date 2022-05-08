using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class PerlingNoiceTerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform[] _cameraPositions;

    [SerializeField]
    private CubeGenerator _cubeGeneratorPrefab;
    [SerializeField]
    private QuadGenerator _quadGeneratorPrefab;

    [SerializeField]
    private int _terrainHeight;
    [SerializeField]
    private int _chunkSize;

    [SerializeField]
    private Vector2 _offset;
    private int[,] _grid;
    private CubeType[,,] _voxelGrid;
    private int _maxHeight;
    private int _minHeight;
    private int VoxelDimX => _voxelGrid.GetLength(0);
    private int VoxelDimY => _voxelGrid.GetLength(2);
    private int VoxelDimZ => _voxelGrid.GetLength(1);

    private List<CubeData> _terreinCubes;

    private Dictionary<Vector3, QuadData> _frontQuads;
    private Dictionary<Vector3, QuadData> _backQuads;
    private Dictionary<Vector3, QuadData> _leftQuads;
    private Dictionary<Vector3, QuadData> _rightQuads;
    private Dictionary<Vector3, QuadData> _topQuads;

    private int[,,] _frontQuadGrid;
    private int[,,] _backQuadGrid;
    private int[,,] _leftQuadGrid;
    private int[,,] _rightQuadGrid;
    private int[,,] _topQuadGrid;
    private List<QuadData> _quadsToBake;

    // Start is called before the first frame update
    void Start()
    {
        //Generate();
        //RemoveExcessCubes();
        //RemoveExcessQuads();
        //MergeQuads();
        //BakeQuads();
        StartCoroutine(StepByStepDisplay());
    }

    private void Generate()
    {
        _grid = new int[_chunkSize, _chunkSize];
        _voxelGrid = new CubeType[_chunkSize, _chunkSize, _terrainHeight];// x z y
        _maxHeight = 0;
        _minHeight = _terrainHeight;

        for (int i = 0; i < _chunkSize; i++)//x
        {
            for (int j = 0; j < _chunkSize; j++)//z
            {
                float noice = Mathf.PerlinNoise((float)(i + _offset.x) / _chunkSize, (float)(j + _offset.y) / _chunkSize);
                int height = (int)(noice * _terrainHeight);//y
                _maxHeight = Mathf.Max(height, _maxHeight);
                _minHeight = Mathf.Min(height, _minHeight);
                _grid[i, j] = height;
                for (int k = 0; k < height; k++)
                {
                    _voxelGrid[i, j, k] = CubeType.Terrain;
                }
            }
        }
    }

    private void RemoveExcessCubes()
    {
        _terreinCubes = new List<CubeData>();
        CubeType[,,] newGrid = new CubeType[_chunkSize, _chunkSize, _terrainHeight];
        for (int i = 0; i < _voxelGrid.GetLength(0); i++) // x
        {
            for (int j = 0; j < _voxelGrid.GetLength(1); j++) // z
            {
                for (int k = 0; k < _voxelGrid.GetLength(2); k++) //y
                {
                    if (HaveAllNeighborsCheck(i, j, k))
                    {
                        newGrid[i, j, k] = CubeType.Inner;
                    }
                    else
                    {
                        newGrid[i, j, k] = _voxelGrid[i, j, k];
                        if (newGrid[i, j, k] == CubeType.Terrain)
                        {
                            _terreinCubes.Add(new CubeData(new Vector3(i, k, j)));
                        }
                    }
                }
            }
        }
        _voxelGrid = newGrid;
    }

    private void RemoveExcessQuads()
    {
        _leftQuads = new Dictionary<Vector3, QuadData>();
        _rightQuads = new Dictionary<Vector3, QuadData>();
        _topQuads = new Dictionary<Vector3, QuadData>();
        _frontQuads = new Dictionary<Vector3, QuadData>();
        _backQuads = new Dictionary<Vector3, QuadData>();

        _frontQuadGrid = new int[_chunkSize, _chunkSize, _terrainHeight];
        _rightQuadGrid = new int[_chunkSize, _chunkSize, _terrainHeight];
        _leftQuadGrid = new int[_chunkSize, _chunkSize, _terrainHeight];
        _topQuadGrid = new int[_chunkSize, _chunkSize, _terrainHeight];
        _backQuadGrid = new int[_chunkSize, _chunkSize, _terrainHeight];

        foreach (var cube in _terreinCubes)
        {
            if (GetVoxel(cube.Position + Vector3.left) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Left).Visible = false;
            }
            else
            {
                Vector3 gridPos = new Vector3(cube.Position.x, cube.Position.z, cube.Position.y);
                _leftQuads.Add(gridPos, cube.GetQuad(QuadPosition.Left));
                _leftQuadGrid[(int)cube.Position.x, (int)cube.Position.z, (int)cube.Position.y] = 1;
            }
            if (GetVoxel(cube.Position + Vector3.right) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Right).Visible = false;
            }
            else
            {
                Vector3 gridPos = new Vector3(cube.Position.x, cube.Position.z, cube.Position.y);
                _rightQuads.Add(gridPos, cube.GetQuad(QuadPosition.Right));
                _rightQuadGrid[(int)cube.Position.x, (int)cube.Position.z, (int)cube.Position.y] = 1;
            }
            if (GetVoxel(cube.Position + Vector3.forward) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Back).Visible = false;
            }
            else
            {
                Vector3 gridPos = new Vector3(cube.Position.x, cube.Position.z, cube.Position.y);
                _backQuads.Add(gridPos, cube.GetQuad(QuadPosition.Back));
                _backQuadGrid[(int)cube.Position.x, (int)cube.Position.z, (int)cube.Position.y] = 1;
            }
            if (GetVoxel(cube.Position + Vector3.back) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Front).Visible = false;
            }
            else
            {
                Vector3 gridPos = new Vector3(cube.Position.x, cube.Position.z, cube.Position.y);
                _frontQuads.Add(gridPos, cube.GetQuad(QuadPosition.Front));
                _frontQuadGrid[(int)cube.Position.x, (int)cube.Position.z, (int)cube.Position.y] = 1;
            }
            if (GetVoxel(cube.Position + Vector3.up) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Top).Visible = false;
            }
            else
            {
                Vector3 gridPos = new Vector3(cube.Position.x, cube.Position.z, cube.Position.y);
                _topQuads.Add(gridPos, cube.GetQuad(QuadPosition.Top));
                _topQuadGrid[(int)cube.Position.x, (int)cube.Position.z, (int)cube.Position.y] = 1;
            }
            if (GetVoxel(cube.Position + Vector3.down) != CubeType.Empty)
            {
                cube.GetQuad(QuadPosition.Bottom).Visible = false;
            }
            else
            {
                // Bot quads always invisible
            }
        }
    }

    private CubeType GetVoxel(Vector3 position) => GetVoxel((int)position.x, (int)position.z, (int)position.y);
    private CubeType GetVoxel(int x, int z, int y)
    {
        if (x < 0 || x > VoxelDimX - 1 || z < 0 || /*z > VoxelDimZ - 1 ||*/ y < 0)
        {
            return CubeType.OutOfBorder;
        }
        if (y > VoxelDimY - 1)
        {
            return CubeType.Empty;
        }
        if (z > VoxelDimZ - 1)
        {
            return CubeType.OutOfBorder;
        }
        return _voxelGrid[x, z, y];
    }
    /// <summary>
    /// X Z Y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool HaveAllNeighborsCheck(int x, int z, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (GetVoxel(x+i,z+j,y+k) == CubeType.Empty)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private void MergeQuads()
    {
        _quadsToBake = new List<QuadData>();

        MergeFrontQuads();
        MergeBackQuads();
        MergeRightQuads();
        MergeLeftQuads();
        MergeTopQuads();
    }

    private void MergeFrontQuads()
    {
        // Merge front quads
        var luFront = _frontQuads.ToLookup(q => q.Key.z, q => q);
        for (int z = 0; z < _chunkSize; z++)
        {
            if (luFront[z] == null || luFront[z].Count() == 0)
            {
                continue;
            }
            var slice = luFront[z].ToList();
            while (slice.Count > 0)
            {
                var current = slice[0];
                slice.Remove(current);
                FrontBackQuadGroup quadGroup = new FrontBackQuadGroup(current.Value);
                _frontQuadGrid[(int)current.Value.Position.x, (int)current.Value.Position.z, (int)current.Value.Position.y] = 0;
                _frontQuads.Remove(current.Key);
                bool canExpand = true;
                while (canExpand)
                {
                    int sqrLeft = quadGroup.CanExpandLeft(_frontQuadGrid);
                    int sqrRight = quadGroup.CanExpandRight(_frontQuadGrid);
                    int sqrTop = quadGroup.CanExpandTop(_frontQuadGrid);
                    int sqrBot = quadGroup.CanExpandBottom(_frontQuadGrid);
                    if (sqrLeft == 0 && sqrRight == 0 && sqrTop == 0 && sqrBot == 0)
                    {
                        // Nowhere to expand
                        canExpand = false;
                        continue;
                    }
                    if (sqrLeft > sqrRight && sqrLeft > sqrTop && sqrLeft > sqrBot)
                    {
                        quadGroup.ExpandLeft(_frontQuadGrid, _frontQuads, slice);
                    }
                    if (sqrRight > sqrLeft && sqrRight > sqrTop && sqrRight > sqrBot)
                    {
                        quadGroup.ExpandRight(_frontQuadGrid, _frontQuads, slice);
                    }
                    if (sqrTop > sqrLeft && sqrTop > sqrRight && sqrTop > sqrBot)
                    {
                        quadGroup.ExpandTop(_frontQuadGrid, _frontQuads, slice);
                    }
                    if (sqrBot > sqrLeft && sqrBot > sqrRight && sqrBot > sqrTop)
                    {
                        quadGroup.ExpandBottom(_frontQuadGrid, _frontQuads, slice);
                    }
                }
                // QuadGroup ready
                _quadsToBake.Add(quadGroup.CombineQuadsFront());
            }

        }
    }

    private void MergeBackQuads()
    {
        // merge back quads

        var luBack = _backQuads.ToLookup(q => q.Key.z, q => q);
        for (int z = 0; z < _chunkSize; z++)
        {
            if (luBack[z] == null || luBack[z].Count() == 0)
            {
                continue;
            }
            var slice = luBack[z].ToList();
            while (slice.Count > 0)
            {
                var current = slice[0];
                slice.Remove(current);
                FrontBackQuadGroup quadGroup = new FrontBackQuadGroup(current.Value);
                _backQuadGrid[(int)current.Value.Position.x, (int)current.Value.Position.z, (int)current.Value.Position.y] = 0;
                _backQuads.Remove(current.Key);
                bool canExpand = true;
                while (canExpand)
                {
                    int sqrLeft = quadGroup.CanExpandLeft(_backQuadGrid);
                    int sqrRight = quadGroup.CanExpandRight(_backQuadGrid);
                    int sqrTop = quadGroup.CanExpandTop(_backQuadGrid);
                    int sqrBot = quadGroup.CanExpandBottom(_backQuadGrid);
                    if (sqrLeft == 0 && sqrRight == 0 && sqrTop == 0 && sqrBot == 0)
                    {
                        // Nowhere to expand
                        canExpand = false;
                        continue;
                    }
                    if (sqrLeft > sqrRight && sqrLeft > sqrTop && sqrLeft > sqrBot)
                    {
                        quadGroup.ExpandLeft(_backQuadGrid, _backQuads, slice);
                    }
                    if (sqrRight > sqrLeft && sqrRight > sqrTop && sqrRight > sqrBot)
                    {
                        quadGroup.ExpandRight(_backQuadGrid, _backQuads, slice);
                    }
                    if (sqrTop > sqrLeft && sqrTop > sqrRight && sqrTop > sqrBot)
                    {
                        quadGroup.ExpandTop(_backQuadGrid, _backQuads, slice);
                    }
                    if (sqrBot > sqrLeft && sqrBot > sqrRight && sqrBot > sqrTop)
                    {
                        quadGroup.ExpandBottom(_backQuadGrid, _backQuads, slice);
                    }
                }
                // QuadGroup ready
                _quadsToBake.Add(quadGroup.CombineQuadsBack());
            }

        }
    }

    private void MergeRightQuads()
    {
        // merge right quads

        var luRight = _rightQuads.ToLookup(q => q.Key.x, q => q);
        for (int x = 0; x < _chunkSize; x++)
        {
            if (luRight[x] == null || luRight[x].Count() == 0)
            {
                continue;
            }
            var slice = luRight[x].ToList();
            while (slice.Count > 0)
            {
                var current = slice[0];
                slice.Remove(current);
                LeftRightQuadGroup quadGroup = new LeftRightQuadGroup(current.Value);
                _rightQuadGrid[(int)current.Value.Position.x, (int)current.Value.Position.z, (int)current.Value.Position.y] = 0;
                _rightQuads.Remove(current.Key);
                bool canExpand = true;
                while (canExpand)
                {
                    int sqrBack = quadGroup.CanExpandBack(_rightQuadGrid);
                    int sqrFront = quadGroup.CanExpandFront(_rightQuadGrid);
                    int sqrTop = quadGroup.CanExpandTop(_rightQuadGrid);
                    int sqrBot = quadGroup.CanExpandBottom(_rightQuadGrid);

                    //Debug.Log($"{current.Key} {sqrBack} {sqrFront} {sqrTop} {sqrBot}");

                    if (sqrBack == 0 && sqrFront == 0 && sqrTop == 0 && sqrBot == 0)
                    {
                        // Nowhere to expand
                        canExpand = false;
                        continue;
                    }
                    if (sqrBack > sqrFront && sqrBack > sqrTop && sqrBack > sqrBot)
                    {
                        quadGroup.ExpandBack(_rightQuadGrid, _rightQuads, slice);
                    }
                    if (sqrFront > sqrBack && sqrFront > sqrTop && sqrFront > sqrBot)
                    {
                        quadGroup.ExpandFront(_rightQuadGrid, _rightQuads, slice);
                    }
                    if (sqrTop > sqrBack && sqrTop > sqrFront && sqrTop > sqrBot)
                    {
                        quadGroup.ExpandTop(_rightQuadGrid, _rightQuads, slice);
                    }
                    if (sqrBot > sqrBack && sqrBot > sqrFront && sqrBot > sqrTop)
                    {
                        quadGroup.ExpandBottom(_rightQuadGrid, _rightQuads, slice);
                    }
                }
                // QuadGroup ready
                _quadsToBake.Add(quadGroup.CombineQuadsRight());
            }

        }
    }

    private void MergeLeftQuads()
    {
        // Merge left quads

        var luLeft = _leftQuads.ToLookup(q => q.Key.x, q => q);
        for (int x = 0; x < _chunkSize; x++)
        {
            if (luLeft[x] == null || luLeft[x].Count() == 0)
            {
                continue;
            }
            var slice = luLeft[x].ToList();
            while (slice.Count > 0)
            {
                var current = slice[0];
                slice.Remove(current);
                LeftRightQuadGroup quadGroup = new LeftRightQuadGroup(current.Value);
                _leftQuadGrid[(int)current.Value.Position.x, (int)current.Value.Position.z, (int)current.Value.Position.y] = 0;
                _leftQuads.Remove(current.Key);
                bool canExpand = true;
                while (canExpand)
                {
                    int sqrBack = quadGroup.CanExpandBack(_leftQuadGrid);
                    int sqrFront = quadGroup.CanExpandFront(_leftQuadGrid);
                    int sqrTop = quadGroup.CanExpandTop(_leftQuadGrid);
                    int sqrBot = quadGroup.CanExpandBottom(_leftQuadGrid);

                    //Debug.Log($"{current.Key} {sqrBack} {sqrFront} {sqrTop} {sqrBot}");

                    if (sqrBack == 0 && sqrFront == 0 && sqrTop == 0 && sqrBot == 0)
                    {
                        // Nowhere to expand
                        canExpand = false;
                        continue;
                    }
                    if (sqrBack > sqrFront && sqrBack > sqrTop && sqrBack > sqrBot)
                    {
                        quadGroup.ExpandBack(_leftQuadGrid, _leftQuads, slice);
                    }
                    if (sqrFront > sqrBack && sqrFront > sqrTop && sqrFront > sqrBot)
                    {
                        quadGroup.ExpandFront(_leftQuadGrid, _leftQuads, slice);
                    }
                    if (sqrTop > sqrBack && sqrTop > sqrFront && sqrTop > sqrBot)
                    {
                        quadGroup.ExpandTop(_leftQuadGrid, _leftQuads, slice);
                    }
                    if (sqrBot > sqrBack && sqrBot > sqrFront && sqrBot > sqrTop)
                    {
                        quadGroup.ExpandBottom(_leftQuadGrid, _leftQuads, slice);
                    }
                }
                // QuadGroup ready
                _quadsToBake.Add(quadGroup.CombineQuadsLeft());
            }

        }
    }

    private void MergeTopQuads()
    {
        // Merge top
        var luTop = _topQuads.ToLookup(q => q.Key.z, q => q);
        for (int z = 0; z < _terrainHeight; z++)
        {
            if (luTop[z] == null || luTop[z].Count() == 0)
            {
                continue;
            }
            var slice = luTop[z].ToList();
            while (slice.Count > 0)
            {
                var current = slice[0];
                slice.Remove(current);
                TopQuadGroup quadGroup = new TopQuadGroup(current.Value);
                _topQuadGrid[(int)current.Value.Position.x, (int)current.Value.Position.z, (int)current.Value.Position.y] = 0;
                _topQuads.Remove(current.Key);
                bool canExpand = true;
                while (canExpand)
                {
                    int sqrBack = quadGroup.CanExpandBack(_topQuadGrid);
                    int sqrFront = quadGroup.CanExpandFront(_topQuadGrid);
                    int sqrRight = quadGroup.CanExpandRight(_topQuadGrid);
                    int sqrLeft = quadGroup.CanExpandLeft(_topQuadGrid);

                    if (sqrBack == 0 && sqrFront == 0 && sqrRight == 0 && sqrLeft == 0)
                    {
                        // Nowhere to expand
                        canExpand = false;
                        continue;
                    }
                    if (sqrRight >= sqrBack && sqrRight >= sqrFront && sqrRight >= sqrLeft)
                    {
                        quadGroup.ExpandRight(_topQuadGrid, _topQuads, slice);
                        continue;
                    }
                    if (sqrLeft >= sqrBack && sqrLeft >= sqrFront && sqrLeft >= sqrRight)
                    {
                        quadGroup.ExpandLeft(_topQuadGrid, _topQuads, slice);
                        continue;
                    }
                    if (sqrBack >= sqrFront && sqrBack >= sqrRight && sqrBack >= sqrLeft)
                    {
                        quadGroup.ExpandBack(_topQuadGrid, _topQuads, slice);
                        continue;
                    }
                    if (sqrFront >= sqrBack && sqrFront >= sqrRight && sqrFront >= sqrLeft)
                    {
                        quadGroup.ExpandFront(_topQuadGrid, _topQuads, slice);
                        continue;
                    }
                }
                // QuadGroup ready
                _quadsToBake.Add(quadGroup.CombineQuads());
            }
        }
    }

    private void BakeQuads()
    {
        Vector3[] vertices = new Vector3[_quadsToBake.Count * 4];
        int[] triangles = new int[_quadsToBake.Count * 6];
        Vector3[] normals = new Vector3[_quadsToBake.Count * 4];
        for (int i = 0; i < _quadsToBake.Count; i++)
        {
            var quadData = _quadsToBake[i];
            for (int j = 0; j < 4; j++)
            {
                vertices[i * 4 + j] = quadData.Points[j];
                normals[i * 4 + j] = quadData.Normal;
            }
            for (int k = 0; k < 6; k++)
            {
                triangles[i * 6 + k] = quadData.Triangles[k] + i * 4;
            }
        }
        GameObject generatedTerrain = new GameObject("GeneratedTerrain");
        var meshFilter = generatedTerrain.AddComponent<MeshFilter>();
        var renderer = generatedTerrain.AddComponent<MeshRenderer>();

        Mesh mesh = meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        renderer.material.color = Color.white;
    }

    private GameObject DisplayCube()
    {
        var cube = Instantiate(_cubeGeneratorPrefab, Vector3.zero, Quaternion.identity);
        cube.Generate(new CubeData(Vector3.zero));
        return cube.gameObject;
    }
    private GameObject DisplayCubicGrid()
    {
        GameObject gridHolder = new GameObject("GridHolder");
        for (int i = 0; i < _voxelGrid.GetLength(0); i++)// x
        {
            for (int j = 0; j < _voxelGrid.GetLength(1); j++)// z 
            {
                for (int k = 0; k < _voxelGrid.GetLength(2); k++)// y
                {
                    if (_voxelGrid[i, j, k] == CubeType.Terrain)
                    {
                        var cube = Instantiate(_cubeGeneratorPrefab, Vector3.zero, Quaternion.identity);
                        cube.transform.parent = gridHolder.transform;
                        cube.Generate(new CubeData(new Vector3(i, k, j)));
                    }
                }
            }
        }
        return gridHolder;
    }
    private GameObject DisplayCubicGridWithCubeSeparation()
    {
        GameObject gridHolder = new GameObject("SeparatedGridHolder");
        for (int i = 0; i < _voxelGrid.GetLength(0); i++)// x
        {
            for (int j = 0; j < _voxelGrid.GetLength(1); j++)// z 
            {
                for (int k = 0; k < _voxelGrid.GetLength(2); k++)// y
                {
                    if (_voxelGrid[i, j, k] == CubeType.Terrain)
                    {
                        var cube = Instantiate(_cubeGeneratorPrefab, Vector3.zero, Quaternion.identity);
                        cube.transform.parent = gridHolder.transform;
                        cube.Generate(new CubeData(new Vector3(i, k, j)));
                        cube.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                    if (_voxelGrid[i, j, k] == CubeType.Inner)
                    {
                        var cube = Instantiate(_cubeGeneratorPrefab, Vector3.zero, Quaternion.identity);
                        cube.transform.parent = gridHolder.transform;
                        cube.Generate(new CubeData(new Vector3(i, k, j)));
                        cube.GetComponent<MeshRenderer>().material.color = Color.black;
                    }
                }
            }
        }
        return gridHolder;
    }

    private GameObject DisplayTerrainCubes()
    {
        GameObject gridHolder = new GameObject("TerrainGridHolder");
        foreach (var cube in _terreinCubes)
        {
            var cubeGenerator = Instantiate(_cubeGeneratorPrefab, Vector3.zero, Quaternion.identity);
            cubeGenerator.transform.parent = gridHolder.transform;
            cubeGenerator.Generate(cube);
        }
        return gridHolder;
    }

    private GameObject DisplaySeparatedQuads()
    {
        GameObject quadsHolder = new GameObject("SeparatedQuadsHolder");
        foreach (var quad in _frontQuads)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad.Value);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        foreach (var quad in _backQuads)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad.Value);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        foreach (var quad in _leftQuads)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad.Value);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.black;
        }
        foreach (var quad in _rightQuads)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad.Value);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        foreach (var quad in _topQuads)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad.Value);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        return quadsHolder;
    }

    private GameObject DisplayMergedQuads()
    {
        GameObject quadsHolder = new GameObject("MergedQuadsHolder");
        foreach (var quad in _quadsToBake)
        {
            var quadGenerator = Instantiate(_quadGeneratorPrefab, Vector3.zero, Quaternion.identity);
            quadGenerator.transform.parent = quadsHolder.transform;
            quadGenerator.Generate(quad);
            quadGenerator.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        return quadsHolder;
    }

    private IEnumerator StepByStepDisplay()
    {
        _camera.transform.position = _cameraPositions[0].position;
        _camera.transform.rotation = _cameraPositions[0].rotation;

        var cube = DisplayCube();
        yield return new WaitForSeconds(2f);
        Destroy(cube);

        _camera.transform.position = _cameraPositions[1].position;
        _camera.transform.rotation = _cameraPositions[1].rotation;

        Generate();
        var grid = DisplayCubicGrid();
        yield return new WaitForSeconds(2f);
        Destroy(grid);

        RemoveExcessCubes();
        grid = DisplayCubicGridWithCubeSeparation();
        yield return new WaitForSeconds(2f);
        Destroy(grid);

        grid = DisplayTerrainCubes();
        yield return new WaitForSeconds(2f);
        Destroy(grid);

        RemoveExcessQuads();
        grid = DisplayTerrainCubes();
        yield return new WaitForSeconds(2f);
        Destroy(grid);



        grid = DisplaySeparatedQuads();
        yield return new WaitForSeconds(2f);
        Destroy(grid);

        MergeQuads();
        grid = DisplayMergedQuads();
        yield return new WaitForSeconds(2f);
        Destroy(grid);

        BakeQuads();
    }
}
