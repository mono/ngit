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
using System.IO;
using NGit;
using NGit.Errors;
using NGit.Transport;
using Sharpen;

namespace NGit.Transport
{
	/// <summary>Implements the server side of a push connection, receiving objects.</summary>
	/// <remarks>Implements the server side of a push connection, receiving objects.</remarks>
	public class ReceivePack : BaseReceivePack
	{
		/// <summary>Hook to validate the update commands before execution.</summary>
		/// <remarks>Hook to validate the update commands before execution.</remarks>
		private PreReceiveHook preReceive;

		/// <summary>Hook to report on the commands after execution.</summary>
		/// <remarks>Hook to report on the commands after execution.</remarks>
		private PostReceiveHook postReceive;

		private bool echoCommandFailures;

		/// <summary>Create a new pack receive for an open repository.</summary>
		/// <remarks>Create a new pack receive for an open repository.</remarks>
		/// <param name="into">the destination repository.</param>
		protected internal ReceivePack(Repository into) : base(into)
		{
			preReceive = PreReceiveHook.NULL;
			postReceive = PostReceiveHook.NULL;
		}

		/// <returns>the hook invoked before updates occur.</returns>
		public virtual PreReceiveHook GetPreReceiveHook()
		{
			return preReceive;
		}

		/// <summary>Set the hook which is invoked prior to commands being executed.</summary>
		/// <remarks>
		/// Set the hook which is invoked prior to commands being executed.
		/// <p>
		/// Only valid commands (those which have no obvious errors according to the
		/// received input and this instance's configuration) are passed into the
		/// hook. The hook may mark a command with a result of any value other than
		/// <see cref="Result.NOT_ATTEMPTED">Result.NOT_ATTEMPTED</see>
		/// to block its execution.
		/// <p>
		/// The hook may be called with an empty command collection if the current
		/// set is completely invalid.
		/// </remarks>
		/// <param name="h">the hook instance; may be null to disable the hook.</param>
		public virtual void SetPreReceiveHook(PreReceiveHook h)
		{
			preReceive = h != null ? h : PreReceiveHook.NULL;
		}

		/// <returns>the hook invoked after updates occur.</returns>
		public virtual PostReceiveHook GetPostReceiveHook()
		{
			return postReceive;
		}

		/// <summary>Set the hook which is invoked after commands are executed.</summary>
		/// <remarks>
		/// Set the hook which is invoked after commands are executed.
		/// <p>
		/// Only successful commands (type is
		/// <see cref="Result.OK">Result.OK</see>
		/// ) are passed into the
		/// hook. The hook may be called with an empty command collection if the
		/// current set all resulted in an error.
		/// </remarks>
		/// <param name="h">the hook instance; may be null to disable the hook.</param>
		public virtual void SetPostReceiveHook(PostReceiveHook h)
		{
			postReceive = h != null ? h : PostReceiveHook.NULL;
		}

		/// <param name="echo">
		/// if true this class will report command failures as warning
		/// messages before sending the command results. This is usually
		/// not necessary, but may help buggy Git clients that discard the
		/// errors when all branches fail.
		/// </param>
		public virtual void SetEchoCommandFailures(bool echo)
		{
			echoCommandFailures = echo;
		}

		/// <summary>Execute the receive task on the socket.</summary>
		/// <remarks>Execute the receive task on the socket.</remarks>
		/// <param name="input">
		/// raw input to read client commands and pack data from. Caller
		/// must ensure the input is buffered, otherwise read performance
		/// may suffer.
		/// </param>
		/// <param name="output">
		/// response back to the Git network client. Caller must ensure
		/// the output is buffered, otherwise write performance may
		/// suffer.
		/// </param>
		/// <param name="messages">
		/// secondary "notice" channel to send additional messages out
		/// through. When run over SSH this should be tied back to the
		/// standard error channel of the command execution. For most
		/// other network connections this should be null.
		/// </param>
		/// <exception cref="System.IO.IOException">System.IO.IOException</exception>
		public virtual void Receive(InputStream input, OutputStream output, OutputStream 
			messages)
		{
			Init(input, output, messages);
			try
			{
				Service();
			}
			finally
			{
				try
				{
					Close();
				}
				finally
				{
					Release();
				}
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Service()
		{
			if (biDirectionalPipe)
			{
				SendAdvertisedRefs(new RefAdvertiser.PacketLineOutRefAdvertiser(pckOut));
				pckOut.Flush();
			}
			else
			{
				GetAdvertisedOrDefaultRefs();
			}
			if (HasError())
			{
				return;
			}
			RecvCommands();
			if (HasCommands())
			{
				EnableCapabilities();
				Exception unpackError = null;
				if (NeedPack())
				{
					try
					{
						ReceivePackAndCheckConnectivity();
					}
					catch (IOException err)
					{
						unpackError = err;
					}
					catch (RuntimeException err)
					{
						unpackError = err;
					}
					catch (Error err)
					{
						unpackError = err;
					}
				}
				if (unpackError == null)
				{
					ValidateCommands();
					preReceive.OnPreReceive(this, FilterCommands(ReceiveCommand.Result.NOT_ATTEMPTED)
						);
					ExecuteCommands();
				}
				UnlockPack();
				if (reportStatus)
				{
					if (echoCommandFailures && msgOut != null)
					{
						SendStatusReport(false, unpackError, new _Reporter_199(this));
						msgOut.Flush();
						try
						{
							Sharpen.Thread.Sleep(500);
						}
						catch (Exception)
						{
						}
					}
					// Ignore an early wake up.
					SendStatusReport(true, unpackError, new _Reporter_211(this));
					pckOut.End();
				}
				else
				{
					if (msgOut != null)
					{
						SendStatusReport(false, unpackError, new _Reporter_218(this));
					}
				}
				postReceive.OnPostReceive(this, FilterCommands(ReceiveCommand.Result.OK));
				if (unpackError != null)
				{
					throw new UnpackException(unpackError);
				}
			}
		}

		private sealed class _Reporter_199 : BaseReceivePack.Reporter
		{
			public _Reporter_199(ReceivePack _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal override void SendString(string s)
			{
				this._enclosing.msgOut.Write(Constants.Encode(s + "\n"));
			}

			private readonly ReceivePack _enclosing;
		}

		private sealed class _Reporter_211 : BaseReceivePack.Reporter
		{
			public _Reporter_211(ReceivePack _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal override void SendString(string s)
			{
				this._enclosing.pckOut.WriteString(s + "\n");
			}

			private readonly ReceivePack _enclosing;
		}

		private sealed class _Reporter_218 : BaseReceivePack.Reporter
		{
			public _Reporter_218(ReceivePack _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.IO.IOException"></exception>
			internal override void SendString(string s)
			{
				this._enclosing.msgOut.Write(Constants.Encode(s + "\n"));
			}

			private readonly ReceivePack _enclosing;
		}

		protected internal override string GetLockMessageProcessName()
		{
			return "jgit receive-pack";
		}
	}
}
