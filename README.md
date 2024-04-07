# MoneyTrack
這個 Repository 僅為了保存我過去在處理資料庫時所開發的一些 API，後續也不會再進行維護。因為不想花費太多時間，僅建立了一個簡單的網站，套用過去的資料庫 API 與專案架構，並稍作完善。由於網站設計得太簡單，導致有些設計並沒有使用到。

在有了 Entity Framework 的情況下，我不建議重新造輪子。對於那些可以接受使用 Entity Framework 的人來說，Entity Framework 提供的功能已經足夠完善，不需要使用這些自定義的 API。對於那些不使用 Entity Framework 並且希望自己組 SQL 語句的人來說，對這種自己實現的 API 的接受度也不會太高。

儘管如此，這些 API 也是我在學習 .NET 過程中的一部分成果，因此我還是希望保留它們。

## Projects
### MoneyTrack.V1
#### 專案架構
專案框架使用 ASP.NET Web Forms，當初在開發 Web Forms 時，有考慮建立頁面模板的概念，但以往的專案並未真正實現此想法。因此，這次嘗試將這些概念進行了實作。不過老實說，有些地方可能有些過度設計的嫌疑。

在維護 Web Forms 時，常常覺得有些流程每個頁面都在重複，所以選擇建立 `WebPageBase` 作為每個 `Page` 的基底類別，並進一步規劃 `Page_Load()` 內部流程，以及定義了細部方法供各 Page 實作。具體如下：
```csharp
public abstract class WebPageBase : Page {
     // 省略

    protected override void OnLoad(EventArgs e) {
        base.OnLoad(e);

        IEnumerable<string> messages = UserContext.PageTransfer
            .GetMessages(PageTransferContext.AnyPage, GetType().BaseType.Name);

        foreach (string message in messages) {
            Script.Alert(message);
        }

        UserContext.PageTransfer
            .RemoveMessages(PageTransferContext.AnyPage, GetType().BaseType.Name);

        if (!IsPostBack) {
            if (UserContext.PageTransfer.Variables.Any()) {
                LoadTransferVariables(UserContext.PageTransfer.Variables);
            }

            BindControlDataOnce();

            if (UserContext.PageCache.Variables.Any()) {
                LoadPageCacheVariables(UserContext.PageCache.Variables);
            }
        }

        BindControlData();
    }

    /// <summary>
    /// Triggered on the initial page load; won't execute again during PostBacks. Mainly used for binding control data sources and setting initial values.
    /// </summary>
    protected virtual void BindControlDataOnce() { }

    /// <summary>
    /// Triggered on each page load, executed at the end of <c>Page_Load</c>. Used for operations like setting <c>PagedListPager.DataBinder = BindData</c>.
    /// </summary>
    protected virtual void BindControlData() { }

    /// <summary>
    /// Triggered on the initial page load; won't execute again during PostBacks. Executed before <c>BindControlDataOnce</c>. Mainly used to read passed values from other pages, process them, and set data sources or values for page controls. For example, on an edit page, querying existing data using the passed <c>Id</c> from a search page, then setting values to respective controls.
    /// </summary>
    protected virtual void LoadTransferVariables(VariableDictionary variables) { }

    /// <summary>
    /// Triggered on the initial page load; won't execute again during PostBacks. Executed after <c>BindControlDataOnce</c>. Mainly used to read previously cached values, such as storing query conditions on a query page and retrieving them when returning from an edit page to prevent loss of query conditions.
    /// </summary>
    protected virtual void LoadPageCacheVariables(VariableDictionary variables) { }

    protected override void OnPreRender(EventArgs e) {
        base.OnPreRender(e);

        if (IsPostBack || AutoLoadData) {
            BindData();
        }

        SitePageTitle(SiteMap.RootNode);
    }

    protected virtual void BindData() { }

    // 省略
}
```

