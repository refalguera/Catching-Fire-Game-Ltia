using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

        public float startX;
        private float speedSecs = 120f;
        public float ynow = 0;
        [SerializeField]
        private Vector3 updatePos;
        [SerializeField]
        private Vector3 startPos;
    
    //Controls continuous camera movement during the game. The camera moves slowly from the bottom to the top of the game.
    void FixedUpdate()
        {
            if (ynow < speedSecs)
            {
            //  Debug.Log("camera update! " + Mathf.Lerp(startX, 0.5f, ynow));
            ynow += Time.deltaTime;

                transform.position = Vector3.Lerp(startPos, updatePos, ynow / speedSecs);
            }

    }
}
