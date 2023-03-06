# Commande

---

> **DÉFINITION** :  
> La Commande est un patron de conception comportemental qui convertit des demandes ou des traitements simples en objets.

Appliqué au jeu vidéo, ce patron permet de conceptualiser des tâches avec début et fin afin de les traiter en FIFO. 
Jason Weimann donne [ici](https://youtu.be/hQE8lQk9ikE?t=592) un très bon exemple d'utilisation.

Créez vos commandes en héritant de [`Command`](/api/GGL.Commands.Command.html) et exécutez les en héritant de [`CommandProcessor<T>`](/api/GGL.Commands.CommandProcessor-1.html). Cette dernière permet de:
- définir les priorités de vos commandes ;
- exécuter automatiquement les commandes les unes <u>après</u> les autres ;
- exécuter une commande immédiatement.