using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorScript : MonoBehaviour
{
    public Transform playerTarget;
    public Transform mirror;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        transform.position = mirror.TransformPoint(new Vector3(localPlayer.x, localPlayer.y, -localPlayer.z));

        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localPlayer.x, localPlayer.y, localPlayer.z));
        transform.LookAt(lookAtMirror);
    }
}
