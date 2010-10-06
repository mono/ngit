using NGit;
using NGit.Storage.File;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// Updates any reference stored by
	/// <see cref="RefDirectory">RefDirectory</see>
	/// .
	/// </summary>
	internal class RefDirectoryUpdate : RefUpdate
	{
		private readonly RefDirectory database;

		private LockFile Lock;

		internal RefDirectoryUpdate(RefDirectory r, Ref @ref) : base(@ref)
		{
			database = r;
		}

		protected internal override RefDatabase GetRefDatabase()
		{
			return database;
		}

		protected internal override Repository GetRepository()
		{
			return database.GetRepository();
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override bool TryLock(bool deref)
		{
			Ref dst = GetRef();
			if (deref)
			{
				dst = dst.GetLeaf();
			}
			string name = dst.GetName();
			Lock = new LockFile(database.FileFor(name), GetRepository().FileSystem);
			if (Lock.Lock())
			{
				dst = database.GetRef(name);
				SetOldObjectId(dst != null ? dst.GetObjectId() : null);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected internal override void Unlock()
		{
			if (Lock != null)
			{
				Lock.Unlock();
				Lock = null;
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override RefUpdate.Result DoUpdate(RefUpdate.Result status)
		{
			Lock.SetNeedStatInformation(true);
			Lock.Write(GetNewObjectId());
			string msg = GetRefLogMessage();
			if (msg != null)
			{
				if (IsRefLogIncludingResult())
				{
					string strResult = ToResultString(status);
					if (strResult != null)
					{
						if (msg.Length > 0)
						{
							msg = msg + ": " + strResult;
						}
						else
						{
							msg = strResult;
						}
					}
				}
				database.Log(this, msg, true);
			}
			if (!Lock.Commit())
			{
				return RefUpdate.Result.LOCK_FAILURE;
			}
			database.Stored(this, Lock.GetCommitLastModified());
			return status;
		}

		private string ToResultString(RefUpdate.Result status)
		{
			switch (status)
			{
				case RefUpdate.Result.FORCED:
				{
					return "forced-update";
				}

				case RefUpdate.Result.FAST_FORWARD:
				{
					return "fast forward";
				}

				case RefUpdate.Result.NEW:
				{
					return "created";
				}

				default:
				{
					return null;
					break;
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override RefUpdate.Result DoDelete(RefUpdate.Result status)
		{
			if (GetRef().GetLeaf().GetStorage() != RefStorage.NEW)
			{
				database.Delete(this);
			}
			return status;
		}

		/// <exception cref="System.IO.IOException"></exception>
		protected internal override RefUpdate.Result DoLink(string target)
		{
			Lock.SetNeedStatInformation(true);
			Lock.Write(Constants.Encode(RefDirectory.SYMREF + target + '\n'));
			string msg = GetRefLogMessage();
			if (msg != null)
			{
				database.Log(this, msg, false);
			}
			if (!Lock.Commit())
			{
				return RefUpdate.Result.LOCK_FAILURE;
			}
			database.StoredSymbolicRef(this, Lock.GetCommitLastModified(), target);
			if (GetRef().GetStorage() == RefStorage.NEW)
			{
				return RefUpdate.Result.NEW;
			}
			return RefUpdate.Result.FORCED;
		}
	}
}
