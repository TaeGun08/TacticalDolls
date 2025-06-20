using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyRoom : MonoBehaviour
{
    [SerializeField] private Button myInfoButton;
    [SerializeField] private Button optionButton;
    
    [SerializeField] private GameObject myInfoPanel;
    [SerializeField] private GameObject optionPanel;
    
    [SerializeField] private Button languageButton;
    [SerializeField] private Button soundButton;
    
    [SerializeField] private GameObject languagePanel;
    [SerializeField] private GameObject soundPanel;

    private void Start()
    {
        myInfoButton.onClick.AddListener(() =>
        {
            myInfoPanel.SetActive(true);
            optionPanel.SetActive(false);
        });
        
        optionButton.onClick.AddListener(() =>
        {
            myInfoPanel.SetActive(false);
            optionPanel.SetActive(true);
        });
        
        languageButton.onClick.AddListener(() =>
        {
            languagePanel.SetActive(true);
            soundPanel.SetActive(false);
        });
        
        soundButton.onClick.AddListener(() =>
        {
            languagePanel.SetActive(false);
            soundPanel.SetActive(true);
        });
    }
}
