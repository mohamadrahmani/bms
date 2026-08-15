# راهنمای فنی بک‌اند Preventive Maintenance

> نام ماژول: Preventive Maintenance یا PM  
> پروژه: BMS  
> تاریخ پیاده‌سازی و بررسی: 2026-08-14  
> Branch اولیه پیاده‌سازی: `codex/preventive-maintenance-backend`  
> Commit اولیه پیاده‌سازی: `039a357`  
> محدوده: فقط Backend؛ پروژه Angular در این تغییر حضور ندارد.

## 1. هدف سند

این سند تمام تصمیم‌ها و تغییرات انجام‌شده برای اضافه‌شدن نگهداری پیشگیرانه تجهیزات به بک‌اند BMS را ثبت می‌کند. با خواندن این فایل، یک توسعه‌دهنده یا دستیار هوش مصنوعی دیگر باید بتواند:

- منطق کسب‌وکار PM را بفهمد؛
- محل هر بخش از پیاده‌سازی را پیدا کند؛
- قرارداد API موردنیاز Angular را بشناسد؛
- دیتابیس را آماده کند؛
- بدون درگیرکردن Worker، ماژول را توسعه دهد؛
- محدودیت‌ها و تصمیم‌های فاز اول را بداند.

## 2. خلاصه قابلیت

PM مستقیماً به `Device` متعلق است. ممکن است یک Device هیچ PM فعالی نداشته باشد، اما در هر لحظه حداکثر یک PM فعال برای هر Device مجاز است.

هر PM دارای عنوان، توضیح، تاریخ سررسید و تعداد روزهای هشدار است. وضعیت نمایشی PM هنگام Query محاسبه می‌شود و در دیتابیس ذخیره نمی‌شود.

پس از انجام یا لغو سرویس:

1. PM فعال بسته می‌شود؛
2. یک رکورد تاریخچه ساخته می‌شود؛
3. اطلاعات اصلی PM به‌صورت Snapshot داخل تاریخچه ثبت می‌شود؛
4. رکوردهای قبلی حذف نمی‌شوند؛
5. بعداً می‌توان PM فعال جدیدی برای همان Device ایجاد کرد.

فایل می‌تواند به PM فعال یا رکورد تاریخچه متعلق باشد. محتوای فایل در SQL Server و در ستون `varbinary(max)` ذخیره می‌شود.

## 3. تصمیم‌های معماری

- پیاده‌سازی از CQRS و MediatR موجود پروژه استفاده می‌کند.
- برای Read و Write دیتابیس جداگانه وجود ندارد؛ هر دو از همان `BMSDbContext` استفاده می‌کنند.
- این ماژول Event Sourced نیست.
- برای PM، Domain Event جدیدی اضافه نشده است.
- وضعیت رنگی در Query و بر اساس زمان فعلی محاسبه می‌شود.
- PM فقط Date-based است و به Runtime تجهیزات وابسته نیست.
- ایجاد خودکار PM بعدی یا Interval-based scheduling در فاز اول وجود ندارد.
- هیچ تغییری در Worker، Modbus، Telemetry، Historian، Realtime یا `DeviceSchedule` انجام نشده است.
- حذف PM، تاریخچه یا Attachment در API فاز اول وجود ندارد.
- برای تغییر یا نهایی‌سازی PM از optimistic concurrency و `RowVersion` استفاده می‌شود.
- شناسه‌های `Guid` و زمان‌های Audit توسط برنامه و مدل دامنه تولید می‌شوند.
- به‌جای EF Migration، یک DDL صریح اضافه شده است؛ زیرا repository در زمان پیاده‌سازی baseline استاندارد Migration نداشت.

## 4. وضعیت‌ها و رنگ‌ها

Enum مربوط به وضعیت نمایش:

| مقدار | نام | رنگ UI | قانون |
|---:|---|---|---|
| `0` | `None` | خاکستری | Device فاقد PM فعال است یا PM بسته شده است. |
| `1` | `Normal` | سبز | زمان فعلی قبل از شروع بازه هشدار است. |
| `2` | `Warning` | زرد | زمان فعلی وارد بازه هشدار شده ولی هنوز به سررسید نرسیده است. |
| `3` | `Overdue` | قرمز | زمان فعلی برابر یا بعد از تاریخ سررسید است. |

