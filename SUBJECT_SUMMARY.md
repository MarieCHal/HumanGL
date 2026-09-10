IV. General instructions
A Makefile or a similar build system is required. Only the contents of your repository will be evaluated.
You can use the graphic library of your choice (SDL2, Glut, SFML..).
You need to use your own matrices and transformations with at least OpenGL 4.0[cite: 1].
You are free to use whatever language you want[cite: 1].

V. Mandatory part
Body parts should be correctly articulated using your matrix stack[cite: 1]. If the torso rotates, all the limbs must follow accordingly[cite: 1]. Therefore, if the upper arm moves, only the forearm should follow[cite: 1]. When you modify the size of a limb, related parts must automatically reposition themselves[cite: 1].

Your model will have the following parts:
 - a head[cite: 1]
 - a torso[cite: 1]
 - two arms with[cite: 1]
   - upper arm[cite: 1]
   - forearm[cite: 1]
 - two legs with[cite: 1]
   - thigh[cite: 1]
   - lower part[cite: 1]

It should be able to walk, jump and stay put[cite: 1].

VI. Constraints
Each body part will be drawn by one and only one function call[cite: 1]. This function will draw a 1x1x1 geometric shape at the origin of the current matrix[cite: 1]. Otherwise you will not get all the points[cite: 1].

VII. Bonus part
When your hierarchical model is completely working, it will be easy to add:
• More body parts[cite: 1].
• Other move patterns (Disco dance, Kung-fu fighting, ...)[cite: 1].
• A kick-ass graphic interface where you can for example modify body part size, change their color, etc...[cite: 1]
There will be some points dedicated to these bonuses and some more for your creativity[cite: 1].

The bonus part will only be assessed if the mandatory part is PERFECT[cite: 1]. Perfect means the mandatory part has been integrally done and works without malfunctioning[cite: 1]. If you have not passed ALL the mandatory requirements, your bonus part will not be evaluated at all[cite: 1].

VIII. Submission and peer-evaluation
Turn in your assignment in your Git repository as usual[cite: 1]. Only the work inside your repository will be evaluated during the defense[cite: 1]. Don't hesitate to double check the names of your folders and files to ensure they are correct[cite: 1].

Be prepared to:
• Obviously, run the program and demonstrate the different movement patterns[cite: 1].
• Modify limb sizes. This must be easy to achieve, either in your code or at runtime[cite: 1].
• Show your drawing function, its calls, and explain how it works[cite: 1].
• Explain your hierarchical model and the resulting matrix stack[cite: 1].