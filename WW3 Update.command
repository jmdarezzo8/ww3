#!/bin/bash
# WW3 updater — safely fetch approved changes from the default branch.
cd "$(dirname "$0")" || exit 1
echo "==========================================="
echo "         WW3 - Update from GitHub"
echo "==========================================="
echo ""
if ! command -v git >/dev/null 2>&1; then
  echo "ERROR: Git not available."
elif [ ! -d ".git" ]; then
  echo "ERROR: Keep this updater inside your WW3 project folder."
elif [ "$(git branch --show-current)" != "main" ]; then
  echo "Update stopped: currently on a development branch."
  echo "Switch back to main before using the regular updater."
elif ! git -c core.fileMode=false diff --quiet || ! git diff --cached --quiet; then
  echo "Update stopped: tracked files have unsaved Git changes."
  echo "Your work has NOT been overwritten."
else
  echo "Checking for WW3 updates..."
  git -c core.fileMode=false pull --ff-only
  if [ "$?" -eq 0 ]; then
    echo ""
    echo "Update downloaded successfully."
    if [ -f "WW3 Install Soldier.command" ]; then
      bash "WW3 Install Soldier.command"
    fi
    echo ""
    echo "Open WW3 in Unity Hub and press Play. A first-time GLB import can take time."
  else
    echo ""
    echo "Update stopped safely. See Git message above."
  fi
fi
echo ""
read -r -p "Press Return to close this window..."
