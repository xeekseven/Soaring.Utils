namespace Soaring.Utils
{
    public class SingletonObject<T> where T : class, new()
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                }
                return instance;
            }
        }
    }
}