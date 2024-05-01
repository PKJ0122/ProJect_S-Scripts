using System;

public abstract class SingletonBase<T>
    where T : SingletonBase<T>
{
    private static T s_instance;
    public static T instance
    {
        get
        {
            if (s_instance == null)
            {
                /*ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[] { });
                constructorInfo.Invoke(null);*/

                s_instance = (T)Activator.CreateInstance(typeof(T));
            }

            return s_instance;
        }
    }
}

