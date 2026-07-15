using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service.Tests
{
    public class InMemoryCustomerServiceTests
    {
        private static readonly DateTime FixedNow = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static InMemoryCustomerService NewService(out RecordingCustomerEventPublisher publisher)
        {
            publisher = new RecordingCustomerEventPublisher();
            return new InMemoryCustomerService(publisher, () => FixedNow);
        }

        private static CreateCustomerRequest NewCreate(string name, string email, string? city = "Metropolis") => new()
        {
            Name = name,
            Email = email,
            PhoneNumber = "555-0100",
            Address = "1 Main St",
            City = city,
            Gender = Gender.Female
        };

        [Fact]
        public async Task CreateAsync_AssignsId_PersistsFields_AndPublishesEvent()
        {
            var svc = NewService(out var publisher);

            var result = await svc.CreateAsync(NewCreate("Alice", "alice@example.com"));

            Assert.True(result.Customer.Id > 0);
            Assert.Equal("Alice", result.Customer.Name);
            Assert.Equal("alice@example.com", result.Customer.Email);
            Assert.Equal(Gender.Female, result.Customer.Gender);
            Assert.Equal(FixedNow, result.Customer.CreatedDate);

            Assert.Equal(CustomerChangeKind.Created, result.Event.ChangeKind);
            Assert.Equal(result.Customer.Id, result.Event.CustomerId);
            var published = Assert.Single(publisher.Published);
            Assert.Equal(CustomerChangeKind.Created, published.ChangeKind);
            Assert.Equal("alice@example.com", published.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCustomer_WhenPresent()
        {
            var svc = NewService(out _);
            var created = await svc.CreateAsync(NewCreate("Bob", "bob@example.com"));

            var fetched = await svc.GetByIdAsync(created.Customer.Id);

            Assert.NotNull(fetched);
            Assert.Equal("Bob", fetched!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenMissing()
        {
            var svc = NewService(out _);
            Assert.Null(await svc.GetByIdAsync(42));
        }

        [Fact]
        public async Task UpdateAsync_ChangesFields_AndPublishesUpdatedEvent()
        {
            var svc = NewService(out var publisher);
            var created = await svc.CreateAsync(NewCreate("Bob", "bob@example.com"));

            var updated = await svc.UpdateAsync(created.Customer.Id, new UpdateCustomerRequest
            {
                Name = "Bobby",
                Email = "bobby@example.com",
                Gender = Gender.Male
            });

            Assert.NotNull(updated);
            Assert.Equal("Bobby", updated!.Customer.Name);
            Assert.Equal("bobby@example.com", updated.Customer.Email);
            Assert.Equal(Gender.Male, updated.Customer.Gender);
            Assert.Equal(CustomerChangeKind.Updated, updated.Event.ChangeKind);
            Assert.Equal(2, publisher.Published.Count);
            Assert.Equal(CustomerChangeKind.Updated, publisher.Published[1].ChangeKind);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_ForUnknownId()
        {
            var svc = NewService(out var publisher);

            var result = await svc.UpdateAsync(999, new UpdateCustomerRequest { Name = "X", Email = "x@x.com" });

            Assert.Null(result);
            Assert.Empty(publisher.Published);
        }

        [Fact]
        public async Task SearchAsync_EmptyQuery_ReturnsAll_OrderedByName()
        {
            var svc = NewService(out _);
            await svc.CreateAsync(NewCreate("Charlie", "c@example.com"));
            await svc.CreateAsync(NewCreate("Alice", "a@example.com"));
            await svc.CreateAsync(NewCreate("Bob", "b@example.com"));

            var all = await svc.SearchAsync(new CustomerSearchRequest());

            Assert.Equal(new[] { "Alice", "Bob", "Charlie" }, all.Select(c => c.Name).ToArray());
        }

        [Fact]
        public async Task SearchAsync_MatchesNameEmailAndCity_CaseInsensitively()
        {
            var svc = NewService(out _);
            await svc.CreateAsync(NewCreate("Alice", "alice@example.com", city: "Gotham"));
            await svc.CreateAsync(NewCreate("Bob", "bob@contoso.com", city: "Metropolis"));

            var byName = await svc.SearchAsync(new CustomerSearchRequest { Query = "ali" });
            Assert.Equal(new[] { "Alice" }, byName.Select(c => c.Name).ToArray());

            var byCity = await svc.SearchAsync(new CustomerSearchRequest { Query = "gotham" });
            Assert.Equal(new[] { "Alice" }, byCity.Select(c => c.Name).ToArray());

            var byEmail = await svc.SearchAsync(new CustomerSearchRequest { Query = "contoso" });
            Assert.Equal(new[] { "Bob" }, byEmail.Select(c => c.Name).ToArray());
        }

        [Fact]
        public async Task SearchAsync_AppliesPaging()
        {
            var svc = NewService(out _);
            await svc.CreateAsync(NewCreate("Alice", "a@example.com"));
            await svc.CreateAsync(NewCreate("Bob", "b@example.com"));
            await svc.CreateAsync(NewCreate("Charlie", "c@example.com"));

            var page = await svc.SearchAsync(new CustomerSearchRequest { Skip = 1, Take = 1 });

            Assert.Equal(new[] { "Bob" }, page.Select(c => c.Name).ToArray());
        }

        [Fact]
        public async Task Operations_ThrowWhenCancelled()
        {
            var svc = NewService(out _);
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => svc.GetByIdAsync(1, cts.Token));
            await Assert.ThrowsAsync<OperationCanceledException>(() => svc.SearchAsync(new CustomerSearchRequest(), cts.Token));
            await Assert.ThrowsAsync<OperationCanceledException>(() => svc.CreateAsync(NewCreate("A", "a@a.com"), cts.Token));
        }

        [Fact]
        public async Task CustomerDto_DoesNotExposeOrderHistory()
        {
            // Guards the coupling rule: Customer contracts must not carry Order/OrderDetail/Product data.
            var forbidden = new[] { "Order", "OrderDetail", "Product", "ApplicationUser", "VM" };
            var propertyNames = typeof(CustomerDto).GetProperties().Select(p => p.Name).ToArray();

            Assert.DoesNotContain(propertyNames, name => forbidden.Any(f => name.Contains(f, StringComparison.Ordinal)));
            await Task.CompletedTask;
        }
    }
}
