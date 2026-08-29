using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace animator
{
    public class Anim : MonoBehaviour
    {
        #region events, properties and fields

        // === ДАННЫЕ ТРИГГЕРА / TRRIGGER DATAS ===

        [System.Serializable]
        public class AnimationTriggerData
        {
            public int frameNumber;      // Кадр срабатывания
            public string eventName;     // Имя события
        }

        // === СОСТОЯНИЕ ТРИГГЕРА / TRIGGER STATYS ===

        private class AnimationTriggerState
        {
            public AnimationTriggerData data;
            public bool fired;

            public AnimationTriggerState(AnimationTriggerData data)
            {
                this.data = data;
                this.fired = false;
            }
        }

        [System.Serializable]
        public class AnimationSequence
        {
            public string name;                          // Уникальное имя анимации
            public List<Sprite> frames;                  // Кадры анимации
            public float framesPerSecond = 10f;          // Скорость для конкретной анимации
            public bool loop = false;                    // Зацикливать ли анимацию
            public List<AnimationTriggerData> triggers = new List<AnimationTriggerData>(); // Триггеры для этой анимации
        }


        // === СОБЫТИЯ / EVENT ===

        public event System.Action<string> OnAnimationTrigger; // Вызывается при срабатывании триггера
        public event System.Action OnAnimationComplete;        // Вызывается при завершении анимации

        [Header("Список анимаций")]
        public List<AnimationSequence> animations = new List<AnimationSequence>();

        [Header("Текущие настройки")]
        public string defaultAnimation = "Idle";
        public bool isPlaying = false;

        private SpriteRenderer spriteRenderer;
        private float timer = 0f;
        private int currentFrame = 0;
        private bool animationCompleted = false;
        private string currentAnimation;
        private AnimationSequence currentSequence;

        // === РАНТАЙМ-СОСТОЯНИЯ ТРИГГЕРОВ / TRIGGER RUNTIME STATES ===
        private List<AnimationTriggerState> triggerStates = new List<AnimationTriggerState>();

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SwitchAnimation(defaultAnimation);
        }

        void FixedUpdate()
        {
            if (!isPlaying || currentSequence == null)
                return;

            // Обновление таймера
            timer += Time.fixedDeltaTime;
            float interval = 1f / currentSequence.framesPerSecond;

            // Проверка необходимости смены кадра
            if (timer >= interval)
            {
                timer -= interval;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            // Переход к следующему кадру
            currentFrame++;

            // Проверка завершения анимации
            if (currentFrame >= currentSequence.frames.Count)
            {
                // При перезапуске сбрасываем состояния триггеров
                ResetTriggerStates();

                if (currentSequence.loop)
                {
                    currentFrame = 0; // Циклический переход
                }
                else
                {
                    currentFrame = currentSequence.frames.Count - 1; // Фиксация на последнем кадре
                    animationCompleted = true;
                    isPlaying = false;
                    OnAnimationComplete?.Invoke();
                    return; // Выходим, чтобы не проверять триггеры на несуществующем кадре
                }
            }

            // === ПРОВЕРКА ТРИГГЕРОВ / TRIGGER CHECK===

            CheckTriggers();

            // Обновление отображаемого спрайта
            if (currentFrame < currentSequence.frames.Count)
            {
                spriteRenderer.sprite = currentSequence.frames[currentFrame];
            }
        }

        /// <summary>
        /// Проверка всех триггеров на текущем кадре / Checking all triggers on the current frame
        /// </summary>
        private void CheckTriggers()
        {
            if (triggerStates == null || triggerStates.Count == 0)
                return;

            foreach (var state in triggerStates)
            {
                if (state.data.frameNumber == currentFrame && !state.fired)
                {
                    state.fired = true;
                    OnAnimationTrigger?.Invoke(state.data.eventName);
                }
            }
        }

        /// <summary>
        /// Сброс состояний триггеров (при старте/перезапуске анимации) / Resetting trigger states (upon animation start/restart)
        /// </summary>
        private void ResetTriggerStates()
        {
            if (triggerStates != null)
            {
                foreach (var state in triggerStates)
                {
                    state.fired = false;
                }
            }
        }

        /// <summary>
        /// Инициализация состояний триггеров для текущей анимации / Initializing trigger states for the current animation
        /// </summary>
        private void InitTriggerStates()
        {
            triggerStates.Clear();
            if (currentSequence != null && currentSequence.triggers != null)
            {
                foreach (var triggerData in currentSequence.triggers)
                {
                    triggerStates.Add(new AnimationTriggerState(triggerData));
                }
            }
        }
        #endregion
        #region Public

        // === ПУБЛИЧНЫЕ МЕТОДЫ / PUBLIC METHODS ===

        /// <summary>
        /// Переключение анимаций / Swich animation
        /// </summary>
        public void SwitchAnimation(string animationName)
        {
            // Поиск анимации в списке
            AnimationSequence newSequence = animations.Find(a => a.name == animationName);

            if (newSequence == null)
            {
                Debug.LogWarning($"Анимация {animationName} не найдена!");
                return;
            }

            // Инициализация новой анимации
            currentAnimation = animationName;
            currentSequence = newSequence;
            currentFrame = 0;
            timer = 0;
            animationCompleted = false;
            isPlaying = true;

            // Инициализация триггеров для новой анимации
            InitTriggerStates();

            // Немедленное отображение первого кадра
            if (currentSequence.frames != null && currentSequence.frames.Count > 0)
            {
                spriteRenderer.sprite = currentSequence.frames[0];
            }
        }

        /// <summary>
        /// Принудительный запуск текущей анимации с начала / Force the current animation to start from the beginning
        /// </summary>
        public void Play()
        {
            if (currentSequence == null) return;

            currentFrame = 0;
            timer = 0;
            animationCompleted = false;
            isPlaying = true;

            // Сброс состояний триггеров
            ResetTriggerStates();

            if (currentSequence.frames != null && currentSequence.frames.Count > 0)
            {
                spriteRenderer.sprite = currentSequence.frames[0];
            }
        }

        /// <summary>
        /// Остановка анимации (остается на текущем кадре) / Stop animation (remains on the current frame)
        /// </summary>
        public void Stop()
        {
            isPlaying = false;
        }

        /// <summary>
        /// Продолжение анимации с текущего кадра / Resume animation from the current frame
        /// </summary>
        public void Resume()
        {
            if (currentSequence != null)
                isPlaying = true;
        }

        /// <summary>
        /// Проверка завершения анимации (для нецикличных) / Checking for animation completion (for non-looping animations)
        /// </summary>
        public bool IsAnimationComplete()
        {
            return animationCompleted;
        }

        /// <summary>
        /// Получить имя текущей анимации / Get the name of the current animation
        /// </summary>
        public string GetCurrentAnimationName()
        {
            return currentAnimation;
        }

        /// <summary>
        /// Получить текущий кадр / Get the current frame
        /// </summary>
        public int GetCurrentFrame()
        {
            return currentFrame;
        }

        /// <summary>
        /// Проверка, проигрывается ли анимация / Check whether the animation is playing
        /// </summary>
        public bool IsPlaying()
        {
            return isPlaying;
        }
        #endregion
    }
}