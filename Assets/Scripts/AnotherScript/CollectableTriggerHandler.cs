using UnityEngine;

public class CollectableTriggerHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _whoCanCollect = LayerMaskHelper.CreateLayerMask(10);

    private Collectable _collectable;

    private void Awake()
    {
        _collectable = GetComponent<Collectable>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMaskHelper.ObjectIsInLayerMask(other.gameObject, _whoCanCollect))
        {
            //collect
            _collectable.Collect(other.gameObject);


            Destroy(gameObject);
        }
    }
}
