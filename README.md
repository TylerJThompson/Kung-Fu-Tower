# Kung-Fu-Tower
The fate of Kung Fu itself is at risk. The spirit of every ancient Kung Fu Master has been kidnapped, and trapped in the bodies of evil robots stored in the Kung Fu Tower, causing every living martial artist on Earth to lose the ability to perform Kung Fu! However, without the knowledge of the Masters, none of the martial artists can defeat the robots and climb the Tower! At least, none of the living martial artists... You and a friend can take control of the last hope for Kung Fu by controlling a duo of Kung Fu Robots sent in to save the Masters! Too bad the engineers could only remember how to program a karate chop...

# Features
Take part in a co-op experience in which you and a friend climb increasingly difficult randomly generated floors in the Kung Fu Tower! Fight against the evil robots housing the spirits of the Masters, as they attempt to stop you from saving Kung Fu!

# Controls
Player one is controlled by W (move forward), S (move backward), A (pivot left), and D (pivot right). Player two is controlled by the up arrrow (move forward), down arrow (move backward), left arrow (pivot left), and right arrow (pivot right). You will automatically attack when running into an enemy robot. If you would like to karate chop just for the heck of it (as all good Kung Fu artists do, from time to time), player one can press Q (left arm chop) or E (right arm chop) while player two can press home (left arm chop) or page up (right arm chop).

# Post-Mortem
1. While not explicitly stated in my initial design document, I was hoping to give the ability to move in any direction, not just the cardinal directions. This was dropped because the system I used fof checking where walls were in the maze required the player to be in a specific maze cell moving in a specific direction. I was also hoping to use a model more closely resembling a human, but found this too difficult to build and animate with only Unity primitives and without an easy way to rig the model.
2. I improved upon the enemy AI by having them use to raycasting to spot a player, after which they will seek the player out if the player moves too far away and stays in the enemy's line of sight. I decided to make these changes to make the enemies more agressive and lifelike, as they would no longer be standing relatively still, only spinning to look at the player.
3. If I were to continue working on this in the future without the constraints of project 2, I would replace the models I built with primitives with more advanced rigged models from the Asset Store, and use one of the character controllers from the Standard Assets. I would also use nav meshes to help enemies seek out the player even if they lose sight of them, and move away from the current grid system so players and enemies can move in any direction instead of just cardinal directions. However, I am unlikely to continue with this game.
