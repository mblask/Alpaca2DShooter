using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletSpeed;
    public float DestroyAfter;
    public LayerMask EnemyLayerMask;

    private Rigidbody2D _rigidbody;
    private Vector2 _bulletDirection;
    private float _stopwatch;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Destroy(gameObject, DestroyAfter);
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position + _bulletDirection.normalized * BulletSpeed * Time.fixedDeltaTime);
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction == null)
            return;

        _bulletDirection = direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == Mathf.Log(EnemyLayerMask.value, 2.0f))
        {
            Debug.Log("Enemy hit");
            GamePlayCanvas.Instance.IncrementEnemyKillsText();
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == ConstsEnums.ObstacleTag)
        {
            Destroy(gameObject);
        }
    }
}
