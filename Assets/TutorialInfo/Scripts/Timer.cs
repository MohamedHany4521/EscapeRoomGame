using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _timeRemaining;

    // Update is called once per frame
    void Update()
    {
        if (_timeRemaining > 0)
        {
            _timeRemaining = Mathf.Max(0, _timeRemaining - Time.deltaTime);
        }

        else if (_timeRemaining <= 0)
        {
            _timerText.text = "00:00";
            return;
        }
        int minutes = Mathf.FloorToInt(_timeRemaining / 60);
        int seconds = Mathf.FloorToInt(_timeRemaining % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
