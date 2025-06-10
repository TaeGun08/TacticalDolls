using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClickSystem : MonoBehaviour
{
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private SkillSelectSystem skillSelectSystem;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, characterLayer))
            {
                CharacterData characterData = hit.collider.GetComponent<CharacterData>();
                if (characterData != null)
                {
                    SkillRangeSystem.Instance.ResetAllHighlights();
                    MoveRangeSystem.Instance.ShowMoveRange(characterData);
                    skillSelectSystem.Open(characterData);
                }
            }
        }
    }
}
