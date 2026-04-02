# EduMotion Kids

Интерактивное обучающее ПО с управлением движением (Kinect v1 / Xbox 360 Kinect).

## Стек
- Unity 2022 LTS
- C#
- Kinect SDK 1.8 (Windows)
- MockKinectProvider (Linux / Editor без сенсора)

## Архитектура
```
Kinect → SkeletonData → GestureManager → EventBus → Game Logic
```

## Структура
```
Assets/
├── Scripts/
│   ├── Core/          EventBus, GameManager, RewardSystem
│   ├── Kinect/        KinectManager, KinectProviders, NativeMethods, SkeletonData
│   ├── Gestures/      GestureManager, GestureTypes
│   ├── Games/         TrafficLightGame, CrossingGame, EcologyGame, AudioManager
│   └── UI/            MainMenuUI, RewardScreenUI, GameViews, SkeletonDebugView
├── Scenes/
├── Prefabs/
├── Animations/
├── Audio/
├── UI/
├── Materials/
└── Plugins/Kinect/    → сюда кладём Kinect10.dll из Kinect SDK 1.8
```

## MVP — 3 игры
| Игра | Жест GO | Жест СТОП |
|------|---------|-----------|
| Светофор | StepForward (зелёный) | Stop (красный) |
| Переход дороги | StepForward | — |
| Экология | Turn (выбор) + RaiseHand (подтвердить) | — |

## Жесты (4 штуки)
| Жест | Описание |
|------|----------|
| RaiseHand | Рука выше головы |
| StepForward | Нога выдвинута вперёд по Z |
| Turn | Поворот плеч (ΔZ плечей) |
| Stop | Обе руки горизонтально в стороны |

## Установка Kinect SDK
1. Скачать [Kinect SDK 1.8](https://www.microsoft.com/en-us/download/details.aspx?id=40278)
2. Установить
3. Скопировать `Kinect10.dll` из `C:\Windows\System32\` в `Assets/Plugins/Kinect/`

## Запуск без Kinect
На Linux или в Editor без сенсора автоматически активируется `MockKinectProvider` —
анимирует руку для тестирования жестов.
