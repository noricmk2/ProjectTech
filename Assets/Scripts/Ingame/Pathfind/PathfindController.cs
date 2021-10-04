using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TCUtil;
using System;
using Random = UnityEngine.Random;

public class JPSNode
{
    public JPSNode parent;
    public bool closed;
    public int X;
    public int Y;
    public float G;
    public float? H = null;
    public float F;
    public int state;
    public Vector2Int pos;
    
    public JPSNode(int x, int y, int state)
    {
        this.X = x;
        this.Y = y;
        this.state = state;
        pos = new Vector2Int(x, y);
        Reset();
    }

    public void Reset()
    {
        G = 0;
        H = null;
        F = 0;
        parent = null;
        closed = false;
    }

    public JPSNode Clone()
    {
        var clone = new JPSNode(this.X, this.Y, this.state);
        clone.G = this.G;
        clone.H = this.H;
        clone.F = this.F;

        return clone;
    }
}

public enum DiagonalMovement
{
    Always,
    Never,
    IfAtLeastOneWalkable,
    OnlyWhenNoObstacles
}

public enum HeuristicType
{
    Manhattan,
    Euclid,
    Octile,
}

public class PathfindController 
{
    private List<JPSNode> _closeList = new List<JPSNode>();
    private List<JPSNode> _openList = new List<JPSNode>();
    private PathfindGrid _grid;
    private DiagonalMovement _diagonalMovement;
    private HeuristicType _heuristicType;

    public void Init(PathfindGrid grid)
    {
        _grid = grid;
    }

    public void SetDiagonalMovement(DiagonalMovement type)
    {
        _diagonalMovement = type;
    }

    public void Reset()
    {
        _grid.Reset();
    }

    public List<JPSNode> GetRandomPathInRange(Vector2Int start, int range)
    {
        var targetList = GetAllNodeInRange(start, range);
        for (int i = 0; i < targetList.Count;)
        {
            var rand = Random.Range(0, targetList.Count);
            var path = FindPath(start, targetList[rand].pos);
            if (path != null && path.Count > 0)
                return path;
            else
                targetList.RemoveAt(rand);
        }

        return null;
    }

    public List<JPSNode> GetAllNodeInRange(Vector2Int start, int range)
    {
        var list = new List<JPSNode>();
        int xCount = 0;
        for (int i = start.x - range; i < start.x + range; ++i)
        {
            for (int j = start.y - xCount; j < start.y + xCount; ++j)
            {
                var target = _grid.GetNode(i, j);
                if (target != null && target.state != (int)MapNodeType.Block && target.pos != start)
                    list.Add(target);
            }
            ++xCount;
        }
        return list;
    }

    public List<JPSNode> FindPath(Vector2Int start, Vector2Int target)
    {
        Reset();
        _openList.Clear();
        _closeList.Clear();

        var startNode = _grid.GetNode(start);
        var targetNode = _grid.GetNode(target);
        startNode.G = 0;
        startNode.F = 0;

        _openList.Add(startNode);

        while (_openList.Count > 0)
        {
            var node = _openList.PopFirst();
            node.closed = true;

            if (node == targetNode)
                return Backtrace(node);
            else
                Search(node, targetNode, _grid);
        }

        return null;
    }

    private void Search(JPSNode curNode, JPSNode targetNode, PathfindGrid grid)
    {
        var neighborList = FindNeighborList(curNode, grid);
        for (int i = 0; i < neighborList.Count; ++i)
        {
            var jumpPoint = Jump(grid, neighborList[i], curNode, targetNode);
            if (jumpPoint != null)
            {
                if (jumpPoint.closed)
                    continue;

                float nodeToJump = CalHeuristic(Mathf.Abs(jumpPoint.X - curNode.X), Mathf.Abs(jumpPoint.Y - curNode.Y));
                float startToJump = curNode.G + nodeToJump;

                if (!_openList.Contains(jumpPoint) || startToJump < jumpPoint.G)
                {
                    jumpPoint.G = startToJump;
                    jumpPoint.H = jumpPoint.H == null ? CalHeuristic(Mathf.Abs(jumpPoint.X - targetNode.X), Mathf.Abs(jumpPoint.Y - targetNode.Y)) : jumpPoint.H;
                    jumpPoint.F = jumpPoint.G + jumpPoint.H.Value;
                    jumpPoint.parent = curNode;

                    if (!_openList.Contains(jumpPoint))
                        _openList.Add(jumpPoint);
                }
            }
        }
    }

