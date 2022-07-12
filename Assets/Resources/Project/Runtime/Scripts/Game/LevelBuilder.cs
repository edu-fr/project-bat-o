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
        [SerializeField] private Transform enemiesParent;
        [SerializeField] private List<Transform> mapSections;
        [SerializeField] private Tile floorTile;
        [SerializeField] private List<Transform> mapSectionObjectsPrefabs;
        [SerializeField] private List<Transform> enemiesPrefabs;
        private List<Transform> _enemySpawns = new List<Transform>(); 
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
                var randomIndex = Random.Range(0, mapSectionObjectsPrefabs.Count);
                var randomPrefab = mapSectionObjectsPrefabs[randomIndex];
                var objectsParent = Instantiate(randomPrefab, Vector3.zero, Quaternion.identity, section);
                objectsParent.transform.localPosition = Vector3.zero;
                var enemySpawnsParent = objectsParent.transform.Find("Enemy Spawns");
                foreach (Transform enemySpawn in enemySpawnsParent)
                {
                    _enemySpawns.Add(enemySpawn);
                }
                // Avoid duplicates
                mapSectionObjectsPrefabs.RemoveAt(randomIndex);
            }
        }

        private void SpawnEnemies()
        {
            levelManager.enemiesRemaining = 0;
            foreach (var spawn in _enemySpawns)
            {
                var enemySpawned = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], Vector3.zero,
                    Quaternion.identity, spawn);
                var enemySpawnedTransform = enemySpawned.transform; 
                enemySpawnedTransform.localPosition = Vector3.zero;
                enemySpawnedTransform.parent = enemiesParent;
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
