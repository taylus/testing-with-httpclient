using System.Collections.Generic;

namespace Example.Services
{
    public class Foo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Data { get; private set; } = new Dictionary<string, string>();

        public Foo(int id, string name, Dictionary<string, string> data)
        {
            Id = id;
            Name = name;
            Data = data;
        }
    }
}
