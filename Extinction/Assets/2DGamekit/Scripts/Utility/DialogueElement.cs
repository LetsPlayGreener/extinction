using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{

    public class DialogueElement : MonoBehaviour
    {
        public string text;
        [TextArea]
        public string english;
        [TextArea]
        public string french;
        [TextArea]
        public string italian;
        [TextArea]
        public string romanian;
        public List<DialogueElement> nextDialogElems;
        private List<DialogueElement> initialNextDialogElems;
        public bool speakerIsPlayer = true;
        [HideInInspector]
        public bool selectRandomly = false;
        public int numberSelectedRandomly = 0;
        //this list of elem will be added to nextDialogElems after the random choice
        public List<DialogueElement> addAfterRandom;
        public bool callEventFunctionsOnSkipDialogue = false;

        public AudioClip audio;
        public AudioClip englishAudio;
        public AudioClip frenchAudio;
        public AudioClip italianAudio;
        public AudioClip romanianAudio;

        //This event is called when the dialogue element is displayed
        public DialogueElementReadEvent OnElementRead;

        private DialogueElement tmpDialogueElem;

        private void Awake()
        {
            OnElementRead.elem = this;

            //if (DataManager.playerData == null)
            //    DataManager.LoadData();
            //switch (DataManager.playerData.PlayerLanguage)
            //{
            //    case PlayerData.Language.french:
            //        text = french;
            //        audio = frenchAudio;
            //        break;

            //    case PlayerData.Language.italian:
            //        text = italian;
            //        audio = italianAudio;
            //        break;

            //    case PlayerData.Language.romanian:
            //        text = romanian;
            //        audio = romanianAudio;
            //        break;

            //    default:
            //        text = english;
            //        audio = englishAudio;
            //        break;
            //}
        }

        private void OnEnable()
        {
            InitRandom();
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

                if (addAfterRandom != null)
                    nextDialogElems.AddRange(addAfterRandom);
            }
        }

        public void InitRandom()
        {
            if (nextDialogElems != null)
            {
                initialNextDialogElems = new List<DialogueElement>(nextDialogElems);
                SelectNextRandomly(numberSelectedRandomly);
            }
        }
    }
}