فرمول بازه هشدار:

```text
warningStartUtc = dueDateUtc - warningDays
```

قواعد مرزی:

```text
nowUtc < warningStartUtc              => Normal
warningStartUtc <= nowUtc < dueDateUtc => Warning
nowUtc >= dueDateUtc                   => Overdue
```

Enum نتیجه نهایی سرویس:

| مقدار | نام | مفهوم |
|---:|---|---|
| `1` | `Completed` | سرویس انجام شده است. |
| `2` | `Cancelled` | سرویس لغو یا بدون انجام بسته شده است. |

در قرارداد فعلی JSON، Enumها به‌صورت عددی ارسال و دریافت می‌شوند؛ مگر اینکه در آینده `JsonStringEnumConverter` به تنظیمات عمومی API اضافه شود.

## 5. مدل دامنه

### `PmSchedule`

Aggregate اصلی PM است و شامل موارد زیر است:

- `DeviceId`
- `Title`
- `Description`
- `DueDateUtc`
- `WarningDays`
- `IsActive`
- اطلاعات ایجاد، ویرایش و بسته‌شدن
- `RowVersion`
- یک `ServiceHistory` اختیاری
- مجموعه Attachmentهای متعلق به Schedule

رفتارهای اصلی:

- `Update`: فقط PM فعال را تغییر می‌دهد.
- `Finalize`: PM را غیرفعال و رکورد `PmServiceHistory` را تولید می‌کند.
- `CalculateIndicator`: وضعیت رنگی را بدون ذخیره در دیتابیس محاسبه می‌کند.

### `PmServiceHistory`

نتیجه نهایی یک Schedule است. هر Schedule حداکثر یک History دارد.

علاوه بر نتیجه عملیات، Snapshot زیر را نگهداری می‌کند:

- `DueDateUtcSnapshot`
- `TitleSnapshot`
- `BaseDescriptionSnapshot`

این Snapshot باعث می‌شود تغییرات احتمالی آینده در مدل PM، معنای تاریخچه قبلی را از بین نبرد.

### `PmAttachment`

هر Attachment باید دقیقاً یک Owner داشته باشد:

- یا `PmScheduleId`
- یا `PmServiceHistoryId`

وجود هم‌زمان هر دو یا خالی‌بودن هر دو در Domain و Database ممنوع است.

## 6. ساختار دیتابیس

اسکریپت آماده‌سازی دیتابیس:

```text
BMS.Infrastructure/Persistence/Scripts/20260814_AddPreventiveMaintenance.sql
```

این اسکریپت سه جدول می‌سازد:

### `PmSchedules`

نکات مهم:

- ارتباط Restrict با `Devices`
- ارتباط Restrict با کاربران ایجادکننده، ویرایش‌کننده و بسته‌کننده
- `RowVersion` از نوع SQL Server `rowversion`
- Check Constraint برای منفی‌نبودن `WarningDays`
- Unique Filtered Index روی PM فعال Device:

```sql
WHERE [IsActive] = 1 AND [IsDeleted] = 0
```

این Index آخرین خط دفاعی در برابر ایجاد هم‌زمان دو PM فعال برای یک Device است.

### `PmServiceHistories`

نکات مهم:

- ارتباط یک‌به‌یک با `PmSchedules`
- Unique Index روی `PmScheduleId`
- Check Constraint برای Statusهای `1` و `2`
- نگهداری Snapshot اطلاعات Schedule
- ارتباط Restrict با کاربران اجراکننده و ثبت‌کننده

### `PmAttachments`

نکات مهم:

- محتوای فایل: `varbinary(max)`
- اندازه فایل: `bigint`
- Check Constraint مالکیت انحصاری
- ارتباط Restrict با Schedule، History و User

تمام جدول‌ها ستون‌های پایه `BaseEntity` شامل `Id`، `IsDeleted`، `CreatedAtUtc` و `UpdatedAtUtc` را دارند.

