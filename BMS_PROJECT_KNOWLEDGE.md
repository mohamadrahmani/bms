# BMS Project Knowledge & AI Handoff

> مرجع فنی پروژه برای توسعه‌دهندگان و دستیارهای هوش مصنوعی  
> تاریخ بررسی: 2026-08-14  
> مسیر پروژه بررسی‌شده: `E:\GitHub\mohamadrahmani\bms`  
> شاخه: `develop`  
> commit هنگام بررسی: `c66c910`  
> وضعیت Build: موفق (`Release`، صفر خطا و ۵۲ هشدار)

## 1. هدف این سند

این فایل برای انتقال سریع دانش پروژه ساخته شده است. اگر پروژه را به یک توسعه‌دهنده یا حساب هوش مصنوعی دیگر می‌دهید، این فایل را نیز همراه آن ارائه کنید.

این سند شامل موارد زیر است:

- معماری و مسئولیت هر پروژه
- مدل دامنه و ارتباط مفاهیم اصلی
- جریان‌های ورود، دسترسی، telemetry، command، Modbus، realtime و historian
- فایل‌های ورودی و نقاط مهم کد
- وضعیت فعلی Build و Git
- ایرادهای امنیتی و منطقی شناخته‌شده
- بخش‌های تأییدنشده که برای شناخت کامل نیاز به اجرای عملی دارند

این سند حاوی مقدار Connection String، رمز دیتابیس یا کلید JWT نیست.

## 2. معرفی سامانه

BMS یک سامانه مدیریت ساختمان و تجهیزات است. سیستم ساختار مکانی ساختمان، کنترلرها، دستگاه‌ها، نقاط ورودی/خروجی، فرمان‌ها، زمان‌بندی‌ها، کاربران و مجوزها را مدیریت می‌کند.

اجزای اجرایی اصلی:

1. `WebApi`: API مدیریتی، احراز هویت، مجوزها، SignalR و صف فرمان‌ها.
2. `BMS.Worker`: ارتباط با PLCها، polling از طریق Modbus، ارسال telemetry و دریافت فرمان نوشتن روی Point.
3. SQL Server: ذخیره داده‌های دامنه، کاربران، مجوزها، تنظیمات تجهیزات، لاگ‌ها و تاریخچه نقاط.

## 3. ساختار Solution

Solution شامل شش پروژه با Target Framework برابر `net8.0` است:

### `BMS.Domain`

لایه دامنه و مستقل از زیرساخت است.

شامل:

- Entityهای امنیتی: `User`, `Person`, `Role`, `Permission`, `UserRole`, `RolePermission`, `UserPermission`
- ساختار مکانی: `Site`, `Building`, `Floor`, `Ward`, `Room`
- مدل BMS: `Controller`, `Device`, `Point`, `CommandDefinition`, `DeviceCommand`, `DeviceSchedule`
- Domain Eventها مانند:
  - `DataPointUpdatedDomainEvent`
  - `DeviceCommandCompletedDomainEvent`
  - `GetDeviceStateDomainEvent`
  - `AlarmRaisedDomainEvent`
- Exceptionهای دامنه

### `BMS.Application`

لایه use case و orchestration است و به `BMS.Domain` وابسته است.

الگوهای اصلی:

- CQRS با MediatR
- Command، Query، Handler و DTO برای موجودیت‌ها
- FluentValidation
- Repository interfaceها و abstractionهای زیرساخت
- Pipeline behaviorها برای validation و audit logging
- Use caseهای فرمان و به‌روزرسانی وضعیت دستگاه

Featureهای اصلی:

- Auth و Login/Logout
- Users و Persons
- Sites, Buildings, Floors, Wards, Rooms
- Controllers, Devices, Points
- CommandDefinitions
- DeviceSchedules

### `BMS.Infrastructure`

پیاده‌سازی زیرساخت و persistence است و به Application و Domain وابسته است.

مسئولیت‌ها:

