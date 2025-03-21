using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonMapRenderer : MonoBehaviour
{
    [Header("Generation Settings")]
    //Map Height / Width
    [SerializeField] Vector2Int mapDimensions = new Vector2Int(256, 256);
    //Colors List
    //0 - Room, 1 - Cleared, 2 - Boss, 3 - Current, 4 - Line, 5 - Outline, 6 - Background
    [SerializeField] Color[] mapColors = new Color[7];
    [SerializeField] Vector2Int spacing;
    [SerializeField] Vector2Int offset;

    // Texture
    Texture2D mapTexture;

    // Generation
    Dictionary<int, List<DMR_RoomInfo>> roomsByDepth; // Key is int depth, returns list of rooms at that depth
    Dictionary<Room, DMR_RoomInfo> roomLookup;
    int maxDepth;

    // Components
    SpriteRenderer sr;

    // Icon patterns
    string[] circle = new string[]
    {
        "xxxxxxx",
        "xxoooxx",
        "xooooox",
        "xooooox",
        "xooooox",
        "xxoooxx",
        "xxxxxxx"
    };
    string[] square = new string[]
{
        "xxxxxxx",
        "xooooox",
        "xoxxxox",
        "xoxxxox",
        "xoxxxox",
        "xooooox",
        "xxxxxxx"
};
    string[] skull = new string[]
    {
        "xxoooxx",
        "xooxoox",
        "oxxoxxo",
        "oxxoxxo",
        "ooooooo",
        "ooooooo",
        "xooooox"
    };
    string[] xmark = new string[]
    {
        "xxxxxxx",
        "xooxoox",
        "xooooox",
        "xxoooxx",
        "xooooox",
        "xooxoox",
        "xxxxxxx"
    };

    class DMR_RoomInfo
    {
        public DMR_RoomInfo(int depth, bool bossRoom, bool cleared)
        {
            this.depth = depth;
            children = new List<DMR_RoomInfo>();
            this.bossRoom = bossRoom;
            this.cleared = cleared;
            currentRoom = false;
        }

        public int depth;
        public Vector2Int position;
        public List<DMR_RoomInfo> children;
        public DMR_RoomInfo parentRoom;
        public bool cleared;
        public bool currentRoom;
        public bool bossRoom;
    }

    void Awake()
    {
        // Initialize variables
        roomsByDepth = new Dictionary<int, List<DMR_RoomInfo>>();

        // Initialize texure
        mapTexture = new Texture2D(mapDimensions.x, mapDimensions.y);
        mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;

        // Set components
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(mapTexture, new Rect(0.0f, 0.0f, mapDimensions.x, mapDimensions.y), new Vector2(0.5f, 0.5f), 16);
    }

    public void DisplayMap(List<Room> mapRooms, Room currentRoom)
    {
        ClearMap();

        SortRooms(mapRooms);
        roomLookup[currentRoom].currentRoom = true;
        SetRoomPositions();

        // Draw connections
        foreach(int depth in roomsByDepth.Keys)
        {
            foreach(DMR_RoomInfo room in roomsByDepth[depth])
            {
                // Draw connections to children
                foreach (DMR_RoomInfo child in room.children)
                {
                    DrawLine(room.position, child.position, mapColors[4]);
                }

                // Draw room
                DrawIcon(room.position,
                         room.currentRoom ? square : room.bossRoom ? skull : room.cleared ? xmark : circle,
                         mapColors[6],
                         room.currentRoom ? mapColors[3] : room.bossRoom ? mapColors[2] : room.cleared ? mapColors[1] : mapColors[0]);
            }
        }
        DrawOutline(mapColors[5]);

        mapTexture.Apply();
    }
    
    /// <summary>
    /// 
    /// </summary>
    void SetRoomPositions()
    {
        foreach(int depth in roomsByDepth.Keys)
        {
            int count = 0;
            int totalRoomsAtDepth = roomsByDepth[depth].Count;

            foreach (DMR_RoomInfo room in roomsByDepth[depth])
            {
                // Set vertical position based on depth
                room.position.y = offset.y + depth * spacing.y;

                // Distribute rooms horizontally within the depth level
                room.position.x = offset.x + count * spacing.x - ((totalRoomsAtDepth - 1) * spacing.x / 2);
                // if boss room, place in line with parent room
                if (room.bossRoom)
                    roomsByDepth[depth][0].position.x = room.parentRoom.position.x;

                count++;
            }
        }
    }

    /// <summary>
    /// Sorts list of given rooms into dictionary by depth
    /// Assigns all children and parent rooms
    /// Sets max depth of generated dungeon
    /// </summary>
    void SortRooms(List<Room> mapRooms)
    {
        roomsByDepth = new Dictionary<int, List<DMR_RoomInfo>>();
        roomLookup = new Dictionary<Room, DMR_RoomInfo>();

        // Create and sort all rooms into roomsByDepth
        foreach (Room room in mapRooms)
        {
            DMR_RoomInfo roomInfo = new DMR_RoomInfo(room.depth, room.type == Room.RoomType.boss ? true : false, room.Clear);

            // Add to room map dictionary for lookup later
            roomLookup[room] = roomInfo;

            // Add new depth if necessary
            if (!roomsByDepth.ContainsKey(room.depth))
            {
                roomsByDepth[room.depth] = new List<DMR_RoomInfo>();
            }

            // Add to roomsByDepth
            roomsByDepth[room.depth].Add(roomInfo);
        }

        // Assign parents and children
        foreach (Room room in mapRooms)
        {
            DMR_RoomInfo roomInfo = roomLookup[room];

            foreach (Room child in room.children)
            {
                if (roomLookup.TryGetValue(child, out DMR_RoomInfo childInfo))
                {
                    // Assign child
                    roomInfo.children.Add(childInfo);

                    // Assign parent
                    childInfo.parentRoom = roomInfo;
                }
            }
        }

        // Find max depth
        maxDepth = roomsByDepth.Keys.Max();
    }

    /// <summary>
    /// Sets entire canvas to bgColor
    /// </summary>
    void ClearMap()
    {
        for (int y = 0; y < mapDimensions.y; y++)
        {
            for (int x = 0; x < mapDimensions.x; x++)
            {
                mapTexture.SetPixel(x, y, mapColors[6]);
            }
        }
    }

    /// <summary>
    /// Draws a line between two given points
    /// </summary>
    void DrawLine(Vector2 start, Vector2 end, Color color)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            // Interpolate the position
            Vector2 point = Vector2.Lerp(start, end, i / (float)steps);

            // Round to nearest integer pixel
            int x = Mathf.RoundToInt(point.x);
            int y = Mathf.RoundToInt(point.y);

            // Set the pixel if within bounds
            if (x >= 0 && x < mapDimensions.x && y >= 0 && y < mapDimensions.y)
            {
                mapTexture.SetPixel(x, y, color);
            }
        }
    }

    /// <summary>
    /// Draws a 7x7 icon on the map texture, centered at a given position
    /// </summary>
    void DrawIcon(Vector2Int center, string[] pattern, Color xColor, Color oColor)
    {
        // Check if pattern is 7x7
        if (pattern.Length != 7 || pattern.Any(row => row.Length != 7))
        {
            return;
        }

        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                int px = center.x + x - 3;
                int py = center.y + y - 3;

                // Bounds check
                if (px >= 0 && px < mapDimensions.x && py >= 0 && py < mapDimensions.y)
                {
                    if (pattern[y][x] == 'x')
                    {
                        mapTexture.SetPixel(px, py, xColor);
                    }
                    else
                    {
                        mapTexture.SetPixel(px, py, oColor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws an outline around all pixels that are not bgColor
    /// </summary>
    void DrawOutline(Color outlineColor)
    {
        Color[] originalPixels = mapTexture.GetPixels();
        Color[] newPixels = mapTexture.GetPixels();

        for (int y = 1; y < mapDimensions.y - 1; y++)
        {
            for (int x = 1; x < mapDimensions.x - 1; x++)
            {
                int index = y * mapDimensions.x + x;

                if (originalPixels[index] != mapColors[6])
                    continue;

                // Check neighbors for non background pixels
                bool hasNeighbor = false;
                for (int ny = -1; ny <= 1; ny++)
                {
                    for (int nx = -1; nx <= 1; nx++)
                    {
                        if (nx == 0 && ny == 0) continue; // Skip the current pixel

                        int neighborIndex = (y + ny) * mapDimensions.x + (x + nx);
                        if (originalPixels[neighborIndex] != mapColors[6])
                        {
                            hasNeighbor = true;
                            break;
                        }
                    }

                    if (hasNeighbor) break;
                }

                if (hasNeighbor)
                {
                    newPixels[index] = outlineColor;
                }
            }
        }

        mapTexture.SetPixels(newPixels);
    }
}
