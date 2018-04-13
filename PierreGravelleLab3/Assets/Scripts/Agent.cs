using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Agent : MonoBehaviour
{
    [SerializeField]
    Material MAT_Selected;
    Material MAT_Deselected;

    public Vector3 moveDestination;
    public float attackStoppingDistance = 10f;
    bool moveDestinationChosen = false;
    bool isSelected = false;
    bool isAttacking = false;
    Transform attackTarget;
    RaycastHit targetSightedHit;
    GameObject followCamera;
    int groupNumber = 0;

    float attackStamp = -100f;
    float attackDelay = 1f;
    float health = 100f;
    float sfxVolume;
    public float damageDelt = 15f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public GameObject deathEffect;
    public AudioClip attackSound;
    public AudioClip hitSound;
    AudioSource[] sources;
    AudioSource attackAudio;
    AudioSource hitAudio;
    public AudioClip deathSound;

    List<Transform> myAttackers = new List<Transform>();

    // Use this for initialization
    void Start()
    {
        sources = GetComponents<AudioSource>();

        attackAudio = sources[0];
        hitAudio = sources[1];

        attackAudio.clip = attackSound;
        hitAudio.clip = hitSound;

        if(MAT_Selected == null)
        {
           
            MAT_Selected = GetComponentInChildren<Renderer>().material;
        }
        MAT_Deselected = GetComponentInChildren<Renderer>().material;
        followCamera = gameObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {

            // If we have a SfxVolume key
            if (PlayerPrefs.HasKey("SfxVolume"))
            {
                // Get the volume for our sfx as a float linear
                sfxVolume = DecibelToLinear(PlayerPrefs.GetFloat("SfxVolume"));
            }
            else
            {
                sfxVolume = 1f;
            }


            if (isAttacking && attackTarget != null)
            {
                transform.LookAt(attackTarget);
                attackTarget.GetComponent<EnemyAgent>().ChangeMATAttacked();

                GetComponentInParent<NavMeshAgent>().stoppingDistance = attackStoppingDistance;

                //// Tell the unit controller we're attacking this unit
                //SendMessageUpwards("AttackCommand", attackTarget);

                Physics.Raycast(transform.position, attackTarget.position - transform.position, out targetSightedHit, 10f);

                moveDestination = attackTarget.position;
                if (targetSightedHit.transform != null && targetSightedHit.transform.tag == "Enemy"
                    && Vector3.Distance(transform.position, attackTarget.position) <= attackStoppingDistance + 0.5f)
                {
                    if (Time.time > attackStamp + attackDelay)
                    {

                        attackTarget.GetComponent<EnemyAgent>().TakeDamage(damageDelt, transform);
                        muzzleFlash.Emit(1);
                        attackAudio.Play();
                        attackStamp = Time.time;
                    }
                }
            }
            else if (isAttacking && attackTarget == null)
            {
                isAttacking = false;
            }

            if (moveDestinationChosen || isAttacking)
            {
                GetComponentInParent<NavMeshAgent>().destination = moveDestination;
            }

            if (transform.position == moveDestination)
            {
                moveDestinationChosen = false;
            }


            if (health <= 0)
            {
                foreach (Transform unit in myAttackers)
                {
                    unit.GetComponent<EnemyAgent>().KilledPlayerUnit(transform);
                    SendMessageUpwards("UnitDied", transform);
                }

                AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                Win_Lose_Controller.enemyUnits.Remove(gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void SetMoveDestination(Vector3 selectedDestination)
    {
        if(isSelected == true)
        {
            moveDestination = selectedDestination;
            moveDestinationChosen = true;
            isAttacking = false;
            if(attackTarget != null)
            {
                attackTarget.GetComponent<EnemyAgent>().ChangeMATNotAttacked();
            }

            GetComponentInParent<NavMeshAgent>().stoppingDistance = 0;
        }
    }

    private void AttemptSelection()
    {
        if(GetComponentInParent<UnitController>().AttemptUnitSelection(transform))
        {
            Selected();
        }
    }

    private void Selected()
    {
        if(!isSelected)
        {
            followCamera.SetActive(true);
            isSelected = true;
            GetComponentInChildren<Renderer>().material = MAT_Selected;
        }
    }

    private void Deselected()
    {
        isSelected = false;
        followCamera.SetActive(false);
        if(attackTarget != null)
        {
            attackTarget.GetComponent<EnemyAgent>().ChangeMATNotAttacked();
        }
        GetComponentInChildren<Renderer>().material = MAT_Deselected;
    }

    private void Die()
    {
        SendMessageUpwards("UnitDied", transform);
        Destroy(gameObject);
    }

    private void Attack(Transform target)
    {
        if(attackTarget != null)
        {
            attackTarget.GetComponent<EnemyAgent>().ChangeMATNotAttacked();
        }
        attackTarget = target;
        isAttacking = true;
        moveDestinationChosen = false;
    }

    public void BeingAttacked(Transform attacker)
    {
        myAttackers.Add(attacker);
    }

    public Transform CheckAttackTarget()
    {
        if(isAttacking)
        {
            return attackTarget;
        }
        else
        {
            return null;
        }
    }

    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(float damage, Transform attacker)
    {
        if(!myAttackers.Contains(attacker))
        {
            myAttackers.Add(attacker);
        }
        health -= damage;

        hitEffect.Emit(100);
        hitAudio.PlayDelayed(0.2f);
    }

    public void KilledEnemy(Transform enemy)
    {
        moveDestination = transform.position;
        moveDestinationChosen = true;
        if(myAttackers.Contains(enemy))
        {
            myAttackers.Remove(enemy);
        }

        if(enemy == attackTarget)
        {
            attackTarget = null;
            isAttacking = false;
        }
    }

    public void AddHealth(int amount)
    {
        if(health + amount > 100)
        {
            health = 100;
        }
        else
        {
            health += amount;
        }
    }

    // Converts our volume mixer channel's decibels to a linear value
    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20.0f);

        return linear;
    }

    public int CheckGroup()
    {
        return groupNumber;
    }

    public void SetGroup(int num)
    {
        groupNumber = num;
    }

    public void RemoveFromGroup()
    {
        groupNumber = 0;
    }

    private void OnMouseDown()
    {
        if (Time.timeScale != 0)
        {
            AttemptSelection();
        }
    }
}
