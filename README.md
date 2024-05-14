# Task timer

## Sobre o projeto
Temporizador de tarefas com cálculo da média de tempo gasto por tarefa com autenticação de usuários.

## Features

### Users
Utiliza o Microsoft Identity e JWT para lidar com o registro, login, logout e refresh token de usuário.
- Criar usuário: recebe username, email e senha;
- Login: retorna JWT;
- Logout.

### Categorias
- Criar categoria - associada ao id do usuário logado que foi passado no JWT;
- Editar categoria;
- Deletar categoria;
- Listar categorias - retorna apenas as categorias associadas ao id do usuário logado.

### Tarefas
Retorna apenas tarefas associadas ao usuário logado.
- Listar todas as tarefas, tarefas por categoria e tarefa por id;
- Criar tarefa manualmente: com início e fim informados pelo cliente;
- Inicia tarefa: recebe os dados e horário de início informados pelo cliente;
- Encerra tarefa: recebe id da tarefa e horário de encerramento informados pelo cliente;
- Tempo médio das tarefas por categoria.

## Sobre
API desenvolvida em ASP.NET Core, utilizando PostgreSQL e Entity Framework Core e documentação gerada pelo Swagger utilizando a SwaggerUI.

Foram utilizados os padrões repository e unit of work garantindo o desacoplamento dos controllers da conexão com o banco de dados.

**Para Testar:**

1. **Clone o repositório**: Primeiro, o usuário precisa clonar o repositório do GitHub para a máquina local usando o comando `git clone`.

```bash
git clone https://github.com/carolrochafloro/task-timer.git
```

2. **Navegue até o diretório do projeto**: Use o comando `cd` para navegar até o diretório do projeto.

```bash
cd task-timer/task-timer
```

3. **Baixe as dependências**: O comando `dotnet restore` baixa todas as dependências necessárias para o projeto.

```bash
dotnet restore
```

4. **Crie a migração inicial**: O comando `dotnet ef migrations add FirstMigration` cria a primeira migração.

```bash
dotnet ef migrations add FirstMigration
```

5. **Atualize o banco de dados**: O comando `dotnet ef database update` aplica a migração ao banco de dados.

```bash
dotnet ef database update
```

6. **Configure o secrets.json**: conforme o modelo `AppSecrets.cs` configure as variáveis de ambiente para execução do projeto.

7. **Construa o projeto**: O comando `dotnet build` compila o projeto e verifica se há algum erro.

```bash
dotnet build
```

8. **Execute o projeto**: Finalmente, o comando `dotnet run` inicia o servidor e executa o projeto. Se o projeto estiver configurado para usar HTTPS, ele será executado em HTTPS por padrão.

```bash
dotnet run
```

9. **Acesse a API**: Agora a API deve estar rodando e pode ser acessada no navegador em `https://localhost:5432`. Será utilizada a SwaggerUI para acessar as rotas, fornecendo os schemas necessários.

## Backlog
- Testes unitários
- Logging
- Paginação de resultados
- Swagger annotations 

