# PhoneStore — أشرف الخوالدة

متجر إلكتروني لقطع غيار الموبايلات مبني على ASP.NET Core 8 MVC مع SQL Server.

## المتطلبات

- .NET SDK 8.0+
- SQL Server (محلي أو سحابي)
- Docker (اختياري — للنشر)

## التشغيل المحلي

```bash
# 1. ضبط connection string
# عبر متغير بيئة:
export PHONESTORE_DB_CONNECTION="Server=...;Database=...;User Id=...;Password=...;Encrypt=False;TrustServerCertificate=True;"

# أو في PhoneStore/appsettings.Development.json:
# {
#   "ConnectionStrings": { "DefaultConnection": "..." }
# }

# 2. تشغيل المشروع
cd PhoneStore
dotnet restore
dotnet run
```

الموقع يفتح على http://localhost:5000

## النشر

### Render (موصى به — مجاني)

1. ادفع المشروع لـ GitHub
2. على [render.com](https://render.com): **New** → **Web Service** → اربط الريبو
3. سيكتشف تلقائياً ملف `render.yaml`
4. أضف **Environment Variable**:
   - `PHONESTORE_DB_CONNECTION` = connection string قاعدة البيانات
5. اضغط **Deploy**

### Railway

1. على [railway.app](https://railway.app): **New Project** → **Deploy from GitHub**
2. سيستخدم الـ `Dockerfile` تلقائياً
3. أضف متغير `PHONESTORE_DB_CONNECTION`

### Azure App Service

```bash
az webapp up --name phonestore --runtime "DOTNET:8.0" --sku F1
az webapp config appsettings set --name phonestore --settings PHONESTORE_DB_CONNECTION="..."
```

### Docker (يدوي)

```bash
docker build -t phonestore .
docker run -p 8080:8080 -e PHONESTORE_DB_CONNECTION="..." phonestore
```

## متغيرات البيئة

| المتغير | الوصف | مطلوب |
|---------|--------|-------|
| `PHONESTORE_DB_CONNECTION` | SQL Server connection string | ✅ |
| `ASPNETCORE_ENVIRONMENT` | `Production` أو `Development` | ⚪ |
| `PORT` | البورت (يضبطه PaaS تلقائياً) | ⚪ |

## بنية المشروع

```
PhoneStore/
├── Controllers/      # MVC controllers
├── Data/             # EF Core DbContext
├── Migrations/       # EF Core migrations
├── Models/           # Domain models + ViewModels
├── Views/            # Razor views
├── ViewComponents/
├── wwwroot/          # Static assets (CSS/JS/images)
└── Program.cs        # نقطة التشغيل
```

## ⚠️ ملاحظة بخصوص Vercel

مشروع ASP.NET Core MVC **لا يعمل على Vercel** (المخصص لـ Node.js/serverless). استخدم Render أو Railway أو Azure بدلاً منه.

## الترخيص

خاص — جميع الحقوق محفوظة.
