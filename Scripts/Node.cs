using System;
using UnityEngine;


[Serializable]
public class Node
{
    public int ID;

    public int F;

    public int G;

    public int H;

    public Vector3 Pos;

    public Node Parent;

    public int Busy;

    public Pos PosA;

    public Node(int id,int f, int g, int h, Vector3 pos,Node p, int busy, Pos posA)
    {
        ID = id;
        F = f;
        G = g;
        H = h;
        Parent = p;
        Pos = pos;
        //0 no ocupado 1 = ocupado
        Busy = busy;
        PosA = posA;
    }
}
