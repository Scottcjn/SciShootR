using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy stats")]
    [SerializeField] int enemyScore = 1;

    [Header("Enemies Loot")]

    [SerializeField] int lootProbabilityRangeMax = 2;
    [SerializeField] GameObject[] lootArray;

    [Header("Laser Variables")]
    [SerializeField] bool canShoot = true;
    [SerializeField] GameObject explosionParticlesPrefab;

    [Header("Sfxs")]
    [SerializeField] AudioSource sfxsAudioSource;
    [SerializeField] AudioClip shootSfx;
    [SerializeField] AudioClip destroySfx;
    [SerializeField] float sfxVolume = 0.2f;

    private GameSession gameSession;
    void Start()
    {
        sfxsAudioSource = GetComponentInParent<EnemySpawner>().GetSfxsAudioSource();
        gameSession = FindObjectOfType<GameSession>();
    }
    public void SetCanShoot(bool variableState)
    {
        canShoot = variableState;
    }
    public bool GetCanShoot()
    {
        return canShoot;
    }
    public void Die()
    {
        GameObject explosionParticles = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        Destroy(explosionParticles, 1f);
        sfxsAudioSource.PlayOneShot(destroySfx, sfxVolume);
        gameSession.AddScore(enemyScore);
        CheckIfCanSpawnRandomLoot();
        gameSession.SubstractFromTotalLevelEnemies();
        Destroy(gameObject);
    }
    private void CheckIfCanSpawnRandomLoot()
    {
        int randomNumber = Random.Range(1, 11);
        if (randomNumber <= lootProbabilityRangeMax)
        {
            int randomLootItem = Random.Range(0, lootArray.Length);
            Instantiate(lootArray[randomLootItem], transform.position, Quaternion.identity);
        }
    }
}
