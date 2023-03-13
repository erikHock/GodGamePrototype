#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DebugAi : MonoBehaviour
{
    [SerializeField] private GameObject _agentPrefab;
    [SerializeField] private NavMeshSurface _meshSurface;
    [SerializeField] private float _agentBaseOffset;

    private NavMeshAgent _agent;


    private bool _isSpawned = false;

    void Update()
    {
       
        if(MouseRaycaster.Instance.isHit)
        {
            RaycastHit hit = MouseRaycaster.Instance.hit;

            if (Input.GetKey(KeyCode.M) && !_isSpawned)
            {
                GameObject spawnedAgent = Instantiate(_agentPrefab, hit.point, Quaternion.identity);

                _agent = spawnedAgent.AddComponent<NavMeshAgent>();
                _agent.baseOffset = _agentBaseOffset;
                
                spawnedAgent.AddComponent<AnimationHandler>();

                _isSpawned = true;
            }

            if (Input.GetKey(KeyCode.N))
            {
                if (_agent != null)
                _agent.SetDestination(hit.point);
            }
        }

       
    }
}
