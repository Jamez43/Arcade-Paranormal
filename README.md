# Arcade Paranormal

Arcade Paranormal is a mobile 2D survival roguelite built in Unity. The player fights possessed arcade machines, collects XP, and chooses upgrades while surviving increasingly difficult enemy waves inside a haunted retro arcade.

> **Status:** MVP in active development. The core gameplay loop is playable; progression, content, animation, and release systems are still being developed.

## [Play Now](https://jamez43.itch.io/arcade-paranormal)

## Core Gameplay Loop

1. Move with a virtual joystick to evade enemies and collect XP.
2. Fight automatically with melee or projectile weapons.
3. Level up and choose upgrades that modify the current run.
4. Survive escalating enemy waves for as long as possible.

## Implemented Features

- Virtual-joystick movement using Unity's Input System
- Melee and projectile weapon architectures
- Enemy waves with obstacle-aware spawning and navigation
- Three enemy types: joystick, arcade cabinet, and pinball machine
- Player health, enemy health, XP, cooldown, and timer UI
- XP-based leveling and an upgrade-selection screen
- Six functional upgrades affecting damage, defense, movement, attack speed, and pickup range
- Main menu, character selection, game-over flow, and multiple playable scenes
- ScriptableObject-based player and enemy configuration

## Technical Design

### Data-driven stats

Player and enemy base attributes are stored in ScriptableObjects. Runtime player stats are copied from the selected base configuration so upgrades can modify the current run without changing the source asset. This keeps gameplay configuration separate from mutable state and makes balancing easier inside the Unity Editor.

### Combat

Melee and projectile attacks use separate controllers while reading from the same runtime player-stat system. Projectile weapons search for targets within a configurable range; melee weapons apply damage within a configurable attack arc.

### Enemies and spawning

Enemy types use dedicated ScriptableObject configurations for health, damage, speed, attack delay, defense, and spawn cost. The spawning system validates candidate positions against floor, walls, obstacles, and nearby colliders before placing an enemy. Unity navigation components provide obstacle-aware movement through the 2D maps.

### Progression

Defeated enemies drop XP pickups. Reaching the current XP threshold pauses normal gameplay and opens an upgrade menu. Selected upgrades modify runtime stats, allowing each run to develop differently.

## Project Structure

```text
Assets/
├── Scenes/               Playable, menu, and testing scenes
├── Scripts/
│   ├── Player/           Movement, stats, weapons, and collisions
│   ├── Enemy/            Enemy behavior, attacks, spawning, and data
│   ├── Upgrades/         Upgrade-selection behavior
│   ├── Controls/         Virtual joystick and input indicators
│   └── Menus/            Main-menu and restart flows
├── Scriptable Objects/   Configurable player and enemy data
├── Prefabs/              Reusable gameplay objects
└── Sprites/              Original game art
```

## Run Locally

### Requirements

- Unity `6000.2.8f1`
- Git

### Setup

```bash
git clone https://github.com/Jamez43/Arcade-Paranormal.git
```

1. Open the cloned directory through Unity Hub with Unity `6000.2.8f1`.
2. Allow Unity to restore the packages declared in `Packages/manifest.json`.
3. Open `Assets/Scenes/Main Menu.unity`.
4. Enter Play Mode.

The project is designed around mobile input and targets iOS and Android.

## Roadmap

- Boss encounters and additional enemy behaviors
- Character, enemy, and weapon animation
- Procedurally generated arcade layouts
- Permanent upgrades and meta-progression
- Save data and cloud-save support
- Additional menus and character-selection improvements
- Mobile performance profiling and optimization

## Assets

The art, audio, and other project assets are original to Arcade Paranormal.
