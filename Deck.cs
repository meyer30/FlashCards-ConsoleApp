using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
	public class Deck
	{

		#region Properties/Atrributes

		public string Comment { get; set; }

		public string Description { get; set; }

		public bool CardsCompleted { get; set; }

		public int Size
		{
			get { return this.cards.Count; }
		}

		public FlashCard this[int idx]
		{
			get { return this.cards[idx]; }
		}

		private List<FlashCard> cards = new List<FlashCard>();


		private List<FlashCard> DeletedCards = new List<FlashCard>();

		#endregion


		/// <summary>
		/// Creates Deck based on saved text file.
		/// </summary>
		public Deck(string filepath)
		{
			int idxAStart = 0, idxAEnd;
			int idxBStart, idxBEnd;
			int idxCommentStart, idxCommentEnd;

			this.CardsCompleted = true;

			string text = System.IO.File.ReadAllText(filepath);


			idxCommentStart = text.IndexOf("/*");

			if (idxCommentStart > 0)
			{
				idxAStart = idxCommentEnd = text.IndexOf("*/") + 2;
				this.Description = text.Substring(0, idxCommentStart);
				this.Comment = text.Substring(idxCommentStart + 2, idxCommentEnd - idxCommentStart-4);
			}
			else if (text.IndexOf('~') > 0)
			{
				this.Description = text.Substring(0, text.IndexOf('~'));
			}
			else
			{
				this.Description = text;
			}



			if (text.IndexOf('~', idxAStart) > 0)
			{
				bool endReached = false;
				int id = 0;

				while (!endReached)
				{
					idxAStart = text.IndexOf('~', idxAStart + 1) + 1;
					idxAEnd = text.IndexOf('@', idxAStart);
					idxBStart = idxAEnd + 1;
					idxBEnd = text.IndexOf('~', idxBStart);
					if (idxBEnd < 0)
					{
						endReached = true;
						idxBEnd = text.Length;
					}

					FlashCard c = new FlashCard();
					c.SideA = text.Substring(idxAStart, idxAEnd - idxAStart).Trim();
					c.SideB = text.Substring(idxBStart, idxBEnd - idxBStart).Trim();
					c.AdjustForLineBreaks();
					c.Id = id;
					cards.Add(c);

					id++;
					this.CardsCompleted &= c.IsComplete();
				}
			}
		}



		internal void Remove(FlashCard f)
		{
			this.DeletedCards.Add(f);
			this.cards.Remove(f);
		}

		internal void Save(string filepath)
		{
			StringBuilder text = new StringBuilder();
			if (this.Description.Length > 0)
			{
				text.Append(this.Description);
			}

			if (this.Comment.Length > 0 || this.DeletedCards.Count > 0)
			{
				text.Append("/*");
				if (this.Comment.Length > 0)
				{
					text.Append(this.Comment);
				}

				for (int idx = 0; idx < this.DeletedCards.Count; idx++)
				{
					FlashCard f = this.DeletedCards[idx];
					text = text.Append("~");
					text = text.Append(f.SideA);
					text = text.Append("@");
					text = text.Append(f.SideB);
					text = text.Append(Environment.NewLine);
				}
				
				text.Append("*/");
				text = text.Append(Environment.NewLine);
			}

			for (int idx = 0; idx < this.cards.Count; idx++)
			{
				FlashCard f = this[idx];
				text = text.Append("~");
				text = text.Append(f.SideA);
				text = text.Append("@");
				text = text.Append(f.SideB);
				text = text.Append(Environment.NewLine);
			}

			System.IO.File.WriteAllText(filepath, text.ToString());
		}

		

		internal void Shuffle()
		{
			Random r = new Random();
			int n = this.Size;
			while (n > 1)
			{
				n--;
				int k = r.Next(n + 1);
				FlashCard swapVal = this.cards[k];
				this.cards[k] = this.cards[n];
				this.cards[n] = swapVal;
			}
		}

		internal void AddToCheatSheet(string filepath)
		{

			StringBuilder text = new StringBuilder();
			if (this.Description.Length > 0)
			{
				text.Append(this.Description);
				text.Append(":");
			}

			for (int idx = 0; idx < this.cards.Count; idx++)
			{
				FlashCard f = this[idx];
				f.RemoveNewLines();
				text = text.Append(f.SideA);
				text = text.Append(f.SideB);
				text.Append("  ");
			}

			System.IO.File.AppendAllText(filepath, text.ToString());
		}
	};
};
