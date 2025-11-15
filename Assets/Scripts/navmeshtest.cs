using UnityEngine;
using UnityEngine.AI;

public class navmeshtest : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}