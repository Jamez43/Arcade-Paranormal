using System.Collections.Generic;
using UnityEngine;

public class XPController : MonoBehaviour
{
    public List<GameObject> disabledXP = new List<GameObject>();

    private void Start()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("XP"))
            {
                disabledXP.Add(obj);
            }
        }
    }
}
