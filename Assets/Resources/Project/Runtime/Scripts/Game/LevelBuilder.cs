using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private Tilemap gridWalkable;
        [SerializeField] private GameObject worldObjectsWalkable;
        [SerializeField] private GameObject worldObjectsObstacle;
        [SerializeField] private GameObject enemiesParent;

        public void BuildLevel(LevelInfo levelInfo)
        {
            BuildFloor(levelInfo.floorTile);
            BuildObjects(levelInfo.objectsPrefabsList, levelInfo.seed);
            SpawnEnemies(levelInfo.enemiesPrefabsList, levelInfo.seed);
        }

        private void BuildFloor(Tile floorTile)
        {
            gridWalkable.FloodFill(gridWalkable.origin, floorTile);
        }

        private void BuildObjects(List<Transform> objectsPrefabsList, float seed)
        {
        }


        private void SpawnEnemies(List<Transform> enemiesPrefabsList, float seed)
        {
            
        }
    }
}
