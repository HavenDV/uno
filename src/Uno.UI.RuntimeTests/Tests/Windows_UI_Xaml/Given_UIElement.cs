﻿#if __NETSTD__
#define MEASURE_DIRTY_PATH_AVAILABLE
#define ARRANGE_DIRTY_PATH_AVAILABLE
#elif __ANDROID__
#define MEASURE_DIRTY_PATH_AVAILABLE
#endif

using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;
using Private.Infrastructure;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Shapes;
using System;
using Windows.UI.Xaml.Media;
using FluentAssertions;
using FluentAssertions.Execution;
#if NETFX_CORE
using Uno.UI.Extensions;
#elif __IOS__
using UIKit;
#elif __MACOS__
using AppKit;
#else
using Uno.UI;
#endif

namespace Uno.UI.RuntimeTests.Tests.Windows_UI_Xaml
{
	[TestClass]
	public partial class Given_UIElement
	{
#if HAS_UNO // Tests use IsArrangeDirty, which is an internal property
		[TestMethod]
		[RunsOnUIThread]
		public async Task When_Visible_InvalidateArrange()
		{
			var sut = new Border() { Width = 100, Height = 10 };

			TestServices.WindowHelper.WindowContent = sut;
			await TestServices.WindowHelper.WaitForIdle();
			sut.InvalidateArrange();
			await TestServices.WindowHelper.WaitForIdle();

			Assert.IsFalse(sut.IsArrangeDirty);
		}

#if !__ANDROID__ && !__IOS__ // Fails on Android & iOS (issue #5002)
		[TestMethod]
		[RunsOnUIThread]
		public async Task When_Collapsed_InvalidateArrange()
		{
			var sut = new Border()
			{
				Width = 100,
				Height = 10,
				Visibility = Visibility.Collapsed
			};

			TestServices.WindowHelper.WindowContent = sut;
			await TestServices.WindowHelper.WaitForIdle();
			sut.InvalidateArrange();
			await TestServices.WindowHelper.WaitForIdle();

			await TestServices.WindowHelper.WaitFor(() => !sut.IsArrangeDirty);
		}
#endif
#endif

#if HAS_UNO // Issue #2840 - ActualSize is available since 18362
		[TestMethod]
		[RunsOnUIThread]
		public async Task When_TextBlock_ActualSize()
		{
			Border border = new Border();
			TextBlock text = new TextBlock()
			{
				HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
				VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
				Text = "Short text"
			};
			border.Child = text;

			TestServices.WindowHelper.WindowContent = border;
			await TestServices.WindowHelper.WaitForIdle();

			border.UpdateLayout();

			await TestServices.WindowHelper.WaitFor(() => Math.Abs(text.ActualWidth - text.ActualSize.X) < 0.01);
			await TestServices.WindowHelper.WaitFor(() => Math.Abs(text.ActualHeight - text.ActualSize.Y) < 0.01);

			text.Text = "This is a longer text";
			border.UpdateLayout();

			await TestServices.WindowHelper.WaitFor(() => Math.Abs(text.ActualWidth - text.ActualSize.X) < 0.01);
			await TestServices.WindowHelper.WaitFor(() => Math.Abs(text.ActualHeight - text.ActualSize.Y) < 0.01);
		}