處理按鈕事件時，我當時偏好統一使用 Command Event ，而不是 Command Event 和 Click Event 混用，所以在 `WebPageBase` 建立 `CommandHandler()`，然後再使用 Reflection 呼叫各 `Page` 的具體方法。

資料檢核的部分沒有使用內建的 Validation Controls，也不想自己寫一堆條件語句來檢核控制項的值，而是使用自己開發「FormValidators」來處理。
```csharp
BulkValidator validators = new BulkValidator(cfg => {
    cfg.Add(lblCategoryId.Text, ddlCategoryId.SelectedValue, opt => opt.Integer());
    cfg.Add(lblIncome.Text, rblIncome.SelectedValue, opt => opt.Bool());
    cfg.Add(lblStartDate.Text, txtStartDate.Text, opt => opt.DateTime());
    cfg.Add(lblEndDate.Text, txtEndDate.Text, opt => opt.DateTime());
    cfg.Add(lblMinAmount.Text, txtMinAmount.Text, opt => opt.Integer());
    cfg.Add(lblMaxAmount.Text, txtMaxAmount.Text, opt => opt.Integer());
});

if (!validators.Validate()) {
    Script.Alert(validators.ErrorMessageWithLF);
    return;
}
```

#### 資料庫 API
這是我剛從 PHP 轉換到 .NET 時開發的產物。當時 Entity Framework 尚未流行，這時開發思路受到之前 PHP 專案的影響。

之前接觸的專案會將查詢的 API 和資料異動的 API 分開寫（後面用 Updater 稱呼），但 Updater 的作法並不統一，有些僅做單純的新增、修改、刪除，有些則包含較具體的資料業務邏輯，而維護 PHP 專案時，覺得應該是拆分兩個層級，由業務邏輯的 DataUpdater 內部呼叫單純異動資料的 Updater。

因此設計了單純異動資料的 `DataUpdater`，並使用 Transaction Action 類別來對應單一的資料業務邏輯，但後續覺得建立太多 Action 類別，應該進行統整就是。

為了處理單一資料表的 CRUD，設計了 `SingleTableQuerier` 與 `DataUpdater`，並使用 T4 範本從資料庫讀取資料，生成對應的每個資料表結構的 `DataModel`，而 `SingleTableQuerier` 與 `DataUpdater` 內部再使用 `DataModel` 產生要執行的 SQL 語法。
如有需要比較複雜的查詢在額外封裝成其他的 `Querier`，具體範例如下：
```csharp
public class CategoryTransaction : DataTransaction {
    public CategoryInsertAction CreateInsertAction(string name) {
        return SetAction(new CategoryInsertAction(name));
    }

    // 其他省略
}

public class CategoryInsertAction : ActionBase {
    public CategoryInsertAction(string name) {
        Name = name;
    }

    public string Name { get; set; }

    public override bool Verify() {
        if (string.IsNullOrWhiteSpace(Name)) {
            ErrorMessage = "分類名稱不得為 Null 或空白字元。";
            return false;
        }

        return true;
    }

    public override bool Execute() {
        SingleTableQuerier querier = new SingleTableQuerier(CategoryModel);
        int displayOrder = querier.GetCount() + 1;

        DataUpdater updater = new DataUpdater(CategoryModel);
        updater.SetValue(CategoryModel.Name, Name);
        updater.SetValue(CategoryModel.DisplayOrder, displayOrder);

        return updater.Insert() > 0;
    }
}
```

### MoneyTrack.V2
#### 專案架構
這專案使用 MVC 的架構，算是把過往的一些想法整理起來，為了方便，所以沒特意拆分專案和使用 'ajax'。

#### 資料庫 API
由於對之前設計的資料庫 API 架構不滿意，所以當時有去研究 Repository Pattern 與 Service Layer 架構。後續將原本的 `DataUpdater` 對應到 Repository Pattern，Transaction 對應到 Service Layer。

