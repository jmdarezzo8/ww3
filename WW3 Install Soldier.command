#!/bin/bash
# WW3 soldier installer — only copies the user's own licensed GLB to Unity.
cd "$(dirname "$0")" || exit 1
target="Assets/WW3/Characters/Soldier01/Models"
file="soldier_low_poly_mesh_game_ready_4k_pbr_rigged.glb"
mkdir -p "$target"
if [ -s "$target/$file" ]; then
  echo "WW3: soldier model already installed."
  exit 0
fi
source=""
# First search predictable locations, without crawling the user's entire Mac.
for folder in "$HOME/Downloads" "$HOME/Desktop" "$HOME/Documents"; do
  if [ -d "$folder" ]; then
    source="$(find "$folder" -maxdepth 3 -type f -name "$file" -print -quit 2>/dev/null)"
    if [ -n "$source" ]; then break; fi
  fi
done
if [ -z "$source" ]; then
  echo "WW3: Can't find your downloaded soldier. Opening the file picker..."
  source="$(osascript -e 'POSIX path of (choose file with prompt "Select the soldier GLB file you downloaded for WW3")' 2>/dev/null)" || true
fi
if [ -z "$source" ] || [ ! -f "$source" ]; then
  echo "WW3: Soldier was not selected. Existing game files are unchanged."
  exit 0
fi
case "$source" in
  *.glb) ;;
  *) echo "WW3: selected file must be a .glb model."; exit 1;;
esac
if [ "$source" = "$(pwd)/$target/$file" ]; then
  echo "WW3: model is already in place."
  exit 0
fi
echo "WW3: installing soldier model, preserving its 4K textures and rig..."
cp "$source" "$target/$file" || exit 1
if [ ! -s "$target/$file" ]; then
  echo "WW3: file copy failed."; exit 1
fi
echo "WW3: soldier GLB installed successfully."
echo "Next: open WW3 in Unity. The editor will import the GLB and construct a playable test scene."