- `BMSDbContext` و EF Core SQL Server
- Repositoryها و UnitOfWork
- JWT، BCrypt و Permission resolution
- Modbus connection/client/value parsing/batching
- صف سراسری فرمان و background command processor
- SignalR Hub و event handlerهای realtime
- Historian مبتنی بر Channel
- Audit logger
- Seed مربوط به Role و Permission

### `WebApi`

ورودی HTTP سامانه است و به Application و Infrastructure وابسته است.

مسئولیت‌ها:

- Controllerهای REST
- Swagger
- JWT Authentication
- Permission-based Authorization
- Exception middleware
- SignalR endpoint
- state داخلی Digital Twin
- اجرای Web API و background serviceهای ثبت‌شده در Infrastructure

نقطه ورود: `WebApi/Program.cs`

### `BMS.Worker`

سرویس ارتباط با تجهیزات است. با وجود نام Worker، با `WebApplication.CreateBuilder` اجرا شده و علاوه بر Hosted Service، endpoint HTTP نیز دارد.

مسئولیت‌ها:

- دریافت پیکربندی PLC از دیتابیس
- polling دوره‌ای Modbus
- نگهداری state موقت PLC
- ارسال telemetry به backend/UI sink
- endpoint نوشتن مقدار Point روی PLC
- endpoint invalidate کردن cache تنظیمات PLC

نقطه ورود: `BMS.Worker/Program.cs`

Hosted Serviceها:

- `ModbusPollingWorker`
- `TelemetryForwarderWorker`

### `Realtime`

در وضعیت فعلی تقریباً خالی است و فقط یک class پیش‌فرض دارد. منطق واقعی realtime در `BMS.Infrastructure` و `WebApi` قرار گرفته است. احتمالاً این پروژه باقی‌مانده یک طراحی قدیمی یا توسعه نیمه‌کاره است.

## 4. وابستگی لایه‌ها

```text
BMS.Domain
    ↑
BMS.Application
    ↑
BMS.Infrastructure
    ↑             ↑
WebApi        BMS.Worker

Realtime (فعلاً مستقل و تقریباً خالی)
```

جهت کلی وابستگی شبیه Clean Architecture است، هرچند در بعضی فایل‌ها کد آزمایشی، abstractionهای تکراری و coupling اضافی دیده می‌شود.

## 5. مدل مفهومی دامنه

### ساختار مکانی

```text
Site
 └─ Building
     └─ Floor
         └─ Ward
             └─ Room
```

### ساختار تجهیزات

```text
Controller / PLC
 └─ Device
     ├─ Point
     ├─ CommandDefinition
     └─ DeviceSchedule
```

- `Controller`: مشخصات PLC یا کنترلر، از جمله نام، IP، Port و وضعیت فعال بودن.
- `Device`: تجهیز منطقی متصل به کنترلر.
- `Point`: رجیستر یا نقطه داده؛ شامل Address، RegisterType، DataType، Length، Scale و قابلیت نوشتن.
- `CommandDefinition`: تعریف فرمان و ارتباط آن با Point قابل نوشتن.
- `DeviceSchedule`: تنظیم زمان‌بندی فعال/غیرفعال شدن دستگاه یا فرمان‌های زمان‌بندی‌شده.

### امنیت

```text
User ── UserRole ── Role ── RolePermission ── Permission
  └──── UserPermission (override مستقیم allow/deny)
```

مجوز نهایی کاربر از اجتماع Permissionهای Roleها ساخته می‌شود و سپس `UserPermission`ها روی آن override می‌شوند.

## 6. جریان‌های اصلی برنامه

### 6.1 ورود و احراز هویت

