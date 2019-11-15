using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// Task to filter an Input list and to match groups with a Regex expression.
	/// Output list contains matched group  values together with corresponding group names and original values from Input list that matched given expression
	/// </summary>
	/// <example>Matches TestGroup items containing at lease two numeric groups connected by a dot and returns the first two groups with values
	/// <code><![CDATA[
	/// <ItemGroup>
	///    <TestGroup Include="foo.my.foo.foo.test.o" />
	///    <TestGroup Include="foo.my.faa.foo.test.a" />
	///    <TestGroup Include="foo.my.fbb.foo.test.b" />
	///    <TestGroup Include="foo.my.fcc.foo.test.c" />
	///    <TestGroup Include="foo.my.fdd.foo.test.d" />
	///    <TestGroup Include="foo.my.fee.foo.test.e" />
	///    <TestGroup Include="foo.my.fff.foo.test.f" />
	///    <TestGroup Include="1.2" />
	///    <TestGroup Include="1.2.3" />
	/// </ItemGroup>
	/// <Target Name="Test">
	///    <!-- Outputs only items that consist of at least twho numbers conected by a dot (1.2|1.2.3) as groups (1 from 1.2, 2 from 1.2, 1 from 1.2.3, 2 from 1.2.3)-->
	///    <RegexMatch Input="@(TestGroup)" Expression="(?<FirstGroup>\d+)\.(?<SecondGroup>\d+)">
	///       <Output ItemName ="MatchReturn" TaskParameter="Output" />
	///    </RegexMatch>
	///    <Message Text="&#xA;Output Match:&#xA;@(MatchReturn->' matched value: %(Identity), original value: %(OriginalItem), matched group name: %(GroupName)', '&#xA;')" />
	/// </Target>
	/// ]]></code>
	/// </example>
	public class RegexMatchGroups : RegexBase
	{
		private class ResultEntry
		{
			public String OriginalItem { get; set; }
			public String GroupName { get; set; }
			public String Value { get; set; }
		}

		private Regex Regex { get; set; }
		/// <summary>
		/// Performs the Match task
		/// </summary>
		/// <returns><see langword="true"/> if the task ran successfully; 
		/// otherwise <see langword="false"/>.</returns>
		public override bool Execute()
		{
			Regex = new Regex(Expression.ItemSpec, ExpressionOptions);

			var results = Input.SelectMany(MatchInputItem);

			Output = CreateOutputItems(results);

			return !Log.HasLoggedErrors;
		}

		private IEnumerable<ResultEntry> MatchInputItem(ITaskItem inputItem)
		{
			var sourceItem = inputItem.ItemSpec;

			var match = Regex.Match(sourceItem);

			return match.Success ?
				Regex.GetGroupNames().Select(groupName =>
				{
					var group = match.Groups[groupName];

					return group.Success
						? new ResultEntry() { OriginalItem = sourceItem, GroupName = groupName, Value = group.Value }
						: null;
				}).Where(q => q != null) : new ResultEntry[0];
		}

		private ITaskItem[] CreateOutputItems(IEnumerable<ResultEntry> results)
		{
			return results.Select((result) =>
				{
					ITaskItem resultItem = new TaskItem(result.Value);
					resultItem.SetMetadata("OriginalItem", result.OriginalItem);
					resultItem.SetMetadata("GroupName", result.GroupName);
					return resultItem;
				})
				.ToArray();
		}
	}
}