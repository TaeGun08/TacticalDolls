using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageRoom : MonoBehaviour
{
    [SerializeField] private Button stage1;
    [SerializeField] private Button stage2;
    [SerializeField] private Button stage3;
    [SerializeField] private Button stage4;

    private void Start()
    {
        stage1.onClick.AddListener(() =>
        {
            TileManager.selectedStageStatic = StageType.Stage1;
            SceneManager.LoadScene("ProtoIngame");
        });
        
        stage2.onClick.AddListener(() =>
        {
            TileManager.selectedStageStatic = StageType.Stage2;
            SceneManager.LoadScene("ProtoIngame");
        });
        
        stage3.onClick.AddListener(() =>
        {
            TileManager.selectedStageStatic = StageType.Stage3;
            SceneManager.LoadScene("ProtoIngame");
        });
        
        stage4.onClick.AddListener(() =>
        {
            TileManager.selectedStageStatic = StageType.Stage4;
            SceneManager.LoadScene("ProtoIngame");
        });
    }
}