1. درخواست Login به `AuthController` ارسال می‌شود.
2. `LoginCommandHandler` کاربر را با username نرمال‌شده می‌خواند.
3. وضعیت فعال/قفل بودن کاربر بررسی می‌شود.
4. رمز با BCrypt اعتبارسنجی می‌شود.
5. نتیجه ورود روی User و دیتابیس ثبت می‌شود.
6. Permissionها توسط `PermissionResolver` استخراج می‌شوند.
7. `JwtProvider` توکن شامل claimهای کاربر و `perm`ها را می‌سازد.
8. Controllerهای محافظت‌شده با `RequirePermissionAttribute` مجوز لازم را درخواست می‌کنند.
9. `PermissionHandler` وجود claim مناسب را بررسی می‌کند.

نکته: Permissionها داخل JWT قرار دارند؛ تغییر Permission کاربر تا زمان صدور توکن جدید یا انقضای توکن ممکن است فوراً منعکس نشود، مگر سازوکار `PermissionVersion` به‌طور کامل در validation استفاده شود.

### 6.2 عملیات CRUD

جریان معمول:

```text
HTTP Controller
 → MediatR Command/Query
 → Handler
 → Repository
 → EF Core DbContext / UnitOfWork
 → SQL Server
```

Validation از طریق FluentValidation و `ValidationBehavior` اجرا می‌شود. Audit برای commandهای علامت‌گذاری‌شده از طریق `AuditLoggingBehavior` انجام می‌شود.

### 6.3 دریافت Telemetry

جریان مورد انتظار:

```text
PLC
 → BMS.Worker / ModbusPollingWorker
 → TelemetryMessage Channel
 → TelemetryForwarderWorker
 → WebApi ControllerDevice/update
 → InMemoryDeviceStateStore
 → UpdateDataPointUseCase
 → Domain Event
 → SignalR / Historian handlers
```

Web API وضعیت زنده دستگاه را در `InMemoryDeviceStateStore` نگهداری می‌کند. این state با restart برنامه از بین می‌رود و میان چند instance توزیع نمی‌شود.

### 6.4 اجرای فرمان

جریان فعلی Web API:

```text
POST /api/commands
 → CommandsController
 → ExecuteCommandUseCase
 → پیدا کردن Point قابل نوشتن
 → GlobalDeviceCommandQueue
 → CommandProcessorBackgroundService
 → SimulatedDeviceGateway
 → POST به Command API در Worker
 → IPlcCommandDispatcher
 → Modbus write
```

نکته مهم: Controller در وضعیت فعلی بلافاصله بعد از enqueue، رویداد موفقیت منتشر می‌کند؛ این موفقیت اجرای واقعی PLC را تضمین نمی‌کند.

### 6.5 Polling و Modbus

1. `PlcClientProvider` تنظیمات Controller/Device/Point فعال را از دیتابیس می‌گیرد و cache می‌کند.
2. `ModbusPollingWorker` PLCها را polling می‌کند.
3. `ModbusBatchBuilder` Pointهای نزدیک را برای کاهش تعداد read گروه‌بندی می‌کند.
4. `ModbusConnectionManager` اتصال و retry/circuit behavior را مدیریت می‌کند.
5. `ModbusValueParser` مقادیر registerها را به مقدار مهندسی تبدیل می‌کند.
6. نتیجه به Channel مربوط به telemetry نوشته می‌شود.
7. `TelemetryForwarderWorker` پیام‌ها را به مقصد تنظیم‌شده ارسال می‌کند.

### 6.6 Realtime

- SignalR با `AddSignalR` ثبت می‌شود.
- Hub اصلی فعلی در مسیر `/hubs/deviceState` map شده است.
- Client می‌تواند بر اساس `deviceId` عضو Group شود.
- Domain Eventهای تغییر Point یا تکمیل Command توسط handlerهای realtime منتشر می‌شوند.

Hub فعلی `[Authorize]` ندارد و عضویت در Group نیز مالکیت یا دسترسی کاربر به Device را بررسی نمی‌کند.

### 6.7 Historian

طراحی مورد انتظار:

```text
DataPointUpdatedDomainEvent
 → DataPointUpdatedHistorianHandler
 → ChannelHistorianWriter
 → Channel<DataPointDeltaModel>
 → HistorianBackgroundService
 → batch insert into DataPointHistory
```

