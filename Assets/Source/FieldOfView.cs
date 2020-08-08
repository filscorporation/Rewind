using System;
using UnityEngine;

namespace Source
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfView : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Transform target;
        
        private MeshFilter meshFilter;
        private const float viewDistance = 50f;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            UpdateFieldOfView();
        }

        private void UpdateFieldOfView()
        {
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            float fov = 90f;
            Vector3 origin = target.position;
            int rayCount = 350;

            float angle = target.eulerAngles.z + fov / 2f;
            float angleStep = fov / rayCount;
            
            Vector3[] vertices = new Vector3[rayCount + 2];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[rayCount * 3];

            vertices[0] = origin;

            int vertexIndex = 1;
            int triangleIndex = 0;
            for (int i = 0; i <= rayCount; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(origin, AngleToVector(angle), viewDistance, layerMask);
                if (hit.collider == null)
                {
                    vertices[vertexIndex] = origin + AngleToVector(angle) * viewDistance;
                }
                else
                {
                    vertices[vertexIndex] = hit.point;
                }

                if (i > 0)
                {
                    triangles[triangleIndex] = 0;
                    triangles[triangleIndex + 1] = vertexIndex - 1;
                    triangles[triangleIndex + 2] = vertexIndex;

                    triangleIndex += 3;
                }

                vertexIndex++;
                angle -= angleStep;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }

        private Vector3 AngleToVector(float angle)
        {
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        private float VectorToAngle(Vector3 vector)
        {
            vector = vector.normalized;
            float n = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
            if (n < 0)
                n += 360;

            return n;
        }
    }
}