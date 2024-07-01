using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class World
{
    private Node[,] grid;

    public Node[,] Grid { get => grid; }

    public List<Node> openList;
    public List<Node> closedList;

    public List<Node> path;


    private Pos actual;
    private Pos PosI;
    private Pos PosF;

    public void InitGrid(int size, List<Pos> posBlocked, Pos posI, Pos posF)
    {
        path = new List<Node>();
        openList = new List<Node>();
        closedList = new List<Node>();
        grid = new Node[size, size];
        PosI = posI;
        PosF = posF;
        int id = 0;
        for (int i = 0; i < size; i++)
        {
            for (int e = 0; e < size; e++)
            {
                Pos p = new Pos(i, e);
                grid[i, e] = new Node(id, 0, 0, 0, new Vector3(i, 0, e), null, 0, p);
                id++;
            }
        }
        posBlocked.ForEach(pos => grid[pos.X, pos.Y].Busy = 1);
    }

    public IEnumerator GeneratePath()
    {
        int sizeW = grid.GetUpperBound(0);
        int sizeH = grid.GetUpperBound(1);

        closedList.Add(grid[PosI.X, PosI.Y]);
        actual = PosI;
        bool found = false;
        while (!found)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int pos = new Vector2Int(actual.X + x, actual.Y + y);

                    if (pos.x < 0 || pos.y < 0)
                        continue;

                    if (pos.x > sizeW || pos.y > sizeH)
                        continue;

                    if (grid[pos.x, pos.y].Busy == 1)
                        continue;

                    Node newNode = grid[pos.x, pos.y];

                    if (closedList.Contains(newNode))
                        continue;
                    if (openList.Contains(newNode))
                    {
                        // int sum = 0;
                        // if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
                        // {
                        //     sum = grid[actual.X, actual.Y].G + 14;
                        //     if (sum < grid[pos.x, pos.y].G)
                        //     {
                        //         openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e => e.Parent = grid[newNode.PosA.X, newNode.PosA.Y]);
                        //     }
                        // }
                        // else
                        // {
                        //     int G = grid[pos.x, pos.y].G;
                        //     sum = grid[actual.X, actual.Y].G + 10;
                        //     if (sum < G)
                        //     {
                        //         openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e => e.Parent = grid[newNode.PosA.X, newNode.PosA.Y]);
                        //     }
                        // }
                        if (AllCalculated(openList, grid[actual.X, actual.Y]))
                        {
                            actual = GetLow(grid[actual.X, actual.Y]).PosA;
                            closedList.Add(GetLow(grid[actual.X, actual.Y]));
                            openList.Remove(GetLow(grid[actual.X, actual.Y]));
                        }
                        continue;
                    }


                    Pos posAct = new Pos(pos.x, pos.y);

                    newNode.H = GetDistance(newNode.PosA, PosF) * 10;

                    if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
                        newNode.G = grid[actual.X, actual.Y].G + 14;
                    else
                        newNode.G = grid[actual.X, actual.Y].G + 10;

                    newNode.F = newNode.G + newNode.H;
                    newNode.Parent = grid[actual.X, actual.Y];

                    openList.Add(newNode);

                    if (openList.Contains(grid[PosF.X, PosF.Y]))
                    {
                        closedList.Add(grid[PosF.X, PosF.Y]);
                        found = true;
                        break;
                    }
                }
            }


            if (openList.Count <= 0)
            {
                found = true;
                Debug.Log("No hay path camino bloqueado");
                break;
            }

            Pos minorPos2 = GetMinor2(openList);
            Node minor = grid[minorPos2.X, minorPos2.Y];
            closedList.Add(minor);
            openList.Remove(minor);
            actual = minorPos2;
            yield return new WaitForSeconds(0.02f);
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

            path.Add(grid[PosI.X, PosI.Y]);

            Debug.Log(GetDistance(PosI, PosF));
        }
    }

    private bool AllCalculated(List<Node> open, Node newN)
    {
        bool ans = true;
        int sizeW = grid.GetUpperBound(0);
        int sizeH = grid.GetUpperBound(1);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int pos = new Vector2Int(newN.PosA.X + x, newN.PosA.Y + y);

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
        int sizeW = grid.GetUpperBound(0);
        int sizeH = grid.GetUpperBound(1);
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int pos = new Vector2Int(act.PosA.X + x, act.PosA.Y + y);

                if (pos.x < 0 || pos.y < 0)
                    continue;

                if (pos.x > sizeW || pos.y > sizeH)
                    continue;

                if (grid[pos.x, pos.y].Busy == 1)
                    continue;

                if (grid[pos.x, pos.y].Busy == 1)
                    continue;

                Node newNode = grid[pos.x, pos.y];

                if (closedList.Contains(newNode))
                    continue;

                int sum = 0;
                if (Math.Abs(x) == 1 && Math.Abs(y) == 1)
                {
                    sum = grid[act.PosA.X, act.PosA.Y].G + 14;
                    if (sum < grid[pos.x, pos.y].G)
                    {

                        openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e =>
                        {
                            e.Parent = grid[act.PosA.X, act.PosA.Y];
                            e.G = sum;
                            e.H = GetDistance(new Pos(pos.x, pos.y), PosF) * 10;
                            e.F = e.G + e.H;
                        });
                    }
                }
                else
                {
                    sum = grid[act.PosA.X, act.PosA.Y].G + 10;
                    if (sum < grid[pos.x, pos.y].G)
                    {
                        openList.Where(o => o.ID == grid[pos.x, pos.y].ID).ToList().ForEach(e =>
                        {
                            e.Parent = grid[act.PosA.X, act.PosA.Y];
                            e.G = sum;
                            e.H = GetDistance(new Pos(pos.x, pos.y), PosF) * 10;
                            e.F = e.G + e.H;
                        });
                    }
                }
            }
        }

        return grid[GetMinor(openList).X, GetMinor(openList).Y];
    }

    private int GetDistance(Pos a, Pos b)
    {
        return Math.Abs((b.X - a.X)) + Math.Abs((b.Y - a.Y));
    }

    private Pos GetMinor(List<Node> open)
    {
        int min = int.MaxValue;
        Pos n = new Pos(open[0].PosA.X, open[0].PosA.Y);

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

    private Pos GetMinor2(List<Node> open)
    {
        var s = open.OrderBy(d => d.F).ToList();
        return s[0].PosA;
    }
}

[Serializable]
public class Pos
{
    public int X;

    public int Y;

    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }
}
