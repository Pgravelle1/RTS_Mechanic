using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickupScript : MonoBehaviour
{
    public int healAmount = 50;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerUnit")
        {
            other.transform.GetComponent<Agent>().AddHealth(healAmount);
            Destroy(gameObject);
        }
        else if (other.tag == "Enemy")
        {
            other.transform.GetComponent<EnemyAgent>().AddHealth(healAmount);
            Destroy(gameObject);
        }

    }
}
