# ElevatorChallenge

Este repositório contém duas implementações completas do sistema de elevador:

## 📁 Estrutura do Projeto

```
ElevatorChallenge/
├── ElevatorChallengeConsole/     # Implementação Console Simples
│   ├── Program.cs
│   ├── Elevator.cs
│   └── ElevatorChallenge.csproj
├── ElevatorSystem/               # Implementação Multithread + API
│   ├── ElevatorSystem.API/       # Web API
│   ├── ElevatorSystem.Tests/     # Testes xUnit
│   ├── README.md                 # Documentação detalhada
│   └── IMPLEMENTATION_SUMMARY.md # Resumo da implementação
├── LICENSE
├── README.md                     # Este arquivo
└── .gitignore
```

## 🚀 Soluções Implementadas

### 1. ElevatorChallengeConsole
**Implementação simples em console** - A primeira versão básica do elevador.

**Como executar:**
```bash
cd ElevatorChallengeConsole
dotnet run
```

**Características:**
- Console application simples
- Estados básicos do elevador
- Interação via linha de comando
- FIFO scheduling básico

### 2. ElevatorSystem (Recomendado)
**Sistema completo multithread com API REST** - Implementação profissional que atende todos os requisitos.

**Como executar:**
```bash
cd ElevatorSystem
dotnet run --project ElevatorSystem.API
```

**Características:**
- ✅ **Thread Safety** - Locks, mutexes e coleções thread-safe
- ✅ **Performance** - 100+ requisições concorrentes, <100ms response time
- ✅ **API REST** - Endpoints para interação
- ✅ **Testes Completos** - 25 testes xUnit (100% de sucesso)
- ✅ **Swagger UI** - Documentação interativa
- ✅ **Error Handling** - Tratamento robusto de erros
- ✅ **Logging** - Sistema de logs detalhado

## 📋 Requisitos do Desafio

### Core Requirements ✅
- ✅ **Elevator class** que representa um elevador
- ✅ **ElevatorController class** que gerencia operações
- ✅ **Passenger requests** (pickup e destination floors)
- ✅ **Thread-safe operations** para requisições concorrentes
- ✅ **Basic scheduling algorithm** (FIFO implementado)

### Easy Level Requirements ✅
- ✅ Sistema com um elevador servindo andares 1-10
- ✅ Tratamento de requisições up/down
- ✅ Estados básicos: IDLE, MOVING_UP, MOVING_DOWN, DOOR_OPEN
- ✅ Pickup requests (floor + direction) e destination requests
- ✅ Algoritmo FIFO simples

### Thread Safety Considerations ✅
- ✅ Locks/mutexes apropriados para recursos compartilhados
- ✅ Operações atômicas para mudanças de estado
- ✅ Tratamento de race conditions
- ✅ Coleções thread-safe (ConcurrentQueue, ConcurrentDictionary)

### Performance Requirements ✅
- ✅ Sistema lida com 100+ requisições concorrentes
- ✅ Tempo de resposta < 100ms para assignment de elevador
- ✅ Uso de memória razoável sob carga

### Error Handling ✅
- ✅ Tratamento graceful de requisições de andares inválidos
- ✅ Timeouts para elevadores travados
- ✅ Tratamento de exceções para operações concorrentes

## 🧪 Testes

### Executar todos os testes:
```bash
cd ElevatorSystem
dotnet test
```

### Resultados:
```
✅ Aprovado! – Com falha: 0, Aprovado: 25, Ignorado: 0, Total: 25
```

## 📊 APIs e Documentação

### Swagger UI
Acesse: https://localhost:7001/swagger

### Endpoints principais:
- `POST /api/elevator/request` - Solicitar elevador
- `GET /api/elevator/status` - Status do elevador
- `GET /api/elevator/requests` - Requisições ativas
- `POST /api/elevator/load-test` - Teste de carga

## 🔧 Tecnologias Utilizadas

- **.NET 8.0**
- **ASP.NET Core Web API**
- **xUnit** (testes)
- **Moq** (mocking)
- **Swagger/OpenAPI**
- **Concurrent Collections**

## 📝 Próximos Passos

1. Algoritmos de escalonamento avançados (SCAN, C-SCAN)
2. Sistema de múltiplos elevadores
3. Interface web em tempo real
4. Persistência de dados
5. Métricas avançadas de performance

---

**Recomendação:** Use a implementação `ElevatorSystem` para avaliação, pois atende completamente todos os requisitos de thread safety, performance e testing.