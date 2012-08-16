/*
This code is derived from jgit (http://eclipse.org/jgit).
Copyright owners are documented in jgit's IP log.

This program and the accompanying materials are made available
under the terms of the Eclipse Distribution License v1.0 which
accompanies this distribution, is reproduced below, and is
available at http://www.eclipse.org/org/documents/edl-v10.php

All rights reserved.

Redistribution and use in source and binary forms, with or
without modification, are permitted provided that the following
conditions are met:

- Redistributions of source code must retain the above copyright
  notice, this list of conditions and the following disclaimer.

- Redistributions in binary form must reproduce the above
  copyright notice, this list of conditions and the following
  disclaimer in the documentation and/or other materials provided
  with the distribution.

- Neither the name of the Eclipse Foundation, Inc. nor the
  names of its contributors may be used to endorse or promote
  products derived from this software without specific prior
  written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using NGit;
using NGit.Dircache;
using NGit.Errors;
using NGit.Internal;
using NGit.Revwalk;
using NGit.Storage.File;
using NGit.Storage.Pack;
using NGit.Treewalk;
using NGit.Treewalk.Filter;
using NGit.Util;
using Sharpen;

namespace NGit.Storage.File
{
	/// <summary>
	/// A garbage collector for git
	/// <see cref="FileRepository">FileRepository</see>
	/// . Instances of this class
	/// are not thread-safe. Don't use the same instance from multiple threads.
	/// This class started as a copy of DfsGarbageCollector from Shawn O. Pearce
	/// adapted to FileRepositories.
	/// </summary>
	public class GC
	{
		private readonly FileRepository repo;

		private ProgressMonitor pm;

		private long expireAgeMillis;

		/// <summary>
		/// the refs which existed during the last call to
		/// <see cref="Repack()">Repack()</see>
		/// . This is
		/// needed during
		/// <see cref="Prune(System.Collections.Generic.ICollection{E})">Prune(System.Collections.Generic.ICollection&lt;E&gt;)
		/// 	</see>
		/// where we can optimize by looking at the
		/// difference between the current refs and the refs which existed during
		/// last
		/// <see cref="Repack()">Repack()</see>
		/// .
		/// </summary>
		private IDictionary<string, Ref> lastPackedRefs;

		/// <summary>Holds the starting time of the last repack() execution.</summary>
		/// <remarks>
		/// Holds the starting time of the last repack() execution. This is needed in
		/// prune() to inspect only those reflog entries which have been added since
		/// last repack().
		/// </remarks>
		private long lastRepackTime;

		/// <summary>Creates a new garbage collector with default values.</summary>
		/// <remarks>
		/// Creates a new garbage collector with default values. An expirationTime of
		/// two weeks and <code>null</code> as progress monitor will be used.
		/// </remarks>
		/// <param name="repo">the repo to work on</param>
		public GC(FileRepository repo)
		{
			this.repo = repo;
			this.pm = NullProgressMonitor.INSTANCE;
			this.expireAgeMillis = 14 * 24 * 60 * 60 * 1000L;
		}

		/// <summary>
		/// Runs a garbage collector on a
		/// <see cref="FileRepository">FileRepository</see>
		/// . It will
		/// <ul>
		/// <li>pack loose references into packed-refs</li>
		/// <li>repack all reachable objects into new pack files and delete the old
		/// pack files</li>
		/// <li>prune all loose objects which are now reachable by packs</li>
		/// </ul>
		/// </summary>
		/// <returns>
		/// the collection of
		/// <see cref="PackFile">PackFile</see>
		/// 's which are newly created
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual ICollection<PackFile> Gc()
		{
			PackRefs();
			// TODO: implement reflog_expire(pm, repo);
			ICollection<PackFile> newPacks = Repack();
			Prune(Sharpen.Collections.EmptySet<ObjectId>());
			// TODO: implement rerere_gc(pm);
			return newPacks;
		}

		/// <summary>Delete old pack files.</summary>
		/// <remarks>
		/// Delete old pack files. What is 'old' is defined by specifying a set of
		/// old pack files and a set of new pack files. Each pack file contained in
		/// old pack files but not contained in new pack files will be deleted.
		/// </remarks>
		/// <param name="oldPacks"></param>
		/// <param name="newPacks"></param>
		/// <param name="ignoreErrors">
		/// <code>true</code> if we should ignore the fact that a certain
		/// pack files or index files couldn't be deleted.
		/// <code>false</code> if an exception should be thrown in such
		/// cases
		/// </param>
		/// <exception cref="System.IO.IOException">
		/// if a pack file couldn't be deleted and
		/// <code>ignoreErrors</code> is set to <code>false</code>
		/// </exception>
		private void DeleteOldPacks(ICollection<PackFile> oldPacks, ICollection<PackFile>
			 newPacks, bool ignoreErrors)
		{
			int deleteOptions = FileUtils.RETRY | FileUtils.SKIP_MISSING;
			if (ignoreErrors)
			{
				deleteOptions |= FileUtils.IGNORE_ERRORS;
			}
			foreach (PackFile oldPack in oldPacks)
			{
				bool retainPack = false;
				string oldName = oldPack.GetPackName();
				// check whether an old pack file is also among the list of new
				// pack files. Then we must not delete it.
				foreach (PackFile newPack in newPacks)
				{
					if (oldName.Equals(newPack.GetPackName()))
					{
						retainPack = true;
						break;
					}
				}
				if (!retainPack && !oldPack.ShouldBeKept())
				{
					oldPack.Close();
					FileUtils.Delete(NameFor(oldName, ".pack"), deleteOptions);
					FileUtils.Delete(NameFor(oldName, ".idx"), deleteOptions);
				}
			}

			// close the complete object database. Thats my only chance to force
			// rescanning and to detect that certain pack files are now deleted.
			((ObjectDirectory)repo.ObjectDatabase).Close();
		}

		/// <summary>
		/// Like "git prune-packed" this method tries to prune all loose objects
		/// which can be found in packs.
		/// </summary>
		/// <remarks>
		/// Like "git prune-packed" this method tries to prune all loose objects
		/// which can be found in packs. If certain objects can't be pruned (e.g.
		/// because the filesystem delete operation fails) this is silently ignored.
		/// </remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void PrunePacked()
		{
			ObjectDirectory objdb = ((ObjectDirectory)repo.ObjectDatabase);
			ICollection<PackFile> packs = objdb.GetPacks();
			FilePath objects = repo.ObjectsDirectory;
			string[] fanout = objects.List();
			if (fanout != null && fanout.Length > 0)
			{
				pm.BeginTask(JGitText.Get().pruneLoosePackedObjects, fanout.Length);
				try
				{
					foreach (string d in fanout)
					{
						pm.Update(1);
						if (d.Length != 2)
						{
							continue;
						}
						string[] entries = new FilePath(objects, d).List();
						if (entries == null)
						{
							continue;
						}
						foreach (string e in entries)
						{
							if (e.Length != Constants.OBJECT_ID_STRING_LENGTH - 2)
							{
								continue;
							}
							ObjectId id;
							try
							{
								id = ObjectId.FromString(d + e);
							}
							catch (ArgumentException)
							{
								// ignoring the file that does not represent loose
								// object
								continue;
							}
							bool found = false;
							foreach (PackFile p in packs)
							{
								if (p.HasObject(id))
								{
									found = true;
									break;
								}
							}
							if (found)
							{
								FileUtils.Delete(objdb.FileFor(id), FileUtils.RETRY | FileUtils.SKIP_MISSING | FileUtils
									.IGNORE_ERRORS);
							}
						}
					}
				}
				finally
				{
					pm.EndTask();
				}
			}
		}

		/// <summary>
		/// Like "git prune" this method tries to prune all loose objects which are
		/// unreferenced.
		/// </summary>
		/// <remarks>
		/// Like "git prune" this method tries to prune all loose objects which are
		/// unreferenced. If certain objects can't be pruned (e.g. because the
		/// filesystem delete operation fails) this is silently ignored.
		/// </remarks>
		/// <param name="objectsToKeep">a set of objects which should explicitly not be pruned
		/// 	</param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Prune(ICollection<ObjectId> objectsToKeep)
		{
			long expireDate = (expireAgeMillis == 0) ? long.MaxValue : Runtime.CurrentTimeMillis
				() - expireAgeMillis;
			// Collect all loose objects which are old enough, not referenced from
			// the index and not in objectsToKeep
			IDictionary<ObjectId, FilePath> deletionCandidates = new Dictionary<ObjectId, FilePath
				>();
			ICollection<ObjectId> indexObjects = null;
			FilePath objects = repo.ObjectsDirectory;
			string[] fanout = objects.List();
			if (fanout != null && fanout.Length > 0)
			{
				pm.BeginTask(JGitText.Get().pruneLooseUnreferencedObjects, fanout.Length);
				foreach (string d in fanout)
				{
					pm.Update(1);
					if (d.Length != 2)
					{
						continue;
					}
					FilePath[] entries = new FilePath(objects, d).ListFiles();
					if (entries == null)
					{
						continue;
					}
					foreach (FilePath f in entries)
					{
						string fName = f.GetName();
						if (fName.Length != Constants.OBJECT_ID_STRING_LENGTH - 2)
						{
							continue;
						}
						if (f.LastModified() >= expireDate)
						{
							continue;
						}
						try
						{
							ObjectId id = ObjectId.FromString(d + fName);
							if (objectsToKeep.Contains(id))
							{
								continue;
							}
							if (indexObjects == null)
							{
								indexObjects = ListNonHEADIndexObjects();
							}
							if (indexObjects.Contains(id))
							{
								continue;
							}
							deletionCandidates.Put(id, f);
						}
						catch (ArgumentException)
						{
							// ignoring the file that does not represent loose
							// object
							continue;
						}
					}
				}
			}
			if (deletionCandidates.IsEmpty())
			{
				return;
			}
			// From the set of current refs remove all those which have been handled
			// during last repack(). Only those refs will survive which have been
			// added or modified since the last repack. Only these can save existing
			// loose refs from being pruned.
			IDictionary<string, Ref> newRefs;
			if (lastPackedRefs == null || lastPackedRefs.IsEmpty())
			{
				newRefs = GetAllRefs();
			}
			else
			{
				newRefs = new Dictionary<string, Ref>();
				for (Iterator<KeyValuePair<string, Ref>> i = GetAllRefs().EntrySet().Iterator(); 
					i.HasNext(); )
				{
					KeyValuePair<string, Ref> newEntry = i.Next();
					Ref old = lastPackedRefs.Get(newEntry.Key);
					if (!Equals(newEntry.Value, old))
					{
						newRefs.Put(newEntry.Key, newEntry.Value);
					}
				}
			}
			if (!newRefs.IsEmpty())
			{
				// There are new/modified refs! Check which loose objects are now
				// referenced by these modified refs (or their reflogentries).
				// Remove these loose objects
				// from the deletionCandidates. When the last candidate is removed
				// leave this method.
				ObjectWalk w = new ObjectWalk(repo);
				try
				{
					foreach (Ref cr in newRefs.Values)
					{
						w.MarkStart(w.ParseAny(cr.GetObjectId()));
					}
					if (lastPackedRefs != null)
					{
						foreach (Ref lpr in lastPackedRefs.Values)
						{
							w.MarkUninteresting(w.ParseAny(lpr.GetObjectId()));
						}
					}
					RemoveReferenced(deletionCandidates, w);
				}
				finally
				{
					w.Dispose();
				}
			}
			if (deletionCandidates.IsEmpty())
			{
				return;
			}
			// Since we have not left the method yet there are still
			// deletionCandidates. Last chance for these objects not to be pruned is
			// that they are referenced by reflog entries. Even refs which currently
			// point to the same object as during last repack() may have
			// additional reflog entries not handled during last repack()
			ObjectWalk w_1 = new ObjectWalk(repo);
			try
			{
				foreach (Ref ar in GetAllRefs().Values)
				{
					foreach (ObjectId id in ListRefLogObjects(ar, lastRepackTime))
					{
						w_1.MarkStart(w_1.ParseAny(id));
					}
				}
				if (lastPackedRefs != null)
				{
					foreach (Ref lpr in lastPackedRefs.Values)
					{
						w_1.MarkUninteresting(w_1.ParseAny(lpr.GetObjectId()));
					}
				}
				RemoveReferenced(deletionCandidates, w_1);
			}
			finally
			{
				w_1.Dispose();
			}
			if (deletionCandidates.IsEmpty())
			{
				return;
			}
			// delete all candidates which have survived: these are unreferenced
			// loose objects
			foreach (FilePath f_1 in deletionCandidates.Values)
			{
				f_1.Delete();
			}
			((ObjectDirectory)repo.ObjectDatabase).Close();
		}

		/// <summary>
		/// Remove all entries from a map which key is the id of an object referenced
		/// by the given ObjectWalk
		/// </summary>
		/// <param name="id2File"></param>
		/// <param name="w"></param>
		/// <exception cref="NGit.Errors.MissingObjectException">NGit.Errors.MissingObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.IncorrectObjectTypeException">NGit.Errors.IncorrectObjectTypeException
		/// 	</exception>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private void RemoveReferenced(IDictionary<ObjectId, FilePath> id2File, ObjectWalk
			 w)
		{
			RevObject ro = w.Next();
			while (ro != null)
			{
				if (Sharpen.Collections.Remove(id2File, ro.Id) != null)
				{
					if (id2File.IsEmpty())
					{
						return;
					}
				}
				ro = w.Next();
			}
			ro = w.NextObject();
			while (ro != null)
			{
				if (Sharpen.Collections.Remove(id2File, ro.Id) != null)
				{
					if (id2File.IsEmpty())
					{
						return;
					}
				}
				ro = w.NextObject();
			}
		}

		private static bool Equals(Ref r1, Ref r2)
		{
			if (r1 == null || r2 == null)
			{
				return false;
			}
			if (r1.IsSymbolic())
			{
				if (!r2.IsSymbolic())
				{
					return false;
				}
				return r1.GetTarget().GetName().Equals(r2.GetTarget().GetName());
			}
			else
			{
				if (r2.IsSymbolic())
				{
					return false;
				}
				return r1.GetObjectId().Equals(r2.GetObjectId());
			}
		}

		/// <summary>Packs all non-symbolic, loose refs into packed-refs.</summary>
		/// <remarks>Packs all non-symbolic, loose refs into packed-refs.</remarks>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void PackRefs()
		{
			ICollection<Ref> refs = repo.GetAllRefs().Values;
			IList<string> refsToBePacked = new AList<string>(refs.Count);
			pm.BeginTask(JGitText.Get().packRefs, refs.Count);
			try
			{
				foreach (Ref @ref in refs)
				{
					if (!@ref.IsSymbolic() && @ref.GetStorage().IsLoose())
					{
						refsToBePacked.AddItem(@ref.GetName());
					}
					pm.Update(1);
				}
				((RefDirectory)repo.RefDatabase).Pack(refsToBePacked);
			}
			finally
			{
				pm.EndTask();
			}
		}

		/// <summary>
		/// Packs all objects which reachable from any of the heads into one pack
		/// file.
		/// </summary>
		/// <remarks>
		/// Packs all objects which reachable from any of the heads into one pack
		/// file. Additionally all objects which are not reachable from any head but
		/// which are reachable from any of the other refs (e.g. tags), special refs
		/// (e.g. FETCH_HEAD) or index are packed into a separate pack file. Objects
		/// included in pack files which have a .keep file associated are never
		/// repacked. All old pack files which existed before are deleted.
		/// </remarks>
		/// <returns>a collection of the newly created pack files</returns>
		/// <exception cref="System.IO.IOException">
		/// when during reading of refs, index, packfiles, objects,
		/// reflog-entries or during writing to the packfiles
		/// <see cref="System.IO.IOException">System.IO.IOException</see>
		/// occurs
		/// </exception>
		public virtual ICollection<PackFile> Repack()
		{
			ICollection<PackFile> toBeDeleted = ((ObjectDirectory)repo.ObjectDatabase).GetPacks
				();
			long time = Runtime.CurrentTimeMillis();
			IDictionary<string, Ref> refsBefore = GetAllRefs();
			ICollection<ObjectId> allHeads = new HashSet<ObjectId>();
			ICollection<ObjectId> nonHeads = new HashSet<ObjectId>();
			ICollection<ObjectId> tagTargets = new HashSet<ObjectId>();
			ICollection<ObjectId> indexObjects = ListNonHEADIndexObjects();
			foreach (Ref @ref in refsBefore.Values)
			{
				Sharpen.Collections.AddAll(nonHeads, ListRefLogObjects(@ref, 0));
				if (@ref.IsSymbolic() || @ref.GetObjectId() == null)
				{
					continue;
				}
				if (@ref.GetName().StartsWith(Constants.R_HEADS))
				{
					allHeads.AddItem(@ref.GetObjectId());
				}
				else
				{
					nonHeads.AddItem(@ref.GetObjectId());
				}
				if (@ref.GetPeeledObjectId() != null)
				{
					tagTargets.AddItem(@ref.GetPeeledObjectId());
				}
			}
			IList<PackIndex> excluded = new List<PackIndex>();
			foreach (PackFile f in ((ObjectDirectory)repo.ObjectDatabase).GetPacks())
			{
				if (f.ShouldBeKept())
				{
					excluded.AddItem(f.GetIndex());
				}
			}
			Sharpen.Collections.AddAll(tagTargets, allHeads);
			Sharpen.Collections.AddAll(nonHeads, indexObjects);
			IList<PackFile> ret = new AList<PackFile>(2);
			PackFile heads = null;
			if (!allHeads.IsEmpty())
			{
				heads = WritePack(allHeads, Sharpen.Collections.EmptySet<ObjectId>(), tagTargets, 
					excluded);
				if (heads != null)
				{
					ret.AddItem(heads);
					excluded.Add(0, heads.GetIndex());
				}
			}
			if (!nonHeads.IsEmpty())
			{
				PackFile rest = WritePack(nonHeads, allHeads, tagTargets, excluded);
				if (rest != null)
				{
					ret.AddItem(rest);
				}
			}
			DeleteOldPacks(toBeDeleted, ret, true);
			PrunePacked();
			lastPackedRefs = refsBefore;
			lastRepackTime = time;
			return ret;
		}

		/// <param name="ref">the ref which log should be inspected</param>
		/// <param name="minTime">only reflog entries not older then this time are processed</param>
		/// <returns>
		/// the
		/// <see cref="NGit.ObjectId">NGit.ObjectId</see>
		/// s contained in the reflog
		/// </returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private ICollection<ObjectId> ListRefLogObjects(Ref @ref, long minTime)
		{
			IList<ReflogEntry> rlEntries = repo.GetReflogReader(@ref.GetName()).GetReverseEntries
				();
			if (rlEntries == null || rlEntries.IsEmpty())
			{
				return Sharpen.Collections.EmptySet<ObjectId>();
			}
			ICollection<ObjectId> ret = new HashSet<ObjectId>();
			foreach (ReflogEntry e in rlEntries)
			{
				if (e.GetWho().GetWhen().GetTime() < minTime)
				{
					break;
				}
				ret.AddItem(e.GetNewId());
				ObjectId oldId = e.GetOldId();
				if (oldId != null && !ObjectId.ZeroId.Equals(oldId))
				{
					ret.AddItem(oldId);
				}
			}
			return ret;
		}

		/// <summary>Returns a map of all refs and additional refs (e.g.</summary>
		/// <remarks>
		/// Returns a map of all refs and additional refs (e.g. FETCH_HEAD,
		/// MERGE_HEAD, ...)
		/// </remarks>
		/// <returns>a map where names of refs point to ref objects</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		private IDictionary<string, Ref> GetAllRefs()
		{
			IDictionary<string, Ref> ret = repo.GetAllRefs();
			foreach (Ref @ref in repo.RefDatabase.GetAdditionalRefs())
			{
				ret.Put(@ref.GetName(), @ref);
			}
			return ret;
		}

		/// <summary>
		/// Return a list of those objects in the index which differ from whats in
		/// HEAD
		/// </summary>
		/// <returns>a set of ObjectIds of changed objects in the index</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		/// <exception cref="NGit.Errors.CorruptObjectException">NGit.Errors.CorruptObjectException
		/// 	</exception>
		/// <exception cref="NGit.Errors.NoWorkTreeException">NGit.Errors.NoWorkTreeException
		/// 	</exception>
		private ICollection<ObjectId> ListNonHEADIndexObjects()
		{
			RevWalk revWalk = null;
			try
			{
				if (repo.GetIndexFile() == null)
				{
					return Sharpen.Collections.EmptySet<ObjectId>();
				}
			}
			catch (NoWorkTreeException)
			{
				return Sharpen.Collections.EmptySet<ObjectId>();
			}
			TreeWalk treeWalk = new TreeWalk(repo);
			try
			{
				treeWalk.AddTree(new DirCacheIterator(repo.ReadDirCache()));
				ObjectId headID = repo.Resolve(Constants.HEAD);
				if (headID != null)
				{
					revWalk = new RevWalk(repo);
					treeWalk.AddTree(revWalk.ParseTree(headID));
					revWalk.Dispose();
					revWalk = null;
				}
				treeWalk.Filter = TreeFilter.ANY_DIFF;
				treeWalk.Recursive = true;
				ICollection<ObjectId> ret = new HashSet<ObjectId>();
				while (treeWalk.Next())
				{
					ObjectId objectId = treeWalk.GetObjectId(0);
					switch (treeWalk.GetRawMode(0) & FileMode.TYPE_MASK)
					{
						case FileMode.TYPE_MISSING:
						case FileMode.TYPE_GITLINK:
						{
							continue;
							goto case FileMode.TYPE_TREE;
						}

						case FileMode.TYPE_TREE:
						case FileMode.TYPE_FILE:
						case FileMode.TYPE_SYMLINK:
						{
							ret.AddItem(objectId);
							continue;
							goto default;
						}

						default:
						{
							throw new IOException(MessageFormat.Format(JGitText.Get().corruptObjectInvalidMode3
								, string.Format("%o", Sharpen.Extensions.ValueOf(treeWalk.GetRawMode(0)), (objectId
								 == null) ? "null" : objectId.Name, treeWalk.PathString, repo.GetIndexFile())));
						}
					}
				}
				return ret;
			}
			finally
			{
				if (revWalk != null)
				{
					revWalk.Dispose();
				}
				treeWalk.Release();
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private PackFile WritePack<_T0, _T1>(ICollection<_T0> want, ICollection<_T1> have
			, ICollection<ObjectId> tagTargets, IList<PackIndex> excludeObjects) where _T0:ObjectId
			 where _T1:ObjectId
		{
			FilePath tmpPack = null;
			FilePath tmpIdx = null;
			PackWriter pw = new PackWriter(repo);
			try
			{
				// prepare the PackWriter
				pw.SetDeltaBaseAsOffset(true);
				pw.SetReuseDeltaCommits(false);
				if (tagTargets != null)
				{
					pw.SetTagTargets(tagTargets);
				}
				if (excludeObjects != null)
				{
					foreach (PackIndex idx in excludeObjects)
					{
						pw.ExcludeObjects(idx);
					}
				}
				pw.PreparePack(pm, want, have);
				if (pw.GetObjectCount() == 0)
				{
					return null;
				}
				// create temporary files
				string id = pw.ComputeName().GetName();
				FilePath packdir = new FilePath(repo.ObjectsDirectory, "pack");
				tmpPack = FilePath.CreateTempFile("gc_", ".pack_tmp", packdir);
				tmpIdx = new FilePath(packdir, Sharpen.Runtime.Substring(tmpPack.GetName(), 0, tmpPack
					.GetName().LastIndexOf('.')) + ".idx_tmp");
				if (!tmpIdx.CreateNewFile())
				{
					throw new IOException(MessageFormat.Format(JGitText.Get().cannotCreateIndexfile, 
						tmpIdx.GetPath()));
				}
				// write the packfile
				FileChannel channel = new FileOutputStream(tmpPack).GetChannel();
				OutputStream channelStream = Channels.NewOutputStream(channel);
				try
				{
					pw.WritePack(pm, pm, channelStream);
				}
				finally
				{
					channel.Force(true);
					channelStream.Close();
					channel.Close();
				}
				// write the packindex
				FileChannel idxChannel = new FileOutputStream(tmpIdx).GetChannel();
				OutputStream idxStream = Channels.NewOutputStream(idxChannel);
				try
				{
					pw.WriteIndex(idxStream);
				}
				finally
				{
					idxChannel.Force(true);
					idxStream.Close();
					idxChannel.Close();
				}
				// rename the temporary files to real files
				FilePath realPack = NameFor(id, ".pack");
				tmpPack.SetReadOnly();
				FilePath realIdx = NameFor(id, ".idx");
				realIdx.SetReadOnly();
				bool delete = true;
				try
				{
					if (!tmpPack.RenameTo(realPack))
					{
						return null;
					}
					delete = false;
					if (!tmpIdx.RenameTo(realIdx))
					{
						FilePath newIdx = new FilePath(realIdx.GetParentFile(), realIdx.GetName() + ".new"
							);
						if (!tmpIdx.RenameTo(newIdx))
						{
							newIdx = tmpIdx;
						}
						throw new IOException(MessageFormat.Format(JGitText.Get().panicCantRenameIndexFile
							, newIdx, realIdx));
					}
				}
				finally
				{
					if (delete && tmpPack.Exists())
					{
						tmpPack.Delete();
					}
					if (delete && tmpIdx.Exists())
					{
						tmpIdx.Delete();
					}
				}
				return ((ObjectDirectory)repo.ObjectDatabase).OpenPack(realPack, realIdx);
			}
			finally
			{
				pw.Release();
				if (tmpPack != null && tmpPack.Exists())
				{
					tmpPack.Delete();
				}
				if (tmpIdx != null && tmpIdx.Exists())
				{
					tmpIdx.Delete();
				}
			}
		}

		private FilePath NameFor(string name, string ext)
		{
			FilePath packdir = new FilePath(repo.ObjectsDirectory, "pack");
			return new FilePath(packdir, "pack-" + name + ext);
		}

		/// <summary>
		/// A class holding statistical data for a FileRepository regarding how many
		/// objects are stored as loose or packed objects
		/// </summary>
		public class RepoStatistics
		{
			/// <summary>The number of objects stored in pack files.</summary>
			/// <remarks>
			/// The number of objects stored in pack files. If the same object is
			/// stored in multiple pack files then it is counted as often as it
			/// occurs in pack files.
			/// </remarks>
			public long numberOfPackedObjects;

			/// <summary>The number of pack files</summary>
			public long numberOfPackFiles;

			/// <summary>The number of objects stored as loose objects.</summary>
			/// <remarks>The number of objects stored as loose objects.</remarks>
			public long numberOfLooseObjects;

			internal RepoStatistics(GC _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly GC _enclosing;
		}

		/// <summary>Returns the number of objects stored in pack files.</summary>
		/// <remarks>
		/// Returns the number of objects stored in pack files. If an object is
		/// contained in multiple pack files it is counted as often as it occurs.
		/// </remarks>
		/// <returns>the number of objects stored in pack files</returns>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual GC.RepoStatistics GetStatistics()
		{
			GC.RepoStatistics ret = new GC.RepoStatistics(this);
			ICollection<PackFile> packs = ((ObjectDirectory)repo.ObjectDatabase).GetPacks();
			foreach (PackFile f in packs)
			{
				ret.numberOfPackedObjects += f.GetIndex().GetObjectCount();
			}
			ret.numberOfPackFiles = packs.Count;
			FilePath objDir = repo.ObjectsDirectory;
			string[] fanout = objDir.List();
			if (fanout != null && fanout.Length > 0)
			{
				foreach (string d in fanout)
				{
					if (d.Length != 2)
					{
						continue;
					}
					string[] entries = new FilePath(objDir, d).List();
					if (entries == null)
					{
						continue;
					}
					foreach (string e in entries)
					{
						if (e.Length != Constants.OBJECT_ID_STRING_LENGTH - 2)
						{
							continue;
						}
						ret.numberOfLooseObjects++;
					}
				}
			}
			return ret;
		}

		/// <summary>Set the progress monitor used for garbage collection methods.</summary>
		/// <remarks>Set the progress monitor used for garbage collection methods.</remarks>
		/// <param name="pm"></param>
		/// <returns>this</returns>
		public virtual GC SetProgressMonitor(ProgressMonitor pm)
		{
			this.pm = (pm == null) ? NullProgressMonitor.INSTANCE : pm;
			return this;
		}

		/// <summary>
		/// During gc() or prune() each unreferenced, loose object which has been
		/// created or modified in the last <code>expireAgeMillis</code> milliseconds
		/// will not be pruned.
		/// </summary>
		/// <remarks>
		/// During gc() or prune() each unreferenced, loose object which has been
		/// created or modified in the last <code>expireAgeMillis</code> milliseconds
		/// will not be pruned. Only older objects may be pruned. If set to 0 then
		/// every object is a candidate for pruning.
		/// </remarks>
		/// <param name="expireAgeMillis">minimal age of objects to be pruned in milliseconds.
		/// 	</param>
		public virtual void SetExpireAgeMillis(long expireAgeMillis)
		{
			this.expireAgeMillis = expireAgeMillis;
		}
	}
}
