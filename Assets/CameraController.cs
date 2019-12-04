using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFreeLockCameraActive(bool bIs)
    {
        gameObject.GetComponent<CustomFreeLookCam>().enabled = bIs;
        gameObject.GetComponent<ProtectCameraFromWallClip>().enabled = bIs;
    }
}
