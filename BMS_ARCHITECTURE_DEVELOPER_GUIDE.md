# راهنمای معماری و توسعه BMS (به‌جز Worker)

> مخاطب: توسعه‌دهنده‌ای که بخش Worker را می‌شناسد اما با بقیه پروژه آشنا نیست  
> محدوده: `BMS.Domain`، `BMS.Application`، `BMS.Infrastructure` و `WebApi`  
> تاریخ بررسی: 2026-08-14  
> مبنا: branch `develop`، commit `c66c910`

## 1. پاسخ کوتاه به دو سؤال اصلی

### آیا پروژه CQRS است؟

بله، در لایه Application از الگوی CQRS استفاده شده است:

- عملیات تغییردهنده با کلاس‌های `Command` مدل شده‌اند.
- عملیات خواندن با کلاس‌های `Query` مدل شده‌اند.
- هر درخواست یک `IRequest<TResponse>` در MediatR است.
- منطق آن در `IRequestHandler<TRequest,TResponse>` اجرا می‌شود.
- Controller معمولاً فقط `_mediator.Send(...)` را صدا می‌زند.

در زمان بررسی حدود ۶۰ فایل دارای `IRequest<T>`، حدود ۶۸ Handler و ۶۹ فراخوانی MediatR در Controllerها وجود داشت.

اما معماری «CQRS خالص یا فیزیکی» نیست:

- Command و Query از همان SQL Server و همان `BMSDbContext` استفاده می‌کنند.
- read model یا دیتابیس خواندن جداگانه وجود ندارد.
- بعضی endpointها مستقیماً UseCase یا service را صدا می‌زنند و MediatR را دور می‌زنند.
- event bus یا message broker خارجی وجود ندارد.

پس تعریف دقیق‌تر این است:

> پروژه از CQRS در سطح ساختار کد و جداسازی Command/Query استفاده می‌کند، نه CQRS توزیع‌شده با دیتابیس‌های جدا.

### آیا پروژه Event-Based است؟

به‌صورت محدود بله، ولی Event Sourcing یا معماری کاملاً Event-Driven نیست.

وضعیت فعلی:

- یک marker interface به نام `BMS.Domain.Events.IDomainEvent` وجود دارد.
- eventهایی مانند `DataPointUpdatedDomainEvent` و `DeviceCommandCompletedDomainEvent` وجود دارند.
- handlerها `IEventHandler<TEvent>` را پیاده‌سازی می‌کنند.
- `EventDispatcher` handlerها را از DI پیدا و در همان process اجرا می‌کند.

محدودیت‌ها:

- Base class مشترکی برای Event وجود ندارد؛ فقط interface خالی وجود دارد.
- `BaseEntity<TKey>` لیست `DomainEvents` ندارد.
- Entityها event را ذخیره نمی‌کنند؛ معمولاً event را به caller برمی‌گردانند.
- caller باید دستی `DispatchAsync` را اجرا کند.
- dispatch هم‌زمان و in-process است.
- broker، queue پایدار، outbox، retry پایدار و event store وجود ندارد.
- state سیستم از eventها بازسازی نمی‌شود؛ بنابراین Event Sourcing نیست.
- خطای event handler در dispatcher فقط log می‌شود و به caller منتقل نمی‌شود.

تعریف دقیق:

> پروژه Domain Event داخلی دارد، اما event-based بودن آن مکمل CQRS است و ستون اصلی persistence محسوب نمی‌شود.

## 2. تصویر کلی معماری

```text
Client / Angular / Integration
              │
              ▼
          WebApi
     Controller + Auth
              │
              ▼
      BMS.Application
 Commands / Queries / Handlers
 Validators / Behaviors / UseCases
       │                 │
       ▼                 ▼
   BMS.Domain      Abstraction Interfaces
 Entities/Rules     Repository/Gateway/Event
       ▲                 │
       │                 ▼
       └──── BMS.Infrastructure
          EF Core / SQL Server
          Security / SignalR
          Command Queue / Historian
```

قاعده مطلوب وابستگی:

- Domain نباید به لایه دیگری وابسته باشد.
- Application فقط به Domain وابسته است و interfaceهای مورد نیاز را تعریف می‌کند.
- Infrastructure interfaceهای Application را پیاده‌سازی می‌کند.
- WebApi فقط composition root و ورودی HTTP است.

