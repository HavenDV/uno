using NUnit.Framework;
using SamplesApp.UITests.TestFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Uno.UITest.Helpers;
using Uno.UITest.Helpers.Queries;
using Uno.UITests.Helpers;

namespace SamplesApp.UITests.Windows_UI_Xaml_Controls.Line.CommandBarTests
{
	[TestFixture]
	public partial class CommandBarLine : SampleControlUITestBase
	{
		[Test]
		[AutoRetry]
		[ActivePlatforms(Platform.iOS)]
		public void NativeCommandBar_Line()
		{
			Run("UITests.Windows_UI_Xaml_Controls.CommandBar.CommandBarLine");

			using var screenshot = TakeScreenshot("CommandBarLine");
			var rect = _app.Query("MyCommandBar").Single().Rect;

			ImageAssert.HasColorAt(screenshot, 0, rect.Bottom - 1, Color.Purple);

			Assert.Fail("Expected a red pixel.");
		}
	}
}
