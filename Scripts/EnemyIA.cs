using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour
{
    private List<Node> _path;

    public List<Node> Path { set { _path = value; } get => _path; }

    int index = 0;

    Rigidbody rg;
    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody>();
    }

    float elapsed = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_path == null)
            return;

        if ((index + 1) > _path.Count - 1)
            return;

        if (index >= _path.Count - 1)
            return;

        Vector3 posB = _path[index + 1].Pos;
        Vector3 posToGo = posB - transform.position;
        float dis = Vector3.Distance(this.transform.position, posB);
        rg.velocity = posToGo.normalized * 15;
        transform.position = new Vector3(transform.position.x, 2.83f, transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateObject(this.transform.position, posB), elapsed / 0.7f);

        if (dis < 2.2f)
        {
            // elapsed = 0;
            index++;
        }
        if (elapsed < 0.7f)
            elapsed = 0;
        elapsed += Time.deltaTime;
    }

    Quaternion RotateObject(Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;
        Quaternion qua = Quaternion.LookRotation(dir);
        qua.eulerAngles = new Vector3(0, qua.eulerAngles.y, 0);
        return qua;
    }
}
