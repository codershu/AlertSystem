using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SystemAlert.Service.Tests
{
    public class HandlerTestBase<T> where T : class
    {
        protected readonly AutoFixture.Fixture fixture;
        protected readonly AutoMocker mocker;
        protected readonly T handler;
        protected readonly Mock<AdminContext> contextMock;

        public HandlerTestBase()
        {
            fixture = new AutoFixture.Fixture();
            fixture.Behaviors
               .OfType<ThrowingRecursionBehavior>()
               .ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(2));

            mocker = new AutoMocker();
            var options = new DbContextOptions<AdminContext>();
            contextMock = new Mock<AdminContext>(options);
            mocker.Use<AdminContext>(contextMock);
            var m = new MemoryCache(Microsoft.Extensions.Options.Options.Create<MemoryCacheOptions>(new MemoryCacheOptions()));
            mocker.Use<IMemoryCache>(m);
            handler = CreateHandler();
        }

        protected virtual T CreateHandler()
        {
            return mocker.CreateInstance<T>();
        }

        protected void SetupUserHasAccessToCustomer(Guid userId, Guid? customerId)
        {
            var mock = mocker.GetMock<ISecurityHelper>();
            mock
                .Setup(s => s.UserCanAccessCustomerAsync(userId, customerId))
                .Returns(Task.FromResult(true));
            mock
                .Setup(s => s.UserCanAccessCustomerAsync(It.Is<ClaimsPrincipal>(p =>
                    SecurityHelper.GetUserClaims(p).ObjectIdentifier == userId
                ), customerId))
                .Returns(Task.FromResult(true));
        }

        protected Mock<DbSet<TItem>> SetupMockData<TContext, TItem>(
            Expression<Func<TContext, DbSet<TItem>>> expression, params TItem[] items)
            where TContext : DbContext
            where TItem : class
        {
            var contextMock = mocker.GetMock<TContext>();
            var dataMock = MockDbSet(items);
            contextMock.Setup<DbSet<TItem>>(expression).Returns(dataMock.Object);
            return dataMock;
        }

        private static Mock<DbSet<TItem>> MockDbSet<TItem>(params TItem[] items) where TItem : class
        {
            return items.AsQueryable().BuildMockDbSet();
        }


        protected static ClaimsPrincipal GetClaimsPrincipal(Guid oid)
        {
            var claim = new Claim(ClaimConstants.ObjectId, oid.ToString());
            var id = new ClaimsIdentity(new[] { claim });
            return new ClaimsPrincipal(id);
        }
    }
}
