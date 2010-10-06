using System;
using System.Collections.Generic;
using System.Text;
using NGit;
using Sharpen;

namespace NGit.Errors
{
	/// <summary>An exception detailing multiple reasons for failure.</summary>
	/// <remarks>An exception detailing multiple reasons for failure.</remarks>
	[System.Serializable]
	public class CompoundException : Exception
	{
		private const long serialVersionUID = 1L;

		private static string Format(ICollection<Exception> causes)
		{
			StringBuilder msg = new StringBuilder();
			msg.Append(JGitText.Get().failureDueToOneOfTheFollowing);
			foreach (Exception c in causes)
			{
				msg.Append("  ");
				msg.Append(c.Message);
				msg.Append("\n");
			}
			return msg.ToString();
		}

		private readonly IList<Exception> causeList;

		/// <summary>Constructs an exception detailing many potential reasons for failure.</summary>
		/// <remarks>Constructs an exception detailing many potential reasons for failure.</remarks>
		/// <param name="why">Two or more exceptions that may have been the problem.</param>
		public CompoundException(ICollection<Exception> why) : base(Format(why))
		{
			causeList = Sharpen.Collections.UnmodifiableList(new AList<Exception>(why));
		}

		/// <summary>Get the complete list of reasons why this failure happened.</summary>
		/// <remarks>Get the complete list of reasons why this failure happened.</remarks>
		/// <returns>unmodifiable collection of all possible reasons.</returns>
		public virtual IList<Exception> GetAllCauses()
		{
			return causeList;
		}
	}
}
