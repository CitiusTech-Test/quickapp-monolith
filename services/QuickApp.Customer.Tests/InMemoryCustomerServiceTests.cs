using QuickApp.Customer.Contracts;
using QuickApp.Customer.Service;
using Xunit;

namespace QuickApp.Customer.Tests
{
    public class InMemoryCustomerServiceTests
    {
        private static InMemoryCustomerService NewService()
        {
            var fixedNow = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return new InMemoryCustomerService(() => fixedNow);
        }

        private static CreateCustomerRequest NewCreate(string name, string email) => new()
        {
            Name = name,
            Email = email,
            PhoneNumber = "555-0100",
            Address = "1 Main St",
            City = "Metropolis",
            Gender = Gender.Female
        };

        [Fact]
        public void Create_AssignsId_AndPersistsFields()
        {
            var svc = NewService();

            var created = svc.Create(NewCreate("Alice", "alice@example.com"));

            Assert.True(created.Id > 0);
            Assert.Equal("Alice", created.Name);
            Assert.Equal("alice@example.com", created.Email);
            Assert.Equal(Gender.Female, created.Gender);
            Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), created.CreatedDate);
        }

        [Fact]
        public void Crud_RoundTrip_Works()
        {
            var svc = NewService();

            var created = svc.Create(NewCreate("Bob", "bob@example.com"));

            var fetched = svc.GetById(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Bob", fetched!.Name);

            var updated = svc.Update(created.Id, new UpdateCustomerRequest
            {
                Name = "Bobby",
                Email = "bobby@example.com",
                Gender = Gender.Male
            });
            Assert.NotNull(updated);
            Assert.Equal("Bobby", updated!.Name);
            Assert.Equal("bobby@example.com", updated.Email);
            Assert.Equal(Gender.Male, updated.Gender);

            Assert.True(svc.Delete(created.Id));
            Assert.Null(svc.GetById(created.Id));
            Assert.False(svc.Delete(created.Id));
        }

        [Fact]
        public void Update_UnknownId_ReturnsNull()
        {
            var svc = NewService();
            var result = svc.Update(999, new UpdateCustomerRequest { Name = "X", Email = "x@x.com" });
            Assert.Null(result);
        }

        [Fact]
        public void GetById_Unknown_ReturnsNull()
        {
            var svc = NewService();
            Assert.Null(svc.GetById(42));
        }

        [Fact]
        public void GetAllCustomersData_ReturnsAll_OrderedByName()
        {
            var svc = NewService();
            svc.Create(NewCreate("Charlie", "c@example.com"));
            svc.Create(NewCreate("Alice", "a@example.com"));
            svc.Create(NewCreate("Bob", "b@example.com"));

            var all = svc.GetAllCustomersData().ToList();

            Assert.Equal(3, all.Count);
            Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, all.Select(c => c.Name).ToArray());
        }

        [Fact]
        public void GetTopActiveCustomers_UsesSuppliedActivityRanking()
        {
            var svc = NewService();
            var alice = svc.Create(NewCreate("Alice", "a@example.com"));
            var bob = svc.Create(NewCreate("Bob", "b@example.com"));
            var carol = svc.Create(NewCreate("Carol", "c@example.com"));

            // Order service says: Bob most active, then Carol, then Alice.
            svc.SetActivityRanking(new[] { bob.Id, carol.Id, alice.Id });

            var top2 = svc.GetTopActiveCustomers(2).ToList();

            Assert.Equal(2, top2.Count);
            Assert.Equal(new[] { "Bob", "Carol" }, top2.Select(c => c.Name).ToArray());
        }

        [Fact]
        public void GetTopActiveCustomers_WithoutRanking_IsDeterministicByName()
        {
            var svc = NewService();
            svc.Create(NewCreate("Charlie", "c@example.com"));
            svc.Create(NewCreate("Alice", "a@example.com"));
            svc.Create(NewCreate("Bob", "b@example.com"));

            var top2 = svc.GetTopActiveCustomers(2).ToList();

            Assert.Equal(new[] { "Alice", "Bob" }, top2.Select(c => c.Name).ToArray());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetTopActiveCustomers_NonPositiveCount_ReturnsEmpty(int count)
        {
            var svc = NewService();
            svc.Create(NewCreate("Alice", "a@example.com"));

            Assert.Empty(svc.GetTopActiveCustomers(count));
        }
    }
}
