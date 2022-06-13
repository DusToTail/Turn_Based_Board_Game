# Turn_Based_Board_Game

This is a project used to learn about turn-based board game.
The concept will revolve around statues acting as player and enemy interacting with each other on a grid-based board.
The goal is to reach the goal point whilst avoiding traps and enemies

★The project will feature:
+ Turn-based gameplay alternating between player controlling a character and enemies trying to attack the player
+ A battle system where each character is limited with action points (or moves)
+ Level design tool using JSON file format (.txt) to save design and load design into scene
+ Elevation in map design and character (and AI) movement (A*)
+ Limited 3d lighting surrounding the player
+ A variety of enemies, traps, gimmicks, abilities  (SOME)
+ Multiple levels and puzzles (NOT YET)
+ Save system  (NOT YET)

★Level 0(Menu) important block
Cyan(id:500): Load level 0 
Purple(id:501): Load level 1 

★Level 1 important block
School uniform girl statue(id:1): Player
Skeleton statue(id:2): Basic enemy
Transparent yellow(id:201):Start and end point of stairs (auto, no move cost needed)
Brown(id:202): Destroyable block (by attacking)
Light blue(id:203): Trigger for remote door
Dark blue(id:204): Remote door
Transparent black(id:300): Spike, deal 1 damage to those who step on
Transparent red(id:530): Goal, clear level when player steps on

★Current stats
School uniform girl statue(id:1): action points 3, HP 3, Vision 3, Attack range 1
Skeleton statue(id:2): action points 2, HP 3, Vision 5, Attack range 2

Gameplay Control

★Mode: Character
+ W: Move forward (action cost: 1)
+ A: Rotate left (action cost: 1)
+ D: Rotate right (action cost: 1)
+ S: Skip action (action cost: 1)
+ E: Interact with object block (action cost: 1)
+ Spacebar: Deal damage forward (damage and attack grid/range depending on the weapon) (action cost: 1)
+ Q: Switch to Reveal Skill mode + Reset focus cell to player cell

★Mode: Reveal Skill
+ W: Move focus cell one cell forward
+ A: Move focus cell one cell left
+ D: Move focus cell one cell right
+ S: Move focus cell one cell backward
+ E: None
+ Spacebar: Reveal the area at the focus cell (cooldown: 3 turns, reset when level loaded)
+ Q: Switch to Survey mode + Reset focus cell to player cell

★Mode: Survey
+ W: Move focus cell one cell forward
+ A: Move focus cell one cell left
+ D: Move focus cell one cell right
+ S: Move focus cell one cell backward
+ E: None
+ Spacebar: None
+ Q: Switch to Character mode + Reset focus cell to player cell

★Menu Control:
+ WASD: movement in menu
+ Spacebar: Choose option