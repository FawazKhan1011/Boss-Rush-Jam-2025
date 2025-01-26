using OpenCover.Framework.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent

public class SmokeChase : MonoBehaviour
{
    public GameObject chaseCollider;
    public GameObject bodycolllider;
    public Animator animator;
    public GameObject eye;
    public GameObject fog;
    public GameObject aura;
    public Transform player;
    public NavMeshAgent agent;

    void Start()
    {
        bodycolllider.SetActive(false);
        chaseCollider.SetActive(true);
        animator.SetTrigger("move_forward_fast");
        eye.SetActive(true);
        fog.SetActive(true);
        aura.SetActive(false);
    }
    void Update()
    {
        agent.destination = player.position;   
    }
}
