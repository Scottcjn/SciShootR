using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<WaveConfig> waveConfig;
    [SerializeField] bool looping = false;
    [SerializeField] AudioSource sfxsAudioSource;
    [SerializeField] List<WaveConfig> wavesThatPassed;
    private IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (looping == true);
    }
    private IEnumerator SpawnAllWaves()
    {
        SetWavesMoveSpeedToDefault();
        for (int waveIndex = 0; waveIndex < waveConfig.Count; waveIndex++)
        {
            WaveConfig currentWave = waveConfig[waveIndex];
            wavesThatPassed.Add(currentWave);
            yield return StartCoroutine(CreateWave(currentWave));
        }
    }
    private IEnumerator CreateWave(WaveConfig waveConfig)
    {
        for (int enemies = 0; enemies < waveConfig.GetNumberOfEnemys(); enemies++)
        {
            GameObject enemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetPathWayPoints()[0].transform.position, Quaternion.identity);
            enemy.transform.parent = transform;
            enemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawn());
        }
    }
    public AudioSource GetSfxsAudioSource()
    {
        return sfxsAudioSource;
    }
    public void StopEnemiesMovement()
    {
        foreach (WaveConfig wave in wavesThatPassed)
        {
            wave.SetEnemiesMoveSpeedToZero();
        }
    }
    public void ResumeEnemiesMovement()
    {
        foreach(WaveConfig wave in wavesThatPassed)
        {
            wave.SetEnemiesMoveSpeedToDefault();
        }
    }
    public int GetNumberOfEnemies()
    {
        int numberOfEnemies = 0;
        foreach(WaveConfig wave in waveConfig)
        {
            numberOfEnemies += wave.GetNumberOfEnemys();
        }
        return numberOfEnemies;
    }
    private void SetWavesMoveSpeedToDefault()
    {
        foreach (WaveConfig wave in waveConfig)
        {
            wave.SetEnemiesMoveSpeedToDefault();
        }
    }
}
