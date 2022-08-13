using static Dz1RestarauntAsyncTableCall.Entities.EnumState;

namespace Dz1RestarauntAsyncTableCall.Entities
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
