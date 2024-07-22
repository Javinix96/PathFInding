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

    private World world;
    public int size;

    public List<GridNode> GridNodes { get => gridNodes; }

    public World World { get => world; }


    void Start()
    {
        world = new World();
        world.InitGrid(posI, posF, 40, 4, noHit);
        gridNodes = new List<GridNode>();
        CreateNode();
    }

    void CreateNode()
    {
        if (world.Grid == null)
            return;

        var nodes = world.Grid.ArrToList<Node>(world.Grid);

        nodes.ForEach(n => CreateNode(n.Pos, n.ID));
    }

    public List<GridNode> GetNodesGrid(Vector2Int pos, int sizeX, int sizeY)
    {
        var nodes = world.GetNodesToBuild(pos, sizeX, sizeY);
        List<GridNode> nodesSelected = new List<GridNode>();
        gridNodes.ForEach(no =>
                {
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
        node.transform.localScale = new Vector3(world.SizeNode - 0.4f, world.SizeNode - 0.4f, world.SizeNode - 0.4f);
        var gn = node.AddComponent<GridNode>();
        gn.Init();
        gridNodes.Add(gn);
        gn.SetColor(new Color(216 / 255, 216 / 255, 216 / 255));
        gn.Id = id;
        return node;
    }

}
