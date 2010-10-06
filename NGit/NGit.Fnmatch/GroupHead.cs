using System.Collections.Generic;
using NGit;
using NGit.Errors;
using NGit.Fnmatch;
using Sharpen;

namespace NGit.Fnmatch
{
	internal sealed class GroupHead : AbstractHead
	{
		private readonly IList<GroupHead.CharacterPattern> characterClasses;

		private static readonly Sharpen.Pattern REGEX_PATTERN = Sharpen.Pattern.Compile("([^-][-][^-]|\\[[.:=].*?[.:=]\\])"
			);

		private readonly bool inverse;

		/// <exception cref="NGit.Errors.InvalidPatternException"></exception>
		internal GroupHead(string pattern, string wholePattern) : base(false)
		{
			this.characterClasses = new AList<GroupHead.CharacterPattern>();
			this.inverse = pattern.StartsWith("!");
			if (inverse)
			{
				pattern = Sharpen.Runtime.Substring(pattern, 1);
			}
			Matcher matcher = REGEX_PATTERN.Matcher(pattern);
			while (matcher.Find())
			{
				string characterClass = matcher.Group(0);
				if (characterClass.Length == 3 && characterClass[1] == '-')
				{
					char start = characterClass[0];
					char end = characterClass[2];
					characterClasses.AddItem(new GroupHead.CharacterRange(start, end));
				}
				else
				{
					if (characterClass.Equals("[:alnum:]"))
					{
						characterClasses.AddItem(GroupHead.LetterPattern.INSTANCE);
						characterClasses.AddItem(GroupHead.DigitPattern.INSTANCE);
					}
					else
					{
						if (characterClass.Equals("[:alpha:]"))
						{
							characterClasses.AddItem(GroupHead.LetterPattern.INSTANCE);
						}
						else
						{
							if (characterClass.Equals("[:blank:]"))
							{
								characterClasses.AddItem(new GroupHead.OneCharacterPattern(' '));
								characterClasses.AddItem(new GroupHead.OneCharacterPattern('\t'));
							}
							else
							{
								if (characterClass.Equals("[:cntrl:]"))
								{
									characterClasses.AddItem(new GroupHead.CharacterRange('\u0000', '\u001F'));
									characterClasses.AddItem(new GroupHead.OneCharacterPattern('\u007F'));
								}
								else
								{
									if (characterClass.Equals("[:digit:]"))
									{
										characterClasses.AddItem(GroupHead.DigitPattern.INSTANCE);
									}
									else
									{
										if (characterClass.Equals("[:graph:]"))
										{
											characterClasses.AddItem(new GroupHead.CharacterRange('\u0021', '\u007E'));
											characterClasses.AddItem(GroupHead.LetterPattern.INSTANCE);
											characterClasses.AddItem(GroupHead.DigitPattern.INSTANCE);
										}
										else
										{
											if (characterClass.Equals("[:lower:]"))
											{
												characterClasses.AddItem(GroupHead.LowerPattern.INSTANCE);
											}
											else
											{
												if (characterClass.Equals("[:print:]"))
												{
													characterClasses.AddItem(new GroupHead.CharacterRange('\u0020', '\u007E'));
													characterClasses.AddItem(GroupHead.LetterPattern.INSTANCE);
													characterClasses.AddItem(GroupHead.DigitPattern.INSTANCE);
												}
												else
												{
													if (characterClass.Equals("[:punct:]"))
													{
														characterClasses.AddItem(GroupHead.PunctPattern.INSTANCE);
													}
													else
													{
														if (characterClass.Equals("[:space:]"))
														{
															characterClasses.AddItem(GroupHead.WhitespacePattern.INSTANCE);
														}
														else
														{
															if (characterClass.Equals("[:upper:]"))
															{
																characterClasses.AddItem(GroupHead.UpperPattern.INSTANCE);
															}
															else
															{
																if (characterClass.Equals("[:xdigit:]"))
																{
																	characterClasses.AddItem(new GroupHead.CharacterRange('0', '9'));
																	characterClasses.AddItem(new GroupHead.CharacterRange('a', 'f'));
																	characterClasses.AddItem(new GroupHead.CharacterRange('A', 'F'));
																}
																else
																{
																	if (characterClass.Equals("[:word:]"))
																	{
																		characterClasses.AddItem(new GroupHead.OneCharacterPattern('_'));
																		characterClasses.AddItem(GroupHead.LetterPattern.INSTANCE);
																		characterClasses.AddItem(GroupHead.DigitPattern.INSTANCE);
																	}
																	else
																	{
																		string message = string.Format(MessageFormat.Format(JGitText.Get().characterClassIsNotSupported
																			, characterClass));
																		throw new InvalidPatternException(message, wholePattern);
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				pattern = matcher.ReplaceFirst(string.Empty);
				matcher.Reset(pattern);
			}
			// pattern contains now no ranges
			for (int i = 0; i < pattern.Length; i++)
			{
				char c = pattern[i];
				characterClasses.AddItem(new GroupHead.OneCharacterPattern(c));
			}
		}

		protected internal sealed override bool Matches(char c)
		{
			foreach (GroupHead.CharacterPattern pattern in characterClasses)
			{
				if (pattern.Matches(c))
				{
					return !inverse;
				}
			}
			return inverse;
		}

		private interface CharacterPattern
		{
			/// <param name="c">the character to test</param>
			/// <returns>returns true if the character matches a pattern.</returns>
			bool Matches(char c);
		}

		private sealed class CharacterRange : GroupHead.CharacterPattern
		{
			private readonly char start;

			private readonly char end;

			internal CharacterRange(char start, char end)
			{
				this.start = start;
				this.end = end;
			}

			public bool Matches(char c)
			{
				return start <= c && c <= end;
			}
		}

		private sealed class DigitPattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.DigitPattern INSTANCE = new GroupHead.DigitPattern
				();

			public bool Matches(char c)
			{
				return char.IsDigit(c);
			}
		}

		private sealed class LetterPattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.LetterPattern INSTANCE = new GroupHead.LetterPattern
				();

			public bool Matches(char c)
			{
				return char.IsLetter(c);
			}
		}

		private sealed class LowerPattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.LowerPattern INSTANCE = new GroupHead.LowerPattern
				();

			public bool Matches(char c)
			{
				return System.Char.IsLower(c);
			}
		}

		private sealed class UpperPattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.UpperPattern INSTANCE = new GroupHead.UpperPattern
				();

			public bool Matches(char c)
			{
				return System.Char.IsUpper(c);
			}
		}

		private sealed class WhitespacePattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.WhitespacePattern INSTANCE = new GroupHead.WhitespacePattern
				();

			public bool Matches(char c)
			{
				return char.IsWhiteSpace(c);
			}
		}

		private sealed class OneCharacterPattern : GroupHead.CharacterPattern
		{
			private char expectedCharacter;

			internal OneCharacterPattern(char c)
			{
				this.expectedCharacter = c;
			}

			public bool Matches(char c)
			{
				return this.expectedCharacter == c;
			}
		}

		private sealed class PunctPattern : GroupHead.CharacterPattern
		{
			internal static readonly GroupHead.PunctPattern INSTANCE = new GroupHead.PunctPattern
				();

			private static string punctCharacters = "-!\"#$%&'()*+,./:;<=>?@[\\]_`{|}~";

			public bool Matches(char c)
			{
				return punctCharacters.IndexOf(c) != -1;
			}
		}
	}
}
