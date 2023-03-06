namespace GGL.Singleton
{
    /// <summary>
    /// Dummy creational design pattern that ensures only one object of its kind exists and provides a single point of access to it for any other code.
    /// </summary>
    /// <typeparam name="T">Non-behaviour class</typeparam>
    /// <remarks>Make sure your class has everything set up throught constructor. And please do not create instance by yourself...</remarks>
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static T _instance;
        
        /// <value>
        /// Access the instance. If it does not exist, it will be created.
        /// </value>
        public static T Instance => _instance == null ? _instance : _instance = new T();
    }
}