using UnityEngine;

public class MeshDebugger : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;

    private void OnDrawGizmos()
    {

        if (meshFilter == null || meshFilter.mesh == null)
        {
            return;
        }

        var mesh = meshFilter.mesh;

        Quaternion rotation = meshFilter.transform.rotation;
        Vector3 scale = meshFilter.transform.localScale;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 position = Vector3.Scale(rotation * mesh.vertices[i], scale);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(position, 0.2f);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position, mesh.normals[i] * 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, mesh.tangents[i] * 2f);
        }

    }
}
