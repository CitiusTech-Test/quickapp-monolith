using QuickApp.Customer.Contracts;

namespace QuickApp.Customer.Service
{
    /// <summary>
    /// Deterministic, dependency-free in-memory stub of <see cref="ICustomerService"/> for the
    /// decomposition POC. It stores only customer-owned data (no EF, no database, no Orders join)
    /// and publishes a <see cref="CustomerChanged"/> event on every mutation. Replace with an
    /// EF-backed implementation against the Customer service's own schema.
    /// </summary>
    public sealed class InMemoryCustomerService : ICustomerService
    {
        private readonly Dictionary<int, CustomerDto> _store = new();
        private readonly ICustomerEventPublisher _publisher;
        private readonly Func<DateTime> _clock;
        private readonly object _gate = new();
        private int _nextId = 1;

        public InMemoryCustomerService(ICustomerEventPublisher publisher, Func<DateTime>? clock = null)
        {
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            _clock = clock ?? (() => DateTime.UtcNow);
        }

        public Task<CustomerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lock (_gate)
            {
                return Task.FromResult(_store.TryGetValue(id, out var customer) ? customer : null);
            }
        }

        public Task<IReadOnlyList<CustomerDto>> SearchAsync(CustomerSearchRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            var take = request.Take < 0 ? 0 : request.Take;
            var skip = request.Skip < 0 ? 0 : request.Skip;
            var query = request.Query?.Trim();

            lock (_gate)
            {
                IEnumerable<CustomerDto> matches = _store.Values;

                if (!string.IsNullOrEmpty(query))
                {
                    matches = matches.Where(c =>
                        c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        c.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        (c.City?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                IReadOnlyList<CustomerDto> result = matches
                    .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(c => c.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public async Task<CustomerMutationResult> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            CustomerDto customer;
            lock (_gate)
            {
                var now = _clock();
                customer = new CustomerDto
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
            }

            return await PublishAsync(CustomerChangeKind.Created, customer, cancellationToken);
        }

        public async Task<CustomerMutationResult?> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            cancellationToken.ThrowIfCancellationRequested();

            CustomerDto updated;
            lock (_gate)
            {
                if (!_store.TryGetValue(id, out var existing))
                    return null;

                updated = existing with
                {
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    City = request.City,
                    Gender = request.Gender,
                    UpdatedDate = _clock()
                };
                _store[id] = updated;
            }

            return await PublishAsync(CustomerChangeKind.Updated, updated, cancellationToken);
        }

        private async Task<CustomerMutationResult> PublishAsync(CustomerChangeKind kind, CustomerDto customer, CancellationToken cancellationToken)
        {
            var @event = new CustomerChanged
            {
                ChangeKind = kind,
                CustomerId = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                OccurredOnUtc = _clock()
            };

            await _publisher.PublishAsync(@event, cancellationToken);

            return new CustomerMutationResult { Customer = customer, Event = @event };
        }
    }
}
