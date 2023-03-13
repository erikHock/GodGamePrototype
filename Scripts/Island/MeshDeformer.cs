//#define DEBUGGING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshDeformer : MonoBehaviour
{
    [SerializeField] private float _radius = 600f;
    [SerializeField] private float _deformationStrength = 17000f;
    [SerializeField] private float _smoothingFactor = 6f;

    [Header("Maximum Terrain Height")]
    [SerializeField] private bool _hasHeightMax;
    [SerializeField] private float _heightMax = 7.0f;

    [Header("Maximum Terrain Count")]
    [SerializeField] private bool _hasModifiedVertsMax;
    [SerializeField] private int _modifiedVertsMax;

    [Header("Modify Terrain Only from Neighborhood")]
    [SerializeField] private bool _modifyOnlyNeighborhood;
    
    // Bool for validation is setted on end of frame and used in loop before mesh recalculating
    private bool _neighborhoodIsValid = false;

    [Header("Every X frames, per second")]
    [SerializeField] private bool _hasUpdateInterval;
    [SerializeField] private int _updateInterval = 3;

    // Added hashset for performance problem with Contains() in list
    private HashSet<Vector3> _cachedModifiedVertices = new HashSet<Vector3>();
   
    // Added for method IsInRadius() because hashset not support [i]
    private List<Vector3> _cachedModifiedVertsList = new List<Vector3>();

    // Counts minimum verts with Y higher than 90% of max
    int _minimumVertsCounter = 0;

    // Maximum count for averaging distance of vertices
    int _maxNearestCount = 100;
    List<VectorWithDistance> _nearestVertices = new List<VectorWithDistance>();

    public static event Action<List<Vector3>> OnIslandComplete;

    private Mesh _mesh;
    private MeshCollider _collider;
    private Vector3[] _vertices, _modifiedVerts;
    private RaycastHit _hit;

#if DEBUGGING
    private Vector3 _gizmoHitPos;
    private bool _gizmoActive;
    public int maxVertsCachedCount;
#endif

    private void Awake()
    {
        _mesh = GetComponentInChildren<MeshFilter>().mesh;//GetComponent<MeshFilter>().mesh;
        _vertices = _mesh.vertices;
        _modifiedVerts = _mesh.vertices;
        _collider = GetComponentInChildren<MeshCollider>();//GetComponent<MeshCollider>();


    }

    private void RecalculateMesh()
    {
        _mesh.vertices = _modifiedVerts;
        _collider.sharedMesh = _mesh;
        _mesh.RecalculateNormals();
    }

    private void Update()
    {
        if(GameManager.Instance.currentState != GameStates.CreatingIsland)
        {
            return;
        }
        
        if(MouseRaycaster.Instance.isHit)
        {
            _hit = MouseRaycaster.Instance.hit;

            for (int v = 0; v < _modifiedVerts.Length; v++)
            {
                Vector3 distance = _modifiedVerts[v] - _hit.point;
                
                // Vector with X and Z for comparing with cached vertices
                Vector3 vecWithoutY = new Vector3(_modifiedVerts[v].x, 0f, _modifiedVerts[v].z);

                float force = _deformationStrength / (1f + _hit.point.sqrMagnitude);  // Before 1f it will be 10f
                
                if (distance.sqrMagnitude < _radius)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (_hasModifiedVertsMax)
                        {
                            if (_cachedModifiedVertices.Count < _modifiedVertsMax)
                            {
                                if (!_cachedModifiedVertices.Contains(vecWithoutY))
                                {
                                    _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);

                                    _cachedModifiedVertices.Add(vecWithoutY);
                                }

                                if (_cachedModifiedVertices.Contains(vecWithoutY))
                                {
                                    _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);
                                }

                            }
                        }
                        else if (_hasModifiedVertsMax && _modifyOnlyNeighborhood)
                        {
                            if (_cachedModifiedVertices.Count.Equals(0))
                            {
                                if (_cachedModifiedVertices.Count < _modifiedVertsMax)
                                {
                                    if (!_cachedModifiedVertices.Contains(vecWithoutY))
                                    {
                                        _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);

                                        //if (_modifiedVerts[v].y > 0.01f)
                                        _cachedModifiedVertices.Add(vecWithoutY);
                                    }
                                    
                                    if (_cachedModifiedVertices.Contains(vecWithoutY))
                                    {
                                        _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);
                                    }
                                    
                                }
                            }
                            else
                            {
                                if (_neighborhoodIsValid)
                                {
                                    if (_cachedModifiedVertices.Count < _modifiedVertsMax)
                                    {
                                        if (_cachedModifiedVertices.Contains(vecWithoutY))
                                        {
                                            _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);
                                        }
                                        
                                        if (!_cachedModifiedVertices.Contains(vecWithoutY))
                                        {
                                            _modifiedVerts[v] = MoveUp(_modifiedVerts[v], force);

                                            //if (_modifiedVerts[v].y > 0.01f)
                                            _cachedModifiedVertices.Add(vecWithoutY);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

#if DEBUGGING
        _gizmoHitPos = _hit.point;
        maxVertsCachedCount = _cachedModifiedVertices.Count;
#endif
        //Run every x frames
        if (_hasUpdateInterval)
        {
            if (Time.frameCount % _updateInterval == 0)
            {
                RecalculateMesh();
                _neighborhoodIsValid = IsInRadiusOfModifiedVertices(_hit);
            }
        }
        else
        {
            RecalculateMesh();
            _neighborhoodIsValid = IsInRadiusOfModifiedVertices(_hit);
        }
       

        // Send modified vertes in action event if its max count
        if (_hasModifiedVertsMax && _modifiedVertsMax == _cachedModifiedVertices.Count)
        {
            // Send only when is minimum 95% of vertices with 95% y height
            float minHeight = (_heightMax / 100) * 95;

            int minVertsCount = (_modifiedVerts.Length / 100) * 95;

            for (int i = 0; i < _modifiedVerts.Length; i++)
            {
                if (_modifiedVerts[i].y >= minHeight) // Height max of vertex
                {
                    _minimumVertsCounter++;
                }
            }
            
            if (_modifiedVerts.Length <= _minimumVertsCounter)
            {
                GameDebugLog.LogMessage(minVertsCount + "Is less or equal than " + _minimumVertsCounter);
               
                OnIslandComplete?.Invoke(_cachedModifiedVertices.ToList());
                GameManager.Instance.currentState = GameStates.GameLoop;
            }
        }
    }

    private bool IsInRadiusOfModifiedVertices(RaycastHit hit)
    {
        float nearestDistance = 1000f;

        // Clear list for next iteration
        _nearestVertices.Clear();

        // Find vertices nearest to hit point from cached array
        if (_cachedModifiedVertices.Count.Equals(0))
        {
            GameDebugLog.LogMessage("Cached vertices list is empty");
            return false;
        }
        else 
        {
            _cachedModifiedVertsList = _cachedModifiedVertices.ToList();

            for (int i = 0; i < _cachedModifiedVertsList.Count; i++)
            {
                Vector3 hitPoint = new Vector3(hit.point.x, 0f, hit.point.z);
                Vector3 cachedVertex = new Vector3(_cachedModifiedVertsList[i].x, 0f, _cachedModifiedVertsList[i].z);
                float dist = (cachedVertex - hitPoint).sqrMagnitude;

                if (dist < nearestDistance)
                {
                    GameDebugLog.LogMessage($"Distance:{dist} is less than Nearest:{nearestDistance}");
                    
                    nearestDistance = dist;

                    // Fill list with vectors
                    if (_nearestVertices.Count < _maxNearestCount)
                    {
                        _nearestVertices.Add(new VectorWithDistance(_cachedModifiedVertsList[i], nearestDistance));
                        
                        GameDebugLog.LogMessage($"Nearest vertices count:{_nearestVertices.Count}");
                    }
                    else
                    {
                        // If list is full cycle trought and change vector is distance is small than distance in list
                        for (int v = 0; v <= _nearestVertices.Count; v++)
                        {
                            if (_nearestVertices[v].GetDistance() > nearestDistance)
                            {
                                GameDebugLog.LogMessage($"Replacing {_nearestVertices[v].GetDistance()} with {nearestDistance}");
                                
                                _nearestVertices[v].SetVectorAndDistance(_cachedModifiedVertsList[i], nearestDistance);
                            }
                        }

                    }

                    
                }


            }
        }

        // Create average distance from list of vectors
        float averageDistance = nearestDistance; 
        for (int a = 0; a < _nearestVertices.Count; a++)
        {
            averageDistance += _nearestVertices[a].GetDistance();
        }

        averageDistance = averageDistance / _nearestVertices.Count;

        GameDebugLog.LogMessage($"Average distance is:{averageDistance} comparing with radius:{_radius * 1.5f}");

        if (averageDistance < _radius * 1.5f)
        {
#if DEBUGGING
            _gizmoActive = true;
#endif
            return true;
        }
        else
        {
#if DEBUGGING
            _gizmoActive = false;
#endif
            return false;
        }
    }


#if DEBUGGING
    private void OnDrawGizmos()
    {
        if(_gizmoActive)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_gizmoHitPos, _radius / 17f);
        }

        if(!_gizmoActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_gizmoHitPos, _radius / 17f);
        }
    }
#endif

    public Vector3 MoveUp(Vector3 vertexToMove, float force)
    {
        if (_hasHeightMax)
        {
            if (vertexToMove.y < _heightMax)
            {
                vertexToMove = vertexToMove + (Vector3.up * force) / _smoothingFactor;
                vertexToMove.y = Mathf.Clamp(vertexToMove.y, 0, _heightMax);

                return vertexToMove;
            }
        }
        else
        {
            vertexToMove = vertexToMove + (Vector3.up * force) / _smoothingFactor;
        }
        return vertexToMove;
    }

    
   
}

public struct VectorWithDistance
{
    private Vector3 _vector;
    private float _distance;
    
    public Vector3 GetVector()
    {
        return _vector;
    }

    public float GetDistance()
    {
        return _distance;
    }

    public void SetVector(Vector3 vector)
    {
        _vector = vector;
    }

    public void SetDistance(float distance)
    {
        _distance = distance;
    }

    public void SetVectorAndDistance(Vector3 vector, float distance)
    {
        _vector = vector;
        _distance = distance;
    }

    public VectorWithDistance(Vector3 vector, float distance)
    {
        _vector = vector;
        _distance = distance;
    }
}




