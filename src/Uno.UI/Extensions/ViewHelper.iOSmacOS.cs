﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Uno.UI.Extensions;
using Uno.Foundation.Logging;
using Uno.Extensions;
using Windows.Graphics.Display;

using Foundation;
using CoreGraphics;

#if NET6_0_OR_GREATER
using ObjCRuntime;
#endif

#if __IOS__
using UIKit;
using _View = UIKit.UIView;
using Windows.UI.Xaml;
#elif __MACOS__
using AppKit;
using _View = AppKit.NSView;
#endif

namespace Uno.UI
{
	public static class ViewHelper
	{
#if __IOS__
		// This return the value from the original screen. Use 'DisplayInformation.RawPixelsPerViewPixel' to get the value for the current screen.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly nfloat MainScreenScale = UIScreen.MainScreen.Scale;
		// This return the value from the original screen. Use 'DisplayInformation.RawPixelsPerViewPixel > 1.0f' for the current screen.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly bool IsRetinaDisplay = MainScreenScale > 1.0f;
#elif __MACOS__
		// This return the value from the original screen. Use 'DisplayInformation.RawPixelsPerViewPixel' to get the value for the current screen.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly nfloat MainScreenScale = NSScreen.MainScreen.BackingScaleFactor;
		// This return the value from the original screen. Use 'DisplayInformation.RawPixelsPerViewPixel > 1.0f' for the current screen.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly bool IsRetinaDisplay = MainScreenScale > 1.0f;
#endif

		private static double _rectangleRoundingEpsilon = 0.05;
		private static double _scaledRectangleRoundingEpsilon = _rectangleRoundingEpsilon * DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

		/// <summary>
		/// This is used to correct some errors when using Floor and Ceiling in LogicalToPhysicalPixels for CGRect.
		/// </summary>
		public static double RectangleRoundingEpsilon
		{
			get { return _rectangleRoundingEpsilon; }
			set
			{
				_rectangleRoundingEpsilon = value;
				_scaledRectangleRoundingEpsilon = value * DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
			}
		}

		[Uno.NotImplemented]
		public static string Architecture => null;

		static ViewHelper()
		{
			if (typeof(ViewHelper).Log().IsEnabled(Uno.Foundation.Logging.LogLevel.Debug))
			{
				typeof(ViewHelper).Log().DebugFormat("Display scale is {0}", DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel);
			}
		}

		public static nfloat OnePixel
		{
			get
			{
				return (nfloat)(1.0d / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CGSize PhysicalToLogicalPixels(this CGSize size)
		{
			return size;
			// UISize are automatically scaled to the device's DPI, we don't need to adjust.
			// return new SizeF(size.Width / MainScreenScale, size.Height / MainScreenScale);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Windows.Foundation.Size PhysicalToLogicalPixels(this Windows.Foundation.Size size)
		{
			return size;
			// UISize are automatically scaled to the device's DPI, we don't need to adjust.
			// return new SizeF(size.Width / MainScreenScale, size.Height / MainScreenScale);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CGSize LogicalToPhysicalPixels(this Windows.Foundation.Size size)
		{
			var ret = new CGSize((nfloat)size.Width, (nfloat)size.Height);
			return ret;
			// UISize are automatically scaled to the device's DPI, we don't need to adjust.
			// return new SizeF(size.Width * MainScreenScale, size.Height * MainScreenScale);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CGSize LogicalToPhysicalPixels(this CGSize size)
		{
			return size;
			// UISize are automatically scaled to the device's DPI, we don't need to adjust.
			// return new SizeF(size.Width * MainScreenScale, size.Height * MainScreenScale);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CGRect PhysicalToLogicalPixels(this CGRect size)
		{
			return size;
			// UISize are automatically scaled to the device's DPI, we don't need to adjust.
			//return new RectangleF(
			//	size.X / MainScreenScale,
			//	size.Y / MainScreenScale,
			//	size.Width / MainScreenScale,
			//	size.Height / MainScreenScale
			//);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Windows.Foundation.Point PhysicalToLogicalPixels(this Windows.Foundation.Point point)
		{
			return point;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Windows.Foundation.Point LogicalToPhysicalPixels(this Windows.Foundation.Point point)
		{
			return point;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static CGRect LogicalToPhysicalPixels(this CGRect size)
		{
			// https://markpospesel.wordpress.com/2013/02/27/cgrectintegral/
			// According to the Apple Documentation for CGRectIntegral:
			// A rectangle with the smallest integer values for its origin and size 
			// that contains the source rectangle.
			// That is, given a rectangle with fractional origin or size values, 
			// CGRectIntegral rounds the rectangle’s origin downward 
			// and its size upward to the nearest whole integers, 
			// such that the result contains the original rectangle.

			var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
			return new CGRect
			(
				(nfloat)FloorWithEpsilon(size.X * scale) / scale,
				(nfloat)FloorWithEpsilon(size.Y * scale) / scale,
				(nfloat)CeilingWithEpsilon(size.Width * scale) / scale,
				(nfloat)CeilingWithEpsilon(size.Height * scale) / scale
			);
		}

		/// <summary>
		/// if the value would be 0.01, result would be 0 instead of 1 
		/// </summary>
		private static double CeilingWithEpsilon(double value)
		{
			var decimals = value - Math.Truncate(value);
			if (decimals < _scaledRectangleRoundingEpsilon)
			{
				return Math.Floor(value);
			}
			else
			{
				return Math.Ceiling(value);
			}
		}

		/// <summary>
		/// if the value would be 0.99, result would be 1 instead of 0
		/// </summary>
		private static double FloorWithEpsilon(double value)
		{
			var decimals = value - Math.Truncate(value);
			if (1 - decimals < _scaledRectangleRoundingEpsilon)
			{
				return Math.Ceiling(value);
			}
			else
			{
				return Math.Floor(value);
			}
		}

		public static nfloat GetConvertedPixel(float thickness)
		{
			if (thickness > 0 && thickness <= 1 && (DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel > 1.0f))
			{
				return OnePixel;
			}
			return thickness;
		}

		public static nfloat StackSubViews(IEnumerable<_View> views)
		{
			nfloat lastBottom = 0f;
			foreach (var view in views)
			{

				if (view.Hidden)
				{
					continue;
				}

				view.Frame = view.Frame.SetY(lastBottom);
				lastBottom = view.Frame.Bottom;
			}

			return lastBottom;
		}

		public static nfloat StackSubViews(_View thisView, float topPadding, float spaceBetweenElements)
		{
			nfloat lastBottom = topPadding;

			foreach (var view in thisView.Subviews)
			{

				if (view.Hidden)
				{
					continue;
				}
				view.Frame = view.Frame.SetY(lastBottom);

				lastBottom = view.Frame.Bottom + spaceBetweenElements;
			}

			return lastBottom;
		}

		/// <summary>
		/// Gets the orientation-dependent screen size
		/// </summary>
		/// <returns></returns>
		public static CGSize GetScreenSize()
		{
			return GetScreenSizeInternal(window: Windows.UI.Xaml.Window.Current);
		}
		
		internal static CGSize GetScreenSizeInternal(Windows.UI.Xaml.Window window)
		{
#if __IOS__
			var nativeFrame = window?.NativeWindow?.Frame ?? CGRect.Empty;

			return new CGSize(nativeFrame.Width, nativeFrame.Height);
#else
			var applicationFrameSize = NSScreen.MainScreen.VisibleFrame;
			return new CGSize(applicationFrameSize.Width, applicationFrameSize.Height);
#endif
		}
	}
}
