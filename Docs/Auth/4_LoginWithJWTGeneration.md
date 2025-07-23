```mermaid
sequenceDiagram
    participant C as Cliente
    participant API as API Server
    participant IM as Identity Framework
    participant DB as Database
    participant JWT as Token Service

    Note over C,JWT: LOGIN SOLO DESPUÉS DE VERIFICACIÓN DE EMAIL

    C->>+API: POST /api/auth/login
    Note over C,API: Content-Type: application/json<br/>{<br/>  "email": "user@example.com",<br/>  "password": "Test123!",<br/>  "rememberMe": false<br/>}

    API->>+IM: FindByEmailAsync(email)
    IM->>+DB: SELECT User WHERE Email = email
    DB-->>-IM: User entity
    IM-->>-API: User entity

    alt User Not Found
        API-->>C: 401 Unauthorized<br/>{"message": "Credenciales inválidas", "data": null}
    end

    alt Email Not Confirmed
        Note over API: if (!user.EmailConfirmed)
        API-->>C: 401 Unauthorized<br/>{"message": "Debes verificar tu email antes de iniciar sesión", "data": null}
    end

    API->>+IM: CheckPasswordAsync(user, password)
    IM-->>-API: bool isPasswordValid

    alt Invalid Password
        API-->>C: 401 Unauthorized<br/>{"message": "Credenciales inválidas", "data": null}
    end

    Note over API: CREDENCIALES VÁLIDAS - GENERAR JWT

    API->>+IM: GetRolesAsync(user)
    IM-->>-API: List<string> roles (["Customer"])

    API->>+JWT: GenerateToken(user, roles, rememberMe)
    Note over JWT: JWT CLAIMS:<br/>{<br/>  "nameid": "user.Id",<br/>  "email": "user@example.com",<br/>  "role": "Customer",<br/>  "exp": timestamp,<br/>  "iat": timestamp<br/>}<br/><br/>EXPIRY:<br/>rememberMe ? 24 hours : 1 hour<br/><br/>ALGORITHM: HS256

    JWT-->>-API: JWT token string

    API-->>-C: 200 OK<br/>{"message": "Inicio de sesión exitoso", "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."}

    Note over C: CLIENTE GUARDA TOKEN PARA REQUESTS FUTUROS<br/>Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```
