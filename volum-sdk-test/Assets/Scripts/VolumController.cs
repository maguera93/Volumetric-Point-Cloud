using UnityEngine;
using Volum.SDK;

public class VolumController : MonoBehaviour
{
    [SerializeField]
    private int _pointCount = 50000;
    [SerializeField]
    private int _frameRate = 60;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    private void OnEnable()
    {
        var config = new VolumStreamConfig();
    }
}
