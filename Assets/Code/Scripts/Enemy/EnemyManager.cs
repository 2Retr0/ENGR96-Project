using Code.Scripts.Enemy;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameObject[] enemies;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject playerSlot;
    private Vector3 currentEulerAngles;
    
    private int level;


    // Start is called before the first frame update
    private void Start()
    {
        AddNewEnemy();

        level = 1;
    }

    private void AddNewEnemy() {
        Instantiate(enemyPrefab, this.transform);
        var children = GetComponentsInChildren<EnemyBehaviorController>();
        foreach (var enemy in children)
        {
            enemy.player = playerSlot;
        }
    }

    public void LeveledUp() {
        level++;
        for (var i = 0; i < level; i++) {
            AddNewEnemy();
        }

        var children = GetComponentsInChildren<EnemyBehaviorController>();
        foreach (var enemy in children)
        {
            enemy.rotationSpeed = level*3 + 45;
            enemy.speed = level + 4;
        }
    }
}