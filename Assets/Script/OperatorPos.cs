using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperatorDirection 
{ 
    Horizontal, 
    Vertical 
}

public class OperatorPos : MonoBehaviour
{
    public Vector2Int startPos;
    public OperatorDirection direction;
    public string[] elements;
}
