﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlBinder.Parsing.Tokens
{
	public class ScopeSeparator : TextToken
	{
		internal ScopeSeparator(Token parent) : base(parent) { }

		internal static bool Evaluate(Reader reader)
		{
			if (!(reader.Token is NestedToken nestedParent))
				return false;
			if (!char.IsWhiteSpace(reader.Char))
				return false;
			if (!(nestedParent.Children.LastOrDefault() is Scope))
				return false;

			var startingIdx = reader.Index;

			try
			{
				while (reader.Read())
				{
					if (char.IsWhiteSpace(reader.Char))
						continue;
					return Scope.Evaluate(reader);
				}
			}
			finally { reader.Index = startingIdx; }

			return false;
		}

		public override string ToString() => $"{GetType().Name}";
	}
}