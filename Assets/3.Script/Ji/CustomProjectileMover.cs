using System.Collections;
using UnityEngine;

public class CustomProjectileMover : MonoBehaviour
{
    public Transform bulletVisual; // ← 자식 탄환 본체 (Rigidbody 있음)

    public float speed = 15f;
    public float hitOffset = 0f;
    public bool UseFirePointRotation;
    public Vector3 rotationOffset = Vector3.zero;

    public GameObject flash;
    public GameObject hit;
    public GameObject[] Detached;

    private Rigidbody rb;
    [SerializeField] private GameObject parent;

    void Start()
    {
        if (bulletVisual == null)
        {
            Debug.LogError("bulletVisual이 설정되지 않았습니다!");
            return;
        }

        rb = bulletVisual.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("bulletVisual에 Rigidbody가 없습니다!");
            return;
        }
        
        if (flash != null)
        {
            flash.SetActive(true);
            flash.transform.position = bulletVisual.position;
            flash.transform.forward = bulletVisual.forward;

            var flashPs = flash.GetComponent<ParticleSystem>();
            if (flashPs != null)
                StartCoroutine(DisableAfterTime(flash, flashPs.main.duration));
            else
                StartCoroutine(DisableAfterTime(flash, flash.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration));
        }

        StartCoroutine(DisableAfterTime(this.gameObject, 5f));
    }

    // void FixedUpdate()
    // {
    //     if (speed != 0 && rb != null)
    //     {
    //         rb.velocity = bulletVisual.forward * speed;
    //     }
    // }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;
        if (parent == GameManager.Instance.CurrentSkillTarget.GameObject)
        {
            Debug.Log("Collision Enter is Me");
            return;
        }
        
        Debug.Log(parent.name);
        Debug.Log(GameManager.Instance.CurrentSkillTarget.GameObject.name);
        Debug.Log("Collision Enter");
        
        rb.constraints = RigidbodyConstraints.FreezeAll;
        speed = 0;

        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point + contact.normal * hitOffset;

        if (hit != null)
        {
            hit.SetActive(true);
            hit.transform.position = pos;

            if (UseFirePointRotation)
                hit.transform.rotation = bulletVisual.rotation * Quaternion.Euler(0, 180f, 0);
            else if (rotationOffset != Vector3.zero)
                hit.transform.rotation = Quaternion.Euler(rotationOffset);
            else
                hit.transform.rotation = Quaternion.LookRotation(contact.normal);

            var hitPs = hit.GetComponent<ParticleSystem>();
            if (hitPs != null)
                StartCoroutine(DisableAfterTime(hit, hitPs.main.duration));
            else
                StartCoroutine(DisableAfterTime(hit, hit.transform.GetChild(0).GetComponent<ParticleSystem>().main.duration));
        }

        foreach (var go in Detached)
        {
            if (go != null)
            {
                go.transform.parent = null;
                go.SetActive(false);
            }
        }

        this.gameObject.SetActive(false);
    }

    IEnumerator DisableAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
            obj.SetActive(false);
    }
}
