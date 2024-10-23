using UnityEngine;

public class FBObstacleGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] _obstaclePrefab = null; // ��Q���̃v���n�u
    [SerializeField] float _interval = 1f; // �����Ԋu
    [SerializeField] float _destroyInterval = 5f; // �폜�����܂ł̎���
    [SerializeField] float _generatorYPosition = 0f; // Y���W�̐����n�_
    [SerializeField] float _generatorMoveSpeed = 1f; // �v���C���[�ƈꏏ�ɓ������x

    private float _timer = 0f;
    void Start()
    {
        _timer = _interval;
    }


    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > _interval)
        {
            _timer = 0;
            // ��Q���̐�������
            int index = Random.Range(0, _obstaclePrefab.Length);
            GameObject gameObject = Instantiate(_obstaclePrefab[index]);

            float currentXPosition = transform.position.x;
            gameObject.transform.position = new Vector2(currentXPosition, _generatorYPosition);
            Destroy(gameObject, _destroyInterval);  // ��Q������莞�Ԍ�ɍ폜
        }
        transform.Translate(Vector2.right * _generatorMoveSpeed * Time.deltaTime);
    }
}

