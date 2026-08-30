using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Volum.SDK;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace Volum.SDK.Samples.Demo
{
    public class VolumController : MonoBehaviour
    {
        [SerializeField]
        private int _pointCount = 50000;
        [SerializeField]
        private int _frameRate = 60;
        [SerializeField]
        private float _volumeRadius = 5f;
        [SerializeField]
        private float _speed = 1f;
        [SerializeField]
        private float _noiseFrequency = 0.15f;

        [Space, SerializeField]
        private Material _pointMaterial;

        private VolumStream _stream;

        private Mesh _mesh;
        private NativeArray<int> _index;
        private int _allocatedPointCount = -1;

        private FramesCounter _framesCounter;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            var filter = gameObject.AddComponent<MeshFilter>();
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _framesCounter =  GetComponent<FramesCounter>();

            _mesh = new Mesh { name = "VolumStream Mesh" };
            _mesh.MarkDynamic(); // hint the driver this mesh's vertex data changes every frame
            filter.sharedMesh = _mesh;

            meshRenderer.sharedMaterial = _pointMaterial != null
                ? _pointMaterial
                : new Material(Shader.Find("Hidden/Internal-Colored"));
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
        }


        private void OnEnable()
        {
            var config = new VolumStreamConfig
            {
                FrameRate = _frameRate,
                PointCount = _pointCount,
                VolumRadius = _volumeRadius,
                NoiseFrequency = _noiseFrequency,
                Speed = _speed
            };

            _stream = VolumSDK.InitStream(config);
            _stream.OnFrameReady += UpdateMesh;
            _stream.Start();

            _framesCounter.Track(_stream);
        }

        private void OnDisable()
        {
            if (_stream != null)
                return;

            _stream.OnFrameReady -= UpdateMesh;
            _stream.Dispose();
            _stream = null;
            _framesCounter.StopTracking();
        }

        private void OnDestroy()
        {
            if (_index.IsCreated) _index.Dispose();
            if (_mesh != null) Destroy(_mesh);
            _framesCounter.StopTracking();
        }

        /// <summary>
        /// View update of the mesh points
        /// </summary>
        /// <param name="frame"></param>
        private void UpdateMesh(VolumFrame frame)
        {
            if (frame.PointCount != _allocatedPointCount)
            {
                AllocateMeshBuffers(frame.PointCount);
            }

            _mesh.SetVertexBufferData(
                frame.Points, 0, 0, frame.PointCount, 0,
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
        }

        /// <summary>
        /// Only calls when the number of volum points changes
        /// </summary>
        /// <param name="count"></param>
        private void AllocateMeshBuffers(int count)
        {
            _mesh.Clear();

            var layout = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
            };
            _mesh.SetVertexBufferParams(count, layout);

            if (_index.IsCreated) _index.Dispose();
            _index = new NativeArray<int>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < count; i++) _index[i] = i;

            _mesh.SetIndexBufferParams(count, IndexFormat.UInt32);
            _mesh.SetIndexBufferData(_index, 0, 0, count);
            _mesh.SetSubMesh(0, new SubMeshDescriptor(0, count, MeshTopology.Points));

            _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * (_volumeRadius * 3f));
            _allocatedPointCount = count;
        }

    }
}