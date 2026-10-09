
Pourquoi on implémente nos propres structures de données ? 
- on est obligés onne peut pas utiliser System.Numerics.Vector3 par ex. 

---

C'est quoi le JointPivot ? --> pas sûre de comprendre pourquoi on a la pile qui place l'articulation et en plus un pivot.. 

---
hangFromTop and sitOnBottom in Initcharacter -> lien avec JointPivot

   Vector3 center = Vector3.Zero;
        Vector3 hangFromTop = new(0f, -0.5f, 0f);
        Vector3 sitOnBottom = new(0f, 0.5f, 0f);



--- NOTES A CLARIFIER

Affirmations à verifier: 
Une vecteur c'est un point xyz dans l'espace 
Une Matrice sert à bouger des points dans un espace 


Questions:
OnLoad() -> jamais appelé ? 