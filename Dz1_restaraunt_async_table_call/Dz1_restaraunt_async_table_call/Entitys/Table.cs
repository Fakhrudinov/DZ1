using static Dz1_restaraunt_async_table_call.Entitys.EnumState;

namespace Dz1_restaraunt_async_table_call.Entitys
{
    internal class Table
    {
        internal State State { get; private set; }
        internal int SeatsCount { get; }
        internal int Id { get; }

        internal Table(int id)
        {
            Id = id;
            State = State.Free;

            Random rand = new Random();
            SeatsCount = rand.Next(2, 5);
        }

        internal bool SetState(State state)
        {
            if (state == State)
            {
                return false;
            }

            State = state;
            return true;
        }
    }
}
