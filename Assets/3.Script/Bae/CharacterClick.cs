using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterClick : MonoBehaviour
{
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private SkillUI skillUI;

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
                    skillUI.Open(characterData);
                    SkillRangeTester.Instance.ResetAllHighlights();
                }
            }
        }
    }
}
