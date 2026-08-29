
# AnimController2D

Простой и гибкий контроллер 2D анимаций для Unity, основанный на SpriteRenderer. Позволяет создавать покадровые анимации с настраиваемой скоростью, зацикливанием и системой триггеров.

A simple and flexible 2D animation controller for Unity based on SpriteRenderer. Allows creating frame-by-frame animations with customizable speed, looping, and a trigger system.

---

## Возможности / Features

- Покадровые анимации на основе спрайтов / Frame-by-frame sprite animations
- Зацикливание анимаций / Animation looping
- Триггеры на определённых кадрах / Triggers on specific frames
- Полное управление воспроизведением / Full playback control
- Простая интеграция через компонент / Easy component-based integration

---

## Установка / Installation

1. Скопируйте скрипт `Anim.cs` в ваш проект Unity / Copy `Anim.cs` to your Unity project
2. Добавьте компонент `Anim` на объект со `SpriteRenderer` / Add `Anim` component to a GameObject with `SpriteRenderer`
3. Настройте анимации в инспекторе / Configure animations in the Inspector

---

## Настройка анимаций / Animation Setup

### Каждая анимация содержит / Each animation contains:

| Параметр / Parameter | Описание / Description |
|---|---|
| `name` | Уникальное имя анимации / Unique animation name |
| `frames` | Список спрайтов-кадров / List of sprite frames |
| `framesPerSecond` | Скорость воспроизведения (FPS) / Playback speed (FPS) |
| `loop` | Зацикливание / Looping |
| `triggers` | Список триггеров / List of triggers |

### Триггеры / Triggers

Срабатывают на указанном кадре и вызывают событие / Fire on the specified frame and invoke an event:

| Параметр / Parameter | Описание / Description |
|---|---|
| `frameNumber` | Номер кадра / Frame number |
| `eventName` | Имя события / Event name |

---

## Публичные методы / Public Methods

### SwitchAnimation(string animationName)

Переключает анимацию по имени. Если анимация не найдена — выводит предупреждение в консоль.

Switches to the animation by name. If not found — logs a warning.

```csharp
anim.SwitchAnimation("Run");
```

---

### Play()

Запускает текущую анимацию с первого кадра.

Starts the current animation from the first frame.

```csharp
anim.Play();
```

---

### Stop()

Останавливает анимацию, оставаясь на текущем кадре.

Stops the animation, staying on the current frame.

```csharp
anim.Stop();
```

---

### Resume()

Продолжает воспроизведение анимации с текущего кадра.

Resumes playback from the current frame.

```csharp
anim.Resume();
```

---

### IsAnimationComplete()

Возвращает `true`, если нецикличная анимация завершилась.

Returns `true` if a non-looping animation has finished.

```csharp
if (anim.IsAnimationComplete()) { ... }
```

---

### GetCurrentAnimationName()

Возвращает имя текущей анимации.

Returns the name of the current animation.

```csharp
string name = anim.GetCurrentAnimationName();
```

---

### GetCurrentFrame()

Возвращает номер текущего кадра.

Returns the current frame number.

```csharp
int frame = anim.GetCurrentFrame();
```

---

### IsPlaying()

Возвращает `true`, если анимация воспроизводится.

Returns `true` if the animation is playing.

```csharp
if (anim.IsPlaying()) { ... }
```

---

## События / Events

### OnAnimationTrigger(string eventName)

Вызывается при срабатывании триггера на кадре.

Invoked when a frame trigger fires.

```csharp
anim.OnAnimationTrigger += (eventName) =>
{
    Debug.Log($"Trigger: {eventName}");
};
```

---

### OnAnimationComplete

Вызывается при завершении нецикличной анимации.

Invoked when a non-looping animation completes.

```csharp
anim.OnAnimationComplete += () =>
{
    Debug.Log("Animation finished!");
};
```

---

## Пример использования / Usage Example

```csharp
using UnityEngine;
using animator;

public class Example : MonoBehaviour
{
    private Anim anim;

    void Start()
    {
        anim = GetComponent<Anim>();
        anim.OnAnimationTrigger += HandleTrigger;
        anim.OnAnimationComplete += HandleComplete;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SwitchAnimation("Attack");
            anim.Play();
        }
    }

    private void HandleTrigger(string eventName)
    {
        Debug.Log($"Trigger fired: {eventName}");
        // Например, нанести урон / e.g., deal damage
    }

    private void HandleComplete()
    {
        Debug.Log("Animation complete!");
        // Вернуться к Idle / Return to Idle
        anim.SwitchAnimation("Idle");
        anim.Play();
    }
}
```

---

## Лицензия / License

MIT — используйте свободно / Free to use