وضعیت واقعی: `HistorianBackgroundService.ExecuteAsync` در ابتدای متد `return` می‌کند؛ بنابراین هیچ داده‌ای از Channel خوانده یا در دیتابیس ذخیره نمی‌شود.

### 6.8 Device Schedule

برای Device Schedule، Command/Query/Handler/Repository و Controller وجود دارد. Create و Update سعی می‌کنند Pointهای مربوط به زمان‌بندی را پیدا کرده و مقادیر زمان/فعال‌سازی را تنظیم کنند.

وضعیت فعلی نیازمند بازبینی است:

- مجوز endpointهای Update، SetActivation و Get کامل نیست.
- endpoint `SetActivation` از `UpdateDeviceScheduleCommand` استفاده می‌کند، در حالی که `SetDeviceActivationScheduleCommand` جداگانه وجود دارد.
- queryهای ById و ByDeviceId در بعضی فایل‌ها internal/نیمه‌کاره به نظر می‌رسند.
- اجرای end-to-end زمان‌بندی روی PLC هنوز در این بررسی آزمایش نشده است.

## 7. Persistence و داده

`BMSDbContext` شامل DbSetهای زیر است:

- Users, Persons, Roles, Permissions
- UserRoles, RolePermissions, UserPermissions
- Controllers, Devices, Points, CommandDefinitions
- Sites, Buildings, Floors, Wards, Rooms
- Logs
- DeviceSchedules
- DataPointHistory

Configurationهای EF Core با `ApplyConfigurationsFromAssembly` اعمال می‌شوند. Permission، Role و RolePermission seed می‌شوند.

در repository بررسی‌شده هیچ پوشه Migration وجود نداشت. بنابراین ایجاد یا ارتقای schema دیتابیس از روی سورس فعلی قابل بازتولید نیست، مگر migrationها جای دیگری نگهداری شوند یا دیتابیس از قبل آماده باشد.

## 8. تنظیمات اجرایی

فایل‌های اصلی:

- `WebApi/appsettings.json`
- `WebApi/appsettings.Development.json`
- `BMS.Worker/appsettings.json`
- `BMS.Worker/appsettings.Development.json`

