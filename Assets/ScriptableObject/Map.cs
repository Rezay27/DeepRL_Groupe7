using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "ScriptableObjects/Map", order = 1)]
public class Map : ScriptableObject
{
    public Vector2Int mapSize;
    
    public List<MapLigne> blocId = new List<MapLigne>();
}