در کد فعلی این جهت کلی رعایت شده، ولی چند prototype و reference قدیمی باعث شده مرزها کاملاً تمیز نباشند.

## 3. مسئولیت لایه Domain

مسیر: `BMS.Domain`

Domain باید شامل مدل و قوانین مستقل کسب‌وکار باشد؛ نه EF Core، HTTP، JWT یا SQL.

### 3.1 BaseEntity

`BaseEntity<TKey>` ویژگی‌های مشترک زیر را دارد:

- `Id`
- `IsDeleted`
- `CreatedAtUtc`
- `UpdatedAtUtc`
- `MarkAsDeleted()`
- `SetUpdated()`

برای `Guid`، شناسه در constructor ساخته می‌شود.

نکته: `BaseEntity` در وضعیت فعلی Domain Event collection ندارد. اگر entity رویداد تولید کند، آن را از متد برمی‌گرداند؛ برای نمونه `Device.UpdatePoint` یک `DataPointUpdatedDomainEvent` برمی‌گرداند.

### 3.2 موجودیت‌های امنیتی

- `User`: اطلاعات login، password hash، وضعیت فعال بودن، lock و permission version.
- `Person`: مشخصات فردی مرتبط با User.
- `Role` و `Permission`: تعریف نقش و دسترسی.
- `UserRole`: انتساب نقش به کاربر.
- `RolePermission`: دسترسی‌های نقش.
- `UserPermission`: override مستقیم allow/deny برای یک کاربر.

### 3.3 ساختار مکانی

```text
Site → Building → Floor → Ward → Room
```

Featureهای Application برای هر سطح معمولاً Create، Update، Delete، GetById، GetAll و GetByParent دارند.

### 3.4 مدل BMS

- `Controller`: کنترلر یا PLC و مشخصات ارتباطی آن.
- `Device`: تجهیز منطقی وابسته به Controller.
- `Point`: رجیستر یا نقطه داده دستگاه.
- `CommandDefinition`: تعریف فرمان و Point متناظر آن.
- `DeviceCommand`: یک فرمان ایجادشده برای اجرا.
- `CommandResult`: نتیجه اجرای فرمان.
- `DeviceSchedule`: تنظیمات زمان‌بندی دستگاه.

`Device` بخشی از state لحظه‌ای Pointها را نیز نگهداری می‌کند و متدهایی مانند `UpdatePoint`، `GetDeviceState` و `ExecuteCommand` دارد.

### 3.5 Domain Eventها

Eventهای شناخته‌شده:

- `DataPointUpdatedDomainEvent`
- `GetDeviceStateDomainEvent`
- `DeviceCommandExecutedDomainEvent`
- `DeviceCommandCompletedDomainEvent`
- `AlarmRaisedDomainEvent`

دو تعریف با نام مشابه در پروژه وجود دارد:

1. `BMS.Domain.Events.IDomainEvent`: interface واقعی و مورد استفاده.
2. `BMS.Domain.Abstractions.IDomainEvent`: یک class خالی و ظاهراً بلااستفاده.

برای توسعه جدید فقط از `BMS.Domain.Events.IDomainEvent` استفاده شود. تعریف دوم باید در یک cleanup آینده حذف یا اصلاح شود.

## 4. مسئولیت لایه Application

مسیر: `BMS.Application`

Application محل orchestration است:

- دریافت یک Command یا Query
- اعتبارسنجی درخواست
- بارگذاری entity از repository
- اجرای قانون دامنه
- ذخیره تغییرات با UnitOfWork
- ساخت DTO پاسخ
- در صورت نیاز dispatch کردن Domain Event

### 4.1 ساختار معمول Feature

نمونه Site:

```text
BMS.Application/Location/Sites/
├─ Commands/
│  ├─ CreateSiteCommand.cs
│  ├─ UpdateSiteCommand.cs
│  └─ DeleteSiteCommand.cs
├─ Queries/
│  ├─ GetSitesListQuery.cs
│  └─ GetSiteByIdQuery.cs
├─ Handlers/
│  ├─ CreateSiteCommandHandler.cs
│  ├─ UpdateSiteCommandHandler.cs
│  ├─ DeleteSiteCommandHandler.cs
│  ├─ GetSitesListQueryHandler.cs
│  └─ GetSiteByIdQueryHandler.cs
└─ Dtos/
   └─ SiteDto.cs
```

