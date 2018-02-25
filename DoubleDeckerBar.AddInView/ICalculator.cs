using System.AddIn.Pipeline;
using System.Collections.Generic;

namespace DoubleDeckerBar.AddInView
{
	[AddInBase]
	public abstract class LibraryManager
	{
		public abstract void ProcessBooks(IList<BookInfo> books);
		public abstract BookInfo GetBestSeller();

		public abstract string Data(string txt);
	}

	public abstract class BookInfo
	{
		public abstract string Id { get; }
		public abstract string Author { get; }
		public abstract string Title { get; }
		public abstract string Genre { get; }
		public abstract string Price { get; }
		public abstract string PublishDate { get; }
		public abstract string Description { get; }
	}
}