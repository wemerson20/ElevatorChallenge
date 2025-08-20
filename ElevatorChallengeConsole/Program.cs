namespace ElevatorChallenge
{
	public class Program 
	{
		private const string QUIT = "q";
        private const int NumberOfFloors = 10;

		public static void Main (string[] args)
		{
            int floor; Elevator elevator;		    
            elevator = new Elevator (NumberOfFloors);
            Console.WriteLine ("Welcome to the elevator!");
            string? input = string.Empty;

            while (input != QUIT) 
            {
                Console.WriteLine ("Please press which floor you would like to go to");

                input = Console.ReadLine ();
                if (input == QUIT)
                {
                    Console.WriteLine ("Ending the program...");
                    break;
                }
                else if (Int32.TryParse (input, out floor))
                {
                    elevator.FloorPress (floor);
                }
                else
                {
                    Console.WriteLine ("You have pressed an incorrect floor, Please try again");
                }
            }
        }
	}
}