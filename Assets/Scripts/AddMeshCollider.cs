using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;


public class AddMeshCollider : MonoBehaviour
{
    [SerializeField] private Transform self;
    
    public void AddMeshColliders(Transform target = null, int depth = 0){
        if(depth > 20){
            Debug.LogError("Child Depth Exceeded!");
            return;
        }

        if(target == null){
            target = self;
        }

        foreach(Transform child in target){
            if(child.childCount > 0){
                AddMeshColliders(child, depth+1);
            }
            else if(child.gameObject.GetComponent<MeshRenderer>() != null){
                child.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
    
}
