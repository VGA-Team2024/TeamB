using UnityEngine;
using UnityEngine.UI;

public class FBUI : MonoBehaviour
{
    [SerializeField] float _timeLimit = 30; // 制限時間
    [SerializeField] Text _timeLimitText = null; // 制限時間表示テキスト
    [SerializeField] Text _currentPositionText = null; // 現在位置の表示テキスト
    [SerializeField] FBPlayer2 _player;

    void Update()
    {
        _timeLimit -= Time.deltaTime;

        if (_timeLimit < 0)
        {
            _timeLimit = 0;
        }

        _timeLimitText.text = $"残り時間 : {_timeLimit.ToString("F0")} 秒";
        Vector2 playerpos = _player.transform.position;
        _currentPositionText.text = $"現在位置 : {playerpos.x.ToString("F0")}m";
    }
}
