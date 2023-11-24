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
    private int level;


    // Start is called before the first frame update
    void Start()
    {
        AddNewEnemy();
        AddNewEnemy();
        AddNewEnemy();
        AddNewEnemy();
        level = 1;
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

    private void AddNewEnemy() {
        Instantiate(enemyPrefab, this.transform);
        Code.Scripts.Enemy.EnemyBehaviorController[] enemies = GetComponentsInChildren<Code.Scripts.Enemy.EnemyBehaviorController>();
        foreach (Code.Scripts.Enemy.EnemyBehaviorController enemy in enemies)
        {
            enemy.player = playerSlot;
        }
    }

    public void LeveledUp() {
        level++;
        for (int i = 0; i < level; i++) {
            AddNewEnemy();
        }
    }
}