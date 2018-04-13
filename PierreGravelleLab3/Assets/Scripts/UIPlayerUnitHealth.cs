using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerUnitHealth : MonoBehaviour
{
    int layerMask = 1 << 8;
    bool enemySelected = false;
    Agent selectedUnit;
    Text myText;
    public UnitController unitController;

    // Use this for initialization
    void Start()
    {
        layerMask = ~layerMask;
        myText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemySelected && selectedUnit != null)
        {
            myText.text = string.Format("Most Recent Selected Unit Health: {0}", selectedUnit.GetHealth());
        }
        else
        {
            myText.text = "";
        }

        if (Input.GetMouseButtonDown(0) || unitController.selectedUnits.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                if(hit.transform.tag == "PlayerUnit")
                {
                    enemySelected = true;
                    selectedUnit = hit.transform.GetComponent<Agent>();
                }
            }

            if(unitController.selectedUnits.Count > 0)
            {
                enemySelected = true;
                selectedUnit = unitController.selectedUnits[unitController.selectedUnits.Count-1].GetComponent<Agent>();
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            selectedUnit = null;
            enemySelected = false;
        }

        if(selectedUnit != null)
        {
            if(selectedUnit.GetHealth() <= 0)
            {
                enemySelected = false;
                selectedUnit = null;
            }
        }

    }
}
