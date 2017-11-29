using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace VMS.TPS
{
    class Methods
    {

        // SELECT COURSE Index
        static public int Select_course_Index(Patient my_patient)
        {
            // List all courses
            if (my_patient.Courses.Any())
            {
                String course_Id = null;
                Console.WriteLine();
                Console.WriteLine("Courses:");
                int i = 0;
                foreach (Course item in my_patient.Courses)
                {
                    course_Id = item.Id;
                    Console.WriteLine("[" + i.ToString() + "] " + course_Id);
                    ++i;
                }
            }
            else
            { return -1; }

            //SELECT course from list of all courses
            Console.WriteLine();
            Console.WriteLine("Select course [index]:");
            // Do While evaluates always a variable outside its scope
            int course_index = new int();
            bool result = false;
            do
            {
                String Course_Index = Console.ReadLine();
                result = Int32.TryParse(Course_Index, out course_index);
                result = result && (course_index >= 0) && (course_index <= my_patient.Courses.Count() - 1);
            } while (!result);

            return course_index;
        }

        // SLECT PLAN Index 
        static public int Select_Plan_Index(Course my_course)
        {
            // List all plans in selected course
            String plan_Id = null;
            Console.WriteLine();
            Console.WriteLine("Plans:");
            if (my_course.PlanSetups.Any())
            {
                int i = 0;
                foreach (PlanSetup item in my_course.PlanSetups)
                {
                    plan_Id = item.Id;
                    Console.WriteLine("[" + i.ToString() + "] " + plan_Id);
                    ++i;
                }
            }
            else
            { return -1; }

            //SELECT plan from list of all plans in selected course
            Console.WriteLine();
            Console.WriteLine("Select plan [index]:");
            // Do While evaluates always a variable outside its scope
            int plan_index = new int();
            bool result = false;
            do
            {
                String Plan_Index = Console.ReadLine();
                result = Int32.TryParse(Plan_Index, out plan_index);
                result = result && (plan_index >= 0) && (plan_index <= my_course.PlanSetups.Count() - 1);
            } while (!result);

            return plan_index;

        }

    }
}
