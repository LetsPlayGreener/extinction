using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Gamekit2D {
    public class InteractDialogue : MonoBehaviour
    {
        //Dialogue components
        private DialogueCanvasController dcc;
        private TextMeshProUGUI dialogueText;

        public Image speakerImage;

        //Player components
        private PlayerInput playerInput;
        private Damageable playerDamageable;
        private AudioSource playerAudioSource;

        //text to read
        public DialogueElement firstText;
        private DialogueElement currentText = null;
        private int selectedChoice = -1; //-1 if no choice selected

        //this variable is true if the component is enable and when dcc.DeactivateCanvasWithDelay is called
        //set to false when the component is enabled again
        private bool disabling = false;

        //true if the start of this component has been called at least once
        private bool initialized = false;

        //Event called when this dialogue ends
        public DialogueEndEvent OnEndDialogue;

        private bool simulateValidatePressed = false;

        private List<DialogueElement> tmpElemsList;
        private string tmpString;

        // Start is called before the first frame update
        void Start()
        {
            if (firstText)
            {
                initialized = true;
                //initialize variables
                playerInput = PlayerInput.Instance;
                if (playerInput)
                {
                    playerDamageable = playerInput.GetComponent<Damageable>();
                    playerAudioSource = playerInput.GetComponent<AudioSource>();
                }

                dcc = this.GetComponent<DialogueCanvasController>();
                if (dcc == null)
                    this.enabled = false;
                else
                {
                    //find the text mesh pro component to initialize dialogueText
                    foreach (Transform child in dcc.transform)
                    {
                        bool found = false;
                        foreach (Transform grandChild in child)
                        {
                            if (grandChild.GetComponent<TextMeshProUGUI>())
                            {
                                found = true;
                                dialogueText = grandChild.GetComponent<TextMeshProUGUI>();
                                break;
                            }
                        }
                        if (found)
                            break;
                    }
                }
            }
            else
                gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            //if the player validate
            if ((simulateValidatePressed || Input.GetKeyDown(KeyCode.Space) /*|| Input.GetKeyDown(playerInput.Interact.key) */||
                Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)) && !disabling)
            {
                simulateValidatePressed = false;

                //invoke event for the read text
                if (currentText)
                {
                    currentText.OnElementRead.Invoke(currentText);

                    if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                    {
                        if (currentText.speakerIsPlayer)
                            tmpString = "player";
                        else
                            tmpString = "other";
                        if (currentText.text != "")
                            GBL_Interface.SendStatement("read", "text", "DialogueElement", new Dictionary<string, List<string>>()
                            {
                                { "content", new List<string>() { currentText.text } },
                                { "from", new List<string>() { tmpString } }
                            });
                    }
                }

                //set the selected choice as currentText to prepere the next text
                if (selectedChoice > -1)
                {
                    currentText = currentText.nextDialogElems[selectedChoice];
                    //invoke event for the selected choice
                    currentText.OnElementRead.Invoke(currentText);

                    if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                    {
                        if (currentText.speakerIsPlayer)
                            tmpString = "player";
                        else
                            tmpString = "other";
                        if (currentText.text != "")
                            GBL_Interface.SendStatement("chose", "dialog-tree", "DialogueChoice", new Dictionary<string, List<string>>()
                            {
                                { "content", new List<string>() { currentText.text } }
                            });
                    }
                }

                //if there are texts left to read
                if (currentText.nextDialogElems.Count > 0)
                {
                    if(currentText.nextDialogElems.Count == 1)
                    {
                        if (currentText.nextDialogElems[0])
                        {
                            if (currentText.nextDialogElems[0].text == "")
                                simulateValidatePressed = true;

                            //change text displayed
                            dialogueText.text = currentText.nextDialogElems[0].text;
                            currentText = currentText.nextDialogElems[0];
                            selectedChoice = -1;

                            if(currentText.audio && playerAudioSource)
                                playerAudioSource.PlayOneShot(currentText.audio);

                            speakerImage.gameObject.SetActive(currentText.speakerIsPlayer);
                        }
                        else
                        {
                            //if the next elem is null close dialogue
                            selectedChoice = -1;

                            playerInput.GainControl();
                            playerDamageable.enabled = true;

                            //disable this gameobject
                            disabling = true;
                            currentText = null;
                            OnEndDialogue.Invoke(gameObject);
                            dcc.DeactivateCanvasWithDelay(0);
                        }
                    }
                    else
                    {
                        speakerImage.gameObject.SetActive(true);
                        if (currentText.selectRandomly)
                        {
                            if (tmpElemsList == null)
                                tmpElemsList = new List<DialogueElement>();
                            tmpElemsList.Clear();
                            foreach (DialogueElement elem in currentText.nextDialogElems)
                                if (elem)
                                    tmpElemsList.Add(elem);
                            currentText = tmpElemsList[(int)(UnityEngine.Random.Range(0, tmpElemsList.Count - 0.01f))];
                            simulateValidatePressed = true;
                            selectedChoice = -1;
                        }
                        else
                        {
                            //display choices and wait for the player to choose
                            int initialChoice = -1;
                            int oneNotNull = -1;
                            //set the first choice
                            for (int i = 0; i < currentText.nextDialogElems.Count; i++)
                                if (currentText.nextDialogElems[i] && currentText.nextDialogElems[i].text != "")
                                {
                                    oneNotNull = i;
                                    if (currentText.nextDialogElems[i].text != "")
                                    {
                                        dialogueText.text = string.Concat("<color=\"yellow\">", currentText.nextDialogElems[i].text, "<color=#", ColorUtility.ToHtmlStringRGBA(dialogueText.color), ">");
                                        initialChoice = i;
                                        break;
                                    }
                                }
                            if (initialChoice != -1)
                            {
                                //if there is an initial choice set others choices too
                                for (int i = initialChoice + 1; i < currentText.nextDialogElems.Count; i++)
                                    if (currentText.nextDialogElems[i] && currentText.nextDialogElems[i].text != "")
                                        dialogueText.text = string.Concat(dialogueText.text, System.Environment.NewLine, currentText.nextDialogElems[i].text);
                                selectedChoice = initialChoice;
                            }
                            else
                            {
                                if (oneNotNull != -1)
                                {
                                    selectedChoice = oneNotNull;
                                    simulateValidatePressed = true;
                                }
                                else
                                {
                                    //if all choices are null, close dialogue
                                    selectedChoice = -1;

                                    playerInput.GainControl();
                                    playerDamageable.enabled = true;

                                    //disable this gameobject
                                    disabling = true;
                                    currentText = null;
                                    OnEndDialogue.Invoke(gameObject);
                                    dcc.DeactivateCanvasWithDelay(0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    //if there are no next dialogue element, close dialogue
                    selectedChoice = -1;

                    playerInput.GainControl();
                    playerDamageable.enabled = true;

                    //else disable this gameobject
                    disabling = true;
                    currentText = null;
                    OnEndDialogue.Invoke(gameObject);
                    dcc.DeactivateCanvasWithDelay(0);
                }
            }

            //if "up" input pressed
            if (selectedChoice > -1 && (Input.GetKeyDown(playerInput.Vertical.positive) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                //change the selected choice id stored by checking valid choices above in the list
                int previousChoice = selectedChoice;
                selectedChoice--;
                bool validChoiceFound = false;
                while (selectedChoice != previousChoice)
                {
                    if (selectedChoice < 0)
                        selectedChoice = currentText.nextDialogElems.Count - 1;
                    if (currentText.nextDialogElems[selectedChoice])
                    {
                        validChoiceFound = true;
                        break;
                    }
                    selectedChoice--;
                }

                if (validChoiceFound)
                {
                    //if a new valid choice was found change text to highlight new selected choice
                    int initialChoice = -1;
                    //set the first choice
                    for (int i = 0; i < currentText.nextDialogElems.Count; i++)
                        if (currentText.nextDialogElems[i])
                        {
                            if (i == selectedChoice)
                                dialogueText.text = string.Concat("<color=\"yellow\">", currentText.nextDialogElems[i].text, "<color=#", ColorUtility.ToHtmlStringRGBA(dialogueText.color), ">");
                            else
                                dialogueText.text = currentText.nextDialogElems[i].text;
                            initialChoice = i;
                            break;
                        }
                    //set other choices
                    for (int i = initialChoice + 1; i < currentText.nextDialogElems.Count; i++)
                        if (currentText.nextDialogElems[i])
                        {
                            if(i == selectedChoice)
                                dialogueText.text = string.Concat(dialogueText.text, System.Environment.NewLine, "<color=\"yellow\">", currentText.nextDialogElems[i].text, "<color=#", ColorUtility.ToHtmlStringRGBA(dialogueText.color), ">");
                            else
                                dialogueText.text = string.Concat(dialogueText.text, System.Environment.NewLine, currentText.nextDialogElems[i].text);
                        }
                }
            }
            //if "down" input pressed
            else if (selectedChoice > -1 && (Input.GetKeyDown(playerInput.Vertical.negative) || Input.GetKeyDown(KeyCode.DownArrow)))
            {
                //change the selected choice id stored by checking valid choices under in the list
                int previousChoice = selectedChoice;
                selectedChoice++;
                bool validChoiceFound = false;
                while (selectedChoice != previousChoice)
                {
                    if (selectedChoice > currentText.nextDialogElems.Count - 1)
                        selectedChoice = 0;
                    if (currentText.nextDialogElems[selectedChoice])
                    {
                        validChoiceFound = true;
                        break;
                    }
                    selectedChoice++;
                }

                if (validChoiceFound)
                {
                    //if a new valid choice was found change text to highlight new selected choice
                    int initialChoice = -1;
                    //set the first choice
                    for (int i = 0; i < currentText.nextDialogElems.Count; i++)
                        if (currentText.nextDialogElems[i])
                        {
                            if (i == selectedChoice)
                                dialogueText.text = string.Concat("<color=\"yellow\">", currentText.nextDialogElems[i].text, "<color=#", ColorUtility.ToHtmlStringRGBA(dialogueText.color), ">");
                            else
                                dialogueText.text = currentText.nextDialogElems[i].text;
                            initialChoice = i;
                            break;
                        }
                    //set other choices
                    for (int i = initialChoice + 1; i < currentText.nextDialogElems.Count; i++)
                        if (currentText.nextDialogElems[i])
                        {
                            if (i == selectedChoice)
                                dialogueText.text = string.Concat(dialogueText.text, System.Environment.NewLine, "<color=\"yellow\">", currentText.nextDialogElems[i].text, "<color=#", ColorUtility.ToHtmlStringRGBA(dialogueText.color), ">");
                            else
                                dialogueText.text = string.Concat(dialogueText.text, System.Environment.NewLine, currentText.nextDialogElems[i].text);
                        }
                }
            }

            //close dialogue on press interact button
            if (Input.GetKeyDown(playerInput.Interact.key))
            {
                dcc.DeactivateCanvasWithDelay(0);
                playerInput.GainControl();
            }
        }

        private void OnEnable()
        {
            disabling = false;

            if (!initialized)
                //init this component
                Start();
            playerInput.ReleaseControl();
            playerDamageable.enabled = false;

            if (firstText)
            {
                currentText = firstText;
                if (currentText.audio && playerAudioSource)
                    playerAudioSource.PlayOneShot(currentText.audio);
                speakerImage.gameObject.SetActive(currentText.speakerIsPlayer);

                if (currentText.text == "")
                    simulateValidatePressed = true;
            }
        }

        public void ActivateDialogueWithFirstText()
        {
            if (!initialized)
                //init this component
                Start();

            if (firstText)
            {
                dcc.ActivateCanvasWithText(firstText.text);
                if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                {
                    GBL_Interface.SendStatement("started", "dialog", gameObject.name);
                }
            }
        }
    }

    [Serializable]
    public class DialogueElementReadEvent: UnityEvent<DialogueElement> { }

    [Serializable]
    public class DialogueEndEvent: UnityEvent<GameObject> { }
}
