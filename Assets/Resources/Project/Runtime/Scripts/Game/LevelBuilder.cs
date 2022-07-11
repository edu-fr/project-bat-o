using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Resources.Project.Runtime.Scripts.Game
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private Tilemap gridWalkable;
        [SerializeField] private List<Transform> mapSections;
        [SerializeField] private Tile floorTile;
        [SerializeField] private List<Transform> mapSectionObjectsPrefabs;
        [SerializeField] private List<Transform> enemiesPrefabs;
        private List<Transform> _enemySpawns; 
        private int currentAreaIndex;

        public void BuildWorld()
        {
            BuildFloor();
            CreateNewAreaObjects();
        }

        public void CreateNextArea()
        {
            DestroyCurrentAreaObjects();
            CreateNewAreaObjects();
        }
        public void CreateNewAreaObjects()
        {
            currentAreaIndex++;
            if (currentAreaIndex < 10)
            {
                SpawnObjects();
                SpawnEnemies();
            }
            else
            {
                print("VENCEU!");
                // TODO win menu
                SceneManager.LoadSceneAsync("Menu");
            }
        
        }
        
        private void BuildFloor()
        {
            gridWalkable.FloodFill(gridWalkable.origin, floorTile);
        }

        private void SpawnObjects()
        {
            foreach (var section in mapSections)
            {
                var objects = Instantiate(mapSectionObjectsPrefabs[Random.Range(0, mapSectionObjectsPrefabs.Count)], Vector3.zero,
                    Quaternion.identity, section);
                foreach (var enemySpawn in objects.transform.Find("Enemy Spawns").GetComponentsInChildren<Transform>())
                {
                    _enemySpawns.Add(enemySpawn);
                }
            }
        }

        private void SpawnEnemies()
        {
            levelManager.enemiesRemaining = 0;
            foreach (var spawn in _enemySpawns)
            {
                Instantiate(enemiesPrefabs[Random.Range(0, mapSectionObjectsPrefabs.Count)], Vector3.zero,
                    Quaternion.identity, spawn);
                levelManager.enemiesRemaining++;
            }
        }
        
        public void DestroyCurrentAreaObjects()
        {
            foreach (var section in mapSections)
            {
                Destroy(section.GetChild(0));
            }
        }

    }
}
