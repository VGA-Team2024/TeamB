using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CatchFalling : MonoBehaviour
{
    [SerializeField] private StartCount _startCount;
    [SerializeField] private CatchFallingScore _score;
    [SerializeField] private CatchFallingItem _item;
    [SerializeField] private RectTransform _content;
    [SerializeField] private int _maxCount;
    [SerializeField] private float _makeInterval;
    [SerializeField] private int _clearCount;

    private int getCount = 0;
    private int deleteCount = 0;
    


    // Start is called before the first frame update
    void Start()
    {
        InGameExec().Forget();
    }

    private async UniTask InGameExec()
    {
            _startCount.SetTextEnable(true);
            _startCount.Setting(3);
            await UniTask.Delay(500);
            _startCount.Setting(2);
            await UniTask.Delay(500);
            _startCount.Setting(1);
            await UniTask.Delay(500);
            _startCount.SettingEnd();
            await UniTask.Delay(500);
            _startCount.SettingEnabled();
            
            _score.SetTextEnable(true);
            _score.Setting(getCount);

            await UniTask.Delay(500);
        


            for(int i = 0; i < _maxCount; i++)
            {
                var clone = Instantiate(_item, _content);
                float posX = Random.Range(_content.offsetMin.x,_content.offsetMax.x);
                clone.transform.localPosition = new Vector3(posX, clone.transform.localPosition.y);
                clone.Setting(getaction:() =>
                {
                    getCount++;
                    _score.Setting(getCount);
                    CheckClear();
                }, deleteaction:() =>
                {
                    deleteCount++;
                    CheckClear();
                }) ;

                await UniTask.Delay((int)(_makeInterval*500));
            }
        
    }

    private void CheckClear()
    {
        if(getCount >= _clearCount)
        {
            
        }

        if (getCount + deleteCount == _maxCount)
        {
            
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
