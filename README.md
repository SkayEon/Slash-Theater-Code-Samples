# Slash-Theater-Code-Samples# 

Slash Theater — Gameplay Programming Portfolio

OVERVIEW

Slash Theater is a 2D roguelike fighting game developed with Unity and C#.

The player fights increasingly challenging enemies, then chooses between skills and passive upgrades to build their character throughout a run.

This repository contains a selection of gameplay programming scripts from the project, showcasing the systems I developed.

MY CONTRIBUTIONS

I was the main programmer on the project, responsible for implementing the core gameplay systems, including:

- Combat system: melee attacks, projectiles, damage handling, knockback and hit stun.
- Character systems: player movement, dashing, shielding and enemy behaviours.
- Enemy AI: state-based behaviour, movement, attack selection and boss phases.
- Skills and passive upgrades: reusable attack architecture and effects that interact with one another.
- Status effects: elemental effects such as fire damage over time, ice stacks, slowing and projectile reflection.
- Gameplay management: fight progression, scene management and save/load functionality.

TECHNICAL DETAILS

- Engine: Unity
- Language: C#
- Architecture: ScriptableObjects, inheritance, interfaces and state machines
- Other systems: Unity Input System, coroutines, events and JSON serialization

REPOSITORY STRUCTURE

The code is organised by gameplay system:

- Attacks/ — attack logic, hitboxes and projectile spawning
- Characters/ — character behaviours, player controls and enemy AI
- FightManagement/ — fight scene and combat management
- GameManagement/ — game state and save/load systems
- Passives/ — passive upgrades and their management
- StatusEffects/ — status effects applied to characters

ABOUT THIS REPOSITORY

This repository is intended as a code portfolio, highlighting selected gameplay systems rather than providing a complete, standalone version of the game. Some scripts depend on other project components and Unity assets that are not included here.

