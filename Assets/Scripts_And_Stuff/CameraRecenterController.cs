using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRecenterController : MonoBehaviour
{
    public CinemachineFreeLook FreeLookCamera;
    public InputActionReference XYAxis;
    public InputActionReference MoveInput;
    public playerScript Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("CAMERATHING: XYAxis.action.ReadValue<Vector2>().x: " + XYAxis.action.ReadValue<Vector2>().x+ "MoveInput.action.ReadValue<Vector2>().magnitude" + MoveInput.action.ReadValue<Vector3>());
        FreeLookCamera.m_RecenterToTargetHeading.m_enabled = Player.IsInCombat || !((XYAxis.action.ReadValue<Vector2>().x != 0) || MoveInput.action.ReadValue<Vector3>().magnitude==0);
        
       
    }
}
