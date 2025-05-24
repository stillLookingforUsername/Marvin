using UnityEngine;
using Unity.Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform _respawnPoint;
    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private float _respawnTime;

    private float _respawnTimeStart;

    private CinemachineCamera _cm;
    private bool _respawn;

    private void Start()
    {
        _cm = GameObject.Find("CmCam").GetComponent<CinemachineCamera>();
    }
    private void Update()
    {
        CheckRespawn();
    }

    public void Respawn()
    {
        Debug.Log("it's Called");
        _respawnTimeStart = Time.time;
        _respawn = true;
    }

    private void CheckRespawn()
    {
        if (Time.time >= _respawnTimeStart + _respawnTime && _respawn)
        {
        //    Debug.Log("CheckRespawn is called");
            GameObject playerTemp = Instantiate(_player, _respawnPoint);
            _cm.Follow = playerTemp.transform;
            _respawn = false;
        }
    }

}
