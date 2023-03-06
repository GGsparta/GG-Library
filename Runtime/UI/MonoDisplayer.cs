using GGL.Observer;
using UnityEngine;

namespace GGL.UI
{
    /// <summary>
    /// Contains a data and refresh the display on data change.
    /// </summary>
    /// <typeparam name="T">Your data</typeparam>
    [HelpURL(Settings.ARTICLE_URL + "ui" + Settings.COMMON_EXT + "#afficher-une-donnée")]
    public abstract class MonoDisplayer<T> : MonoBehaviour
    {
        /// <value>
        /// Current data to store
        /// </value>
        public Observable<T> Data { get; private set; } = new();

        [ExcludeFromDocFx]
        private void Awake() => Data.OnChange += Refresh;
        [ExcludeFromDocFx]
        private void OnDestroy() => Data.OnChange -= Refresh;

        /// <summary>
        /// Refresh the display of the data.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected abstract void Refresh(T oldValue, T newValue);

        /// <summary>
        /// Refresh the display of the data.
        /// </summary>
        public void Refresh() => Refresh(Data, Data);
        
        /// <summary>
        /// If you want to define the <see cref="Observable{T}"/> data yourself, set it here.
        /// </summary>
        /// <param name="data"></param>
        public void ListenTo(Observable<T> data)
        {
            Data.OnChange -= Refresh;
            Data = data;
            Data.OnChange += Refresh;
        }
    }
}