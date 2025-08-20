namespace ElevatorChallenge
{
    public class Elevator {

            private bool[] floorReady;
            public int CurrentFloor = 1;
            private int topfloor;
            public ElevatorStatus Status = ElevatorStatus.IDLE; 

            public Elevator (int NumberOfFloors = 10)
            {
                floorReady = new bool[NumberOfFloors + 1];
                topfloor = NumberOfFloors;
            }

            private void Stop(int floor) 
            {
                Status = ElevatorStatus.IDLE;
                CurrentFloor = floor;
                floorReady[floor] = false;
                Console.WriteLine ("IDLE at floor {0}", floor);
            }

            private void MoveDown(int floor) 
            {
                for (int i = CurrentFloor; i >= 1; i--) 
                {
                    if (floorReady[i])
                        Stop(floor);
                    else
                        continue;
                }

                Status = ElevatorStatus.IDLE;
                Console.WriteLine ("Waiting..");
            }

            private void MoveUp(int floor) 
            {
                for (int i = CurrentFloor; i <= topfloor; i++) 
                {
                        if (floorReady[i])
                            Stop(floor);
                        else 
                            continue;
                }

                Status = ElevatorStatus.IDLE;
                Console.WriteLine ("Waiting..");
            }

            void StayPut () 
            { 
                Console.WriteLine ("That's our current floor"); 
            }

            public void FloorPress (int floor)
            {
                if (floor > topfloor) {
                    Console.WriteLine ("We only have {0} floors", topfloor);
                    return;
                }

                floorReady[floor] = true;
        
                switch (Status) {

                    case ElevatorStatus.MOVING_DOWN :
                        MoveDown(floor);
                        break;

                    case ElevatorStatus.IDLE:
                        if (CurrentFloor < floor)
                            MoveUp(floor);
                        else if (CurrentFloor == floor)
                            StayPut();
                        else
                            MoveDown(floor);
                        break;

                    case ElevatorStatus.MOVING_UP :
                            MoveUp(floor);
                            break;

                    default:
                        break;
                }


            }

            public enum ElevatorStatus
            {
                IDLE, 
                MOVING_UP, 
                MOVING_DOWN,
                DOOR_OPEN
            }	
    }
}