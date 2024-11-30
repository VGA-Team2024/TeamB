using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamB.Data;

namespace TeamB.Data
{
    public class ResultData
    {
        //防御回数
        public int defense;
        //被弾回数
        public int hit;
        //残り時間
        public int leftoverTime;
        //残り体力
        public int leftoverHp;
        //1次試験ポイント
        public int firsttestP;
        //2次試験ポイント
        public int secondtestP;
        //1次試験合否
        public bool firstpass;
        //1次試験合否
        public bool secondpass;
    }
}

