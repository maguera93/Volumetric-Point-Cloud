using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Volum.SDK;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    [SerializeField]
    private float _seed = 5f;

    private VolumStream _stream;

    VolumFrame _frame;
    Mesh _mesh;
    NativeArray<int> _indices;
    int _allocatedPointCount = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var filter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();

        _mesh = new Mesh { name = "VolumStream Mesh" };
        _mesh.MarkDynamic(); // hint the driver this mesh's vertex data changes every frame
        filter.sharedMesh = _mesh;
    }


    private void OnEnable()
    {
        var config = new VolumStreamConfig 
        {
            FrameRate = _frameRate,
            PointCount = _pointCount,
            VolumRadius = _volumeRadius,
            NoiseFrequency = _noiseFrequency,
            Speed = _speed,
            Seed = (uint)Time.time
        };

        _stream = VolumSDK.InitStream(config);
        _stream.OnFrameReady += UpdateMesh;
        _stream.Start();
    }

    private void OnDisable()
    {
        if (_stream != null)
            return; 

        _stream.OnFrameReady -= UpdateMesh;
        _stream.Dispose();
        _stream = null;
    }

    private void OnDestroy()
    {
        if (_indices.IsCreated) _indices.Dispose();
        if (_mesh != null) Destroy(_mesh);
    }

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


    private void AllocateMeshBuffers(int count)
    {
        _mesh.Clear();

        var layout = new[]
        {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
            };
        _mesh.SetVertexBufferParams(count, layout);

        if (_indices.IsCreated) _indices.Dispose();
        _indices = new NativeArray<int>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        for (int i = 0; i < count; i++) _indices[i] = i;

        _mesh.SetIndexBufferParams(count, IndexFormat.UInt32);
        _mesh.SetIndexBufferData(_indices, 0, 0, count);
        _mesh.SetSubMesh(0, new SubMeshDescriptor(0, count, MeshTopology.Points));

        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * (_volumeRadius * 3f));
        _allocatedPointCount = count;
    }

}
