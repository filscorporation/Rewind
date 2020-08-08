using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(LineRenderer))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private LayerMask aimLayerMask;
        [SerializeField] private LayerMask shootLayerMask;
        private const float viewDistance = 50f;
        
        private LineRenderer line;

        public float ShootDelay = 0.5f;
        public float Damage = 10f;

        private void Start()
        {
            line = GetComponent<LineRenderer>();
        }
        
        private void Update()
        {
            DrawAim();
        }

        private void DrawAim()
        {
            Vector3 mp;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, viewDistance, aimLayerMask);
            if (hit.collider == null)
            {
                mp = transform.position + transform.right * viewDistance + new Vector3(0, 0, -5);
            }
            else
            {
                mp = (Vector3)hit.point + new Vector3(0, 0, -5);
            }
            
            line.SetPositions(new []{ transform.position, mp });
        }

        public IShootTarget Shoot()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, viewDistance, shootLayerMask);
            if (hit.collider != null)
            {
                return hit.collider.gameObject.GetComponent<IShootTarget>();
            }

            return null;
        }
    }
}