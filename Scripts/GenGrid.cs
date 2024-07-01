using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenGrid : MonoBehaviour
{

    private World world;
    [SerializeField]
    private Node[,] grid;
    [SerializeField]
    private List<Pos> pos;
    [SerializeField]
    private Pos posI;
    [SerializeField]
    private Pos posF;

    [SerializeField]
    private List<Node> nodes;


    public int size;

    // Start is called before the first frame update
    void Start()
    {
        world = new World();
        world.InitGrid(size, pos, posI, posF);
        grid = world.Grid;
        nodes = ArrayToList<Node>(grid);
        world.GeneratePath();
    }

    private List<T> ArrayToList<T>(T[,] arr)
    {
        List<T> list = new List<T>();
        foreach (T e in arr)
            list.Add(e);
        return list;
    }

    private void OnDrawGizmos()
    {
        if (world != null)
            grid = world.Grid;
        for (int i = 0; i < size; i++)
        {
            for (int e = 0; e < size; e++)
            {
                Vector3 n = new Vector3(i, 0, e);
                if (grid == null)
                {

                    Pos pt = new Pos(i, e);
                    if (Contains(pt))
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                        continue;
                    }
                    Gizmos.color = Color.gray;
                    Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                }
                else
                {

                    if (world.path.Contains(grid[i, e]))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                        continue;
                    }
                    if (world.openList.Contains(grid[i, e]))
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                        continue;
                    }
                    if (world.closedList.Contains(grid[i, e]))
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                        continue;
                    }

                    Gizmos.color = grid[i, e].Busy == 1 ? Color.red : Color.gray;
                    Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                }

                if (i == posI.X && e == posI.Y)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                    continue;
                }

                if (i == posF.X && e == posF.Y)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawCube(n, new Vector3(0.8f, 0.8f, 0.8f));
                    continue;
                }
            }
        }
    }


    private bool Contains(Pos ps)
    {
        foreach (Pos p in pos)
        {
            if (p.X == ps.X && p.Y == ps.Y)
                return true;
        }

        return false;

    }
}
