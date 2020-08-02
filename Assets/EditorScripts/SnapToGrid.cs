using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SnapToGrid : MonoBehaviour
{
    #if UNITY_EDITOR
    // Start is called before the first frame update
    void Update()
    {
        if(Application.isEditor) {
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        }
    }
    #endif
}
