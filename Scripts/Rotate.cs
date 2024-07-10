using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private GameObject watchObject;


    void Start()
    {
        RotateObject();
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject();

    }

    void RotateObject()
    {
        Vector3 dir = watchObject.transform.position - this.transform.position;
        transform.rotation = Quaternion.LookRotation(dir);
        transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}
