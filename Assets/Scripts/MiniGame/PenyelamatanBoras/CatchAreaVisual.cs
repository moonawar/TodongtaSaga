using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class CatchAreaVisual : MonoBehaviour
    {
        [SerializeField] private PlayerCatch playerCatch;
        // private float catchRadius = 2f;
        // private float catchAngle = 45f;

        // private Mesh catchAreaMesh;
        // private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        [Header("Sorting Layer Settings")]
        [SerializeField] private string sortingLayerName = "Default";
        [SerializeField] private int sortingOrder = 0;

        [Header("Mesh Settings")]
        [SerializeField] private int segmentCount = 10;


        private void Awake() {
            // meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            meshRenderer.sortingLayerName = sortingLayerName;
            meshRenderer.sortingOrder = sortingOrder;

            // CreateCatchAreaMesh();
        }

        // private void CreateCatchAreaMesh()
        // {
        //     catchAreaMesh = new Mesh();
        //     meshFilter.mesh = catchAreaMesh;

        //     float halfAngle = catchAngle * 0.5f * Mathf.Deg2Rad;
        //     float angleStep = catchAngle * Mathf.Deg2Rad / segmentCount;

        //     Vector3[] vertices = new Vector3[segmentCount + 2];
        //     int[] triangles = new int[segmentCount * 3];

        //     vertices[0] = Vector3.zero;

        //     for (int i = 0; i <= segmentCount; i++)
        //     {
        //         float currentAngle = -halfAngle + i * angleStep;
        //         vertices[i + 1] = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0) * catchRadius / scaleFactor;

        //         if (i < segmentCount)
        //         {
        //             triangles[i * 3] = 0;
        //             triangles[i * 3 + 1] = i + 1;
        //             triangles[i * 3 + 2] = i + 2;
        //         }
        //     }

        //     catchAreaMesh.vertices = vertices;
        //     catchAreaMesh.triangles = triangles;
        //     catchAreaMesh.RecalculateNormals();
        // }

        // public void UpdateVisual(float newRadius, float newAngle)
        // {
        //     if (catchRadius != newRadius || catchAngle != newAngle)
        //     {
        //         catchRadius = newRadius;
        //         catchAngle = newAngle;
        //         CreateCatchAreaMesh();
        //     }
        // }

        private static void CreateCatchAreaMesh(ref Mesh mesh, float radius, float angle, int segments, float scaleFactor)
        {
            float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
            float angleStep = angle * Mathf.Deg2Rad / segments;

            Vector3[] vertices = new Vector3[segments + 2];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;

            float rps = radius / scaleFactor;

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -halfAngle + i * angleStep;
                vertices[i + 1] = new Vector3(Mathf.Sin(currentAngle), Mathf.Cos(currentAngle), 0) * rps;

                if (i < segments)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

    #if UNITY_EDITOR
        [MenuItem("Tools/Save Catch Area Mesh")]
        public static void SaveCatchAreaMesh()
        {
            CatchAreaVisual self = FindObjectOfType<CatchAreaVisual>();
            if (self != null && self.playerCatch != null)
            {
                float radius = self.playerCatch.CatchRadius;
                float angle = self.playerCatch.CatchAngle;
                int segments = self.segmentCount;
                float scaleFactor = self.playerCatch.transform.localScale.x;

                // Create the mesh
                Mesh catchAreaMesh = new Mesh();
                CreateCatchAreaMesh(ref catchAreaMesh, radius, angle, segments, scaleFactor);

                // Save the mesh as an asset
                string path = "Assets/Meshes/CatchArea.asset";
                AssetDatabase.CreateAsset(catchAreaMesh, path);
                AssetDatabase.SaveAssets();

                self.GetComponent<MeshFilter>().mesh = catchAreaMesh;

                Debug.Log("Catch area mesh saved to " + path);
            }
            else
            {
                Debug.LogError("CatchAreaVisual or PlayerCatch not found in the scene.");
            }
        }
    #endif
    }
}