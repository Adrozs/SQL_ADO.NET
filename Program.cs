namespace SQLLAB1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Loops until user chooses to exit program 
            while (true)
            {
                // Clears console window each time user comes back to the menu
                Console.Clear();

                // Create array with all navigation options
                string[] navOptions = {
                    "Get all students", 
                    "Get all students in a class", 
                    "Get all grades", 
                    "Get average course grades",
                    "Add students", 
                    "Get staff", 
                    "Add staff", 
                    "Exit program"};

                // Call navigation which returns what method the user chose to the MethodSelector
                UserMethods.MethodSelect(NavMethods.Navigation(navOptions, "What do you want to do? \n"));

                // After method has run promt user to press enter to continue
                Console.WriteLine(); // New line for formatting
                Console.Write("Press ");


                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.Write("[ENTER]");
                Console.ResetColor();

                Console.Write(" to continue");
                Console.ReadKey();

            }
        }
    }
}