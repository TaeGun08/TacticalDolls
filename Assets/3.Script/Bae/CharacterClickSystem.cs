using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClickSystem : MonoBehaviour
{
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private SkillSelectSystem skillUI;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, unitLayer))
            {
                IDamageAble target= hit.collider.GetComponent<IDamageAble>();
                
                if (target != null)
                {
                    MoveRangeSystem.Instance.ResetAllHighlights();
                    SkillRangeSystem.Instance.ResetAllHighlights();
                
                    MoveRangeSystem.Instance.ShowMoveRange(target);
                    skillUI.Open(target);
                }
            }
        }
    }
}
