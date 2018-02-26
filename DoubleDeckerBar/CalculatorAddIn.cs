using System;
using System.AddIn;
using System.Collections.Generic;
using DoubleDeckerBar.AddInView;

namespace DoubleDeckerBar
{
	[AddIn("Books AddIn", Description = "Book Store Data",
		Publisher = "Microsoft", Version = "1.0.0.0")]
	public class BooksAddIn : LibraryManager
	{
		// Calls methods that updates book data
		// and removes books by their genre.
		public override void ProcessBooks(IList<BookInfo> books)
		{
			for (var i = 0; i < books.Count; i++) books[i] = UpdateBook(books[i]);

			RemoveGenre("horror", books);
		}

		public override string Data(string txt)
		{
			// assumes txt = "sales tax"
			string rtxt = txt + "= 8.5%";
			return rtxt;
		}

		internal static IList<BookInfo> RemoveGenre(string genre, IList<BookInfo> books)
		{
			// Remove all horror books from the collection.
			for (var i = 0; i < books.Count; i++)
				if (books[i].Genre.ToLower() == "horror")
					books.RemoveAt(i);

			return books;
		}

		// Populate a BookInfo object with data
		// about the best selling book.
		public override BookInfo GetBestSeller()
		{
			var paramId = "bk999";
			var paramAuthor = "Corets, Eva";
			var paramTitle = "Cooking with Oberon";
			var paramGenre = "Cooking";
			var paramPrice = "7.95";
			var paramPublishDate = "2006-12-01";
			var paramDescription = "Recipes for a post-apocalyptic society.";

			var bestBook = new MyBookInfo(paramId, paramAuthor, paramTitle, paramGenre,
				paramPrice, paramPublishDate, paramDescription);
			return bestBook;
		}

		internal static BookInfo UpdateBook(BookInfo bk)
		{
			// Discounts the price of all
			// computer books by 20 percent.
			string paramId = bk.Id;
			string paramAuthor = bk.Author;
			string paramTitle = bk.Title;
			string paramGenre = bk.Genre;
			string paramPrice = bk.Price;
			if (paramGenre.ToLower() == "computer")
			{
				double oldprice = Convert.ToDouble(paramPrice);
				double newprice = oldprice - oldprice * .20;
				paramPrice = newprice.ToString();
				if (paramPrice.IndexOf(".") == paramPrice.Length - 4)
					paramPrice = paramPrice.Substring(1, paramPrice.Length - 1);
				Console.WriteLine("{0} - Old Price: {1}, New Price: {2}", paramTitle, oldprice, paramPrice);
			}

			string paramPublishDate = bk.PublishDate;
			string paramDescription = bk.Description;

			BookInfo bookUpdated = new MyBookInfo(paramId, paramAuthor, paramTitle, paramGenre,
				paramPrice, paramPublishDate, paramDescription);

			return bookUpdated;
		}
	}

	internal class MyBookInfo : BookInfo
	{
		public MyBookInfo(string id, string title, string author, string genre, string price, string publishDate,
			string description)
		{
			Id = id;
			Title = title;
			Author = author;
			Genre = genre;
			Price = price;
			PublishDate = publishDate;
			Description = description;
		}

		public override string Id { get; }
		public override string Title { get; }
		public override string Author { get; }
		public override string Genre { get; }
		public override string Price { get; }
		public override string PublishDate { get; }
		public override string Description { get; }
	}
}