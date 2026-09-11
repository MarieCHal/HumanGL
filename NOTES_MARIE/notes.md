## STACKING DE MATRICES

Un membre parent hérite de la matrice de son parent:

Example: le trose bouge de deux, le bras bouge de 45 degrés mais herite du mouvement du torse 
    -> donc il bouge de 2 et il rotate de 45 deg. 

Pour avoir du coup le mouvement du bras on a multiplier sa matrice avec celle du torse on a 'push', 
maintenant pour bouger la jambe on veut se défaire du mouvement du bras donc on 'pop'

Terme technique: Matrix Stack -> litteralement avec des push et de pop
In c#: System.Collections.Generic

## PRINCIPE DE RENDER:

De base on donne au GPU seulement un cue 1 X 1 X 1 ce qui veut dire qu'il prends 1 unité de haut, 1 de large et 1 de profondeur. 
ensuite tout ce qu'on va donner au GPU vont être des Matrices qui vont être appliquée à ce cube. 
La matrice en l'occurence est calculée par rapport au coordonates 

## TERMES MATHEMATIQUES
Matrice locale: transformation d'un membre calculer seulement par rapport à son parent. 
Matrice monde: où est postiionnée de manière absolue le membre dans l'espace 3d, toujours par rapport au point d'origine (0, 0, 0).
                -> c'est elle qu'on envoie au shader


## FONCTION DE GRAVITÉ (saut)

Formule de la hauteur en fonction du temps :

```
y(t) = y₀ + v₀ · t_saut − ½ · g · t_saut²
```

| Symbole   | Signification                                      |
|-----------|----------------------------------------------------|
| `y₀`      | hauteur initiale au sol                            |
| `v₀`      | vitesse d'impulsion vers le haut (ex: 5 m/s)       |
| `g`       | gravité (ex: 9.81 m/s²)                            |
| `t_saut`  | chrono déclenché quand on presse la touche Saut     |

**Logique :**
1. Tant que `y(t) ≥ y₀` → `torso.localPosition.y = y(t)`
2. Dès que `y(t) ≤ y₀` → atterrissage :
   - `torso.localPosition.y = y₀`
   - `t_saut = 0`



## TRS

`T × R × S` signifie **Translation × Rotation × Scale** : c'est l'ordre mathématique strict dans lequel on multiplie les matrices pour transformer le cube unitaire `1 × 1 × 1` sans déformer son articulation.

### Pourquoi cet ordre exact ?

L'ordre des multiplications matricielles n'est pas commutatif (`A × B ≠ B × A`). Chaque fonction génère une grille `4 × 4` spécifique :

| Matrice | Fonction | Rôle |
|---------|----------|------|
| `S` (Scale) | `Matrix4x4_Scale(scale)` | Place les dimensions `(sx, sy, sz)` sur la diagonale. Étire d'abord le cube `1 × 1 × 1` pour lui donner sa forme de membre (ex: long et fin). |
| `R` (Rotation) | `Matrix4x4_RotationXYZ(rot)` | Remplit la matrice avec des sinus et cosinus (`cos θ`, `sin θ`). Fait pivoter le membre déjà étiré autour de son articulation. |
| `T` (Translation) | `Matrix4x4_Translation(pos)` | Place le décalage `(tx, ty, tz)` dans la 4ème colonne. Décale le membre orienté vers son point d'attache sur le parent. |

### Application sur un sommet

```
M_locale = T × R × S
V'       = T × (R × (S × V))
```

Le vecteur d'un sommet `V` subit les transformations **de droite à gauche** : le cube est d'abord dimensionné, puis tourné, puis déplacé.

Si on inverse l'ordre (ex: `S × R × T`), la rotation fait tourner le membre en grand arc autour du parent au lieu de pivoter sur son articulation (coude, genou, etc.).


## Articulations & coordonnée homogène `w`

### Représentation de l'articulation

Aucun composant spécifique : l'articulation correspond simplement à l'origine locale `(0, 0, 0)` du `Node`.

