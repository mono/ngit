using NGit.Revwalk;
using Sharpen;

namespace NGit.Revwalk
{
	internal class BlockObjQueue
	{
		private BlockObjQueue.BlockFreeList free;

		private BlockObjQueue.Block head;

		private BlockObjQueue.Block tail;

		/// <summary>Create an empty queue.</summary>
		/// <remarks>Create an empty queue.</remarks>
		public BlockObjQueue()
		{
			free = new BlockObjQueue.BlockFreeList();
		}

		internal virtual void Add(RevObject c)
		{
			BlockObjQueue.Block b = tail;
			if (b == null)
			{
				b = free.NewBlock();
				b.Add(c);
				head = b;
				tail = b;
				return;
			}
			else
			{
				if (b.IsFull())
				{
					b = free.NewBlock();
					tail.next = b;
					tail = b;
				}
			}
			b.Add(c);
		}

		internal virtual RevObject Next()
		{
			BlockObjQueue.Block b = head;
			if (b == null)
			{
				return null;
			}
			RevObject c = b.Pop();
			if (b.IsEmpty())
			{
				head = b.next;
				if (head == null)
				{
					tail = null;
				}
				free.FreeBlock(b);
			}
			return c;
		}

		internal sealed class BlockFreeList
		{
			private BlockObjQueue.Block next;

			internal BlockObjQueue.Block NewBlock()
			{
				BlockObjQueue.Block b = next;
				if (b == null)
				{
					return new BlockObjQueue.Block();
				}
				next = b.next;
				b.Clear();
				return b;
			}

			internal void FreeBlock(BlockObjQueue.Block b)
			{
				b.next = next;
				next = b;
			}
		}

		internal sealed class Block
		{
			private const int BLOCK_SIZE = 256;

			/// <summary>Next block in our chain of blocks; null if we are the last.</summary>
			/// <remarks>Next block in our chain of blocks; null if we are the last.</remarks>
			internal BlockObjQueue.Block next;

			/// <summary>Our table of queued objects.</summary>
			/// <remarks>Our table of queued objects.</remarks>
			internal readonly RevObject[] objects = new RevObject[BLOCK_SIZE];

			/// <summary>
			/// Next valid entry in
			/// <see cref="objects">objects</see>
			/// .
			/// </summary>
			internal int headIndex;

			/// <summary>
			/// Next free entry in
			/// <see cref="objects">objects</see>
			/// for addition at.
			/// </summary>
			internal int tailIndex;

			internal bool IsFull()
			{
				return tailIndex == BLOCK_SIZE;
			}

			internal bool IsEmpty()
			{
				return headIndex == tailIndex;
			}

			internal void Add(RevObject c)
			{
				objects[tailIndex++] = c;
			}

			internal RevObject Pop()
			{
				return objects[headIndex++];
			}

			internal void Clear()
			{
				next = null;
				headIndex = 0;
				tailIndex = 0;
			}
		}
	}
}
