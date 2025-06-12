using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Test_Coroutine());
        _= Test_Task();
    }

    private async Task Test_Task()
    {
        Debug.Log("Task Start");
        await Test_Task2();

        Debug.Log("Task End");
    }

    private Task Test_Task2()
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log("Task Start");
        }
        
        StopCoroutine(Test_Coroutine());
        StartCoroutine(Test_Coroutine());
        return Task.CompletedTask;
    }

    private IEnumerator Test_Coroutine()
    {
        Debug.Log("Coroutine Start");
        yield return null;
    }
}
