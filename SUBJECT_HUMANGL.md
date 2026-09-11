# HumanGL — Subject

## IV. General instructions

- A Makefile or a similar build system is required.
- Only the contents of your repository will be evaluated.
- You can use the graphic library of your choice (SDL2, Glut, SFML…).
- You need to use your own matrices and transformations with at least **OpenGL 4.0**.
- You are free to use whatever language you want.

## V. Mandatory part

Body parts should be correctly articulated using your **matrix stack**:

- If the torso rotates, all the limbs must follow accordingly.
- If the upper arm moves, only the forearm should follow.
- When you modify the size of a limb, related parts must automatically reposition themselves.

### Required body parts

- Head
- Torso
- Two arms, each with:
  - Upper arm
  - Forearm
- Two legs, each with:
  - Thigh
  - Lower part

### Required animations

The model must be able to:

- Walk
- Jump
- Stay put (idle)

## VI. Constraints

- Each body part will be drawn by **one and only one** function call.
- This function will draw a **1 × 1 × 1** geometric shape at the origin of the current matrix.
- Otherwise you will not get all the points.

## VII. Bonus part

When the hierarchical model is completely working, it will be easy to add:

- More body parts
- Other move patterns (disco dance, kung-fu fighting, …)
- A kick-ass graphic interface (modify body part size, change colors, etc.)

There will be some points dedicated to these bonuses and some more for creativity.

> **Important:** The bonus part will only be assessed if the mandatory part is **PERFECT**.  
> Perfect means the mandatory part has been integrally done and works without malfunctioning.  
> If you have not passed **ALL** the mandatory requirements, your bonus part will not be evaluated at all.

## VIII. Submission and peer-evaluation

- Turn in your assignment in your Git repository as usual.
- Only the work inside your repository will be evaluated during the defense.
- Double-check the names of your folders and files to ensure they are correct.

### Be prepared to

- Run the program and demonstrate the different movement patterns
- Modify limb sizes (easy to achieve, either in code or at runtime)
- Show your drawing function, its calls, and explain how it works
- Explain your hierarchical model and the resulting matrix stack
