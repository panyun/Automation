using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL
{
	public class UnOrderMultiMap<T, K> : Dictionary<T, List<K>>
	{
		public void Add(T t, K k)
		{
			List<K> list;
			this.TryGetValue(t, out list);
			if (list == null)
			{
				list = new List<K>();
				base[t] = list;
			}
			list.Add(k);
		}

		public bool Remove(T t, K k)
		{
			List<K> list;
			this.TryGetValue(t, out list);
			if (list == null)
			{
				return false;
			}
			if (!list.Remove(k))
			{
				return false;
			}
			if (list.Count == 0)
			{
				this.Remove(t);
			}
			return true;
		}

		/// <summary>
		/// 不返回内部的list,copy一份出来
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public K[] GetAll(T t)
		{
			List<K> list;
			this.TryGetValue(t, out list);
			if (list == null)
			{
				return Array.Empty<K>();
			}
			return list.ToArray();
		}

		/// <summary>
		/// 返回内部的list
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public new List<K> this[T t]
		{
			get
			{
				List<K> list;
				this.TryGetValue(t, out list);
				return list;
			}
		}

		public K GetOne(T t)
		{
			List<K> list;
			this.TryGetValue(t, out list);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return default(K);
		}

		public bool Contains(T t, K k)
		{
			List<K> list;
			this.TryGetValue(t, out list);
			if (list == null)
			{
				return false;
			}
			return list.Contains(k);
		}
	}
	public class UnOrderMultiMapSet<T, K> : Dictionary<T, HashSet<K>>
	{
		// 重用HashSet
		public new HashSet<K> this[T t]
		{
			get
			{
				HashSet<K> set;
				if (!this.TryGetValue(t, out set))
				{
					set = new HashSet<K>();
				}
				return set;
			}
		}

		public Dictionary<T, HashSet<K>> GetDictionary()
		{
			return this;
		}

		public void Add(T t, K k)
		{
			HashSet<K> set;
			this.TryGetValue(t, out set);
			if (set == null)
			{
				set = new HashSet<K>();
				base[t] = set;
			}
			set.Add(k);
		}

		public bool Remove(T t, K k)
		{
			HashSet<K> set;
			this.TryGetValue(t, out set);
			if (set == null)
			{
				return false;
			}
			if (!set.Remove(k))
			{
				return false;
			}
			if (set.Count == 0)
			{
				this.Remove(t);
			}
			return true;
		}

		public bool Contains(T t, K k)
		{
			HashSet<K> set;
			this.TryGetValue(t, out set);
			if (set == null)
			{
				return false;
			}
			return set.Contains(k);
		}

		public new int Count
		{
			get
			{
				int count = 0;
				foreach (KeyValuePair<T, HashSet<K>> kv in this)
				{
					count += kv.Value.Count;
				}
				return count;
			}
		}
	}
}
