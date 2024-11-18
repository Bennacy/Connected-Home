using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AddMeshCollider))] 
public class AddMeshColliderEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        AddMeshCollider script = (AddMeshCollider)target;

        if(GUILayout.Button("Add Colliders")){
            script.AddMeshColliders();
        }
        
        // base.OnInspectorGUI();
    }

}