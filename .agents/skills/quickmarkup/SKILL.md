---
name: quickmarkup
description: Write and edit QuickMarkup declarative UI markup in C# projects. Use when the project uses QuickMarkup, identifiable by `using QuickMarkup.Infra`, `using QuickMarkup.SourceGen`, or `[QuickMarkup(...)]` attribute in .cs files.
---

# QuickMarkup

QuickMarkup is a Vue-inspired declarative markup DSL embedded in C# that replaces XAML for UI declaration. It uses a **reactivity system** (not MVVM).

## How It Works

QuickMarkup code is placed inside a `[QuickMarkup("""...""")]` attribute on a `partial class`.

```csharp
[QuickMarkup("""
    int Counter = 0;
    <root>
        <StackPanel>
            <Button Text="Click Me" @Click+=`Counter++` />
            <TextBlock Text=`$"You clicked {Counter} time(s)"` />
        </StackPanel>
    </root>
    """)]
partial class CounterPage : Page
{
    public CounterPage() { Init(); }
}
```

A source generator processes the attribute. If the class has at least one user-defined constructor (including primary constructors and record syntax), it generates an `Init()` method the class must call (typically at the end of the constructor, after all other setup). If there are no constructors, it generates a public constructor automatically.

## Sections (in order)

1. **Usings** (optional) — namespace imports for the markup scope. Global usings from C# files also apply.
2. **Reference/Computed declarations** (optional) — reactive variable declarations.
3. **`<setup>`** (optional) — C# code that runs before UI creation. Variables declared here are accessible in `<root>` but not exported outside.
4. **`<root>`** — the UI tree. This is where the markup goes.

## Reference Declarations

Declaring variables outside `<setup>` creates reactive references. The generated code wraps them in `Reference<T>` with a property getter/setter.

```
double Value = 0;          // creates Reference<double>, property Value, backing field ValueProp
double Output => `A + B`;  // creates Computed<double>, property Output, backing field OutputComp
```

References auto-notify the UI on change. Computed variables cache and re-evaluate when dependencies change.

## Markup Syntax

### Tags

```
<TypeName Property=Value>
    <Child />
</TypeName>
<SelfClosing Property=Value />
```

Comments use `//` or `/* */`. **Not** `<!-- -->`.

### Property Values

Values are **not** quoted (unlike XML/XAML). Use raw values directly.

| Kind | Syntax | Example |
|------|--------|---------|
| Integer | literal | `Width=100` |
| Double | literal | `FontSize=14.5` |
| Boolean | literal | `IsChecked=true` |
| Boolean true shorthand | property name alone | `IsEnabled` |
| Boolean false shorthand | `!` prefix | `!IsHitTestVisible` |
| String | double quotes | `Text="Hello"` |
| Enum member | name alone | `HorizontalAlignment=Center` |
| null/default | keyword | `Tag=null` |
| C# expression | backticks | `` Text=`$"Count: {Counter}"` `` |
| Alternate C# literal (backward compatability legacy syntax of above) | `/-...-/` | `Source=/-new Uri("ms-appx:///icon.png")-/` |

### Automatic `new` (single-argument constructors)

When you assign a **numeric or bool** literal to a property, the source generator may emit **`new PropertyType(literal)`** instead of the raw literal. That happens when the property type does not take the literal directly but exposes a **constructor with exactly one parameter** that does (`CodeTypeResolver.ShouldAutoNew`, `Binder.ValueOrAutoNew`).

**Examples:** `CornerRadius=16` → `new CornerRadius(16)`; `BorderThickness=1` → `new Thickness(1)` when the uniform constructor applies.

**Not covered:** multi-value `Thickness` / `CornerRadius` corners — use a backtick C# expression, e.g. `` Margin=`new(0,12,0,0)` ``. If assignment fails, use an explicit `` `new Thickness(...)` ``.

### C# Expressions (backtick syntax)

`` Property=`expression` `` — the expression re-evaluates automatically whenever any referenced reactive variable changes.

### Binding Directions

| Syntax | Direction | Example |
|--------|-----------|---------|
| `` =`expr` `` | One-way (source→UI) | `` Text=`Name` `` |
| `` =>`var` `` | Bindback (UI→source) | `` SelectedValue=>`Choice` `` |
| `` <=>`var` `` | Two-way | `` Value<=>`Amount` `` |

You can combine one-way + bindback for preprocessing:

```
<NumberBox Value=`Math.Round(Val, 2)` Value=>`Val` />
```

