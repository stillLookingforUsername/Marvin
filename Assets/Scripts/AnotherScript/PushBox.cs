using UnityEngine;
using DG.Tweening;

public class PushBox : MonoBehaviour
{
    public float pushForce = 10f;
    public Vector2 direction = Vector2.right;
    public float force = 5f;
    public float duration = 1f;
    public float pushTimer = 1f;
    public AreaEffector2D areaEffector;
    private Vector3 originalPosition;
    private ParticleSystem myParticle;
    private AudioSource audioSource;

    private void Start()
    {
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        areaEffector = GetComponent<AreaEffector2D>();
        myParticle = GetComponent<ParticleSystem>();
        if(areaEffector != null)
        {
            areaEffector.enabled = false;
        }
        pushTimer -= Time.deltaTime;
        if(pushTimer <= 0f)
        {
            pushTimer = 1f;
        }
        if(myParticle != null)
        {
            myParticle.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if(obj.gameObject.CompareTag("Player"))
        {
            obj.transform.SetParent(transform);
            Push();
        }
    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        if(obj.gameObject.CompareTag("Player"))
        {
            obj.transform.SetParent(null);
        }
    }

    private void Push()
    {
        Vector3 pushVector = transform.localPosition + (Vector3)direction.normalized * force;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(pushVector, duration).SetEase(Ease.OutCubic));

        seq.AppendCallback(() =>
        {
            if(areaEffector != null)
            {
                myParticle.Play();
                audioSource.Play();
                areaEffector.enabled = true;
            }
        });
        //wait for short time
        seq.AppendInterval(0.3f);
        seq.Append(transform.DOLocalMove(originalPosition, duration).SetEase(Ease.InQuad));

        seq.AppendCallback(() =>
        {
            if(areaEffector != null)
            {
                areaEffector.enabled = false;
                myParticle.Stop();
            }
        });

        //this.gameObject.transform.DOLocalMove(transform.position + pushVector,duration).SetEase(Ease.OutCubic);
    }
}
