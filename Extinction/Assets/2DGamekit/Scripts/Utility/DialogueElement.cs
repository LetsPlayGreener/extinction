using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{

    public class DialogueElement : MonoBehaviour
    {
        public string text;
        public List<DialogueElement> nextDialogElems;
        private List<DialogueElement> initialNextDialogElems;
        public bool speakerIsPlayer = true;
        [HideInInspector]
        public bool selectRandomly = false;
        public int numberSelectedRandomly = 0;
        public bool callEventFunctionsOnSkipDialogue = false;

        public AudioClip audio;

        //This event is called when the dialogue element is displayed
        public DialogueElementReadEvent OnElementRead;

        private DialogueElement tmpDialogueElem;

        private void Awake()
        {
            OnElementRead.elem = this;
            
            if (nextDialogElems != null)
            {
                initialNextDialogElems = new List<DialogueElement>(nextDialogElems);
                SelectNextRandomly(numberSelectedRandomly);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SelectNextRandomly(int number)
        {
            if (number > 0 && number < initialNextDialogElems.Count)
            {
                nextDialogElems = new List<DialogueElement>(initialNextDialogElems);
                for(int i = nextDialogElems.Count; i > number; i--)
                {
                    tmpDialogueElem = nextDialogElems[(int)(Random.Range(0, nextDialogElems.Count - 0.0001f))];
                    nextDialogElems.Remove(tmpDialogueElem);
                }
            }
        }
    }
}
