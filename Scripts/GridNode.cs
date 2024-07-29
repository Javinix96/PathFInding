using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    private int _id;
    private MeshRenderer mh;
    private int busy = 0;

    public int Id { set { _id = value; } get => _id; }
    public int Busy { set { busy = value; } get => busy; }

    public void Init()
    {
        mh = GetComponent<MeshRenderer>();
        this.gameObject.SetActive(false);
    }

    public void SetColor(Color color) => mh.material.color = color;
    public void SetColor(Color color, float opacity)
    {
        color.a = opacity;
        mh.material.color = color;
        this.gameObject.SetActive(false);
    }

    public void SetColor(Color color, bool active)
    {
        mh.material.color = color;
        this.gameObject.SetActive(active);
    }

}
