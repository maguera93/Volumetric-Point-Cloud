using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public struct TransformJob : IJobParallelFor
{
    public float Time;
    public float Speed;
    public float NoiseFrequency;
    public NativeArray<float3> BasePos;
    public NativeArray<float3> OutPut;

    //  Execute in parallel
    public void Execute(int index)
    {
        float3 basePos = BasePos[index];
        float acceleration = Time * Speed;

        float x = noise.snoise(new float4(basePos * NoiseFrequency, acceleration));
        float y = noise.snoise(new float4(basePos.yzx * NoiseFrequency, acceleration + 30f));
        float z = noise.snoise(new float4(basePos.zxy * NoiseFrequency, acceleration + 70f));

        float3 newPos = basePos + new float3(x, y, z) * 0.6f;

        float angle = acceleration * 0.15f;
        float cs = math.cos(angle);
        float sn = math.sin(angle);
        newPos = new float3(newPos.x * cs - newPos.z * sn, newPos.y, newPos.x * sn + newPos.z * cs);

        OutPut[index] = newPos;
    }
}

public class TestMovement : MonoBehaviour
{
    private JobHandle _handle;
    public Transform[] spheres;

    public float _speed = 10;
    public float _noiseFrequency = 1;

    public NativeArray<float3> basePos;
    public NativeArray<float3> result;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Persistent: vive hasta que lo liberemos manualmente (Temp solo dura el frame/scope actual)
        basePos = new NativeArray<float3>(spheres.Length, Allocator.Persistent);
        result = new NativeArray<float3>(spheres.Length, Allocator.Persistent);

        for (int i = 0; i < spheres.Length; i++)
        {
            basePos[i] = spheres[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Completamos el job del frame anterior antes de tocar los arrays o programar uno nuevo
        _handle.Complete();

        // Aplicamos el resultado del job ya completado a las esferas
        for (int i = 0; i < spheres.Length; i++)
        {
            spheres[i].position = result[i];
        }

        TransformJob job = new TransformJob
        {
            Time = Time.time,
            Speed = _speed,
            NoiseFrequency = _noiseFrequency,
            BasePos = basePos,
            OutPut = result
        };

        _handle = job.Schedule(spheres.Length, 64);
    }

    void LateUpdate()
    {
        // Nos aseguramos de que el job termine antes de que el frame acabe
        _handle.Complete();
    }

    void OnDestroy()
    {
        _handle.Complete();

        if (basePos.IsCreated) basePos.Dispose();
        if (result.IsCreated) result.Dispose();
    }
}