| Champ            | Rôle |
|------------------|------|
| `localPosition`  | Définit l'emplacement de l'ancrage (ex: le coude positionné sous l'épaule) |
| `localRotation`  | Fait pivoter le cube autour de ce point d'ancrage (comme une porte sur ses gonds) |

### Invariance de `w = 1`

| Valeur    | Type                  | Effet |
|-----------|-----------------------|-------|
| `w = 1`   | Point                 | Active la translation dans le calcul matriciel (`t × 1 = t`) |
| `w = 0`   | Vecteur de direction  | Désactive la translation (`t × 0 = 0`) |

**Règle :** `w` reste égal à `1` en permanence pour l'ensemble des sommets du personnage, y compris à l'arrêt (`IDLE`).

---



questions en vrac / à se familiariser:
- pourquoi 4x4 ? et pas 3x3 -> solution: revoir les calculs de matrices
vidéo youtube qui explique très bien pourquoi on utilise des matrices 4x4 en rendering 3D: 
https://www.youtube.com/watch?v=Do_vEjd6gF0


-> on pourrait faire la rotation sans la 4ème valeur mais

- matrices trigonométriques -> axe de la marche x par example
- matrix stack

## Concepts mathématiques à digérer

### Vecteurs 4D

On représente un point sous la forme `(x, y, z, w)`.  
Le `w` est une astuce mathématique (**coordonnées homogènes**) qui permet d'exprimer les translations avec des matrices.

### Matrices 4x4

Tout déplacement en 3D se résume à multiplier un vecteur par une matrice `4 × 4`.

### Multiplication de matrices

Ce n'est pas commutatif : `A × B ≠ B × A`.  
L'ordre dans lequel on multiplie les matrices de rotation et de translation change complètement le résultat.

### Trigonométrie (`cos` / `sin`)

Utilisée pour :
- créer les matrices de rotation (autour des axes X, Y ou Z)
- faire osciller les membres en fonction du temps pour l'animation  
  (ex: `sin(temps)` pour une démarche fluide)

## Checklist — concepts indispensables pour la soutenance

1. **Interdiction des fonctions matricielles natives**  
   Pouvoir prouver au correcteur qu'on n'utilise ni `glRotatef` / `glTranslatef`, ni les utilitaires matriciels de bibliothèques tierces. Toutes les grilles `4 × 4` sont générées par nos propres fonctions mathématiques.

2. **Ordre TRS (`T × R × S`)**  
   Savoir expliquer pourquoi l'échelle s'applique en premier, la rotation en deuxième, et la translation en dernier — et rappeler que la multiplication matricielle n'est pas commutative (`A × B ≠ B × A`).

3. **Coordonnées homogènes (`w = 1`)**  
   Expliquer pourquoi l'espace 3D requiert un vecteur à 4 composantes et une matrice `4 × 4` pour intégrer le déplacement (translation) dans une multiplication.

4. **Mécanisme de la MatrixStack**  
   Être capable de schématiser sur papier le déroulement des appels `Push()` et `Pop()` : passage du Torse → Bras → Avant-bras, puis retour vers la Jambe.

5. **Rendu à forme unique**  
   Montrer dans le code que la commande de dessin (`glDrawArrays` / `glDrawElements`) pointe systématiquement sur le même buffer de cube `1 × 1 × 1`.

6. **Modification dynamique des nœuds**  
   Démontrer en direct que si on allonge la taille (`scale`) d'un membre parent, ses enfants restent correctement articulés au bout de celui-ci, sans se détacher.

7. **Machine à états d'animation**  
   Expliquer comment la variable temporelle `t` pilote les équations de mouvement différemment selon le mode actif (`WALK`, `JUMP`, `IDLE`).


---

## Tâches C# — côté rendu

1. **Fenêtrage et contexte OpenGL**  
   Instancier la fenêtre d'affichage (via Silk.NET ou OpenTK) et intercepter les événements clavier.

2. **VBO / VAO du cube unique**  
   Définir les sommets d'un unique cube `1 × 1 × 1` centré en zéro, transférer ce tableau une seule fois dans la VRAM au démarrage, et créer le Vertex Array Object.

3. **Compilateur de shaders**  
   Écrire et compiler le Vertex Shader (réception de `P × V × M` et calcul de la position) et le Fragment Shader (coloration élémentaire des faces).

4. **Liaison des uniforms**  
   Exposer une méthode permettant d'injecter la matrice du membre (`u_ModelMatrix`) dans le shader à chaque draw call.

5. **Caméra 3D (View & Projection)**  
   Générer les matrices de projection perspective (FOV, aspect ratio) et de vue (`LookAt`) pour pouvoir tourner autour du personnage à la souris ou au clavier.
