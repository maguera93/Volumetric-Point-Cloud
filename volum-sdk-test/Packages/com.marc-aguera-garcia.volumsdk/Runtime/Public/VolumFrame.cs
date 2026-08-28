using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Volum.SDK
{
    /// <summary>
    /// Process a frame
    /// </summary>
    public class VolumFrame
    {
        public VolumPoint[] volumPoints = new VolumPoint[100];
        private float3[] basePos = new float3[100];
        private float _radius = 5f;
        private float _time;
        private float _speed;
        private float _noiseFrequency;


        public void Initialize()
        {
            for (int i = 0; i < basePos.Length; i++)
            {
                VolumPoint point = new VolumPoint();

                Random rnd = Random.CreateFromIndex(1);
                
                float3 dir = rnd.NextFloat3Direction();
                float r = math.pow(rnd.NextFloat(), 1f / 3f) * _radius;

                basePos[i] = dir * r;
            }
        }

        public void UpdateFrame()
        {
            for (int i = 0; i < volumPoints.Length; i++)
            {

                volumPoints[i] = TranslatePoint(basePos[i]);
            }
        }

        // test movement
        public VolumPoint TranslatePoint(float3 basePos)
        {
            float acceleration = _time * _speed;

            float x = noise.snoise(new float4(basePos * _noiseFrequency, acceleration));
            float y = noise.snoise(new float4(basePos.yzx * _noiseFrequency, acceleration + 31.7f));
            float z = noise.snoise(new float4(basePos.zxy * _noiseFrequency, acceleration + 71.3f));

            float3 newPos = basePos + new float3(x, y, z) * 0.6f;

            // test color red
            float4 color = new float4(255f, 0f, 0f, 255f);

            return new VolumPoint { Position = newPos, Color =  color};
        }
    }
}