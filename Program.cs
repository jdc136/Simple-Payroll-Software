using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace PayrollSoftware {

    class Staff {
        // Fields
        private float hourlyRate;
        private int hWorked;

        // Properties
        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }
        public int HoursWorked {
            get {
                return hWorked;
            }
            set {
                if (value /* important! (had HoursWorked) */ > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }

        // Constructor(s)
        public Staff(string name, float rate) {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        // Method(s)
        public virtual void CalculatePay() {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hWorked * hourlyRate;
            TotalPay = BasicPay;
        }

        public override string ToString() {
            //return base.ToString(); returns "PayrollSoftware.Staff"
            return $"Name of Staff: {NameOfStaff}, Hourly Rate: {hourlyRate}, Hours Worked: {hWorked}, Basic Pay: {BasicPay}, Total Pay: {TotalPay}";
        }
    }

    class Manager : Staff {
        // Fields/Properties
        private const float managerHourlyRate = 50;
        public int Allowance { get; private set; }

        // Constructor
        public Manager(string name) : base (name, managerHourlyRate) { }

        // Methods
        public override void CalculatePay() {
            base.CalculatePay();
            if (HoursWorked > 160) {
                Allowance = 1000;
                TotalPay = TotalPay + Allowance;
            } else {
                Allowance = 0;
            }
        }

        public override string ToString() {
            return $"Name of Staff: {NameOfStaff}, Hourly Rate: {managerHourlyRate}, Hours Worked: {HoursWorked}, Basic Pay: {BasicPay}, Total Pay: {TotalPay}";
        }
    }

    class Admin : Staff {
        // Fields/Properties
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30f;
        public float Overtime { get; private set; }

        // Constructor
        public Admin(string name) : base(name, adminHourlyRate) { }

        // Methods
        public override void CalculatePay() {
            base.CalculatePay();
            if (HoursWorked > 160) {
                Overtime = overtimeRate * (HoursWorked - 160);
                TotalPay = TotalPay + Overtime;
            } else {
                Overtime = 0;
            }
        }

        public override string ToString() {
            return $"Name of Staff: {NameOfStaff}, Hourly Rate: {adminHourlyRate}, Hours Worked: {HoursWorked}, Basic Pay: {BasicPay}, Total Pay: {TotalPay}";
        }
    }

    class FileReader {
        // Method
        public List<Staff> ReadFile() {
            // Fields
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = { ", " };

            // Logic
            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path)) {
                    while (!sr.EndOfStream) {
                        //Console.WriteLine(sr.ReadLine());
                        result = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        if (result[1].Equals("Manager")) {
                            myStaff.Add(new Manager(result[0]));
                        } else {
                            myStaff.Add(new Admin(result[0]));
                        }
                    }
                    sr.Close();
                }
            } else {
                Console.WriteLine("No staff.txt file found");
            }
            return myStaff;
        }
    }

    class PaySlip {
        // Fields
        private int month;
        private int year;

        enum MonthsOfYear {
            JAN = 1
            , FEB = 2
            , MAR = 3
            , APR = 4
            , MAY = 5
            , JUN = 6
            , JUL = 7
            , AUG = 8
            , SEP = 9
            , OCT = 10
            , NOV = 11
            , DEC = 12
        }

        // Constructor
        public PaySlip(int payMonth, int payYear) {
            month = payMonth;
            year = payYear;
        }

        // Methods
        public void GeneratePaySlip(List<Staff> myStaff) {
            string path;

            foreach (Staff f in myStaff) {
                path = "reports\\" + f.NameOfStaff + ".txt";
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine($"PAYSLIP FOR {(MonthsOfYear)month} {year}\n"
                    + "==========================\n"
                    + $"Name of Staff: {f.NameOfStaff}\n"
                    + $"Hours Worked: {f.HoursWorked}\n\n"
                    + $"Basic Pay: $ {f.BasicPay:n}");

                if (f.GetType() == typeof(Manager))
                    sw.WriteLine($"Allowance: $ {((Manager)f).Allowance:n}\n");
                else
                    sw.WriteLine($"Overtime Pay: $ {((Admin)f).Overtime:n}\n");

                sw.WriteLine("==========================\n"
                    + $"Total Pay: $ {f.TotalPay:n}\n"
                    + "=========================="
                    );
                sw.Close();
            }           
        }

        public void GenerateSummary(List<Staff> myStaff) {            
            var result =
                from f in myStaff
                where f.HoursWorked < 10
                orderby f.NameOfStaff ascending
                select new { f.NameOfStaff, f.HoursWorked };
            
            string path = "reports\\summary.txt";

            using (StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine($"Staff with less than 10 working hours\n");
                foreach (var f in result /* important! (had myStaff) */) {
                    sw.WriteLine($"Name of Staff: {f.NameOfStaff}, Hours Worked: {f.HoursWorked}");
                }
                sw.Close();
            } 
        }

        public override string ToString() {
            return $"Month: {month}, Year: {year}";
        }
    }

    class Program {
        static void Main(string[] args) {

            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0;
            int year = 0;

            while (year == 0) {
                Console.Write("Please enter the year: ");
                try {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message + " Please try again.");
                    //year = 0;
                }
            }

            while (month == 0) {
                Console.Write("Please enter the month: ");
                try {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12) {
                        Console.WriteLine("Please enter a valid number for month, 1-12");
                        month = 0;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message + " Please try again.");
                    //month = 0;
                }
            }

            // Add items to our myStaff list
            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++) {
                try {
                    Console.Write($"Enter hours worked for {myStaff[i].NameOfStaff}: ");
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    myStaff[i].ToString();
                }
                catch (Exception e) {
                    if (myStaff[i].HoursWorked < 0 || myStaff[i].HoursWorked > 81) {
                        Console.WriteLine("Number of hours worked must be between 0 and 80, please try again");
                        i--;
                    }
                }
            }

            // Generate pay reports
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();

        }
    }
}
