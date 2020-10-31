using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Caching;


namespace LRUGenericCache
{
	public class CacheItemNode: CacheItem
	{
		public CacheItemNode Previous { get; set; }
		public CacheItemNode Next { get; set; }

		public CacheItemNode(string key, object value): base(key, value){
			Previous = null;
			Next = null;
        }
	}

	public class LRUQueue
	{
		private CacheItemNode Head;
		private CacheItemNode Tail;

		public LRUQueue()
		{
			Head = new CacheItemNode(default(String), null);
			Tail = new CacheItemNode(default(String), null);
			Head.Next = Tail;
			Tail.Previous = Head;
		}

		public void MoveToFront(CacheItemNode node)
        {
            if (node.Next != null && node.Previous != null)
            {
				node.Previous.Next = node.Next;
				node.Next.Previous = node.Previous;
			}
			node.Next = Head.Next;
			Head.Next.Previous = node;
			node.Previous = Head;
			Head.Next = node;
		}

		public CacheItemNode Remove(CacheItemNode node)
		{
			if (node.Next != null && node.Previous != null)
			{
				node.Previous.Next = node.Next;
				node.Next.Previous = node.Previous;
			}
			node.Next = null;
			node.Previous = null;
			return node;
		}

		public CacheItemNode RemoveLast()
        {
			CacheItemNode node = Tail.Previous;
			Tail.Previous = node.Previous;
			node.Previous.Next = Tail;
			node.Next = null;
			node.Previous = null;
			return node;
		}

		public string QueueState()
        {
			StringBuilder strb = new StringBuilder();
			CacheItemNode node = Head.Next;
			while(node.Next != null)
            {
				if (node != null)
                {
					strb.Append($"{node.Key}:{node.Value.ToString()}, ");
                }
				node = node.Next;
            }
			return strb.ToString();
        }
	}
}
