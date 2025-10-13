using System;
using TMPro;
using UnityEngine;

public class HourTimer : MonoBehaviour
{
    private TextMeshProUGUI hourText;
    private float updateTimer = 0f;

    private void Awake()
    {
        hourText = GetComponent<TextMeshProUGUI>();
        hourText.text = Get12HourFormat();
    }

    private void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= 60f)
        {
            updateTimer = 0f;

            hourText.text = Get12HourFormat();
        }
    }

    private String Get12HourFormat()
    {
        DateTime now = DateTime.Now;
        return now.ToString("h:mm tt");
    }
}