بعضی featureها پوشه `Validators` نیز دارند.

### 4.2 Command

Command نماینده قصد تغییر state است:

```csharp
[Audit(EventType.AddData, "Sites")]
public sealed class CreateSiteCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public string? Address { get; set; }
    public string? Description { get; set; }
}
```

نکات:

- نام باید به `Command` ختم شود.
- Response باید مشخص باشد؛ مثلاً `Guid`، `bool` یا `ApiResponse<T>`.
- برای عملیات قابل audit از `[Audit]` استفاده می‌شود.
- Command نباید منطق دیتابیس یا HTTP داشته باشد.

### 4.3 Command Handler

الگوی معمول:

```csharp
public sealed class CreateSiteCommandHandler
    : IRequestHandler<CreateSiteCommand, Guid>
{
    private readonly ISiteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Guid> Handle(
        CreateSiteCommand request,
        CancellationToken cancellationToken)
    {
        var site = new Site(request.Name, request.Address, request.Description);
        await _repository.AddAsync(site, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return site.Id;
    }
}
```

ترتیب پیشنهادی در Handler:

1. بارگذاری وابستگی‌های دامنه.
2. بررسی not-found و conflict.
3. اجرای رفتار روی entity.
4. Add/Update/Delete از طریق repository.
5. یک بار `SaveChangesAsync`.
6. dispatch event فقط اگر رویداد واقعی رخ داده است.
7. بازگرداندن DTO یا نتیجه.

### 4.4 Query

Query فقط برای خواندن است:

```csharp
public sealed class GetSitesListQuery
    : PagedRequest, IRequest<PagedResult<SiteDto>>
{
    public string? Search { get; set; }
}
```

Query handler معمولاً از `IQueryable` repository استفاده و مستقیم به DTO projection می‌کند تا entityهای کامل بی‌دلیل load نشوند.

برای queryها:

- `AsNoTracking` ترجیح داده می‌شود.
- pagination باید قبل از materialize اعمال شود.
- entity نباید مستقیم به API برگردد.
- query نباید state دیتابیس را تغییر دهد.

### 4.5 Repository Interfaceها

interfaceها در `BMS.Application/Common/Interfaces` قرار دارند؛ مانند:

- `ISiteRepository`
- `IBuildingRepository`
- `IControllerRepository`
- `IDeviceRepository`
- `IPointRepository`
- `IUserRepository`
- `IUnitOfWork`

Application فقط interface را می‌شناسد. پیاده‌سازی در Infrastructure قرار می‌گیرد.

### 4.6 Validation Behavior

`ValidationBehavior<TRequest,TResponse>` قبل از Handler تمام `IValidator<TRequest>`ها را اجرا می‌کند. در صورت خطا `FluentValidation.ValidationException` ایجاد می‌شود و Middleware آن را به HTTP 400 تبدیل می‌کند.

برای feature جدید بهتر است Validator جدا ساخته شود:

```csharp
public sealed class CreateExampleCommandValidator
    : AbstractValidator<CreateExampleCommand>
{
    public CreateExampleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
```

Validatorها با assembly scanning ثبت می‌شوند؛ معمولاً ثبت دستی لازم نیست.

### 4.7 Audit Behavior

Commandهایی که `[Audit]` دارند از `AuditLoggingBehavior` عبور می‌کنند. Behavior موارد زیر را ثبت می‌کند:

- UserId
- IP
- User-Agent
- نوع عملیات و object name
- request body پس از sanitize
- زمان اجرا
- موفقیت یا خطا

نکته: Response controller هنگام اجرای behavior هنوز ممکن است status نهایی را تنظیم نکرده باشد؛ بنابراین اتکا به HTTP status در این نقطه باید با احتیاط باشد.

### 4.8 ApiResponse و Exception

در پروژه دو سبک پاسخ دیده می‌شود:

- بازگرداندن مستقیم DTO/Guid/PagedResult
- استفاده از `ApiResponse<T>`

این ناهمگونی باید در توسعه آینده کاهش یابد. برای feature جدید ابتدا سبک featureهای مجاور را رعایت کنید، سپس در refactor جداگانه قرارداد پاسخ را یکسان کنید.

Exceptionهای مناسب موجود:

- `NotFoundException`
- `BusinessRuleException`
- Domain exceptionها
- FluentValidation exception

