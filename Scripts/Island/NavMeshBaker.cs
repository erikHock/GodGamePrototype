using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(MeshDeformer))]
public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface _meshSurface;

    private void Awake()
    {
        _meshSurface = GetComponentInChildren<NavMeshSurface>();
    }

    private void OnEnable()
    {
        MeshDeformer.OnIslandComplete += _meshDeformer_OnIslandComplete;
    }
    private void OnDisable()
    {
        MeshDeformer.OnIslandComplete -= _meshDeformer_OnIslandComplete;
    }

    private void _meshDeformer_OnIslandComplete(List<Vector3> obj)
    {
        BakeMeshSurface();
    }


    public void BakeMeshSurface()
    {
        _meshSurface.buildHeightMesh = true;
        _meshSurface.BuildNavMesh();
    }
}
