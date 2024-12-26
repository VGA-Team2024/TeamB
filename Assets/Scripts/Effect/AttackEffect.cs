using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] GameObject maineffect;
    [SerializeField] List<GameObject> trackingeffect = new List<GameObject>();
    [SerializeField] Vector3 speed;
    [SerializeField] float survivalTime;
    private float duration = 0f;
    private float timer;

    public event Action AtDestroy;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Attackeffect());
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            return;
        }
        maineffect.transform.position += speed *Time.deltaTime;

        for(int i = 0; i < trackingeffect.Count; i++)
        {
            trackingeffect[i].transform.position = maineffect.transform.position;
        }
        Destroy(this.gameObject, survivalTime);
    }

    IEnumerator Attackeffect()
    {
        maineffect.SetActive(true);

        while (true)
        {
            trackingeffect[0].SetActive(true);
            yield return new WaitForSeconds(0.15f);
            trackingeffect[0].SetActive(false);
            trackingeffect[1].SetActive(true);
            yield return new WaitForSeconds(0.15f);
            trackingeffect[1].SetActive(false);
            trackingeffect[2].SetActive(true);
            yield return new WaitForSeconds(0.15f);
            trackingeffect[2].SetActive(false);
        }

    }

    private void OnDestroy()
    {
        AtDestroy?.Invoke();
    }
}
