# MoneyTrack
�o�� Repository �Ȭ��F�O�s�ڹL�h�b�B�z��Ʈw�ɩҶ}�o���@�� API�A����]���|�A�i����@�C�]�����Q��O�Ӧh�ɶ��A�ȫإߤF�@��²�檺�����A�M�ιL�h����Ʈw API �P�M�׬[�c�A�õy�@�����C�ѩ�����]�p�o��²��A�ɭP���ǳ]�p�èS���ϥΨ�C

�b���F Entity Framework �����p�U�A�ڤ���ĳ���s�y���l�C��󨺨ǥi�H�����ϥ� Entity Framework ���H�ӻ��AEntity Framework ���Ѫ��\��w�g���������A���ݭn�ϥγo�Ǧ۩w�q�� API�C��󨺨Ǥ��ϥ� Entity Framework �åB�Ʊ�ۤv�� SQL �y�y���H�ӻ��A��o�ئۤv��{�� API �������פ]���|�Ӱ��C

���ަp���A�o�� API �]�O�ڦb�ǲ� .NET �L�{�����@�������G�A�]�����٬O�Ʊ�O�d���̡C

## Projects
### MoneyTrack.V1
#### �M�׬[�c
�M�׮ج[�ϥ� ASP.NET Web Forms�A���b�}�o Web Forms �ɡA���Ҽ{�إ߭����ҪO�������A���H�����M�רå��u����{���Q�k�C�]���A�o�����ձN�o�Ƿ����i��F��@�C���L�ѹ껡�A���Ǧa��i�঳�ǹL�׳]�p�����áC

�b���@ Web Forms �ɡA�`�`ı�o���Ǭy�{�C�ӭ������b���ơA�ҥH��ܫإ� `WebPageBase` �@���C�� `Page` �������O�A�öi�@�B�W�� `Page_Load()` �����y�{�A�H�Ωw�q�F�ӳ���k�ѦU Page ��@�C����p�U�G
```csharp
public abstract class WebPageBase : Page {
     // �ٲ�

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

    // �ٲ�
}
```

�B�z���s�ƥ�ɡA�ڷ�ɰ��n�Τ@�ϥ� Command Event �A�Ӥ��O Command Event �M Click Event �V�ΡA�ҥH�b `WebPageBase` �إ� `CommandHandler()`�A�M��A�ϥ� Reflection �I�s�U `Page` �������k�C

����ˮ֪������S���ϥΤ��ت� Validation Controls�A�]���Q�ۤv�g�@�����y�y���ˮֱ�����ȡA�ӬO�ϥΦۤv�}�o�uFormValidators�v�ӳB�z�C
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

#### ��Ʈw API
�o�O�ڭ�q PHP �ഫ�� .NET �ɶ}�o�������C��� Entity Framework �|���y��A�o�ɶ}�o������줧�e PHP �M�ת��v�T�C

���e��Ĳ���M�׷|�N�d�ߪ� API �M��Ʋ��ʪ� API ���}�g�]�᭱�� Updater �٩I�^�A�� Updater ���@�k�ä��Τ@�A���ǶȰ���ª��s�W�B�ק�B�R���A���ǫh�]�t�����骺��Ʒ~���޿�A�Ӻ��@ PHP �M�׮ɡAı�o���ӬO�����Ӽh�šA�ѷ~���޿誺 DataUpdater �����I�s��²��ʸ�ƪ� Updater�C

�]���]�p�F��²��ʸ�ƪ� `DataUpdater`�A�èϥ� Transaction Action ���O�ӹ�����@����Ʒ~���޿�A������ı�o�إߤӦh Action ���O�A���Ӷi��ξ�N�O�C

���F�B�z��@��ƪ� CRUD�A�]�p�F `SingleTableQuerier` �P `DataUpdater`�A�èϥ� T4 �d���q��ƮwŪ����ơA�ͦ��������C�Ӹ�ƪ��c�� `DataModel`�A�� `SingleTableQuerier` �P `DataUpdater` �����A�ϥ� `DataModel` ���ͭn���檺 SQL �y�k�C
�p���ݭn����������d�ߦb�B�~�ʸ˦���L�� `Querier`�A����d�Ҧp�U�G
```csharp
public class CategoryTransaction : DataTransaction {
    public CategoryInsertAction CreateInsertAction(string name) {
        return SetAction(new CategoryInsertAction(name));
    }

    // ��L�ٲ�
}

public class CategoryInsertAction : ActionBase {
    public CategoryInsertAction(string name) {
        Name = name;
    }

    public string Name { get; set; }

    public override bool Verify() {
        if (string.IsNullOrWhiteSpace(Name)) {
            ErrorMessage = "�����W�٤��o�� Null �Ϊťզr���C";
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
#### �M�׬[�c
�o�M�רϥ� MVC ���[�c�A��O��L�����@�ǷQ�k��z�_�ӡA���F��K�A�ҥH�S�S�N����M�שM�ϥ� 'ajax'�C

#### ��Ʈw API
�ѩ�蠟�e�]�p����Ʈw API �[�c�����N�A�ҥH��ɦ��h��s Repository Pattern �P Service Layer �[�c�C����N�쥻�� `DataUpdater` ������ Repository Pattern�ATransaction ������ Service Layer�C

��ɰѦҪ��M�׬d�߳]�p�O�z�L�ǤJ `Condition` ����A�ǥ� `Condition` ���ݩʦW�٩M�ȨӲզ� `where` �y�y�A���o�˪��]�p�զ� `where` ����u�� `=`�A�p�G�n��䴩 `>=`�B`<=` �M `like` ���d�߱���A�h�ݬ��C�Ӹ�ƪ����إߦh�ӹ����ݩʡC

���s�]�p��A�N `Condition` ���ݩʫ��O�אּ�ϥΦۦ�w�q `ConditionColumn<T>` ���O�A�Ψ��x�s��@��쪺�h�جd�߱���A����t���p�U�G
```csharp
// RecordDate �p�G�n�䴩 =�B>= �M <=�A�n� RecordDate�BGreaterThanOrEquals_RecordDate�MLessThanOrEquals_RecordDate
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

