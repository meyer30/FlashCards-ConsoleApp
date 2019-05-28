using System;
using System.IO;

namespace ConsoleApplication2
{
    public class Program
	{
		#region Static Fields
		const int lineLength = 80;
		#endregion


		static void Main(string[] args)
        {
            string[] directories, options, activeFileNames = null;

            Console.WriteLine("Flash Card Simulator\n\n");

            var consoleAppDirectory = GetConsoleAppDirectory();
            var inputDataDirectory = $"{consoleAppDirectory}\\inputData";

            directories = Directory.GetDirectories(inputDataDirectory);
            options = SelectLastPart(directories);

            int optNum = GetOptNum(options);


            if (optNum != 0)
            {
                activeFileNames = Directory.GetFiles(directories[optNum - 1]);
                options = SelectLastPart(activeFileNames);

                //Choose Flash Cards
                while ((optNum = GetOptNum(options)) != 0)
                {

                    string filePath = activeFileNames[optNum - 1];

                    Deck deck = new Deck(filePath);

                    Console.WriteLine(deck.Size + " number of cards in deck.");
                    Console.WriteLine(deck.Description);
                    Console.WriteLine("Do you want to view these cards?");
                    if (GetBooleanFromUser())
                    {
                        Console.WriteLine("Do you want to shuffle?");
                        if (GetBooleanFromUser())
                        {
                            deck.Shuffle();
                        }

                        ViewCards(deck, filePath);
                    }
                }
            }

            Console.WriteLine("Do you want to save these cards to a file.");
            if (GetBooleanFromUser())
            {
                var cheatSheetFilePath = $"{consoleAppDirectory}\\outputData\\cheatSheet.txt";
                File.WriteAllText(cheatSheetFilePath, "");

                for (int idx = 0; idx < activeFileNames.Length; idx++)
                {
                    Deck d = new Deck(activeFileNames[idx]);

                    d.AddToCheatSheet(cheatSheetFilePath);
                }
            }

            Console.WriteLine("\n\nDone with flashcards\n\nEnter anything to quit.");
            Console.ReadLine();
        }

        private static string GetConsoleAppDirectory()
        {
            var curExPath = AppDomain.CurrentDomain.BaseDirectory;
            var idxOfBin = curExPath.IndexOf("\\bin");
            var consoleAppDirectory = curExPath.Substring(0, idxOfBin);
            return consoleAppDirectory;
        }

        private static string[] SelectLastPart(string[] pathLis)
		{
			string[] lastPartLis = new string[pathLis.Length];

			for(int idx=0; idx<pathLis.Length; idx++)
			{
				string curPath = pathLis[idx];
				int lastIdx = curPath.LastIndexOf("\\");
				lastPartLis[idx] = curPath.Substring(lastIdx + 1);
			}

			return lastPartLis;
		}


		private static int ViewCards(Deck deck, string saveFilePath)
		{
			bool changesMade = false;
			int numWrite = 0;

			for (int idx = 0; idx < deck.Size; idx++)
			{
				FlashCard f = deck[idx];

				DisplayText(f.SideA + "\n");
				if(Console.ReadLine().Length > 0)
				{
					Console.WriteLine();
				}

				DisplayText(f.SideB + "\n");

				Console.WriteLine("Edit/Remove Card?\n");
				string input = Console.ReadLine();
				if (input.Length > 0)
				{
					int oldDeckSize = deck.Size;
					if (TryEditOrRemoveCard(deck, f, input))
					{
						if(oldDeckSize>deck.Size)
						{
							idx = idx - 1;
						}
						Console.WriteLine();
						changesMade = true;
					}
				}
			}
			if (changesMade)
			{
				deck.Save(saveFilePath);
			}

			return numWrite;
		}

		private static void DisplayText(string text)
		{
			if (text.Length <= lineLength)
			{
				Console.WriteLine(text);
			}
			else
			{
				string lineOfText = text.Substring(0, 80);
				int idxOfLineBreak = lineOfText.LastIndexOf(' ');
				int idxOfNextLine = idxOfLineBreak + 1;

				if (lineOfText.Contains(Environment.NewLine))
				{
					idxOfLineBreak = lineOfText.IndexOf(Environment.NewLine);
					idxOfNextLine = idxOfLineBreak + 2;
				}

				Console.WriteLine(text.Substring(0, idxOfLineBreak));
				DisplayText(text.Substring(idxOfNextLine));
			}
		}




		private static bool TryEditOrRemoveCard(Deck deck, FlashCard f, string input)
		{
			bool changesMade = false;

			input = input.Substring(0, 1).ToUpper();
			if (input.Equals("E"))
			{
				changesMade = TryEdit(deck, f);
			}
			else if (input.Equals("R"))
			{
				deck.Remove(f);
				changesMade = true;
			}

			return changesMade;
		}

		private static bool TryEdit(Deck deck, FlashCard f)
		{
			bool changesMade = false;
			string input;

			Console.WriteLine("Change Side A?");
			input = Console.ReadLine();
			if (input.Length > 5)
			{
				f.SideA = input;
				changesMade = true;
			}
			else if (input.Length > 0 && input.Substring(0, 1).ToUpper().Equals("Y"))
			{
				Console.WriteLine("Enter what should be on Side A");
				f.SideA = Console.ReadLine();
				changesMade = true;
			}


			Console.WriteLine("Change Side B?");
			input = Console.ReadLine();
			if (input.Length > 5)
			{
				f.SideB = input;
				changesMade = true;
			}
			else if (input.Length > 0 && input.Substring(0, 1).ToUpper().Equals("Y"))
			{
				Console.WriteLine("Enter what should be on Side B");
				f.SideB = Console.ReadLine();
				changesMade = true;
			}

			return changesMade;
		}

		private static bool GetBooleanFromUser()
		{
			bool agrees = false;
			string input = Console.ReadLine();
			if (input.Length > 0)
			{
				input = input.Substring(0, 1).ToUpper();
				agrees = input.Equals("Y");
			}

			return agrees;
		}



		private static int GetOptNum(string[] options)
		{
			if(options.Length == 1)
			{
				return 1;
			}

			int optNum;

			Console.WriteLine("Choose from the following options.");
			Console.WriteLine("  0) Quit");
			for (int idx = 0; idx < options.Length; idx++)
			{
				optNum = idx + 1;
				Console.WriteLine("  " + optNum + ") " + options[idx]);
			}
			string input = Console.ReadLine();

			if (int.TryParse(input, out optNum))
			{
				if (optNum >= 0 && optNum <= options.Length)
				{
					Console.Clear();
					return optNum;
				}
			}

			Console.Clear();
			Console.WriteLine("\n\n  '" + input + "' is an Invalid Response.\n\n");
			return GetOptNum(options);
		}
	};
};
