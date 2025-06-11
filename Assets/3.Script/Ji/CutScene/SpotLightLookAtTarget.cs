using UnityEngine;

[ExecuteAlways]
public class SpotLightLookAtTarget : MonoBehaviour //에디터와 런타임에서 스포트라이트를 캐릭터에 조준시키는 스크립트입니다.
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.position);
        }
    }
    
    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}