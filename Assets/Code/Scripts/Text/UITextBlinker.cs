using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITextBlinker : MonoBehaviour
{
    private TMP_Text textComponent;
    private bool isTextVisible = true;

    void Start()
    {
        textComponent = GetComponent<TMP_Text>();
        // Invoke the BlinkText method every 1 second (change the second parameter to modify the blinking interval)
        InvokeRepeating(nameof(ToggleVisibility), 0.6f, 0.6f);
    }

    private void ToggleVisibility()
    {
        isTextVisible = !isTextVisible;
        textComponent.enabled = isTextVisible;
    }
}
