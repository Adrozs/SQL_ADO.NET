using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SQLLAB1
{
    internal class UserMethods
    {
        public static void MethodSelect(int choice)
        {
            string connectionString = @"Data Source=(localdb)\.;Initial Catalog=SQLLab1;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Calls method depending on user input
                switch (choice)
                {
                    case 0:
                        GetStudents(connection);
                        break;
                    case 1:
                        GetStudentsInClass(connection);
                        break;
                    case 2:
                        GetGrades(connection);
                        break;
                    case 3:
                        GetAverageGrades(connection);
                        break;
                    case 4:
                        AddStudent(connection);
                        break;
                    case 5:
                        GetStaff(connection);
                        break;
                    case 6:
                        AddStaff(connection);
                        break;
                    case 7:
                        Environment.Exit(0);
                        break;
                    default:

                        break;
                }
            }
        }

        // EXTRA CHALLENGES
        // [] Control that the personal numbers are valid through SQL
        // [] Build a view to get grades that have been set the past month
        // [] Create another method for the user to choose. Where you can get the average grade based on gender, age group/birth year
        // based on the average for all the courses they have read

        // Note for me self
        // Okay then we need to change so student id is a 12-digit personnr instead (with a dash if possible(?))
        // as well we need to add gender and birth year to the student table 


        // Prints out all students in the chosen order
        private static void GetStudents(SqlConnection connection)
        {
            // Promts user and returns if they want to sort by first or last name
            string sortName = Utilities.ChooseNameSort();

            // Promts user and returns if they want to sort in ascending or descending order
            string sortOrder = Utilities.ChooseSortOrder();


            // NOTE! I didn't get it to work with parameters no matter what I tried. Some issue with it not being possible with ORDER BY. This should however be safe from SQL injections anyway by how the code is structured. No user input is ever taken in text form.
            string query = $"SELECT firstName, lastName FROM Students ORDER BY {sortName} {sortOrder}";


            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Create reader object from the command and print out the first and last names 
                using (SqlDataReader reader = command.ExecuteReader()) 
                { 
                    while (reader.Read())
                    {
                        string firstName = reader.GetString(reader.GetOrdinal("firstName")).TrimEnd();
                        string lastName = reader.GetString(reader.GetOrdinal("lastName")).TrimEnd();

                        Console.WriteLine($"{firstName} {lastName}");

                    }
                }
            }
        }
        
        // Prints all students in a chosen class sorted in the selected order
        private static void GetStudentsInClass(SqlConnection connection)
        {
        // Get all classes

            // Create a list of all the classes names in the classes table
            List<string> classes = new List<string>();

            using (SqlCommand command = new SqlCommand("SELECT name FROM Classes", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Gets the current class name 
                        string className = reader.GetString(reader.GetOrdinal("name"));

                        // Checks if that class name isn't already in the list. If it isn't add it to the list
                        if (!classes.Contains(className))
                        {
                            classes.Add(className);
                        }
                    }
                }
            }

        // Get specific class

            // Convert the list to an array to be used in the navigation options
            string[] navOptions = classes.ToArray();

            // Promt user which class they want to see
            int choice = NavMethods.Navigation(navOptions, "Which class' students do you wish to see?");

            // Sets chosen class to the name of the class (as choice returns the index of the classes array)
            string chosenClass = classes[choice];


        // Select sort order

            // Promts user and returns if they want to sort by first or last name
            string sortName = Utilities.ChooseNameSort();

            // Promts user and returns if they want to sort in ascending or descending order
            string sortOrder = Utilities.ChooseSortOrder();


            // Create query to get all full names of students of the chosen class that they belong to sorted in the chosen order
            string query = "SELECT firstName, lastName, Classes.name FROM Students " +
                "JOIN Classes ON Classes.classId = Students.classId " +
                $"ORDER BY {sortName} {sortOrder}";


        // Print out everything to user

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Informative text for user
                    Console.Clear();
                    Console.WriteLine($"Students in class {chosenClass}\n");

                    // Prints out each students full name and class name for the chosen class
                    while (reader.Read())
                    {
                        string firstName = reader.GetString(reader.GetOrdinal("firstName"));
                        string lastName = reader.GetString(reader.GetOrdinal("lastName"));
                        string className = reader.GetString(reader.GetOrdinal("name"));

                        // If class name matches the chosen class, print it out. Else don't do anything.
                        if (className == chosenClass)
                            Console.WriteLine($"{firstName} {lastName}");
                    }
                }
            }
        }

        // Prints all information of students and their courses that have been graded the past 30 days
        private static void GetGrades(SqlConnection connection)
        {
            // Gets user choice if on what they want to sort the grades on
            string[] navOptions = { "Students", "Grades", "Courses" };
            int choice = NavMethods.Navigation(navOptions, "How do you want to sort the results?");

            // Initialize variable
            string sortChoice = null;

            // Select the query phrase to sort on that the user chose
            switch (choice)
            {
                case 0:
                    sortChoice = "[Students].firstName";
                    break;
                case 1:
                    sortChoice = "[StudentCourses].grade";
                    break;
                case 2:
                    sortChoice = "[Courses].name";
                    break;
            }

            // Gets user choice if sorting by ASC or DESC
            string sortOrder = Utilities.ChooseSortOrder();


            // Query of student's full name and their grade in all courses they are in. Which have been graded the past 30 days. Ordered by the course name
            string query = "SELECT [StudentCourses].grade, [Students].firstName, [Students].lastName, [Courses].name " +
                "FROM StudentCourses " +
                "JOIN Students ON Students.studentId = StudentCourses.studentId " +
                "JOIN Courses ON Courses.CourseId = StudentCourses.CourseId " +
                "WHERE [StudentCourses].gradeDate >= CURRENT_TIMESTAMP -30 " +
                $"ORDER BY {sortChoice} {sortOrder}";


            Console.WriteLine($"Grades set the past 30 days. Sorted on {navOptions[choice]} in {sortOrder}ending order\n"); // Informative text

            // Prints out all results to user
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        string firstName = reader.GetString(reader.GetOrdinal("firstName"));
                        string lastName = reader.GetString(reader.GetOrdinal("lastName"));
                        string courseName = reader.GetString(reader.GetOrdinal("name"));
                        int grade = reader.GetInt32(reader.GetOrdinal("grade"));

                        Console.WriteLine($"Student: {firstName} {lastName} | Course: {courseName} | Grade: {grade}");
                    }
                }
            }
        }

        // Prints out all courses and their, min, max & average grade
        private static void GetAverageGrades(SqlConnection connection)
        {
            // Query that selects the avg, min, max grade. Along with the courses names
            string query = "SELECT AVG(sc.grade) avgGrade, MIN(sc.grade) minGrade, MAX(sc.grade) maxGrade, c.name " +
                "FROM StudentCourses sc " +
                "JOIN Courses c ON c.courseId = sc.courseId " +
                "GROUP BY c.name " +
                "ORDER BY c.name";

            // Text formatting and information
            Console.Clear();
            Console.WriteLine("Average grades for all courses\n");


            // Reads data and prints it out for user
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int avgGrade = reader.GetInt32(reader.GetOrdinal("avgGrade"));
                        int minGrade = reader.GetInt32(reader.GetOrdinal("minGrade"));
                        int maxGrade = reader.GetInt32(reader.GetOrdinal("maxGrade"));
                        string course = reader.GetString(reader.GetOrdinal("name"));

                        Console.WriteLine($"Course: {course} | Grades: Average: {avgGrade}, Highest {maxGrade}, Lowest {minGrade}");
                    }                  
                }
            }
        }

        // Creates new student and adds to the database
        private static void AddStudent(SqlConnection connection)
        {

        // Get class and class id saved into list and dict

            // Get a list of all the classes names in the classes table
            List<string> classes = new List<string>();

            // Get a dictionary of all the classes and their classId's - to be able to convert the class name to the classId to add the right class to the student
            Dictionary<string, int> classDict = new Dictionary<string, int>();

            // Get the class name and id
            using (SqlCommand command = new SqlCommand("SELECT name, classId FROM Classes", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Gets the current class name 
                        string className = reader.GetString(reader.GetOrdinal("name"));
                        int classId = reader.GetInt32(reader.GetOrdinal("classId"));

                        // Checks if that class' name isn't already in the list. If it isn't add it to the list and the dictionary
                        if (!classes.Contains(className))
                        {
                            classes.Add(className);
                            classDict.Add(className, classId);
                        }
                    }
                }
            }

        // User input

            // User enters new student information
            Console.WriteLine("Enter new students information\n");

            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = Console.ReadLine();

            // User chooses which class from the class list and saves the index of the class the user chose
            int classChoice = NavMethods.Navigation(classes.ToArray(), "Which class does the student belong to?");

            
            // Sets studentClass to the chosen class' name.
            string studentClass = classes.ToArray()[classChoice];

            // Sets classIdNum to the id of the class as it is stored in the dictionary
            int classIdNum = classDict[studentClass];


        // Query and db interaction

            // Query to insert all information 
            string query = "INSERT INTO Students (firstName, lastName, classId) " +
                "VALUES (@firstName, @lastName, @classId)";


            // Run commands
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@firstName", firstName);
                command.Parameters.AddWithValue("@lastName", lastName);
                command.Parameters.AddWithValue("@classId", classIdNum);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                }
            }

            // Print out confirmation to user
            Console.WriteLine($"Student {firstName} {lastName} in class {studentClass} added to the system");
        }


        // Prints out all staff members in the db in the order chosen by the user
        private static void GetStaff(SqlConnection connection)
        {

            // Get a list of all the staff positions
            List<string> positions = new List<string>();

            using (SqlCommand command = new SqlCommand("SELECT position FROM Staff", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Gets the current class name 
                        string pos = reader.GetString(reader.GetOrdinal("position"));

                        // Checks if that class' name isn't already in the list. If it isn't add it to the list and the dictionary
                        if (!positions.Contains(pos))
                        {
                            positions.Add(pos);
                        }
                    }
                }
            }
            
            // Promt user with the options in the navigation 
            int selection = NavMethods.Navigation(positions.ToArray(), "Choose the type of staff do you wish to see. \n");

            // Sets choice to the option the user selected in the menu
            string choice = positions.ToArray()[selection];


            // Depending on if user chose all or a specific type of staff it changes the query to match
            string sqlQuery;
            if (choice == "All")
            {
                sqlQuery = "SELECT name, position FROM Staff";
            }
            else
                sqlQuery = "SELECT name, position FROM Staff WHERE position = @Position";


            using (SqlCommand command = new SqlCommand(sqlQuery, connection))
            {
                // Changes the parameter in the query to the users input
                command.Parameters.AddWithValue("@Position", choice);
               
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Loops as long as there are things to read in the reader object
                    while (reader.Read())
                    {
                        // Gets the string that is at our position on the specified column and saves them before printing them out
                        string name = reader.GetString(reader.GetOrdinal("name")).TrimEnd();
                        string position = reader.GetString(reader.GetOrdinal("position")).TrimEnd();
                        Console.WriteLine($"Name: {name}, Position: {position}");
                    }
                }
            }
        }

        // Creates new staff and adds to the database
        private static void AddStaff(SqlConnection connection)
        {
            // User should be able to put in information on a new employee (Name and Position) that then will be saved in the db


            // Get user input
            Console.WriteLine("Enter new staffs information\n");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            Console.Write("Position: ");
            string position = Console.ReadLine();


            // Query to insert all information
            string query = "INSERT INTO Staff (name, position) " +
                "VALUES (@name, @position)";

            // Run commands
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@position", position);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    reader.Read();
                }
            }

            // Print out confirmation to user
            Console.WriteLine($"Staff {name} working as {position} added to the system");


        }

    }
}
