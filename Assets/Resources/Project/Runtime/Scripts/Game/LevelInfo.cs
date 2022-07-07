using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Resources.Project.Runtime.Scripts.Game
{
    [Serializable]
    public class LevelInfo
    {
        public Tile floorTile;
        public List<Transform> objectsPrefabsList;
        public List<Transform> enemiesPrefabsList;
        public float seed;
    }
    
}
