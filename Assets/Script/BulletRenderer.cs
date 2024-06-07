using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class BulletRenderer : MonoBehaviour
{
    [SerializeField] private Mesh bulletMeshHead;
    [SerializeField] private Material bulletMaterial;
    [SerializeField] private Mesh bulletMeshBody;

    readonly float z = -0.025456795f;

    static public BulletRenderer Instance { get; private set; }

    private void Start()
    {
        if(Instance == null)
            Instance = this;
    }

    public void RenderBulletsAtPositions(NativeArray<Vector3> positions, NativeArray<Quaternion> rotations)
    {
        // Create instance data array
        Matrix4x4[] instanceDataHead = new Matrix4x4[positions.Length];
        Matrix4x4[] instanceDataBody = new Matrix4x4[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            // Create a transformation matrix from the position
            Matrix4x4 matrix = Matrix4x4.TRS(positions[i] + new Vector3(0, 0, z), rotations[i] * Quaternion.Euler(0, 90, 0), Vector3.one);
            instanceDataHead[i] = matrix;
            matrix = Matrix4x4.TRS(positions[i], rotations[i] * Quaternion.Euler(0,90,0), Vector3.one);
            instanceDataBody[i] = matrix;
        }

        // Render using Graphics.DrawMeshInstanced
        Graphics.DrawMeshInstanced(bulletMeshHead, 0, bulletMaterial, instanceDataHead, positions.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off);
        Graphics.DrawMeshInstanced(bulletMeshBody, 0, bulletMaterial, instanceDataBody, positions.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off);
    }
}