using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TopQuadGroup
{
    public TopQuadGroup(QuadData quad)
    {
        _origin = new Vector3Int((int)quad.Position.x, (int)quad.Position.z, (int)quad.Position.y);
        _originInWorld = quad.Position;
        _quadsToCombine = new Dictionary<Vector3, QuadData>();
        _quadsToCombine.Add(quad.Position, quad);
    }
    private Vector3Int _origin;
    private Vector3 _originInWorld;
    private int _stepsLeft;
    private int _stepsRight;
    private int _stepsFront;
    private int _stepsBack;
    private Dictionary<Vector3, QuadData> _quadsToCombine;

    public int Lenght => _stepsBack + _stepsFront + 1;
    public int Height => _stepsLeft + _stepsRight + 1;
    

    public int CanExpandBack(int[,,] grid)
    {
        if (_origin.y - _stepsBack <= 0)
        {
            return 0;
        }
        for (int i = _origin.x - _stepsLeft; i <= _origin.x + _stepsRight; i++)
        {
            if (grid[i, _origin.y - _stepsBack - 1, _origin.z] == 0)
            {
                return 0;
            }
        }
        return (Lenght + 1) * Height;
    }

    public void ExpandBack(int[,,] grid, Dictionary<Vector3, QuadData> quadDictionary, List<KeyValuePair<Vector3, QuadData>> slice)
    {
        for (int i = _origin.x - _stepsLeft; i <= _origin.x + _stepsRight; i++)
        {

            var positionInGrid = new Vector3(i, _origin.y - _stepsBack - 1, _origin.z);
            grid[i, _origin.y - _stepsBack - 1, _origin.z] = 0;
            var quadToAdd = quadDictionary[positionInGrid];
            _quadsToCombine.Add(quadToAdd.Position, quadToAdd);
            quadDictionary.Remove(positionInGrid);
            var quadInSlice = slice.First(p => p.Key == positionInGrid);
            slice.Remove(quadInSlice);
        }
        _stepsBack++;
    }

    public int CanExpandFront(int[,,] grid)
    {
        if (grid.GetLength(1) <= _origin.y + _stepsFront + 1)
        {
            return 0;
        }
        for (int i = _origin.x - _stepsLeft; i <= _origin.x + _stepsRight; i++)
        {
            if (grid[i, _origin.y + _stepsFront + 1, _origin.z] == 0)
            {
                return 0;
            }
        }
        return (Lenght + 1) * Height;
    }
    public void ExpandFront(int[,,] grid, Dictionary<Vector3, QuadData> quads, List<KeyValuePair<Vector3, QuadData>> slice)
    {
        for (int i = _origin.x - _stepsLeft; i <= _origin.x + _stepsRight; i++)
        {
            var positionInGrid = new Vector3(i, _origin.y + _stepsFront + 1, _origin.z);
            grid[i, _origin.y + _stepsFront + 1, _origin.z] = 0;
            var quadToAdd = quads[positionInGrid];
            _quadsToCombine.Add(quadToAdd.Position, quadToAdd);
            quads.Remove(positionInGrid);
            var quadInSlice = slice.First(p => p.Key == positionInGrid);
            slice.Remove(quadInSlice);
        }
        _stepsFront++;
    }

    public int CanExpandLeft(int[,,] grid)
    {
        if (_origin.x - _stepsLeft <= 0)
        {
            return 0;
        }
        for (int i = _origin.z - _stepsBack; i <= _origin.z + _stepsFront; i++)
        {
            if (grid[_origin.x - _stepsLeft - 1, _origin.y, i] == 0)
            {
                return 0;
            }
        }
        return Lenght * (Height+1);
    }

    public void ExpandLeft(int[,,] grid, Dictionary<Vector3, QuadData> quadDictionary, List<KeyValuePair<Vector3, QuadData>> slice)
    {
        for (int i = _origin.z - _stepsBack; i <= _origin.z + _stepsFront; i++)
        {
            var positionInGrid = new Vector3(_origin.x - _stepsLeft - 1, _origin.y, i);
            grid[_origin.x - _stepsLeft - 1, _origin.y, i] = 0;
            var quadToAdd = quadDictionary[positionInGrid];
            _quadsToCombine.Add(quadToAdd.Position, quadToAdd);
            quadDictionary.Remove(positionInGrid);
            var quadInSlice = slice.First(p => p.Key == positionInGrid);
            slice.Remove(quadInSlice);
        }
        _stepsLeft++;
    }

    public int CanExpandRight(int[,,] grid)
    {
        if (grid.GetLength(0) <= _origin.x + _stepsRight + 1)
        {
            return 0;
        }
        for (int i = _origin.z - _stepsBack; i <= _origin.z + _stepsFront; i++)
        {
            if (grid[_origin.x + _stepsRight + 1, _origin.y, i] == 0)
            {
                return 0;
            }
        }
        return Lenght * (Height + 1);
    }
    public void ExpandRight(int[,,] grid, Dictionary<Vector3, QuadData> quads, List<KeyValuePair<Vector3, QuadData>> slice)
    {
        for (int i = _origin.z - _stepsBack; i <= _origin.z + _stepsFront; i++)
        {
            var positionInGrid = new Vector3(_origin.x + _stepsRight + 1, _origin.y, i);
            grid[_origin.x + _stepsRight + 1, _origin.y, i] = 0;
            var quadToAdd = quads[positionInGrid];
            _quadsToCombine.Add(quadToAdd.Position, quadToAdd);
            quads.Remove(positionInGrid);
            var quadInSlice = slice.First(p => p.Key == positionInGrid);
            slice.Remove(quadInSlice);
        }
        _stepsRight++;
    }

    public QuadData CombineQuads()
    {
        var blQuad = _quadsToCombine[_originInWorld + _stepsBack * new Vector3(0, 0, -1) + _stepsLeft * new Vector3(-1, 0, 0)];
        Vector3 blPoint = blQuad.Points[0];
        var brQuad = _quadsToCombine[_originInWorld + _stepsBack * new Vector3(0, 0, -1) + _stepsRight * new Vector3(1, 0, 0)];
        Vector3 brPoint = brQuad.Points[1];
        var tlQuad = _quadsToCombine[_originInWorld + _stepsFront * new Vector3(0, 0, 1) + _stepsLeft * new Vector3(-1, 0, 0)];
        Vector3 tlPoint = tlQuad.Points[2];
        var trQuad = _quadsToCombine[_originInWorld + _stepsFront * new Vector3(0, 0, 1) + _stepsRight * new Vector3(1, 0, 0)];
        Vector3 trPoint = trQuad.Points[3];
        Vector3[] vertices = new Vector3[4] { blPoint, brPoint, tlPoint, trPoint };
        QuadData combinedData = new QuadData(vertices, new Vector3(0, -1, 0), _originInWorld);
        return combinedData;
    }
}