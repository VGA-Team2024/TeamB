using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsBasketPlayer : MonoBehaviour
{
    //移動定数
    private const float cADD_MOVE_X = 10;

    //座標初期化
    public void PositionReset()
    {
        this.transform.localPosition = new Vector3(0, this.transform.localPosition.y);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //移動
        if(Input.GetKey(KeyCode.A))
        {
            this.transform.localPosition
                = new Vector3(this.transform.localPosition.x - cADD_MOVE_X, this.transform.localPosition.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.localPosition
                = new Vector3(this.transform.localPosition.x + cADD_MOVE_X, this.transform.localPosition.y);
        }
    }
    
}
