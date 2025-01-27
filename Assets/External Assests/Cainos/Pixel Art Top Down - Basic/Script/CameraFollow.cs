using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    // Let camera follow target and center it
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // The target to follow
        public float lerpSpeed = 1.0f; // Speed of camera movement

        private Vector3 targetPos;

        private void Start()
        {
            if (target == null) return;

            // Center the camera on the target at the start
            transform.position = target.position;
        }

        private void Update()
        {
            if (target == null) return;

            // Set the target position to the target's position (no offset)
            targetPos = target.position;

            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
        }
    }
}