using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloc
{
    public GameObject myGo;
    public int ID;
    public float Vs = 0;
    public bool wall = false;
    public float reward;

    public void Spawn()
    {
        myGo=GameObject.Instantiate(myGo,Vector3.zero,  Quaternion.Euler(Vector3.right * 90));
        //Debug.Log(myGo.name+" "+ID);
    }
}