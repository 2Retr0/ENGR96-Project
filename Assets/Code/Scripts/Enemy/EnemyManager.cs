using Code.Scripts.Enemy;
using Code.Scripts.Player;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameObject[] enemies;

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject player;
    private Vector3 currentEulerAngles;
    
    private int level;


    // Start is called before the first frame update
    private void Start()
    {
        if (!player) player = FindObjectOfType<PlayerController>().gameObject;
        player.GetComponent<PlayerController>().onLevelUp.AddListener(LeveledUp);
        AddNewEnemy();

        level = 1;
    }

    private void AddNewEnemy() {
        Instantiate(enemyPrefab, this.transform);
        var children = GetComponentsInChildren<EnemyBehaviorController>();
        foreach (var enemy in children)
        {
            enemy.player = player;
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