از `throw new Exception(...)` برای خطاهای قابل انتظار کسب‌وکار استفاده نشود، زیرا Middleware آن را خطای ناشناخته 500 در نظر می‌گیرد.

## 5. مسئولیت لایه Infrastructure

مسیر: `BMS.Infrastructure`

### 5.1 EF Core و DbContext

`BMSDbContext` نقطه اصلی persistence است. Configurationها با این دستور load می‌شوند:

```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(BMSDbContext).Assembly);
```

برای Entity جدید باید:

1. Entity در Domain ساخته شود.
2. `DbSet<TEntity>` به `BMSDbContext` اضافه شود.
3. یک `IEntityTypeConfiguration<TEntity>` در `Persistence/Configurations` ایجاد شود.
4. migration ساخته و بررسی شود.

در snapshot فعلی migrationها داخل repository نیستند؛ قبل از توسعه schema باید درباره محل نگهداری migration با تیم تصمیم‌گیری شود.

### 5.2 Repository Implementation

پیاده‌سازی repositoryها در `BMS.Infrastructure/Repositories` یا `Persistence/Repositories` قرار دارد.

Repository مسئول query و tracking است، اما معمولاً `SaveChanges` را اجرا نمی‌کند. commit تغییرات با `IUnitOfWork` در Handler انجام می‌شود.

الگوی مناسب:

```csharp
public sealed class ExampleRepository : IExampleRepository
{
    private readonly BMSDbContext _db;

    public IQueryable<Example> Examples => _db.Examples;

    public Task<Example?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Examples.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AddAsync(Example entity, CancellationToken ct) =>
        _db.Examples.AddAsync(entity, ct).AsTask();
}
```

### 5.3 Dependency Injection

ثبت‌های Infrastructure در `BMS.Infrastructure/DependencyInjection.cs` انجام می‌شود.

هنگام افزودن repository جدید:

```csharp
services.AddScoped<IExampleRepository, ExampleRepository>();
```

راهنمای lifetime:

- Repository و DbContext: `Scoped`
- service بدون state وابسته به request: معمولاً `Scoped` یا `Transient`
- cache/thread-safe queue/channel: `Singleton`
- Hosted Service: با `AddHostedService`

هرگز DbContext یا repository scoped را مستقیماً داخل singleton نگهداری نکنید. Singleton باید با `IServiceScopeFactory` در زمان کار scope بسازد.

### 5.4 Security

اجزای اصلی:

- `PasswordHasher`: BCrypt
- `JwtProvider`: تولید JWT
- `PermissionResolver`: محاسبه Permissionهای نقش و override کاربر
- `PermissionPolicyProvider`: ساخت policy بر اساس نام Permission
- `PermissionHandler`: بررسی claimهای `perm`

Permissionهای ثابت در `BMS.Infrastructure/Seeds/PermissionKeys.cs` تعریف می‌شوند و seedهای Permission/Role نیز در Infrastructure هستند.

### 5.5 EventDispatcher

`EventDispatcher`:

1. نوع runtime رویداد را می‌گیرد.
2. `IEventHandler<TEvent>` را با reflection می‌سازد.
3. handlerها را از DI scope دریافت می‌کند.
4. آن‌ها را به‌ترتیب و با `await` اجرا می‌کند.
5. خطای هر handler را log کرده و ادامه می‌دهد.

نتیجه مهم:

- اجرای event handler بخشی از همان process است.
- ترتیب handlerها قرارداد صریح و تضمین پایدار ندارد.
- failure یک handler transaction اصلی را rollback نمی‌کند.
- event برای restart یا crash ذخیره نمی‌شود.

برای عملیات حیاتی مثل ارسال فرمان واقعی یا یکپارچگی بین سرویس‌ها، به event فعلی به‌تنهایی تکیه نشود؛ Outbox یا queue پایدار لازم است.

### 5.6 Realtime و SignalR

Hubهای realtime در Infrastructure قرار دارند. `BMSHub` مسیر عضویت در group دستگاه و درخواست state را ارائه می‌کند. Handlerهای event نیز با `IHubContext` داده را برای clientها ارسال می‌کنند.

SignalR state دائمی نیست و در deployment چند instance به backplane مانند Redis نیاز دارد.

### 5.7 Command Queue

هرچند اجرای فیزیکی فرمان به Worker می‌رسد، بخش ابتدایی جریان در Infrastructure است:

