# Резюме проекта EduMotion Kids

## Репозиторий
https://github.com/igorriazanovst-debug/kinect

## Текущее состояние
- Unity 2022.3.61f1 (6000.4.1f1) установлен
- Unity проект создан в: `C:\Temp\2026\kinect\kinect\My project\`
- Все скрипты скомпилированы БЕЗ ошибок (только warnings)
- Kinect10.dll скопирована в Assets/Plugins/Kinect/
- **MediaPipe pipeline работает** — UDP данные принимаются, жесты распознаются
- Кириллица в кнопках сломана (фикс: EduMotion → Fix MainMenu Text, запускать не в Play mode)

## Структура скриптов (в My project\Assets\Scripts\)
- Core: EventBus.cs, GameManager.cs, RewardSystem.cs
- Kinect: KinectManager.cs, KinectProviders.cs, NativeMethods.cs, SkeletonData.cs, UDPKinectProvider.cs
- Gestures: GestureManager.cs, GestureTypes.cs
- Games: TrafficLightGame.cs, CrossingGame.cs, EcologyGame.cs
- Audio: AudioManager.cs
- UI: GameViews.cs, MainMenuUI.cs, RewardScreenUI.cs, SkeletonDebugView.cs
- Editor: AudioManagerSetup.cs, BuildSettingsSetup.cs, BuildSettingsSetup.cs, FixMainMenuText.cs, GameScenesSetup.cs, MainMenuUISetup.cs

## Железо
- Kinect v1 (Xbox 360 Kinect) — нестандартный разъём, адаптер не куплен
- Замена: встроенная камера + MediaPipe через UDP
- Windows 11
- SDK: Kinect SDK 1.8 (Kinect10.dll)

## Архитектура
MediaPipe (Python) → UDP → UDPKinectProvider → SkeletonData → GestureManager → EventBus → Game Logic

## KinectManager Provider Types
- `Auto` — сначала KinectV1, при неудаче UDP
- `UDP` — только камера через MediaPipe (текущий режим)
- `Mock` — тест без железа
- `KinectV1` — оригинальный Kinect

## Python скрипт
- `mediapipe_sender.py` в корне репо
- Запуск: `python mediapipe_sender.py [--host 127.0.0.1] [--port 7777] [--cam 0]`
- Установка: `pip install mediapipe==0.10.13 opencv-python`

## MVP — 3 игры
- Светофор (TrafficLightGame) — жесты: StepForward, Stop, RaiseHand
- Переход дороги (CrossingGame) — жест: StepForward
- Экология (EcologyGame) — жесты: Turn, RaiseHand

## 4 жеста
- RaiseHand: рука выше головы
- StepForward: нога вперёд по Z
- Turn: поворот плеч
- Stop: обе руки горизонтально

## Сцены
- MainMenu — UI создан, Bootstrap с компонентами
- Game_Traffic — UI создан
- Game_Crossing — UI создан
- Game_Ecology — UI создан
- Все 4 сцены в Build Settings

## Что нужно сделать дальше
1. Починить кириллицу в кнопках (Fix MainMenu Text в не-Play режиме)
2. Откалибровать пороги жестов (StepForward срабатывает постоянно — занижен порог)
3. Добавить Bootstrap в игровые сцены (Game_Traffic, Game_Crossing, Game_Ecology)
4. Тестировать игровые сцены end-to-end
5. Добавить аудио клипы в AudioManager
6. Git push всего проекта

## Важные пути
- Unity проект: `C:\Temp\2026\kinect\kinect\My project\`
- Репо: `C:\Temp\2026\kinect\kinect\`
- Unity exe: `C:\Program Files\Unity 2022.3.61f1\Editor\Unity.exe`
- Python скрипт: `C:\Temp\2026\kinect\kinect\mediapipe_sender.py`

## Правила работы
- Диалог на русском
- Не хардкодить URL/токены/пароли
- Исправления через скрипты PowerShell
- Не объяснять, только код
- Резюме обновлять в виде файла перед git push
