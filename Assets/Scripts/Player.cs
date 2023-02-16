using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject playerPrefab;
    Vector3 playerPos = new Vector3(0, 1, 0);
    private GameObject playerInstantiate;

    public int[,] blocId;

    public Vector2 pos;
    // Start is called before the first frame update
    void Start()
    {
        playerInstantiate = Instantiate(playerPrefab, new Vector3(0, 1, 0), Quaternion.identity);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Up();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Down();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Right();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Left();
        }
    }
    void Right()
    {
        Debug.LogWarningFormat("Right");
        playerInstantiate.transform.position = playerInstantiate.transform.position + new Vector3(1, 0, 0);
    }
    void Left()
    {
        Debug.LogWarningFormat("Left");
        playerInstantiate.transform.position = playerInstantiate.transform.position + new Vector3(-1, 0, 0);
    }
    void Up()
    {
        Debug.LogWarningFormat("Up");
        playerInstantiate.transform.position = playerInstantiate.transform.position + new Vector3(0, 0, 1);
    }
    void Down()
    {
        Debug.LogWarningFormat("Down");
        playerInstantiate.transform.position = playerInstantiate.transform.position + new Vector3(0, 0, -1);
    }
}
