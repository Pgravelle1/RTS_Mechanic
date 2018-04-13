using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealth : MonoBehaviour
{
    int layerMask = 1 << 8;
    bool enemySelected = false;
    EnemyAgent selectedEnemy;
    Text myText;

    // Use this for initialization
    void Start()
    {
        layerMask = ~layerMask;
        myText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySelected && selectedEnemy != null)
        {
            myText.text = string.Format("Most Recent Selected Enemy Health: {0}", selectedEnemy.GetHealth());
        }
        else
        {
            myText.text = "";
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                if(hit.transform.tag == "Enemy")
                {
                    enemySelected = true;
                    selectedEnemy = hit.transform.GetComponent<EnemyAgent>();
                }
                else
                {
                    enemySelected = false;
                    selectedEnemy = null;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            selectedEnemy = null;
            enemySelected = false;
        }

        if(selectedEnemy != null)
        {
            if(selectedEnemy.GetHealth() <= 0)
            {
                enemySelected = false;
                selectedEnemy = null;
            }
        }

    }
}
