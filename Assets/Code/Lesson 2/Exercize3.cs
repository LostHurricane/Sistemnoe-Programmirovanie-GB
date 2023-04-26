using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Collections;

/// <summary>
/// Часть 3*: создайте задачу типа IJobForTransform, которая будет вращать указанные
/// Transform вокруг своей оси с заданной скоростью.
/// </summary>
public class Exercize3 : MonoBehaviour
{
    [SerializeField]
    private Transform[] _transform;

    [SerializeField]
    private Transform _direction;
    
    [SerializeField]
    private float _turningSpeed;

    private TransformAccessArray _nativeTransformArray;


    private void Start()
    {
        if (_transform == null || _transform.Length == 0)
            return;

        _nativeTransformArray = new TransformAccessArray(_transform);
    }

    private void Update()
    {
        var job = new MyJob()
        {
            Direction = _direction.rotation.eulerAngles,
            Speed = _turningSpeed,
            DeltaTime = Time.deltaTime,
        };

        var handle = job.Schedule(_nativeTransformArray);
        handle.Complete();
    }

    private struct MyJob : IJobParallelForTransform
    {
        public Vector3 Direction;
        public float Speed;
        public float DeltaTime;


        public void Execute(int index, TransformAccess transform)
        {
            transform.localRotation = transform.localRotation * Quaternion.AngleAxis( Speed, Direction);
        }
    }

    private void OnDestroy()
    {
        if (_nativeTransformArray.isCreated)
        {
            _nativeTransformArray.Dispose();
        }
    }
}
