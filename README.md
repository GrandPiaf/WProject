# WProject

Un document présentant Le groupe, les fonctionnalités implémentés, les difficultés rencontrées ainsi qu'un rétrospectif par rapport aux éléments prévus / réalisés.

## Membres du groupe

CALVET Nicolas
DAMBRAINE Jérôme
GUYAMIER Thomas

## Descriptif du jeu

L'utilisateur dispose de cartes formés de formes simples : Triangle, Rectangle et Cercle.
En montrant ces cartes à la caméra, le joueur a la possibilité de lancer des sorts sur l'ennemi présent en face du héro.

Le but du jeu est simple : vaincre le plus d'ennemis à la suite avant que le héro ne meurt. Chaque ennemi battu rapporte 1 point, augmentant le score du joueur.

Afin d'augmenter la difficulté, le joueur ne dispose que d'un temps limité pour lancer son attaque. S'il ne parvient pas à lancer une attaque à la fin du temps imparti (à la fin du tour) rien ne se passe, sauf pour l'ennemi qui va lancer un sort !.

A chaque mort d'un ennemi, le temps imparti pour chaque tour diminue; Le joueur se doit d'être de plus en plus rapide et efficace pour décider du sort et le placer correctement sous la caméra.

Chaque carte est unique, elles sont composés d'une combinaison de formes qui correspondent toutes à un sort unique et différent. L'ennemi dispose lui aussi des mêmes sorts que le joueur, choisi aléatoirement à chaque tour.
Par principe, toute carte formé d'un triangle représente des dégâts infligé à l'ennemi; toute carte formé d'un rectangle représente une défense et toute carte formé d'un cercle représente un soin au joueur.

Le joueur et l'ennemi disposent d'une barre de mana / d'énergie. Ils ne peuvent lancer un sort que s'ils disposent d'assez d'énergie.

Voici la liste des combinaisons et des sorts associés :

| **Sort**                          | **Effet**                                                 | Coût de mana |
| --------------------------------- | --------------------------------------------------------- | ------------ |
| Rectangle unique                  | Bouclier simple : bloque les attaques simples             | Normal : 20  |
| Triangle unique                   | Attaque simple : effectue des dégâts normaux              | Normal : 20  |
| Cercle unique                     | Soin : effectue des soins normaux                         | Normal : 20  |
| Rectangle <u>double</u>           | Fort bouclier : bloque tout                               | Elevé : 50   |
| Triangle <u>double</u>            | Forte attaque : effectue le double des dégâts             | Elevé : 50   |
| Cercle <u>double</u>              | Fort soin : effectue le double de soin                    | Elevé : 50   |
| Grand rectangle et petit triangle | Bouclier réflecteur : renvoie des dégâts                  | Normal : 20  |
| Grand rectangle et petit cercle   | Bouclier soigneur : bloque des attaques simples et soigne | Normal : 20  |
| Grand triangle et petit rectangle | Effectue des dégâts normaux et bloque des dégâts          | Normal : 20  |
| Grand triangle et petit cercle    | Vol de vie : dégâts normaux et soins normaux              | Normal : 20  |
| Grand cercle et petit rectangle   | Ajoute une armure au joueur                               | Normal : 20  |
| Grand cercle et petit triangle    | Sacrifie de la vie pour effectue des dégâts               | Normal : 20  |

Un sort double est un sort puissant. Seulement il coute bien plus de mana. Ils sont à utiliser avec précaution.



## Matériel et installation

Afin de jouer au jeu vous aurez besoin de :

- Une caméra branché à l'ordinateur
- Des cartes imprimés (trouvable dans le dossier Cartes)

Et il suffit de montrer les cartes à l'écran.

**Attention tout de même : il faut que l'éclairage soit le plus uniforme possible (pas de reflets, pas d'ombres sur les cartes).**
**Il faut que les cartes soient montrés à l'endroit (triangle pointant vers le haut)**



## Reconnaissance de formes

Dans ce projet nous utilisons la reconnaissance de formes pour détecter les cartes et donc le sorts utilisés par le joueur.

Nous avons alors créer un script de détection de formes qui applique l'algorithme suivant :

```
Détection des 4 marqueurs Aruco
Si et seulement si les 4 marqueurs sont détectés alors :
	Détection des formes
	Si une combinaison de forme et détecté alors :
		Retourner la combinaison trouvée
Sinon :
	Retourner aucune combinaison trouvée (NONE)
```

Pour la détection des marqueurs Aruco, nous avons utilisé la détection de marqueurs de la librairie EmguCV sur l'image récupéré de la caméra sans traitement.

De là, nous découpons l'image pour ne garder l'intérieur des marqueurs. Cela permet donc d'éviter des faux-positifs d'objets présent à l'image lors de la détection de formes.

Puis nous calculons l'image négative afin de ne pas détecter les bords de l'image comme un rectangle.
Et nous appliquons un seuillage dans le but d'augmenter le contraste des formes. Cela aide à la détection des contours que l'on effectue juste après. Après seuillage, l'image est transformée en niveau de gris.

Nous éliminons les contours trop petits. Pour cela, nous trions selon les aires.

Des contours détectés, nous calculons une formes polygonale et selon le nombre de côtés on en déduit la forme : Triangle, Rectangle ou Cercle.

Si nous avons détecté 2 formes, nous décidons laquelle est à l'intérieur de l'autre selon leur aire respectives.

Et nous pouvons enfin retourner la combinaison de formes récupérés.

Ce traitement réduit considérablement les FPS du jeu, mais il reste toutefois très lisible par sa simplicité d'affichage.



## Pistes d'amélioration et éléments prévus non réalisés.

### Eléments prévus :

Au départs, nous souhaitions que chaque forme puissent posséder différentes couleurs. Ces couleurs devaient représenter des éléments (mécanisme classique d'un jeu vidéo) tels que l'eau, la terre, le feu et l'air. Chaque élément était plus puissant et moins puissant qu'un autre, impliquant au joueur de choisir aussi les bonnes combinaisons de couleurs.

Seulement cela nous posait 2 difficultés :

- Une trop grande complexité dans le gameplay. En effet, avec seulement 3 éléments nous aurions eu 36 cartes à gérer au lieu des 12 actuelles.
- Mais aussi, notre méthodologie ne peut réellement détecter les couleurs. En effet, nous transformons l'image en niveau de gris et nous appliquons un seuillage, ce qui complexifie la détection de la couleur d'une forme.

### Pistes d'améliorations

- Les reflets de lumière et les ombres peuvent empêcher dans certains cas la détection des marqueurs et des formes. Nous pourrions ajouter des traitements afin de régler ce problème.
- Ajouter des couleurs représentant des éléments (voir ci-dessus).
- Et en termes de gameplay : approfondir l'intelligence des ennemis en leur donnant des *patterns*, des *boss*, d'autres récompenses, voir même des temps de repos et des zones d'amélioration d'équipement.

**En effet, on peut voir en ce projet les bases d'une détection de cartes à jouer : technique pouvant être appliqué à pleins de jeu !**

