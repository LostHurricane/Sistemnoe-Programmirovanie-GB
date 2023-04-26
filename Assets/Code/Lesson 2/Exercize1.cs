using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// Часть 1: Создайте задачу типа IJob, которая принимает данные в формате NativeArray<int> 
/// и в результате выполнения все значения более десяти делает равными нулю.
/// Вызовите выполнение этой задачи из внешнего метода и выведите в консоль результат.
/// </summary>
public class Exercize1 : MonoBehaviour
{
    [SerializeField]
    private int _length;
    [SerializeField]
    private int _min;
    [SerializeField]
    private int _max;

    private NativeArray<int> _ints; 

    IEnumerator Start()
    {
        _ints = new NativeArray<int>(GenerateRandomIntArray(_length, _min, _max), Allocator.TempJob);

        PrintData("before");

        var job = new MyJob
        {
            _data = _ints
        };

        var handle = job.Schedule();
        
        int i = 0;
        while (!handle.IsCompleted)
        {
            Debug.Log($"{++i} frame before complete");
            yield return null;
        }

        handle.Complete();
        PrintData("after");
        _ints.Dispose();

    }

    private void PrintData(string keyword)
    {
        for (int i = 0; i < _ints.Length; i++)
        {
            Debug.Log($"{keyword} ins num  {i} = {_ints[i]}");

        }
    }

    private int[] GenerateRandomIntArray(int length, int min, int max)
    {
        var temp = new int[length];

        for (int i = 0; i < length; i++)
        {
            temp[i] = UnityEngine.Random.Range(min, max);
        }
        return temp;
    }

    private struct MyJob : IJob
    {
        public NativeArray<int> _data;

        public void Execute()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] > 10)
                    _data[i] = 0;
            }
        }
    }

    private void OnDestroy()
    {
        if(_ints.IsCreated)
            _ints.Dispose();
    }

}
