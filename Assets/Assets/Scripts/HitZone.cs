using UnityEngine;

public class HitZone : MonoBehaviour
{
    [SerializeField] private HitType hitType;
    [SerializeField] private SpiderHitReceiver spiderHitReceiver;
    [SerializeField] private ParticleSystem hitParticles;

    private void Awake()
    {
        if (spiderHitReceiver == null)
        {
            spiderHitReceiver = GetComponentInParent<SpiderHitReceiver>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo ha tocado esta hitbox: " + other.name + " | Tag: " + other.tag);

        if (!IsWeapon(other))
        {
            Debug.Log("Ha tocado algo, pero NO es Weapon: " + other.name);
            return;
        }

        if (spiderHitReceiver == null)
        {
            Debug.LogError("No hay SpiderHitReceiver asignado en " + gameObject.name);
            return;
        }

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        Debug.Log("GOLPE VÁLIDO: " + hitType);

        spiderHitReceiver.ReceiveHit(hitType, hitPoint, hitParticles);
    }

    private bool IsWeapon(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            return true;
        }

        if (other.transform.parent != null && other.transform.parent.CompareTag("Weapon"))
        {
            return true;
        }

        return false;
    }
}