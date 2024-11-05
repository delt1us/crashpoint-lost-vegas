/**************************************************************************************************************
* Oil Slip
* Used in the oil slip prefab to reduce the friction of all the tires of the vehicles that collide with it.
*
* Created by Dean Atkinson-Walker 2023
* 
***************************************************************************************************************/

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OilSlip : NetworkBehaviour
{
    private readonly List<GameObject> objsInOil = new();

    [SerializeField, Tooltip("The layers that the oil is allowed to sit on")] 
    private LayerMask layers;

    [SerializeField] private float lifeSpan = 9;
    private float lifeTime;

    private readonly Vector3 upOffset = new(0, 3.6f, 0);

    private void Start()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100, layers);

        transform.position = hit.point + upOffset;
    }

    private void Update()
    {
        if (!IsServer) return;
        lifeTime += Time.deltaTime;
        if (lifeTime > lifeSpan) GetComponent<NetworkObject>().Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<MovementController>())
        {
            MovementController controller = other.GetComponentInParent<MovementController>();
            objsInOil.Add(other.gameObject);
            controller.InOil(true);
        }
    }

    private void OuttaOil(GameObject other)
    {
        if (other.GetComponentInParent<MovementController>())
        {
            MovementController controller = other.GetComponentInParent<MovementController>();
            objsInOil.Remove(other);
            controller.InOil(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OuttaOil(other.gameObject);
    }

    public override void OnDestroy()
    {
        for(int i = 0; i < objsInOil.Count; i++) if (objsInOil[i]) OuttaOil(objsInOil[i]);

        base.OnDestroy();
    }
}
