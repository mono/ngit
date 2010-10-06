using System.Collections.Generic;
using NGit.Util;
using Sharpen;

namespace NGit.Util
{
	/// <summary>Abstract authenticator which remembers prior authentications.</summary>
	/// <remarks>Abstract authenticator which remembers prior authentications.</remarks>
	public abstract class CachedAuthenticator : Authenticator
	{
		private static readonly ICollection<CachedAuthenticator.CachedAuthentication> cached
			 = new CopyOnWriteArrayList<CachedAuthenticator.CachedAuthentication>();

		/// <summary>Add a cached authentication for future use.</summary>
		/// <remarks>Add a cached authentication for future use.</remarks>
		/// <param name="ca">the information we should remember.</param>
		public static void Add(CachedAuthenticator.CachedAuthentication ca)
		{
			cached.AddItem(ca);
		}

		protected sealed override PasswordAuthentication GetPasswordAuthentication()
		{
			string host = GetRequestingHost();
			int port = GetRequestingPort();
			foreach (CachedAuthenticator.CachedAuthentication ca in cached)
			{
				if (ca.host.Equals(host) && ca.port == port)
				{
					return ca.ToPasswordAuthentication();
				}
			}
			PasswordAuthentication pa = PromptPasswordAuthentication();
			if (pa != null)
			{
				CachedAuthenticator.CachedAuthentication ca_1 = new CachedAuthenticator.CachedAuthentication
					(host, port, pa.GetUserName(), Sharpen.Extensions.CreateString(pa.GetPassword())
					);
				Add(ca_1);
				return ca_1.ToPasswordAuthentication();
			}
			return null;
		}

		/// <summary>Prompt for and request authentication from the end-user.</summary>
		/// <remarks>Prompt for and request authentication from the end-user.</remarks>
		/// <returns>
		/// the authentication data; null if the user canceled the request
		/// and does not want to continue.
		/// </returns>
		protected internal abstract PasswordAuthentication PromptPasswordAuthentication();

		/// <summary>Authentication data to remember and reuse.</summary>
		/// <remarks>Authentication data to remember and reuse.</remarks>
		public class CachedAuthentication
		{
			internal readonly string host;

			internal readonly int port;

			internal readonly string user;

			internal readonly string pass;

			/// <summary>Create a new cached authentication.</summary>
			/// <remarks>Create a new cached authentication.</remarks>
			/// <param name="aHost">system this is for.</param>
			/// <param name="aPort">port number of the service.</param>
			/// <param name="aUser">username at the service.</param>
			/// <param name="aPass">password at the service.</param>
			public CachedAuthentication(string aHost, int aPort, string aUser, string aPass)
			{
				host = aHost;
				port = aPort;
				user = aUser;
				pass = aPass;
			}

			internal virtual PasswordAuthentication ToPasswordAuthentication()
			{
				return new PasswordAuthentication(user, pass.ToCharArray());
			}
		}
	}
}
