using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenGrid : MonoBehaviour
{
    [SerializeField]
    private List<GridNode> gridNodes;
    [SerializeField]
    private GameObject gridCube;
    [SerializeField]
    private Vector2Int posI;
    [SerializeField]
    private Vector2Int posF;
    [SerializeField]
    private LayerMask noHit;
    [SerializeField]
    private Transform nodesHolder;
    [SerializeField]
    private int sizeNode;

    private World world;
    public int size;

    public int SizeNode { get => sizeNode; }


    public List<GridNode> GridNodes { get => gridNodes; }

    public World World { get => world; }

    void Start()
    {
        world = new World();
        world.InitGrid(posI, posF, size, sizeNode, noHit);
        gridNodes = new List<GridNode>();
        CreateNode();
        WatchPath(World.GetPath());
    }

    public void WatchPath(List<Node> path)
    {
        path.ForEach(n =>
        {
            GridNode no = gridNodes.Find(gn => gn.Id == n.ID);
            if (no != null)
                no.SetColor(new Color(0, 81 / 255, 188 / 255), 1);
        });
    }

    void CreateNode()
    {
        if (world.Grid == null)
            return;

        var nodes = world.Grid.ArrToList<Node>(world.Grid);

        nodes.ForEach(n => CreateNode(n.Pos, n.ID));
    }

    public List<GridNode> GetNodesGrid(Vector2Int pos, float sizeX, float sizeY, List<Node> ph)
    {
        var nodes = world.GetNodesToBuild(pos, sizeX, sizeY, 1);
        List<GridNode> nodesSelected = new List<GridNode>();
        gridNodes.ForEach(no =>
        {
            if (ph != null)
            {
                var ng = ph.Find(n => n.ID == no.Id);
                if (ng == null)
                    no.SetColor(new Color(216 / 255, 216 / 255, 216 / 255), true);
                else
                    no.SetColor(new Color(0, 88 / 255, 188 / 255), 1);
            }
            else
                no.SetColor(new Color(216 / 255, 216 / 255, 216 / 255), false);
        });
        nodes.ForEach(n =>
        {
            GridNode node = gridNodes.Find(n2 => n2.Id == n.ID);
            node.Busy = n.Busy;
            nodesSelected.Add(node);
        });
        return nodesSelected;
    }

    GameObject CreateNode(Vector3 pos, int id)
    {
        GameObject node = Instantiate(gridCube, pos, Quaternion.identity);
        node.transform.SetParent(nodesHolder);
        node.transform.localScale = new Vector3(world.SizeNode - 0.4f, world.SizeNode - 0.4f, world.SizeNode - 0.4f);
        var gn = node.AddComponent<GridNode>();
        gn.Init();
        gridNodes.Add(gn);
        gn.SetColor(new Color(216 / 255, 216 / 255, 216 / 255));
        gn.Id = id;
        return node;
    }

}
