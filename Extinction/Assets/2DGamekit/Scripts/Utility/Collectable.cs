using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit2D
{
    public class Collectable : MonoBehaviour
    {
        public bool collectableByPlayer;
        public CollectableType type;

        private SpriteRenderer image;

        public CollectableEvent OnInteract;
        public CollectableEvent OnCollected;
        public CollectableEvent OnFailCollection;

        [HideInInspector]
        public bool destroyed = false;

        public CollectableType Type
        {
            get
            {
                return type;
            }

            set
            {
                Unregister();
                type = value;
                Refresh();
            }
        }

        public SpriteRenderer Image
        {
            get
            {
                return image;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Type)
            {
                image = GetComponent<SpriteRenderer>();
                if (!image)
                    image = gameObject.AddComponent<SpriteRenderer>();
                //set sprite as type sprite
                //image.sprite = Type.sprite;
                //set scale and position according to value set in type corresponding to the first collected
                if (Type.WasCollectedAtLeastOnce)
                {
                    transform.localScale = Type.Scale;
                    transform.position += Vector3.up * (-transform.position.y + Type.Height + CollectionManager.instance.RaycastedPosition.transform.position.y);
                }
                collectableByPlayer = Type.collectableByPlayer;

                gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
                Register();
            }
            else
                gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Register()
        {
            if (CollectionManager.instance)
            {
                if (!CollectionManager.instance.collectables.ContainsKey(Type.Id))
                {
                    CollectionManager.instance.collectables.Add(Type.Id, new List<Collectable>());
                    CollectionManager.instance.types.Add(Type.Id, Type);
                }
                if (!CollectionManager.instance.collectables[Type.Id].Contains(this))
                    CollectionManager.instance.collectables[Type.Id].Add(this);
            }
        }

        private void Unregister()
        {
            if (Type && CollectionManager.instance && CollectionManager.instance.collectables.ContainsKey(Type.Id) && CollectionManager.instance.collectables[Type.Id].Contains(this))
                CollectionManager.instance.collectables[Type.Id].Remove(this);
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            Unregister();
        }

        public void Refresh()
        {
            if (Type && Type.Id == null)
                Type.SetID();

            gameObject.SetActive(true);
            enabled = true;
            Unregister();
            Start();
        }
    }
}
