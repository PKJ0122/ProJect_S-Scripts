using UnityEngine;


public abstract class SingletonMonoBase<T> : MonoBehaviour
        where T : SingletonMonoBase<T>
{
    private static T s_instance;
    public static T instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }

            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = (T)this;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }
}

