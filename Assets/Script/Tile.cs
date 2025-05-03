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

    public Vector2Int gridPosition; // 보드상 위치
    public List<Equation> belongingEquations = new List<Equation>(); // 포함된 수식들

    public void SetTile(TileType type, string val, Vector2Int pos)
    {
        tileType = type;
        value = val;
        gridPosition = pos;
        tileText.text = val;
    }

    public void AddEquation(Equation equation)
    {
        if (!belongingEquations.Contains(equation))
        {
            belongingEquations.Add(equation);
        }
    }

}
