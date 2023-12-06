using System.Collections;
using System.Collections.Generic;
using Code.Scripts;
using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        CompassManager.Instance.AddPositionSpeed(transform.position);
    }

    public void OnDisable()
    {
        CompassManager.Instance.RemovePosition(transform.position);
        Debug.Log("HELLLO!!!");
    }
}
