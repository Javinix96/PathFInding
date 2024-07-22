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
    private List<Node> openList;
    private List<Node> closedList;
    private List<Node> path;

    private GameObject curretnTurret;


    private bool canBuild;

    private void Start()
    {

    }

    void CreateRay()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 3000))
            if (curretnTurret != null)
                curretnTurret.transform.position = GetCords(hit.point);

    }

    private Vector3 GetCords(Vector3 hit)
    {
        int x = (int)(hit.x / grid.World.SizeNode);
        int y = (int)(hit.z / grid.World.SizeNode);

        canBuild = grid.World.CanBuildTurret(new Vector2Int(x, y), 10, 10);

        if (grid.World.Grid == null)
        {
            canBuild = false;
            return Vector3.zero;
        }

        return new Vector3((grid.World.Grid[x, y].Pos.x - 2) + 6, curretnTurret.transform.localScale.y / 2, (grid.World.Grid[x, y].Pos.z - 2) + 6);
    }

    private void Update()
    {
        CreateRay();
        if (Input.GetKeyDown(KeyCode.E))
        {
            curretnTurret = Instantiate(turrets[2], Vector3.zero, Quaternion.identity);
            curretnTurret.GetComponent<BoxCollider>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
            if (curretnTurret != null)
                Destroy(curretnTurret);


        if (Input.GetMouseButtonDown(0))
        {
            if (curretnTurret != null)
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
        GameObject turret = Instantiate(curretnTurret);
        turret.GetComponent<BoxCollider>().enabled = true;
        grid.World.InitGrid();
        if (!grid.World.CanBuild())
        {
            path = grid.World.GetPath(GetPath);
            Destroy(turret);
            await Task.Delay(100);
            grid.World.InitGrid();
            path = grid.World.GetPath(GetPath);
            return;
        }

        path = grid.World.GetPath(GetPath);
    }

    private void GetPath(List<Node> _open, List<Node> _closed)
    {
        openList = _open;
        closedList = _closed;
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < 40; i++)
        {
            for (int e = 0; e < 40; e++)
            {
                if (grid.World == null)
                    continue;

                if (path == null)
                    return;

                Vector3 n = new Vector3(i * 4, 0, e * 4);

                if (grid.World.Grid[i, e].Busy == 1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
                    continue;
                }

                if (path.Contains(grid.World.Grid[i, e]))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
                    continue;
                }
                Gizmos.color = Color.gray;
                Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));




                if (i == grid.World.PosI.x && e == grid.World.PosI.y)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
                    continue;
                }

                if (i == grid.World.PosF.x && e == grid.World.PosF.y)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawCube(n, new Vector3(grid.World.SizeNode, grid.World.SizeNode, grid.World.SizeNode));
                    continue;
                }
            }
        }
    }

}
