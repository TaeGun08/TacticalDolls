using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Dark
{
    public class ScrollForMore : MonoBehaviour
    {
        [Header("Resources")]
        public Animator objectAnimator;

        [Header("Settings")]
        public float fadeOutValue;
        public bool invertValue = false;

        void Start()
        {
            CheckValue();
        }

        public void CheckValue()
        {
            if (invertValue == false)
            {
                if (objectAnimator != null)
                    objectAnimator.Play("SFM In");

                else if (objectAnimator != null)
                    objectAnimator.Play("SFM Out");
            }
            
            else
            {
                if (objectAnimator != null)
                    objectAnimator.Play("SFM In");

                else if (objectAnimator != null)
                    objectAnimator.Play("SFM Out");
            }
        }
    }
}
