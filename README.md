# Blazor.Virtualization

This is a virtual list component packaged with blazor. Built in layouts such as Waterfall, LinedFlow, and GridList. And it supports incremental loading.

*However, the size of the data items for WaterfallLayout and LinedFlowLayout must be known, or can be calculated based on the data items. So the two layouts don't support auto-sizing items.*

### Installation
##### 1 .NET CLI
```shell
dotnet add package Blazor.Virtualization
```
##### 2 Package Manager
```shell
PM> Install-Package Blazor.Virtualization
```
#### 3 PackageReference
```xml
<PackageReference Include="Blazor.Virtualization" Version="1.0.4" />
````

### Quick start
1. Add the package to project.
2. Add the style reference.
    ```html
    <link href="_content/Blazor.Virtualization/Blazor.Virtualization.bundle.scp.css" rel="stylesheet" />
    ```
3. Add the following reference to the file *_Imports. razor*.
    ```csharp
    @using Blazor.Virtualization
    @using Blazor.Virtualization.Layout
    ```
4. Create razor page or component to use the *VirtualList*.

### Samples

<details open>
<summary>1. WaterfallLayout.</summary>

```razor
<VirtualList TItem="int" Items="data">
    <Layout>
        <WaterfallLayout VirtualList="context"
                         HorizontalSpacing="12"
                         VerticalSpacing="24"
                         HeightCalculator="(o,_)=>o"></WaterfallLayout>
    </Layout>
    <ItemTemplate>
        <div style="height:100%;background-color:#aaccee;">@context</div>
    </ItemTemplate>
</VirtualList>
@code {
    private List<int> data =
    Enumerable
        .Range(0, 500)
        .Select(o => new Random().Next(120, 300))
        .ToList();
}
```
</details>

<details>
<summary>2. Incremental loading WaterafllLayout.</summary>

```razor
<VirtualList TItem="int"
             IncrementalItemsProvider="@this.LoadDataAsync">
    <Layout>
        <WaterfallLayout VirtualList="context"
                         HorizontalSpacing="12"
                         VerticalSpacing="24"
                         HeightCalculator="(o,_)=>o"></WaterfallLayout>
    </Layout>
    <ItemTemplate>
        <div style="height:100%;background-color:#aaccee;">@context</div>
    </ItemTemplate>
</VirtualList>

@code {
    private async ValueTask<IEnumerable<int>> LoadDataAsync()
    {
        await Task.Delay(500);
        var items = Enumerable
              .Range(0, 50)
              .Select(o => new Random().Next(120, 600));
        return items;
    }
}
```
</details>

<details>
<summary>3. GridListLayout</summary>

```razor
<VirtualList TItem="int"
             IncrementalItemsProvider="@this.LoadDataAsync">
    <Layout>
        <GridListLayout VirtualList="context"
                        HorizontalSpacing="12"
                        VerticalSpacing="24"
                        ItemWidth="236"></GridListLayout>
    </Layout>
    <ItemTemplate>
        <div style="height:100%;background-color:#aaccee;">@context</div>
    </ItemTemplate>
</VirtualList>

@code {
    private async ValueTask<IEnumerable<int>> LoadDataAsync()
    {
        await Task.Delay(300);
        var items = Enumerable
              .Range(0, 50)
              .Select(o => new Random().Next(120, 600));
        return items;
    }
}
```
</details>

<details>
<summary>4. LindedFlowLayout</summary>

```razor
<VirtualList TItem="int"
             IncrementalItemsProvider="@this.LoadDataAsync">
    <Layout>
        <LinedFlowLayout VirtualList="context"
                         HorizontalSpacing="8"
                         VerticalSpacing="24"
                         WidthCalculator="o=>o*1f"></LinedFlowLayout>
    </Layout>
    <ItemTemplate>
        <div style="height:100%;background-color:#aaccee;">@context</div>
    </ItemTemplate>
</VirtualList>

@code {
    private async ValueTask<IEnumerable<int>> LoadDataAsync()
    {
        await Task.Delay(300);
        var items = Enumerable
              .Range(0, 50)
              .Select(o => new Random().Next(120, 300));
        return items;
    }
}
```
</details>

### API
#### VirtualList

| Name                     | Type                   | Default value | Description                                 |
|--------------------------|------------------------|---------------|---------------------------------------------|
| Items                    | IList<T>               | null          | The datasource.                             |
| Layout                   | RenderFragment<TItem>  | null          | The list layout.                            |
| EmptyTemplate            | RenderFragment         | null          | The template if no data.                    |
| IncrementalItemsProvider | ValueTask<List<TItem>> | null          | The Incremental loading datasource provider |

#### WaterafallLayout

| Name              | Type                      | Default value | Description                                                                                                                                                                              |
|-------------------|---------------------------|---------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| ItemWidth         | float                     | 0             | If this value is greater than 0, then MinItemWidth will be invalid and all items will be centered.                                                                                       |
| HorizontalSpacing | float                     | 8             | The horizontal spacing of items.                                                                                                                                                         |
| VerticalSpacing   | float                     | 8             | The vertical spacing of items.                                                                                                                                                           |
| HeightCalculator  | Func<TItem, float, float> | null          | The calculator of item height. <br>The first parameter is the data item. <br>And then second parameter is the ItemWidth. <br>You can calculate the height based on these two parameters. |
| MinItemWidth      | float                     | 200           | The minimum width of the item.                                                                                                                                                           |
| MinColumnCount    | float                     | 1             | The minimum column count of the component.                                                                                                                                               |

#### GridListLayout

| Name              | Type  | Default value | Description                                                                                        |
|-------------------|-------|---------------|----------------------------------------------------------------------------------------------------|
| ItemWidth         | float | 0             | If this value is greater than 0, then MinItemWidth will be invalid and all items will be centered. |
| HorizontalSpacing | float | 8             | The horizontal spacing of items.                                                                   |
| VerticalSpacing   | float | 8             | The vertical spacing of items.                                                                     |
| MinItemWidth      | float | 200           | The minimum width of the item.                                                                     |
| MinColumnCount    | float | 1             | The minimum column count of the component.                                                         |
| ItemHeight        | float | 0             | The height of all the items. If the value is greater than 0, then the height will be equal to it.  |

#### LinedFlowLayout

| Name              | Type               | Default value | Description                                                        |
|-------------------|--------------------|---------------|--------------------------------------------------------------------|
| HorizontalSpacing | float              | 8             | The horizontal spacing of items.                                   |
| VerticalSpacing   | float              | 8             | The vertical spacing of items.                                     |
| RowHeight         | float              | 200           | The height of all the rows.                                        |
| WidthCalculator   | Func<TItem, float> | null          | The calculator of the minwidth. The parameter is data of the item. |