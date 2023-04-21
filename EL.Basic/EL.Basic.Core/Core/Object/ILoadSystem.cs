using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
	public interface ISerializeToEntity
	{
	}
	public interface ILoadSystem : ISystemType
	{
		void Run(object o);
	}

	[ObjectSystem]
	public abstract class LoadSystem<T> : ILoadSystem
	{
		public void Run(object o)
		{
			this.Load((T)o);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(ILoadSystem);
		}

		public abstract void Load(T self);
	}
	public interface IUpdateSystem : ISystemType
	{
		void Run(object o);
	}

	[ObjectSystem]
	public abstract class UpdateSystem<T> : IUpdateSystem
	{
		public void Run(object o)
		{
			this.Update((T)o);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(IUpdateSystem);
		}

		public abstract void Update(T self);
	}
	public interface ILateUpdateSystem : ISystemType
	{
		void Run(object o);
	}

	[ObjectSystem]
	public abstract class LateUpdateSystem<T> : ILateUpdateSystem
	{
		public void Run(object o)
		{
			this.LateUpdate((T)o);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(ILateUpdateSystem);
		}

		public abstract void LateUpdate(T self);
	}
	public interface IDeserializeSystem : ISystemType
	{
		void Run(object o);
	}

	/// <summary>
	/// 反序列化后执行的System
	/// 要小心使用这个System，因为对象假如要保存到数据库，到dbserver也会进行反序列化，那么也会执行该System
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ObjectSystem]
	public abstract class DeserializeSystem<T> : IDeserializeSystem
	{
		public void Run(object o)
		{
			this.Deserialize((T)o);
		}

		public Type SystemType()
		{
			return typeof(IDeserializeSystem);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public abstract void Deserialize(T self);
	}
	public interface IAwakeSystem : ISystemType
	{
		void Run(object o);
	}

	public interface IAwakeSystem<A> : ISystemType
	{
		void Run(object o, A a);
	}

	public interface IAwakeSystem<A, B> : ISystemType
	{
		void Run(object o, A a, B b);
	}

	public interface IAwakeSystem<A, B, C> : ISystemType
	{
		void Run(object o, A a, B b, C c);
	}

	public interface IAwakeSystem<A, B, C, D> : ISystemType
	{
		void Run(object o, A a, B b, C c, D d);
	}

	[ObjectSystem]
	public abstract class AwakeSystem<T> : IAwakeSystem
	{
		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(IAwakeSystem);
		}

		public void Run(object o)
		{
			this.Awake((T)o);
		}

		public abstract void Awake(T self);
	}

	[ObjectSystem]
	public abstract class AwakeSystem<T, A> : IAwakeSystem<A>
	{
		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(IAwakeSystem<A>);
		}

		public void Run(object o, A a)
		{
			this.Awake((T)o, a);
		}

		public abstract void Awake(T self, A a);
	}

	[ObjectSystem]
	public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B>
	{
		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(IAwakeSystem<A, B>);
		}

		public void Run(object o, A a, B b)
		{
			this.Awake((T)o, a, b);
		}

		public abstract void Awake(T self, A a, B b);
	}

	[ObjectSystem]
	public abstract class AwakeSystem<T, A, B, C> : IAwakeSystem<A, B, C>
	{
		public Type Type()
		{
			return typeof(T);
		}

		public Type SystemType()
		{
			return typeof(IAwakeSystem<A, B, C>);
		}

		public void Run(object o, A a, B b, C c)
		{
			this.Awake((T)o, a, b, c);
		}

		public abstract void Awake(T self, A a, B b, C c);
	}
	public static class ObjectHelper
	{
		public static void Swap<T>(ref T t1, ref T t2)
		{
			(t1, t2) = (t2, t1);
		}
	}
	public interface IDestroySystem : ISystemType
	{
		void Run(object o);
	}

	[ObjectSystem]
	public abstract class DestroySystem<T> : IDestroySystem
	{
		public void Run(object o)
		{
			this.Destroy((T)o);
		}

		public Type SystemType()
		{
			return typeof(IDestroySystem);
		}

		public Type Type()
		{
			return typeof(T);
		}

		public abstract void Destroy(T self);
	}

}