// �վ��u�ݭn�@�� RecordDate
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
���M�o�˪��]�p���M������A�u��B�z��@��쪺 `or` ����A�L�k�B�z���P���զ��� `or` ����C�b�J��o�ر��p�ɡA�u��i���B�~�B�z�C

��Ʋ��ʪ������]�p�F struct `ValueWatcher<T>` ���A�ΨӺʱ� Service Update �� DTO �����ݩ��ܤơC���i������ɡA `HasValue` �~�|�� `true`�A�A�� AutoMapper �P�_�� `HasValue` ���ȡA�~�N `Value` ������ Entity�A�d�Ҧp�U�G

```csharp
public class CategoryEditor {
    public long Id  { get; set; }

    public ValueWatcher<string> Name { get; set; }
}

CategoryEditor editor = new CategoryEditor(); // editor.Name.HasValue �� false
editor.Name = "����"; // editor.Name.HasValue �� true
```

�b Service Layer �������A�̪�ڧƱ椺�e�󰾦V��B�z��ƪ��~���޿�A�H�����i���ΩʡC���O���ǭ������޿�p�G�g�b Controller ���A�|�ϱoController�L���c���A�o�Ӱ��D�x�Z�F�ڤ@�q�ɶ��A�̲קڨM�w�A������� `Service` �M `PageService`�C���L�A����o�{�w�g�������������s�b�A�ҥH�S���s�R�W�� `DomainService` �M `ApplicationService`�C

### MoneyTrack.V3
#### �M�׬[�c
�o�ӱM�׬O�H Razor Pages �[�c����¦�A�åB�ĥ� Vue 2 ���N jQuery�C�̪�p�e�ѦҤ��e���g���峹 [�p��N Vue 3 �P ASP.NET Razor �@�_�ϥ�](https://hackmd.io/@CloudyWing/HyQqsgr2o) �ӹ�@�A����ӵo�{�ۭq�� `VeeValidateFormTagHelper` �|�ɭP `asp-age-handler` �L�k���`�B�@�A�ӥB�A`DisplayName` �b���ҰT���譱�]�L�k���`��ܡC�]���A�٬O��^�f�t Vue 2�A���L�ѩ�ۭ��I�b�s�]�p����Ʈw API�A�H�ι� Razor Pages �ج[���������Q�A�]���]�S�Τ��� JavaScript �P AJAX�C

�ѹ껡�A�o�ӱM�פ��M�O�b���~�C�Ҧp�AURL �� QueryString �� Route ���[�K�|����@�C�e�~���g�d��L ASP.NET Core ���B�z�o�ǥ\�઺���� Source Code�A���ɶ��w�[�A����쪺 Source Code ���}�]�S���O�s�A��ӴN�i�o�A�h��C���o�@�t�C���M�סA�ڷQ���I�i�@�q���A�]�����A�`�J��s�C

#### ��Ʈw API
�o������Ʈw API �]�p�򥻤W����F V2 �������]�p�A���L�b��Ƭd�ߤ譱�A�ڭ��s�]�p�F API�A�H�ѨM�W�@�����h���f�t `or` ����ɹJ�쪺���D�C�]�p����O�Q�� `Func` �e���Ӻc�� SQL �y�k�A�]���ڨS����O�����ѪR `Expression` �ӥͦ� SQL �y�y�A���h�@�B�� `Func` �٬O�i�檺�C����ϥΦp�U�A���ѹ껡�o�˼g���p�����ϥ� Entity Framework �٤��²��@�I�C
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

�b���`���M�פ��h���p�U�A�q�`�|�N Domain Layer �M DataAccess ����줣�P���M�פ��C���o�@�t�C�M�פ��²��A�]���S���i��M�ש���A�ӬO�b Models ��Ƨ����U�A�ϥθ�Ƨ��ӱN���P Layer ���{���i������C�]���A�o�̪��uModels\Application�v��Ƨ��P�����M�פ��h���uModels�v��Ƨ��O�ۦP���C

�b�W�@�����A�U�����ΦU��쪺���O�Q��m�b�uModels\Application�v��Ƨ����U�����P��Ƨ����A�Ҧp�uCategoryModels�v�B�uRecordModels�v�A�o�O����`���������覡�C��M�A�uModels�v�����O�ڭӤH���ߦn�A�o�˰��i�H�קK�P Entity ���R�W�۽Ĭ�C

�Ӧb�o�@�����A�h�O�N�{���X������b�U�ۭ�����Ƨ����U���uModels�v��Ƨ����C���鵲�c�p�U�ҥܡA�o�˪������覡�ϱo�����P�������{���X��K���a�pô�b�@�_�C�p�G�Y�������A�ݭn�A�h�����R��������Ƨ��Y�i�A���Ƨ��ȩ�m�����O�άO Layout �ϥΪ������{�����q�����O�C
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