當時參考的專案查詢設計是透過傳入 `Condition` 物件，藉由 `Condition` 的屬性名稱和值來組成 `where` 語句，但這樣的設計組成 `where` 條件只有 `=`，如果要能支援 `>=`、`<=` 和 `like` 等查詢條件，則需為每個資料表欄位建立多個對應屬性。

重新設計後，將 `Condition` 的屬性型別改為使用自行定義 `ConditionColumn<T>` 類別，用來儲存單一欄位的多種查詢條件，具體差異如下：
```csharp
// RecordDate 如果要支援 =、>= 和 <=，要拆成 RecordDate、GreaterThanOrEquals_RecordDate和LessThanOrEquals_RecordDate
public PagedList<IndexListItemViewModel> GetList(IndexViewModel viewModel) {
    RecordQueryCondition condition = new RecordQueryCondition {
        CategoryId = viewModel.CategoryId,
        IsIncome = viewModel.Income == 1 ? true
            : viewModel.Income == 2 ? (bool?)false
            : null,
        GreaterThanOrEquals_RecordDate = viewModel.StartDate,
        LessThanOrEquals_RecordDate = viewModel.EndDate,
        Contains_Description = viewModel.Description
    };

    return recordService.GetPagedList<IndexListItemViewModel>(condition, viewModel.PageNumber, 20);
}

// 調整後只需要一個 RecordDate
public PagedList<IndexListItemViewModel> GetList(IndexViewModel viewModel) {
    RecordQueryCondition condition = new RecordQueryCondition {
        CategoryId = viewModel.CategoryId,
        IsIncome = viewModel.Income == 1 ? true
            : viewModel.Income == 2 ? (bool?)false
            : null,
        RecordDate = OperatorUtils.GreaterThanOrEquals(viewModel.StartDate)
            & OperatorUtils.LessThanOrEquals(viewModel.EndDate),
        Description = OperatorUtils.Contains(viewModel.Description)
    };

    return recordService.GetPagedList<IndexListItemViewModel>(condition, viewModel.PageNumber, 20);
}
```
雖然這樣的設計仍然有限制，只能處理單一欄位的 `or` 條件，無法處理不同欄位組成的 `or` 條件。在遇到這種情況時，只能進行額外處理。

資料異動的部分設計了 struct `ValueWatcher<T>` 的，用來監控 Service Update 的 DTO 中的屬性變化。當有進行指派時， `HasValue` 才會為 `true`，再用 AutoMapper 判斷當 `HasValue` 有值，才將 `Value` 對應到 Entity，範例如下：

```csharp
public class CategoryEditor {
    public long Id  { get; set; }

    public ValueWatcher<string> Name { get; set; }
}

CategoryEditor editor = new CategoryEditor(); // editor.Name.HasValue 為 false
editor.Name = "飲食"; // editor.Name.HasValue 為 true
```

在 Service Layer 的部分，最初我希望內容更偏向於處理資料的業務邏輯，以提高可重用性。但是有些頁面的邏輯如果寫在 Controller 中，會使得Controller過於繁雜，這個問題困擾了我一段時間，最終我決定再次拆分為 `Service` 和 `PageService`。不過，後續發現已經有類似的概念存在，所以又重新命名為 `DomainService` 和 `ApplicationService`。

