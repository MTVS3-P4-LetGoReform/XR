using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Layer_lab._3D_Casual_Character
{
    public class CharacterControl : MonoBehaviour
    {
        public static CharacterControl Instance;
        public CharacterBase CharacterBase { get; set; }
        private Animator animator; 
        [SerializeField] private TMP_Text textAnimationName;
        private Coroutine _coroutine;
        void Awake()
        {
            Instance = this;
            textAnimationName.text = "Stand_Idle1";
            animator = GetComponentInChildren<Animator>();
           // CharacterBase = GetComponentInChildren<CharacterBase>();
           
        }

        private void Start()
        {
            StartCoroutine(FindPlayer());
        }

        public void PlayAnimation(AnimationClip clip)
        {
            textAnimationName.text = clip.name;
            animator.CrossFadeInFixedTime(clip.name, 0.25f);
            if(_coroutine != null) StopCoroutine(_coroutine);
        
            if (!clip.isLooping)
            {
                StartCoroutine(ChangeIdleText(clip.length));
            }
        
        }

        IEnumerator ChangeIdleText(float duration)
        {
            yield return new WaitForSeconds(duration);
            textAnimationName.text = "Stand_Idle1";
        }
        
        private IEnumerator FindPlayer()
        {
            GameObject playerObject = null;

            while (playerObject == null)
            {
                playerObject = GameObject.FindWithTag("Player");

                if (playerObject == null)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
            
            CharacterBase = playerObject.GetComponentInChildren<CharacterBase>();
        
        }
    }
}
