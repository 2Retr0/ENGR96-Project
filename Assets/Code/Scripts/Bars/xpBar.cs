using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class xpBar : MonoBehaviour
{
    private Slider slider;

    public float fillSpeed = 0.5f;

    private float targetProgress = 0;

    private void Awake(){
        GameObject[] temp = GameObject.FindGameObjectsWithTag("xpBar");
        slider = temp[0].GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        IncrementProgress(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(slider.value < targetProgress){
            slider.value += fillSpeed * Time.deltaTime;
        }

    }

    public void IncrementProgress(float newProgress){
        targetProgress = slider.value + newProgress;
    }
}