## 7. جریان CQRS

### Commandها

- `CreatePmScheduleCommand`
- `UpdatePmScheduleCommand`
- `FinalizePmScheduleCommand`
- `AddPmScheduleAttachmentCommand`
- `AddPmHistoryAttachmentCommand`

### Queryها

- `GetActivePmByDeviceQuery`
- `GetPmHistoryByDeviceQuery`
- `DownloadPmAttachmentQuery`

### جریان ایجاد PM

```text
Controller
  -> CreatePmScheduleCommand
  -> ValidationBehavior
  -> CreatePmScheduleCommandHandler
  -> بررسی وجود Device
  -> بررسی نبود PM فعال
  -> ساخت PmSchedule
  -> SaveChanges
  -> کنترل Unique Index در Database
```

### جریان نهایی‌سازی

```text
Controller
  -> FinalizePmScheduleCommand
  -> بررسی IsActive و RowVersion
  -> PmSchedule.Finalize(...)
  -> غیرفعال‌شدن Schedule
  -> ایجاد PmServiceHistory
  -> SaveChanges اتمیک
```

## 8. APIهای اضافه‌شده

تمام Routeها زیر prefix `api` قرار دارند.

| Method | Route | Permission | کاربرد |
|---|---|---|---|
| `GET` | `/api/devices/{deviceId}/pm/active` | `PM.View` | دریافت PM فعال و Indicator |
| `POST` | `/api/devices/{deviceId}/pm` | `PM.Manage` | ایجاد PM فعال |
| `PUT` | `/api/pm/{pmScheduleId}` | `PM.Manage` | ویرایش PM فعال |
| `POST` | `/api/pm/{pmScheduleId}/finalize` | `PM.Manage` | انجام یا لغو PM و ایجاد History |
| `GET` | `/api/devices/{deviceId}/pm/history` | `PM.View` | دریافت تاریخچه صفحه‌بندی‌شده |
| `POST` | `/api/pm/{pmScheduleId}/attachments` | `PM.Manage` | افزودن فایل به PM فعال |
| `POST` | `/api/pm-history/{historyId}/attachments` | `PM.Manage` | افزودن فایل به History |
| `GET` | `/api/pm-attachments/{attachmentId}/download` | `PM.View` | دانلود فایل |

### ایجاد PM

```http
POST /api/devices/{deviceId}/pm
Content-Type: application/json
```

```json
{
  "title": "Quarterly filter replacement",
  "description": "Replace filters and inspect fan belt",
  "dueDateUtc": "2026-09-01T08:00:00Z",
  "warningDays": 7
}
```

`deviceId` از Route گرفته می‌شود و مقدار احتمالی آن در Body نادیده گرفته و جایگزین می‌شود.

پاسخ موفق: `201 Created`

```json
{
  "id": "00000000-0000-0000-0000-000000000000"
}
```

### دریافت PM فعال

```http
GET /api/devices/{deviceId}/pm/active
```

نمونه پاسخ دارای PM فعال:

```json
{
  "hasActivePm": true,
  "pmScheduleId": "00000000-0000-0000-0000-000000000000",
  "deviceId": "00000000-0000-0000-0000-000000000000",
  "title": "Quarterly filter replacement",
  "description": "Replace filters and inspect fan belt",
  "dueDateUtc": "2026-09-01T08:00:00Z",
  "warningDays": 7,
  "indicator": 1,
  "daysUntilDue": 10,
  "rowVersion": "AAAAAAAAB9E=",
  "attachments": []
}
```

اگر Device وجود داشته ولی PM فعال نداشته باشد:

```json
{
  "hasActivePm": false,
  "deviceId": "00000000-0000-0000-0000-000000000000",
  "indicator": 0,
  "attachments": []
}
```

### ویرایش PM

مقدار `rowVersion` باید دقیقاً از آخرین پاسخ GET گرفته شود. ASP.NET Core آرایه بایت را در JSON به‌صورت Base64 نمایش می‌دهد.

```http
PUT /api/pm/{pmScheduleId}
Content-Type: application/json
```

