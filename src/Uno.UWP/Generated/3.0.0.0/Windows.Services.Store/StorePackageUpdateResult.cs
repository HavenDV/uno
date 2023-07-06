#pragma warning disable 108 // new keyword hiding
#pragma warning disable 114 // new keyword hiding
namespace Windows.Services.Store
{
	#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
	[global::Uno.NotImplemented]
	#endif
	public  partial class StorePackageUpdateResult 
	{
		#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "IS_UNIT_TESTS", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::Windows.Services.Store.StorePackageUpdateState OverallState
		{
			get
			{
				throw new global::System.NotImplementedException("The member StorePackageUpdateState StorePackageUpdateResult.OverallState is not implemented. For more information, visit https://aka.platform.uno/notimplemented#m=StorePackageUpdateState%20StorePackageUpdateResult.OverallState");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "IS_UNIT_TESTS", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::System.Collections.Generic.IReadOnlyList<global::Windows.Services.Store.StorePackageUpdateStatus> StorePackageUpdateStatuses
		{
			get
			{
				throw new global::System.NotImplementedException("The member IReadOnlyList<StorePackageUpdateStatus> StorePackageUpdateResult.StorePackageUpdateStatuses is not implemented. For more information, visit https://aka.platform.uno/notimplemented#m=IReadOnlyList%3CStorePackageUpdateStatus%3E%20StorePackageUpdateResult.StorePackageUpdateStatuses");
			}
		}
		#endif
		#if __ANDROID__ || __IOS__ || IS_UNIT_TESTS || __WASM__ || __SKIA__ || __NETSTD_REFERENCE__ || __MACOS__
		[global::Uno.NotImplemented("__ANDROID__", "__IOS__", "IS_UNIT_TESTS", "__WASM__", "__SKIA__", "__NETSTD_REFERENCE__", "__MACOS__")]
		public  global::System.Collections.Generic.IReadOnlyList<global::Windows.Services.Store.StoreQueueItem> StoreQueueItems
		{
			get
			{
				throw new global::System.NotImplementedException("The member IReadOnlyList<StoreQueueItem> StorePackageUpdateResult.StoreQueueItems is not implemented. For more information, visit https://aka.platform.uno/notimplemented#m=IReadOnlyList%3CStoreQueueItem%3E%20StorePackageUpdateResult.StoreQueueItems");
			}
		}
		#endif
		// Forced skipping of method Windows.Services.Store.StorePackageUpdateResult.OverallState.get
		// Forced skipping of method Windows.Services.Store.StorePackageUpdateResult.StorePackageUpdateStatuses.get
		// Forced skipping of method Windows.Services.Store.StorePackageUpdateResult.StoreQueueItems.get
	}
}
