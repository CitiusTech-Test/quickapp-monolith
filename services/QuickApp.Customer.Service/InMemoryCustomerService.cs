using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// In-memory stub implementation of <see cref="ICustomerService"/> for the decomposition POC.
    /// Deterministic and dependency-free (no EF, no database). Replace with a real EF-backed
    /// repository against the Customer service's own database schema.
    /// </summary>
    public class InMemoryCustomerService : ICustomerService
    {
        private readonly Dictionary<int, Customer> _store = new();
        private readonly Func<DateTime> _clock;
        private int _nextId = 1;

        // Activity ranking (most-active customer id first) supplied by the Order service.
        // Replaces the monolith's Customer.Orders navigation / DB join.
        private IReadOnlyList<int> _activityRanking = Array.Empty<int>();

        public InMemoryCustomerService(Func<DateTime>? clock = null)
        {
            _clock = clock ?? (() => DateTime.UtcNow);
        }

        public IEnumerable<CustomerDto> GetAllCustomersData() =>
            _store.Values
                .OrderBy(c => c.Name, StringComparer.Ordinal)
                .Select(ToDto)
                .ToList();

        public IEnumerable<CustomerDto> GetTopActiveCustomers(int count)
        {
            if (count <= 0)
                return Array.Empty<CustomerDto>();

            // Order by the externally-supplied activity ranking first (most active), then fall
            // back to a deterministic ordering (by name) for customers with no recorded activity.
            var rank = new Dictionary<int, int>();
            for (var i = 0; i < _activityRanking.Count; i++)
                rank[_activityRanking[i]] = i;

            return _store.Values
                .OrderBy(c => rank.TryGetValue(c.Id, out var r) ? r : int.MaxValue)
                .ThenBy(c => c.Name, StringComparer.Ordinal)
                .Take(count)
                .Select(ToDto)
                .ToList();
        }

        public CustomerDto? GetById(int id) =>
            _store.TryGetValue(id, out var customer) ? ToDto(customer) : null;

        public CustomerDto Create(CreateCustomerRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var now = _clock();
            var customer = new Customer
            {
                Id = _nextId++,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                City = request.City,
                Gender = request.Gender,
                CreatedDate = now,
                UpdatedDate = now
            };

            _store[customer.Id] = customer;
            return ToDto(customer);
        }

        public CustomerDto? Update(int id, UpdateCustomerRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (!_store.TryGetValue(id, out var customer))
                return null;

            customer.Name = request.Name;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.Gender = request.Gender;
            customer.UpdatedDate = _clock();

            return ToDto(customer);
        }

        public bool Delete(int id) => _store.Remove(id);

        public void SetActivityRanking(IReadOnlyList<int> customerIdsByActivityDesc)
        {
            _activityRanking = customerIdsByActivityDesc ?? Array.Empty<int>();
        }

        private static CustomerDto ToDto(Customer c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            City = c.City,
            Gender = c.Gender,
            CreatedBy = c.CreatedBy,
            UpdatedBy = c.UpdatedBy,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.UpdatedDate
        };
    }
}
