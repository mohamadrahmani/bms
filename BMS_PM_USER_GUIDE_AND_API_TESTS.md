# راهنمای ساده PM و تست APIها

> این فایل برای مالک محصول، تحلیل‌گر و توسعه‌دهنده‌ای نوشته شده که می‌خواهد بدون درگیرشدن با جزئیات معماری، قابلیت PM را بفهمد و APIهای آن را تست کند.  
> تاریخ تهیه: 2026-08-15  
> Branch: `codex/preventive-maintenance-backend`

## 1. PM دقیقاً چیست؟

PM مخفف Preventive Maintenance و به معنی «نگهداری پیشگیرانه» است.

به‌جای اینکه صبر کنیم یک دستگاه خراب شود، از قبل برای سرویس آن یک تاریخ تعیین می‌کنیم. برای مثال:

> فیلتر دستگاه هواساز AHU-01 باید تا ۱۰ روز دیگر تعویض شود و از ۳ روز مانده به موعد، سیستم هشدار بدهد.

در این مثال:

- Device: دستگاه `AHU-01`
- عنوان PM: تعویض فیلتر
- تاریخ سررسید: ۱۰ روز دیگر
- بازه هشدار: ۳ روز

سیستم با توجه به تاریخ، یکی از چهار وضعیت زیر را نشان می‌دهد:

| وضعیت | رنگ | معنی ساده |
|---|---|---|
| بدون PM | خاکستری | برای دستگاه PM فعالی تعریف نشده است. |
| عادی | سبز | هنوز تا زمان هشدار فاصله داریم. |
| نزدیک سررسید | زرد | وارد روزهای هشدار شده‌ایم. |
| سررسید گذشته | قرمز | زمان انجام سرویس رسیده یا گذشته است. |

## 2. چرخه کامل یک PM

چرخه کار به شکل زیر است:

```text
انتخاب Device
    ↓
ساخت PM
    ↓
نمایش سبز، زرد یا قرمز بر اساس تاریخ
    ↓
ثبت فایل‌های قبل از سرویس، در صورت نیاز
    ↓
انجام یا لغو سرویس
    ↓
بسته‌شدن PM فعال و ایجاد History
    ↓
ثبت گزارش یا فایل نهایی در History
    ↓
امکان تعریف PM جدید برای همان Device
```

چند قانون مهم:

- هر Device در یک لحظه فقط می‌تواند یک PM فعال داشته باشد.
- بعضی Deviceها ممکن است اصلاً PM نداشته باشند.
- PM به‌صورت خودکار تکرار نمی‌شود.
- بعد از نهایی‌شدن، اطلاعات قبلی حذف نمی‌شوند و در History باقی می‌مانند.
- این قابلیت به Worker و Runtime دستگاه وابسته نیست.
- وضعیت رنگی هنگام مشاهده اطلاعات محاسبه می‌شود.

## 3. چه چیزهایی پیاده‌سازی شده است؟

- ایجاد PM برای Device
- مشاهده PM فعال
- ویرایش عنوان، توضیح، سررسید و روزهای هشدار
- جلوگیری از ایجاد دو PM فعال برای یک Device
- نمایش وضعیت خاکستری، سبز، زرد و قرمز
- ثبت نتیجه `انجام شد` یا `لغو شد`
- نگهداری History
- آپلود فایل برای PM فعال
- آپلود فایل برای History
- دانلود فایل
- کنترل دسترسی کاربران
- جلوگیری از ویرایش هم‌زمان با `RowVersion`

مواردی مانند تکرار خودکار PM، ارسال پیامک و حذف فایل فعلاً پیاده‌سازی نشده‌اند.

## 4. پیش‌نیاز تست

برای تست به موارد زیر نیاز دارید:

1. SQL Server پروژه در دسترس باشد.
2. Connection String پروژه درست باشد.
3. اسکریپت جداول PM روی دیتابیس تست اجرا شده باشد.
4. یک کاربر معتبر داشته باشید.
5. کاربر Permissionهای مناسب داشته باشد.
6. حداقل یک Device داخل دیتابیس وجود داشته باشد.
7. WebApi در حال اجرا باشد.

Permissionهای موردنیاز:

- برای مشاهده: `PM.View`
- برای ایجاد، ویرایش، نهایی‌سازی و فایل: `PM.Manage`
- برای گرفتن لیست Deviceها: `Devices.View`

اگر از SuperAdmin استفاده می‌کنید، طبق Seed فعلی پروژه باید Permissionهای PM را داشته باشد.