```json
{
  "title": "Updated title",
  "description": "Updated description",
  "dueDateUtc": "2026-09-05T08:00:00Z",
  "warningDays": 5,
  "rowVersion": "AAAAAAAAB9E="
}
```

پاسخ موفق: `204 No Content`

### نهایی‌سازی PM

```http
POST /api/pm/{pmScheduleId}/finalize
Content-Type: application/json
```

```json
{
  "status": 1,
  "actionDateUtc": "2026-08-30T10:30:00Z",
  "description": "Service completed successfully",
  "performedByUserId": null,
  "rowVersion": "AAAAAAAAB9E="
}
```

اگر `performedByUserId` ارسال نشود، شناسه کاربر احراز هویت‌شده استفاده می‌شود.

پاسخ موفق:

```json
{
  "historyId": "00000000-0000-0000-0000-000000000000"
}
```

### تاریخچه

```http
GET /api/devices/{deviceId}/pm/history?pageNumber=1&pageSize=10
```

پاسخ از قرارداد عمومی `PagedResult<T>` استفاده می‌کند و شامل موارد زیر است:

- `items`
- `totalCount`
- `pageNumber`
- `pageSize`
- `totalPages`
- `hasNext`
- `hasPrevious`

هر آیتم History شامل Snapshot و لیست metadata فایل‌ها است. محتوای باینری فایل در Query تاریخچه بارگذاری نمی‌شود.

### آپلود فایل

```http
POST /api/pm/{pmScheduleId}/attachments
Content-Type: multipart/form-data
```

یا:

```http
POST /api/pm-history/{historyId}/attachments
Content-Type: multipart/form-data
```

Form fields:

- `file`: اجباری
- `description`: اختیاری

آپلود فایل به Schedule فقط تا زمانی مجاز است که Schedule فعال باشد. بعد از بسته‌شدن، فایل‌های جدید باید به History اضافه شوند.

## 9. سیاست فایل

تنظیمات در `WebApi/appsettings.json` و Section زیر قرار دارد:

```json
{
  "PmFiles": {
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [
      ".pdf", ".png", ".jpg", ".jpeg",
      ".doc", ".docx", ".xls", ".xlsx"
    ]
  }
}
```

حداکثر اندازه پیش‌فرض: `10 MB`

اعتبارسنجی روی موارد زیر انجام می‌شود:

- نام فایل
- خالی‌نبودن فایل
- اندازه فایل
- Extension
- Content-Type

نام فایل با `Path.GetFileName` پاک‌سازی می‌شود. در فاز اول محتوای واقعی فایل با Magic Number یا Antivirus اسکن نمی‌شود و Content-Type اعلام‌شده توسط Client کنترل می‌شود.

## 10. احراز هویت و دسترسی

مجوز جدیدی Seed نشده، زیرا پروژه از قبل این دو Permission را داشت:

- `PermissionKeys.PM.View`
- `PermissionKeys.PM.Manage`

شناسه کاربر جاری توسط `CurrentUserService` از Claimهای زیر خوانده می‌شود:

1. `ClaimTypes.NameIdentifier`
2. `sub` به‌عنوان fallback

عملیات Create، Update، Finalize و Upload علاوه بر Permission به UserId معتبر نیاز دارند.

## 11. Concurrency و Conflict

`PmSchedules.RowVersion` یک concurrency token است.

Client باید در Update و Finalize مقدار آخرین `rowVersion` را بازگرداند. در موارد زیر API پاسخ `409 Conflict` می‌دهد:

- ایجاد PM دوم برای Device دارای PM فعال
- ویرایش PM بسته‌شده
- نهایی‌سازی PM بسته‌شده
- ارسال RowVersion قدیمی
- تغییر هم‌زمان رکورد توسط Request دیگر

این کنترل در دو سطح انجام می‌شود:

- بررسی داخل Handler
- Constraint/RowVersion دیتابیس

## 12. نگاشت خطاهای HTTP

موارد اضافه‌شده به `ExceptionHandlingMiddleware`:

| Exception | HTTP Status |
|---|---:|
| `NotFoundException` | `404` |
| `ConflictException` | `409` |
| `ValidationException` | `400` |
| `BusinessRuleException` | `400` |

