# ITG SmartHire

نظام ذكي لادارة التوظيف وتحليل طلبات المتقدمين بالذكاء الاصطناعي.

عند تقديم المتقدم على وظيفة ورفع الـ CV، النظام يحلل الـ CV تلقائي، يعطي درجة مطابقة من 100، يولد ملخص مهني، ويقترح أسئلة مقابلة مخصصة — مما يوفر على HR أكثر من 80% من وقت المراجعة اليدوية.

---

## الميزات

- تحليل CV بالذكاء الاصطناعي واستخراج المهارات والخبرات
- درجة مطابقة ذكية من 100 لكل متقدم
- فلترة تلقائية (الراتب، الخبرة)
- ملخص AI مهني لكل متقدم
- أسئلة مقابلة مخصصة مولدة بالـ AI
- توليد وصف وظيفي بالـ AI
- مقارنة متقدمين جنب بعض
- لوحة احصائيات
- لوحة تحكم HR مع فلاتر وترتيب

---

## التقنيات

- ASP.NET Core 8 MVC — Backend + Frontend
- MVC Views + Bootstrap 5 — واجهات المستخدم
- SQL Server + Entity Framework Core 8 — قاعدة البيانات
- ASP.NET Identity — تسجيل الدخول والصلاحيات
- Google Gemini API — الذكاء الاصطناعي
- Chart.js — الرسوم البيانية
- PdfPig — قراءة ملفات PDF
- AWS (EC2 + RDS + S3) — الاستضافة
- Docker — تجهيز بيئة التشغيل
- GitHub — ادارة الكود

---

## تشغيل المشروع

```
git clone https://github.com/[username]/ITG-SmartHire.git
cd ITG-SmartHire
cp .env.example .env
dotnet restore
dotnet ef database update --project SmartHire
dotnet run --project SmartHire
```

يفتح على http://localhost:5000

---

## الفريق

بشار مقدم — PMO + DevOps
عمر — Backend Developer
مازن — Full-Stack Developer
غزل — Backend + Documentation
سدين — AI Developer
فرح عاطف — Database + QA
راما — UI/UX Design

---

## التدريب — ITG Solutions 2026
