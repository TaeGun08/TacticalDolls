using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DataType
{
    Character,
    Weapon
}

public class MyRoomData : MonoBehaviour
{
    public DataType dataType;
    
    public int characterID;
    public int weaponID;

    
    [Header("DataUI Setting")]
    [SerializeField] private Image image;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Sprite sprite;

    private void Awake()
    {
        image.sprite = sprite;
    }

    public void SetActive()
    {
        canvasGroup.alpha = 1;
    }
}
