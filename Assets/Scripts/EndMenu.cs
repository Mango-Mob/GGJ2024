using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text money;
    public static string moneyText = "";
    // Start is called before the first frame update
    void Start()
    {
        money.text = moneyText;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0))
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
