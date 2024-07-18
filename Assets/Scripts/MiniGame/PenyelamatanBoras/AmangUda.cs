using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

namespace TodongtoaSaga.Minigames.PenyelamatanBoras
{
    public class AmangUda : MonoBehaviour
    {
        [Header("Movement")]

        // Moving
        [SerializeField] private float speed = 5f;
        [SerializeField] private Range idleTime = new(2f, 10f);

        // Idle
        private int currentPointIndex = 0;


        [Tooltip("The parent of points that Amang Uda will move to")]
        [SerializeField] private Transform points;
        private PosOrientation[] pointList;

        private Animator animator;

        private void Awake() {
            pointList = new PosOrientation[points.childCount];
            for (int i = 0; i < points.childCount; i++) {
                pointList[i] = points.GetChild(i).GetComponent<PosOrientation>();
            }

            animator = GetComponent<Animator>();
        }

        private void Start() {
            StartCoroutine(WaitUntilStartThen(Move));
        }
        
        private IEnumerator WaitUntilStartThen(Action callback) {
            yield return new WaitUntil(() => GameStateManager.Instance.CurrentState == GameState.Gameplay);
            callback?.Invoke();
        }

        private IEnumerator Idle(Vector2 orientation) {
            animator.SetFloat("Horizontal", orientation.x);
            animator.SetFloat("Vertical", orientation.y);
            animator.SetFloat("Speed", 0f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(idleTime.min, idleTime.max));
            Move();
        }

        private void Move() {
            if (GameStateManager.Instance.CurrentState != GameState.Gameplay) return;

            PosOrientation pos = GetRandomPosPoint();
            Vector2 targetPosition = pos.transform.position;
            Vector2 direction = targetPosition - (Vector2)transform.position;
            float distance = direction.magnitude;
            float duration = distance / speed;

            animator.SetFloat("Speed", 1f);
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            transform.DOMove(targetPosition, duration).SetEase(Ease.Linear).OnComplete(() => {
                StartCoroutine(Idle(pos.GetOrientationDirection()));
            });
        }

        private PosOrientation GetRandomPosPoint() {
            int nextPointIndex = UnityEngine.Random.Range(0, pointList.Length);
            while (nextPointIndex == currentPointIndex) {
                nextPointIndex = UnityEngine.Random.Range(0, pointList.Length);
            }

            currentPointIndex = nextPointIndex;
            return pointList[currentPointIndex];
        }
    }
}