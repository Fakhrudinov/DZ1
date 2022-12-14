using Messaging;
using Messaging.Interfaces;
using System.Timers;

namespace Dz1RestarauntAsyncTableCall.Entities
{
    internal class Restaurant
    {
        private readonly List<Table> _tables = new List<Table>();
        private System.Timers.Timer _timerResetTablesBooking;

        private Mutex _mutex = new Mutex();

        private readonly IProducer _producer = new Producer("BookingNotification");

        internal Restaurant(int totalTablesInRestaurant)
        {
            for (int i = 1; i <= totalTablesInRestaurant; i++)
            {
                _tables.Add(new Table(i));
            }

            _timerResetTablesBooking = new System.Timers.Timer(60_000);
            _timerResetTablesBooking.AutoReset = true;
            _timerResetTablesBooking.Elapsed += new ElapsedEventHandler(ResetAllTablesBooking);
            _timerResetTablesBooking.Start();
        }

        private async void ResetAllTablesBooking(object? sender, ElapsedEventArgs e)
        {
            await Task.Run( async () =>
            {
                Console.WriteLine("Автоматическое снятие бронирования со всех столов");

                foreach (Table table in _tables)
                {
                    if (table.State == EnumState.State.Booked)
                    {
                        bool isSucces = table.SetState(EnumState.State.Free);
                        _producer.Send($"УВЕДОМЛЕНИЕ асинхронно! Снятие брони с стола номер {table.Id} = {isSucces}");
                    }                    
                }
            });
        }

        internal void BookFreeTable(int countOfPersons)
        {
            Console.WriteLine("\tСинхронный заказ столика, ожидайте выполнения");

            _mutex.WaitOne();
            Table table = GetFreeTable(countOfPersons);
            table?.SetState(EnumState.State.Booked);
            _mutex.ReleaseMutex();

            Thread.Sleep(5000);

            if (table is null)
            {
                Console.WriteLine($"Синхронный заказ - Все столы заняты, попробуйте позже.");
            }
            else
            {
                Console.WriteLine($"Синхронный заказ - Готово, ваш стол={table.Id}");
            }
        }

        private Table GetFreeTable(int countOfPersons)
        {
            return _tables.FirstOrDefault(t =>
                t.SeatsCount > countOfPersons
                && t.State == EnumState.State.Free);
        }

        internal void DeleteBookingForTable(int tableNumber)
        {
            Console.WriteLine("\tСинхронный заказ - Снимем бронь со столика " + tableNumber);
            
            Table table = _tables.FirstOrDefault(t => t.Id == tableNumber);
            Thread.Sleep(3000);

            if (table is null)
            {
                InformManagementAboutNullProblem("Снятие брони синхронно", tableNumber);
            }
            else if (table.State== EnumState.State.Free)
            {
                Console.WriteLine($"Синхронный заказ - Да этот стол #{tableNumber} и так свободен был, что вы нас от работы отвлекаете!");
            }
            else
            {
                bool isSuccesFree = table.SetState(EnumState.State.Free);
                Console.WriteLine($"Синхронный заказ - Снятие брони с стола номер {tableNumber} = {isSuccesFree}");
            }
        }

        internal async void BookFreeTableAsync(int countOfPersons)
        {
            
            Console.WriteLine("\tАсинхронный заказ столика, уведомление о результате придет после проверки");
            await Task.Run(async () =>
            {
                _mutex.WaitOne();
                Table table = GetFreeTable(countOfPersons);
                table?.SetState(EnumState.State.Booked);
                _mutex.ReleaseMutex();

                await Task.Delay(2000);

                if (table is null)
                {
                    _producer.Send($"УВЕДОМЛЕНИЕ асинхронно! Все столы заняты, попробуйте позже.");
                }
                else
                {
                    _producer.Send($"УВЕДОМЛЕНИЕ асинхронно! Готово, ваш стол={table.Id}");
                }
            });
            
        }

        internal void PrintTablesStatus()
        {
            Console.WriteLine("\tСостояние столиков:");
            foreach (Table table in _tables)
            {
                Console.WriteLine($"\t\tСтол#{table.Id} - {table.State}");
            }
        }

        internal async void DeleteBookingForTableAsync(int tableNumber)
        {
            Console.WriteLine($"\tСнимем асинхронно бронь сo столика {tableNumber} и оповестим вас");
            
            await Task.Run(async () =>
            {
                Table table = _tables.FirstOrDefault(t => t.Id == tableNumber);
                await Task.Delay(2000);

                if (table is null)
                {
                    InformManagementAboutNullProblem("Снятие брони асинхронно", tableNumber);
                }
                else if (table.State == EnumState.State.Free)
                {
                    _producer.Send($"УВЕДОМЛЕНИЕ асинхронно! Да этот стол #{tableNumber} и так свободен был, что вы нас от работы отвлекаете!");
                }
                else
                {
                    bool isSucces = table.SetState(EnumState.State.Free);

                    _producer.Send($"УВЕДОМЛЕНИЕ асинхронно! Снятие брони с стола номер {table.Id} = {isSucces}");
                }
            });
        }

        private void InformManagementAboutNullProblem(string action, int tableNumber)
        {
            Console.WriteLine($"Внимание! Что-то пошло не так при выполнении '{action}' для стола #{tableNumber}. " +
                $"Похоже у нас украли стол, так как вернулся null...");
        }
    }
}
