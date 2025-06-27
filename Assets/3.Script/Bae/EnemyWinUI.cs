using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyWinUI : MonoBehaviour
{
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LobbyScene");
        });
    }
}
