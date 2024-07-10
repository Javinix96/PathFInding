using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World
{
    private Node[,] grid;

    private Node[,] Grid { get => grid; }

    private List<Node> openList;
    private List<Node> closedList;

    private List<Node> path;

    private Vector2Int actual;
    private Vector2Int PosI;
    private Vector2Int PosF;

    private int sizeW = 0;
    private int sizeH = 0;
    private bool found = false;

    public void InitGrid(Vector2Int posI, Vector2Int posF, Node[,] gridP)
    {
        path = new List<Node>();
        openList = new List<Node>();
        closedList = new List<Node>();
        PosI = posI;
        PosF = posF;
        grid = gridP;
        sizeW = grid.GetUpperBound(0);
        sizeH = grid.GetUpperBound(1);
        closedList.Add(grid[PosI.x, PosI.y]);
        found = false;
        actual = PosI;
    }

    public IEnumerator GeneratePath(Vector2Int posI, Vector2Int posF, Node[,] gridP, Action<List<Node>, List<Node>> callback, Action<List<Node>> callback2)
    {

        InitGrid(posI, posF, gridP);

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
                        if (actual.x == 34 && actual.y == 1)
                        {
                            Debug.Log("llego");
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
                    // callback(openList, closedList);
                    yield return new WaitForSeconds(0.0f);
                }
            }

            if (openList.Count <= 0)
            {
                found = true;
                Debug.Log("No hay path camino bloqueado");
                callback(openList, closedList);
                break;
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


            yield return new WaitForSeconds(0.01f);
        }

        if (openList.Count > 0)
        {
            closedList.Reverse();
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
            callback2(path);
        }
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

    private int GetDistance(Vector2Int a, Vector2Int b)
    {
        return Math.Abs((b.x - a.x)) + Math.Abs((b.y - a.y));
    }

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