## 5. آماده‌سازی دیتابیس

اسکریپت زیر جداول PM را ایجاد می‌کند:

```text
BMS.Infrastructure/Persistence/Scripts/20260814_AddPreventiveMaintenance.sql
```

روش پیشنهادی:

1. SQL Server Management Studio را باز کنید.
2. به دیتابیس محیط Test متصل شوید.
3. قبل از اجرا مطمئن شوید دیتابیس درست انتخاب شده است.
4. فایل SQL بالا را باز کنید.
5. اسکریپت را اجرا کنید.
6. وجود جدول‌های زیر را کنترل کنید:

```text
PmSchedules
PmServiceHistories
PmAttachments
```

این اسکریپت را ابتدا روی دیتابیس Test اجرا کنید، نه مستقیماً روی Production.

## 6. اجرای WebApi

در PowerShell وارد مسیر پروژه شوید:

```powershell
cd E:\GitHub\mohamadrahmani\bms
```

سپس:

```powershell
dotnet run --project WebApi\WebApi.csproj --launch-profile http
```

طبق تنظیم فعلی پروژه، آدرس معمول API این است:

```text
http://localhost:5127
```

و Swagger:

```text
http://localhost:5127/swagger
```

اگر ترمینال آدرس دیگری با عبارت `Now listening on` نمایش داد، همان آدرس ترمینال را مبنا قرار دهید.

## 7. روش اول تست: Swagger

### 7.1 ورود به سیستم

در Swagger، endpoint زیر را باز کنید:

```text
POST /api/auth/login
```

Body:

```json
{
  "userName": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD"
}
```

پاسخ موفق باید Status برابر `200` و تقریباً چنین شکلی داشته باشد:

```json
{
  "userId": "...",
  "fullName": "...",
  "userName": "...",
  "token": "eyJ...",
  "expiresAt": "...",
  "permissions": [
    "PM.View",
    "PM.Manage"
  ]
}
```

مقدار `token` را کپی کنید.

### 7.2 قراردادن Token در Swagger

1. بالای صفحه Swagger روی `Authorize` کلیک کنید.
2. Token را وارد کنید.
3. معمولاً در Swagger با Security Type از نوع Bearer، فقط خود Token یعنی `eyJ...` را وارد کنید.
4. Swagger باید Header زیر را بسازد:

```text
Authorization: Bearer eyJ...
```

اگر در Request عبارت `Bearer Bearer` دیدید، یعنی کلمه Bearer را دوبار وارد کرده‌اید؛ یکی را حذف کنید.

### 7.3 پیدا کردن DeviceId

Endpoint زیر را اجرا کنید:

```text
GET /api/devices?pageNumber=1&pageSize=10
```

از اولین Device مناسب، مقدار `id` را کپی کنید. این مقدار در ادامه جایگزین `{deviceId}` می‌شود.

اگر `403 Forbidden` دریافت کردید، کاربر Permission به نام `Devices.View` ندارد.

## 8. روش دوم تست: Postman

یک Environment در Postman بسازید و متغیرهای زیر را تعریف کنید:

| Variable | مقدار اولیه |
|---|---|
| `baseUrl` | `http://localhost:5127` |
| `token` | خالی |
| `deviceId` | خالی |
| `pmScheduleId` | خالی |
| `rowVersion` | خالی |
| `historyId` | خالی |
| `attachmentId` | خالی |

برای تمام Requestهای محافظت‌شده:

1. وارد تب Authorization شوید.
2. Type را `Bearer Token` انتخاب کنید.
3. مقدار Token را `{{token}}` بگذارید.

## 9. سناریوی کامل تست API

در این بخش APIها را به ترتیبی تست می‌کنیم که یک چرخه واقعی PM را پوشش دهد.

### تست 1: Login

```http
POST {{baseUrl}}/api/auth/login
Content-Type: application/json
```

```json
{
  "userName": "YOUR_USERNAME",
  "password": "YOUR_PASSWORD"
}
```

نتیجه مورد انتظار:

- Status: `200 OK`
- پاسخ دارای `token`

برای ذخیره خودکار Token در Postman، در تب Tests بنویسید:

```javascript
const body = pm.response.json();
pm.environment.set("token", body.token);
```

هرگز نام کاربری، رمز یا Token واقعی را داخل Git Commit نکنید.

### تست 2: گرفتن DeviceId

```http
GET {{baseUrl}}/api/devices?pageNumber=1&pageSize=10
Authorization: Bearer {{token}}
```

نتیجه مورد انتظار:

