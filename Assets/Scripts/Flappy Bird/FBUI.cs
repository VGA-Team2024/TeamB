using UnityEngine;
using UnityEngine.UI;

public class FBUI : MonoBehaviour
{
    [SerializeField] float _timeLimit = 30; // ��������
    [SerializeField] Text _timeLimitText = null; // �������ԕ\���e�L�X�g
    [SerializeField] Text _currentPositionText = null; // ���݈ʒu�̕\���e�L�X�g
    [SerializeField] FBPlayer2 _player;

    void Update()
    {
        _timeLimit -= Time.deltaTime;

        if (_timeLimit < 0)
        {
            _timeLimit = 0;
        }

        _timeLimitText.text = $"�c�莞�� : {_timeLimit.ToString("F0")} �b";
        Vector2 playerpos = _player.transform.position;
        _currentPositionText.text = $"���݈ʒu : {playerpos.x.ToString("F0")}m";
    }
}