		public async Task When_Rectangle_Set_ActualSize()
		{
			Border border = new Border();

			Rectangle rectangle = new Rectangle()
			{
				HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
				VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
				Width = 42,
				Height = 24,
			};
			border.Child = rectangle;

			TestServices.WindowHelper.WindowContent = border;
			await TestServices.WindowHelper.WaitForIdle();

			border.UpdateLayout();

			await TestServices.WindowHelper.WaitFor(() =>
				Math.Abs(rectangle.ActualWidth - rectangle.ActualSize.X) < 0.01);
			await TestServices.WindowHelper.WaitFor(() =>
				Math.Abs(rectangle.ActualHeight - rectangle.ActualSize.Y) < 0.01);

			rectangle.Width = 16;
			rectangle.Height = 32;
			border.UpdateLayout();

			await TestServices.WindowHelper.WaitFor(() =>
				Math.Abs(rectangle.ActualWidth - rectangle.ActualSize.X) < 0.01);
			await TestServices.WindowHelper.WaitFor(() =>
				Math.Abs(rectangle.ActualHeight - rectangle.ActualSize.Y) < 0.01);
		}
#endif

#if HAS_UNO // Cannot Set the LayoutInformation on UWP
		[TestMethod]
		[RunsOnUIThread]
		public void When_UpdateLayout_Then_TreeNotMeasuredUsingCachedValue()
		{
			if (Window.Current.RootElement is Panel root)
			{
				var sut = new Grid
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch
				};

				var originalRootAvailableSize = LayoutInformation.GetAvailableSize(root);
				var originalRootDesiredSize = LayoutInformation.GetDesiredSize(root);
				var originalRootLayoutSlot = LayoutInformation.GetLayoutSlot(root);

				Size availableSize;
				Rect layoutSlot;
				try
				{
					LayoutInformation.SetAvailableSize(root, default);
					LayoutInformation.SetDesiredSize(root, default);
					LayoutInformation.SetLayoutSlot(root, default);

					root.Children.Add(sut);
					sut.UpdateLayout();

					availableSize = LayoutInformation.GetAvailableSize(sut);
					layoutSlot = LayoutInformation.GetLayoutSlot(sut);
				}
				finally
				{
					LayoutInformation.SetAvailableSize(root, originalRootAvailableSize);
					LayoutInformation.SetDesiredSize(root, originalRootDesiredSize);
					LayoutInformation.SetLayoutSlot(root, originalRootLayoutSlot);

					root.Children.Remove(sut);
					try { root.UpdateLayout(); }
					catch { } // Make sure to restore visual tree if test has failed!
				}

				Assert.AreNotEqual(default, availableSize);
#if !__IOS__ // Arrange is async on iOS!
				Assert.AreNotEqual(default, layoutSlot);
#endif
			}
			else
			{
				Assert.Inconclusive("The RootElement is not a Panel");
			}
		}
#endif

		[TestMethod]
		[RunsOnUIThread]
		public async Task When_UpdateLayout_Then_ReentrancyNotAllowed()
		{
			var sut = new When_UpdateLayout_Then_ReentrancyNotAllowed_Element();

			TestServices.WindowHelper.WindowContent = sut;
			await TestServices.WindowHelper.WaitForIdle();

			Assert.IsFalse(sut.Failed);
		}

#if HAS_UNO
		[TestMethod]
		[RunsOnUIThread]
		public void When_GetVisualTreeParent()
		{
			var treeRoot = GetTreeRoot();
			Assert.IsNotNull(treeRoot);
#if __ANDROID__ || __IOS__ || __MACOS__
			// On Xamarin platforms, we don't expect the real root of the tree to be a XAML element
			Assert.IsNotInstanceOfType(treeRoot, typeof(UIElement));
#else
			//...and everywhere else, we do
			Assert.IsInstanceOfType(treeRoot, typeof(UIElement));
#endif
			object GetTreeRoot()
			{
				var current = Windows.UI.Xaml.Window.Current.Content?.GetVisualTreeParent();
				current = Windows.UI.Xaml.Window.Current.Content;
				var parent = current?.GetVisualTreeParent();
				while (parent != null)
				{
					current = parent;
					parent = current?.GetVisualTreeParent();
				}
				return current;
			}
		} 
#endif

		[TestMethod]
		[RunsOnUIThread]
		public async Task When_LayoutInformation_GetAvailableSize_Constraints()
		{
			var noConstraintsBorder = new Border();
			var maxHeightBorder = new Border() { MaxHeight = 122 };
			var hostGrid = new Grid
			{
				Width = 182,
				Height = 313,
				Children =
				{
					noConstraintsBorder,
					maxHeightBorder
				}
			};

			TestServices.WindowHelper.WindowContent = hostGrid;
			await TestServices.WindowHelper.WaitForLoaded(hostGrid);

			await TestServices.WindowHelper.WaitForEqual(313, () => LayoutInformation.GetAvailableSize(noConstraintsBorder).Height);
			var maxHeightAvailableSize = LayoutInformation.GetAvailableSize(maxHeightBorder);
			Assert.AreEqual(313, maxHeightAvailableSize.Height, delta: 1); // Should return unmodified measure size, ignoring constraints like MaxHeight
		}

