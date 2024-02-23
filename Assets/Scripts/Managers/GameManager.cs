using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Range(0,100)]private int _tension;
    [SerializeField] private RoomPlacement _roomGenerator;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _roomGenerator = FindObjectOfType<RoomPlacement>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer(); 
        SpawnRoom();
    }

    private void SpawnRoom()
    {
        _roomGenerator.Spawn();
    }

    private void SpawnPlayer()
    {
        _player = Instantiate(_playerPrefab, Vector2.zero, Quaternion.identity, null);
    }

    // Update is called once per frame
    void Update()
    {
        _mainCamera.transform.position = _player.transform.position;
    }

    private void Checkroom()
    {
        foreach(var room in _roomGenerator.Rooms)
        {
            if(room.GetComponent<RoomData>()._playerInRoom)
                    _mainCamera.transform.position = room.transform.position;

        }
    }
}
