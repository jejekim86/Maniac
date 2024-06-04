using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [SerializeField] private Mesh bulletMeshHead;
    [SerializeField] private Material bulletMaterialHead;
    [SerializeField] private Mesh bulletMeshBody;
    [SerializeField] private Material bulletMaterialBody;

    float z = -0.025456795f;

    public void RenderBulletsAtPositions(List<Vector3> positions, List<Quaternion> rotations)
    {


        // Create instance data array
        Matrix4x4[] instanceData = new Matrix4x4[positions.Count];
        for (int i = 0; i < positions.Count; i++)
        {
            // Create a transformation matrix from the position
            Matrix4x4 matrix = Matrix4x4.TRS(positions[i] + new Vector3(0, 0, z), rotations[i], Vector3.one);
            instanceData[i] = matrix;
        }

        // Render using Graphics.DrawMeshInstanced
        Graphics.DrawMeshInstanced(bulletMeshHead, 0, bulletMaterialHead, instanceData, positions.Count);
        Graphics.DrawMeshInstanced(bulletMeshBody, 0, bulletMaterialBody, instanceData, positions.Count);

    }

}