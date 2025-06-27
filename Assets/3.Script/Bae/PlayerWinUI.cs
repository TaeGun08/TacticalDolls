using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerWinUI : MonoBehaviour
{
    [SerializeField] private GameObject goodsPrefab;
    [SerializeField] private Transform goodsSpawnPoint;
    [SerializeField] private Sprite goodsSprite;
    
    [SerializeField] private Button confirmButton;

    private void Start()
    {
        confirmButton.onClick.AddListener(() =>
        {
            // TODO 재화 업데이트 추가
            SceneManager.LoadScene("LobbyScene");
        });
    }

    private void OnEnable()
    {
        var goods = Instantiate(goodsPrefab, goodsSpawnPoint);
        goods.GetComponent<Image>().sprite = goodsSprite;
        goods.GetComponentInChildren<TMP_Text>().text = "100";  // Goods 수량 넣어주기
    }
}
