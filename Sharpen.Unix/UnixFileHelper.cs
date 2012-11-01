using System;
using System.IO;
using Mono.Unix;

namespace Sharpen.Unix
{
	class UnixFileHelper : FileHelper
	{
		static UnixFileSystemInfo GetUnixFileInfo (string path)
		{
			try {
				return Mono.Unix.UnixFileInfo.GetFileSystemEntry (path);
			} catch (DirectoryNotFoundException ex) {
				// If we have a file /foo/bar and probe the path /foo/bar/baz, we get a DirectoryNotFound exception
				// because 'bar' is a file and therefore 'baz' cannot possibly exist. This is annoying.
				var inner = ex.InnerException as UnixIOException;
				if (inner != null && inner.ErrorCode == Mono.Unix.Native.Errno.ENOTDIR)
					return null;
				throw;
			}
		}
		
		public override bool CanExecute (FilePath path)
		{
			UnixFileInfo fi = new UnixFileInfo (path);
			if (!fi.Exists)
				return false;
			return 0 != (fi.FileAccessPermissions & (FileAccessPermissions.UserExecute | FileAccessPermissions.GroupExecute | FileAccessPermissions.OtherExecute));
		}
		
		public override bool CanWrite (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.CanAccess (Mono.Unix.Native.AccessModes.W_OK);
		}
		
		public override bool Delete (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			if (info != null && info.Exists) {
				try {
					info.Delete ();
					return true;
				} catch {
					// If the directory is not empty we return false. JGit relies on this
					return false;
				}
			}
			return false;
		}
		
		public override bool Exists (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists;
		}
		
		public override bool IsDirectory (FilePath path)
		{
			try {
				var info = GetUnixFileInfo (path);
				return info != null && info.Exists && info.FileType == FileTypes.Directory;
			} catch (DirectoryNotFoundException) {
				// If the file /foo/bar exists and we query to see if /foo/bar/baz exists, we get a
				// DirectoryNotFound exception for Mono.Unix. In this case the directory definitely
				// does not exist.
				return false;
			}
		}
		
		public override bool IsFile (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists && (info.FileType == FileTypes.RegularFile || info.FileType == FileTypes.SymbolicLink);
		}
		
		public override long LastModified (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
		}
		
		public override long Length (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			return info != null && info.Exists ? info.Length : 0;
		}
		
		public override void MakeFileWritable (FilePath file)
		{
			var info = GetUnixFileInfo (file);
			if (info != null)
				info.FileAccessPermissions |= (FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
		}
		
		public override bool RenameTo (FilePath path, string name)
		{
			var symlink = GetUnixFileInfo (path) as UnixSymbolicLinkInfo;
			if (symlink != null) {
				var newFile = new UnixSymbolicLinkInfo (name);
				newFile.CreateSymbolicLinkTo (symlink.ContentsPath);
				return true;
			} else {
				// This call replaces the file if it already exists.
				// File.Move throws an exception if dest already exists
				return Mono.Unix.Native.Stdlib.rename (path, name) == 0;
			}
		}
		
		public override bool SetExecutable (FilePath path, bool exec)
		{
			UnixFileInfo fi = new UnixFileInfo (path);
			FileAccessPermissions perms = fi.FileAccessPermissions;
			if (exec) {
				if (perms.HasFlag (FileAccessPermissions.UserRead))
					perms |= FileAccessPermissions.UserExecute;
				if (perms.HasFlag (FileAccessPermissions.OtherRead))
					perms |= FileAccessPermissions.OtherExecute;
				if ((perms.HasFlag (FileAccessPermissions.GroupRead)))
					perms |= FileAccessPermissions.GroupExecute;
			} else {
				if (perms.HasFlag (FileAccessPermissions.UserRead))
					perms &= ~FileAccessPermissions.UserExecute;
				if (perms.HasFlag (FileAccessPermissions.OtherRead))
					perms &= ~FileAccessPermissions.OtherExecute;
				if ((perms.HasFlag (FileAccessPermissions.GroupRead)))
					perms &= ~FileAccessPermissions.GroupExecute;
			}
			fi.FileAccessPermissions = perms;
			return true;
		}
		
		public override bool SetLastModified(FilePath path, long milis)
		{
			// How can the last write time be set on a symlink?
			return base.SetLastModified(path, milis);
		}
		
		public override bool SetReadOnly (FilePath path)
		{
			try {
				var info = GetUnixFileInfo (path);
				if (info != null)
					info.FileAccessPermissions &= ~ (FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
				return true;
			} catch {
				return false;
			}
		}
	}
}

