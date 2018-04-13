using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UnitController : MonoBehaviour
{

    public List<Transform> selectedUnits = new List<Transform>();
    RaycastHit hit;
    int layerMask;
    public RawImage destinationMarker;
    float selectionTimeStamp = -100f;
    float selectionReactionDelay = .1f; // This is here because our unit would get a destination set during our selection
                                        //    because UnitWasSelected() gets called and sets selectUnit
                                        //    which makes it NOT null, which makes the if statement
                                        //    true, which allows SetDestination() to be called via message. 

    List<Transform> F1Group = new List<Transform>();
    List<Transform> F2Group = new List<Transform>();
    List<Transform> F3Group = new List<Transform>();
    List<Transform> F4Group = new List<Transform>();
    List<Transform> F5Group = new List<Transform>();
    List<Transform> F6Group = new List<Transform>();
    List<Transform> F7Group = new List<Transform>();
    List<Transform> F8Group = new List<Transform>();
    List<Transform> F9Group = new List<Transform>();

    // Use this for initialization
    void Start()
    {
        layerMask = 1 << 8;
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if(selectedUnits.Count > 0)
            {
                float dist = Vector3.Distance(selectedUnits[0].transform.position, selectedUnits[0].GetComponent<NavMeshAgent>().destination);
                if (dist > 1.2f)
                {
                    destinationMarker.gameObject.SetActive(true);
                    destinationMarker.transform.position = Camera.main.WorldToScreenPoint(selectedUnits[0].GetComponent<NavMeshAgent>().destination);
                }
                else
                {
                    destinationMarker.gameObject.SetActive(false);
                }
            }
            else
            {
                destinationMarker.gameObject.SetActive(false);
            }

            #region Add To Group
            if (selectedUnits.Count > 0)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1))
                {
                    AddSelectionToGroup(1);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    AddSelectionToGroup(2);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    AddSelectionToGroup(3);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    AddSelectionToGroup(4);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    AddSelectionToGroup(5);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    AddSelectionToGroup(6);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    AddSelectionToGroup(7);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    AddSelectionToGroup(8);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    AddSelectionToGroup(9);
                }
            }
            #endregion

            #region Select A Group
            if(Input.GetKeyDown(KeyCode.F1))
            {
                SelectGroup(1);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                SelectGroup(2);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                SelectGroup(3);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                SelectGroup(4);
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                SelectGroup(5);
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                SelectGroup(6);
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                SelectGroup(7);
            }
            else if (Input.GetKeyDown(KeyCode.F8))
            {
                SelectGroup(8);
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                SelectGroup(9);
            }

            #endregion


            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (Time.time > selectionTimeStamp + selectionReactionDelay)
                {
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 9999f, layerMask))
                    {
                        if (hit.transform.tag == "PlayerUnit")
                        {
                            if (AttemptUnitSelection(hit.transform))
                            {
                                hit.transform.SendMessage("Selected");
                            }
                        }
                        else if (hit.transform.tag != "Enemy")
                        {
                            foreach (Transform unit in selectedUnits)
                            {
                                unit.GetComponent<Agent>().SendMessage("SetMoveDestination", hit.point);
                            }
                        }
                        else
                        {
                            AttackCommand(hit.transform);
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                foreach (Transform unit in selectedUnits)
                {
                    unit.GetComponent<Agent>().SendMessage("Deselected");
                }

                selectedUnits.Clear();
            }
        }
    }

    public bool AttemptUnitSelection(Transform selected)
    {
        bool selectSuccess = false;

        // If it's our first unit or if we're holding the control key
        if(selectedUnits.Count == 0 || Input.GetKey(KeyCode.LeftControl))
        {
            selectedUnits.Add(selected);
            selectionTimeStamp = Time.time;
            selectSuccess = true;
        }
        else
        {
            foreach(Transform unit in selectedUnits)
            {
                unit.GetComponent<Agent>().SendMessage("Deselected");
            }

            selectedUnits.Clear();
            selectedUnits.Add(selected);
            selectionTimeStamp = Time.time;
            selectSuccess = true;
        }

        return selectSuccess;
    }

    void UnitDied(Transform selected)
    {
        if(selectedUnits.Contains(selected))
        {
            selectedUnits.Remove(selected);
        }
    }

    void AttackCommand(Transform target)
    {
        if (target != null)
        {
            foreach (Transform unit in selectedUnits)
            {
                unit.GetComponent<Agent>().SendMessage("Attack", target);
            }

            // Tell the target they're being attacked by these units
            target.SendMessage("BeingAttacked", selectedUnits);
        }
    }

    void AddSelectionToGroup(int num)
    {
        switch (num)
        {
            case 1:
                F1Group.Clear();
                break;
            case 2:
                F2Group.Clear();
                break;
            case 3:
                F3Group.Clear();
                break;
            case 4:
                F4Group.Clear();
                break;
            case 5:
                F5Group.Clear();
                break;
            case 6:
                F6Group.Clear();
                break;
            case 7:
                F7Group.Clear();
                break;
            case 8:
                F8Group.Clear();
                break;
            case 9:
                F9Group.Clear();
                break;
            default:
                break;
        }

        foreach (Transform unit in selectedUnits)
        {
            int unitCurrentGroup = unit.GetComponent<Agent>().CheckGroup();

            // Remove from the group it's currently in
            if (unitCurrentGroup != 0)
            {
                switch (unitCurrentGroup)
                {
                    case 1:
                        F1Group.Remove(unit);
                        break;
                    case 2:
                        F2Group.Remove(unit);
                        break;
                    case 3:
                        F3Group.Remove(unit);
                        break;
                    case 4:
                        F4Group.Remove(unit);
                        break;
                    case 5:
                        F5Group.Remove(unit);
                        break;
                    case 6:
                        F6Group.Remove(unit);
                        break;
                    case 7:
                        F7Group.Remove(unit);
                        break;
                    case 8:
                        F8Group.Remove(unit);
                        break;
                    case 9:
                        F9Group.Remove(unit);
                        break;
                    default:
                        break;
                }
            }

            // Add to the selected group
            switch (num)
            {
                case 1:
                    F1Group.Add(unit);
                    break;
                case 2:
                    F2Group.Add(unit);
                    break;
                case 3:
                    F3Group.Add(unit);
                    break;
                case 4:
                    F4Group.Add(unit);
                    break;
                case 5:
                    F5Group.Add(unit);
                    break;
                case 6:
                    F6Group.Add(unit);
                    break;
                case 7:
                    F7Group.Add(unit);
                    break;
                case 8:
                    F8Group.Add(unit);
                    break;
                case 9:
                    F9Group.Add(unit);
                    break;
                default:
                    break;
            }

            // Tell the unit which group it is now a part of
            unit.GetComponent<Agent>().SetGroup(num);
        }
    }

    void SelectGroup(int num)
    {
        List<Transform> selectedGroup = new List<Transform>();
        foreach(Transform unit in selectedUnits)
        {
            unit.GetComponent<Agent>().SendMessage("Deselected");
        }
        selectedUnits.Clear();

        switch (num)
        {
            case 1:
                selectedGroup = F1Group;
                break;
            case 2:
                selectedGroup = F2Group;
                break;
            case 3:
                selectedGroup = F3Group;
                break;
            case 4:
                selectedGroup = F4Group;
                break;
            case 5:
                selectedGroup = F5Group;
                break;
            case 6:
                selectedGroup = F6Group;
                break;
            case 7:
                selectedGroup = F7Group;
                break;
            case 8:
                selectedGroup = F8Group;
                break;
            case 9:
                selectedGroup = F9Group;
                break;
            default:
                break;
        }

        if(selectedGroup.Count > 0)
        {
            foreach (Transform unit in selectedGroup)
            {
                unit.GetComponent<Agent>().SendMessage("Selected");
                selectedUnits.Add(unit);
            }
        }
    }
}
