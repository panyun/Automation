using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
    public class ComponentQueue : ELObject
    {
        public string TypeName
        {
            get;
        }

        private readonly Queue<ELObject> queue = new Queue<ELObject>();

        public ComponentQueue(string typeName)
        {
            this.TypeName = typeName;
        }

        public void Enqueue(ELObject entity)
        {
            this.queue.Enqueue(entity);
        }

        public ELObject Dequeue()
        {
            return this.queue.Dequeue();
        }

        public ELObject Peek()
        {
            return this.queue.Peek();
        }

        public Queue<ELObject> Queue => this.queue;

        public int Count => this.queue.Count;

        public override void Dispose()
        {
            while (this.queue.Count > 0)
            {
                ELObject component = this.queue.Dequeue();
                component.Dispose();
            }
        }
    }

    public class ObjectPool : ELObject
    {
        private static ObjectPool instance;

        public static ObjectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ObjectPool();
                }

                return instance;
            }
        }

        private readonly Dictionary<Type, ComponentQueue> dictionary = new Dictionary<Type, ComponentQueue>();

        public ELObject Fetch(Type type)
        {
            ELObject obj;
            if (!this.dictionary.TryGetValue(type, out ComponentQueue queue))
            {
                obj = (ELObject)Activator.CreateInstance(type);
            }
            else if (queue.Count == 0)
            {
                obj = (ELObject)Activator.CreateInstance(type);
            }
            else
            {
                obj = queue.Dequeue();
            }

            return obj;
        }

        public T Fetch<T>() where T : ELObject
        {
            T t = (T)this.Fetch(typeof(T));
            return t;
        }

        public void Recycle(ELObject obj)
        {
            Type type = obj.GetType();
            ComponentQueue queue;
            if (!this.dictionary.TryGetValue(type, out queue))
            {
                queue = new ComponentQueue(type.Name);
                this.dictionary.Add(type, queue);
            }
            queue.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (KeyValuePair<Type, ComponentQueue> kv in this.dictionary)
            {
                kv.Value.Dispose();
            }

            this.dictionary.Clear();
        }

        public override void Dispose()
        {
            foreach (KeyValuePair<Type, ComponentQueue> kv in this.dictionary)
            {
                kv.Value.Dispose();
            }

            this.dictionary.Clear();
            instance = null;
        }
    }
    public class ListComponent<T> : ELObject
    {
        private bool isDispose;

        public static ListComponent<T> Create()
        {
            ListComponent<T> listComponent = ObjectPool.Instance.Fetch<ListComponent<T>>();
            listComponent.isDispose = false;
            return listComponent;
        }

        public List<T> List { get; } = new List<T>();

        public override void Dispose()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;

            base.Dispose();

            this.List.Clear();
            ObjectPool.Instance.Recycle(this);
        }
    }

    public class ListComponentDisposeChildren<T> : ELObject where T : ELObject
    {
        private bool isDispose;

        public static ListComponentDisposeChildren<T> Create()
        {
            ListComponentDisposeChildren<T> listComponent = ObjectPool.Instance.Fetch<ListComponentDisposeChildren<T>>();
            listComponent.isDispose = false;
            return listComponent;
        }

        public List<T> List = new List<T>();

        public override void Dispose()
        {
            if (this.isDispose)
            {
                return;
            }

            this.isDispose = true;

            base.Dispose();

            foreach (T entity in this.List)
            {
                entity.Dispose();
            }

            this.List.Clear();

            ObjectPool.Instance.Recycle(this);
        }
    }
}
