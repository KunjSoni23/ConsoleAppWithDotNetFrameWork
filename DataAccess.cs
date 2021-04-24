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
    public class DataAccess
    {
        internal void PurchaseGames()
        {
            //using block to avoid closing of connection
            using (var context = new GameShoppingDBEntities())
            {
                var gameQuantity = (from gq in context.Games
                                   select gq.Stock).Sum();

                //if there is no quantity in any of the games then,cw   
                if (gameQuantity == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("-------------- NO GAMES TO PURCHASE !!!---------------");
                    Console.WriteLine("------------------------------------------------------\n");

                    Console.ForegroundColor = ConsoleColor.White;

                }
                //if there atleast one quantity in any of the games then,
                else
                {
                    Console.WriteLine("\nGame Purchasing: \n");

                    //customer name input
                    Console.Write("Enter Customer Name: ");
                    string cname = Console.ReadLine();

                    //game data displaying
                    GameData();

                    //retrieving the game id from the customer
                    int x = GameIDValidation();

                    //no. of quantity for entered game id
                    var stock = (from g in context.Games
                                 where g.GameId == x
                                 select g.Stock).FirstOrDefault();

                    //condition for stock
                    while (stock == 0)
                    {
                        Console.WriteLine("Sorry, this game is out of stock. Please check back later.\n");

                        x = GameIDValidation();

                        stock = (from g in context.Games
                                 where g.GameId == x
                                 select g.Stock).FirstOrDefault();
                    }

                    //game quantity input
                    Console.Write("\nEnter Quantity (More than 5 = 10% discount): ");
                    int qty = int.Parse(Console.ReadLine());

                    //condition for entered quantity is greater than present quantity
                    while (qty > stock)
                    {
                        Console.WriteLine("Please enter Quantity less than stock.\n");

                        x = GameIDValidation();

                        stock = (from g in context.Games
                                 where g.GameId == x
                                 select g.Stock).FirstOrDefault();

                        Console.Write("\nEnter Quantity (More than 5 = 10% discount): ");
                        qty = int.Parse(Console.ReadLine());
                    }

                    //retriving price for entered game id
                    var price = (from p in context.Games
                                 where p.GameId == x
                                 select p.Price).FirstOrDefault();

                    //total price for all the quantity without tax
                    double totalPrice = price * qty;

                    double discount = 0;

                    //condition for discount
                    if (qty >= 5)
                    {
                        discount = 0.10 * totalPrice;
                        totalPrice = totalPrice - discount;
                    }

                    //condition if entered name is already present or not in Customers Table
                    var getName = (from n in context.Customers
                                   where n.Name.ToUpper() == cname.ToUpper()
                                   select n.CustomerId).FirstOrDefault();

                    //if new name then it will add new one or else take old one
                    if (getName == 0)
                    {
                        Customer c = new Customer();
                        c.Name = cname;
                        context.Customers.Add(c);
                    }

                    //process to add entered value in orders table
                    Order o = new Order();
                    o.GameId = x;
                    o.Date = DateTime.Now;
                    o.Quantity = qty;
                    o.CustomerId = getName;
                    o.Discount = discount;
                    context.Orders.Add(o);

                    //process to remove sold stock from games table
                    Game g1 = context.Games.Find(x);
                    g1.Stock = g1.Stock - qty;

                    context.SaveChanges();

                    Console.WriteLine("\nYour order has been placed. Thank you for shopping with us.\n");

                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine("--------------------- YOUR ORDER ---------------------");
                    Console.WriteLine("------------------------------------------------------\n");

                    Console.ForegroundColor = ConsoleColor.White;

                    //displaying the recent order.
                    ViewTransaction();
                }  
            }
        }

        //GameIDValidation method to take GameID input from user
        internal int GameIDValidation()
        {
            using (var context = new GameShoppingDBEntities())
            {
                //Game Id input
                Console.Write("Enter Game Id: ");
                int gid = int.Parse(Console.ReadLine());

                var gidIsPresent = (from gi in context.Games
                                    where gi.GameId == gid
                                    select gi.GameId).FirstOrDefault();

                while (gidIsPresent != gid)
                {
                    Console.WriteLine("Invalid Game Id. Please try again.\n");
                    Console.Write("Enter Game Id: ");
                    gid = int.Parse(Console.ReadLine());


                    gidIsPresent = (from gi in context.Games
                                    where gi.GameId == gid
                                    select gi.GameId).FirstOrDefault();
                }

                return gid;
            }
        }

        //Getting order data of customer by Customer ID
        internal void GetCustomerByID()
        {
            using (var context = new GameShoppingDBEntities())
            {
                //customer ID input
                Console.Write("Enter Customer Id: ");
                int cid = int.Parse(Console.ReadLine());

                Console.WriteLine();

                var transaction = (from o in context.Orders
                                   where o.CustomerId == cid
                                   select o).ToList();

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine($"{"Order Id",8} | {"Date",12} | {"Customer",-14} | {"Game",-14} | {"Price",10} | {"Quantity",10} | {"Discount",10} | {"Tax",10} | {"Net Total",10}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;
                double totalNetTotal = 0;
                double totalTax = 0;
                double totalPrice = 0;
                foreach (var t in transaction)
                {
                    double total = t.Game.Price * t.Quantity;
                    double tax = 0.13 * (total - t.Discount);
                    double netTotal = total - t.Discount + tax;

                    Console.WriteLine($"{t.OrderId,8} | {t.Date.ToString("dd-MMM-yy"),12} | {t.Customer.Name,-14} | {t.Game.Name,-14} | {t.Game.Price.ToString("C"),10} | {t.Quantity + " Qty",10} | {t.Discount.ToString("C"),10} | {tax.ToString("C"),10} | {netTotal.ToString("C"),10}");

                    totalNetTotal = totalNetTotal + netTotal;
                    totalTax = totalTax + tax;
                    totalPrice = totalPrice + total;

                }

                var totalDiscount = (from o in context.Orders
                                       where o.CustomerId == cid
                                       select o.Discount).Sum();

                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n------------------------------------------------------");
                Console.WriteLine("--------------- YOUR TOTAL PURCHASES -----------------");
                Console.WriteLine("------------------------------------------------------\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{"Total Games Amount", -25} = + {totalPrice.ToString("C"),-10}");
                Console.WriteLine($"{"Total Tax",-25} = + {totalTax.ToString("C"),-10}");
                Console.WriteLine($"{"Total Discount",-25} = - {totalDiscount.ToString("C"),-10}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("------------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"\n{"NetTotal",-25} = + {totalNetTotal.ToString("C"),-10}");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine();
                

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        //Displaying Customers Data
        internal void CustomersData()
        {
            using (var context = new GameShoppingDBEntities())
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("------------------ CUSTOMERS DATA --------------------");
                Console.WriteLine("------------------------------------------------------");

                var customers = from c in context.Customers
                            select c;

                if (customers.Count()<=0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Console.WriteLine("No Record(s) Fetched");
                }
                else
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.WriteLine($"\n{"Customer Id",12} | {"Name",-13}\n");

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    foreach (var c in customers)
                    {
                        Console.WriteLine($"{c.CustomerId,12} | {c.Name,-13}");
                    }

                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("------------------------------------------------------");
                    GetCustomerByID();

                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        //Displaying Game Data
        internal void GameData()
        {
            using (var context = new GameShoppingDBEntities())
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("------------------ GAMES COLLECTION ------------------");
                Console.WriteLine("------------------------------------------------------");

                var games = from g in context.Games
                            select g;

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine($"\n{"Game Id",8} | {"Name",-13} | {"Price",7} | {"In Stock",7}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;

                foreach (var g in games)
                {
                    Console.WriteLine($"{g.GameId,8} | {g.Name,-13} | {g.Price.ToString("C"),7} | {g.Stock+" Qty", 7}");
                }

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        //To display the recent Transaction of Customer
        internal void ViewTransaction()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var t = (from o in context.Orders
                         orderby o.OrderId descending
                                   select o).FirstOrDefault();

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine($"{"Order Id",8} | {"Date",12} | {"Customer",-14} | {"Game",-14} | {"Price",10} | {"Quantity",10} | {"Total",8} | {"Discount",10} | {"Tax",10} | {"Net Total",10}\n");

                Console.ForegroundColor = ConsoleColor.Yellow;

                double total = t.Game.Price * t.Quantity;
                double tax = 0.13 * (total - t.Discount);
                double netTotal = total - t.Discount + tax;

                Console.WriteLine($"{t.OrderId,8} | {t.Date.ToString("dd-MMM-yy"),12} | {t.Customer.Name,-14} | {t.Game.Name,-14} | {t.Game.Price.ToString("C"),10} | {t.Quantity + " Qty",10} | {total.ToString("C"),8} | {t.Discount.ToString("C"),10} | {tax.ToString("C"),10} | {netTotal.ToString("C"),10}");

                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        //To View All The Transaction of all the customers
        internal void ViewAllTransactions()
        {
            using (var context = new GameShoppingDBEntities())
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("------------------------------------------------------");
                Console.WriteLine("------------------ ALL TRANSACTIONS ------------------");
                Console.WriteLine("------------------------------------------------------");

                var transaction = (from o in context.Orders
                                  select o).ToList();

                //If not data found
                if (transaction.Count()<=0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Console.WriteLine("No Record(s) Fetched");
                }
                //If Data Found
                else
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;

                    Console.WriteLine($"{"Order Id",8} | {"Date",12} | {"Customer",-14} | {"Game",-14} | {"Price",10} | {"Quantity",10} | {"Discount",10} | {"Tax",10} | {"Net Total",10}\n");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    double totalNetTotal = 0;
                    double totalTax = 0;
                    double totalPrice = 0;

                    foreach (var t in transaction)
                    {
                        double total = t.Game.Price * t.Quantity;
                        double tax = 0.13 * (total - t.Discount);
                        double netTotal = total - t.Discount + tax;


                        Console.WriteLine($"{t.OrderId,8} | {t.Date.ToString("dd-MMM-yy"),12} | {t.Customer.Name,-14} | {t.Game.Name,-14} | {t.Game.Price.ToString("C"),10} | {t.Quantity + " Qty",10} | {t.Discount.ToString("C"),10} | {tax.ToString("C"),10} | {netTotal.ToString("C"),10}");


                        totalNetTotal = totalNetTotal + netTotal;
                        totalTax = totalTax + tax;
                        totalPrice = totalPrice + total;
                    }

                    var totalDiscount = (from o in context.Orders
                                         select o.Discount).Sum();


                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n------------------------------------------------------");
                    Console.WriteLine("--------------- TOTAL CUSTOMER PURCHASES -------------");
                    Console.WriteLine("------------------------------------------------------\n");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"{"Total Games Amount Sold",-25} = + {totalPrice.ToString("C"),-10}");
                    Console.WriteLine($"{"Total Tax",-25} = + {totalTax.ToString("C"),-10}");
                    Console.WriteLine($"{"Total Discount Given",-25} = - {totalDiscount.ToString("C"),-10}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("------------------------------------------------------");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"\n{"Total Sale",-25} = + {totalNetTotal.ToString("C"),-10}");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("------------------------------------------------------");
                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
