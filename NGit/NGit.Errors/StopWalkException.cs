using Sharpen;

namespace NGit.Errors
{
	/// <summary>Stops the driver loop of walker and finish with current results.</summary>
	/// <remarks>Stops the driver loop of walker and finish with current results.</remarks>
	/// <seealso cref="NGit.Revwalk.Filter.RevFilter">NGit.Revwalk.Filter.RevFilter</seealso>
	[System.Serializable]
	public class StopWalkException : RuntimeException
	{
		private const long serialVersionUID = 1L;

		/// <summary>Singleton instance for throwing within a filter.</summary>
		/// <remarks>Singleton instance for throwing within a filter.</remarks>
		public static readonly NGit.Errors.StopWalkException INSTANCE = new NGit.Errors.StopWalkException
			();

		public StopWalkException()
		{
		}
		// Nothing.
	}
}
