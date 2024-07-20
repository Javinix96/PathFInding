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
    private Vector2Int posI;
    [SerializeField]
    private Vector2Int posF;
    [SerializeField]
    private LayerMask noHit;

    private List<Node> openList;
    private List<Node> closedList;
    private List<Node> path;

    private GameObject curretnTurret;

    private World world;


    public World World { get => world; }

    private void Start()
    {
        world = new World();
        world.InitGrid(posI, posF, 40, 4, noHit);
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
        int x = (int)(Math.Round(hit.x / 2) * 2) / 4;
        int y = (int)(Math.Round(hit.z / 2) * 2) / 4;

        var g = world.GridForPos(40, 4);

        Vector3 pos = new Vector3((g[x, y].Pos.x - 2) + 10, curretnTurret.transform.localScale.y / 2, (g[x, y].Pos.z - 2) + 10);

        return pos;
    }

    private async void Update()
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
                if (world == null)
                    return;
                path = null;
                GameObject turret = Instantiate(curretnTurret);
                turret.GetComponent<BoxCollider>().enabled = true;
                world.InitGrid(posI, posF, 40, 4, noHit);
                if (!world.CanBuild())
                {
                    Destroy(turret);
                    await Task.Delay(100);
                    world.InitGrid(posI, posF, 40, 4, noHit);
                    path = world.GetPath(GetPath);

                    return;
                }

                path = world.GetPath(GetPath);
            }
        }

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
                if (world == null)
                    continue;

                if (path == null)
                    return;

                Vector3 n = new Vector3(i * 4, 0, e * 4);

                if (world.Grid[i, e].Busy == 1)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(n, new Vector3(world.SizeNode, world.SizeNode, world.SizeNode));
                    continue;
                }

                if (path.Contains(world.Grid[i, e]))
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(n, new Vector3(world.SizeNode, world.SizeNode, world.SizeNode));
                    continue;
                }
                Gizmos.color = Color.gray;
                Gizmos.DrawCube(n, new Vector3(world.SizeNode, world.SizeNode, world.SizeNode));




                if (i == posI.x && e == posI.y)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(n, new Vector3(world.SizeNode, world.SizeNode, world.SizeNode));
                    continue;
                }

                if (i == posF.x && e == posF.y)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawCube(n, new Vector3(world.SizeNode, world.SizeNode, world.SizeNode));
                    continue;
                }
            }
        }
    }

}
