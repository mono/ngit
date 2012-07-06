using System;
using System.IO;
using Mono.Unix;

namespace Sharpen
{
	class FileHelper
	{
		public static FileHelper Instance {
			get; set;
		}
		
		static FileHelper ()
		{
			if (Environment.OSVersion.Platform.ToString ().StartsWith ("Win"))
				Instance = new FileHelper ();
			else
				Instance = new LinuxFileHelper ();
		}

		public virtual bool CanExecute (FilePath path)
		{
			return false;
		}

		public virtual bool CanWrite (FilePath path)
		{
			return ((File.GetAttributes (path) & FileAttributes.ReadOnly) == 0);
		}
		
		public virtual bool Delete (FilePath path)
		{
			if (Directory.Exists (path)) {
				if (Directory.GetFileSystemEntries (path).Length != 0)
					return false;
				MakeDirWritable (path);
				Directory.Delete (path, true);
				return true;
			}
			else if (File.Exists(path)) {
				MakeFileWritable (path);
				File.Delete (path);
				return true;
			}
			return false;
		}
		
		public virtual bool Exists (FilePath path)
		{
			return (File.Exists (path) || Directory.Exists (path));
		}
		
		public virtual bool IsDirectory (FilePath path)
		{
			return Directory.Exists (path);
		}

		public virtual bool IsFile (FilePath path)
		{
			return File.Exists (path);
		}

		public virtual long LastModified (FilePath path)
		{
			if (IsFile(path)) {
				var info2 = new FileInfo(path);
				return info2.Exists ? info2.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
			} else if (IsDirectory (path)) {
				var info = new DirectoryInfo(path);
				return info.Exists ? info.LastWriteTimeUtc.ToMillisecondsSinceEpoch() : 0;
			}
			return 0;
		}

		public virtual long Length (FilePath path)
		{
			// If you call .Length on a file that doesn't exist, an exception is thrown
			var info2 = new FileInfo (path);
			return info2.Exists ? info2.Length : 0;
		}

		public virtual void MakeDirWritable (FilePath path)
		{
			foreach (string file in Directory.GetFiles (path)) {
				MakeFileWritable (file);
			}
			foreach (string subdir in Directory.GetDirectories (path)) {
				MakeDirWritable (subdir);
			}
		}

		public virtual void MakeFileWritable (FilePath file)
		{
			FileAttributes fileAttributes = File.GetAttributes (file);
			if ((fileAttributes & FileAttributes.ReadOnly) != 0) {
				fileAttributes &= ~FileAttributes.ReadOnly;
				File.SetAttributes (file, fileAttributes);
			}
		}

		public virtual bool RenameTo (FilePath path, string name)
		{
			try {
				File.Move (path, name);
				return true;
			} catch {
				return false;
			}
		}

		public virtual bool SetExecutable (FilePath path, bool exec)
		{
			return false;
		}

		public virtual void SetReadOnly (FilePath path)
		{
			var fileAttributes = File.GetAttributes (path) | FileAttributes.ReadOnly;
			File.SetAttributes (path, fileAttributes);
		}

		public virtual bool SetLastModified(FilePath path, long milis)
		{
			try {
				DateTime utcDateTime = Extensions.MillisToDateTimeOffset(milis, 0L).UtcDateTime;
				if (IsFile(path)) {
					var info2 = new FileInfo(path);
					info2.LastWriteTimeUtc = utcDateTime;
					return true;
				} else if (IsDirectory(path)) {
					var info = new DirectoryInfo(path);
					info.LastWriteTimeUtc = utcDateTime;
					return true;
				}
			} catch  {

			}
			return false;
		}
	}
	
	class LinuxFileHelper : FileHelper
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
				return base.RenameTo (path, name);
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

		public override void SetReadOnly (FilePath path)
		{
			var info = GetUnixFileInfo (path);
			if (info != null)
				info.FileAccessPermissions &= ~ (FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherWrite | FileAccessPermissions.UserWrite);
		}
	}
}

