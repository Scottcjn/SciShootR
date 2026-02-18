using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    [SerializeField] WaveConfig waveConfig;
    private List<Transform> wayPoints;
    private int wayPointsIndex = 0;
    [SerializeField] Enemy enemy;
    void Start()
    {
        wayPoints = waveConfig.GetPathWayPoints();
        transform.position = wayPoints[wayPointsIndex].transform.position;
    }

    void Update()
    {
        if (waveConfig.GetMoveSpeed() != 0)
        {
            if (enemy.GetCanShoot() != true)
            {
                enemy.SetCanShoot(true);
            }
            MoveEnemy();
        }
        else
        {
            enemy.SetCanShoot(false);
        }
    }

    public void SetWaveConfig(WaveConfig waveConfig)
    {
        this.waveConfig = waveConfig;
    }
    private void MoveEnemy()
    {
        if (wayPointsIndex <= wayPoints.Count - 1)
        {
            Vector3 targetPos = wayPoints[wayPointsIndex].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, waveConfig.GetMoveSpeed() * Time.deltaTime);
            if (transform.position == targetPos)
            {
                wayPointsIndex += 1;
            }
        }
        else
        {
            FindObjectOfType<GameSession>().SubstractFromTotalLevelEnemies();
            Destroy(gameObject);
        }
    }
}
