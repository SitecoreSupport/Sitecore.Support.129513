﻿using System;
using System.Text.RegularExpressions;
using Sitecore.Diagnostics;
using Sitecore.Rules.Conditions;

namespace Sitecore.Support.Rules.Conditions
{
	internal static class ConditionsUtility
	{
		internal static ConditionOperator GetConditionOperatorById(string conditionOperatorId)
		{
			if (string.IsNullOrEmpty(conditionOperatorId))
				return ConditionOperator.Unknown;
			switch (conditionOperatorId)
			{
				case "{066602E2-ED1D-44C2-A698-7ED27FD3A2CC}":
					return ConditionOperator.Equal;
				case "{814EF7D0-1639-44FD-AEEF-735B5AC14425}":
					return ConditionOperator.GreaterThanOrEqual;
				case "{B88CD556-082E-4385-BB76-E4D1B565F290}":
					return ConditionOperator.GreaterThan;
				case "{2E1FC840-5919-4C66-8182-A33A1039EDBF}":
					return ConditionOperator.LessThanOrEqual;
				case "{E362A3A4-E230-4A40-A7C4-FC42767E908F}":
					return ConditionOperator.LessThan;
			}
			return conditionOperatorId == "{3627ED99-F454-4B83-841A-A0194F0FB8B4}"
				? ConditionOperator.NotEqual
				: ConditionOperator.Unknown;
		}

		internal static Func<int, int, bool> GetInt32Comparer(string conditionOperatorId)
		{
			return GetInt32Comparer(GetConditionOperatorById(conditionOperatorId));
		}

		internal static Func<int, int, bool> GetInt32Comparer(ConditionOperator conditionOperator)
		{
			switch (conditionOperator)
			{
				case ConditionOperator.Equal:
					return (first, second) => first == second;
				case ConditionOperator.GreaterThanOrEqual:
					return (first, second) => first >= second;
				case ConditionOperator.GreaterThan:
					return (first, second) => first > second;
				case ConditionOperator.LessThanOrEqual:
					return 
					  (first, second) => 
					    first <= second;
				case ConditionOperator.LessThan:
					return (first, second) => 
					  first < second;
				case ConditionOperator.NotEqual:
					return (first, second) => first != second;
				default:
					return null;
			}
		}

		public static StringConditionOperator GetStringConditionOperatorById(string conditionOperatorId)
		{
			if (!string.IsNullOrEmpty(conditionOperatorId))
				switch (conditionOperatorId)
				{
					case "{10537C58-1684-4CAB-B4C0-40C10907CE31}":
						return StringConditionOperator.Equals;

					case "{537244C2-3A3F-4B81-A6ED-02AF494C0563}":
						return StringConditionOperator.CaseInsensitivelyEquals;

					case "{2E67477C-440C-4BCA-A358-3D29AED89F47}":
						return StringConditionOperator.Contains;

					case "{F8641C26-EE27-483C-9FEA-35529ECC8541}":
						return StringConditionOperator.MatchesRegularExpression;

					case "{A6AC5A6B-F409-48B0-ACE7-C3E8C5EC6406}":
						return StringConditionOperator.NotEqual;

					case "{6A7294DF-ECAE-4D5F-A8D2-C69CB1161C09}":
						return StringConditionOperator.NotCaseInsensitivelyEquals;

					case "{22E1F05F-A17A-4D0C-B376-6F7661500F03}":
						return StringConditionOperator.EndsWith;

					case "{FDD7C6B1-622A-4362-9CFF-DDE9866C68EA}":
						return StringConditionOperator.StartsWith;
				}
			return StringConditionOperator.Unknown;
		}


		internal static bool CompareStrings(string first, string second, string conditionOperatorId)
		{
			Assert.ArgumentNotNull(first, "first");
			Assert.ArgumentNotNull(second, "second");
			switch (GetStringConditionOperatorById(conditionOperatorId))
			{
				case StringConditionOperator.Equals:
					return first == second;
				case StringConditionOperator.CaseInsensitivelyEquals:
					return string.Compare(first, second, StringComparison.CurrentCultureIgnoreCase) == 0;
				case StringConditionOperator.NotEqual:
					return first != second;
				case StringConditionOperator.NotCaseInsensitivelyEquals:
					return (uint)string.Compare(first, second, StringComparison.CurrentCultureIgnoreCase) > 0U;
				case StringConditionOperator.Contains:
					return first.IndexOf(second, StringComparison.CurrentCultureIgnoreCase) >= 0;
				case StringConditionOperator.MatchesRegularExpression:
					return Regex.IsMatch(first, second);
				case StringConditionOperator.StartsWith:
					return first.StartsWith(second, StringComparison.CurrentCultureIgnoreCase);
				case StringConditionOperator.EndsWith:
					return first.EndsWith(second, StringComparison.CurrentCultureIgnoreCase);
				default:
					return false;
			}
		}
	}
}