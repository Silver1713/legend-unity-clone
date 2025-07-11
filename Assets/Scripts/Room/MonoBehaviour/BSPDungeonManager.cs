using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeonManager : Singleton<BSPDungeonManager>
{
    public PlayerStateManager playerPrefab;
    public BSPRoomManager roomManager;
    public GameObject miniMap;
    public GameObject mapRoom;
    
    [Header("BSP Dungeon Settings")]
    [SerializeField] private int dungeonWidth = 40;
    [SerializeField] private int dungeonHeight = 40;
    [SerializeField] private float roomScale = 1f;
    
    public BSPDungeon BSPDungeon { get => bspDungeon; }
    public Transform DungeonHolder { get => _dungeonHolder; }
    public bool Shifting { get => _shifting; }
    
    private Transform _mainCamera;
    private BSPRoom _currentRoom;
    private BSPRoom _nextRoom;
    
    private Transform _playerTransform;
    private PlayerStateManager _playerController;
    
    private bool _shifting;
    private readonly float _cameraSpeed = 15.0f;
    
    private BSPDungeon bspDungeon;
    private Dictionary<int, BSPRoom> _rooms;
    private Transform _dungeonHolder;
    private GameObject[] _minimapRooms;
    
    private int _currentRoomIndex = 0;
    
    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main.transform;
        
        _dungeonHolder = new GameObject("BSP Dungeon").transform;
        _rooms = new Dictionary<int, BSPRoom>();
        _minimapRooms = new GameObject[100];
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        Doorway.ShiftRoomEvent += PlayerCollideDoorwayEventHandler;
        Switch.SwitchPressed += SwitchPressedEventHandler;
        
        GenerateBSPDungeon();
        SpawnPlayerInFirstRoom();
        
        if (miniMap.activeSelf)
        {
            DisplayMiniMap();
        }
    }
    
    private void GenerateBSPDungeon()
    {
        bspDungeon = new BSPDungeon(dungeonWidth, dungeonHeight);
        
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            RectInt roomRect = bspDungeon.GetRoomAt(i);
            BSPRoom room = roomManager.GenerateRoom(roomRect, i, roomScale);
            _rooms.Add(i, room);
            
            if (i > 0)
            {
                room.Holder.gameObject.SetActive(false);
            }
        }
        
        _currentRoom = _rooms[0];
        _currentRoomIndex = 0;
        
        ConnectRooms();
    }
    
    private void ConnectRooms()
    {
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            for (int j = i + 1; j < bspDungeon.GetRoomCount(); j++)
            {
                if (bspDungeon.AreRoomsConnected(i, j))
                {
                    BSPRoom room1 = _rooms[i];
                    BSPRoom room2 = _rooms[j];
                    
                    RectInt rect1 = bspDungeon.GetRoomAt(i);
                    RectInt rect2 = bspDungeon.GetRoomAt(j);
                    
                    Position connectionDir = DetermineConnectionDirection(rect1, rect2);
                    Position oppositeDir = GetOppositeDirection(connectionDir);
                    
                    room1.AddDoorway(connectionDir, j);
                    room2.AddDoorway(oppositeDir, i);
                    
                    roomManager.GenerateDoorway(room1, connectionDir, rect1, roomScale);
                    roomManager.GenerateDoorway(room2, oppositeDir, rect2, roomScale);
                }
            }
        }
    }
    
    private Position DetermineConnectionDirection(RectInt room1, RectInt room2)
    {
        Vector2 center1 = new Vector2(room1.center.x, room1.center.y);
        Vector2 center2 = new Vector2(room2.center.x, room2.center.y);
        
        Vector2 direction = (center2 - center1).normalized;
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? Position.RIGHT : Position.LEFT;
        }
        else
        {
            return direction.y > 0 ? Position.TOP : Position.BOTTOM;
        }
    }
    
    private Position GetOppositeDirection(Position direction)
    {
        switch (direction)
        {
            case Position.TOP: return Position.BOTTOM;
            case Position.BOTTOM: return Position.TOP;
            case Position.LEFT: return Position.RIGHT;
            case Position.RIGHT: return Position.LEFT;
            default: return Position.TOP;
        }
    }
    
    private void SpawnPlayerInFirstRoom()
    {
        if (bspDungeon.GetRoomCount() > 0)
        {
            Vector2Int roomCenter = bspDungeon.GetRoomCenter(0);
            Vector2 spawnPosition = new Vector2(roomCenter.x * roomScale, roomCenter.y * roomScale);
            
            _playerController = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            _playerTransform = _playerController.transform;
            
            GameManager.Instance.SetPlayer(_playerController.gameObject);
            
            _mainCamera.position = new Vector3(spawnPosition.x, spawnPosition.y, _mainCamera.position.z);
        }
    }
    
    private void SwitchPressedEventHandler()
    {
        _currentRoom.OpenAllDoors(true);
    }
    
    private void PlayerCollideDoorwayEventHandler(Doorway doorway)
    {
        if (doorway.TryGetComponent<BSPDoorway>(out BSPDoorway bspDoorway))
        {
            StartCoroutine(ShiftToRoom(bspDoorway.TargetRoomIndex, doorway.position));
        }
    }
    
    private IEnumerator ShiftToRoom(int targetRoomIndex, Position fromDirection)
    {
        _shifting = true;
        
        _nextRoom = _rooms[targetRoomIndex];
        _nextRoom.Holder.gameObject.SetActive(true);
        
        RectInt currentRect = bspDungeon.GetRoomAt(_currentRoomIndex);
        RectInt nextRect = bspDungeon.GetRoomAt(targetRoomIndex);
        
        Vector2 currentCenter = new Vector2(currentRect.center.x * roomScale, currentRect.center.y * roomScale);
        Vector2 nextCenter = new Vector2(nextRect.center.x * roomScale, nextRect.center.y * roomScale);
        Vector2 offset = nextCenter - currentCenter;
        
        Vector2 playerTargetPos = _playerTransform.position;
        Position oppositeDir = GetOppositeDirection(fromDirection);
        
        switch (oppositeDir)
        {
            case Position.TOP:
                playerTargetPos = nextCenter + new Vector2(0, (nextRect.height * roomScale / 2) - 2);
                break;
            case Position.BOTTOM:
                playerTargetPos = nextCenter + new Vector2(0, -(nextRect.height * roomScale / 2) + 2);
                break;
            case Position.LEFT:
                playerTargetPos = nextCenter + new Vector2(-(nextRect.width * roomScale / 2) + 2, 0);
                break;
            case Position.RIGHT:
                playerTargetPos = nextCenter + new Vector2((nextRect.width * roomScale / 2) - 2, 0);
                break;
        }
        
        Vector3 cameraTarget = new Vector3(nextCenter.x, nextCenter.y, _mainCamera.position.z);
        
        float transitionTime = 1f;
        float elapsedTime = 0f;
        
        Vector3 startCameraPos = _mainCamera.position;
        Vector3 startPlayerPos = _playerTransform.position;
        
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionTime;
            
            _mainCamera.position = Vector3.Lerp(startCameraPos, cameraTarget, t);
            _playerTransform.position = Vector3.Lerp(startPlayerPos, playerTargetPos, t);
            
            yield return null;
        }
        
        _mainCamera.position = cameraTarget;
        _playerTransform.position = playerTargetPos;
        
        _currentRoom.Holder.gameObject.SetActive(false);
        _currentRoom = _nextRoom;
        _currentRoomIndex = targetRoomIndex;
        
        if (!_currentRoom.HasBeenVisited)
        {
            _currentRoom.OpenAllDoors(false);
            _currentRoom.HasBeenVisited = true;
        }
        
        _shifting = false;
    }
    
    private void DisplayMiniMap()
    {
        float minimapScale = 0.1f;
        
        for (int i = 0; i < bspDungeon.GetRoomCount(); i++)
        {
            RectInt room = bspDungeon.GetRoomAt(i);
            Vector2 center = new Vector2(room.center.x * minimapScale, room.center.y * minimapScale);
            
            GameObject minimapRoom = Instantiate(mapRoom, center, Quaternion.identity, miniMap.transform);
            minimapRoom.transform.localScale = new Vector3(room.width * minimapScale, room.height * minimapScale, 1);
            minimapRoom.name = "MinimapRoom_" + i;
            
            _minimapRooms[i] = minimapRoom;
        }
    }
}