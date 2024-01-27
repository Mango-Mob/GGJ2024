using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            GameManager.Instance.canSell = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            GameManager.Instance.canSell = false;
    }
}