### MoneyTrack.V3
#### 專案架構
這個專案是以 Razor Pages 架構為基礎，並且採用 Vue 2 取代 jQuery。最初計畫參考之前撰寫的文章 [如何將 Vue 3 與 ASP.NET Razor 一起使用](https://hackmd.io/@CloudyWing/HyQqsgr2o) 來實作，但後來發現自訂的 `VeeValidateFormTagHelper` 會導致 `asp-age-handler` 無法正常運作，而且，`DisplayName` 在驗證訊息方面也無法正常顯示。因此，還是改回搭配 Vue 2，不過由於著重點在新設計的資料庫 API，以及對 Razor Pages 框架本身的探討，因為也沒用什麼的 JavaScript 與 AJAX。

老實說，這個專案仍然是半成品。例如，URL 的 QueryString 或 Route 的加密尚未實作。前年曾經查找過 ASP.NET Core 中處理這些功能的相關 Source Code，但時間已久，當初找到的 Source Code 網址也沒有保存，後來就懶得再去找。對於這一系列的專案，我想早點告一段落，因此不再深入研究。

#### 資料庫 API
這次的資料庫 API 設計基本上延續了 V2 版本的設計，不過在資料查詢方面，我重新設計了 API，以解決上一版中多欄位搭配 `or` 條件時遇到的問題。設計思路是利用 `Func` 委派來構建 SQL 語法，因為我沒有能力直接解析 `Expression` 來生成 SQL 語句，但退一步用 `Func` 還是可行的。具體使用如下，但老實說這樣寫不如直接使用 Entity Framework 還比較簡潔一點。
```csharp
PagedList<IndexRecordModel> pagedList = await recordService.GetPagedList<IndexRecordModel>(
    x => x.Column(y => y.Record!.Id, y => y.Id)
        .Column(y => y.Category!.Name, y => y.CategoryName)
        .Column(y => y.Record!.RecordDate, y => y.RecordDate)
        .Column(y => y.Record!.IsIncome, y => y.IsIncome)
        .Column(y => y.Record!.Amount, y => y.Amount)
        .Column(y => y.Record!.Description, y => y.Description),
    x => x.Desc(y => y.Record!.RecordDate),
    input.PageNumber, 20,
    x => x.Column(y => y.Record!.CategoryId).Equal(input.CategoryId.Value)
        .And(x.Column(y => y.Record!.RecordDate) >= input.StartDate.Value)
        .And(x.Column(y => y.Record!.RecordDate) <= input.EndDate.Value)
        .And(x.Column(y => y.Record!.Description).Contain(input.Description))
```

在正常的專案分層情況下，通常會將 Domain Layer 和 DataAccess 拆分到不同的專案中。但這一系列專案比較簡單，因此沒有進行專案拆分，而是在 Models 資料夾底下，使用資料夾來將不同 Layer 的程式進行分類。因此，這裡的「Models\Application」資料夾與有做專案分層的「Models」資料夾是相同的。

在上一版中，各頁面或各領域的類別被放置在「Models\Application」資料夾底下的不同資料夾中，例如「CategoryModels」、「RecordModels」，這是比較常見的分類方式。當然，「Models」結尾是我個人的喜好，這樣做可以避免與 Entity 的命名相衝突。

而在這一版中，則是將程式碼直接放在各自頁面資料夾底下的「Models」資料夾中。具體結構如下所示，這樣的分類方式使得頁面與相關的程式碼更密切地聯繫在一起。如果某頁面不再需要，則直接刪除頁面資料夾即可，原資料夾僅放置父類別或是 Layout 使用的相關程式等通用類別。
```
Pages/
|-- Categories/
|   |-- Models/
|   |   |-- IndexInputModel.cs
|   |   |-- CategoryIndexAppService.cs
|   |-- Index.cshtml
|-- Records/
|   |-- Models/
|   |   |-- IndexInputModel.cs
|   |   |-- RecordIndexAppService.cs
|   |-- Index.cshtml
...

```

## SQL
```sql
CREATE TABLE [dbo].[Categories] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](255) NOT NULL,
    [DisplayOrder] [bigint] NOT NULL,
CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED (
    [Id] ASC
) WITH (
    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Records] (
    [Id] [bigint] IDENTITY(1,1) NOT NULL,
    [CategoryId] [bigint] NOT NULL,
    [RecordDate] [date] NOT NULL,
    [IsIncome] [bit] NOT NULL,
    [Amount] [bigint] NOT NULL,
    [Description] [nvarchar](max) NOT NULL,
CONSTRAINT [PK_Records] PRIMARY KEY CLUSTERED (
    [Id] ASC
) WITH (
    PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
```