- `GlobalDeviceCommandQueue`
- `CommandProcessorBackgroundService`
- `IDeviceCommandGateway`
- `SimulatedDeviceGateway`

Command processor چهار loop موازی دارد و برای هر Device یک `SemaphoreSlim` می‌سازد تا فرمان‌های یک دستگاه هم‌زمان اجرا نشوند.

نکات توسعه:

- queue فعلی in-memory است و با restart از بین می‌رود.
- dictionary قفل‌های دستگاه cleanup ندارد.
- نتیجه gateway در وضعیت فعلی به lifecycle پایدار command متصل نیست.
- برای commandهای حیاتی باید وضعیت آن‌ها در دیتابیس ذخیره شود.

### 5.8 Historian

Historian از Channel و batch insert استفاده می‌کند، اما در کد فعلی background service در ابتدای ExecuteAsync خارج می‌شود و فعال نیست.

تا زمان رفع این مشکل، فرض نکنید داده‌های realtime در `DataPointHistory` ذخیره می‌شوند.

## 6. مسئولیت WebApi

مسیر: `WebApi`

### 6.1 Program.cs به‌عنوان Composition Root

موارد اصلی ثبت‌شده:

- MediatR با assembly مربوط به Application
- FluentValidation با assembly scanning
- Controllerها
- Swagger و Bearer authentication
- CORS مخصوص Angular localhost
- JWT Authentication
- Authorization و Permission policy
- Validation و Audit pipeline behavior
- Infrastructure services
- SignalR Hub
- Exception middleware

ترتیب pipeline مهم است:

```text
Exception Middleware
→ CORS
→ HTTPS Redirection
→ Authentication
→ Authorization
→ Controllers / SignalR
```

### 6.2 Controllerها

Controller مطلوب باید thin باشد:

```csharp
[ApiController]
[Route("api/examples")]
public sealed class ExamplesController : ControllerBase
{
    private readonly IMediator _mediator;

    [RequirePermission(PermissionKeys.Examples.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateExampleCommand command,
        CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }
}
```

Controller نباید:

- مستقیماً DbContext را استفاده کند.
- قانون کسب‌وکار داشته باشد.
- event موفقیت را قبل از نتیجه واقعی عملیات منتشر کند.
- Task طولانی یا حلقه background ایجاد کند.
- secret یا exception داخلی را به client برگرداند.

### 6.3 Authorization

`RequirePermission` در عمل یک `AuthorizeAttribute` با policy پویا است. برای feature جدید باید:

1. Permission key تعریف شود.
2. Permission در seed ثبت شود.
3. در صورت نیاز به Role پیش‌فرض تخصیص داده شود.
4. attribute روی تمام endpointهای مربوط قرار گیرد.

در وضعیت فعلی fallback policy سراسری وجود ندارد، پس فراموش‌کردن attribute باعث public شدن endpoint می‌شود. هنگام توسعه هر action باید authorization صریح بررسی شود.

### 6.4 ExceptionHandlingMiddleware

تبدیل فعلی:

- ValidationException → 400
- ArgumentException → 400
- DomainException → 400
- BusinessRuleException → 400
- DbUpdateException → 400
- سایر exceptionها → 500

ایراد مهم: در Production جزئیات stack trace برگردانده می‌شود. قبل از deployment باید فقط `errorId` و پیام عمومی به client داده شود و جزئیات صرفاً log شوند.

## 7. یک درخواست HTTP چگونه حرکت می‌کند؟

نمونه ایجاد Site:

```text
POST /api/sites
    │
    ├─ Authentication: اعتبار JWT
    ├─ Authorization: بررسی Sites.Create
    │
    ▼
SitesController.Create
    │ mediator.Send(command)
    ▼
MediatR Pipeline
    ├─ ValidationBehavior
    └─ AuditLoggingBehavior
    ▼
CreateSiteCommandHandler
    ├─ ساخت Site در Domain
    ├─ ISiteRepository.AddAsync
    └─ IUnitOfWork.SaveChangesAsync
    ▼
SQL Server
    │
    ▼
Guid response
```

Query لیست Site:

```text
GET /api/sites?pageNumber=1&pageSize=20
    ▼
GetSitesListQuery
    ▼
GetSitesListQueryHandler
    ├─ IQueryable<Site>
    ├─ Search filter
    ├─ Select SiteDto
    └─ Pagination + ToListAsync
    ▼
PagedResult<SiteDto>
```

