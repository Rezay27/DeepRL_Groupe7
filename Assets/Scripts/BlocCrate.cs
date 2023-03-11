using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocCrate : Bloc
{
    //Holds the bloc under the crate
    //bool to know if we moved once, if not, creating empty Space
    public GameObject blocUnderMeGO;
    public Bloc blocUnderMe;
    public bool onTarget=false;
    private bool alreadyMoved = false;
    public Color onTargetColor= Color.magenta;
    public Color notOnTargetColor = new Color(90, 46, 18);
    
    //Check if it's possible to move to new position
    //Take the gameObject list of blocs, the position of the crate on the grid
    //And the direction you want to move it (example:(0,1) to move up)
    //Returns a boolean true is possible, false if not
    public bool isPossible(List<List<Bloc>>gridData, Vector2Int pos, Vector2Int dir)
    {
        Vector2Int newPos = pos + dir;
        Debug.Log(newPos);
        //Checks if out of bounds
        if (newPos.x < gridData.Count - 1 && newPos.y < gridData[0].Count - 1 && newPos.x>0 && newPos.y>0)
        {
            //Debug.Log("is Not OOB");
            //Checks if new position is wall or Crate
            if (!gridData[newPos.x][newPos.y].wall && gridData[newPos.x][newPos.y].ID!=4)
            {
                //Debug.Log("is Possible");
                return true;
            }
        }
        //Debug.Log("is Not Possible");
        return false;
    }

    public List<List<Bloc>> move(List<List<Bloc>> gridData, Vector2Int pos, Vector2Int dir)
    {
        Vector2Int newPos = pos + dir;
        //Checking if already moved*
        //If not, creating the bloc supposed to be under it
        if (!alreadyMoved)
        {
            alreadyMoved = true;
            GameObject go =GameObject.Instantiate(blocUnderMeGO, new Vector3(pos.x, 0, pos.y), Quaternion.Euler(Vector3.right * 90));
            go.transform.parent = gridData[0][0].myGo.transform.parent;
            blocUnderMe.myGo = go;
        }
        //Putting the bloc under me in the grid
        gridData[pos.x][pos.y]=blocUnderMe;
        //Getting the new bloc under me and remembering it
        blocUnderMe=gridData[newPos.x][newPos.y];
        if (blocUnderMe.ID == (int)MapGenerator.Case.TargetCrate)
        {
            onTarget = true;
            ChangeColor();
        }
        else
        {
            onTarget = false;
            ChangeColor();
        }
        //Putting myself at my new position on the grid
        gridData[newPos.x][newPos.y] = this;
        //Moving the visual part
        myGo.transform.position = new Vector3(newPos.x, 0, newPos.y);
        //returning the updated grid
        return gridData;
    }

    public void ChangeColor()
    {
        if (onTarget) myGo.GetComponent<MeshRenderer>().material.color = onTargetColor;
        else myGo.GetComponent<MeshRenderer>().material.color = notOnTargetColor;
    }
}