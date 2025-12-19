using UnityEngine;


namespace Runner {

public enum TileType {
    STRAIGHT,
    LEFT,
    RIGHT,
    SIDEWAYS
}

// Defines the attributes of a tile.

public class Tile : MonoBehaviour
{
    public TileType type;
    public Transform pivot;
}

}