using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TestMovement : MonoBehaviour
{
    //private float _radius = 5f;
    private float _time;
    private float _speed = 10;
    private float _noiseFrequency = 0.1f;
    private float3 basePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        float acceleration = _time * _speed;

        float x = noise.snoise(new float4(basePos * _noiseFrequency, acceleration));
        float y = noise.snoise(new float4(basePos.yzx * _noiseFrequency, acceleration + 30f));
        float z = noise.snoise(new float4(basePos.zxy * _noiseFrequency, acceleration + 70f));

        float3 newPos = basePos + new float3(x, y, z) * 0.6f;

        float angle = acceleration * 0.15f;
        float cs = math.cos(angle);
        float sn = math.sin(angle);
        newPos = new float3(newPos.x * cs - newPos.z * sn, newPos.y, newPos.x * sn + newPos.z * cs);

        transform.position = newPos;
    }
}
