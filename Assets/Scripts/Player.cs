using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [Header("Player Variables")]

    [SerializeField] float shipSpeed = 10f;
    [SerializeField] float shipOffset = 0.5f;
    [SerializeField] float shipShieldHealth = 2;
    [SerializeField] float startingShieldHealth;
    [SerializeField] float shipLives = 3;
    [SerializeField] bool canShoot = true;

    [Header("SFxs")]

    [SerializeField] AudioSource sfxsAudioSource;
    [SerializeField] AudioClip shootSfx;
    [SerializeField] AudioClip deathSfx;
    [SerializeField] float sfxVolume = 0.1f;

    [Header("Particle Objects")]

    [SerializeField] GameObject hitParticles;
    [SerializeField] GameObject destroyParticles;

    [Header("Other Objects and Variables")]

    [SerializeField] GameMngr gameMngr;
    [SerializeField] GameSession gameSession;
    [SerializeField] Image shieldBarFillImage;
    [SerializeField] EnemySpawner enemySpawner;

    private float maxX;
    private float minX;
    private float minY;
    private float maxY;
    private Coroutine slowDownTimeCoroutine;
    private Coroutine stopEnemiesCorutine;

    private void Awake()
    {
        gameMngr = FindObjectOfType<GameMngr>();
    }
    private void Start()
    {
        gameSession.SetPlayerHealthText(shipLives);

        startingShieldHealth = shipShieldHealth;
        if (gameMngr != null)
        {
            GetComponent<SpriteRenderer>().sprite = gameMngr.GetCurrentShipSprite();
        }
        PosLimitVariables();
    }
    private void Update()
    {
        ShipMovement();
        if (Input.GetKeyDown(KeyCode.H))
        {
            StopEnemiesMovement();
        }    
    }
    private void ShipMovement()
    {
        float xPos = Input.GetAxis("Horizontal") * shipSpeed * Time.deltaTime;
        float yPos = Input.GetAxis("Vertical") * shipSpeed * Time.deltaTime;

        float newXPos = Mathf.Clamp(transform.position.x + xPos, minX, maxX);
        float newYPos = Mathf.Clamp(transform.position.y + yPos, minY, maxY);

        transform.position = new Vector2(newXPos, newYPos);
    }
    public void SetCanShootState(bool variableState)
    {
        canShoot = variableState;
    }
    public bool GetCanShoot()
    {
        return canShoot;
    }
    public void TakeDamage(float damage)
    {
        HealthSystem(damage);
        ShieldSystem(damage);

        InstantiateHitParticles();

        if (shipLives <= 0)
        {
            GameOver();
        }
    }
    private void ShieldSystem(float damage)
    {
        if (shipShieldHealth > 0)
        {
            shipShieldHealth -= damage;
            shieldBarFillImage.fillAmount = shipShieldHealth / startingShieldHealth;
        }
    }
    private void HealthSystem(float damage)
    {
        if (shipShieldHealth <= 0)
        {
            shipLives -= damage;
            gameSession.SetPlayerHealthText(shipLives);
        }
    }
    private void InstantiateHitParticles()
    {
        if (shipLives > 0)
        {
            GameObject particlesClone = Instantiate(hitParticles, transform.position, Quaternion.identity);
            Destroy(particlesClone, 2f);
        }
    }    
    public void GameOver()
    {
        sfxsAudioSource.PlayOneShot(deathSfx, sfxVolume);
        GameObject particlesClone = Instantiate(destroyParticles, transform.position, Quaternion.identity);
        Destroy(particlesClone, 2f);
        gameSession.ActivateGameOverCanvas();
        Destroy(gameObject);
    }
    public float GetHealth()
    {
        return shipLives;
    }
    private void PosLimitVariables()
    {
        Camera gameCamera = Camera.main;
        minX = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + shipOffset;
        maxX = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - shipOffset;

        minY = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + shipOffset;
        maxY = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - shipOffset;
    }
    #region PowerUps
    public void IncreasePlayerHealth()
    {
        shipLives += 1;
        gameSession.SetPlayerHealthText(shipLives);
    }
    public void IncreaseShieldHealth()
    {
        if (shipShieldHealth <= 1)
        {
            shipShieldHealth += 1;
            shieldBarFillImage.fillAmount = shipShieldHealth / startingShieldHealth;
        }
    }
    public void SlowDownTime()
    {
        if (slowDownTimeCoroutine != null)
        {
            StopCoroutine(slowDownTimeCoroutine);
        }
        slowDownTimeCoroutine = StartCoroutine(SlowDownTimeCoroutine());
    }
    private IEnumerator SlowDownTimeCoroutine()
    {
        float powerUpDuration = 10f;
        Time.timeScale = 0.6f;
        yield return new WaitForSecondsRealtime(powerUpDuration);
        Time.timeScale = 1f;
    }
    public void StopEnemiesMovement()
    {
        if (stopEnemiesCorutine != null)
        {
            StopCoroutine(stopEnemiesCorutine);
        }
        stopEnemiesCorutine = StartCoroutine(StopEnemiesMovementCoroutine());
    }
    private IEnumerator StopEnemiesMovementCoroutine()
    {
        float powerUpDuration = 5f;
        enemySpawner.StopEnemiesMovement();
        yield return new WaitForSecondsRealtime(powerUpDuration);
        enemySpawner.ResumeEnemiesMovement();
    }
    #endregion
}
