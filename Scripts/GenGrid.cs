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
    }

    void CreateNode()
    {


    }

    GameObject CreateNode(Vector3 pos)
    {
        GameObject node = Instantiate(gridCube, pos, Quaternion.identity);
        node.transform.localScale = new Vector3(world.SizeNode - 0.4f, world.SizeNode - 0.4f, world.SizeNode - 0.4f);
        var gn = node.AddComponent<GridNode>().Init();
        gridNodes.Add(gn);
        return node;
    }

}
