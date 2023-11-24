using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    private GameObject[] enemies;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerSlot;
    Vector3 currentEulerAngles;

    public UnityEvent onEnemyDeath;



    // Start is called before the first frame update
    void Start()
    {
        addNewEnemy();
        addNewEnemy();
        addNewEnemy();
        addNewEnemy();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Code.Scripts.Enemy.EnemyBehaviorController[] enemies = GetComponentsInChildren<Code.Scripts.Enemy.EnemyBehaviorController>();
        foreach (Code.Scripts.Enemy.EnemyBehaviorController enemy in enemies)
        {
            if (enemy.checkIfDead()) {
                onEnemyDeath?.Invoke();
            }
        }

    }

    private void addNewEnemy() {
        Instantiate(enemyPrefab, this.transform);
        Code.Scripts.Enemy.EnemyBehaviorController[] enemies = GetComponentsInChildren<Code.Scripts.Enemy.EnemyBehaviorController>();
        foreach (Code.Scripts.Enemy.EnemyBehaviorController enemy in enemies)
        {
            enemy.player = playerSlot;
        }
    }
}