## 8. چگونه یک Feature جدید اضافه کنیم؟

فرض: موجودیت جدید `Zone` زیر `Floor` نیاز است.

### مرحله 1: Domain

در `BMS.Domain/Entities/Location/Zone.cs`:

- از `BaseEntity<Guid>` ارث‌بری کند.
- propertyها private/protected setter داشته باشند.
- ruleها در constructor و methodهای domain اعمال شوند.
- رفتارهایی مانند Rename یا ChangeFloor به‌جای set مستقیم ایجاد شوند.

### مرحله 2: Application contract

ایجاد موارد زیر:

```text
BMS.Application/Location/Zones/
├─ Commands/
├─ Queries/
├─ Handlers/
├─ Dtos/
└─ Validators/
```

حداقل:

- `CreateZoneCommand`
- `UpdateZoneCommand`
- `DeleteZoneCommand`
- `GetZoneByIdQuery`
- `GetZonesListQuery`
- Handler متناظر هرکدام
- `ZoneDto`
- Validatorهای Create و Update
- `IZoneRepository`

### مرحله 3: Infrastructure persistence

- افزودن `DbSet<Zone>` به DbContext
- ایجاد `ZoneConfiguration`
- ایجاد `ZoneRepository`
- ثبت repository در `DependencyInjection`
- ساخت migration

### مرحله 4: Permission

تعریف کلیدها:

```text
Zones.View
Zones.Create
Zones.Update
Zones.Delete
```

سپس seed و RolePermission پیش‌فرض به‌روزرسانی شوند.

### مرحله 5: WebApi

ایجاد `ZonesController` با MediatR و permission مناسب برای هر action.

### مرحله 6: Test

حداقل موارد:

- Domain validation
- Handler create/update/delete
- Query filtering/pagination
- Authorization هر endpoint
- Repository mapping و foreign key

### مرحله 7: Build و بررسی

```powershell
dotnet restore BMS.sln
dotnet build BMS.sln --configuration Release --no-restore
dotnet test BMS.sln --configuration Release --no-build
```

در وضعیت فعلی test project وجود ندارد؛ برای توسعه جدید بهتر است test project نیز ایجاد شود.

## 9. چگونه Domain Event جدید اضافه کنیم؟

فرض: پس از ایجاد Device باید realtime یا integration handler اجرا شود.

### مرحله 1: تعریف Event در Domain

```csharp
public sealed record DeviceCreatedDomainEvent(
    Guid DeviceId,
    DateTime OccurredAtUtc) : IDomainEvent;
```

### مرحله 2: تولید Event

با الگوی فعلی پروژه، متد Domain event را برمی‌گرداند یا Handler پس از موفقیت آن را می‌سازد.

اصل مهم:

> Event موفقیت فقط بعد از موفقیت transaction یا نتیجه واقعی عملیات dispatch شود.

### مرحله 3: تعریف Handler

```csharp
public sealed class DeviceCreatedRealtimeHandler
    : IEventHandler<DeviceCreatedDomainEvent>
{
    public Task HandleAsync(DeviceCreatedDomainEvent domainEvent)
    {
        // side effect
        return Task.CompletedTask;
    }
}
```

### مرحله 4: ثبت DI

```csharp
services.AddScoped<
    IEventHandler<DeviceCreatedDomainEvent>,
    DeviceCreatedRealtimeHandler>();
```

### مرحله 5: Dispatch

```csharp
await _eventDispatcher.DispatchAsync(domainEvent);
```

### چه زمانی event فعلی کافی نیست؟

اگر عملیات باید در برابر crash، restart یا قطع شبکه پایدار باشد، EventDispatcher فعلی کافی نیست. در آن حالت:

- ابتدا تغییر و event در یک transaction ذخیره شوند.
- Outbox table ایجاد شود.
- background publisher پیام را به broker یا سرویس مقصد ارسال کند.
- idempotency برای consumer تعریف شود.

## 10. تفاوت Command، Domain Event و Integration Event

### Command

درخواست انجام یک کار است و ممکن است رد شود.

مثال: `CreateDeviceCommand` یا `UpdatePointCommand`.

### Domain Event

بیان می‌کند اتفاقی در دامنه رخ داده است.

مثال: `DataPointUpdatedDomainEvent`.

