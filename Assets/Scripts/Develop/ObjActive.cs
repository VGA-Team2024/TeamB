using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjActive : MonoBehaviour
{
    [SerializeField] GameObject objActive;
    public void SetActive()
    {
        objActive.SetActive(false);
    }
}
