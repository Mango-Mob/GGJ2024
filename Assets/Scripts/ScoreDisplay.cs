using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<TMPro.TMP_Text>().text = "Money: " + GameManager.Instance.GetScoreDisplay();
    }
}
