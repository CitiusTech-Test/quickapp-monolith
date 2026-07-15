using QuickApp.Product.Contracts;
using QuickApp.Product.Contracts.Dtos;
using QuickApp.Product.Service;
using Xunit;

namespace QuickApp.Product.Tests
{
    public class InMemoryProductServiceTests
    {
        private static async Task<(IProductService svc, int categoryId)> NewServiceWithCategoryAsync()
        {
            IProductService svc = new InMemoryProductService();
            var category = await svc.CreateCategoryAsync(new CreateProductCategoryRequest { Name = "Beverages" });
            return (svc, category.Id);
        }

        private static CreateProductRequest NewProductRequest(int categoryId, string name = "Widget", int stock = 10)
            => new()
            {
                Name = name,
                BuyingPrice = 5m,
                SellingPrice = 9m,
                UnitsInStock = stock,
                ProductCategoryId = categoryId
            };

        [Fact]
        public async Task CreateProduct_AssignsIdAndPersists()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();

            var created = await svc.CreateProductAsync(NewProductRequest(categoryId));

            Assert.True(created.Id > 0);
            Assert.Equal("Widget", created.Name);
            Assert.Equal(categoryId, created.ProductCategoryId);

            var fetched = await svc.GetProductByIdAsync(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched!.Id);
        }

        [Fact]
        public async Task GetProducts_ReturnsAllCreated()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            await svc.CreateProductAsync(NewProductRequest(categoryId, "A"));
            await svc.CreateProductAsync(NewProductRequest(categoryId, "B"));

            var all = await svc.GetProductsAsync();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, p => p.Name == "A");
            Assert.Contains(all, p => p.Name == "B");
        }

        [Fact]
        public async Task UpdateProduct_ChangesFields()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var created = await svc.CreateProductAsync(NewProductRequest(categoryId));

            var updated = await svc.UpdateProductAsync(created.Id, new UpdateProductRequest
            {
                Name = "Renamed",
                BuyingPrice = 6m,
                SellingPrice = 12m,
                UnitsInStock = 3,
                IsActive = true,
                IsDiscontinued = true,
                ProductCategoryId = categoryId
            });

            Assert.NotNull(updated);
            Assert.Equal("Renamed", updated!.Name);
            Assert.Equal(12m, updated.SellingPrice);
            Assert.True(updated.IsDiscontinued);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNull_WhenMissing()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();

            var result = await svc.UpdateProductAsync(999, new UpdateProductRequest
            {
                Name = "x",
                ProductCategoryId = categoryId
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteProduct_RemovesIt()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var created = await svc.CreateProductAsync(NewProductRequest(categoryId));

            var deleted = await svc.DeleteProductAsync(created.Id);
            Assert.True(deleted);
            Assert.Null(await svc.GetProductByIdAsync(created.Id));

            Assert.False(await svc.DeleteProductAsync(created.Id));
        }

        [Fact]
        public async Task CreateProduct_Throws_WhenCategoryMissing()
        {
            IProductService svc = new InMemoryProductService();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => svc.CreateProductAsync(NewProductRequest(categoryId: 42)));
        }

        [Fact]
        public async Task Category_Association_IsResolvable()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var created = await svc.CreateProductAsync(NewProductRequest(categoryId));

            var category = await svc.GetCategoryByIdAsync(created.ProductCategoryId);

            Assert.NotNull(category);
            Assert.Equal("Beverages", category!.Name);
        }

        [Fact]
        public async Task ParentChild_Linkage_Works()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var parent = await svc.CreateProductAsync(NewProductRequest(categoryId, "Parent"));

            var childReq = NewProductRequest(categoryId, "Child");
            childReq.ParentId = parent.Id;
            var child = await svc.CreateProductAsync(childReq);

            Assert.Equal(parent.Id, child.ParentId);

            var children = await svc.GetChildProductsAsync(parent.Id);
            Assert.Single(children);
            Assert.Equal(child.Id, children[0].Id);
        }

        [Fact]
        public async Task DeleteParent_DetachesChildren_AndChildRemainsUpdatable()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var parent = await svc.CreateProductAsync(NewProductRequest(categoryId, "Parent"));

            var childReq = NewProductRequest(categoryId, "Child");
            childReq.ParentId = parent.Id;
            var child = await svc.CreateProductAsync(childReq);

            Assert.True(await svc.DeleteProductAsync(parent.Id));

            var refreshedChild = await svc.GetProductByIdAsync(child.Id);
            Assert.NotNull(refreshedChild);
            Assert.Null(refreshedChild!.ParentId);

            // Child stays updatable (no orphaned parent reference).
            var updated = await svc.UpdateProductAsync(child.Id, new UpdateProductRequest
            {
                Name = "Child Renamed",
                ProductCategoryId = categoryId
            });
            Assert.NotNull(updated);
            Assert.Equal("Child Renamed", updated!.Name);
        }

        [Fact]
        public async Task CreateProduct_Throws_WhenParentMissing()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();

            var req = NewProductRequest(categoryId);
            req.ParentId = 999;

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateProductAsync(req));
        }

        [Fact]
        public async Task AdjustStock_IncreasesAndDecreases()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var created = await svc.CreateProductAsync(NewProductRequest(categoryId, stock: 10));

            var increased = await svc.AdjustStockAsync(created.Id, 5);
            Assert.Equal(15, increased!.UnitsInStock);

            var decreased = await svc.AdjustStockAsync(created.Id, -4);
            Assert.Equal(11, decreased!.UnitsInStock);
        }

        [Fact]
        public async Task AdjustStock_Throws_WhenGoingNegative()
        {
            var (svc, categoryId) = await NewServiceWithCategoryAsync();
            var created = await svc.CreateProductAsync(NewProductRequest(categoryId, stock: 2));

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.AdjustStockAsync(created.Id, -5));
        }

        [Fact]
        public async Task AdjustStock_ReturnsNull_WhenMissing()
        {
            IProductService svc = new InMemoryProductService();
            Assert.Null(await svc.AdjustStockAsync(123, 1));
        }
    }
}
