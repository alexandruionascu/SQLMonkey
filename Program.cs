using System;
using System.IO;


namespace SQLMonkey {
    class Program {
        static void Main(string[] args) {
            var testUser = new TestUser {
                age = 23,
                id = 1,
                name = "boss"
            };

           var path = Directory.GetParent(
               Directory.GetParent(Directory.GetCurrentDirectory()).ToString()
           ).ToString();
        
           var connectionString = 
                String.Format(@"
                Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDbFilename={0}\Database.mdf;
                Integrated Security=True"
            , path);

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
