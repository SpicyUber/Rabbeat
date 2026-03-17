using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float TimeInSeconds = 5f;
    private float _t;
    public Vector3 StartingPosition, EndingPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        _t += Time.deltaTime;

    }
    void LateUpdate()
    {
        
        transform.position = Vector3.Lerp(StartingPosition, EndingPosition, _t/TimeInSeconds);
    }
}
