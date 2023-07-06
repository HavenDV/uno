#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Services.Store
{
	#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class StorePackageUpdate 
	{
		#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "IS_UNIT_TESTS", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  bool Mandatory
		{
			get
			{
				throw new global::System.NotImplementedException("The member bool StorePackageUpdate.Mandatory is not implemented. For more information, visit https://aka.platform.uno/notimplemented#m=bool%20StorePackageUpdate.Mandatory");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "IS_UNIT_TESTS", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.ApplicationModel.Package Package
		{
			get
			{
				throw new global::System.NotImplementedException("The member Package StorePackageUpdate.Package is not implemented. For more information, visit https://aka.platform.uno/notimplemented#m=Package%20StorePackageUpdate.Package");
			}
		}
		#endif
		// Forced skipping of method Windows.Services.Store.StorePackageUpdate.Package.get
		// Forced skipping of method Windows.Services.Store.StorePackageUpdate.Mandatory.get
	}
}
