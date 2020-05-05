using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    Button startButton = null;

    private void Awake()
    {
        startButton.onClick.AddListener(startGame);
    }

    private void startGame()
    {
        SceneManager.LoadScene("Game");
    }
}
