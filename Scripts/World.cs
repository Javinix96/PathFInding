using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class World
{
    private Node[,] grid;

    private List<Node> openList;
    private List<Node> closedList;

    private List<Node> path;

    private Vector2Int actual;
    private Vector2Int posI;
    private Vector2Int posF;

    private int sizeW = 0;
    private int sizeH = 0;
    private bool found = false;
    private int size;
    private float sizeNode;
    private LayerMask noHit;

    public float SizeNode { get => sizeNode; }
    public Node[,] Grid { get => grid; }
    public Vector2Int PosI { get => posI; }
    public Vector2Int PosF { get => posF; }

    public World()
    {
        path = new List<Node>();

    }

    public void InitGrid(Vector2Int _posI, Vector2Int _posF, int s, float sN, LayerMask nH)
    {
        openList = new List<Node>();
        closedList = new List<Node>();
        posI = _posI;
        posF = _posF;
        found = false;
        actual = PosI;
        size = s;
        grid = new Node[size, size];
        sizeNode = sN;
        noHit = nH;
        StartGrid();
        closedList.Add(grid[PosI.x, PosI.y]);
        sizeW = grid.GetUpperBound(0);
        sizeH = grid.GetUpperBound(1);
    }

    public void InitGrid()
    {
        openList = new List<Node>();
        closedList = new List<Node>();
        found = false;
        actual = PosI;
        grid = new Node[size, size];
        StartGrid();
        closedList.Add(grid[PosI.x, PosI.y]);
    }


    public void InitGrid(int s, float sN)
    {
        size = s;
        grid = new Node[size, size];
        sizeNode = sN;
        StartGrid();
    }

    public Node[,] GridForPos(int s, float sN)
    {

        int id = 0;
        Node[,] tGrid = new Node[s, s];
        for (int i = 0; i < s; i++)
        {
            for (int e = 0; e < s; e++)
            {
                Vector3 pos = new Vector3(i * sN, 1, e * sN);
                Node n = new Node(id, 0, 0, 0, pos, null, 0, new Vector2Int(i, e));
                id++;
                Grid[i, e] = n;
            }
        }

        return tGrid;
    }

    //size X and Y refers to turret size
    public bool CanBuildTurret(Vector2Int pos, float sizeX, float sizeY)
    {
        if (grid == null)
            return true;

        int cX = (int)MathF.Ceiling(sizeX / sizeNode);
        int cY = (int)MathF.Ceiling(sizeY / sizeNode);

        int posIX = pos.x;
        int posIY = pos.y;

        for (int x = posIX; x < (posIX + cX); x++)
            for (int y = posIY; y < (posIY + cY); y++)
                if (grid[x, y].Busy == 1)
                    return false;
        return true;

    }

    public List<Node> GetNodesToBuild(Vector2Int pos, float sizeX, float sizeY)
    {
        if (grid == null)
            return null;

        List<Node> nodes = new List<Node>();
        int cX = (int)MathF.Ceiling(sizeX / sizeNode);
        int cY = (int)MathF.Ceiling(sizeY / sizeNode);

        int posIX = pos.x;
        int posIY = pos.y;

        for (int x = posIX; x < (posIX + cX); x++)
            for (int y = posIY; y < (posIY + cY); y++)
                nodes.Add(grid[x, y]);
        return nodes;

    }

    private void StartGrid()
    {
        int id = 0;
        for (int i = 0; i < size; i++)
        {
            for (int e = 0; e < size; e++)
            {
                Vector3 pos = new Vector3(i * sizeNode, 1, e * sizeNode);
                int busy = Physics.CheckSphere(pos, sizeNode - 1.5f, noHit) ? 1 : 0;
                Node n = new Node(id, 0, 0, 0, pos, null, busy, new Vector2Int(i, e));
                id++;
                grid[i, e] = n;
            }
        }
    }

    public bool CanBuild()
    {
        while (!found)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int pos = new Vector2Int(actual.x + x, actual.y + y);

                    if (pos.x < 0 || pos.y < 0)
                        continue;

                    if (pos.x > sizeW || pos.y > sizeH)
                        continue;

                    if (x == 0 && y == 0)
                        continue;
                    if (grid[pos.x, pos.y].Busy == 1)
                        continue;

                    Node newNode = grid[pos.x, pos.y];

                    if (closedList.Contains(newNode))
                        continue;

                    if (openList.Contains(newNode))
                    {
                        if (openList.Count <= 0)
                        {
                            found = true;
                            Debug.Log("No hay path camino bloqueado");
                            return false;
                        }
                        if (AllCalculated(openList, grid[actual.x, actual.y]))
                        {
                            actual = GetLow(grid[actual.x, actual.y]).PosA;
                            closedList.Add(GetLow(grid[actual.x, actual.y]));
                            openList.Remove(GetLow(grid[actual.x, actual.y]));
                            x = -1;
                            y = -1;
                        }
                        continue;
                    }

                    newNode = CalculateNode(x, y, newNode);

                    openList.Add(newNode);

                    if (openList.Contains(grid[PosF.x, PosF.y]))
                    {
                        closedList.Add(grid[PosF.x, PosF.y]);
                        found = true;
                        break;
                    }
                }
            }

            if (openList.Count <= 0)
            {
                found = true;
                Debug.Log("No hay path camino bloqueado");
                return false;
            }

            if (AllCalculated(openList, grid[actual.x, actual.y]))
            {
                actual = GetLow(grid[actual.x, actual.y]).PosA;
                closedList.Add(GetLow(grid[actual.x, actual.y]));
                openList.Remove(GetLow(grid[actual.x, actual.y]));
            }
            else
            {

                Vector2Int minorPos2 = GetMinor2(openList);
                Node minor = grid[minorPos2.x, minorPos2.y];
                closedList.Add(minor);
                openList.Remove(minor);
                actual = minorPos2;
            }
        }

        return true;
    }

    public List<Node> GetPath(Action<List<Node>, List<Node>> callback = null)
    {
        while (!found)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int pos = new Vector2Int(actual.x + x, actual.y + y);

                    if (pos.x < 0 || pos.y < 0)
                        continue;

                    if (pos.x > sizeW || pos.y > sizeH)
                        continue;

                    if (x == 0 && y == 0)
                        continue;

                    if (grid[pos.x, pos.y].Busy == 1)
                        continue;

                    Node newNode = grid[pos.x, pos.y];

                    if (closedList.Contains(newNode))
                        continue;

                    if (openList.Contains(newNode))
                    {
                        if (AllCalculated(openList, grid[actual.x, actual.y]))
                        {
                            actual = GetLow(grid[actual.x, actual.y]).PosA;
                            closedList.Add(GetLow(grid[actual.x, actual.y]));
                            openList.Remove(GetLow(grid[actual.x, actual.y]));
                            x = -1;
                            y = -1;
                        }
                        continue;
                    }

                    newNode = CalculateNode(x, y, newNode);

                    openList.Add(newNode);

                    if (openList.Contains(grid[PosF.x, PosF.y]))
                    {
                        closedList.Add(grid[PosF.x, PosF.y]);
                        found = true;
                        break;
                    }
                }
            }

            if (openList.Count <= 0)
            {
                found = true;
                Debug.Log("No hay path camino bloqueado");
                callback(openList, closedList);
                return closedList;
            }

            if (AllCalculated(openList, grid[actual.x, actual.y]))
            {
                actual = GetLow(grid[actual.x, actual.y]).PosA;
                closedList.Add(GetLow(grid[actual.x, actual.y]));
                openList.Remove(GetLow(grid[actual.x, actual.y]));
            }
            else
            {

                Vector2Int minorPos2 = GetMinor2(openList);
                Node minor = grid[minorPos2.x, minorPos2.y];
                closedList.Add(minor);
                openList.Remove(minor);
                actual = minorPos2;
            }
        }

        if (openList.Count > 0)
        {
            closedList.Reverse();
            path = new List<Node>();
            path.Add(closedList[0]);
            Node e = closedList[0].Parent;

            while (e.Parent != null)
            {
                path.Add(e);
                e = e.Parent;
            }

            path.Add(grid[PosI.x, PosI.y]);

            callback(openList, closedList);
            path.Reverse();
        }

        return path;
    }


    private Node CalculateNode(int x, int y, Node newNode)
    {
        Node node = newNode;

        newNode.H = GetDistance(newNode.PosA, PosF) * 10;

        if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
            newNode.G = grid[actual.x, actual.y].G + 14;
        else
            newNode.G = grid[actual.x, actual.y].G + 10;

        newNode.F = newNode.G + newNode.H;
        newNode.Parent = grid[actual.x, actual.y];

        return node;
    }

    private bool AllCalculated(List<Node> open, Node newN)
    {
        bool ans = true;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int pos = new Vector2Int(newN.PosA.x + x, newN.PosA.y + y);

                if (pos.x < 0 || pos.y < 0)
                    continue;

                if (pos.x > sizeW || pos.y > sizeH)
                    continue;

                if (grid[pos.x, pos.y].Busy == 1)
                    continue;

                Node newNode = grid[pos.x, pos.y];

                if (closedList.Contains(newNode))
                {
                    continue;
                }

                if (!open.Contains(newNode))
                    ans = false;
                else
                    ans = true;
            }
        }
        return ans;
    }

    private Node GetLow(Node act)
    {
        List<Node> nei = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int pos = new Vector2Int(act.PosA.x + x, act.PosA.y + y);

                if (pos.x < 0 || pos.y < 0)
                    continue;

                if (pos.x > sizeW || pos.y > sizeH)
                    continue;

                if (grid[pos.x, pos.y].Busy == 1)
                    continue;

                if (x == 0 && y == 0)
                    continue;

                Node newNode = grid[pos.x, pos.y];

                if (closedList.Contains(newNode))
                    continue;

                if (openList.Contains(newNode))
                    continue;

                int sum = 0;
                if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
                {
                    sum = grid[act.PosA.x, act.PosA.y].G + 14;
                    if (sum < grid[pos.x, pos.y].G)
                    {

                        openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e =>
                        {
                            e.Parent = grid[act.PosA.y, act.PosA.y];
                            e.G = sum;
                            e.H = GetDistance(pos, PosF) * 10;
                            e.F = e.G + e.H;
                        });
                    }
                }
                else
                {
                    sum = grid[act.PosA.x, act.PosA.y].G + 10;
                    if (sum < grid[pos.x, pos.y].G)
                    {
                        openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e =>
                        {
                            e.Parent = grid[act.PosA.x, act.PosA.y];
                            e.G = sum;
                            e.H = GetDistance(pos, PosF) * 10;
                            e.F = e.G + e.H;
                        });
                    }
                }
            }
        }

        return grid[GetMinor2(openList).x, GetMinor2(openList).y];
    }

    private int GetDistance(Vector2Int a, Vector2Int b) => Math.Abs((b.x - a.x)) + Math.Abs((b.y - a.y));

    private Vector2Int GetMinor(List<Node> open)
    {
        int min = int.MaxValue;
        Vector2Int n = new Vector2Int(open[0].PosA.x, open[0].PosA.y);

        for (int i = 0; i < open.Count; i++)
        {
            if (min < open[i].F)
            {
                min = open[i].F;
                n = open[i].PosA;
            }
        }
        return n;
    }

    private Vector2Int GetMinor2(List<Node> open)
    {
        var s = open.OrderBy(d => d.F).ToList();
        return s[0].PosA;
    }
}

public static class ExtensionNode
{
    public static List<T> ArrToList<T>(this T[,] rer, T[,] arr)
    {
        List<T> nodes = new List<T>();

        foreach (T n in arr)
            nodes.Add(n);
        return nodes;
    }
}
