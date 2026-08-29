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

    private VolumStream _stream;

    VolumFrame _frame;
    Mesh _mesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        var filter = gameObject.AddComponent<MeshFilter>();
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();

        _mesh = new Mesh { name = "VolumStream Mesh" };
        _mesh.MarkDynamic(); // hint the driver this mesh's vertex data changes every frame
        filter.sharedMesh = _mesh;

        _frame = new VolumFrame();
        _frame.Initialize();
    }
    /*
    private void Update()
    {
        _frame.UpdateFrame();

        foreach(var point in _frame.volumPoints)
        {
            int i = 0;
            AllocateMeshBuffers(i);
            _mesh.SetVertexBufferData(
            _frame.volumPoints, 0, 0, 100, 0,
            MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices);
            i++;
        }
    }*/

    private void OnEnable()
    {
        var config = new VolumStreamConfig 
        {
            FrameRate = _frameRate,
            PointCount = _pointCount
        };

        _stream = VolumSDK.InitStream(config);
    }
    /*
    private void AllocateMeshBuffers(int count)
    {

        var layout = new[]
        {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4),
            };
        _mesh.SetVertexBufferParams(count, layout);


        _mesh.SetIndexBufferParams(count, IndexFormat.UInt32);
        _mesh.SetSubMesh(0, new SubMeshDescriptor(0, count, MeshTopology.Points));

        _mesh.bounds = new Bounds(Vector3.zero, Vector3.one * (5f * 3f));
    }*/
}