- Status: `200 OK`
- وجود حداقل یک رکورد در `items`

ذخیره خودکار اولین Device:

```javascript
const body = pm.response.json();
if (body.items && body.items.length > 0) {
    pm.environment.set("deviceId", body.items[0].id);
}
```

اگر Device ندارید، ابتدا از جریان فعلی پروژه یک Device بسازید یا شناسه یک Device موجود را از دیتابیس بردارید.

### تست 3: مشاهده وضعیت اولیه

```http
GET {{baseUrl}}/api/devices/{{deviceId}}/pm/active
Authorization: Bearer {{token}}
```

اگر هنوز PM نساخته‌اید، نتیجه مورد انتظار:

```json
{
  "hasActivePm": false,
  "deviceId": "...",
  "indicator": 0,
  "attachments": []
}
```

`indicator: 0` یعنی وضعیت خاکستری.

### تست 4: ساخت PM سبز

ابتدا یک تاریخ UTC حدود ۱۰ روز آینده بسازید. در PowerShell:

```powershell
(Get-Date).ToUniversalTime().AddDays(10).ToString("yyyy-MM-ddTHH:mm:ssZ")
```

مقدار خروجی را در `dueDateUtc` قرار دهید.

```http
POST {{baseUrl}}/api/devices/{{deviceId}}/pm
Authorization: Bearer {{token}}
Content-Type: application/json
```

```json
{
  "title": "تعویض فیلتر دستگاه",
  "description": "فیلتر تعویض و تسمه فن بازدید شود",
  "dueDateUtc": "PUT_UTC_DATE_10_DAYS_LATER_HERE",
  "warningDays": 3
}
```

نتیجه مورد انتظار:

- Status: `201 Created`
- پاسخ دارای `id`

ذخیره خودکار شناسه:

```javascript
const body = pm.response.json();
pm.environment.set("pmScheduleId", body.id);
```

### تست 5: کنترل وضعیت سبز و RowVersion

دوباره اجرا کنید:

```http
GET {{baseUrl}}/api/devices/{{deviceId}}/pm/active
```

نتیجه مورد انتظار:

- `hasActivePm = true`
- `indicator = 1`
- `daysUntilDue` تقریباً ۱۰
- وجود مقدار `rowVersion`

در Postman، مقادیر را ذخیره کنید:

```javascript
const body = pm.response.json();
pm.environment.set("pmScheduleId", body.pmScheduleId);
pm.environment.set("rowVersion", body.rowVersion);
```

`rowVersion` را تغییر ندهید. این مقدار Base64 است و برای جلوگیری از ویرایش هم‌زمان استفاده می‌شود.

### تست 6: کنترل جلوگیری از PM تکراری

همان Request ساخت PM را دوباره اجرا کنید.

نتیجه مورد انتظار:

- Status: `409 Conflict`
- پیام: Device از قبل PM فعال دارد.

این نتیجه صحیح است و نشان می‌دهد قانون «حداکثر یک PM فعال» کار می‌کند.

### تست 7: تبدیل وضعیت به زرد

یک تاریخ حدود ۲ روز آینده بسازید:

```powershell
(Get-Date).ToUniversalTime().AddDays(2).ToString("yyyy-MM-ddTHH:mm:ssZ")
```

سپس:

```http
PUT {{baseUrl}}/api/pm/{{pmScheduleId}}
Authorization: Bearer {{token}}
Content-Type: application/json
```

```json
{
  "title": "تعویض فیلتر دستگاه",
  "description": "آزمایش وضعیت زرد",
  "dueDateUtc": "PUT_UTC_DATE_2_DAYS_LATER_HERE",
  "warningDays": 3,
  "rowVersion": "{{rowVersion}}"
}
```

نتیجه مورد انتظار:

- Status: `204 No Content`

حالا دوباره GET مربوط به PM فعال را اجرا کنید. نتیجه باید `indicator = 2` یعنی زرد باشد.

بعد از هر Update حتماً `rowVersion` جدید را از GET دریافت و ذخیره کنید.

### تست 8: تبدیل وضعیت به قرمز

یک تاریخ حدود یک روز قبل بسازید:

```powershell
(Get-Date).ToUniversalTime().AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ")
```

همان PUT قبلی را با تاریخ گذشته و آخرین `rowVersion` اجرا کنید. سپس GET بگیرید.

نتیجه مورد انتظار:

- `indicator = 3`
- `daysUntilDue` صفر یا منفی

### تست 9: آزمایش RowVersion قدیمی

