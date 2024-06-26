Web api ini dibuat dengan CQRS Pattern, dengan layering kedalam folder yang lebih sederhana.
Untuk tech stack yang digunakan adalah .NET 7 dengan EF Core 7.020 dan menggunakan database MS SQLServer. 
Menggunakan EFCore sebagai ORM nya ditambah dengan lazyloadingproxies. 
Berikut tahapan cara untuk menjalankan migrasi setelah clone repository : 
- Buat Database baru
- Buka project, kemudian jalankan pada nugget console "update-database"
