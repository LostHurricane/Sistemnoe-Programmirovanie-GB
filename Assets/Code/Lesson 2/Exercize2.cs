using System.Collections;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// Часть 2. Cоздайте задачу типа IJobParallelFor, которая будет принимать данные в 
/// виде двух контейнеров: Positions и Velocities — типа NativeArray<Vector3>. Также
/// создайте массив FinalPositions типа NativeArray<Vector3>. 
/// Сделайте так, чтобы в результате выполнения задачи в элементы массива 
/// FinalPositions были записаны суммы соответствующих элементов массивов Positions 
/// и Velocities. Вызовите выполнение созданной задачи из внешнего метода и выведите в консоль
/// результат.
/// </summary>
public class Exercize2 : MonoBehaviour
{
    [SerializeField]
    private Vector3[] _positionsSource;
    [SerializeField]
    private Vector3[] _velocitiesSource;
    
    private NativeArray<Vector3> Positions; 
    private NativeArray<Vector3> Velocities;
    private NativeArray<Vector3> FinalPositions;

    IEnumerator Start()
    {
        Positions = new NativeArray<Vector3>(_positionsSource, Allocator.Persistent);
        Velocities = new NativeArray<Vector3>(_velocitiesSource, Allocator.Persistent);
        FinalPositions = new NativeArray<Vector3>(Mathf.Min(_positionsSource.Length, _velocitiesSource.Length), Allocator.Persistent);

        PrintData(Positions, "positions");
        PrintData(Velocities, "Velocities");

        var job = new MyJob()
        {
            Positions = this.Positions,
            Velocities = this.Velocities,
            FinalPositions = this.FinalPositions
        };

        var handle = job.Schedule(Mathf.Min(_positionsSource.Length, _velocitiesSource.Length), 0);
        int i = 0;
        while (!handle.IsCompleted)
        {
            Debug.Log($"{++i} frame before complete");
            yield return null;
        }

        handle.Complete();
        PrintData(FinalPositions, "final");

        Positions.Dispose();
        Velocities.Dispose();
        FinalPositions.Dispose();
    }

    private struct MyJob: IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Vector3> Positions;
        [ReadOnly]
        public NativeArray<Vector3> Velocities;
        [WriteOnly]
        public NativeArray<Vector3> FinalPositions;

        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }
    }


    private void PrintData(NativeArray <Vector3> array, string keyword)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Debug.Log($"{keyword} num  {i} = {array[i]}");

        }
    }

}
