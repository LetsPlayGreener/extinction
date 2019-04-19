using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    public class DegraderBehaviour : MonoBehaviour
    {
        public CharacterController2D controller;
        private SpriteRenderer sprite;

        public CollectableType degradedType;

        private Transform movingTarget = null;
        private Collectable degradingTarget = null;
        private bool initialOrientation;
        public float speed = 1;
        public float range = 3;

        //the time necessary to degrade one element
        public float degradingDuration = 10;
        private float degradingTimer;
        private bool degradationPaused = false;
        private float spentTimeAtPause;

        private int nbDegradedElements = 0;
        public int nbDegradedGoal = -1;

        private List<Collectable> cantReach;

        private float degradationBlinkTimer = float.MaxValue;

        // Start is called before the first frame update
        void Start()
        {
            cantReach = new List<Collectable>();
            if (speed < 0)
                speed = 0;
            if (nbDegradedGoal < 0)
                nbDegradedGoal = 0;

            if (!degradedType)
                enabled = false;

            if (GetComponent<SpriteAnimator>())
                GetComponent<SpriteAnimator>().movingSpeed = 0;

            sprite = GetComponent<SpriteRenderer>();
            if (sprite)
                initialOrientation = sprite.flipX;
            else
                enabled = false;
        }

        private void FixedUpdate()
        {
            if (degradationPaused)
                return;

            if(movingTarget && !degradingTarget)
            {
                Vector3 target = new Vector3(movingTarget.transform.position.x, transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                if (transform.position == target)
                {
                    if ((transform.position - movingTarget.position).magnitude < range)
                    {
                        //Start degrading
                        degradingTarget = movingTarget.GetComponent<Collectable>();
                        degradingTimer = Time.time;
                        degradationBlinkTimer = Time.time;
                    }
                    else
                    {
                        cantReach.Add(movingTarget.GetComponent<Collectable>());
                        movingTarget = null;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (degradationPaused)
                return;

            if(!movingTarget)
            {
                //if no target, find the closest target of the type degradedType
                if (CollectionManager.instance.collectables.ContainsKey(degradedType.Id) && CollectionManager.instance.collectables[degradedType.Id].Count > 0)
                {
                    foreach (Collectable elem in CollectionManager.instance.collectables[degradedType.Id])
                    {
                        if (!cantReach.Contains(elem) && !elem.destroyed)
                        {
                            if (movingTarget == null)
                                movingTarget = elem.transform;
                            //else if elem is closer than current movingTarget
                            else if ((elem.transform.position - transform.position).magnitude < (movingTarget.transform.position - transform.position).magnitude)
                                movingTarget = elem.transform;
                        }
                    }
                    if (movingTarget.transform.position.x > transform.position.x)
                        sprite.flipX = initialOrientation;
                    else
                        sprite.flipX = !initialOrientation;
                }
            }

            if (degradingTarget)
            {
                if (Time.time - degradationBlinkTimer > 2f)
                {
                    degradingTarget.Image.color = Color.red;
                    degradationBlinkTimer = Time.time + 1.5f;
                }
                else if(Time.time - degradationBlinkTimer < 0 && Time.time - degradationBlinkTimer > -1f)
                {
                    degradingTarget.Image.color = Color.white;
                    degradationBlinkTimer = Time.time;
                }

                if(Time.time - degradingTimer > degradingDuration)
                {
                    degradingTarget.Image.color = Color.white;
                    //animate and disable degraded object
                    degradingTarget.gameObject.GetComponent<SpriteAnimator>().PlayAnimationOnce(true);
                    degradingTarget.destroyed = true;

                    degradingTarget = null;
                    movingTarget = null;

                    degradationBlinkTimer = float.MaxValue;

                    nbDegradedElements++;
                    if (nbDegradedElements >= nbDegradedGoal)
                        PauseDegradation();
                }
            }
        }

        public void SetSpeed(float newSpeed)
        {
            if (newSpeed < 0)
                speed = 0;
            else
                speed = newSpeed;
        }

        public void PauseDegradation()
        {
            if (!degradationPaused)
            {
                degradationPaused = true;
                spentTimeAtPause = Time.time - degradingTimer;
            }
        }

        public void ResumeDegradation()
        {
            if (degradationPaused)
            {
                degradationPaused = false;
                degradingTimer = Time.time - spentTimeAtPause;
            }
        }
    }
}
