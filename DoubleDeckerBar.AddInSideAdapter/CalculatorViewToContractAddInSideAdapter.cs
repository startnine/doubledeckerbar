using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Runtime.Remoting;
using DoubleDeckerBar.AddInView;
using Start9.Api.Contracts;

namespace DoubleDeckerBar.AddInSideAdapter
{
	public class BookInfoContractToViewAddInAdapter : BookInfo
	{
		private readonly IBookInfoContract _contract;
		private ContractHandle _handle;

		public BookInfoContractToViewAddInAdapter(IBookInfoContract contract)
		{
			_contract = contract;
			_handle = new ContractHandle(contract);
		}

		public override string Id => _contract.Id;

		public override string Author => _contract.Author;

		public override string Title => _contract.Title;

		public override string Genre => _contract.Genre;

		public override string Price => _contract.Price;

		public override string PublishDate => _contract.PublishDate;

		public override string Description => _contract.Description;

		internal IBookInfoContract GetSourceContract()
		{
			return _contract;
		}
	}

	public class BookInfoViewToContractAddInAdapter : ContractBase, IBookInfoContract
	{
		private readonly BookInfo _view;

		public BookInfoViewToContractAddInAdapter(BookInfo view)
		{
			_view = view;
		}

		public virtual string Id => _view.Id;

		public virtual string Author => _view.Author;

		public virtual string Title => _view.Title;

		public virtual string Genre => _view.Genre;

		public virtual string Price => _view.Price;

		public virtual string PublishDate => _view.PublishDate;

		public virtual string Description => _view.Description;

		internal BookInfo GetSourceView()
		{
			return _view;
		}
	}

	[AddInAdapter]
	public class LibraryManagerViewToContractAddInAdapter : ContractBase, ILibraryManagerContract
	{
		private readonly LibraryManager _view;

		public LibraryManagerViewToContractAddInAdapter(LibraryManager view)
		{
			_view = view;
		}

		public virtual void ProcessBooks(IListContract<IBookInfoContract> books)
		{
			_view.ProcessBooks(CollectionAdapters.ToIList(books,
				BookInfoAddInAdapter.ContractToViewAdapter,
				BookInfoAddInAdapter.ViewToContractAdapter));
		}

		public virtual IBookInfoContract BestSeller => BookInfoAddInAdapter.ViewToContractAdapter(_view.GetBestSeller());

		public virtual string Data(string txt)
		{
			string rtxt = _view.Data(txt);
			return rtxt;
		}

		internal LibraryManager GetSourceView()
		{
			return _view;
		}
	}

	public class BookInfoAddInAdapter
	{
		internal static BookInfo ContractToViewAdapter(IBookInfoContract contract)
		{
			if (!RemotingServices.IsObjectOutOfAppDomain(contract) &&
			    contract.GetType().Equals(typeof(BookInfoViewToContractAddInAdapter)))
				return ((BookInfoViewToContractAddInAdapter) contract).GetSourceView();
			return new BookInfoContractToViewAddInAdapter(contract);
		}

		internal static IBookInfoContract ViewToContractAdapter(BookInfo view)
		{
			if (!RemotingServices.IsObjectOutOfAppDomain(view) &&
			    view.GetType().Equals(typeof(BookInfoContractToViewAddInAdapter)))
				return ((BookInfoContractToViewAddInAdapter) view).GetSourceContract();
			return new BookInfoViewToContractAddInAdapter(view);
		}
	}
}