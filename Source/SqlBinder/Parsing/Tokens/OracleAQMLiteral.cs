﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBinder.Parsing.Tokens
{
	public class OracleAQMLiteral : ContentToken // Oracle alternative quoting mechanism
	{
		public const string OPENING_TAG = "q'";
		public const string CLOSING_TAG = "'";
		public const string STANDARD_PAIRS = "[]{}()<>";
		public const string ILLEGAL_CHARACTERS = "\r\n\t '";
		public const string WHITE_SPACE = "\r\n\t \0";

		public override string OpeningTag { get; }
		public override string ClosingTag { get; }

		internal OracleAQMLiteral(Token parent, Reader reader) 
			: base(parent)
		{
			var alternativeTag = reader.Peek(2);

			OpeningTag = OPENING_TAG + alternativeTag;

			var standardPair = STANDARD_PAIRS.IndexOf(alternativeTag);
			if (standardPair > -1)
				ClosingTag = STANDARD_PAIRS[standardPair + 1] + CLOSING_TAG;
			else
				ClosingTag = alternativeTag + CLOSING_TAG;

		}

		internal OracleAQMLiteral(Token parent) : base(parent) { }

		internal static bool Evaluate(Reader reader)
		{
			if (!Evaluate(reader, OPENING_TAG))
				return false;
			if (ILLEGAL_CHARACTERS.Contains(reader.Peek(2)))
				return false;
			if (!WHITE_SPACE.Contains(reader.Peek(-1)))
				return false;
			var standardPair = STANDARD_PAIRS.IndexOf(reader.Peek(2));
			return standardPair < 0 || standardPair % 2 == 0;
		}

		internal override bool EvaluateClosingTag(Reader reader) 
			=> Content != null && base.EvaluateClosingTag(reader);
	}
}