#region Copyright © 2005 Paul Welter. All rights reserved.
/*
Copyright © 2005 Paul Welter. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.Text;

namespace MSBuild.Community.Tasks
{
	/// <summary>
	/// Gets the current date and time.
	/// </summary>
	/// <example>Using the CurrentDateTime task to get the Month, Day,
	/// Year, Hour, Minute, and Second.
	/// <code><![CDATA[
	/// <CurrentDateTime>
	///   <Output TaskParameter="Month" PropertyName="Month" />
	///   <Output TaskParameter="Day" PropertyName="Day" />
	///   <Output TaskParameter="Year" PropertyName="Year" />
	///   <Output TaskParameter="Hour" PropertyName="Hour" />
	///	  <Output TaskParameter="Minute" PropertyName="Minute" />
	///	  <Output TaskParameter="Second" PropertyName="Second" />
	///	</CurrentDateTime>
	///	<Message Text="Current DateTime: $(Month)/$(Day)/$(Year) $(Hour):$(Minute):$(Second)" />
	/// ]]></code>
	/// </example>
	public class CurrentDateTime : Task
	{
		#region Fields

		private System.DateTime mDate;
		private string mMonth;
		private string mDay;
		private string mYear;
		private string mHour;
		private string mMinute;
		private string mSecond;
		private string mMillisecond;
		private string mTicks;
		private string mKind;
		private string mTimeOfDay;
		private string mDayOfYear;
		private string mDayOfWeek;
		private string mDateTime;
		private string mShortDate;
		private string mLongDate;
		private string mShortTime;
		private string mLongTime;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the month component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Month
		{
			get { return mMonth; }
		}

		/// <summary>
		/// Gets the day of the month represented by this instance.
		/// </summary>
		[Output]
		public string Day
		{
			get { return mDay; }
		}

		/// <summary>
		/// Gets the year component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Year
		{
			get { return mYear; }
		}

		/// <summary>
		/// Gets the hour component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Hour
		{
			get { return mHour; }
		}

		/// <summary>
		/// Gets the minute component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Minute
		{
			get { return mMinute; }
		}

		/// <summary>
		/// Gets the seconds component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Second
		{
			get { return mSecond; }
		}

		/// <summary>
		/// Gets the milliseconds component of the date represented by this instance.
		/// </summary>
		[Output]
		public string Millisecond
		{
			get { return mMillisecond; }
		}

		/// <summary>
		/// Gets the number of ticks that represent the date and time of this instance.
		/// </summary>
		[Output]
		public string Ticks
		{
			get { return mTicks; }
		}

		/// <summary>
		/// Gets a value that indicates whether the time represented by this instance is based
		/// on local time, Coordinated Universal Time (UTC), or neither.
		/// </summary>
		[Output]
		public string Kind
		{
			get { return mKind; }
		}

		/// <summary>
		/// Gets the time of day for this instance.
		/// </summary>
		[Output]
		public string TimeOfDay
		{
			get { return mTimeOfDay; }
		}

		/// <summary>
		/// Gets the day of the year represented by this instance.
		/// </summary>
		[Output]
		public string DayOfYear
		{
			get { return mDayOfYear; }
		}

		/// <summary>
		/// Gets the day of the week represented by this instance.
		/// </summary>
		[Output]
		public string DayOfWeek
		{
			get { return mDayOfWeek; }
		}

		/// <summary>
		/// Gets the value of this instance to its equivalent string representation. 
		/// </summary>
		[Output]
		public string DateTime
		{
			get { return mDateTime; }
		}

		/// <summary>
		/// Gets the value of this instance to its equivalent short date string representation.
		/// </summary>
		[Output]
		public string ShortDate
		{
			get { return mShortDate; }
		}

		/// <summary>
		/// Gets the value of this instance to its equivalent long date string representation.
		/// </summary>
		[Output]
		public string LongDate
		{
			get { return mLongDate; }
		}

		/// <summary>
		/// Gets the value of this instance to its equivalent short time string representation.
		/// </summary>
		[Output]
		public string ShortTime
		{
			get { return mShortTime; }
		}

		/// <summary>
		/// Gets the value of this instance to its equivalent long time string representation.
		/// </summary>
		[Output]
		public string LongTime
		{
			get { return mLongTime; }
		}

		#endregion

		/// <summary>
		/// When overridden in a derived class, executes the task.
		/// </summary>
		/// <returns>
		/// True if the task successfully executed; otherwise, false.
		/// </returns>
		public override bool Execute()
		{
			bool bSuccess = true;

			try
			{
				GetDate();
				Log.LogMessage(MessageImportance.Normal, "Getting current date.");
			}
			catch (Exception ex)
			{
				// Log failure
				Log.LogErrorFromException(ex);
				Log.LogMessage(MessageImportance.High, "Failed to get current date!");
				bSuccess = false;
			}

			return bSuccess;
		}

		#region Private Methods

		private void GetDate()
		{
			mDate = System.DateTime.Now;
			mMonth = mDate.Month.ToString();
			mDay = mDate.Day.ToString();
			mYear = mDate.Year.ToString();
			mHour = mDate.Hour.ToString();
			mMinute = mDate.Minute.ToString();
			mSecond = mDate.Second.ToString();
			mMillisecond = mDate.Millisecond.ToString();
			mTicks = mDate.Ticks.ToString();
			mKind = mDate.Kind.ToString();
			mTimeOfDay = mDate.TimeOfDay.ToString();
			mDayOfYear = mDate.DayOfYear.ToString();
			mDayOfWeek = mDate.DayOfWeek.ToString();
			mDateTime = mDate.ToString();
			mShortDate = mDate.ToShortDateString();
			mLongDate = mDate.ToLongDateString();
			mShortTime = mDate.ToShortTimeString();
			mLongTime = mDate.ToLongTimeString();
		}

		#endregion

	}
}
