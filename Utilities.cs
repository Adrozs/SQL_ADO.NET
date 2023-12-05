using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLLAB1
{
    // Class to hold calculations and misc methods
    internal class Utilities
    {

        public static string ChooseNameSort()
        {
            // Sort on first name

            // Creates nav options, promts the user for which one then depending on the choice sortColumn is changed to match
            string[] navOptions1 = { "First name", "Last name" };

            int choice = NavMethods.Navigation(navOptions1, "Do you want to sort on first- or last names?");

            string sortColumn;
            if (choice == 0)
                sortColumn = "firstName";
            else
                sortColumn = "lastName";

            return sortColumn;
        }

        public static string ChooseSortOrder() 
        {
            // Sort ascending or descending order

            // Creates nav options, promts the user for which one then depending on the choice sortOrder is changed to match
            string[] navOptions2 = { "Ascending", "Descending" };

            int choice = NavMethods.Navigation(navOptions2, "Do you want to sort in ascending or descending order?");

            // Pascal case instead of CAPS on Asc and Desc as there are times we print out "{sortOrder}ending" to get it to say "Ascending" "Descending". Doesn't matter for rest of the code as SQL queries are case insensitive.
            string sortOrder;
            if (choice == 0)
                sortOrder = "Asc";
            else
                sortOrder = "Desc";

            return sortOrder;
        }




    }
}