		[TestMethod]
		[RunsOnUIThread]
#if !MEASURE_DIRTY_PATH_AVAILABLE
		[Ignore("Not supported on this platform")]
#endif
		public async Task When_InvalidatingMeasureExplicitly()
		{
			var (ctl1, ctl2, ctl3) = await SetupMeasureArrangeTest();

			ctl2.InvalidateMeasure();

			await TestServices.WindowHelper.WaitFor(() => ctl2.MeasureCount > 1);

			await TestServices.WindowHelper.WaitForIdle();

			using var _ = new AssertionScope();

			ctl1.MeasureCount.Should().Be(1);
			ctl2.MeasureCount.Should().Be(2);
			ctl3.MeasureCount.Should().Be(1);

#if ARRANGE_DIRTY_PATH_AVAILABLE
			ctl1.ArrangeCount.Should().Be(1);
			ctl2.ArrangeCount.Should().BeInRange(1, 2); // both are acceptable, depends on the capabilities of the platform
			ctl3.ArrangeCount.Should().Be(1);
#endif
		}

#if __WASM__ || __SKIA__
		[TestMethod]
		[RunsOnUIThread]
		[DataRow(0d)]
		[DataRow(-1d)]
		[DataRow(0.001d)]
		[DataRow(0.1d)]
		[DataRow(100d)]
		public async Task When_InvalidatingMeasureThenMeasure(double size)
		{
			var sut = new MeasureAndArrangeCounter();

			sut.IsMeasureDirty.Should().BeFalse();

			sut.InvalidateMeasure();

			sut.IsMeasureDirty.Should().BeTrue();
			sut.IsMeasureDirtyPath.Should().BeFalse();
			sut.IsMeasureDirtyOrMeasureDirtyPath.Should().BeTrue();

			sut.Measure(new Size(size, size));

			sut.IsMeasureDirtyOrMeasureDirtyPath.Should().BeFalse();
			sut.MeasureCount.Should().Be(1);
			sut.ArrangeCount.Should().Be(0);
		}

		[TestMethod]
		[RunsOnUIThread]
		[DataRow(0d)]
		[DataRow(-1d)]
		[DataRow(0.001d)]
		[DataRow(0.1d)]
		[DataRow(100d)]
		public async Task When_InvalidatingArrangeThenMeasureAndArrange(double size)
		{
			var sut = new MeasureAndArrangeCounter();

			sut.IsMeasureDirtyOrMeasureDirtyPath.Should().BeFalse();
			sut.IsArrangeDirtyOrArrangeDirtyPath.Should().BeFalse();

			sut.InvalidateMeasure();
			sut.InvalidateArrange();

			sut.IsMeasureDirty.Should().BeTrue();
			sut.IsMeasureDirtyPath.Should().BeFalse();
			sut.IsMeasureDirtyOrMeasureDirtyPath.Should().BeTrue();
			sut.IsArrangeDirtyOrArrangeDirtyPath.Should().BeTrue();

			sut.MeasureCount.Should().Be(0);
			sut.ArrangeCount.Should().Be(0);

			sut.Measure(new Size(size, size));
			sut.Arrange(new Rect(0, 0, size, size));

			sut.IsMeasureDirtyOrMeasureDirtyPath.Should().BeFalse();
			sut.IsArrangeDirtyOrArrangeDirtyPath.Should().BeFalse();
			sut.MeasureCount.Should().Be(1);
			sut.ArrangeCount.Should().Be(1);
		}
#endif

