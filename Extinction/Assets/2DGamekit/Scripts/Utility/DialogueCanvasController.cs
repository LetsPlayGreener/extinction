using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit2D
{
    public class DialogueCanvasController : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI textMeshProUGUI;
        public Image dialogueImage;

        protected Coroutine m_DeactivationCoroutine;
    
        protected readonly int m_HashActivePara = Animator.StringToHash ("Active");

        IEnumerator SetAnimatorParameterWithDelay (float delay)
        {
            yield return new WaitForSeconds (delay);
            animator.SetBool(m_HashActivePara, false);

            //disable gameobject at the end of the animation
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(animator.runtimeAnimatorController.name))
                yield return new WaitForSeconds(0.1f);
            gameObject.SetActive(false);
        }

        public void ActivateCanvasWithText (string text)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine (m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive (true);
            animator.SetBool (m_HashActivePara, true);
            textMeshProUGUI.text = text;
            textMeshProUGUI.parseCtrlCharacters = false;
            textMeshProUGUI.parseCtrlCharacters = true;

            if(LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                GBL_Interface.SendStatement("read", "text", "Popup", new Dictionary<string, List<string>>()
                    {
                        { "content", new List<string>() { text } }
                    });
        }

        public void ActivateCanvasWithTranslatedText (string phraseKey)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive(true);
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = Translator.Instance[phraseKey];
        }

        public void DeactivateCanvasWithDelay (float delay)
        {
            m_DeactivationCoroutine = StartCoroutine (SetAnimatorParameterWithDelay (delay));
        }

        public void ChangeSprite(Sprite sprite)
        {
            if (sprite)
            {
                animator.speed = 0;
                dialogueImage.sprite = sprite;
            }
            else
                animator.speed = 1;
        }
    }
}
