using NGit;
using Sharpen;

namespace NGit.Revwalk
{
	/// <summary>
	/// Case insensitive key for a
	/// <see cref="FooterLine">FooterLine</see>
	/// .
	/// </summary>
	public sealed class FooterKey
	{
		/// <summary>
		/// Standard
		/// <code>Signed-off-by</code>
		/// 
		/// </summary>
		public static readonly NGit.Revwalk.FooterKey SIGNED_OFF_BY = new NGit.Revwalk.FooterKey
			("Signed-off-by");

		/// <summary>
		/// Standard
		/// <code>Acked-by</code>
		/// 
		/// </summary>
		public static readonly NGit.Revwalk.FooterKey ACKED_BY = new NGit.Revwalk.FooterKey
			("Acked-by");

		/// <summary>
		/// Standard
		/// <code>CC</code>
		/// 
		/// </summary>
		public static readonly NGit.Revwalk.FooterKey CC = new NGit.Revwalk.FooterKey("CC"
			);

		private readonly string name;

		internal readonly byte[] raw;

		/// <summary>Create a key for a specific footer line.</summary>
		/// <remarks>Create a key for a specific footer line.</remarks>
		/// <param name="keyName">name of the footer line.</param>
		public FooterKey(string keyName)
		{
			name = keyName;
			raw = Constants.Encode(keyName.ToLower());
		}

		/// <returns>name of this footer line.</returns>
		public string GetName()
		{
			return name;
		}

		public override string ToString()
		{
			return "FooterKey[" + name + "]";
		}
	}
}
