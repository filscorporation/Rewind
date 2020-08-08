using System;
using UnityEngine;

namespace Source
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] [Range(0.1f, 10f)] private float speed;
        [SerializeField] [Range(0f, 5f)] private float offset;

        private void Update()
        {
            if (target == null)
                return;
            Vector2 v = Vector2.Lerp(transform.position, target.position + target.right * offset, Time.deltaTime * speed);
            transform.position = new Vector3(v.x, v.y, -10);
        }
    }
}