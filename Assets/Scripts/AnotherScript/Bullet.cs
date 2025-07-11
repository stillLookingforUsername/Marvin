using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region old 
    /*
    public float speed = 10f;
    public int damage = 1;
    private Rigidbody2D rb;
    public GameObject bulletEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo);
        Triangle triangle = hitInfo.GetComponent<Triangle>();
        if (triangle != null)
        {
            triangle.TakeDamage(damage);
        }
        Instantiate(bulletEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    */
    #endregion

    [Header("General Bullet Stats")]
    [SerializeField] private LayerMask _whatDestroysBullet;
    [SerializeField] private float _destroyTime = 2f;
    //[SerializeField] private LayerMask _whatToDestroy;

    [Header("Normal Bullet Stats")]
    [SerializeField, Range(5f, 20f)] private float _normalBulletSpeed = 15f;
    [SerializeField, Range(1f, 20f)] private float _normalBulletDamage = 1f;
    
    

    [Header("Physics Bullet Stats")]
    [SerializeField, Range(5f, 20f)] private float _physicsBulletSpeed = 5f;
    [SerializeField, Range(0.2f, 10f)] private float _physicsBulletGravity = 1f;
    [SerializeField, Range(1f, 20f)] private float _physicsBulletDamage = 2f;

    private Rigidbody2D _rb;
    private float damage;

    public enum BulletType
    {
        NormalType,
        PhysicsType
    }
    public BulletType bulletType;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        SetDestroyTime();
        //change RB stats based on bullet type
        SetRBStats();

        InitializeBulletStats();
    }
    private void FixedUpdate()
    {
        if (bulletType == BulletType.PhysicsType)
        {
            //rotate bullet in direction of velocity
            transform.right = _rb.linearVelocity;
        }
    }
    private void InitializeBulletStats()
    {
        if (bulletType == BulletType.NormalType)
        {
            SetVelocity();
            damage = _normalBulletDamage;
        }
        else if (bulletType == BulletType.PhysicsType)
        {
            SetPhysicsVelocity();
            damage = _physicsBulletDamage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //is the collision within the whatDestroyBullet layerMask
        if ((_whatDestroysBullet.value & (1 << collision.gameObject.layer)) > 0)
        {
            //spawn particles

            //play sound Fx

            //ScreenShake

            //Damage Enemy  -- refer 123
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                iDamageable.Damage(damage);
            }

            //Destroy GameObject
                Destroy(gameObject);

            //If you want to destroy the object that the bullet hits
            //Destroy(collision.gameObject);
        }
    }
    /*
    private void OnTriggerEnter2D(Collision2D collision)
    {
        if ((_whatToDestroy.value & (1 << collision.gameObject.layer)) > 0)
        {
            Destroy(collision.gameObject);
        }
    }
    */

    private void SetPhysicsVelocity()
    {
        _rb.linearVelocity = transform.right * _physicsBulletSpeed;
    }
    private void SetRBStats()
    {
        if (bulletType == BulletType.NormalType)
        {
            _rb.gravityScale = 0f;
        }
        else if (bulletType == BulletType.PhysicsType)
        {
            _rb.gravityScale = _physicsBulletGravity;
        }
    }
    private void SetVelocity()
    {
        _rb.linearVelocity = transform.right * _normalBulletSpeed;
    }
    private void SetDestroyTime()
    {
        Destroy(gameObject, _destroyTime);
    }

}