کلیدهای مهم:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:ExpiryMinutes`
- `commandApi:url`
- `UiSink:Url`
- `CommandApi:Url`
- `CacheInvalidateApi:Url`
- `Plcs`

هشدار امنیتی: فایل‌های فعلی دارای Connection String شامل username/password و کلید JWT هستند و در Git track شده‌اند. مقدارها نباید در مستندات یا پیام‌های انتقال دانش کپی شوند.

## 9. Endpointها و سطح دسترسی

بیشتر Controllerهای مدیریتی از `RequirePermission` استفاده می‌کنند، اما موارد زیر فاقد حفاظت کامل‌اند:

- `CommandsController`: اجرای فرمان دستگاه
- `CommandDefinitionController`: تمام endpointها
- `ControllerDeviceController`: ورود telemetry و endpoint آزمایشی AutoUpdate
- `DeviceSchedulesController`: Update، SetActivation و Get
- Worker command API: نوشتن روی Point
- Worker cache invalidation API
- SignalR Hub

راهکار مطلوب:

- تعریف fallback policy سراسری که همه endpointها را authenticated کند.
- استفاده صریح از `[AllowAnonymous]` فقط برای Login و health checkهای مورد نیاز.
- Permission مجزا برای read/write command و telemetry ingestion.
- حفاظت ارتباط داخلی WebApi و Worker با API key چرخش‌پذیر، mTLS یا شبکه داخلی محدود.
- محدود کردن CORS به originهای مشخص.

## 10. ایرادهای شناخته‌شده و اولویت‌ها

### بحرانی

1. Secretهای واقعی در appsettingsهای track‌شده قرار دارند.
2. endpointهای فرمان، telemetry، CommandDefinition و Worker بدون authorization کافی‌اند.
3. Exception middleware در Production، stack trace و inner exception را به client برمی‌گرداند.

### زیاد

1. Historian به دلیل `return` ابتدای Worker کاملاً خاموش است.
2. `SimulatedDeviceGateway` exception را می‌بلعد و حتی در خطای HTTP نتیجه موفق برمی‌گرداند.
3. `CommandsController` قبل از نتیجه واقعی PLC، command را موفق اعلام می‌کند.
4. `ControllerDeviceController.AutoUpdate` یک حلقه بی‌نهایت fire-and-forget ایجاد می‌کند و از `Thread.Sleep` در async استفاده می‌کند.
5. چند فراخوانی `IAuditLogger.Add` در Login بدون `await` هستند.
6. در `ModbusPlcClient.WriteAsync`، داخل شرط `point == null` از `point.Code` استفاده می‌شود.
7. state زنده فقط in-memory است و برای deployment چند instance مناسب نیست.

### متوسط

1. ۵۲ هشدار Build شامل nullability، task بدون await، کد unreachable و using تکراری.
2. نسخه packageها نامتوازن‌اند؛ packageهای ASP.NET Core 1.x، Hosting 6، .NET 8 و Extensions.Http 10 کنار هم هستند.
3. پروژه تست وجود ندارد.
4. Migration، README، Docker و CI/CD وجود ندارند.
5. `Realtime` تقریباً خالی است.
6. فایل صفر بایتی `git` و فایل `project.bundle` در repository track شده‌اند.
7. کدهای کامنت‌شده و prototypeها در مسیر production زیادند.
8. چند نوع/namespace مشابه یا تکراری برای Event، DataType، realtime و state وجود دارد.

## 11. نتیجه Build بررسی‌شده

دستور استفاده‌شده:

```powershell
dotnet restore E:\GitHub\mohamadrahmani\bms\BMS.sln
dotnet build E:\GitHub\mohamadrahmani\bms\BMS.sln --configuration Release --no-restore
```

نتیجه:

```text
Build succeeded.
52 Warning(s)
0 Error(s)
```

SDK استفاده‌شده `9.0.308` بود و runtime مربوط به .NET 8 روی سیستم نصب بود. پروژه `global.json` ندارد.

پس از restore/build، Git همچنان clean بود؛ خروجی‌های `bin` و `obj` ignore شده‌اند.

## 12. تست و قابلیت اطمینان

در Solution هیچ test project پیدا نشد. حداقل تست‌های موردنیاز:

1. Unit test برای Domain entityها و قوانین Device/Point/User.
2. Unit test برای PermissionResolver و PermissionHandler.
3. Handler test برای CRUD و DeviceSchedule.
4. Integration test برای EF Core repositories.
5. Integration test برای Login/JWT/Authorization.
6. Contract test بین WebApi و Worker.
7. تست ModbusValueParser و ModbusBatchBuilder برای register typeها، endian و scale.
8. تست command lifecycle شامل queued/succeeded/failed/timeout.
9. تست Historian برای batch، cancellation، retry و shutdown flush.
10. تست SignalR برای isolation گروه‌ها و authorization.

## 13. ترتیب پیشنهادی اصلاحات

### فاز 1: امنیت فوری

1. rotate کردن رمزهای دیتابیس و کلید JWT.
2. خارج کردن secretها از Git و بررسی تاریخچه repository.
3. افزودن authorization سراسری و امن‌سازی Worker/SignalR.
4. حذف stack trace از response محیط Production.

### فاز 2: صحت جریان فرمان و telemetry

1. تعریف lifecycle روشن برای command: queued, processing, succeeded, failed, timeout.
2. انتقال انتشار `DeviceCommandCompletedDomainEvent` به محل دریافت نتیجه واقعی.
3. اصلاح `SimulatedDeviceGateway` و استفاده از `IHttpClientFactory`.
4. حذف یا محدود کردن endpoint `AutoUpdate`.
5. اعتبارسنجی ورودی‌های telemetry و write-point.

### فاز 3: Historian و پایداری

1. فعال‌سازی Historian.
2. تبدیل Channel به bounded channel یا تعریف backpressure روشن.
3. retry و error handling برای batch insert.
4. flush کردن buffer هنگام shutdown.
5. تصمیم‌گیری برای state مشترک در deployment چند instance.

### فاز 4: کیفیت و نگهداری

1. افزودن test projectها.
2. رساندن Build warningها به صفر یا baseline کنترل‌شده.
3. هم‌تراز کردن نسخه packageها با .NET 8.
4. افزودن migrationها و دستور راه‌اندازی دیتابیس.
5. افزودن README، health checks، Docker و CI.
6. حذف پروژه‌ها، فایل‌ها و prototypeهای بلااستفاده.

## 14. مواردی که هنوز تأیید نشده‌اند

شناخت کامل عملیاتی نیازمند موارد زیر است:

- مشاهده schema و داده واقعی SQL Server
- مشخصات register map واقعی PLCها
- endian، scaling و نوع داده مورد انتظار هر برند PLC
- اجرای هم‌زمان WebApi و Worker و مشاهده telemetry واقعی
- قرارداد frontend با REST API و SignalR
- سیاست deployment و network topology
- تعداد PLC/Device/Point و نرخ telemetry در محیط واقعی
- قوانین کسب‌وکاری DeviceSchedule
- سیاست نگهداری DataPointHistory و Logs
- رفتار مورد انتظار هنگام قطع PLC، restart سرویس و پر شدن صف‌ها

هیچ دستیار یا توسعه‌دهنده‌ای نباید درباره این موارد بدون داده عملی فرض قطعی بسازد.

## 15. راهنمای شروع برای یک حساب AI دیگر

متن پیشنهادی هنگام انتقال پروژه:

```text
ابتدا فایل BMS_PROJECT_KNOWLEDGE.md را کامل بخوان.
سپس وضعیت فعلی branch و commit را با اطلاعات سند مقایسه کن.
قبل از هر تغییر، AGENTS.md و تغییرات محلی Git را بررسی کن.
هیچ مقدار secret را چاپ یا در commit ثبت نکن.
برای تغییرات امنیتی، command processing، Modbus و historian ابتدا مسیر end-to-end مربوطه را دوباره trace کن.
یافته‌های این سند snapshot تاریخ 2026-08-14 هستند و ممکن است کد جدیدتر شده باشد.
```

فایل‌های پیشنهادی برای مطالعه اولیه حساب جدید:

1. `BMS.sln`
2. همه فایل‌های `*.csproj`
3. `WebApi/Program.cs`
4. `BMS.Worker/Program.cs`
5. `BMS.Infrastructure/DependencyInjection.cs`
6. `BMS.Infrastructure/Persistence/BMSDbContext.cs`
7. `BMS.Application/UseCases/ExecuteCommandUseCase.cs`
8. `BMS.Infrastructure/Commanding/CommandProcessorBackgroundService.cs`
9. `BMS.Worker/Workers/ModbusPollingWorker.cs`
10. `BMS.Worker/Workers/TelemetryForwarderWorker.cs`
11. `BMS.Infrastructure/Modbus/*`
12. `BMS.Infrastructure/Historian/*`
13. `BMS.Infrastructure/Security/*`
14. `WebApi/Controllers/*`

## 16. قانون نگهداری این سند

این فایل باید همراه تغییرات معماری به‌روزرسانی شود. مواردی که الزاماً باید در آن ثبت شوند:

- تغییر جریان command یا telemetry
- اضافه شدن سرویس اجرایی جدید
- تغییر schema یا migration مهم
- تغییر مدل authorization
- تغییر پروتکل PLC یا register mapping
- تغییر endpointهای داخلی WebApi/Worker
- رفع هرکدام از ایرادهای بحرانی این سند
- تغییر دستور Build، Run یا Deployment

هنگام به‌روزرسانی، تاریخ، branch و commit بالای سند نیز اصلاح شود.
