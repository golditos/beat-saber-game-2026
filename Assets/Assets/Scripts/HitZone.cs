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

        if (spiderHitReceiver == null)
        {
            Debug.LogError("No se ha encontrado SpiderHitReceiver en el padre de " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HITZONE TOCADA por: " + other.name + " | Tag: " + other.tag);

        if (!IsWeapon(other))
        {
            Debug.Log("No es Weapon: " + other.name);
            return;
        }

        if (spiderHitReceiver == null)
        {
            spiderHitReceiver = GetComponentInParent<SpiderHitReceiver>();
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