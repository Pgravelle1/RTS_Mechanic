using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class EnemyAgents : MonoBehaviour
{
    [SerializeField]
    Material MAT_NotAttacked;

    [SerializeField]
    Material MAT_Attacked;


    public Transform patrolPath;
    public float attackStoppingDistance = 10f;

    List<Transform> patrolPoints = new List<Transform>();
    public Vector3 currentDestination;
    List<Transform> myAttackers = new List<Transform>();
    Transform target;
    RaycastHit attackerSightedHit;
    int patrolIndex = 0;
    bool wasAttacked = false;
    bool focusedOnUnit = false;
    float health = 50;
    float sfxVolume;
    public float damageDelt = 15f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;

    public GameObject deathEffect;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    AudioSource[] sources;
    AudioSource attackAudio;
    AudioSource hitAudio;

    float attackStamp = -100f;
    float attackDelay = 1f;

    float hitDelay = 1f;
    float hitStamp = -100f;


    // Use this for initialization
    void Start()
    {
        sources = GetComponents<AudioSource>();

        attackAudio = sources[0];
        hitAudio = sources[1];

        attackAudio.clip = attackSound;
        hitAudio.clip = hitSound;

        for (int i = 0; i < patrolPath.childCount; i++)
        {
            patrolPoints.Add(patrolPath.GetChild(i));
        }

        currentDestination = new Vector3(patrolPoints[patrolIndex].position.x, 0, patrolPoints[patrolIndex].position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {

            if (PlayerPrefs.HasKey("SfxVolume"))
        {
            // Get the volume for our sfx as a float linear
            sfxVolume = DecibelToLinear(PlayerPrefs.GetFloat("SfxVolume"));
        }
        else
        {
            sfxVolume = 1f;
        }

            if (!wasAttacked)
            {
                GetComponent<NavMeshAgent>().stoppingDistance = 0;
                Patrol();
            }
            else if (wasAttacked)
            {

                if (target != null)
                {
                    transform.LookAt(target);
                    currentDestination = target.position;

                    // Check if we can see our target
                    Physics.Raycast(transform.position, target.position - transform.position, out attackerSightedHit);

                    if (target.transform != null && attackerSightedHit.transform.tag == "PlayerUnit"
                        && Vector3.Distance(transform.position, target.position) <= attackStoppingDistance + 0.5f)
                    {
                        if (Time.time > attackStamp + attackDelay)
                        {
                            target.GetComponent<Agent>().TakeDamage(damageDelt, transform);
                            muzzleFlash.Emit(1);
                            attackAudio.Play();
                            attackStamp = Time.time;
                        }
                    }
                }
                else
                {
                    Patrol();
                }

                foreach (Transform unit in myAttackers)
                {
                    // Check if we can see the unit attacking us
                    Physics.Raycast(transform.position, unit.position - transform.position, out attackerSightedHit);

                    // If we're not already focusing on a PlayerUnit and we see a PlayerUnit
                    if (!focusedOnUnit && attackerSightedHit.transform != null && attackerSightedHit.transform.tag == "PlayerUnit")
                    {
                        // Set this unit as your target
                        target = unit;
                        focusedOnUnit = true;
                        GetComponent<NavMeshAgent>().stoppingDistance = attackStoppingDistance;
                    }
                }

                GetComponent<NavMeshAgent>().destination = currentDestination;
            }

            if (health <= 0)
            {
                foreach (Transform unit in myAttackers)
                {
                    unit.GetComponent<Agent>().KilledEnemy(transform);
                }

                AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Win_Lose_Controller.enemyUnits.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void SetDestination(Vector3 selectedDestination)
    {
        currentDestination = new Vector3(selectedDestination.x, 0, selectedDestination.z);
    }

    private void BeingAttacked(List<Transform> attackers)
    {
        RaycastHit hit;
        foreach(Transform attacker in attackers)
        {
            // If we can see this attacker and this attacker is not in our myAttackers list
            if (Physics.Raycast(transform.position, attacker.transform.position - transform.position, out hit) &&
                hit.transform == attacker.transform &&
                !myAttackers.Contains(attacker))
            {
                // Then we know he was attacking us
                myAttackers.Add(attacker);
            }
        }

        wasAttacked = true;

        GetComponent<Renderer>().material = MAT_Attacked;
    }

    public void ChangeMATNotAttacked()
    {
        GetComponent<Renderer>().material = MAT_NotAttacked;
    }

    public void ChangeMATAttacked()
    {
        GetComponent<Renderer>().material = MAT_Attacked;
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        if (!myAttackers.Contains(attacker))
        {
            myAttackers.Add(attacker);
        }
        health -= damage;

        hitEffect.Emit(100);
        hitAudio.PlayDelayed(0.2f);
    }

    public void KilledPlayerUnit(Transform unit)
    {
        if(myAttackers.Contains(unit))
        {
            myAttackers.Remove(unit);

            if(target.transform == unit)
            {
                // The unit we are focusing on died
                focusedOnUnit = false;
                target = null;
            }

            if(myAttackers.Count == 0)
            {
                wasAttacked = false;
            }
        }

    }

    private void Patrol()
    {
        currentDestination = new Vector3(patrolPoints[patrolIndex].position.x, 0, patrolPoints[patrolIndex].position.z);
        #warning SHOULD USE DISTANCE INSTEAD
        if (new Vector3(transform.position.x, 0, transform.position.z) == currentDestination)
        {
            GetComponent<NavMeshAgent>().isStopped = true;

            patrolIndex = patrolIndex != 5 ? patrolIndex + 1 : 0;

            GetComponent<NavMeshAgent>().isStopped = false;
        }

        GetComponent<NavMeshAgent>().destination = currentDestination;
    }

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }

    public float GetHealth()
    {
        return health;
    }

    private void OnMouseDown()
    {
        if (Time.timeScale != 0)
        {
            SendMessageUpwards("AttackCommand", transform);
        }
    }

}
