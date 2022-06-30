using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (EnemyWaveSpawner))]
public class LevelDifficultyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float difficultyLevel = 10, minAmountPercent = 0.8f, maxAmountPercent = 1f, difficultyIncrease = 0.1f;

    // The system works per wave, so for each enemy wave, the system needs to know the last upper index (top index) of the previous wave.
    [HideInInspector] public int lastTopIndex = 0;
    [HideInInspector] public List<EnemyData> enemyDataList = new List<EnemyData>();

    private List<int> difficultyList;
    private EnemyWaveSpawner spawner;
    private float screenEdgeOffset = 0.25f;
    private Vector2 previousPos;

    void Awake()
    {
        difficultyList = new List<int>();

        spawner = GetComponent<EnemyWaveSpawner>();
        foreach(GameObject enemyPrefab in enemyPrefabs)
        {
            difficultyList.Add(enemyPrefab.GetComponent<EnemyDifficulty>().enemyDifficulty);
        }
        GenerateEnemies();
    }
    private void FixedUpdate()
    {
        if(GlobalReferenceManager.MainCamera.transform.position.y > previousPos.y)
        {
            GenerateEnemies();  
        }
        difficultyLevel += difficultyIncrease * Time.deltaTime;
    }
    private void GenerateEnemies()
    {
        //Creating random amount of datasets for formations
        int spendableDifficultyLevel = (int)Random.Range(difficultyLevel * minAmountPercent, difficultyLevel * maxAmountPercent);
        while (spendableDifficultyLevel > 0) {
            EnemyData newEnemyData;
            //This is part of the inspector view list.
            enemyDataList.Add(newEnemyData = new EnemyData());

            //Pick random number for enemy prefab
            //subtract difficulty value of random enemy from spendableDifficultyLevel,
            //If it cant subtract that amount pick a new random that IS within cost range
            //If it doesnt have any points left just spawn the lowest cost enemies
            int randomEnemy = Random.Range(0, enemyPrefabs.Length);
            if (spendableDifficultyLevel >= difficultyList[randomEnemy]) {
                spendableDifficultyLevel -= difficultyList[randomEnemy];
                newEnemyData.EnemyPrefab = enemyPrefabs[randomEnemy];
            } else
            {
                int newMaxRandomEnemy = Random.Range(0, spendableDifficultyLevel + 1);
                if (spendableDifficultyLevel >= difficultyList[newMaxRandomEnemy])
                {
                    spendableDifficultyLevel -= difficultyList[newMaxRandomEnemy];
                    newEnemyData.EnemyPrefab = enemyPrefabs[newMaxRandomEnemy];
                }
                else
                {
                    spendableDifficultyLevel -= difficultyList[0];
                    newEnemyData.EnemyPrefab = enemyPrefabs[0];
                }
            }

            //Randomize this enemies position within the screen bounds
            newEnemyData.EnemyPosition = randomisePosition();

            //Activate spawner with generated data
            spawner.SpawnEnemy(newEnemyData);
        }
        spawner.OverlapCheck();
        lastTopIndex = enemyDataList.Count;
        previousPos = enemyDataList[enemyDataList.Count - 1].EnemyPosition;
    }
    public Vector2 randomisePosition()
    {
        //Set spawn position above visible playing area with random offset
        float positionOffset = Random.Range(1, GlobalReferenceManager.MainCamera.orthographicSize * 2);
        float withinScreenRange = (GlobalReferenceManager.ScreenCollider.sizeX / 2) - screenEdgeOffset;
        Vector2 newPos = new Vector2(Random.Range(-withinScreenRange, withinScreenRange), GlobalReferenceManager.MainCamera.orthographicSize + GlobalReferenceManager.MainCamera.transform.position.y + positionOffset);
        return newPos;
    }
}
