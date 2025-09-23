# Vision For DP
This is a Unity project I made to show off the simple 2d vision/light algorithm along with a player controller that I made. All of the code that was used in the project was made by me, mainly taken from an earlier personal project, and was written without the use of AI.
<img width="1912" height="942" alt="image" src="https://github.com/user-attachments/assets/7ffdc129-9425-47e0-bdf8-aae1da8788bb" />

## The 2d Vision System
### Corners
Since just shooting rays in front of the player and just making the mesh around where the rays land would make it jittery, let the mesh cut corners, and in general look bad, I needed to make my algorithm form around the corners themselves. To do this I made a system to record all the corners on each collider.   
<img width="419" height="128" alt="image" src="https://github.com/user-attachments/assets/958b93c4-db90-46cf-9c86-bebae7ad9fc0" />  


### FOV
The algorithm first gets all the corners within the field of view(FOV). In order to have the mesh to curve along the field of view it adds a number of corners on the far end of the FOV. These corners at the far end of the FOV checks for any colliders in between and then places a corner along the edge of the collider so that mesh will form along the collider. All these corners are then sorted from left to right. Once all corners are sorted, it is then determined if the corner is in line of sight, if so a vertex is then added to where the corner is and a vertex past the corner(unless it is being looked at head on). These vertices are the final points of the mesh.  
<img width="515" height="350" alt="image" src="https://github.com/user-attachments/assets/4a1b10cb-024a-4c09-bec2-b6c2de3a86f3" />

### Sprite Mask
To add the mask I created a very large circle sprite that covers everything which is put into a sprite mask. The geometry of the sprite UV is then overridden with the vision mesh. This method lets the resolution of the sprite to be irrelevant to the resolution of the sprite mask (with some small modifications to the code it can work with a white 1p sprite).  
<img width="1916" height="810" alt="image" src="https://github.com/user-attachments/assets/2fe39d9f-b10a-4f7e-a1da-d0b3dc42c984" />

## Movement system
I made this movement system to stop the player from jittering in and out of colliders when running into them and without using a rigidbody. This is done by casting small raycasts all along the front of the player's collider in the direction the player is moving. Once one of the raycasts hit a collider the player movement is reduced to the distance of the smallest ray to have the player's collider be flush with the other collider.  
<img width="53" height="47" alt="image" src="https://github.com/user-attachments/assets/eccbb37b-19fa-43de-a03c-04d715f2afd2" />  

### Moving Along the Wall
I realized that once the player would hit the collider they would stop in their tracks even if they were coming at it from an angle, which felt very clunky. To resolve this I made it so that after the movement is reduced (which if the player is flush against the wall would be zero), the original movement is added back but it would be projected along the normal of the collider. This allows the player to glide along the wall while still being flush with it.  
<img width="139" height="79" alt="image" src="https://github.com/user-attachments/assets/5e4cbff9-9746-47e4-abfa-c4a8ce1060a8" />


