# Déformation avant-bras / tibia : scale non uniforme + rotation

## Symptôme (avant le fix)

Quand on pliait le coude ou le genou, le membre enfant (avant-bras, tibia) ne
restait pas un pavé : le bas paraissait **beaucoup plus large**, cube en
trapèze / losange.

Ce n’était **pas** (surtout) la perspective caméra. En vue ¾ douce, un vrai pavé
garde à peu près la même épaisseur sur toute sa longueur.

## Cause

Chaque nœud poussait sur la pile un simple :

```text
T × R × S
```

Le scale `S` est **dans** la pile → les enfants héritent de la taille du parent
(nécessaire pour le resize du sujet).

Ordre sur un point `p` de l’avant-bras (lecture droite → gauche) :

```text
p' = … × S_bras × T_avantBras × R_coude × S_avantBras × T_pivot × p
```

Le **S du bras** (non uniforme : fin en X/Z, long en Y) s’appliquait **après**
la rotation du coude → **cisaillement**. Même chose cuisse → tibia.

Tant que `R = 0`, pas de déformation visible. Dès qu’on plie → le bout s’évase.

---

## Exemple concret (sans compensation)

| | Valeur |
|---|---|
| Scale bras | `S_bras = (0.3, 1.0, 0.3)` |
| Position avant-bras | `T = (0, -1, 0)` (espace unitaire) |
| Rotation coude | `R_x = 90°` |
| Point local | `p = (0.5, -0.5, 0)` |

```text
p' = S_bras × T × R_x(90°) × p
```

1. `R` tourne le “bas” du cube vers l’avant.
2. `T` place le membre au bout du bras.
3. `S_bras` écrase X/Z par `0.3` **après** la rotation → silhouette en trapèze.

---

## Solution implémentée (à relire plus tard)

### Idée

Garder le **S dans la pile** (resize : grossir un membre affecte ses enfants),
mais faire tourner chaque membre dans un espace **sans** le squash des ancêtres.

Sandwich sur **chaque** nœud :

```text
T × S_accum⁻¹ × R × S_accum × S_local
```

- `S_accum` = produit des `LocalScale` des **ancêtres** (`(1,1,1)` à la racine).
- `S⁻¹ … S` autour de `R` : annule le cisaillement, **réapplique** la taille
  héritée (sinon le membre devient trop petit = seulement `S_local`).
- `S_local` reste le scale du nœud courant → toujours poussé dans la pile.

Chaîne typique avant-bras :

```text
p' = … × S_accum × T × S_accum⁻¹ × R × S_accum × S_local × p
```

- `T` unitaire `(0,-1,0)` profite toujours du S parent → l’avant-bras reste au
  bout quand on resize le bras (`ScaleMember` + anchors unitaires).
- Après simplification linéaire : rotations puis échelle “propre” → pavé OK.

### Pourquoi `S_accum` (produit des ancêtres) et pas seulement le parent ?

Le torse a aussi un scale non uniforme `(1.4, 1.5, 0.75)`. Si on ne compense
que le bras, le squash du torse peut encore cisailer l’avant-bras après le
coude. On accumule donc **tous** les scales le long de la chaîne.

Exemple :

```text
racine     → parentAccum = (1, 1, 1)
torse      → enfants reçoivent LocalScale du torse
bras       → enfants reçoivent (torso.X×arm.X, torso.Y×arm.Y, torso.Z×arm.Z)
avant-bras → utilise ce produit comme S_accum dans le sandwich
```

### Où c’est dans le code

| Fichier | Rôle |
|---|---|
| `Mathematics/Matrix4x4.cs` | `InverseScale` = `Scale(1/sx, 1/sy, 1/sz)` (+ garde-fou ~0) |
| `Models/SceneNode.cs` | `GetStackMatrix(parentAccumScale)` = le sandwich |
| `Models/SceneNode.cs` | `AccumScaleForChildren` = produit pour la récursion |
| `HumanGLWindow.cs` | `RenderNode(node, parentAccumScale)` passe l’accum aux enfants |

Appel racine :

```csharp
RenderNode(_character, new Vector3(1f, 1f, 1f));
```

`GetStackMatrix` :

```csharp
return Translation(LocalPosition)
     * InverseScale(parentAccumScale)
     * RotationXYZ(LocalRotation)
     * Scale(parentAccumScale)
     * Scale(LocalScale);
```

Schéma pile (simplifié) :

```text
RenderNode(torse, (1,1,1))
  Push
  Multiply( T × I × R × I × S_torse )
  Draw(× pivot)
  childAccum = S_torse
  RenderNode(bras, S_torse)
    Push
    Multiply( T × S_torse⁻¹ × R × S_torse × S_bras )
    Draw(× pivot)
    childAccum = S_torse × S_bras   (composante par composante)
    RenderNode(avantBras, childAccum)
      Push
      Multiply( T × (S_t×S_b)⁻¹ × R × (S_t×S_b) × S_avantBras )
      Draw(× pivot)
      Pop
    Pop
  Pop
```

Le pivot géométrie reste **uniquement au Draw** (`× T_pivot`), pas dans la
matrice poussée pour les enfants.

### Ce qu’on a volontairement gardé

- Scale **dans** la stack → contrainte sujet “related parts reposition”.
- Anchors unitaires (`RefreshUnitAnchors`, ex. avant-bras à `(0,-1,0)`).
- `ScaleMember` inchangé côté API.

### Pièges (si ça casse un jour)

1. Oublier de **réappliquer** `Scale(S_accum)` après `R` → membres trop petits.
2. Compenser seulement le parent direct, pas le **produit** des ancêtres.
3. Mettre `S⁻¹` au mauvais endroit (doit être après `T`, avant `R`).
4. Remettre le pivot dans la pile → les enfants se décrochent.

### Alternatives qu’on n’a pas prises

| Approche | Pourquoi pas |
|---|---|
| S seulement au Draw | casse le resize auto via la pile |
| Scales uniformes partout | plus de bras “bâton” avec un cube 1×1×1 |
| Recalcul manuel des anchors | “triche” / plus fragile au resize |

---

## Lien avec le sujet

Obligatoire : pile, hiérarchie, resize qui repositionne les parties liées.
Le sujet n’exige pas des cubes déformés. Cette solution = S dans la pile
**et** articulation propre.
