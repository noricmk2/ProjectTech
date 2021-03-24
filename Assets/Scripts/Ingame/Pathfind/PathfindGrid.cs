using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindGrid
{
    private JPSNode[,] _grid;
    private int _width;
    private int _height;
    private List<JPSNode> _nodeList;


    public void Init(int width, int height, List<JPSNode> nodeList)
    {
        _width = width;
        _height = height;
        _nodeList = nodeList;
        _grid = new JPSNode[width, height];
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
                _grid[i, j] = nodeList.Find(x => x.X == i && x.Y == j).Clone();
        }
    }

    public void Reset()
    {
        for (int i = 0; i < _grid.GetLength(0); ++i)
        {
            for (int j = 0; j < _grid.GetLength(1); ++j)
                _grid[i, j].Reset();
        }
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || y < 0 ||
            _grid.GetLength(0) <= x || _grid.GetLength(1) <= y)
            return false;

        if (_grid[x, y].state != 1)
            return false;

        return true;
    }

    public JPSNode GetNode(int x, int y)
    {
        if (x < 0 || y < 0 ||
            _grid.GetLength(0) <= x || _grid.GetLength(1) <= y)
        {
            //Debug.Log("[Failed]not exist index - grid:" + _grid.GetLength(0) + "," + _grid.GetLength(1) + "/pos:" + x + "," + y);
            return null;
        }

        return _grid[x, y];
    }

    public JPSNode GetNode(Vector2Int pos)
    {
        return GetNode(pos.x, pos.y);
    }

    public List<JPSNode> GetNeighborList(JPSNode node, DiagonalMovement diagonalMovement)
    {
        int targetX = node.X;
        int targetY = node.Y;
        var neighborList = new List<JPSNode>();
        bool bottom = false, leftBottom = false,
        right = false, rightBottom = false,
        top = false, rightTop = false,
        left = false, leftTop = false;

        if (IsWalkable(targetX, targetY - 1))
        {
            neighborList.Add(GetNode(targetX, targetY - 1));
            bottom = true;
        }
        if (IsWalkable(targetX + 1, targetY))
        {
            neighborList.Add(GetNode(targetX + 1, targetY));
            right = true;
        }
        if (IsWalkable(targetX, targetY + 1))
        {
            neighborList.Add(GetNode(targetX, targetY + 1));
            top = true;
        }
        if (IsWalkable(targetX - 1, targetY))
        {
            neighborList.Add(GetNode(targetX - 1, targetY));
            left = true;
        }

        switch (diagonalMovement)
        {
            case DiagonalMovement.Always:
                leftBottom = true;
                rightBottom = true;
                rightTop = true;
                leftTop = true;
                break;
            case DiagonalMovement.Never:
                break;
            case DiagonalMovement.IfAtLeastOneWalkable:
                leftBottom = left || bottom;
                rightBottom = bottom || right;
                rightTop = right || top;
                leftTop = top || left;
                break;
            case DiagonalMovement.OnlyWhenNoObstacles:
                leftBottom = left && bottom;
                rightBottom = bottom && right;
                rightTop = right && top;
                leftTop = top && left;
                break;
            default:
                break;
        }

        if (leftBottom && IsWalkable(targetX - 1, targetY - 1))
        {
            neighborList.Add(GetNode(targetX - 1, targetY - 1));
        }
        if (rightBottom && IsWalkable(targetX + 1, targetY - 1))
        {
            neighborList.Add(GetNode(targetX + 1, targetY - 1));
        }
        if (rightTop && IsWalkable(targetX + 1, targetY + 1))
        {
            neighborList.Add(GetNode(targetX + 1, targetY + 1));
        }
        if (leftTop && IsWalkable(targetX - 1, targetY + 1))
        {
            neighborList.Add(GetNode(targetX - 1, targetY + 1));
        }

        return neighborList;
    }
}
