using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

public class CreateTurret : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject[] turrets;
    [SerializeField]
    private GenGrid grid;
    private List<Node> path;

    private GameObject currentTurret;
    private bool canBuild;

    void CreateRay()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 3000))
            if (currentTurret != null)
                currentTurret.transform.position = GetCords(hit.point);

    }

    private Vector3 GetCords(Vector3 hit)
    {
        int x = (int)(hit.x / grid.World.SizeNode);
        int y = (int)(hit.z / grid.World.SizeNode);

        Vector2Int pos = new Vector2Int(x, y);

        if (currentTurret == null)
            return Vector3.zero;

        if (!grid.World.isOnBounds(x, y))
            return Vector3.zero;

        canBuild = grid.World.CanBuildTurret(pos, currentTurret.transform.localScale.x, currentTurret.transform.localScale.z, 1);

        var nodes = grid.GetNodesGrid(pos, currentTurret.transform.localScale.x, currentTurret.transform.localScale.z, path);

        nodes.ForEach(n =>
        {
            if (n.Busy == 1)
                n.SetColor(new Color(255 / 255, 0, 25 / 255), true);
            else
                n.SetColor(new Color(117 / 255, 255 / 255, 0), true);
        });

        if (grid.World.Grid == null)
        {
            canBuild = false;
            return Vector3.zero;
        }

        return new Vector3((grid.World.Grid[x, y].Pos.x - (grid.SizeNode / 2)) + (currentTurret.transform.localScale.x / 2), currentTurret.transform.localScale.y / 2, (grid.World.Grid[x, y].Pos.z - (grid.SizeNode / 2)) + (currentTurret.transform.localScale.z / 2));
    }

    private void Update()
    {
        CreateRay();
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentTurret = Instantiate(turrets[2], Vector3.zero, Quaternion.identity);
            currentTurret.GetComponent<BoxCollider>().enabled = false;
            currentTurret.transform.localScale = new Vector3(grid.SizeNode * 2, currentTurret.transform.localScale.y, grid.SizeNode * 2);
        }

        if (Input.GetKeyDown(KeyCode.R))
            if (currentTurret != null)
                Destroy(currentTurret);


        if (Input.GetMouseButtonDown(0))
        {
            if (currentTurret != null)
            {
                if (grid.World == null)
                    return;

                if (!canBuild)
                    return;

                CalculatePath();
            }
        }
    }

    private async void CalculatePath()
    {
        path = null;
        var turret = SetTurret();
        grid.World.InitGrid();

        if (!grid.World.CanBuild())
        {
            Destroy(turret);
            await Task.Delay(100);
            grid.World.InitGrid();
            path = grid.World.GetPath();
            grid.WatchPath(path);
            return;
        }
        path = grid.World.GetPath();
        grid.WatchPath(path);
    }

    private GameObject SetTurret()
    {
        GameObject turret = Instantiate(currentTurret);
        turret.GetComponent<BoxCollider>().enabled = true;
        MeshRenderer mr = turret.GetComponent<MeshRenderer>();
        Color tc = mr.material.color;
        tc.a = 1;
        if (mr != null)
            mr.material.color = tc;
        return turret;
    }


    // private void OnDrawGizmos()
    // {
    //     for (int i = 0; i < 40; i++)
    //     {
    //         for (int e = 0; e < 40; e++)
    //         {
    //             if (grid.World == null)
    //                 continue;

    //             if (path == null)
    //                 return;

    //             Vector3 n = new Vector3(i * 4, 0, e * 4);

    //             if (grid.World.Grid[i, e].Busy == 1)
    //             {
    //                 Gizmos.color = Color.red;
    //                 Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
    //                 continue;
    //             }

    //             if (path.Contains(grid.World.Grid[i, e]))
    //             {
    //                 Gizmos.color = Color.green;
    //                 Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
    //                 continue;
    //             }
    //             Gizmos.color = Color.gray;
    //             Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));




    //             if (i == grid.World.PosI.x && e == grid.World.PosI.y)
    //             {
    //                 Gizmos.color = Color.blue;
    //                 Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
    //                 continue;
    //             }

    //             if (i == grid.World.PosF.x && e == grid.World.PosF.y)
    //             {
    //                 Gizmos.color = Color.magenta;
    //                 Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
    //                 continue;
    //             }
    //         }
    //     }
    // }

}
