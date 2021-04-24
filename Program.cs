/*
    Name: Kunj Soni
    Student ID: 991591881
    Date: 22 April 2021
    Title: Final Exam - Winter 2021
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalKunjSoni
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                //Class Instance DataAccess
                DataAccess data = new DataAccess();

                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Green;

                

                //menu
                Console.WriteLine("\n\n\t\tFINAL EXAM by Kunj Soni");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n\n-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n\t\t1 - Purchase Games");
                Console.WriteLine("\t\t2 - View Customer's Transaction history");
                Console.WriteLine("\t\t3 - View All Transactions");
                Console.WriteLine("\t\t4 - Exit\n");

                Console.ForegroundColor = ConsoleColor.White;


                Console.Write("\nEnter your choice: ");
                int choice;
                bool choiceIsNumeric = Int32.TryParse(Console.ReadLine(), out choice);

                while (!choiceIsNumeric)
                {
                    Console.WriteLine("Invalid Input. Please Try again\n");
                    Console.Write("\nEnter your choice: ");
                    choiceIsNumeric = Int32.TryParse(Console.ReadLine(), out choice);
                }

                switch (choice)
                {
                    case 1:
                        Console.Clear();
                        data.PurchaseGames();
                        Console.Write("\nPress any key to continue... ");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        data.CustomersData();
                        Console.Write("\nPress any key to continue... ");
                        Console.ReadKey();
                        break;

                    case 3:
                        Console.Clear();
                        data.ViewAllTransactions();
                        Console.Write("\nPress any key to continue... ");
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.WriteLine();
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("\nInvalid Choice. Please try again!");
                        break;
                }
            } while (true);
        }
    }
}
