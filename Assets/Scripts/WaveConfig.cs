using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (menuName = "Enemy Wave Config")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject pathPrefab;
    [SerializeField] float timeBetweenSpawn = 0.5f;
    [SerializeField] float spawnRandomFactor = 0.3f;
    [SerializeField] int numberOfEnemys = 10;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float startingMoveSpeed = 5f;

    public GameObject GetEnemyPrefab()
    {
        return enemyPrefab;
    }
    public List<Transform> GetPathWayPoints()
    {
        List<Transform> wavePoints = new List<Transform>();
        foreach(Transform child in pathPrefab.transform)
        {
            wavePoints.Add(child);
        }
        return wavePoints;
    }
    public float GetTimeBetweenSpawn()
    {
        return timeBetweenSpawn;
    }
    public float GetSpawnRandomFactor()
    {
        return spawnRandomFactor;
    }
    public int GetNumberOfEnemys()
    {
        return numberOfEnemys;
    }
    public float GetMoveSpeed()
    {
        return moveSpeed;
    }
    public void SetEnemiesMoveSpeedToZero()
    {
        moveSpeed = 0f;
    }
    public void SetEnemiesMoveSpeedToDefault()
    {
        moveSpeed = startingMoveSpeed;
    }
}
