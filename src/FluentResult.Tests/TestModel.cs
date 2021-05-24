using System;

namespace FluentResult.Tests
{
    /// <summary>A test model.</summary>
    public class TestModel
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }

        /// <summary>Generates test model.</summary>
        public static TestModel Generate() =>
            new TestModel { Id = new Random().Next(1, 100) };
    }
}
