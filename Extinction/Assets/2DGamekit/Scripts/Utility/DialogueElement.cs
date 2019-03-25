using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{

    public class DialogueElement : MonoBehaviour
    {
        public string text;
        public List<DialogueElement> nextDialogElems;
        public bool speakerIsPlayer = true;
        public bool selectRandomly = false;
        public int numberSelectedRandomly = 0;

        public AudioClip audio;

        //This event is called when the dialogue element is displayed
        public DialogueElementReadEvent OnElementRead;

        private void Awake()
        {
            if (numberSelectedRandomly > 0 && numberSelectedRandomly < nextDialogElems.Count)
            {

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
    }
}
