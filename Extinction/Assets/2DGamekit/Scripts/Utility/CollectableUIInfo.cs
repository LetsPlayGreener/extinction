using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit2D
{
    public class CollectableUIInfo : Displayable
    {
        public static List<CollectableType> usedTypes = new List<CollectableType>();

        public enum SelectByUnique: int
        {
            Species = 0,
            Feature = 1,
            Classification = 2,
            Zone = 3,
            AlreadyHasType = -1
        }

        public SelectByUnique selectByUnique;
        public Image itemImage;

        public CollectableType type = null;

        public GenerationEvent OnEnable;
        public GenerationEvent OnCollected;
        public GenerationEvent OnInteract;
        public GenerationEvent OnInteractNotCollectedYet;

        private List<int> tmpIntList = new List<int>();
        private List<CollectableType> tmpTypeList;

        private bool initialized = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!initialized)
            {
                //initialisation is made in update because we have to wait that all CollectableType register to CollectionManager (done in start)
                #region Initialization
                if (usedTypes.Count < CollectionManager.instance.types.Keys.Count && !type && (int)selectByUnique != -1)
                {
                    //look for an unused "SelectByUnique"
                    int randomID;
                    switch ((int)selectByUnique)
                    {
                        case 1:
                            //store all used features
                            tmpIntList.Clear();
                            foreach (CollectableType t in usedTypes)
                            {
                                if (!tmpIntList.Contains(t.feature))
                                    tmpIntList.Add(t.feature);
                            }
                            //find a type with unused feature
                            tmpTypeList = new List<CollectableType>(CollectionManager.instance.types.Values);
                            for(int i = 0; i < CollectionManager.instance.types.Values.Count; i++)
                            {
                                randomID = (int)Random.Range(0, tmpTypeList.Count - 0.001f);
                                if (tmpTypeList[randomID].collectableByPlayer && !tmpIntList.Contains(tmpTypeList[randomID].feature))
                                {
                                    type = tmpTypeList[randomID];
                                    break;
                                }
                                tmpTypeList.RemoveAt(randomID);
                            }
                            break;

                        case 2:
                            //store all used classifications
                            tmpIntList.Clear();
                            foreach (CollectableType t in usedTypes)
                            {
                                if (!tmpIntList.Contains(t.classification))
                                    tmpIntList.Add(t.classification);
                            }
                            //find a type with unused classification
                            tmpTypeList = new List<CollectableType>(CollectionManager.instance.types.Values);
                            for (int i = 0; i < CollectionManager.instance.types.Values.Count; i++)
                            {
                                randomID = (int)Random.Range(0, tmpTypeList.Count - 0.001f);
                                if (tmpTypeList[randomID].collectableByPlayer && !tmpIntList.Contains(tmpTypeList[randomID].classification))
                                {
                                    type = tmpTypeList[randomID];
                                    break;
                                }
                                tmpTypeList.RemoveAt(randomID);
                            }
                            break;

                        case 3:
                            //store all used zones
                            tmpIntList.Clear();
                            foreach (CollectableType t in usedTypes)
                            {
                                if (!tmpIntList.Contains(t.zone))
                                    tmpIntList.Add(t.zone);
                            }
                            //find a type with unused zone
                            tmpTypeList = new List<CollectableType>(CollectionManager.instance.types.Values);
                            for (int i = 0; i < CollectionManager.instance.types.Values.Count; i++)
                            {
                                randomID = (int)Random.Range(0, tmpTypeList.Count - 0.001f);
                                if (tmpTypeList[randomID].collectableByPlayer && !tmpIntList.Contains(tmpTypeList[randomID].zone))
                                {
                                    type = tmpTypeList[randomID];
                                    break;
                                }
                                tmpTypeList.RemoveAt(randomID);
                            }
                            break;

                        default:
                            break;
                    }

                    if (!type)
                    {
                        //if no type found, find an unused species/type
                        foreach (CollectableType t in CollectionManager.instance.types.Values)
                        {
                            if (!usedTypes.Contains(t))
                            {
                                type = t;
                                break;
                            }
                        }
                    }

                    if (!type)
                        gameObject.SetActive(false);
                    else
                    {
                        itemImage.sprite = type.sprite;
                        //add type to usedTypes
                        usedTypes.Add(type);

                        //set Displayable elementName and description
                        elementName = CollectionManager.instance.listSpecies[type.species];
                        description = string.Concat(CollectionManager.instance.listZones[type.zone], " - ", CollectionManager.instance.listClassifications[type.classification], " - ", CollectionManager.instance.listFeatures[type.feature]);

                        //bind event to OnCollected of the type
                        type.OnCollected.AddListener(InvokeEventOnCollected);
                        type.OnInteract.AddListener(InvokeEnventOnInteract);
                        OnCollected.AddListener(DisplaySprite);
                    }

                    //invoke event OnEnable (used to inscrease mission count target)
                    OnEnable.Invoke(type);
                }
                else
                    gameObject.SetActive(false);
                #endregion

                initialized = true;
            }
        }

        public void DisplaySprite(CollectableType type)
        {
            itemImage.gameObject.SetActive(true);
        }

        private void InvokeEventOnCollected(Collectable collectable)
        {
            OnCollected.Invoke(type);
        }

        private void InvokeEnventOnInteract(Collectable collectable)
        {
            if (!CollectionManager.instance.CheckIfIDIsCollected(type.Id))
                OnInteractNotCollectedYet.Invoke(type);
            OnInteract.Invoke(type);
        }
    }
}
