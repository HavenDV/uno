---
uid: Uno.Controls.ComboBox
---

# ComboBox

The `ComboBox` is designed to select a value in a set of items. For more information about its usage, see [Combo box and list box Microsoft documentation](https://learn.microsoft.com/windows/apps/design/controls/combo-box).

## Customize the placement of the Drop-Down (UNO-only feature)

By default, when opening a `ComboBox`, WinUI aligns its drop-down to keep the selected item at the same location. This means that, if the currently selected value is the last one in the list, the drop-down will appear above the `ComboBox`.
If there isn't any selected item, the drop-down will appear centered over the `ComboBox`.

On Uno, you can change this behavior using `DefaultDropDownPreferredPlacement` or `DropDownPreferredPlacement`.

### Change the default value for all the `ComboBox` in your application

The default placement for all `ComboBox` instances can be changed by setting the feature flag in the startup of your app (`App.cs` or `App.xaml.cs`):

```csharp
Uno.UI.FeatureConfiguration.ComboBox.DefaultDropDownPreferredPlacement = DropDownPlacement.Below;
```

### Change only for a specific `ComboBox`

```xml
<Page
    [...]
    xmlns:not_win="using:Uno.UI.Xaml.Controls"
    mc:Ignorable="d not_win">

    <ComboBox
        ItemsSource="12345"
        not_win:ComboBox.DropDownPreferredPlacement="Below" />

```
