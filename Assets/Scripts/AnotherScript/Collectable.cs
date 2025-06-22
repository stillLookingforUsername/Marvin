using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(CollectableTriggerHandler))]

public class Collectable : MonoBehaviour
{
    [SerializeField] private CollectableSO _collectableSO;
    private void Reset()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    //this method will be called when we collide with the object
    public void Collect(GameObject objectThatTouchTrigger)
    {
        _collectableSO.Collect(objectThatTouchTrigger);
    }

}
