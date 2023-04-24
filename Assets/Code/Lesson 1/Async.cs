using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Async : MonoBehaviour
{
    private CancellationTokenSource _cancellationTokenSource;

    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        
        WhatTaskFasterAsync(_cancellationTokenSource.Token, Task1(_cancellationTokenSource.Token), Task2(_cancellationTokenSource.Token));
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

    }

    private void hfhf(CancellationToken token, Func<CancellationToken, Task> task1, Func<CancellationToken, Task> task2)
    {
        throw new NotImplementedException();
    }

    private async Task <bool> WhatTaskFasterAsync (CancellationToken cancellationToken, Task task1, Task task2)
    {
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            linkedCts.Cancel();
            linkedCts.Dispose();
            return false;
        }


        linkedCts.Cancel();
        linkedCts.Dispose();
        return false;
    }


    private async Task Task1 (CancellationToken cancellationToken)
    {
        Debug.Log("Starting Task1 waiting for 1 second");
        //await Task.Delay(1000);

        for (int i = 0; i < 10; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Task.Delay(1000/10);
            Debug.Log($"Task1 waited {i*100}");

        }
        Debug.Log("Ending Task1 finished waiting");

    }

    private async Task Task2(CancellationToken cancellationToken)
    {
        Debug.Log("Starting Task2 counting 60 frames");
        for (int i = 0; i < 60; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await Task.Yield();
        }
        Debug.Log("Ending Task2");
    }
}
