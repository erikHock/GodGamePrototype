#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class IslandBoundaries : MonoBehaviour
{
    private float minX, maxX, minZ, maxZ;
    
    public BoundariesOfIsland boundaries { get; private set; }

    public static event Action<BoundariesOfIsland> OnIslandBoundariesGetted;
   
    private void OnEnable()
    {
        MeshDeformer.OnIslandComplete += GetIslandBoundaries;
    }

    private void OnDisable()
    {
        MeshDeformer.OnIslandComplete -= GetIslandBoundaries;
    }

    public void GetIslandBoundaries(List<Vector3> cachedVertices)
    {
        var minXquery = (from x in cachedVertices select x.x).Min();
        var maxXquery = (from x in cachedVertices select x.x).Max();
        var minZquery = (from x in cachedVertices select x.z).Min();
        var maxZquery = (from x in cachedVertices select x.z).Max();

        minX = minXquery;
        maxX = maxXquery;
        minZ = minZquery;
        maxZ = maxZquery;

        OnIslandBoundariesGetted?.Invoke(new BoundariesOfIsland(minX, maxX, minZ, maxZ));
    }

    public Vector2 GetBoundariesCenter()
    {
        Vector2 center = new Vector2((maxX - minX) / 2, (maxZ - minZ) / 2);
        return center;
    }

#if DEBUGGING
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        float linesHeight = 10f;

        Vector3 bottomLeft = new Vector3(minX, linesHeight, minZ);
        Vector3 bottomRight = new Vector3(maxX, linesHeight, minZ);
        Vector3 topLeft = new Vector3(minX, linesHeight, maxZ);
        Vector3 topRight = new Vector3(maxX, linesHeight, maxZ);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
#endif
}

public struct BoundariesOfIsland
{
    public float minX { get; }
    public float maxX { get; }
    public float minZ { get; }
    public float maxZ { get; }

    public BoundariesOfIsland(float minX,float maxX, float minZ, float maxZ)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minZ = minZ;
        this.maxZ = maxZ;
    }

    public bool isNull()
    {
        if(minX == 0 && maxX == 0 && minZ == 0 && maxZ == 0)
        {
            return true;
        }
        return false;
    }
}

