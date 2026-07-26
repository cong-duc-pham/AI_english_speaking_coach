# AI English Speaking Coach 🎙️🤖

Ứng dụng luyện nói tiếng Anh tương tác với Trí tuệ Nhân tạo (AI English Speaking Coach) - Đồ án cá nhân.

## 🏗️ Kiến trúc Hệ thống (Multi-Client Architecture)

- **Backend**: C# ASP.NET Core 8 Web API + Entity Framework Core + SQL Server
- **Frontend Web**: React Web (Vite + TailwindCSS)
- **Mobile App**: Flutter App (Android / iOS)
- **AI Core**: Gemini 1.5 Flash API (Structured JSON Feedback)
- **Authentication**: Firebase Authentication (Google Sign-In / Email)

## 📁 Cấu trúc Thư mục

```plaintext
├── backend_api/    # C# ASP.NET Core Web API Server
├── tieng_anh_ai/   # Flutter Mobile App
└── frontend_web/   # React Web App (Sẽ khởi tạo ở bước tiếp theo)
```

## 🌿 Quy tắc Git Branching & Commit

- `main`: Code ổn định, production-ready.
- `develop`: Nhánh phát triển chính.
- `feature/*`: Các nhánh tính năng (vd: `feature/initial-project-setup`).
- Format Commit Message: `<type>: <short description>` (`feat`, `fix`, `docs`, `refactor`, `test`, `chore`).
