using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    [SerializeField] List<string> markTag;
    [SerializeField] List<int> markscore;
    [SerializeField] float speed;
    Vector2 bulletangle;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] MarkDataAsset mark_asset;
    [SerializeField] MarkManager mark_mgr;
    [SerializeField] MarkPlayer mark_player;
  
    void Awake()
    {
        mark_mgr = FindAnyObjectByType<MarkManager>();
        mark_player = FindAnyObjectByType<MarkPlayer>();
    }

    void Start()
    {
        bulletangle = mark_player.angle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = bulletangle * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < markTag.Count; i++)
        {
            if (other.gameObject.CompareTag(markTag[i]))
            {
                mark_asset.Score += markscore[i];
                mark_mgr.des_mark = true;
                Destroy(this.gameObject);
            }
        }
    }
}
