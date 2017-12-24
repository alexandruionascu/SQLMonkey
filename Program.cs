using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLMonkey {
    class Program {
        static void Main(string[] args) {
            var testUser = new TestUser {
                age = 23,
                id = 1,
                name = "boss"
            };


           var connectionString = 
                @"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDbFilename=|DataDirectory|Database.mdf;
                Integrated Security=True";

            var monkey = new SQLMonkey(connectionString);
            var users = monkey.retrieve<TestUser>("users");
            foreach (var user in users) {
                user.age++;
                Console.WriteLine(user.age);
                monkey.update<TestUser>(user, "users");
            }
            
        }
    }
}
