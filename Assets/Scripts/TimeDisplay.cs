using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<TMPro.TMP_Text>().text = "Time: " + GameManager.Instance.GetTimeNow();
    }
}