یک مقدار قدیمی `rowVersion` را عمداً در PUT قرار دهید.

نتیجه مورد انتظار:

- Status: `409 Conflict`

برای ادامه، PM را دوباره GET کنید و آخرین RowVersion را بردارید.

### تست 10: افزودن فایل به PM فعال

```http
POST {{baseUrl}}/api/pm/{{pmScheduleId}}/attachments
Authorization: Bearer {{token}}
Content-Type: multipart/form-data
```

در Postman، Body را روی `form-data` قرار دهید:

| Key | Type | Value |
|---|---|---|
| `file` | File | یک فایل مجاز انتخاب کنید |
| `description` | Text | گزارش یا تصویر قبل از سرویس |

پسوندهای پیش‌فرض مجاز:

```text
pdf, png, jpg, jpeg, doc, docx, xls, xlsx
```

حداکثر اندازه پیش‌فرض: `10 MB`

نتیجه مورد انتظار:

- Status: `201 Created`
- پاسخ دارای `id`

ذخیره شناسه Attachment:

```javascript
const body = pm.response.json();
pm.environment.set("attachmentId", body.id);
```

### تست 11: دانلود فایل

```http
GET {{baseUrl}}/api/pm-attachments/{{attachmentId}}/download
Authorization: Bearer {{token}}
```

نتیجه مورد انتظار:

- Status: `200 OK`
- دریافت همان فایل با نام و Content-Type ثبت‌شده

در Postman می‌توانید از گزینه `Send and Download` استفاده کنید.

### تست 12: نهایی‌سازی PM

ابتدا آخرین PM فعال را GET کنید و `rowVersion` جدید را ذخیره کنید.

زمان فعلی UTC را بسازید:

```powershell
(Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
```

سپس:

```http
POST {{baseUrl}}/api/pm/{{pmScheduleId}}/finalize
Authorization: Bearer {{token}}
Content-Type: application/json
```

```json
{
  "status": 1,
  "actionDateUtc": "PUT_CURRENT_UTC_DATE_HERE",
  "description": "فیلتر تعویض و سرویس با موفقیت انجام شد",
  "performedByUserId": null,
  "rowVersion": "{{rowVersion}}"
}
```

`status`:

- `1`: انجام شد
- `2`: لغو شد

وقتی `performedByUserId` برابر null باشد، سیستم کاربر Login‌شده را ثبت می‌کند.

نتیجه مورد انتظار:

- Status: `200 OK`
- پاسخ دارای `historyId`

ذخیره خودکار:

```javascript
const body = pm.response.json();
pm.environment.set("historyId", body.historyId);
```

### تست 13: کنترل بسته‌شدن PM

دوباره اجرا کنید:

```http
GET {{baseUrl}}/api/devices/{{deviceId}}/pm/active
```

نتیجه مورد انتظار:

- `hasActivePm = false`
- `indicator = 0`

### تست 14: دریافت History

```http
GET {{baseUrl}}/api/devices/{{deviceId}}/pm/history?pageNumber=1&pageSize=10
Authorization: Bearer {{token}}
```

نتیجه مورد انتظار:

- Status: `200 OK`
- `totalCount` حداقل ۱
- وجود رکورد نهایی‌شده در `items`
- `status = 1` برای Completed
- وجود `titleSnapshot` و `dueDateUtcSnapshot`

Snapshot یعنی اطلاعات PM در زمان بسته‌شدن حفظ شده‌اند.

### تست 15: افزودن فایل به History

```http
POST {{baseUrl}}/api/pm-history/{{historyId}}/attachments
Authorization: Bearer {{token}}
Content-Type: multipart/form-data
```

Body از نوع form-data:

| Key | Type | Value |
|---|---|---|
| `file` | File | گزارش نهایی سرویس |
| `description` | Text | گزارش بعد از انجام سرویس |

نتیجه مورد انتظار:

- Status: `201 Created`

بعد از آن History را دوباره GET کنید؛ metadata فایل باید داخل `attachments` دیده شود.

### تست 16: جلوگیری از فایل جدید روی PM بسته‌شده

دوباره تلاش کنید با Route مربوط به Schedule فایل اضافه کنید:

```text
POST /api/pm/{{pmScheduleId}}/attachments
```

نتیجه مورد انتظار:

- Status: `409 Conflict`

بعد از بسته‌شدن، فایل جدید باید به History اضافه شود.

### تست 17: ساخت PM جدید برای همان Device

حالا که PM قبلی بسته شده، دوباره Request ساخت PM را اجرا کنید.