## 13. فایل‌های اصلی پیاده‌سازی

### Domain

```text
BMS.Domain/Entities/PreventiveMaintenance/PmSchedule.cs
BMS.Domain/Entities/PreventiveMaintenance/PmServiceHistory.cs
BMS.Domain/Entities/PreventiveMaintenance/PmAttachment.cs
BMS.Domain/Enums/PmIndicatorStatus.cs
BMS.Domain/Enums/PmFinalStatus.cs
```

### Application

```text
BMS.Application/PreventiveMaintenance/Commands/
BMS.Application/PreventiveMaintenance/Queries/
BMS.Application/PreventiveMaintenance/Handlers/
BMS.Application/PreventiveMaintenance/Dtos/
BMS.Application/PreventiveMaintenance/Validators/
BMS.Application/Common/Interfaces/IPmRepository.cs
BMS.Application/Common/Interfaces/IPmFilePolicy.cs
BMS.Application/Common/Interfaces/ICurrentUserService.cs
BMS.Application/Common/Settings/PmFileOptions.cs
BMS.Application/Common/Exceptions/ConflictException.cs
```

### Infrastructure

```text
BMS.Infrastructure/Repositories/PmRepository.cs
BMS.Infrastructure/Files/ConfiguredPmFilePolicy.cs
BMS.Infrastructure/Security/CurrentUserService.cs
BMS.Infrastructure/Persistence/Configurations/PmScheduleConfiguration.cs
BMS.Infrastructure/Persistence/Configurations/PmServiceHistoryConfiguration.cs
BMS.Infrastructure/Persistence/Configurations/PmAttachmentConfiguration.cs
BMS.Infrastructure/Persistence/Scripts/20260814_AddPreventiveMaintenance.sql
```

### WebApi

```text
WebApi/Controllers/PreventiveMaintenanceController.cs
WebApi/Middlewares/ExceptionHandlingMiddleware.cs
WebApi/Program.cs
WebApi/appsettings.json
```

فایل‌های اتصال به سیستم:

```text
BMS.Infrastructure/Persistence/BMSDbContext.cs
BMS.Infrastructure/DependencyInjection.cs
```

## 14. راه‌اندازی

### مرحله 1: دیتابیس

قبل از استفاده از Endpointها، اسکریپت زیر را روی دیتابیس محیط مقصد اجرا کنید:

```text
BMS.Infrastructure/Persistence/Scripts/20260814_AddPreventiveMaintenance.sql
```

این اسکریپت در زمان پیاده‌سازی روی دیتابیس واقعی اجرا نشده است. قبل از Production ابتدا در محیط Test/Staging اجرا و Backup دیتابیس بررسی شود.

### مرحله 2: تنظیم فایل

مقادیر `PmFiles` در `appsettings.json` یا تنظیمات محیط Deployment بررسی شوند.

### مرحله 3: Permission

نقش‌های موردنظر باید Permissionهای `PM.View` و `PM.Manage` را داشته باشند. SuperAdmin بر اساس Seed موجود تمام Permissionها را دریافت می‌کند.

### مرحله 4: Build

```powershell
dotnet restore BMS.sln
dotnet build BMS.sln --configuration Release
```

### مرحله 5: بررسی API

ترتیب پیشنهادی تست دستی:

1. ایجاد PM برای یک Device معتبر؛
2. دریافت PM فعال و نگهداری `rowVersion`؛
3. تلاش برای ایجاد PM دوم و انتظار `409`؛
4. ویرایش PM با RowVersion معتبر؛
5. آپلود فایل Schedule؛
6. نهایی‌سازی و دریافت `historyId`؛
7. دریافت تاریخچه؛
8. افزودن فایل به History؛
9. دانلود فایل؛
10. ایجاد PM فعال جدید برای همان Device.

## 15. بررسی‌های انجام‌شده

در زمان پیاده‌سازی موارد زیر بررسی شدند:

