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
    }


    private void OnEnable()
    {
        var config = new VolumStreamConfig 
        {
            FrameRate = _frameRate,
            PointCount = _pointCount
        };

        _stream = VolumSDK.InitStream(config);
    }

}