### Events

```
Click+=`(sender, args) => DoSomething()`
@Click+=`Counter++`                        // @ auto-wraps in delegate { ... }
```

### Variable Capture

Assign a tag to a variable accessible from C# code-behind:

```
myButton = <Button Content="Click" />
```

The variable `myButton` becomes a field on the partial class, usable in code-behind methods.

### Inline Tag as Property Value

```
<NumberBox NumberFormatter=<DecimalFormatter IntegerDigits=1 /> />
```

### Inline Collection Children

Use `<>...</>` for collection-typed properties:

```
<Grid RowDefinitions=<>
        <RowDefinition Auto />
        <RowDefinition />
    </>
>
```

### Extension Method Callbacks

An identifier that isn't a recognized property is called as an extension method on the element:

```
<StackPanel CenterH CenterV>
```

This calls `element.CenterH()` and `element.CenterV()`. Define these as extension methods in C#.

### Lambda Callbacks

A standalone backtick expression that is `Action<T>` runs once with the created element:

```
<Grid `x => Grid.SetRow(x, 1)` />
```

### Foreach Loops

```
// Range (lower inclusive, upper exclusive)
foreach (var i in ..3) { <TextBlock Text=/-$"Row {i}"-/ /> }
foreach (var i in 1..4) { <TextBlock Text=/-$"Item {i}"-/ /> }

// Iterable (evaluated once for non-observable collection; reactive to collection changes otherwise)
foreach (var item in items) { <TextBlock Text=`item` /> }
```

### Conditional rendering
```
if (`condition`) {
    <Button Content="True Ruote" />
    <Button Content="Can have multiple" />
} else if (`condition 2`) {
    <TextBlock Content="Else if Ruote" />
    <TextBlock Content="Else if Ruote" />
} else
    <TextBlock Content="Else Ruote, can ignore {curry brackets} when only one item" />
```

Note: for elements that expects single child item, must have `else` condition, and enforced for all branches to have a single child item.

### Root Tag With Properties

`<root>` can carry properties that apply to the class itself (since it inherits from a UI type):

```
<root Background=`bgBrush.Value` CornerRadius=16 Margin=16 Padding=8 />
```

## Setup & Bootstrapping

The entry page must initialize the reactive scheduler so changes propagate automatically:

```csharp
// UWP Example
ReactiveScheduler.AddTickCallbackForCurrentThread(delegate
{
    _ = Dispatcher.TryRunAsync(CoreDispatcherPriority.High, ReactiveScheduler.Tick);
});
```

## Reactivity Infrastructure

For advanced use in C# code-behind (not inside markup):

```csharp
var r = Ref(0);                     // Reference<int>
var c = Computed(() => r.Value + 1); // Computed<int>
r.Watch(val => { ... });            // callback on change
r.Watch(val => { ... }, immediete: true); // also runs immediately
Effect(() => { ... }, ref1, ref2);  // runs when any listed ref changes
```

`ReferenceTracker.NoCapture(() => expr)` reads without tracking dependencies.

## Best Practices From QuickMarkup

- Define **global usings** for common namespaces (`Windows.UI.Xaml.Controls`, `QuickMarkup.Infra`, `static QuickMarkup.Infra.QuickRefs`, etc.) so markup stays clean.
- Define **C# extension methods** (`CenterH`, `CenterV`, `Center`, `Right`, `Bottom`, `StretchH`, `StretchV`) for layout shortcuts.
- Define **C# extension properties** (e.g., `IsVisible`, `Grid_Row`, `Grid_Column`) to work around QuickMarkup not supporting attached properties directly.
- Use `ThemeResources.Get<Brush>("ResourceKey", element).CreateReadOnlyReference()` in `<setup>` for theme-aware brushes. Requires `<PackageReference Include="Get.UI.Data.UWP.NET9" Version="1.0.8" />` (Or WinUI3 equivalent) and the extension below.
- The class must be `partial` (source generator emits the other part). The base class should be a UI element (`Page`, `Grid`, `StackPanel`, etc.).

### `CreateReadOnlyReference` Extension

This bridges `IReadOnlyBinding<T>` (from `Get.UI.Data`) into QuickMarkup's reactivity system:

```csharp
extension<T>(IReadOnlyBinding<T> prop)
{
    public Reference<T> CreateReadOnlyReference()
    {
        var r = new Reference<T>(prop.CurrentValue);
        prop.ValueChanged += (_, val) => r.Value = val;
        return r;
    }
}
```