- Build کامل Solution با صفر خطای کامپایل
- صحت ساخته‌شدن مدل EF بدون اتصال یا تغییر دیتابیس
- مرز Normal و Warning
- مرز Warning و Overdue
- وضعیت None پس از Finalize
- ساخت History هنگام Finalize
- ارتباط History با Schedule
- مالکیت انحصاری Attachment
- صحت JSON تنظیمات `PmFiles`
- `git diff --check`

در Solution فعلی Test Project رسمی وجود نداشت؛ بنابراین تست‌های این مرحله به‌صورت Build و smoke test اجرا شدند و Test Project دائمی اضافه نشده است.

## 16. نکات لازم برای Angular

- Indicator عددی را طبق جدول بخش 4 به رنگ تبدیل کنید.
- `rowVersion` را بدون تغییر از GET به PUT یا Finalize برگردانید.
- تمام تاریخ‌ها را UTC و با پسوند `Z` ارسال کنید.
- در پاسخ `409` اطلاعات PM را دوباره GET کنید.
- دانلود Attachment از Endpoint اختصاصی انجام شود؛ لیست‌ها فقط metadata دارند.
- برای آپلود از `multipart/form-data` با field نام `file` استفاده کنید.
- اگر `hasActivePm=false` بود، حالت خاکستری و امکان ساخت PM نمایش داده شود.
- بعد از Finalize، Active PM دوباره Query شود و تاریخچه Refresh شود.

## 17. محدودیت‌ها و توسعه‌های آینده

موارد زیر عمداً در فاز اول پیاده‌سازی نشده‌اند:

- حذف Attachment
- ویرایش History
- حذف PM یا History
- تولید خودکار PM بعدی
- PM مبتنی بر Runtime یا Counter
- اعلان Email/SMS/Push برای نزدیک‌شدن سررسید
- Job زمان‌بندی‌شده برای اعلان‌ها
- Antivirus یا Magic Number validation فایل
- ذخیره فایل در Object Storage
- Audit مستقل برای دانلود فایل
- Unit Test و Integration Test دائمی

پیشنهاد برای فاز بعد:

1. اضافه‌کردن Test Project برای Domain/Application/API؛
2. اضافه‌کردن Notification Job بدون وابستگی به Worker تجهیزات؛
3. بررسی Object Storage در صورت افزایش حجم فایل‌ها؛
4. اضافه‌کردن endpoint حذف نرم Attachment با Audit؛
5. اضافه‌کردن فیلتر تاریخ و Status به History؛
6. تبدیل DDL فعلی به EF Migration پس از ایجاد migration baseline پروژه.

## 18. دستور تحویل به یک اکانت AI دیگر

```text
ابتدا BMS_PROJECT_KNOWLEDGE.md و BMS_ARCHITECTURE_DEVELOPER_GUIDE.md را کامل بخوان.
برای ماژول نگهداری پیشگیرانه، BMS_PREVENTIVE_MAINTENANCE_BACKEND.md را مرجع اصلی قرار بده.
قبل از تغییر، وضعیت Git و Branch را بررسی کن.
PM یک feature مبتنی بر CQRS است ولی Event Sourced نیست.
این feature نباید بدون نیاز صریح به Worker، Modbus، Telemetry یا DeviceSchedule متصل شود.
قانون حداکثر یک PM فعال برای هر Device و کنترل RowVersion باید حفظ شود.
قبل از تغییر قرارداد API، هماهنگی آن با Angular را بررسی کن.
اسکریپت دیتابیس را بدون مشخص‌شدن محیط مقصد و Backup اجرا نکن.
```

## 19. جمع‌بندی

ماژول PM به‌صورت یک Vertical Feature در معماری فعلی پروژه پیاده‌سازی شده است. Domain قوانین اصلی را نگه می‌دارد، Application جریان CQRS را مدیریت می‌کند، Infrastructure نگاشت EF و سیاست فایل را فراهم می‌کند و WebApi فقط ورودی/خروجی HTTP و Permission را مدیریت می‌کند.

این پیاده‌سازی مستقل از Worker است و برای شروع توسعه Angular یا گسترش قابلیت‌های PM در Backend، مبنای مشخص و قابل‌ردیابی فراهم می‌کند.
