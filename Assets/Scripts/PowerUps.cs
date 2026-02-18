using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUps : MonoBehaviour
{
    [Header("PowerUps")]

    [SerializeField] float powerUpsMoveSpeed = 3f;

    [SerializeField] bool extraHealthPowerUp = false;
    [SerializeField] bool extraShieldPowerUp = false;
    [SerializeField] bool slowDownTimePowerUp = false;
    [SerializeField] bool freezeEnemiesPowerUp = false;
    [SerializeField] bool extraDamagePowerUp = false;
    [SerializeField] bool extraBulletsPowerUp = false;

    [Header("Other Objects or Variables")]

    [SerializeField] Player player;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        MoveDown();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Debug.Log("Do powerup stuff");
            DoActivePowerUpAction();
        }
    }
    private void MoveDown()
    {
        transform.Translate(Vector2.down * powerUpsMoveSpeed * Time.deltaTime);
    }
    private void DoActivePowerUpAction()
    {
        if (extraHealthPowerUp == true)
        {
            ExtraHealthPowerUpAction();
        }
        else if (extraShieldPowerUp == true)
        {
            ExtraShieldPowerUpAction();
        }
        else if (slowDownTimePowerUp == true)
        {
            SlowDownTimePowerUpAction();
        }
        else if (freezeEnemiesPowerUp == true)
        {
            FreezeEnemiesPowerUpAction();
        }
        else if (extraDamagePowerUp == true)
        {

        }
        else if (extraBulletsPowerUp == true)
        {

        }
        Destroy(gameObject);
    }
    private void ExtraHealthPowerUpAction()
    {
        player.IncreasePlayerHealth();
    }
    private void ExtraShieldPowerUpAction()
    {
        player.IncreaseShieldHealth();
    }    
    private void SlowDownTimePowerUpAction()
    {
        player.SlowDownTime();
    }
    private void FreezeEnemiesPowerUpAction()
    {
        player.StopEnemiesMovement();
    }
}
