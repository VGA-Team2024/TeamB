using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamB
{
    public class FrameMove : MonoBehaviour
    {
        private const float cADD_MOVE_X = 10;
        private const float cADD_MOVE_Y = 10;

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
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.localPosition
                    = new Vector3(this.transform.localPosition.x - cADD_MOVE_X, this.transform.localPosition.y);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.localPosition
                    = new Vector3(this.transform.localPosition.x + cADD_MOVE_X, this.transform.localPosition.y);
            }
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.localPosition
                    = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + cADD_MOVE_Y);
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.localPosition
                    = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - cADD_MOVE_Y);
            }


        }

    }
}
