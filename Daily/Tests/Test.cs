namespace Daily
{
    class Test
    {
        public Test(string name, string suite)
        {
            Name = name;
            Suite = suite;
        }

        public string Name { get; set; }
        public string Suite { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
