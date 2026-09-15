# Biblioteca API

API RESTful em **.NET 8 (ASP.NET Core + Entity Framework Core)** com banco **PostgreSQL**, empacotada com **Docker** e **Docker Compose**.

Trabalho da disciplina Programação Concorrente e Distribuída — Entrega 1 (Opção 1).

## Autor e Docker Hub

Trabalho individual.

| Aluno | RA | Docker Hub |
|-------|----|------------|
| Marcos Felipe Silva Celestino | 2189168 | https://hub.docker.com/r/mxrcos/biblioteca-api |

Responsável por todas as etapas: modelagem dos recursos, implementação da API REST (controllers de Autores e Livros), documentação OpenAPI, Dockerfile, Docker Compose e publicação da imagem no Docker Hub.

## Recursos

### Autor
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | int | gerado |
| nome | string (150) | sim |
| nacionalidade | string (80) | não |
| dataNascimento | date (`yyyy-MM-dd`) | não |

### Livro
| Campo | Tipo | Obrigatório |
|-------|------|-------------|
| id | int | gerado |
| titulo | string (200) | sim |
| isbn | string (20) | não |
| anoPublicacao | int | sim |
| paginas | int | sim |
| autorId | int (FK → Autor) | sim |

Um **Autor** possui vários **Livros** (1:N). Ao remover um autor, seus livros são removidos (cascade).

## Endpoints

| Método | Rota | Operação |
|--------|------|----------|
| GET | `/api/autores` | Listar autores |
| GET | `/api/autores/{id}` | Detalhar autor |
| GET | `/api/autores/{id}/livros` | Listar livros do autor |
| POST | `/api/autores` | Criar autor |
| PUT | `/api/autores/{id}` | Atualização total |
| PATCH | `/api/autores/{id}` | Atualização parcial |
| DELETE | `/api/autores/{id}` | Remover autor |
| GET | `/api/livros` | Listar livros |
| GET | `/api/livros/{id}` | Detalhar livro |
| POST | `/api/livros` | Criar livro |
| PUT | `/api/livros/{id}` | Atualização total |
| PATCH | `/api/livros/{id}` | Atualização parcial |
| DELETE | `/api/livros/{id}` | Remover livro |

Documentação OpenAPI (Swagger):
- UI: http://localhost:8080/swagger
- JSON: http://localhost:8080/swagger/v1/swagger.json

## Como executar

```bash
docker compose up --build
```

Serviços:
- `api` — API REST (porta 8080)
- `db` — PostgreSQL 16 (acessível apenas pela rede interna do compose, volume `pgdata`)

A API aguarda o healthcheck do banco e cria as tabelas automaticamente na inicialização.

### Exemplos

```bash
curl -X POST http://localhost:8080/api/autores \
  -H "Content-Type: application/json" \
  -d '{"nome":"Machado de Assis","nacionalidade":"Brasileira","dataNascimento":"1839-06-21"}'

curl -X POST http://localhost:8080/api/livros \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Dom Casmurro","isbn":"9788535910663","anoPublicacao":1899,"paginas":256,"autorId":1}'

curl -X PATCH http://localhost:8080/api/livros/1 \
  -H "Content-Type: application/json" \
  -d '{"paginas":300}'

curl http://localhost:8080/api/autores/1/livros
curl -X DELETE http://localhost:8080/api/livros/1
```

## Publicar no Docker Hub

Comandos usados para publicar a imagem:

```bash
docker login
docker build -t mxrcos/biblioteca-api:latest .
docker push mxrcos/biblioteca-api:latest
```

Ou usando o compose (a variável define o nome da imagem):

```bash
DOCKERHUB_USER=mxrcos docker compose build
DOCKERHUB_USER=mxrcos docker compose push api
```
