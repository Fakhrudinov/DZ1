using Dz1RestarauntAsyncTableCall.Entities;
using System.Diagnostics;

namespace Dz1RestarauntAsyncTableCall
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int totalTablesInRestaurant = 10;
            Restaurant restaurant = new Restaurant(totalTablesInRestaurant);

            while (true)
            {
                Console.WriteLine("\r\n\tВыберите действие:\r\n" +
                    "\t\t0 Распечатать список столов и их состояние\r\n" +
                    "\t\t1 Забронировать свободный столик асинхронно\r\n" +
                    "\t\t2 Забронировать свободный столик синхронно\r\n" +
                    "\t\t3 Снять бронь асинхронно\r\n" +
                    "\t\t4 Снять бронь синхронно\r\n");

                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int choise) && ( choise < 0 || choise > 4 ))
                {
                    Console.WriteLine("\tВнимание, некорректный ввод! допускается только целые числа  0  1  2  3  4");
                    continue;
                }

                if (choise == 0)
                {
                    restaurant.PrintTablesStatus();
                    continue;
                }

                Stopwatch stopWatch = new Stopwatch();

                if (choise == 1)
                {
                    stopWatch.Start();
                    restaurant.BookFreeTableAsync(1);
                }
                else if (choise == 2)
                {
                    stopWatch.Start();
                    restaurant.BookFreeTable(1);
                }
                else
                {
                    bool tableNumberNotInputed = true;
                    while (tableNumberNotInputed)
                    {
                        Console.WriteLine("\tВыберите номер стола, которому отменяем бронь");

                        string userInputTableNumber = Console.ReadLine();

                        if (int.TryParse(userInputTableNumber, out int tableNumber))
                        {
                            if (tableNumber < 1 || // минимум
                                tableNumber > totalTablesInRestaurant) //максимум
                            {
                                Console.WriteLine($"Tакого столика не существует. Вводите целое число, с 1 по {totalTablesInRestaurant} включительно");
                                continue;
                            }

                            stopWatch.Start();
                            tableNumberNotInputed = false; // ввод номера стола завершен

                            if (choise == 3)
                            {
                                restaurant.DeleteBookingForTableAsync(tableNumber);
                            }
                            else
                            {
                                restaurant.DeleteBookingForTable(tableNumber);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Допустимы только целые числа!");
                        }
                    }
                }

                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                Console.WriteLine($"\tЗаказ завершен за время = {ts.Seconds:00}:{ts.Milliseconds:00}");
            }
        }
    }    
}

        

 