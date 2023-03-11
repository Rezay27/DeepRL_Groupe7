using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public GameObject playerPrefab;
    private Vector3 _playerPos;
    private GameObject _playerInstantiate;

    public void Init(Vector3 startPosition)
    {
        _playerPos = startPosition;
        _playerInstantiate = Instantiate(playerPrefab, _playerPos, Quaternion.identity);
    }
    public void MovePlayer(Vector3 direction)
    {
        _playerInstantiate.transform.position += direction;
    }

    public Vector3 GetPlayerPosition()
    {
        Vector3 playerPosition = _playerInstantiate.transform.position;
        return playerPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MovePlayer(new Vector3(0, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MovePlayer(new Vector3(0, 0, -1));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer(new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(new Vector3(-1, 0, 0));
        }
    }
    public void Right()
    {
        Debug.LogWarningFormat("Right");
        _playerInstantiate.transform.position = _playerInstantiate.transform.position + new Vector3(1, 0, 0);
    }
    public void Left()
    {
        Debug.LogWarningFormat("Left");
        _playerInstantiate.transform.position = _playerInstantiate.transform.position + new Vector3(-1, 0, 0);
    }
    public void Up()
    {
        Debug.LogWarningFormat("Up");
        _playerInstantiate.transform.position = _playerInstantiate.transform.position + new Vector3(0, 0, 1);
    }
    public void Down()
    {
        Debug.LogWarningFormat("Down");
        _playerInstantiate.transform.position = _playerInstantiate.transform.position + new Vector3(0, 0, -1);
    }
}
