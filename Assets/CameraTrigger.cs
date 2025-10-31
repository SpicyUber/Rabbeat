using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
   public CinemachineFreeLook MainCamera;
    public CinemachineVirtualCamera[] OtherCameras;
    // Start is called before the first frame update
    void Start()
    {
        foreach(CinemachineVirtualCamera cam in OtherCameras)
        {
            cam.enabled = false;
        }

        MainCamera.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))
        {
            CinemachineVirtualCamera cam= other.GetComponentInChildren<CinemachineVirtualCamera>();
           // Debug.Log(cam + " rabbbeet");
            foreach (CinemachineVirtualCamera currentCam in OtherCameras)
            {
                currentCam.enabled = cam == currentCam;
                //Debug.Log((cam == currentCam) + " rabbbeet");
            }

            MainCamera.enabled = false;
        }
       

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraTrigger"))
        {
           ResetCamera();
           
        }


       
    }

    public void ResetCamera()
    {
        foreach (CinemachineVirtualCamera cam in OtherCameras)
        {
            cam.enabled = false;
        }

        MainCamera.enabled = true;
    }
}