		[TestMethod]
		[RunsOnUIThread]
#if !ARRANGE_DIRTY_PATH_AVAILABLE
		[Ignore("Not supported on this platform")]
#endif
		public async Task When_InvalidatingArrangeExplicitly()
		{
			var (ctl1, ctl2, ctl3) = await SetupMeasureArrangeTest();

			ctl2.InvalidateArrange();

			await TestServices.WindowHelper.WaitFor(() => ctl2.ArrangeCount > 1);

			await TestServices.WindowHelper.WaitForIdle();

			using var _ = new AssertionScope();

			ctl1.MeasureCount.Should().Be(1);
			ctl2.MeasureCount.Should().Be(1);
			ctl3.MeasureCount.Should().Be(1);

			ctl1.ArrangeCount.Should().Be(1);
			ctl2.ArrangeCount.Should().Be(2);
			ctl3.ArrangeCount.Should().Be(1);
		}

		[TestMethod]
		[RunsOnUIThread]
#if !(MEASURE_DIRTY_PATH_AVAILABLE && ARRANGE_DIRTY_PATH_AVAILABLE)
		[Ignore("Not supported on this platform")]
#endif
		public async Task When_InvalidatingMeasureAndArrangeByChangingSize()
		{
			var (ctl1, ctl2, ctl3) = await SetupMeasureArrangeTest();

			ctl2.Width = 200;

			await TestServices.WindowHelper.WaitFor(() => ctl2.ArrangeCount > 1);

			await TestServices.WindowHelper.WaitForIdle();

			using var _ = new AssertionScope();

			// Everything should be remeasured & rearranged
			ctl1.MeasureCount.Should().Be(2);
			ctl2.MeasureCount.Should().Be(2);
			ctl3.MeasureCount.Should().Be(2);

			ctl1.ArrangeCount.Should().Be(2);
			ctl2.ArrangeCount.Should().Be(2);
			ctl3.ArrangeCount.Should().BeInRange(1, 2); // both are acceptable, depends on the capabilities of the platform
		}

		[TestMethod]
		[RunsOnUIThread]
#if !(MEASURE_DIRTY_PATH_AVAILABLE && ARRANGE_DIRTY_PATH_AVAILABLE)
		[Ignore("Not supported on this platform")]
#endif
		public async Task When_InvalidatingMeasureAndArrangeByChangingSizeTwice()
		{
			var (ctl1, ctl2, ctl3) = await SetupMeasureArrangeTest();

			ctl2.Width = 200;
			ctl3.Width = 200;

			await TestServices.WindowHelper.WaitFor(() => ctl2.ArrangeCount > 1);

			await TestServices.WindowHelper.WaitForIdle();

			using (var _ = new AssertionScope("First pass"))
			{
				// Everything should be remeasured & rearranged

				ctl1.MeasureCount.Should().Be(2);
				ctl2.MeasureCount.Should().Be(2);
				ctl3.MeasureCount.Should().Be(2);

				ctl1.ArrangeCount.Should().Be(2);
				ctl2.ArrangeCount.Should().Be(2);
				ctl3.ArrangeCount.Should().Be(2);
			}

			ctl3.Width = 50;

			await TestServices.WindowHelper.WaitFor(() => ctl2.ArrangeCount > 2);

			await TestServices.WindowHelper.WaitForIdle();

			using (var _ = new AssertionScope("Second pass"))
			{
				// "ctl1" should be untouched

				ctl1.MeasureCount.Should().Be(2);
				ctl2.MeasureCount.Should().Be(3);
				ctl3.MeasureCount.Should().Be(3);

				ctl1.ArrangeCount.Should().Be(2);
				ctl2.ArrangeCount.Should().Be(3);
				ctl3.ArrangeCount.Should().Be(3);
			}
		}

