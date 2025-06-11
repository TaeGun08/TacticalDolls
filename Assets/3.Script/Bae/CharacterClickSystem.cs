using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClickSystem : MonoBehaviour
{
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private SkillSelectSystem skillUI;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, characterLayer))
            {
                CharacterData characterData = hit.collider.GetComponent<CharacterData>();
                if (characterData != null)
                {
                    MoveRangeSystem.Instance.ResetAllHighlights();
                    SkillRangeSystem.Instance.ResetAllHighlights();
                    
                    MoveRangeSystem.Instance.ShowMoveRange(characterData);
                    skillUI.Open(characterData);
                }
            }
        }
    }
}
