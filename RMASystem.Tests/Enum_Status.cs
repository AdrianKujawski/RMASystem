using System.Data.Entity;
using System.Linq;
using Xunit;

namespace RMASystem.Tests {
	public class Enum_Status {

		int excecute(IQueryable<Application> applications) {
			var t = applications.Select(a => a).ToList();
			return t.Count(a => a.Statue.EName == EStatue.Sended);
		}

		[Fact]
		public void filtr_app_by_status() {
			var context = new RmaEntities();
			var application =
				context.Application.Include(a => a.AppType).Include(a => a.User).Include(a => a.User1).Include(a => a.Product).Include(a => a.Realization).Include(a => a.Result).Include(a => a.Statue);

			var result = excecute(application);
			Assert.True(result > 0);
		}
	}
}
