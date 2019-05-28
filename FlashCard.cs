using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
	public class FlashCard
	{
		public int Id { get; set; }

		public string SideA { get; set; }

		public string SideB { get; set; }

		internal void AdjustForLineBreaks()
		{
			int idxStart=0, idxNewL;
			StringBuilder sb = new StringBuilder();


			while((idxNewL = this.SideB.IndexOf("\\n",idxStart))!=-1)
			{
				sb.Append(this.SideB.Substring(idxStart,idxNewL-idxStart));
				sb.Append(Environment.NewLine);
				idxStart = idxNewL + 2;
			}

			if(idxStart >0)
			{
				int lastLineLen = this.SideB.Length - idxStart;
				sb.Append(this.SideB.Substring(idxStart, lastLineLen));
				this.SideB = sb.ToString();
			}
		}

		internal bool IsComplete()
		{
			if (SideB.Length < 2)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		internal void RemoveNewLines()
		{
			if(this.SideA.Contains(Environment.NewLine))
			{
				this.SideA = this.SideA.Replace(Environment.NewLine, ". ");
			}
			if(this.SideB.Contains(Environment.NewLine))
			{
				this.SideB = this.SideB.Replace(Environment.NewLine, ". ");
			}
		}

		private string RemoveNewLines(string str)
		{
			if(!str.Contains(Environment.NewLine))
			{
				return str;
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				int idx = str.IndexOf(Environment.NewLine);
			
				sb.Append(str.Substring(0,idx));
				sb.Append(". ");
				sb.Append(str.Substring(idx+1));

				return sb.ToString();
			}
		}
	};
};