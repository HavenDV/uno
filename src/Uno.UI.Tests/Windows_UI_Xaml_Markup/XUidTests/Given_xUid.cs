﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.Tests.App.Xaml;
using Uno.UI.Tests.Helpers;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using _ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader;
using Uno.UI.Helpers;
using Uno.UI.Tests.Windows_UI_Xaml_Markup.XUidTests.Controls;

namespace Uno.UI.Tests.Windows_UI_Xaml_Markup.XUidTests
{
	[TestClass]
	public class Given_xUid
	{
		[TestInitialize]
		public void Init()
		{
			UnitTestsApp.App.EnsureApplication();
		}

		[TestMethod]
		public void When_EmptyXUid()
		{
			var SUT = new When_Empty_XUid();
			var conv = (TestEmptyConverter)SUT.Resources["TestEmptyConverterText"];

			Assert.IsNull(conv.ValueIfNotNull);
			Assert.AreEqual("Test", conv.ValueIfNull);
		}
	}
}
