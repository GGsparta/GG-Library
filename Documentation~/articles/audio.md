# Audio

---

Cette bibliothèque inclut `GG-AudioProfile` : un **profil audio préconfiguré** avec un channel d'effet sonore et un channel pour les musiques. Vous pouvez cependant utiliser un autre profil si vous le souhaitez: chaque module est entièrement reconfigurable !

## Configuration

Assurez vous d"avoir un `AudioListener` dans la scène. Pour chaque `AudioSource`, choisissez un `AudioMixerGroup` qui correspond:

![Audio groups included](/images/audio_0.jpg)

Pour les boutons d'interface utilisateur, ajoutez simplement [`ButtonAudio`](/api/GGL.Audio.ButtonAudio.html) pour simplifier le processus.

## Interaction

Vous pouvez modifier le son de l'un des channels avec [`VolumeSlider`](/api/GGL.Audio.VolumeSlider.html). Ajoutez ce composant, choisissez le channel... et c'est bon ! Ce composant s'occupera de modifier, sauvegarder et charger le volume.

![Audio slider example](/images/audio_1.jpg)

> [!NOTE]
> - Si vous n'utilisez pas un slider pour modifier votre volume, vous pouvez créer votre propre binder en héritant [`VolumeBinder`](/api/GGL.Audio.VolumeBinder.html).
> - Les paramètres sonores sont chargés par votre binder. Si celui-ci est inactif, exécutez sa méthode `Init()` au démarrage de la scène.

## Optimisation

Dans une scène avec un petit millier de bulles à éclater, vous n'allez pas mettre un `AudioSource` à chacune d'entre elles... N'est-ce pas? 🤔

Faites appel au singleton [`SoundPlayer`](/api/GGL.Audio.Player.SoundPlayer.html) pour jouer votre son ! Le composant va se créer de lui-même s'il n'existe pas déjà, et jouera par défaut sur le channel correspondant du `GG-AudioProfile`. 

> [!NOTE]
> - Pour les musiques vous pouvez également utiliser [`MusicPlayer`](/api/GGL.Audio.Player.MusicPlayer.html) qui inclut les fondus _(eg. crossfade)_.
> - Vous pouvez personnaliser ces composants en les créant au préalable dans la scène initiale.
> - Créez votre propre player en héritant de [`AbstractPlayer`](/api/GGL.Audio.Player.AbstractPlayer-1.html) !