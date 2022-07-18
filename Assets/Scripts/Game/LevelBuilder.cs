using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Game
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
        [SerializeField] private Transform playerSpawn;
        [SerializeField] private TextMeshProUGUI loadingPanelText;
        private List<Transform> _enemySpawns = new List<Transform>(); 
        private List<GameObject> _enemiesList = new List<GameObject>(); 
        private int currentAreaIndex;

        public void StartWorld()
        {
            StartCoroutine(BuildWorld());
        }

        private IEnumerator BuildWorld()
        {
            /* Loading Panel */
            SetLoadingPanelText(currentAreaIndex + "/10");
            OpenLoadingPanel();
            /***/
            
            SpawnPlayer();
            BuildFloor();
            CreateNewAreaObjects();
            
            yield return new WaitForSecondsRealtime(2f);
            
            SetEnemiesEnable(true);
            
            /* Loading Panel */
            CloseLoadingPanel();
            /***/
        }

        public IEnumerator CreateNextArea()
        {
            /* Loading Panel */
            SetLoadingPanelText(currentAreaIndex + 1 + "/10");
            OpenLoadingPanel();
            /***/
            
            SpawnPlayer();
            ClearCurrentArea();
            CreateNewAreaObjects();
            
            yield return new WaitForSecondsRealtime(2f);
            
            SetEnemiesEnable(true);
            
            /* Loading Panel */
            CloseLoadingPanel();
            /***/
        }

        private void CreateNewAreaObjects()
        {
            currentAreaIndex++;
            if (currentAreaIndex < 10)
            {
                SpawnObjects();
                SpawnEnemiesDisabled();
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
            var selectedObjectsIndexes = GetRandomObjectsPrefabsIndexes();
            for (var i = 0; i < mapSections.Count; i++)
            {
                var selectedPrefab = mapSectionObjectsPrefabs[selectedObjectsIndexes[i]];
                var spawnedObjects = Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity, mapSections[i]);
                spawnedObjects.transform.localPosition = Vector3.zero;
                var enemySpawnsParent = spawnedObjects.transform.Find("Enemy Spawns");
                foreach (Transform enemySpawn in enemySpawnsParent)
                    _enemySpawns.Add(enemySpawn);
            }
        }

        private List<int> GetRandomObjectsPrefabsIndexes()
        {
            var randomIndexes = new List<int>();
            for (var i = 0; i < mapSections.Count; i++)
            {
                var randomIndex = Random.Range(0, mapSectionObjectsPrefabs.Count);
                if (!randomIndexes.Contains(randomIndex))
                    randomIndexes.Add(randomIndex);
                else
                    i--;
            }
            return randomIndexes;
        }

        private void SpawnEnemiesDisabled()
        {
            levelManager.enemiesRemaining = 0;
            foreach (var spawn in _enemySpawns)
            {
                var enemySpawned = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Count)], Vector3.zero,
                    Quaternion.identity, spawn);
                _enemiesList.Add(enemySpawned.gameObject);
                var enemySpawnedTransform = enemySpawned.transform; 
                enemySpawnedTransform.localPosition = Vector3.zero;
                enemySpawnedTransform.parent = enemiesParent;
                levelManager.enemiesRemaining++;
                enemySpawned.gameObject.SetActive(false);
            }
        }

        private void ClearCurrentArea()
        {
            foreach (var section in mapSections)
            {
                Destroy(section.GetChild(0).gameObject);
            }
            _enemiesList = new List<GameObject>();
            _enemySpawns = new List<Transform>();
        }
        

        private void SpawnPlayer()
        {
            levelManager.playerTransform.position = playerSpawn.position;
        }

        private void OpenLoadingPanel()
        {
            loadingPanelText.transform.parent.gameObject.SetActive(true);
        }

        private void SetLoadingPanelText(string text)
        {
            loadingPanelText.text = text;
        }

        private void CloseLoadingPanel()
        {
            loadingPanelText.transform.parent.gameObject.SetActive(false);
        }

        private void SetEnemiesEnable(bool boolean)
        {
            foreach (var enemy in _enemiesList.Where(enemy => enemy != null))
            {
                enemy.SetActive(boolean);
            }
        }

    }
}
