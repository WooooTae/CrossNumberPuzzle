using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    Number,
    Operator,
    Equal,
    Empty
}

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public string value;
    public Text tileText;

    public void SetTile(TileType type,string val)
    {
        tileType = type;
        value = val;
        tileText.text = val;
    }
}
