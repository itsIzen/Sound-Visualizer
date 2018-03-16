using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatingScript : MonoBehaviour {

   float mfX;
     float mfY;
     // Use this for initialization
     void Start ()
     {
        mfX = transform.position.x - transform.localScale.x/2.0f;
         mfY = transform.position.y - transform.localScale.y/2.0f;
     }
     
     // Update is called once per frame
     void LateUpdate () 
     {
             Vector3 v3Scale = transform.localScale;
             transform.localScale = new Vector3(v3Scale.x, v3Scale.y, v3Scale.z);
             transform.position = new Vector3(mfX + transform.localScale.x / 2.0f, mfY  + transform.localScale.y / 2.0f , transform.position.z);
     }
}
