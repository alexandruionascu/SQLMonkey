using System;
using System.Collections.Generic;
using System.Text;

namespace SQLMonkey {
   
    class TestUser {
        [PrimaryKey]
        public int id { get; set; }
        public int age { get; set; }
        public string name { get; set; }
    }
}