		private static async Task<(MeasureAndArrangeCounter, MeasureAndArrangeCounter, MeasureAndArrangeCounter)> SetupMeasureArrangeTest()
		{
			var ctl1 = new MeasureAndArrangeCounter
			{
				Background = new SolidColorBrush(Windows.UI.Colors.Yellow),
				Margin = new Thickness(20)
			};
			var ctl2 = new MeasureAndArrangeCounter
			{
				Background = new SolidColorBrush(Windows.UI.Colors.DarkRed),
				Margin = new Thickness(20)
			};
			var ctl3 = new MeasureAndArrangeCounter
			{
				Background = new SolidColorBrush(Windows.UI.Colors.Cornsilk),
				Margin = new Thickness(20),
				Width = 100,
				Height = 100
			};

			ctl1.Children.Add(ctl2);
			ctl2.Children.Add(ctl3);

			TestServices.WindowHelper.WindowContent = ctl1;

			await TestServices.WindowHelper.WaitForLoaded(ctl3);

			using var _ = new AssertionScope("Setup");

			ctl1.MeasureCount.Should().Be(1);
			ctl2.MeasureCount.Should().Be(1);
			ctl3.MeasureCount.Should().Be(1);

			ctl1.ArrangeCount.Should().Be(1);
			ctl2.ArrangeCount.Should().Be(1);
			ctl3.ArrangeCount.Should().Be(1);

			return (ctl1, ctl2, ctl3);
		}

		private partial class MeasureAndArrangeCounter : Panel
		{
			internal int MeasureCount;
			internal int ArrangeCount;
			protected override Size MeasureOverride(Size availableSize)
			{
				MeasureCount++;
				return base.MeasureOverride(availableSize);
			}

			protected override Size ArrangeOverride(Size finalSize)
			{
				ArrangeCount++;
				return base.ArrangeOverride(finalSize);
			}
		}

#if __NETSTD__
		[TestMethod]
		[RunsOnUIThread]
		public void MeasureDirtyTest()
		{
			var sut = new Grid();
			sut.Children.Add(new MeasureAndArrangeCounter());

			using var x = new AssertionScope();

			using (_ = new AssertionScope("Before Measure"))
			{
				sut.IsFirstMeasureDone.Should().BeFalse("IsFirstMeasureDone");
				sut.IsMeasureDirty.Should().BeTrue("IsMeasureDirty");
				sut.IsMeasureDirtyPath.Should().BeTrue("IsMeasureDirtyPath");
			}

			sut.Measure(new Size(100, 100));

			using (_ = new AssertionScope("After Measure"))
			{
				sut.IsFirstMeasureDone.Should().BeTrue("IsFirstMeasureDone");
				sut.IsMeasureDirty.Should().BeFalse("IsMeasureDirty");
				sut.IsMeasureDirtyPath.Should().BeFalse("IsMeasureDirtyPath");
			}
		}


		[TestMethod]
		[RunsOnUIThread]
		public void ArrangeDirtyTest()
		{
			var sut = new Grid();
			sut.Children.Add(new MeasureAndArrangeCounter());

			sut.Measure(new Size(100, 100));

			using var x = new AssertionScope();

			using (_ = new AssertionScope("Before Arrange"))
			{
				sut.IsArrangeDirty.Should().BeTrue("IsArrangeDirty");
			}

			sut.Arrange(new Rect(0, 0, 100, 100));
			using (_ = new AssertionScope("After Arrange"))
			{
				sut.IsArrangeDirty.Should().BeFalse("IsArrangeDirty");
				sut.IsArrangeDirtyPath.Should().BeFalse("IsArrangeDirtyPath");
			}
		}
#endif
	}

	internal partial class When_UpdateLayout_Then_ReentrancyNotAllowed_Element : FrameworkElement
	{
		private bool _isMeasuring, _isArranging;

		public bool Failed { get; private set; }

		protected override Size MeasureOverride(Size availableSize)
		{
			Failed |= _isMeasuring;

			if (!Failed)
			{
				_isMeasuring = true;
				UpdateLayout();
				_isMeasuring = false;
			}

			return base.MeasureOverride(availableSize);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Failed |= _isArranging;

			if (!Failed)
			{
				_isArranging = true;
				UpdateLayout();
				_isArranging = false;
			}

			return base.ArrangeOverride(finalSize);
		}
	}
}
