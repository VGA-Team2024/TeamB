using UnityEngine;

public class FBPlayer2 : MonoBehaviour
{
    [SerializeField] float _jumpPower = 1f; // ジャンプ力
    [SerializeField] float _moveSpeed = 1f; // 移動速度
    [SerializeField] Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = new Vector2(_moveSpeed, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _rb.velocity = new Vector2(_moveSpeed, _jumpPower);
        }

        // 移動範囲の制限
        if (transform.position.y > 4.5f)
        {
            transform.position = new Vector2(transform.position.x, 4.5f);
        }
        else if (transform.position.y < -4.5f)
        {
            transform.position = new Vector2(transform.position.x, -4.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Debug.Log("障害物に当たった");
        }
    }
}
