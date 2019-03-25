using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Gamekit2D
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class CollectionManager : MonoBehaviour
    {
        public static CollectionManager instance;

        //list to define CollectableType possible values
        public string[] listZones;
        public string[] listClassifications;
        public string[] listFeatures;
        public string[] listSpecies;

        //variables used for raycast
        Collider2D[] colliders;
        public GameObject player;
        public Transform debugRedDot;
        private Transform raycastedPosition;
        private Collectable raycastedCollectable = null;

        //the key correspond to the id of the type of collectable
        public Dictionary<string, List<Collectable>> collectables = new Dictionary<string, List<Collectable>>();
        public Dictionary<string, CollectableType> types = new Dictionary<string, CollectableType>();
        private List<string> collected = new List<string>();
        private Collectable lastCollected = null;
        private int playerCurrentZone;

        private PlayerInput playerInput;
        private CharacterController2D playerController;

        [HideInInspector]
        public List<CheckRehabilitation> rehabCheckers = new List<CheckRehabilitation>();
        
        public List<CollectableUIInfo> collectableUIBG;

        //events
        public GenerationEvent OnGeneration;
        public GenerationEvent OnGeneratedWrongType;
        public GenerationEvent OnGeneratedInWrongZone;

        private Collectable tmpCollectable;
        private SpriteRenderer tmpSpriteRenderer;

        #region Getter/Setter
        public int PlayerCurrentZone
        {
            set
            {
                playerCurrentZone = value;
            }
        }

        public Transform RaycastedPosition
        {
            get
            {
                return raycastedPosition;
            }
        }

        public Collectable LastCollected
        {
            get
            {
                return lastCollected;
            }
        }
        #endregion

        private void Awake()
        {
            if (!instance)
                instance = this;
        }

        private void OnEnable()
        {
            if (!instance)
                instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Application.isPlaying)
            {
                foreach (Transform child in player.transform)
                    if (child.gameObject.name == "CameraFollowTarget")
                    {
                        raycastedPosition = child;
                        break;
                    }
                playerInput = PlayerInput.Instance;
                playerController = player.GetComponent<CharacterController2D>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isPlaying)
            {
                if (debugRedDot)
                    debugRedDot.position = raycastedPosition.position + Vector3.up;

                #region Raycast
                colliders = Physics2D.OverlapBoxAll(new Vector2(raycastedPosition.position.x, raycastedPosition.position.y + 1), new Vector2(0.2f, 2), 0);
                bool noRaycast = true;
                for (int i = 0; i < colliders.Length; i++)
                {
                    tmpCollectable = colliders[i].gameObject.GetComponent<Collectable>();
                    if (tmpCollectable && tmpCollectable.Type && tmpCollectable.Type.collectableByPlayer && tmpCollectable.collectableByPlayer)
                    {
                        noRaycast = false;
                        if (raycastedCollectable != colliders[i].gameObject.GetComponent<Collectable>())
                        {
                            if (raycastedCollectable)
                                raycastedCollectable.GetComponent<SpriteRenderer>().color = Color.white;
                            raycastedCollectable = colliders[i].gameObject.GetComponent<Collectable>();
                            raycastedCollectable.GetComponent<SpriteRenderer>().color = Color.green;
                        }
                        break;
                    }
                }
                if (noRaycast)
                {
                    if (raycastedCollectable)
                        raycastedCollectable.GetComponent<SpriteRenderer>().color = Color.white;
                    raycastedCollectable = null;
                }
                #endregion

                if (playerInput.HaveControl)
                {
                    //On interact key pressed collect the object
                    if (Input.GetKeyDown(playerInput.Interact.key))
                    {
                        if (raycastedCollectable)
                        {
                            lastCollected = raycastedCollectable;
                            lastCollected.Type.OnInteract.Invoke(lastCollected);
                            lastCollected.OnInteract.Invoke(lastCollected);
                            if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                                GBL_Interface.SendStatement("interacted", "interactable", lastCollected.gameObject.name, new Dictionary<string, List<string>>()
                                {
                                    { "type", new List<string>() { lastCollected.Type.ToString(), lastCollected.Type.Id } },
                                    { "position", new List<string>() { lastCollected.transform.position.ToString() } }
                                });

                            bool hasTypeInUI = false;
                            foreach(CollectableUIInfo uiSlot in collectableUIBG)
                            {
                                if(uiSlot.gameObject.activeSelf && lastCollected.Type.Id == uiSlot.type.Id)
                                {
                                    hasTypeInUI = true;
                                    break;
                                }
                            }

                            if (!hasTypeInUI)
                                lastCollected.Type.OnInteractWithNoUI.Invoke(lastCollected);
                            else if (CheckIfIDIsCollected(lastCollected.Type.Id))
                                Collect(lastCollected);
                        }
                    }
                    if (Input.anyKeyDown && rehabCheckers.Count > 0 && playerController.IsGrounded)
                    {
                        for (int i = 1; i < 10; i++)
                        {
                            if (Input.GetKeyDown(i.ToString()))
                            {
                                if (i - 1 < collectableUIBG.Count)
                                {
                                    //check if type matches type of the checker before generating
                                    bool matches = false;
                                    foreach (CheckRehabilitation cr in rehabCheckers)
                                        if (cr.CheckType(collectableUIBG[i - 1].type))
                                        {
                                            matches = true;
                                            break;
                                        }

                                    if (matches)
                                    {
                                        //check if player is in the correct zone
                                        if (playerCurrentZone == collectableUIBG[i - 1].type.zone)
                                        {
                                            OnGeneration.Invoke(collectableUIBG[i - 1].type);

                                            //generate collectableUIBG[i - 1].type
                                            GameObject go = new GameObject();
                                            go.transform.position = raycastedPosition.position + Vector3.up;
                                            go.AddComponent<SpriteRenderer>().sprite = collectableUIBG[i - 1].type.sprite;
                                            go.transform.localScale = collectableUIBG[i - 1].type.Scale;
                                            tmpCollectable = go.AddComponent<Collectable>();
                                            tmpCollectable.Type = collectableUIBG[i - 1].type;
                                            go.AddComponent<BoxCollider2D>().isTrigger = true;

                                            tmpCollectable.Type.OnGenerated.Invoke(tmpCollectable);
                                        }
                                        else
                                            OnGeneratedInWrongZone.Invoke(collectableUIBG[i - 1].type);
                                    }
                                    else
                                        OnGeneratedWrongType.Invoke(collectableUIBG[i - 1].type);
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Collect(Collectable collectable)
        {
            if (collectable && !collected.Contains(collectable.Type.Id))
            {
                collectable.Type.OnFirstCollection.Invoke(collectable);
                tmpSpriteRenderer = collectable.GetComponent<SpriteRenderer>();
                if (tmpSpriteRenderer)
                {
                    collected.Add(collectable.Type.Id);
                    collectable.Type.Scale = collectable.transform.localScale;
                    collectable.Type.Height = collectable.transform.position.y - raycastedPosition.position.y;
                    collectable.Type.SetAsCollectedAtLeastOnce();

                    if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                        GBL_Interface.SendStatement("collected", "collectable", collectable.gameObject.name, new Dictionary<string, List<string>>()
                        {
                            { "type", new List<string>() { collectable.Type.ToString(), collectable.Type.Id } },
                            { "position", new List<string>() { collectable.transform.position.ToString() } }
                        });

                    //for(int i = 0; i < collectableUI.Count; i++)
                    //{
                    //    if (!collectableUI[i].activeSelf)
                    //    {
                    //        collectableUI[i].SetActive(true);
                    //        collectableUI[i].GetComponent<Image>().sprite = tmpSpriteRenderer.sprite;
                    //        break;
                    //    }
                    //}
                }
            }

            if(collectable && collectable.Type)
            {
                collectable.Type.OnCollected.Invoke(collectable);
                collectable.OnCollected.Invoke(collectable);
            }

            //animation

            //add the sprite in the UI and its id
        }

        public void CheckCollected(int slot)
        {
            if (slot < collectableUIBG.Count && collectableUIBG[slot-1].gameObject.activeSelf && lastCollected.Type.Id == collectableUIBG[slot-1].type.Id)
            {
                Collect(lastCollected);
            }
            else
            {
                lastCollected.Type.OnFailCollection.Invoke(lastCollected);
                lastCollected.OnFailCollection.Invoke(lastCollected);
                if (LearningAnalyticsGenerator.instance && LearningAnalyticsGenerator.instance.canGenerateLA)
                    GBL_Interface.SendStatementWithResult("attempted", "collectable", lastCollected.gameObject.name, new Dictionary<string, List<string>>()
                    {
                        { "type", new List<string>() { lastCollected.Type.ToString(), lastCollected.Type.Id } },
                        { "position", new List<string>() { lastCollected.transform.position.ToString() } }
                    },
                    null, null, false, slot < collectableUIBG.Count && collectableUIBG[slot - 1].gameObject.activeSelf ? collectableUIBG[slot - 1].type.ToString() : "");
            }
        }

        public bool CheckIfIDIsCollected(string id)
        {
            return collected.Contains(id);
        }
    }

    [Serializable]
    public class CollectableEvent : UnityEvent<Collectable> { }

    [Serializable]
    public class GenerationEvent : UnityEvent<CollectableType> { }
}
