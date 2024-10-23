using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPlayer : MonoBehaviour
{
    [SerializeField] MarkDataAsset mark_asset;

    [SerializeField] GameObject bullet;

    [SerializeField] Vector3 playertrans;

    Vector3 mousepos;

    public Vector2 angle;

    bool delaytime = true;

    bool reload = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && delaytime)
        {
            Vector3 screen_trans = Input.mousePosition;
            screen_trans.z = 1.0f;
            mousepos = Camera.main.ScreenToWorldPoint(screen_trans);
            LeftMouse().Forget();
        }

        if(Input.GetMouseButtonDown(1) && reload && mark_asset.Remainder_bullet < 30)
        {
            Reload().Forget();
        }
    }

    async UniTaskVoid LeftMouse()
    {
        angle = (Vector2)(mousepos - playertrans);
        Instantiate(bullet, playertrans, Quaternion.identity);
        mark_asset.Remainder_bullet -= 1;
        delaytime = false;

        if (!delaytime)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            delaytime = true;
        }
    }

    async UniTaskVoid Reload()
    {
        mark_asset.Remainder_bullet = mark_asset.Maxbullet;
        reload = false;

        if (!reload)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            reload = true;
        }
    }
}
