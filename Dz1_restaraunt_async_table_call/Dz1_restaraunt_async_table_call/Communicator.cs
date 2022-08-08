using Dz1_restaraunt_async_table_call.Entitys;

namespace Dz1_restaraunt_async_table_call
{
    internal static class Communicator
    {
        internal static async Task InformClientAboutBookingAsync(Table? table)
        {
            await Task.Delay(2000);

            if (table is null)
            {
                Console.WriteLine($"УВЕДОМЛЕНИЕ асинхронно! Все столы заняты, попробуйте позже.");
            }
            else
            {
                Console.WriteLine($"УВЕДОМЛЕНИЕ асинхронно! Готово, ваш стол={table.Id}");
            }
        }

        internal static async Task InformClientAboutFailedCanselBookingAsync(int tableNumber)
        {
            await Task.Delay(1000);

            Console.WriteLine($"УВЕДОМЛЕНИЕ асинхронно! Да этот стол #{tableNumber} и так свободен был, что вы нас от работы отвлекаете!");
        }

        internal static async Task InformClientAboutCancelBookingAsync(Table table, bool isSucces)
        {
            await Task.Delay(1000);

            Console.WriteLine($"УВЕДОМЛЕНИЕ асинхронно! Снятие брони с стола номер {table.Id} = {isSucces}");
        }
    }
}
