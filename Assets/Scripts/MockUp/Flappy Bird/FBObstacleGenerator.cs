using UnityEngine;

public class FBObstacleGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] _obstaclePrefab = null; // 障害物のプレハブ
    [SerializeField] float _interval = 1f; // 生成間隔
    [SerializeField] float _destroyInterval = 5f; // 削除されるまでの時間
    [SerializeField] float _generatorYPosition = 0f; // Y座標の生成地点
    [SerializeField] float _generatorMoveSpeed = 1f; // プレイヤーと一緒に動く速度

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
            // 障害物の生成処理
            int index = Random.Range(0, _obstaclePrefab.Length);
            GameObject gameObject = Instantiate(_obstaclePrefab[index]);

            float currentXPosition = transform.position.x;
            gameObject.transform.position = new Vector2(currentXPosition, _generatorYPosition);
            Destroy(gameObject, _destroyInterval);  // 障害物を一定時間後に削除
        }
        transform.Translate(Vector2.right * _generatorMoveSpeed * Time.deltaTime);
    }
}