    #region Jump
    private JPSNode Jump(PathfindGrid grid, JPSNode node, JPSNode parent, JPSNode target)
    {
        if (node == null || !grid.IsWalkable(node.X, node.Y))
            return null;
        else if (grid.GetNode(node.X, node.Y) == target)
            return grid.GetNode(node.X, node.Y);

        int directionX = node.X - parent.X;
        int directionY = node.Y - parent.Y;
        if (_diagonalMovement == DiagonalMovement.Always || _diagonalMovement == DiagonalMovement.IfAtLeastOneWalkable)
        {
            // check for forced neighbors
            // along the diagonal
            if (directionX != 0 && directionY != 0)
            {
                if ((grid.IsWalkable(node.X - directionX, node.Y + directionY) && !grid.IsWalkable(node.X - directionX, node.Y)) ||
                    (grid.IsWalkable(node.X + directionX, node.Y - directionY) && !grid.IsWalkable(node.X, node.Y - directionY)))
                {
                    return grid.GetNode(node.X, node.Y);
                }
            }
            // horizontally/vertically
            else
            {
                if (directionX != 0)
                {
                    // moving along x
                    if ((grid.IsWalkable(node.X + directionX, node.Y + 1) && !grid.IsWalkable(node.X, node.Y + 1)) ||
                        (grid.IsWalkable(node.X + directionX, node.Y - 1) && !grid.IsWalkable(node.X, node.Y - 1)))
                    {
                        return grid.GetNode(node.X, node.Y);
                    }
                }
                else
                {
                    if ((grid.IsWalkable(node.X + 1, node.Y + directionY) && !grid.IsWalkable(node.X + 1, node.Y)) ||
                        (grid.IsWalkable(node.X - 1, node.Y + directionY) && !grid.IsWalkable(node.X - 1, node.Y)))
                    {
                        return grid.GetNode(node.X, node.Y);
                    }
                }
            }
            // when moving diagonally, must check for vertical/horizontal jump points
            if (directionX != 0 && directionY != 0)
            {
                if (Jump(grid, grid.GetNode(node.X + directionX, node.Y), node, target) != null)
                {
                    return grid.GetNode(node.X, node.Y);
                }
                if (Jump(grid, grid.GetNode(node.X, node.Y + directionY), node, target) != null)
                {
                    return grid.GetNode(node.X, node.Y);
                }
            }

            // moving diagonally, must make sure one of the vertical/horizontal
            // neighbors is open to allow the path
            if (grid.IsWalkable(node.X + directionX, node.Y) || grid.IsWalkable(node.X, node.Y + directionY))
            {
                return Jump(grid, grid.GetNode(node.X + directionX, node.Y + directionY), node, target);
            }
            else if (_diagonalMovement == DiagonalMovement.Always)
            {
                return Jump(grid, grid.GetNode(node.X + directionX, node.Y + directionY), node, target);
            }
            else
            {
                return null;
            }
        }
        else if (_diagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
        {
            // check for forced neighbors
            // along the diagonal
            if (directionX != 0 && directionY != 0)
            {
                if (grid.IsWalkable(node.X + directionX, node.Y + directionY) && (!grid.IsWalkable(node.X, node.Y + directionY) || !grid.IsWalkable(node.X + directionX, node.Y)))
                {
                    return grid.GetNode(node.X, node.Y);
                }
            }
            // horizontally/vertically
            else
            {
                if (directionX != 0)
                {
                    // moving along x
                    if ((grid.IsWalkable(node.X, node.Y + 1) && !grid.IsWalkable(node.X - directionX, node.Y + 1)) ||
                        (grid.IsWalkable(node.X, node.Y - 1) && !grid.IsWalkable(node.X - directionX, node.Y - 1)))
                    {
                        return grid.GetNode(node.X, node.Y);
                    }
                }
                else
                {
                    if ((grid.IsWalkable(node.X + 1, node.Y) && !grid.IsWalkable(node.X + 1, node.Y - directionY)) ||
                        (grid.IsWalkable(node.X - 1, node.Y) && !grid.IsWalkable(node.X - 1, node.Y - directionY)))
                    {
                        return grid.GetNode(node.X, node.Y);
                    }
                }
            }


            // when moving diagonally, must check for vertical/horizontal jump points
            if (directionX != 0 && directionY != 0)
            {
                if (Jump(grid, grid.GetNode(node.X + directionX, node.Y), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
                if (Jump(grid, grid.GetNode(node.X, node.Y + directionY), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
            }

            // moving diagonally, must make sure both of the vertical/horizontal
            // neighbors is open to allow the path
            if (grid.IsWalkable(node.X + directionX, node.Y) && grid.IsWalkable(node.X, node.Y + directionY))
            {
                return Jump(grid, grid.GetNode(node.X + directionX, node.Y + directionY), node, target);
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (directionX != 0)
            {
                // moving along x
                if (!grid.IsWalkable(node.X + directionX, node.Y))
                {
                    return grid.GetNode(node.X, node.Y);
                }
            }
            else
            {
                if (!grid.IsWalkable(node.X, node.Y + directionY))
                {
                    return grid.GetNode(node.X, node.Y);
                }
            }

            //  must check for perpendicular jump points
            if (directionX != 0)
            {
                if (Jump(grid, grid.GetNode(node.X, node.Y + 1), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
                if (Jump(grid, grid.GetNode(node.X, node.Y - 1), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
            }
            else // tDy != 0
            {
                if (Jump(grid, grid.GetNode(node.X + 1, node.Y), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
                if (Jump(grid, grid.GetNode(node.X - 1, node.Y), node, target) != null)
                    return grid.GetNode(node.X, node.Y);
            }

            // keep going
            if (grid.IsWalkable(node.X + directionX, node.Y) && grid.IsWalkable(node.X, node.Y + directionY))
            {
                return Jump(grid, grid.GetNode(node.X + directionX, node.Y + directionY), node, target);
            }
            else
            {
                return null;
            }
        }
    }
    #endregion

    #region FindNeighbor
    private List<JPSNode> FindNeighborList(JPSNode node, PathfindGrid grid)
    {
        int directionX, directionY;
        List<JPSNode> neighborList;
        JPSNode parent = node.parent;

        // directed pruning: can ignore most neighbors, unless forced.
        if (parent != null)
        {
            neighborList = new List<JPSNode>();
            // get the normalized direction of travel
            directionX = (node.X - parent.X) / Math.Max(Math.Abs(node.X - parent.X), 1);
            directionY = (node.Y - parent.Y) / Math.Max(Math.Abs(node.Y - parent.Y), 1);

            if (_diagonalMovement == DiagonalMovement.Always || _diagonalMovement == DiagonalMovement.IfAtLeastOneWalkable)
            {
                // search diagonally
                if (directionX != 0 && directionY != 0)
                {
                    if (grid.IsWalkable(node.X, node.Y + directionY))
                    {
                        neighborList.Add(grid.GetNode(node.X, node.Y + directionY));
                    }
                    if (grid.IsWalkable(node.X + directionX, node.Y))
                    {
                        neighborList.Add(grid.GetNode(node.X + directionX, node.Y));
                    }

                    if (grid.IsWalkable(node.X + directionX, node.Y + directionY))
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY) || grid.IsWalkable(node.X + directionX, node.Y))
                        {
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y + directionY));
                        }
                        else if (_diagonalMovement == DiagonalMovement.Always)
                        {
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y + directionY));
                        }
                    }

                    if (grid.IsWalkable(node.X - directionX, node.Y + directionY) && !grid.IsWalkable(node.X - directionX, node.Y))
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY))
                        {
                            neighborList.Add(grid.GetNode(node.X - directionX, node.Y + directionY));
                        }
                        else if (_diagonalMovement == DiagonalMovement.Always)
                        {
                            neighborList.Add(grid.GetNode(node.X - directionX, node.Y + directionY));
                        }
                    }

                    if (grid.IsWalkable(node.X + directionX, node.Y - directionY) && !grid.IsWalkable(node.X, node.Y - directionY))
                    {
                        if (grid.IsWalkable(node.X + directionX, node.Y))
                        {
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y - directionY));
                        }
                        else if (_diagonalMovement == DiagonalMovement.Always)
                        {
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y - directionY));
                        }
                    }
                }
                // search horizontally/vertically
                else
                {
                    if (directionX != 0)
                    {
                        if (grid.IsWalkable(node.X + directionX, node.Y))
                        {

                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y));

                            if (grid.IsWalkable(node.X + directionX, node.Y + 1) && !grid.IsWalkable(node.X, node.Y + 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y + 1));
                            }
                            if (grid.IsWalkable(node.X + directionX, node.Y - 1) && !grid.IsWalkable(node.X, node.Y - 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y - 1));
                            }
                        }
                        else if (_diagonalMovement == DiagonalMovement.Always)
                        {
                            if (grid.IsWalkable(node.X + directionX, node.Y + 1) && !grid.IsWalkable(node.X, node.Y + 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y + 1));
                            }
                            if (grid.IsWalkable(node.X + directionX, node.Y - 1) && !grid.IsWalkable(node.X, node.Y - 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y - 1));
                            }
                        }
                    }
                    else
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY))
                        {
                            neighborList.Add(grid.GetNode(node.X, node.Y + directionY));

                            if (grid.IsWalkable(node.X + 1, node.Y + directionY) && !grid.IsWalkable(node.X + 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X + 1, node.Y + directionY));
                            }
                            if (grid.IsWalkable(node.X - 1, node.Y + directionY) && !grid.IsWalkable(node.X - 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X - 1, node.Y + directionY));
                            }
                        }
                        else if (_diagonalMovement == DiagonalMovement.Always)
                        {
                            if (grid.IsWalkable(node.X + 1, node.Y + directionY) && !grid.IsWalkable(node.X + 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X + 1, node.Y + directionY));
                            }
                            if (grid.IsWalkable(node.X - 1, node.Y + directionY) && !grid.IsWalkable(node.X - 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X - 1, node.Y + directionY));
                            }
                        }
                    }
                }
            }
            else if (_diagonalMovement == DiagonalMovement.OnlyWhenNoObstacles)
            {
                // search diagonally
                if (directionX != 0 && directionY != 0)
                {
                    if (grid.IsWalkable(node.X, node.Y + directionY))
                    {
                        neighborList.Add(grid.GetNode(node.X, node.Y + directionY));
                    }
                    if (grid.IsWalkable(node.X + directionX, node.Y))
                    {
                        neighborList.Add(grid.GetNode(node.X + directionX, node.Y));
                    }

                    if (grid.IsWalkable(node.X + directionX, node.Y + directionY))
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY) && grid.IsWalkable(node.X + directionX, node.Y))
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y + directionY));
                    }

                    if (grid.IsWalkable(node.X - directionX, node.Y + directionY))
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY) && grid.IsWalkable(node.X - directionX, node.Y))
                            neighborList.Add(grid.GetNode(node.X - directionX, node.Y + directionY));
                    }

                    if (grid.IsWalkable(node.X + directionX, node.Y - directionY))
                    {
                        if (grid.IsWalkable(node.X, node.Y - directionY) && grid.IsWalkable(node.X + directionX, node.Y))
                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y - directionY));
                    }


                }
                // search horizontally/vertically
                else
                {
                    if (directionX != 0)
                    {
                        if (grid.IsWalkable(node.X + directionX, node.Y))
                        {

                            neighborList.Add(grid.GetNode(node.X + directionX, node.Y));

                            if (grid.IsWalkable(node.X + directionX, node.Y + 1) && grid.IsWalkable(node.X, node.Y + 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y + 1));
                            }
                            if (grid.IsWalkable(node.X + directionX, node.Y - 1) && grid.IsWalkable(node.X, node.Y - 1))
                            {
                                neighborList.Add(grid.GetNode(node.X + directionX, node.Y - 1));
                            }
                        }
                        if (grid.IsWalkable(node.X, node.Y + 1))
                            neighborList.Add(grid.GetNode(node.X, node.Y + 1));
                        if (grid.IsWalkable(node.X, node.Y - 1))
                            neighborList.Add(grid.GetNode(node.X, node.Y - 1));
                    }
                    else
                    {
                        if (grid.IsWalkable(node.X, node.Y + directionY))
                        {
                            neighborList.Add(grid.GetNode(node.X, node.Y + directionY));

                            if (grid.IsWalkable(node.X + 1, node.Y + directionY) && grid.IsWalkable(node.X + 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X + 1, node.Y + directionY));
                            }
                            if (grid.IsWalkable(node.X - 1, node.Y + directionY) && grid.IsWalkable(node.X - 1, node.Y))
                            {
                                neighborList.Add(grid.GetNode(node.X - 1, node.Y + directionY));
                            }
                        }
                        if (grid.IsWalkable(node.X + 1, node.Y))
                            neighborList.Add(grid.GetNode(node.X + 1, node.Y));
                        if (grid.IsWalkable(node.X - 1, node.Y))
                            neighborList.Add(grid.GetNode(node.X - 1, node.Y));
                    }
                }
            }
            else
            {
                if (directionX != 0)
                {
                    if (grid.IsWalkable(node.X + directionX, node.Y))
                    {
                        neighborList.Add(grid.GetNode(node.X + directionX, node.Y));
                    }
                    if (grid.IsWalkable(node.X, node.Y + 1))
                    {
                        neighborList.Add(grid.GetNode(node.X, node.Y + 1));
                    }
                    if (grid.IsWalkable(node.X, node.Y - 1))
                    {
                        neighborList.Add(grid.GetNode(node.X, node.Y - 1));
                    }
                }
                else
                {
                    if (grid.IsWalkable(node.X, node.Y + directionY))
                    {
                        neighborList.Add(grid.GetNode(node.X, node.Y + directionY));
                    }
                    if (grid.IsWalkable(node.X + 1, node.Y))
                    {
                        neighborList.Add(grid.GetNode(node.X + 1, node.Y));
                    }
                    if (grid.IsWalkable(node.X - 1, node.Y))
                    {
                        neighborList.Add(grid.GetNode(node.X - 1, node.Y));
                    }
                }
            }
        }
        // return all neighbors
        else
        {
            neighborList = grid.GetNeighborList(node, _diagonalMovement);
        }
        return neighborList;
    }
    #endregion

    private List<JPSNode> Backtrace(JPSNode node)
    {
        var list = new List<JPSNode>();
        while (node.parent != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }

    private JPSNode Jump()
    {
        return null;
    }

    private static readonly float sqrt_two = Mathf.Sqrt(2);

    public float CalHeuristic(float diffX, float diffY)
    {
        float result = 0;

        switch (_heuristicType)
        {
            case HeuristicType.Manhattan:
                result = diffX + diffY;
                break;
            case HeuristicType.Euclid:
                result = Mathf.Sqrt(diffX * diffX + diffX * diffY);
                break;
            case HeuristicType.Octile:
                result = diffX < diffY ? sqrt_two * diffX + diffY : sqrt_two * diffY + diffX;
                break;
        }

        return result;
    }
}



