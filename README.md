# Authy 🔐

A lightweight authentication microservice for my game backend.

Nothing fancy — register, login, get a JWT, move on with your life.

Built as an authentication stub for [Aucy](https://github.com/Quaximur/Aucy) and other backend services behind [Gaty](https://github.com/Quaximur/Gaty).

## 🛠️ Tech Stack

[![.NET](https://img.shields.io/badge/10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://github.com/dotnet/runtime)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://github.com/dotnet/aspnetcore)
[![MagicOnion](https://img.shields.io/badge/MagicOnion-512BD4?style=for-the-badge)](https://github.com/Cysharp/MagicOnion)
[![gRPC](https://img.shields.io/badge/gRPC-244C5A?style=for-the-badge&logo=google&logoColor=white)](https://github.com/grpc/grpc-dotnet)
[![MessagePack](https://img.shields.io/badge/MessagePack-000000?style=for-the-badge)](https://github.com/MessagePack-CSharp/MessagePack-CSharp)

[![Entity Framework Core](https://img.shields.io/badge/EF_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://github.com/dotnet/efcore)
[![Npgsql](https://img.shields.io/badge/Npgsql-EF_Core-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://github.com/npgsql/efcore.pg)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://github.com/postgres/postgres)

## ⚡ What's Inside

- **Registration** — creates users and rejects duplicate usernames or emails.
- **Login** — authenticates by username or email.
- **JWT Tokens** — signed tokens carry the user's `user_id`.
- **MagicOnion API** — authentication is exposed through lightweight gRPC unary services.
- **PostgreSQL Storage** — users are persisted through Entity Framework Core.
- **Password Validation** — basic password requirements are checked during registration.
- **Development Tokens** — development builds expose a tiny endpoint for generating a token for a user ID.

## 🧩 Microservice Backend

Authy sits behind [Gaty](https://github.com/Quaximur/Gaty):

```text
                    ┌─────────────┐
                    │   Client    │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │    Gaty     │
                    │   Gateway   │
                    └──────┬──────┘
                           │ /authy/*
                           ▼
                    ┌─────────────┐
                    │    Authy    │
                    │             │
                    │ Register    │
                    │ Login       │
                    │ JWT         │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │ PostgreSQL  │
                    │    Users    │
                    └─────────────┘
```

Authentication routes are public at the gateway level. After login, the issued JWT can be used to access protected services such as [Aucy](https://github.com/Quaximur/Aucy).

## 🧱 Project Structure

```text
Authy/
├── API/            # Host, authentication setup and MagicOnion endpoints
├── Application/    # Login, registration, JWT and authentication logic
├── Contracts/      # MagicOnion contracts and DTOs
├── Data/           # EF Core and PostgreSQL persistence
└── Domain/         # User domain models
```

## 🚀 Running

Start PostgreSQL:

```bash
docker compose up -d
```

Configure the database connection and JWT secret, then run:

```bash
dotnet run --project API/Authy.API.csproj
```

Authy listens over HTTP/2 and is intended to be accessed through [Gaty](https://github.com/Quaximur/Gaty).
