using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private RoomConnections _roomConnection;
    public RoomConnections RoomConnection => _roomConnection;
    public bool PlayerAtDoor;
    [field: SerializeField] public BoxCollider2D TriggerBox { get; private set; }
    [field: SerializeField] public BoxCollider2D CollisionBox { get; private set; }

    public void SetConnection(RoomConnections connection)
    {
        _roomConnection = connection;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
            PlayerAtDoor = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag =="Player")
            PlayerAtDoor = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag =="Player")
            PlayerAtDoor = false;
    }
}
