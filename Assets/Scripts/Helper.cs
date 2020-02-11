using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [SerializeField] private GameObject blastRadius;
    private GameObject blastParent;
 
    private static Helper instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public static Helper GetInstance()
    {
        if (instance.blastParent == null)
        {
            instance.blastParent = new GameObject("BlastParent");
        }
        return instance;
    }

    public void ClearHelper()
    {
        if (instance.blastParent == null)
        {
            Destroy(instance.blastParent);
        } 
    }

    public void ShowBlast(Vector3 pos)
    {
        var blast = Instantiate(blastRadius, pos, Quaternion.identity);
        blast.transform.parent = blastParent.transform;
        blast.GetComponent<Renderer>().material.color = new Color(Color.grey.r, Color.grey.g, Color.grey.b, 0.2f);
    }
}