نتیجه مورد انتظار:

- Status: `201 Created`

این نشان می‌دهد محدودیت فقط برای PM فعال است و History مانع ساخت PM جدید نمی‌شود.

## 10. خلاصه Status Codeهای مورد انتظار

| Status | معنی در تست PM |
|---:|---|
| `200` | دریافت اطلاعات، Login، Finalize یا Download موفق |
| `201` | ساخت PM یا Attachment موفق |
| `204` | Update موفق |
| `400` | Body، تاریخ، فایل یا ورودی نامعتبر |
| `401` | Token وجود ندارد یا معتبر نیست |
| `403` | کاربر Permission لازم را ندارد |
| `404` | Device، PM، History یا Attachment پیدا نشده است |
| `409` | PM تکراری، PM بسته‌شده یا RowVersion قدیمی |

## 11. خطاهای رایج

### `401 Unauthorized`

- Token ارسال نشده است.
- Token منقضی شده است.
- Header باید به شکل `Authorization: Bearer TOKEN` باشد.

### `403 Forbidden`

- کاربر `PM.View` یا `PM.Manage` ندارد.
- برای لیست Deviceها ممکن است `Devices.View` لازم باشد.

### `404 Device was not found`

- `deviceId` اشتباه است.
- Device حذف نرم شده یا در دیتابیس فعلی وجود ندارد.

### `409 The device already has an active PM schedule`

- برای Device از قبل PM فعال ساخته شده است.
- ابتدا PM فعال را مشاهده، نهایی یا در صورت نیاز ویرایش کنید.

### `409 ... changed by another request`

- `rowVersion` قدیمی است.
- دوباره GET بگیرید و مقدار جدید را ارسال کنید.

### خطای آپلود فایل

- فایل بیشتر از ۱۰ مگابایت است.
- پسوند مجاز نیست.
- Content-Type با لیست تنظیمات تطابق ندارد.
- فایل خالی است.

### جدول PM پیدا نمی‌شود

- اسکریپت SQL اجرا نشده است.
- اسکریپت روی دیتابیس دیگری اجرا شده است.
- Connection String برنامه به دیتابیس موردنظر اشاره نمی‌کند.

## 12. چک‌لیست تأیید نهایی

- [ ] Login موفق است.
- [ ] کاربر `PM.View` و `PM.Manage` دارد.
- [ ] Device معتبر انتخاب شده است.
- [ ] حالت بدون PM خاکستری است.
- [ ] PM جدید ساخته می‌شود.
- [ ] PM دوم با `409` رد می‌شود.
- [ ] حالت سبز دیده می‌شود.
- [ ] با تغییر تاریخ، حالت زرد دیده می‌شود.
- [ ] با تاریخ گذشته، حالت قرمز دیده می‌شود.
- [ ] RowVersion قدیمی با `409` رد می‌شود.
- [ ] فایل مجاز آپلود و دانلود می‌شود.
- [ ] Finalize باعث ایجاد History می‌شود.
- [ ] بعد از Finalize، Active PM خاکستری می‌شود.
- [ ] فایل History قابل ثبت است.
- [ ] روی PM بسته‌شده فایل جدید ثبت نمی‌شود.
- [ ] برای همان Device می‌توان PM فعال جدید ساخت.

## 13. کدام فایل را چه زمانی بخوانیم؟

اگر فقط می‌خواهید قابلیت را بفهمید و تست کنید:

```text
BMS_PM_USER_GUIDE_AND_API_TESTS.md
```

اگر جزئیات فنی پیاده‌سازی و توسعه Backend را می‌خواهید:

```text
BMS_PREVENTIVE_MAINTENANCE_BACKEND.md
```

اگر معماری کل پروژه را می‌خواهید:

```text
BMS_ARCHITECTURE_DEVELOPER_GUIDE.md
BMS_PROJECT_KNOWLEDGE.md
```

## 14. نتیجه‌ای که باید از این سند بگیرید

در ساده‌ترین بیان، این قابلیت برای هر Device یک «یادآور سرویس قابل پیگیری» ایجاد می‌کند. یادآور بر اساس تاریخ رنگ می‌گیرد، هنگام انجام یا لغو به History می‌رود و فایل‌های قبل و بعد از سرویس را نگه می‌دارد.

برای تست کامل، کافی است سناریوی بخش 9 را به‌ترتیب انجام دهید. اگر تمام نتایج مورد انتظار دریافت شدند، Backend PM از نظر جریان اصلی آماده اتصال به Angular است.