### Integration Event

پیام پایدار برای اطلاع‌دادن به process یا سرویس دیگر است.

پروژه در وضعیت فعلی Integration Event واقعی با broker/outbox ندارد. ارسال HTTP به Worker یا ارسال SignalR جای event bus پایدار را نمی‌گیرد.

## 11. Featureهای موجود خارج از Worker

### مدیریت هویت و دسترسی

- Login/Logout
- User CRUD
- تغییر رمز
- انتساب Role
- انتساب Permission مستقیم
- Person CRUD
- Permission-based authorization

### ساختار مکانی

- Site
- Building
- Floor
- Ward
- Room

### تجهیزات

- Controller CRUD
- Device CRUD
- Point CRUD
- اتصال Controller و Device
- Query بر اساس parent و location

### فرمان‌ها

- CommandDefinition CRUD
- ایجاد command برای Point قابل نوشتن
- صف داخلی command
- dispatch نتیجه command

### زمان‌بندی

- Create و Update DeviceSchedule
- SetActivation
- Query لیست scheduleها

### مشاهده‌پذیری

- Audit logs
- System logs API
- Exception logging
- Historian طراحی‌شده ولی فعلاً غیرفعال
- Realtime با SignalR

## 12. نقاط حساس هنگام توسعه

### Transaction و Event

اگر event قبل از `SaveChanges` dispatch شود، handler ممکن است داده‌ای را منتشر کند که transaction بعداً fail می‌شود. اگر بعد از SaveChanges dispatch شود، failure event handler داده اصلی را rollback نمی‌کند. برای side effect حیاتی، Outbox راهکار درست است.

### IQueryable

چند repository، `IQueryable<TEntity>` را مستقیماً expose می‌کنند. مزیت آن projection و pagination مناسب است، اما Application به رفتار EF Core نزدیک می‌شود. هنگام استفاده:

- query را materialize نکنید تا filter و pagination کامل شود.
- برای read-only از `AsNoTracking` استفاده کنید.
- navigationها را یا با projection بخوانید یا Include صریح داشته باشید.
- از queryهای N+1 جلوگیری کنید.

### Soft Delete

`BaseEntity` دارای `IsDeleted` است، ولی بعضی repositoryها `Remove` فیزیکی انجام می‌دهند و global query filter یکپارچه در بررسی دیده نشد. قبل از Delete feature جدید مشخص کنید soft delete یا hard delete مورد انتظار است.

### CancellationToken

Token را از Controller تا MediatR، Repository و EF Core عبور دهید. event interface فعلی CancellationToken ندارد؛ اگر handler طولانی است این محدودیت باید اصلاح شود.

### DateTime

Domain عمدتاً از UTC استفاده می‌کند. در دیتابیس و API نیز UTC نگه دارید و فقط در UI به timezone محلی تبدیل کنید.

### Nullability

Nullable reference types فعال است، اما Build هنوز هشدارهای nullability دارد. در کد جدید هشدار تازه اضافه نشود و از `!` فقط وقتی invariant واقعاً اثبات شده استفاده شود.

### Secrets

هیچ Connection String، JWT key، PLC credential یا API key در appsettings track‌شده قرار ندهید. از environment variable یا secret store استفاده کنید.

## 13. مشکلات فعلی که نباید به‌عنوان الگو کپی شوند

- Controllerهایی که authorization ندارند.
- `throw new Exception` برای خطاهای business/not-found.
- taskهای بدون `await`.
- ساخت `HttpClient` با `new HttpClient()` در هر عملیات.
- حلقه بی‌نهایت داخل Controller.
- dispatch کردن event موفقیت قبل از نتیجه واقعی.
- swallow کردن exception بدون log و بدون نتیجه failure.
- استفاده از stack trace در response محیط Production.
- `return` ابتدای HistorianBackgroundService.
- کلاس‌ها و usingهای تکراری یا بلااستفاده.
- استفاده هم‌زمان از نسخه‌های بسیار متفاوت packageها.

## 14. پیشنهاد برای تمیزتر کردن معماری

بدون بازنویسی بزرگ، ترتیب زیر کم‌ریسک است:

