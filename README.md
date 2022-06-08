# Turn_Based_Board_Game

This is a project used to learn about turn-based board game.
The concept will revolve around statues acting as player and enemy interacting with each other on a grid-based board.
The goal is to reach the goal point whilst avoiding traps and enemies

The project will feature:
+ Turn-based gameplay alternating between player controlling a character and enemies trying to attack the player
+ A battle system where each character is limited with action points (or moves)
+ Level design tool using JSON file format (.txt) to save design and load design into scene
+ Elevation in map design and character (and AI) movement (A*)
+ Limited 3d lighting surrounding the player
+ A variety of enemies, traps, gimmicks, abilities  (SOME)
+ Multiple levels and puzzles (NOT YET)
+ Save system  (NOT YET)

Current Blocks:
+ Green statue (id: 1): Player
+ Purple block (id: 2) (in gameplay): Basic enemy
+ White block (id: 100): basic terrain
+ Yellow block (id: 201): Stairs start/end point (auto, no action points required)
+ Cyan block (id: 500): Load level 0 (menu stage)
+ Purple block (id: 501) (in menu, when application starts running): Load level 1
+ Red block (id: 530): goal

Current Action Points per Turn:
+ Player: 3
+ Basic Enemy: 2

Gameplay Control
Mode: Character
+ W: Move forward (action cost: 1)
+ A: Rotate left (action cost: 1)
+ D: Rotate right (action cost: 1)
+ S: Skip action (action cost: 1)
+ E: Interact with object block (action cost: 1)
+ Spacebar: Deal damage forward (damage and attack grid/range depending on the weapon) (action cost: 1)
+ Q: Switch to Reveal Skill mode + Reset focus cell to player cell
Mode: Reveal Skill
+ W: Move focus cell one cell forward
+ A: Move focus cell one cell left
+ D: Move focus cell one cell right
+ S: Move focus cell one cell backward
+ E: None
+ Spacebar: Reveal the area at the focus cell (cooldown: 3 turns, reset when level loaded)
+ Q: Switch to Survey mode + Reset focus cell to player cell
Mode: Survey
+ W: Move focus cell one cell forward
+ A: Move focus cell one cell left
+ D: Move focus cell one cell right
+ S: Move focus cell one cell backward
+ E: None
+ Spacebar: None
+ Q: Switch to Character mode + Reset focus cell to player cell


Menu Control:
+ WASD: movement in menu
+ Spacebar: Choose option