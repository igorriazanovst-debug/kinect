#!/bin/bash
# Использование:
#   chmod +x push_to_github.sh
#   GITHUB_TOKEN=<your_token> ./push_to_github.sh

set -e

REPO_URL="https://${GITHUB_TOKEN}@github.com/igorriazanovst-debug/kinect.git"
PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"

cd "$PROJECT_DIR"

if [ ! -d ".git" ]; then
  git init
  git remote add origin "$REPO_URL"
fi

git add -A
git commit -m "feat: initial project structure + MVP scripts

- EventBus (Core)
- KinectManager + KinectV1Provider + MockKinectProvider
- NativeMethods (P/Invoke Kinect SDK 1.8)
- SkeletonData structs
- GestureManager (RaiseHand, StepForward, Turn, Stop)
- GameManager, RewardSystem
- TrafficLightGame, CrossingGame, EcologyGame
- AudioManager
- MainMenuUI, RewardScreenUI, GameViews, SkeletonDebugView
- README"

git branch -M main
git push -u origin main --force
