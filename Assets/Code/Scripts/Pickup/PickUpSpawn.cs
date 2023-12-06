using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawn : MonoBehaviour
{
    public GameObject[] pickUps;
    private readonly List<Vector3> spawn_pts = new List<Vector3>();

    private float timer = 0f;
    private float interval = 15f; // Time interval in seconds
    
    // Update is called once per frame
    void Update()
    {
        if (GameObject.FindGameObjectsWithTag("SpeedIncrease").Length < 1)
        {
            timer += Time.deltaTime;
        }
        
        if(GameObject.FindGameObjectsWithTag("PickUp").Length < 1)
        {
            //zone a
            Vector3 random_pts_1a = new Vector3(Random.Range(23, 28), 0, Random.Range(3,12));
            spawn_pts.Add(random_pts_1a);

            Vector3 random_pts_2a = new Vector3(Random.Range(29, 35), 0, Random.Range(-7,2));
            spawn_pts.Add(random_pts_2a);

            Vector3 random_pts_3a = new Vector3(38, 0, 10);
            spawn_pts.Add(random_pts_3a);

            //zone b
            Vector3 random_pts_1b = new Vector3(Random.Range(15, 17), 0, Random.Range(3,20));
            spawn_pts.Add(random_pts_1b);

            Vector3 random_pts_2b = new Vector3(Random.Range(-19, 17), 0, Random.Range(20,22));
            spawn_pts.Add(random_pts_2b);

            Vector3 random_pts_3b = new Vector3(Random.Range(-10, -3), 0, Random.Range(-2,20));
            spawn_pts.Add(random_pts_3b);

            Vector3 random_pts_4b = new Vector3(Random.Range(-7, 11), 0, Random.Range(-10,-4));
            spawn_pts.Add(random_pts_4b);

            Vector3 random_pts_5b = new Vector3(12, 0, -9);
            spawn_pts.Add(random_pts_5b);


            //zone c
            Vector3 random_pts_1c = new Vector3(Random.Range(-25, 1), 0, Random.Range(29,33));
            spawn_pts.Add(random_pts_1c);

            Vector3 random_pts_2c = new Vector3(Random.Range(-14, -6), 0, Random.Range(33,42));
            spawn_pts.Add(random_pts_2c);

            Vector3 random_pts_3c = new Vector3(Random.Range(-12, -9), 0, Random.Range(43,46));
            spawn_pts.Add(random_pts_3c);

            Vector3 random_pts_4c = new Vector3(Random.Range(-17, -7), 0, Random.Range(46,51));
            spawn_pts.Add(random_pts_4c);

            Vector3 random_pts_5c = new Vector3(Random.Range(-26, -24), 0, Random.Range(33,42));
            spawn_pts.Add(random_pts_5c);

            //zone x
            Vector3 random_pts_x = new Vector3(31, 0, -28);
            spawn_pts.Add(random_pts_x);

            int index = Random.Range(0,spawn_pts.Count-1);
            Instantiate(pickUps[0],spawn_pts[index],Quaternion.identity);
        } else if (timer >= interval && GameObject.FindGameObjectsWithTag("SpeedIncrease").Length < 1)
        {
            //zone a
            Vector3 random_pts_1a = new Vector3(Random.Range(23, 28), 0, Random.Range(3,12));
            spawn_pts.Add(random_pts_1a);

            Vector3 random_pts_2a = new Vector3(Random.Range(29, 35), 0, Random.Range(-7,2));
            spawn_pts.Add(random_pts_2a);

            Vector3 random_pts_3a = new Vector3(38, 0, 10);
            spawn_pts.Add(random_pts_3a);

            //zone b
            Vector3 random_pts_1b = new Vector3(Random.Range(15, 17), 0, Random.Range(3,20));
            spawn_pts.Add(random_pts_1b);

            Vector3 random_pts_2b = new Vector3(Random.Range(-19, 17), 0, Random.Range(20,22));
            spawn_pts.Add(random_pts_2b);

            Vector3 random_pts_3b = new Vector3(Random.Range(-10, -3), 0, Random.Range(-2,20));
            spawn_pts.Add(random_pts_3b);

            Vector3 random_pts_4b = new Vector3(Random.Range(-7, 11), 0, Random.Range(-10,-4));
            spawn_pts.Add(random_pts_4b);

            Vector3 random_pts_5b = new Vector3(12, 0, -9);
            spawn_pts.Add(random_pts_5b);


            //zone c
            Vector3 random_pts_1c = new Vector3(Random.Range(-25, 1), 0, Random.Range(29,33));
            spawn_pts.Add(random_pts_1c);

            Vector3 random_pts_2c = new Vector3(Random.Range(-14, -6), 0, Random.Range(33,42));
            spawn_pts.Add(random_pts_2c);

            Vector3 random_pts_3c = new Vector3(Random.Range(-12, -9), 0, Random.Range(43,46));
            spawn_pts.Add(random_pts_3c);

            Vector3 random_pts_4c = new Vector3(Random.Range(-17, -7), 0, Random.Range(46,51));
            spawn_pts.Add(random_pts_4c);

            Vector3 random_pts_5c = new Vector3(Random.Range(-26, -24), 0, Random.Range(33,42));
            spawn_pts.Add(random_pts_5c);

            //zone x
            Vector3 random_pts_x = new Vector3(31, 0, -28);
            spawn_pts.Add(random_pts_x);
            
            int index = Random.Range(0,spawn_pts.Count-1);
            Instantiate(pickUps[1],spawn_pts[index],Quaternion.identity);
            Debug.Log("Speed added");
            timer = 0.0f;
        }
    }
}