1. اضافه کردن fallback authorization policy.
2. یکسان‌سازی exceptionها و response contract.
3. اصلاح lifecycle command و نتیجه واقعی Worker.
4. حذف prototypeهای Controller و پروژه Realtime بلااستفاده.
5. یکسان‌سازی namespaceهای `Abstraction`, `Abstractions`, `Interfaces`.
6. حذف تعریف اشتباه دوم `IDomainEvent`.
7. افزودن DomainEvents collection یا پذیرش یک الگوی صریح واحد.
8. اضافه کردن Outbox برای eventهای حیاتی.
9. افزودن test project به ازای Domain/Application/WebApi integration.
10. اضافه کردن migration و مستند راه‌اندازی دیتابیس.

## 15. مسیر پیشنهادی مطالعه کد

برای شناخت مرحله‌ای پروژه، فایل‌ها را با این ترتیب بخوانید:

1. `BMS.sln` و تمام `*.csproj`
2. `WebApi/Program.cs`
3. `BMS.Infrastructure/DependencyInjection.cs`
4. `BMS.Domain/Entities/BaseEntity.cs`
5. Entityهای `Site` و `Device`
6. feature کامل `BMS.Application/Location/Sites`
7. feature کامل `BMS.Application/Devices`
8. `BMS.Infrastructure/Persistence/BMSDbContext.cs`
9. repositoryهای Site، Device و Point
10. `ValidationBehavior` و `AuditLoggingBehavior`
11. `AuthController` و `LoginCommandHandler`
12. `JwtProvider`، `PermissionResolver` و Authorization classes
13. `CommandsController` و `ExecuteCommandUseCase`
14. `GlobalDeviceCommandQueue` و `CommandProcessorBackgroundService`
15. `EventDispatcher` و event handlerهای Realtime/Historian
16. `ExceptionHandlingMiddleware`

با این ترتیب ابتدا الگوی استاندارد CRUD را یاد می‌گیرید و سپس وارد بخش‌های پیچیده command/realtime می‌شوید.

## 16. چک‌لیست Pull Request برای Feature جدید

- [ ] قانون کسب‌وکار داخل Domain یا Handler مناسب قرار دارد.
- [ ] Command و Query از هم جدا هستند.
- [ ] DTO به‌جای Entity از API برگردانده می‌شود.
- [ ] Validator وجود دارد.
- [ ] CancellationToken عبور داده شده است.
- [ ] Repository interface در Application و implementation در Infrastructure است.
- [ ] DI registration اضافه شده است.
- [ ] EF configuration و migration بررسی شده است.
- [ ] Permission key، seed و attribute کامل هستند.
- [ ] endpoint بدون authorization ناخواسته وجود ندارد.
- [ ] secret یا داده حساس log نشده است.
- [ ] Event فقط در زمان صحیح dispatch می‌شود.
- [ ] عملیات حیاتی در برابر retry و duplicate ایمن است.
- [ ] queryها pagination و projection مناسب دارند.
- [ ] Build warning جدید ایجاد نشده است.
- [ ] Unit/Integration test اضافه شده است.

## 17. محدودیت این سند

این سند از تحلیل سورس ساخته شده است. موارد زیر بدون محیط واقعی قطعی نیستند:

- schema و داده فعلی SQL Server
- قرارداد دقیق frontend
- سیاست deployment
- رفتار واقعی PermissionVersion
- رفتار کامل DeviceSchedule روی PLC
- نیازهای business که فقط شفاهی هستند
- ظرفیت و نرخ واقعی realtime/command

قبل از تغییرات بزرگ، این موارد باید با اجرای end-to-end یا توضیح مالک محصول تأیید شوند.

## 18. خلاصه نهایی

- معماری پایه، لایه‌ای و نزدیک به Clean Architecture است.
- CQRS با MediatR الگوی اصلی Application است، اما read/write store جدا نیست.
- Domain Event داخلی وجود دارد، اما Event Sourcing و event bus پایدار وجود ندارد.
- Domain مدل و قانون را نگه می‌دارد.
- Application عملیات را با Command/Query/Handler orchestration می‌کند.
- Infrastructure دیتابیس، امنیت، event dispatch، SignalR، queue و historian را پیاده‌سازی می‌کند.
- WebApi باید ورودی thin و composition root باقی بماند.
- برای توسعه جدید، featureهای `Location/Sites` الگوی ساده و مناسبی برای شروع هستند.
- بخش command/realtime پیچیده‌تر است و قبل از توسعه باید lifecycle و consistency آن مشخص شود.
