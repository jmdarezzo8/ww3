#!/bin/bash
# WW3 Unity project updater for macOS.
# Double-click this file inside your WW3 project folder.
cd "$(dirname "$0")" || exit 1
echo "==========================================="
echo "       WW3 - Update from GitHub"
echo "==========================================="
echo ""
if ! command -v git >/dev/null 2>&1; then
  echo "ERROR: Git was not found. Install Xcode Command Line Tools."
elif [ ! -d ".git" ]; then
  echo "ERROR: Place this updater inside the WW3 project folder."
elif [ -n "$(git status --porcelain)" ]; then
  echo "No changes downloaded: you have local project changes."
  echo "Please commit or review them before updating."
else
  echo "Checking GitHub for changes..."
  git pull --ff-only
  result=$?
  if [ "$result" -eq 0 ]; then
    echo ""
    echo "Update complete! Open WW3 in Unity Hub."
  else
    echo ""
    echo "Update stopped safely. See the Git message above."
  fi
fi
echo ""
read -r -p "Press Return to close this window..."
